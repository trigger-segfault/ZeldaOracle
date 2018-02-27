using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Content;

namespace ZeldaOracle.Common.Scripts {

	/// <summary>A helpful exception class for throwing script reader errors.</summary>
	public class ScriptReaderException : LoadContentException {

		private string	fileName;
		private int		lineNumber;
		private int		columnNumber;
		private string	line;
		private bool	showCaret;

		private string  scriptStackTrace;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Constructs the script reader exception.</summary>
		public ScriptReaderException(string message, string fileName, string line,
			int lineNumber, int columnNumber, bool showCaret, string scriptStackTrace = "") :
			base(message)
		{
			this.fileName			= fileName;
			this.line				= line;
			this.lineNumber			= lineNumber;
			this.columnNumber		= columnNumber;
			this.showCaret			= showCaret;
			this.scriptStackTrace   = scriptStackTrace;
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		/// <summary>Prints the exception message.</summary>
		public override void PrintMessage() {
			// Display the error message.
			Console.WriteLine("------------------------------------------------------------------");
			Console.WriteLine("Error in '" + fileName.Replace('/', '\\') + "' at Line " + 
				lineNumber + ", Column " + columnNumber + ":");
			Console.WriteLine();

			// Display the line of the script.
			Console.WriteLine(line);

			// Display the caret.
			if (showCaret) {
				for (int i = 1; i < columnNumber; i++) {
					if (line[i - 1] == '\t')
						Console.Write('\t');
					else
						Console.Write(' ');
				}
				Console.WriteLine('^');
			}
			else
				Console.WriteLine();

			Console.WriteLine(Message);
			Console.WriteLine("------------------------------------------------------------------");
			if (!string.IsNullOrEmpty(scriptStackTrace)) {
				Console.WriteLine(scriptStackTrace);
			}
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets the filename of the script throwing the exception.</summary>
		public string FileName {
			get { return fileName; }
		}

		/// <summary>Gets the line number in the file where the exception occurred.</summary>
		public int LineNumber {
			get { return lineNumber; }
		}

		/// <summary>Gets the column number in the file's line where the exception occurred.</summary>
		public int ColumnNumber {
			get { return columnNumber; }
		}
	}
}
