using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeldaOracle.Common.Scripts {
	
	public class ScriptCommand {
		private string name;
		private Action<CommandParam> action;

		private List<CommandParam> parameterOverloads;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ScriptCommand(string name, Action<CommandParam> action) {
			this.name		= name;
			this.action		= action;
			this.parameterOverloads	= new List<CommandParam>();
		}

		public ScriptCommand(string name, string[] parameterOverloads, Action<CommandParam> action) :
			this(name, action)
		{
			CommandParamFormatParser parser = new CommandParamFormatParser();

			for (int i = 0; i < parameterOverloads.Length; i++) {
				CommandParam p = parser.Parse(parameterOverloads[i]);
				p.Name = parameterOverloads[i];
				this.parameterOverloads.Add(p);
			}
		}

		

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public bool HasName(string commandName) {
			return (String.Compare(commandName, name, StringComparison.OrdinalIgnoreCase) == 0);
		}
		
		public bool HasParameters(CommandParam userParameters, out CommandParam newParameters) {
			if (parameterOverloads.Count == 0) {
				newParameters = new CommandParam(userParameters);
				return true;
			}
			for (int i = 0; i < parameterOverloads.Count; i++) {
				if (AreParametersMatching(parameterOverloads[i], userParameters, out newParameters))
					return true;
			}
			newParameters = null;
			return false;
		}

		/*public bool IsCommand(string commandName, CommandParam userParameters) {
			// Check if name and parameters are matching.
			return (HasName(commandName) && HasParameters(userParameters));
		}*/

		

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		private static bool AreParametersMatching(CommandParam reference, CommandParam userParams, out CommandParam newParameters) {
			newParameters = null;

			if (reference == null) {
				newParameters = new CommandParam(userParams);
				return true;
			}
			if (!userParams.IsValidType(reference.Type)) {
				newParameters = null;
				return false;
			}

			// Make sure arrays are matching.
			if (reference.Type == CommandParamType.Array) {
				newParameters = new CommandParam(CommandParamType.Array);
				newParameters.Name = reference.Name;

				// Find the child index of the first parameter with a default value.
				int defaultIndex = 0;
				for (CommandParam p = reference.Children; p != null; p = p.NextParam) {
					if (p.DefaultValue != null)
						break;
					defaultIndex++;
				}

				// Verify the user parameter's child count is within the valid range.
				if (userParams.Count < defaultIndex || userParams.Count > reference.Count) {
					newParameters = null;
					return false;
				}

				// Verify each child paremeter matches the reference.
				CommandParam referenceChild = reference.Children;
				CommandParam userChild = userParams.Children;
				for (int i = 0; i < reference.Count; i++) {
					CommandParam newChild;
					if (i < userParams.Count) {
						if (!AreParametersMatching(referenceChild, userChild, out newChild)) {
							newParameters = null;
							return false;
						}
						userChild = userChild.NextParam;
					}
					else {
						newChild = new CommandParam(referenceChild.DefaultValue);
					}
					newParameters.AddChild(newChild);
					referenceChild = referenceChild.NextParam;
				}
			}
			else {
				newParameters = new CommandParam(reference);
				newParameters.Name = reference.Name;
				newParameters.SetValueByParse(userParams.Str);
			}

			return true;
		}

		private static CommandParam ParseParameterFormat(string parametersFormat) {
			int endCharIndex;
			return ParseParameterFormat(parametersFormat, out endCharIndex);
		}

		private static CommandParam ParseParameterFormat(string parametersFormat, out int endCharIndex) {
			CommandParam parameters = new CommandParam();
			CommandParam lastChild = null;
			parameters.Type = CommandParamType.Array;

			List<string> words = new List<string>();
			string word = "";

			endCharIndex = parametersFormat.Length;

			bool parsingDefaultValue = false;

			for (int i = 0; i < parametersFormat.Length; i++) {
				char c = parametersFormat[i];
				
				// Whitespace
				if (c == ' ') {
					if (word.Length > 0 && !parsingDefaultValue) {
						words.Add(word);
						word = "";
					}
				}

				// Parameter delimiter (comma)
				else if (c == ',') {
					if (parsingDefaultValue) {
						lastChild.SetValueByParse(word);
					}
					else {
						if (word.Length > 0) {
							words.Add(word);
							word = "";
						}
						if (words.Count == 2) {
							lastChild = parameters.AddChild(new CommandParam() {
								Type = ParseCommandParamType(words[0]),
								Name = words[1]
							});
							words.Clear();
						}
					}
				}

				// Parameter array begin
				else if (c == '(') {
					// Read array params, and skip to the closing parenthesis.
					int skipAmount = 0;
					lastChild = parameters.AddChild(ParseParameterFormat(
						parametersFormat.Substring(i + 1), out skipAmount));
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
					if (!parsingDefaultValue) {
						parsingDefaultValue = true;

						if (word.Length > 0) {
							words.Add(word);
							word = "";
						}
						if (words.Count == 2) {
							parameters.AddChild(new CommandParam() {
								Type = ParseCommandParamType(words[0]),
								Name = words[1]
							});
							words.Clear();
						}
					}
				}
				//else if (c == '.') {
				//}
				else {
					word += c;
				}
			}

			if (word.Length > 0) {
				words.Add(word);
				word = "";
			}
			if (words.Count > 0) {
				parameters.AddChild(new CommandParam() {
					Type = ParseCommandParamType(words[0]),
					Name = words[1]
				});
			}

			return parameters;
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

		
		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public string Name {
			get { return name; }
		}

		public Action<CommandParam> Action {
			get { return action; }
		}

		public List<CommandParam> ParameterOverloads {
			get { return parameterOverloads; }
		}
	}
}
