using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeldaOracle.Common.Scripts.Commands {

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
				if (param.Type == CommandParamType.Const)
					result += "const";
				else if (param.Type == CommandParamType.String)
					result += "string";
				else if (param.Type == CommandParamType.Integer)
					result += "int";
				else if (param.Type == CommandParamType.Float)
					result += "float";
				else if (param.Type == CommandParamType.Boolean)
					result += "bool";
				else if (param.Type == CommandParamType.Any)
					result += "var";
				else if (param.HasCustomType)
					result += param.CustomTypeName;
				result += " " + param.Name;
			}

			// Show the variadic ellipses.
			if (param.IsVariadic)
				result += "...";

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
				if (!string.IsNullOrEmpty(param.Name))
					result += param.Name + " ";
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
				if (param.Type == CommandParamType.String || param.Type == CommandParamType.Any)
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
		public static CommandParam ParseValueParams(string format, CommandParamDefinitions typeDefinitions) {
			return ParseParameters(format, true, typeDefinitions).ValParamStack.Peek();
		}
		
		// Parse a string as reference parameters.
		public static CommandReferenceParam ParseReferenceParams(string format, CommandParamDefinitions typeDefinitions) {
			return ParseParameters(format, false, typeDefinitions).RefParamStack.Peek();
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
			public CommandParamDefinitions TypeDefinitions { get; set; }
			public bool IsParsingCustom { get; set; }
		}

		private static ParseData ParseParameters(string format, bool parseValue, CommandParamDefinitions typeDefinitions) {
			ParseData parseData = new ParseData();
			parseData.RefParamStack		= new Stack<CommandReferenceParam>();
			parseData.ValParamStack		= new Stack<CommandParam>();
			parseData.Tokens			= new List<string>();
			parseData.Token				= "";
			parseData.IsParsingValue	= parseValue;
			parseData.TypeDefinitions	= typeDefinitions;
			parseData.IsParsingCustom	= false;

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
						var param = new CommandReferenceParam(CommandParamType.Array);
						if (parseData.Tokens.Count == 1) {
							param.Name = parseData.Tokens[0];
							parseData.Tokens.Clear();
						}
						parseData.RefParamStack.Push(parseData.RefParamStack.Peek()
							.AddChild(param));
					}
				}

				// Parameter array end.
				else if (c == ')') {
					CompleteToken(parseData);
					CompleteDefaultValue(parseData);
					if (parseData.IsParsingValue)
						parseData.ValParamStack.Pop();
					if (!parseData.IsParsingValue || (parseData.IsParsingValue && parseData.IsParsingDefaultValue)) {
						parseData.RefParamStack.Pop();

					}
				}

				// Default parameter value.
				else if (c == '=') {
					var refChild = parseData.RefParamStack.Peek().GetChildren().Last();
					parseData.IsParsingValue = true;
					parseData.IsParsingDefaultValue = true;
					parseData.IsParsingCustom = refChild.HasCustomType;
					CommandReferenceParam refParent = new CommandReferenceParam(CommandParamType.Array);
					refParent.AddChild(refChild);
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
			bool isVariadic = false;

			if (data.Token.EndsWith("...")) {
				data.Token = data.Token.Substring(0, data.Token.Length - 3);
				isVariadic = true;
			}

			if (data.Token.Length > 0 || isVariadic) {
				if (data.Token.Length > 0)
					data.Tokens.Add(data.Token);

				if (data.IsParsingValue && data.Token.Length > 0) {
					CommandParam p;
					if (data.IsParsingDefaultValue && !data.IsParsingCustom) {
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
				if (!data.IsParsingValue) {
					if (data.Token.Length > 0 && data.Tokens.Count == 2) {
						var refChild = new CommandReferenceParam() {
							Type        = ParseCommandParamType(data.Tokens[0], data.TypeDefinitions),
							Name        = data.Tokens[1],
							IsVariadic  = isVariadic
						};
						if (refChild.Type == CommandParamType.Unknown)
							throw new Exception("Unknown parameter type '" + data.Tokens[0] + "'!");
						if (data.TypeDefinitions.Contains(data.Tokens[0]))
							refChild.CustomTypeName = data.Tokens[0];
						data.RefParamStack.Peek().AddChild(refChild);
						data.Tokens.Clear();
					}
					else if (isVariadic) {
						data.RefParamStack.Peek().GetChildren().Last().IsVariadic = true;
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
				CommandReferenceParam refParam = data.RefParamStack.Peek().GetChildren().Last();
				refParam.DefaultValue =  data.ValParamStack.Peek().Children;
				data.ValParamStack.Clear();
				if (data.IsParsingCustom) {
					CommandParamDefinition def = data.TypeDefinitions.Get(refParam.CustomTypeName);
					CommandParam newDefaultParams = null;
					for (int i = 0; i < def.ParameterOverloads.Count; i++) {
						if (ScriptCommand.AreParametersMatching(def.ParameterOverloads[i].Children, refParam.DefaultValue, out newDefaultParams, data.TypeDefinitions))
							break;
					}
					if (newDefaultParams != null)
						refParam.DefaultValue = newDefaultParams;
				}
				data.IsParsingCustom = false;
			}
		}

		private static CommandParamType ParseCommandParamType(string typeName, CommandParamDefinitions typeDefinitions) {
			// Custom types should match casing
			CommandParamDefinition def = typeDefinitions.Get(typeName);
			if (def != null) return CommandParamType.Custom;

			typeName = typeName.ToLower();
			if (typeName == "int" || typeName == "integer")
				return CommandParamType.Integer;
			if (typeName == "bool" || typeName == "boolean")
				return CommandParamType.Boolean;
			if (typeName == "float")
				return CommandParamType.Float;
			if (typeName == "string" || typeName == "str")
				return CommandParamType.String;
			if (typeName == "const")
				return CommandParamType.Const;
			if (typeName == "any" || typeName == "var" || typeName == "?")
				return CommandParamType.Any;
			return CommandParamType.Unknown;
		}
	}
}
