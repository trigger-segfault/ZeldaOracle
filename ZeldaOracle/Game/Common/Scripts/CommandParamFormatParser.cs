using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeldaOracle.Common.Scripts {

	public class CommandParamFormatParser {
		
		private string word;
		private List<string> words;
		private bool isParsingDefaultValue;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public CommandParamFormatParser() {
			word	= "";
			words	= new List<string>();
		}


		//-----------------------------------------------------------------------------
		// Parsing Methods
		//-----------------------------------------------------------------------------

		public CommandParam Parse(string format) {
			int endCharIndex;
			isParsingDefaultValue = false;
			return ParseSub(format, out endCharIndex);
		}

		private CommandParam GetLastChild(CommandParam param) {
			if (param.Children == null)
				return null;
			CommandParam child = param.Children;
			while (child.NextParam != null)
				child = child.NextParam;
			return child;
		}

		private void CompleteWord(CommandParam parentParam, CommandParam referenceParentParam) {
			if (word.Length > 0) {
				words.Add(word);

				if (isParsingDefaultValue) {
					if (words.Count == 1) {
						CommandParam referenceParentChild = null;
						if (isParsingDefaultValue && referenceParentParam != null && referenceParentParam.Children != null) {
							CommandParam tempParam = parentParam.Children;
							referenceParentChild = referenceParentParam.Children;
							while (tempParam != null && referenceParentChild.NextParam != null) {
								tempParam = tempParam.NextParam;
								referenceParentChild = referenceParentChild.NextParam;
							}
						}

						CommandParam p = new CommandParam(referenceParentChild.Type);
						p.Name = referenceParentChild.Name;
						p.SetValueByParse(words[0]);
						parentParam.AddChild(p);
						words.Clear();
						word = "";
						words.Clear();
					}
				}
				else {
					if (words.Count == 2) {
						parentParam.AddChild(new CommandParam() {
							Type = ParseCommandParamType(words[0]),
							Name = words[1]
						});
						words.Clear();
					}
				}

				word = "";
			}
		}

		private CommandParam ParseSub(string format, out int endCharIndex, bool isDefault = false, CommandParam referenceParam = null) {
			CommandParam parentParam = new CommandParam(CommandParamType.Array);

			bool quotes = false;

			words.Clear();
			word = "";

			endCharIndex = format.Length;

			// Parse the format, character by character.
			for (int i = 0; i < format.Length; i++) {
				char c = format[i];
				
				if (quotes) {
					if (c == '"') {
						quotes = false;
						CompleteWord(parentParam, referenceParam);
					}
					else
						word += c;
				}

				else if (c == '"')
					quotes = true;

				// Whitespace.
				else if (c == ' ' || c == '\t') {
					CompleteWord(parentParam, referenceParam);
				}

				// Parameter delimiter (comma)
				else if (c == ',') {
					if (isDefault) {
						endCharIndex = i + 1;
						break;
					}
					CompleteWord(parentParam, referenceParam);
				}

				// Parameter array begin
				else if (c == '(') {
					CommandParam referenceParentChild = null;
					if (isParsingDefaultValue && referenceParam != null && referenceParam.Children != null) {
						CommandParam p = parentParam.Children;
						referenceParentChild = referenceParam.Children;
						while (p != null && referenceParentChild.NextParam != null) {
							p = p.NextParam;
							referenceParentChild = referenceParentChild.NextParam;
						}
					}
					
					// Read array params recursively, and skip to the closing parenthesis.
					int skipAmount = 0;
					parentParam.AddChild(ParseSub(format.Substring(i + 1), out skipAmount, false, referenceParentChild));
					i += skipAmount;
					word = "";
					words.Clear();
				}

				// Parameter array end.
				else if (c == ')') {
					endCharIndex = i + 1;
					break;
				}

				// Default parameter value.
				else if (c == '=') {
					CompleteWord(parentParam, referenceParam);

					// Recursively parse the default value.
					isParsingDefaultValue = true;
					int skipAmount = 0;
					int endIndex = format.IndexOf(',');
					CommandParam tempReferenceParent = new CommandParam(CommandParamType.Array);
					tempReferenceParent.AddChild(GetLastChild(parentParam));
					CommandParam defaultValue = ParseSub(format.Substring(i + 1), out skipAmount, true, tempReferenceParent);
					i += skipAmount;
					isParsingDefaultValue = false;
					GetLastChild(parentParam).DefaultValue = defaultValue.Children;
				}
				//else if (c == '.') { TODO: variadic parameters.
				//}
				else {
					word += c;
				}
			}

			CompleteWord(parentParam, referenceParam);

			return parentParam;
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
		
		public static string ToString(CommandParam parameterFormat) {
			return ToString(parameterFormat, false);
		}

		private static string ToString(CommandParam param, bool isDefaultValue) {
			string result = "";

			if (param.Type == CommandParamType.Array) {
				result += "(";
				CommandParam child = param.Children;
				while (child != null) {
					result += ToString(child, isDefaultValue);
					if (child.NextParam != null)
						result += ", ";
					child = child.NextParam;
				}
				result += ")";
			}
			else {

				if (isDefaultValue) {
					if (param.Type == CommandParamType.String)
						result += "\"" + param.Str + "\"";
					else if (param.Type == CommandParamType.Integer)
						result += param.Integer;
					else if (param.Type == CommandParamType.Float)
						result += param.Float;
					else if (param.Type == CommandParamType.Boolean)
						result += (param.Boolean ? "true" : "false");
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
			}

			if (!isDefaultValue && param.DefaultValue != null) {
				result += " = " + ToString(param.DefaultValue, true);
			}

			return result;
		}
	}
}
