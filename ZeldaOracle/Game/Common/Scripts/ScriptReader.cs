using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Scripts.Commands;

namespace ZeldaOracle.Common.Scripts {
	/// <summary>
	/// A script reader is an abstract object that
	/// is meant to be implemented to be able to
	/// interpret text files written in a certain syntax.
	/// <summary>
	public class ScriptReader {
		
		private StreamReader	streamReader;
		private string			fileName;
		private List<string>	lines;
		private string			line;			// The string of the current line.
		private int				lineIndex;
		private int				charIndex;
		private int				wordCharIndex;	// Character index for the current word being parsed.
		private string			word;			// The current word being parsed.
		private CommandParam	parameter;
		private CommandParam	parameterParent;
		private CommandParam	parameterRoot;
		private string			commandPrefix;

		private List<ScriptCommand> commands;   // List of possible commands.
		private Dictionary<string, CommandPrefix> commandPrefixes;

		/// <summary>The current mode of the reader used to determine valid commands.</summary>
		private int             mode;

		private TemporaryResources tempResources;

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ScriptReader() {
			parameter		= null;
			parameterRoot	= null;
			commandPrefix   = null;
			commands		= new List<ScriptCommand>();
			commandPrefixes	= new Dictionary<string, CommandPrefix>();
			lines			= new List<string>();
			tempResources   = new TemporaryResources();
		}
		

		//-----------------------------------------------------------------------------
		// Command Creation (No Modes)
		//-----------------------------------------------------------------------------
		
		// Add a command with no parameter format.
		protected void AddCommand(string name, Action<CommandParam> action) {
			AddCommand(name, new string[] {}, action);
		}

		// Add a command with one parameter format.
		protected void AddCommand(string name, string params1, Action<CommandParam> action) {
			AddCommand(name, new string[] { params1 }, action);
		}
		
		// Add a command that handles 2 overloads.
		protected void AddCommand(string name, string params1, string params2, Action<CommandParam> action) {
			AddCommand(name, new string[] { params1, params2 }, action);
		}
		
		// Add a command that handles 3 overloads.
		protected void AddCommand(string name, string params1, string params2, string params3, Action<CommandParam> action) {
			AddCommand(name, new string[] { params1, params2, params3 }, action);
		}
		
		// Add a command that handles 4 overloads.
		protected void AddCommand(string name, string params1, string params2, string params3, string params4, Action<CommandParam> action) {
			AddCommand(name, new string[] { params1, params2, params3, params4 }, action);
		}
		
		// Add a command that handles 5 overloads.
		protected void AddCommand(string name, string params1, string params2, string params3, string params4, string params5, Action<CommandParam> action) {
			AddCommand(name, new string[] { params1, params2, params3, params4, params5 }, action);
		}
		
		// Add a command that handles 6 overloads.
		protected void AddCommand(string name, string params1, string params2, string params3, string params4, string params5, string params6, Action<CommandParam> action) {
			AddCommand(name, new string[] { params1, params2, params3, params4, params5, params6 }, action);
		}
		
		// Add a command that handles the given list of overloads.
		protected void AddCommand(string name, string[] parameterOverloads, Action<CommandParam> action) {
			AddCommand(new ScriptCommand(name, null, parameterOverloads, action));
		}


		//-----------------------------------------------------------------------------
		// Command Creation (Single Mode)
		//-----------------------------------------------------------------------------

		// Add a command with no parameter format.
		protected void AddCommand(string name, int mode, Action<CommandParam> action) {
			AddCommand(name, mode, new string[] { }, action);
		}

		// Add a command with one parameter format.
		protected void AddCommand(string name, int mode, string params1, Action<CommandParam> action) {
			AddCommand(name, mode, new string[] { params1 }, action);
		}

		// Add a command that handles 2 overloads.
		protected void AddCommand(string name, int mode, string params1, string params2, Action<CommandParam> action) {
			AddCommand(name, mode, new string[] { params1, params2 }, action);
		}

		// Add a command that handles 3 overloads.
		protected void AddCommand(string name, int mode, string params1, string params2, string params3, Action<CommandParam> action) {
			AddCommand(name, mode, new string[] { params1, params2, params3 }, action);
		}

		// Add a command that handles 4 overloads.
		protected void AddCommand(string name, int mode, string params1, string params2, string params3, string params4, Action<CommandParam> action) {
			AddCommand(name, mode, new string[] { params1, params2, params3, params4 }, action);
		}

