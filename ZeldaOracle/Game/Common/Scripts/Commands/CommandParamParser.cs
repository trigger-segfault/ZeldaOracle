using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeldaOracle.Common.Scripts {

	public static class CommandParamParser {
		
		//-----------------------------------------------------------------------------
		// Command Parameter ToString
		//-----------------------------------------------------------------------------
		
		// Returns a string representation of a reference command parameter.
		public static string ToString(CommandReferenceParam param) {
			string result = "";

			if (param.Type == CommandParamType.Array) {
				result += "(";
				CommandReferenceParam child = param.Children;
				while (child != null) {
					result += ToString(child);
					if (child.NextParam != null)
						result += ", ";
					child = child.NextParam;
				}
				result += ")";
			}
			else {
				if (param.Type == CommandParamType.String)
					result += "string";
				else if (param.Type == CommandParamType.Integer)
					result += "int";
				else if (param.Type == CommandParamType.Float)
					result += "float";
				else if (param.Type == CommandParamType.Boolean)
					result += "bool";
				result += " " + param.Name;
			}

			// Append the default value.
			if (param.DefaultValue != null) {
				result += " = " + ToString(param.DefaultValue);
			}

			return result;
		}
		
		// Returns a string representation of a value command parameter.
		public static string ToString(CommandParam param) {
			string result = "";

			if (param.Type == CommandParamType.Array) {
				result += "(";
				CommandParam child = param.Children;
				while (child != null) {
					result += ToString(child);
					if (child.NextParam != null)
						result += ", ";
					child = child.NextParam;
				}
				result += ")";
			}
			else {
				if (param.Type == CommandParamType.String)
					result += "\"" + param.StringValue + "\"";
				else if (param.Type == CommandParamType.Integer)
					result += param.IntValue;
				else if (param.Type == CommandParamType.Float)
					result += param.FloatValue;
				else if (param.Type == CommandParamType.Boolean)
					result += (param.BoolValue ? "true" : "false");
			}

			return result;
		}

				
		//-----------------------------------------------------------------------------
		// Command Parameter Parsing
		//-----------------------------------------------------------------------------

		// Parse a string as value parameters.
		public static CommandParam ParseValueParams(string format) {
			return ParseParameters(format, true).ValParamStack.Peek();
		}
		
		// Parse a string as reference parameters.
		public static CommandReferenceParam ParseReferenceParams(string format) {
			return ParseParameters(format, false).RefParamStack.Peek();
		}
		

		//-----------------------------------------------------------------------------
		// Internal Parsing
		//-----------------------------------------------------------------------------

		// Internal data used to parse parameter hierarchies.
		private class ParseData {
			public Stack<CommandReferenceParam> RefParamStack { get; set; }
			public Stack<CommandParam> ValParamStack { get; set; }
			public List<string> Tokens { get; set; }
			public string Token { get; set; }
			public bool IsParsingValue { get; set; }
			public bool IsParsingDefaultValue { get; set; }
		}

		private static ParseData ParseParameters(string format, bool parseValue) {
			ParseData parseData = new ParseData();
			parseData.RefParamStack		= new Stack<CommandReferenceParam>();
			parseData.ValParamStack		= new Stack<CommandParam>();
			parseData.Tokens			= new List<string>();
			parseData.Token				= "";
			parseData.IsParsingValue	= parseValue;

			// Create the root parameter.
			if (parseData.IsParsingValue)
				parseData.ValParamStack.Push(new CommandParam(CommandParamType.Array));
			else
				parseData.RefParamStack.Push(new CommandReferenceParam(CommandParamType.Array));

			// Parse the format, character by character.
			bool quotes = false;
			for (int i = 0; i < format.Length; i++) {
				char c = format[i];
				
				// Parse strings between quotes.
				if (quotes) {
					if (c == '"') {
						quotes = false;
						CompleteToken(parseData);
					}
					else
						parseData.Token += c;
				}
				else if (c == '"')
					quotes = true;

				// Whitespace.
				else if (c == ' ' || c == '\t')
					CompleteToken(parseData);

				// Parameter delimiter (comma)
				else if (c == ',') {
					CompleteToken(parseData);
					CompleteDefaultValue(parseData);
				}

				// Parameter array begin
				else if (c == '(') {
					if (parseData.IsParsingValue) {
						if (parseData.IsParsingDefaultValue) {
							parseData.RefParamStack.Push(parseData.RefParamStack.Peek()
								.GetChildren().ElementAt(parseData.ValParamStack.Peek().ChildCount));
						}
						parseData.ValParamStack.Push(parseData.ValParamStack.Peek()
							.AddChild(new CommandParam(CommandParamType.Array)));
					}
					else {
						parseData.RefParamStack.Push(parseData.RefParamStack.Peek()
							.AddChild(new CommandReferenceParam(CommandParamType.Array)));
					}
				}

				// Parameter array end.
				else if (c == ')') {
					CompleteToken(parseData);
					if (parseData.IsParsingValue)
						parseData.ValParamStack.Pop();
					if (!parseData.IsParsingValue || (parseData.IsParsingValue && parseData.IsParsingDefaultValue))
						parseData.RefParamStack.Pop();
				}

				// Default parameter value.
				else if (c == '=') {
					parseData.IsParsingValue = true;
					parseData.IsParsingDefaultValue = true;
					CommandReferenceParam refParent = new CommandReferenceParam(CommandParamType.Array);
					refParent.AddChild(parseData.RefParamStack.Peek().GetChildren().Last());
					parseData.RefParamStack.Push(refParent);
					parseData.ValParamStack.Push(new CommandParam(CommandParamType.Array));
				}
				//else if (c == '.') { TODO: variadic parameters.
				//}
				else {
					parseData.Token += c;
				}
			}
			
			CompleteToken(parseData);
			CompleteDefaultValue(parseData);

			return parseData;
		}

		private static void CompleteToken(ParseData data) {
			if (data.Token.Length > 0) {
				data.Tokens.Add(data.Token);

				if (data.IsParsingValue) {
					CommandParam p;
					if (data.IsParsingDefaultValue) {
						CommandReferenceParam refParam = data.RefParamStack
							.Peek().GetChildren().ElementAt(data.ValParamStack.Peek().ChildCount);
						p = new CommandParam(refParam);
						p.SetValueByParse(data.Tokens[0]);
					}
					else {
						p = new CommandParam(data.Tokens[0]);
					}
					data.ValParamStack.Peek().AddChild(p);
					data.Tokens.Clear();
				}
				else {
					if (data.Tokens.Count == 2) {
						data.RefParamStack.Peek().AddChild(new CommandReferenceParam() {
							Type = ParseCommandParamType(data.Tokens[0]),
							Name = data.Tokens[1]
						});
						data.Tokens.Clear();
					}
				}

				data.Token = "";
			}
		}

		private static void CompleteDefaultValue(ParseData data) {
			// Finish parsing the default value.
			if (data.IsParsingValue && data.IsParsingDefaultValue && data.ValParamStack.Count == 1) {
				data.IsParsingValue = false;
				data.IsParsingDefaultValue = false;
				data.RefParamStack.Pop();
				data.RefParamStack.Peek().GetChildren().Last().DefaultValue = 
					data.ValParamStack.Peek().Children;
				data.ValParamStack.Clear();
			}
		}

		private static CommandParamType ParseCommandParamType(string typeName) {
			typeName = typeName.ToLower();
			if (typeName == "int" || typeName == "integer")
				return CommandParamType.Integer;
			if (typeName == "bool" || typeName == "boolean")
				return CommandParamType.Boolean;
			if (typeName == "float")
				return CommandParamType.Float;
			if (typeName == "string" || typeName == "str")
				return CommandParamType.String;
			if (typeName == "any")
				return CommandParamType.Any;
			return CommandParamType.Unknown;
		}
	}
}
