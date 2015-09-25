using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using ZeldaOracle.Common.Content;

namespace ZeldaOracle.Common.Scripts {
/** <summary>
 * A script reader is an abstract object that
 * is meant to be implemented to be able to
 * interpret text files written in a certain syntax.
 * </summary> */
abstract public class ScriptReader {

	//=========== ABSTRACT ===========
	#region Abstract

	/** <summary> Begins reading the script. </summary> */
	protected virtual void BeginReading() {}
	/** <summary> Ends reading the script. </summary> */
	protected virtual void EndReading() {}
	/** <summary> Reads a line in the script as a command. </summary> */
	protected virtual bool ReadCommand(string command, List<string> args) { return false; }

	#endregion
	//=========== PARSING ============
	#region Parsing

	/** <summary> Attempt to add a completed word, if it is not empty. </summary> */
	protected virtual String CompleteWord(string word, List<string> words) {
		if (word.Length > 0)
			words.Add(word);
		return "";
	}
	/** <summary> Read a single line of the script. </summary> */
	protected virtual void ReadLine(string line) {
		List<string> words = new List<string>();
		string word = "";
		bool quotes = false;

		// Parse line character by character.
		for (int i = 0; i < line.Length; i++) {
			char c = line[i];

			// Parse quotes.
			if (quotes) {
				// Closing quotes.
				if (c == '\"') {
					quotes = false;
					words.Add(word);
					word = "";
				}
				else
					word += c;
			}

			// Whitespace.
			else if (c == ' ' || c == '\t')
				word = CompleteWord(word, words);

			// Single-line comment.
			else if (c == '#') {
				word = CompleteWord(word, words);
				break;
			}

			// Opening quotes.
			else if (c == '\"')
				quotes = true;

			// Other character.
			else
				word += c;
		}

		word = CompleteWord(word, words);

		// Remove regions
		if (words.Count > 1) {
			if (words[0] == "[region]")
				words.Clear();
			else if (words[0] == "[endregion]")
				words.Clear();
		}

		// Command.
		if (words.Count > 0 && words[0].StartsWith("@")) {
			string command = words[0].Remove(0, 1);
			words.RemoveAt(0);
			ReadCommand(command, words);
		}
	}
	/** <summary> Parse and interpret the given text stream as a script, line by line. </summary> */
	public virtual void ReadScript(StreamReader reader) {
		BeginReading();
		while (!reader.EndOfStream)
			ReadLine(reader.ReadLine());
		EndReading();
	}
	/** <summary> Parse a single line, usually being a command of some sort. </summary> */
	protected virtual void ParseLine(List<string> words) {
		if (words.Count == 0)
			return;

		// Command.
		if (words[0].StartsWith("@")) {
			string command = words[0].Remove(0, 1);
			words.RemoveAt(0);
			ReadCommand(command, words);
		}
	}

	#endregion
}
} // end namespace