		// Add a command that handles 5 overloads.
		protected void AddCommand(string name, int mode, string params1, string params2, string params3, string params4, string params5, Action<CommandParam> action) {
			AddCommand(name, mode, new string[] { params1, params2, params3, params4, params5 }, action);
		}

		// Add a command that handles 6 overloads.
		protected void AddCommand(string name, int mode, string params1, string params2, string params3, string params4, string params5, string params6, Action<CommandParam> action) {
			AddCommand(name, mode, new string[] { params1, params2, params3, params4, params5, params6 }, action);
		}

		// Add a command that handles the given list of overloads.
		protected void AddCommand(string name, int mode, string[] parameterOverloads, Action<CommandParam> action) {
			AddCommand(new ScriptCommand(name, new int[] { mode }, parameterOverloads, action));
		}


		//-----------------------------------------------------------------------------
		// Command Creation (Multiple Modes)
		//-----------------------------------------------------------------------------

		// Add a command with no parameter format.
		protected void AddCommand(string name, int[] modes, Action<CommandParam> action) {
			AddCommand(name, modes, new string[] { }, action);
		}

		// Add a command with one parameter format.
		protected void AddCommand(string name, int[] modes, string params1, Action<CommandParam> action) {
			AddCommand(name, modes, new string[] { params1 }, action);
		}

		// Add a command that handles 2 overloads.
		protected void AddCommand(string name, int[] modes, string params1, string params2, Action<CommandParam> action) {
			AddCommand(name, modes, new string[] { params1, params2 }, action);
		}

		// Add a command that handles 3 overloads.
		protected void AddCommand(string name, int[] modes, string params1, string params2, string params3, Action<CommandParam> action) {
			AddCommand(name, modes, new string[] { params1, params2, params3 }, action);
		}

		// Add a command that handles 4 overloads.
		protected void AddCommand(string name, int[] modes, string params1, string params2, string params3, string params4, Action<CommandParam> action) {
			AddCommand(name, modes, new string[] { params1, params2, params3, params4 }, action);
		}

		// Add a command that handles 5 overloads.
		protected void AddCommand(string name, int[] modes, string params1, string params2, string params3, string params4, string params5, Action<CommandParam> action) {
			AddCommand(name, modes, new string[] { params1, params2, params3, params4, params5 }, action);
		}

		// Add a command that handles 6 overloads.
		protected void AddCommand(string name, int[] modes, string params1, string params2, string params3, string params4, string params5, string params6, Action<CommandParam> action) {
			AddCommand(name, modes, new string[] { params1, params2, params3, params4, params5, params6 }, action);
		}

		// Add a command that handles the given list of overloads.
		protected void AddCommand(string name, int[] modes, string[] parameterOverloads, Action<CommandParam> action) {
			AddCommand(new ScriptCommand(name, modes, parameterOverloads, action));
		}


		//-----------------------------------------------------------------------------
		// Command Creation (Final)
		//-----------------------------------------------------------------------------

		// Add a script command.
		protected void AddCommand(ScriptCommand command) {
			commands.Add(command);
		}


		//-----------------------------------------------------------------------------
		// Command Prefix Creation
		//-----------------------------------------------------------------------------

		protected void AddCommandPrefix(string prefix, params int[] modes) {
			if (string.IsNullOrWhiteSpace(prefix))
				throw new ArgumentNullException("Command prefix cannot be null or whitespace in script reader!");
			if (commandPrefixes.ContainsKey(prefix))
				throw new ArgumentException("Command prefix '" + prefix + "' already exists in script reader!");
			commandPrefixes.Add(prefix, new CommandPrefix(prefix, modes));
		}

		//-----------------------------------------------------------------------------
		// Resources
		//-----------------------------------------------------------------------------

		protected T GetResource<T>(string name) {
			if (name.StartsWith("temp_")) {
				if (!tempResources.ContainsResource<T>(name))
					ThrowCommandParseError("Resource with name '" + name + "' does not exist!");
				return tempResources.GetResource<T>(name);
			}
			else {
				if (!Resources.ContainsResource<T>(name))
					ThrowCommandParseError("Resource with name '" + name + "' does not exist!");
				return Resources.GetResource<T>(name);
			}
		}

		protected T SetResource<T>(string name, T resource) {
			if (name.StartsWith("temp_")) {
				tempResources.SetResource<T>(name, resource);
			}
			else {
				Resources.SetResource<T>(name, resource);
			}
			return resource;
		}

