using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.IO;
using ZeldaOracle.Common.Conscripts.Commands;
using ZeldaOracle.Common.Content;

namespace ZeldaOracle.Common.Conscripts {
	/// <summary>
	/// A script reader is an abstract object that
	/// is meant to be implemented to be able to
	/// interpret text files written in a certain syntax.
	/// <summary>
	public class ScriptReader {
		
		public static Stopwatch Watch = new Stopwatch();

		private ConscriptRunner	script;

		private StreamReader	streamReader;
		private string			fileName;
		private List<string>    lines;
		private string			line;			// The string of the current line.
		private int				lineIndex;
		private int				charIndex;
		private int				wordCharIndex;	// Character index for the current word being parsed.
		private string			word;			// The current word being parsed.
		private CommandParam	parameter;
		private CommandParam	parameterParent;
		private CommandParam	parameterRoot;

		private string          parameterName;
		private CommandParam    namedParameter;

		private IReadOnlyList<ScriptCommand> commands;   // List of possible commands.

		private CommandParamDefinitions typeDefinitions;

		/// <summary>The current mode of the reader used to determine valid commands.</summary>
		private int             mode;

		private TemporaryResources tempResources;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ScriptReader() {
			parameter		= null;
			parameterRoot	= null;
			lines           = new List<string>();
			tempResources   = null;
			parameterName   = null;
			
		}


		//-----------------------------------------------------------------------------
		// Errors Methods
		//-----------------------------------------------------------------------------

		/// <summary>Throw a parse error exception, optionally showing a caret.</summary>
		public void ThrowParseError(string message, bool showCaret = true) {
			throw new ScriptReaderException(message, fileName, lines[lineIndex], lineIndex + 1, charIndex + 1, showCaret);
		}

		/// <summary>Throw a parse error exception for a specific argument.</summary>
		public void ThrowParseError(string message, CommandParam param) {
			throw new ScriptReaderException(message, fileName, lines[param.LineIndex], param.LineIndex + 1, param.CharIndex + 1, true);
		}

		/// <summary>Throw a parse error exception pointing to the command name.</summary>
		public void ThrowCommandParseError(string message) {
			throw new ScriptReaderException(message, fileName, lines[parameterRoot.LineIndex], parameterRoot.LineIndex + 1, parameterRoot.CharIndex + 1, true);
		}



		//-----------------------------------------------------------------------------
		// Resources
		//-----------------------------------------------------------------------------

		/// <summary>Reads a line in the script as a command.</summary>
		private bool PerformCommand(string commandName, CommandParam parameters) {
			List<string> matchingFormats = new List<string>();
			CommandParam newParams = null;

			// Search for the correct command.
			for (int i = 0; i < script.Commands.Count; i++) {
				ScriptCommand command = script.Commands[i];
				if (command.HasName(commandName, parameters) && MatchesMode(mode, command.Modes)) {
					if (command.HasParameters(parameters, out newParams, script.TypeDefinitions)) {
						// Run the command.
						try {
							ScriptReader.Watch.Stop();
							command.Action(newParams);
							ScriptReader.Watch.Start();
						}
						catch (ThreadAbortException) { }
						catch (LoadContentException ex) {
							throw ex;
						}
						catch (Exception ex) {
							throw new ScriptReaderException(ex.Message, fileName, lines[lineIndex], lineIndex + 1, charIndex + 1, true, ex.StackTrace);
						}
						return true;
					}
					else {
						// Preemptively append the possible overloads to the error message.
						for (int j = 0; j < command.ParameterOverloads.Count; j++)
							matchingFormats.Add(command.FullName + " " +
								CommandParamParser.ToString(command.ParameterOverloads[j]));
					}
				}
			}

			// Throw an error because the command was not found.
			if (matchingFormats.Count > 0) {
				Console.WriteLine(CommandParamParser.ToString(parameters));
				string msg = "No matching overload found for the command " + commandName + ". Did you forget a semicolon?\n";
				msg += "Possible overloads include:\n";
				for (int i = 0; i < matchingFormats.Count; i++) {
					msg += "  * " + matchingFormats[i];
					if (i + 1 < matchingFormats.Count)
						msg += "\n";
				}
				ThrowCommandParseError(msg);
			}
			else {
				ThrowCommandParseError(commandName + " is not a valid command. Did you forget an 'END'?");
			}

			return false;
		}


		//-----------------------------------------------------------------------------
		// Parsing methods
		//-----------------------------------------------------------------------------

