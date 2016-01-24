using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ZeldaOracle.Common.Content;

namespace ZeldaOracle.Common.Scripts {

	public class ScriptCommand {
		private string name;
		private Action<CommandParam> action;

		public ScriptCommand(string name, Action<CommandParam> action) {
			this.name	= name;
			this.action	= action;
		}

		public string Name {
			get { return name; }
		}

		public Action<CommandParam> Action {
			get { return action; }
		}
	}

	
	/** <summary>
	 * A script reader is an abstract object that
	 * is meant to be implemented to be able to
	 * interpret text files written in a certain syntax.
	 * </summary> */
	public class NewScriptReader : ScriptReader {

		// A helpful exception class for throwing script errors.
		public class ParseException : LoadContentException {
			private string	fileName;
			private int		lineNumber;
			private int		columnNumber;
			private string	line;


			public ParseException(string message, string fileName, string line, int lineNumber, int columnNumber) :
				base(message)
			{
				this.fileName		= fileName;
				this.line			= line;
				this.lineNumber		= lineNumber;
				this.columnNumber	= columnNumber;
			}

			public override void PrintMessage() {
				// Display the error message.
				Console.WriteLine("------------------------------------------------------------------");
				Console.WriteLine("Error in '" + fileName + "' at Line " + 
					lineNumber + ", Column " + columnNumber + ":");
				Console.WriteLine(Message);

				// Display the line of the script.
				Console.WriteLine(line);
				for (int i = 1; i < columnNumber; i++) {
					if (line[i - 1] == '\t')
						Console.Write('\t');
					else
						Console.Write(' ');
				}
				Console.WriteLine('^');
				Console.WriteLine("------------------------------------------------------------------");
			}
		}
		
		private StreamReader	streamReader;
		private int				lineIndex;
		private int				charIndex;
		private string			word;
		private string			line;
		private string			fileName;

		private List<ScriptCommand> commands;

		private CommandParam parameter;
		private CommandParam parameterParent;
		private CommandParam parameterRoot;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public NewScriptReader() {
			parameter		= null;
			parameterRoot	= null;
			commands		= new List<ScriptCommand>();
		}
		

		//-----------------------------------------------------------------------------
		// Commands
		//-----------------------------------------------------------------------------

		protected void AddCommand(string name, Action<CommandParam> action) {
			// Don't add commands that already exist.
			for (int i = 0; i < commands.Count; i++) {
				if (String.Compare(commands[i].Name, name, StringComparison.CurrentCultureIgnoreCase) == 0)
					return;
			}

			ScriptCommand command = new ScriptCommand(name, action);
			commands.Add(command);
		}


		//-----------------------------------------------------------------------------
		// Virtual methods
		//-----------------------------------------------------------------------------

		// Begins reading the script.
		//protected virtual void BeginReading() {}

		// Ends reading the script.
		//protected virtual void EndReading() {}

		// Reads a line in the script as a command.
		protected virtual bool ReadCommand(string commandName, CommandParam parameters) {
			for (int i = 0; i < commands.Count; i++) {
				if (String.Compare(commands[i].Name, commandName,
					StringComparison.CurrentCultureIgnoreCase) == 0)
				{
					commands[i].Action(parameters);
					return true;
				}
			}

			return false;
		}

		protected void ThrowParseError(string message, bool showCarret = true) {
			throw new ParseException(message, fileName, line, lineIndex + 1, charIndex + 1);
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

		protected void PrintParementers(CommandParam param) {
			CommandParam p = param.Children;
			while (p != null) {
				if (p.Type == CommandParamType.Array) {
					Console.Write("(");
					PrintParementers(p);
					Console.Write(")");
				}
				else
					Console.Write(p.Str);

				p = p.NextParam;
				if (p != null)
					Console.Write(", ");
			}
		}

		protected void CompleteStatement() {
			if (parameterRoot.Children != null) {
				string commandName = parameterRoot.Children.Str;
				parameterRoot.Children = parameterRoot.Children.NextParam;
				parameterRoot.Count--;

				if (!ReadCommand(commandName, parameterRoot)) {
					ThrowParseError(commandName + " is not a valid command", false);
				}
			}

			parameterParent	= new CommandParam("");
			parameterParent.Type = CommandParamType.Array;
			parameterRoot	= parameterParent;
			parameter		= null;
		}

		private CommandParam AddParam() {
			CommandParam newParam = new CommandParam(word);
			if (parameter == null)
				parameterParent.Children = newParam;
			else
				parameter.NextParam = newParam;
			parameterParent.Count++;
			newParam.Parent = parameterParent;
			parameter = newParam;
			return newParam;
		}

		// Read a single line of the script.
		protected override void ReadLine(string line) {
			word = "";
			bool quotes = false;
			charIndex = 0;

			// Parse line character by character.
			for (int i = 0; i < line.Length; i++) {
				char c = line[i];
				charIndex = i;

				// Parse quotes.
				if (quotes) {
					// Closing quotes.
					if (c == '\"') {
						quotes = false;
						CompleteWord(true);
					}
					else
						word += c;
				}

				// Whitespace.
				else if (c == ' ' || c == '\t')
					CompleteWord();

				// Commas.
				else if (c == ',')
					CompleteWord();

				// Semicolons.
				else if (c == ';') {
					CompleteWord();
					int prevLineIndex = lineIndex;
					CompleteStatement();
					if (lineIndex > prevLineIndex)
						return;
				}

				// Single-line comment.
				else if (c == '#') {
					break; // Ignore the rest of the line.
				}

				// Opening quotes.
				else if (word.Length == 0 && c == '\"')
					quotes = true;
					
				// Opening parenthesis.
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
				else if (IsValidKeywordCharacter(c))
					word += c;

				// Error: Unexpected character.
				else
					ThrowParseError("Unexpected symbol '" + c + "'");
			}

			charIndex++;

			// Make sure quotes are closed and statements are ended.
			if (quotes)
				ThrowParseError("Expected \"");

			CompleteWord();
		}

		protected string NextLine() {
			lineIndex++;
			line = streamReader.ReadLine();
			return line;
		}

		// Parse and interpret the given text stream as a script, line by line.
		public override void ReadScript(StreamReader reader, string path) {
			this.fileName = path;
			this.streamReader = reader;

			BeginReading();

			parameterParent	= new CommandParam("");
			parameterParent.Type = CommandParamType.Array;
			parameterRoot	= parameterParent;
			parameter		= null;
			
			// Read all lines.
			lineIndex = -1;
			while (!reader.EndOfStream) {
				NextLine();
				ReadLine(line);
			}

			EndReading();
		}

	}
}