		protected T AddResource<T>(string name, T resource) {
			if (name.StartsWith("temp_")) {
				if (tempResources.ContainsResource<T>(name))
					ThrowCommandParseError("Resource with name '" + name + "' already exists!");
				tempResources.AddResource<T>(name, resource);
			}
			else {
				if (Resources.ContainsResource<T>(name))
					ThrowCommandParseError("Resource with name '" + name + "' already exists!");
				Resources.AddResource<T>(name, resource);
			}
			return resource;
		}


		//-----------------------------------------------------------------------------
		// Virtual methods
		//-----------------------------------------------------------------------------

		// Begins reading the script.
		protected virtual void BeginReading() {}

		// Ends reading the script.
		protected virtual void EndReading() {}
		
		// Reads a line in the script as a command.
		protected virtual bool PerformCommand(string commandName, CommandParam parameters, string commandPrefix) {
			List<string> matchingFormats = new List<string>();
			CommandParam newParams = null;

			// Search for the correct command.
			for (int i = 0; i < commands.Count; i++) {
				ScriptCommand command = commands[i];
				if (command.HasName(commandName) && MatchesMode(mode, command.Modes)) {
					if (command.HasParameters(parameters, out newParams))
					{
						// Run the command.
						newParams.Prefix = commandPrefix ?? "";
						command.Action(newParams);
						return true;
					}
					else {
						// Preemptively append the possible overloads to the error message.
						for (int j = 0; j < command.ParameterOverloads.Count; j++)
							matchingFormats.Add(command.Name + " " +
								CommandParamParser.ToString(command.ParameterOverloads[j]));
					}
				}
			}

			// Throw an error because the command was not found.
			if (matchingFormats.Count > 0) {
				Console.WriteLine(CommandParamParser.ToString(parameters));
				string msg = "No matching overload found for the command " + commandName + "\n";
				msg += "Possible overloads include:\n";
				for (int i = 0; i < matchingFormats.Count; i++) {
					msg += "  * " + matchingFormats[i];
					if (i + 1 < matchingFormats.Count)
						msg += "\n";
				}
				ThrowCommandParseError(msg);
			}
			else {
				ThrowCommandParseError(commandName + " is not a valid command");
			}

			return false;
		}

		
		//-----------------------------------------------------------------------------
		// Errors
		//-----------------------------------------------------------------------------

		// Throw a parse error exception, optionally showing a caret.
		protected void ThrowParseError(string message, bool showCaret = true) {
			throw new ScriptReaderException(message, fileName, lines[lineIndex], lineIndex + 1, charIndex + 1, showCaret);
		}

		// Throw a parse error exception for a specific argument.
		protected void ThrowParseError(string message, CommandParam param) {
			throw new ScriptReaderException(message, fileName, lines[param.LineIndex], param.LineIndex + 1, param.CharIndex + 1, true);
		}

		// Throw a parse error exception pointing to the command name.
		protected void ThrowCommandParseError(string message) {
			throw new ScriptReaderException(message, fileName, lines[parameterRoot.LineIndex], parameterRoot.LineIndex + 1, parameterRoot.CharIndex + 1, true);
		}


		//-----------------------------------------------------------------------------
		// Parsing methods
		//-----------------------------------------------------------------------------

		// Return true if the given character is allowed for keywords
		private bool IsValidKeywordCharacter(char c) {
			string validKeywordSymbols = "$_.-+";

			if (Char.IsLetterOrDigit(c))
				return true;
			if (validKeywordSymbols.IndexOf(c) >= 0)
				return true;
			return false;
		}

		// Attempt to add a completed word, if it is not empty.
		protected void CompleteWord(bool completeIfEmpty = false) {
			if (word.Length > 0 || completeIfEmpty)
				AddParam();
			word = "";
		}
		
		// Complete the current statement being parsed, looking for a command.
		protected void CompleteStatement() {
			if (parameterRoot.Children != null) {
				// Take the command name from the parameters.
				string commandName = parameterRoot.Children.StringValue;
				parameterRoot.LineIndex	= parameterRoot.Children.LineIndex;
				parameterRoot.CharIndex	= parameterRoot.Children.CharIndex;
				parameterRoot.Children	= parameterRoot.Children.NextParam;
				parameterRoot.ChildCount--;
				
				// Attempt to perform the command.
				PerformCommand(commandName, parameterRoot, commandPrefix);
			}

			// Reset the parameter list.
			parameterParent	= new CommandParam("");
			parameterParent.Type = CommandParamType.Array;
			parameterRoot	= parameterParent;
			parameter		= null;
			commandPrefix   = null;
		}