		/// <summary>Return true if the given character is allowed for keywords.</summary>
		private bool IsValidKeywordCharacter(char c) {
			string validKeywordSymbols = "$_.-+";

			if (char.IsLetterOrDigit(c))
				return true;
			if (validKeywordSymbols.IndexOf(c) >= 0)
				return true;
			return false;
		}

		/// <summary>Attempt to add a completed word, if it is not empty.</summary>
		private void CompleteWord(bool completeIfEmpty = false) {
			if (word.Length > 0 || completeIfEmpty)
				AddParam();
			word = "";
		}

		/// <summary>Attempt to name the upcoming parameter.</summary>
		private void NameParameter() {
			if (word.Length == 0)
				ThrowParseError("Unexpected symbol ':'!");
			if (parameterName != null)
				ThrowParseError("Parameter already named!");
			parameterName = word;
			word = "";
		}

		/// <summary>Complete the current statement being parsed, looking for a command.</summary>
		private void CompleteStatement() {
			if (parameterRoot.Children != null) {
				// Take the command name from the parameters.
				string commandName = parameterRoot.Children.StringValue;
				parameterRoot.LineIndex	= parameterRoot.Children.LineIndex;
				parameterRoot.CharIndex	= parameterRoot.Children.CharIndex;
				parameterRoot.Children	= parameterRoot.Children.NextParam;
				parameterRoot.ChildCount--;
				
				// Attempt to perform the command.
				PerformCommand(commandName, parameterRoot);
			}

			// Reset the parameter list.
			parameterParent	= new CommandParam("");
			parameterParent.Type = CommandParamType.Array;
			parameterRoot	= parameterParent;
			parameter		= null;
			namedParameter  = null;
		}

		/// <summary>Add a new command parameter child to the current parent parameter.</summary>
		private CommandParam AddParam() {
			CommandParam newParam = new CommandParam(word);
			newParam.CharIndex = wordCharIndex;
			newParam.LineIndex = lineIndex;
			if (parameter == null)
				parameterParent.Children = newParam;
			else
				parameter.NextParam = newParam;
			parameterParent.ChildCount++;
			if (parameterName != null) {
				newParam.Name = parameterName;
				if (namedParameter == null)
					parameterParent.NamedChildren = newParam;
				else
					namedParameter.NextParam = newParam;
				parameterName = null;
				namedParameter = newParam;
				parameterParent.NamedChildCount++;
			}
			else if (parameter != null && !string.IsNullOrEmpty(parameter.Name)) {
				ThrowParseError("All parameters after the first named parameter must be named! Did you accidentally use a ':' instead of a ';'?");
			}
			newParam.Parent = parameterParent;
			parameter = newParam;
			return newParam;
		}

		/// <summary>Parse a single line in the script.</summary>
		private void ParseLine(string line) {
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

				// Ending for a named parameter
				else if (c == ':')
					NameParameter();

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
						if (string.IsNullOrEmpty(parameterParent.Name))
							namedParameter = null;
						else
							namedParameter = parameterParent;
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

		/// <summary>Read the next line from the file.</summary>
		private string NextLine() {
			lineIndex++;
			line = streamReader.ReadLine();
			lines.Add(line);
			return line;
		}

		/// <summary>Parse and interpret the given text stream as a script, line by line.</summary>
		public void ReadScript(ConscriptRunner runner, Stream stream, string path) {
			ScriptReader.Watch.Start();
			script          = runner;
			commands        = runner.Commands;
			typeDefinitions = runner.TypeDefinitions;
			tempResources	= runner.TempResources;

			fileName = path;
			streamReader = new StreamReader(stream, Encoding.Default);

			script.BeginReading(this);

			// Setup the parsing variables.
			parameterParent	= new CommandParam("");
			parameterParent.Type = CommandParamType.Array;
			parameterRoot	= parameterParent;
			parameter		= null;
			
			// Read all lines.
			lineIndex = -1;
			lines.Clear();
			while (!streamReader.EndOfStream) {
				NextLine();
				ParseLine(line);
			}

			script.EndReading();
			
			ScriptReader.Watch.Stop();
		}


		//-----------------------------------------------------------------------------
		// Misc
		//-----------------------------------------------------------------------------
		
		private void PrintParementers(CommandParam param) {
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

		/// <summary>Gets or sets the current mode of the reader used to determine valid commands.</summary>
		public int Mode {
			get { return mode; }
			set { mode = value; }
		}

		/// <summary>Gets the file path for this conscript.</summary>
		public string FileName {
			get { return fileName; }
		}

		/// <summary>Gets the file directory for this conscript.</summary>
		public string Directory {
			get { return Path.GetDirectoryName(fileName); }
		}
	}
}