		// Add a new command parameter child to the current parent parameter.
		private CommandParam AddParam() {
			// If this is the beginning of the command, check for prefixes
			if (parameterParent == parameterRoot && parameter == null && commandPrefix == null) {
				if (commandPrefixes.ContainsKey(word)) {
					if (MatchesMode(mode, commandPrefixes[word].Modes)) {
						commandPrefix = word;
						return null;
					}
				}
			}
			CommandParam newParam = new CommandParam(word);
			newParam.CharIndex = wordCharIndex;
			newParam.LineIndex = lineIndex;
			if (parameter == null)
				parameterParent.Children = newParam;
			else
				parameter.NextParam = newParam;
			parameterParent.ChildCount++;
			newParam.Parent = parameterParent;
			parameter = newParam;
			return newParam;
		}

		// Parse a single line in the script.
		protected void ParseLine(string line) {
			bool quotes	= false;
			word		= "";
			charIndex	= 0;

			// Parse line character by character.
			for (int i = 0; i < line.Length; i++) {
				char c = line[i];
				charIndex = i;

				// Parse quotes.
				if (quotes) {
					// Closing quotes.
					if (c == '"') {
						quotes = false;
						CompleteWord(true);
					}
					else
						word += c;
				}

				// Whitespace and commas (parameter delimiters).
				else if (c == ' ' || c == '\t' || c == ',')
					CompleteWord();

				// Semicolons.
				else if (c == ';') {
					CompleteWord();
					int prevLineIndex = lineIndex;
					CompleteStatement();
					if (lineIndex > prevLineIndex) // Commands are allowed to read the next lines in the file.
						return;
				}

				// Single-line comment.
				else if (c == '#')
					break; // Ignore the rest of the line.

				// Opening quotes.
				else if (word.Length == 0 && c == '\"')
					quotes = true;
					
				// Opening parenthesis: begin an array parameter.
				else if (word.Length == 0 && c == '(') {
					parameterParent = AddParam();
					parameterParent.Type = CommandParamType.Array;
					parameter = null;
				}
					
				// Closing parenthesis.
				else if (c == ')') {
					CompleteWord();
					if (parameterParent == parameterRoot) {
						ThrowParseError("Unexpected symbol ')'");
					}
					else {
						parameter = parameterParent;
						parameterParent = parameterParent.Parent;
					}
				}

				// Valid keyword character.
				else if (IsValidKeywordCharacter(c)) {
					if (word.Length == 0)
						wordCharIndex = charIndex;
					word += c;
				}

				// Error: Unexpected character.
				else
					ThrowParseError("Unexpected symbol '" + c + "'");
			}

			charIndex++;

			// Make sure quotes are closed at the end of the line.
			if (quotes)
				ThrowParseError("Expected \"");

			CompleteWord();
		}

		// Read the next line from the file.
		protected string NextLine() {
			lineIndex++;
			line = streamReader.ReadLine();
			lines.Add(line);
			return line;
		}

		// Parse and interpret the given text stream as a script, line by line.
		public void ReadScript(StreamReader reader, string path) {
			this.fileName = path;
			this.streamReader = reader;

			BeginReading();

			// Setup the parsing variables.
			parameterParent	= new CommandParam("");
			parameterParent.Type = CommandParamType.Array;
			parameterRoot	= parameterParent;
			parameter		= null;
			
			// Read all lines.
			lineIndex = -1;
			lines.Clear();
			while (!reader.EndOfStream) {
				NextLine();
				ParseLine(line);
			}

			EndReading();
		}


		//-----------------------------------------------------------------------------
		// Misc
		//-----------------------------------------------------------------------------
		
		protected void PrintParementers(CommandParam param) {
			CommandParam p = param.Children;
			while (p != null) {
				if (p.Type == CommandParamType.Array) {
					Console.Write("(");
					PrintParementers(p);
					Console.Write(")");
				}
				else
					Console.Write(p.StringValue);

				p = p.NextParam;
				if (p != null)
					Console.Write(", ");
			}
		}

		private static bool MatchesMode(int mode, int[] modes) {
			if (modes != null && modes.Length > 0) {
				for (int i = 0; i < modes.Length; i++) {
					if (modes[i] == mode)
						return true;
				}
				return false;
			}
			return true;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public List<ScriptCommand> ScriptCommands {
			get { return commands; }
		}

		/// <summary>Gets or sets the current mode of the reader used to determine valid commands.</summary>
		protected int Mode {
			get { return mode; }
			set { mode = value; }
		}
	}
}
