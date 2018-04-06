using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Scripting;

namespace ZeldaOracle.Common.Translation {

	/// <summary>The format code types available.</summary>
	public enum FormatCodeType {
		/// <summary>This is not a format code.</summary>
		None,
		/// <summary>The format code outputs to to changing the current text color.</summary>
		Color,
		/// <summary>The format code outputs to a character or string.</summary>
		String,
		/// <summary>The format code outputs to an uncolored character or string.</summary>
		NoColorString,
		/// <summary>The format code outputs to a string representation of a variable.</summary>
		Variable,
	}

	/// <summary>A static class for managing string format codes.</summary>
	public class FormatCodes {

		//-----------------------------------------------------------------------------
		// Constants
		//-----------------------------------------------------------------------------

		/// <summary>The character used to mark a heart piece display.</summary>
		public const char HeartPieceCharacter = (char) 254;
		/// <summary>The character used to mark a new paragraph.</summary>
		public const char ParagraphCharacter = (char) 253;
		/// <summary>The character to align the current line to the left.</summary>
		public const char AlignLeftCharacter = (char) 252;
		/// <summary>The character to align the current line to the center.</summary>
		public const char AlignCenterCharacter = (char) 251;
		/// <summary>The character to align the current line to the right.</summary>
		public const char AlignRightCharacter = (char) 250;
		public const char MenuOptionCharactersBegin = (char) 140;
		public const char MenuOptionCharactersEnd = (char) 150;

		public static bool IsMenuOption(char c) {
			return (c >= MenuOptionCharactersBegin && c < MenuOptionCharactersEnd);
		}
		
		public static int GetMenuOptionIndex(char c) {
			return (int) (c - MenuOptionCharactersBegin);
		}

		//-----------------------------------------------------------------------------
		// Members
		//-----------------------------------------------------------------------------

		/// <summary>The list of color codes.</summary>
		private static Dictionary<string, ColorOrPalette> colorCodes;
		/// <summary>The list of string codes.</summary>
		private static Dictionary<string, string> stringCodes;
		/// <summary>The list of uncolored string codes.</summary>
		private static Dictionary<string, string> noColorStringCodes;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Initializes the format codes.</summary>
		static FormatCodes() {

			FormatCodes.colorCodes			= new Dictionary<string, ColorOrPalette>();
			FormatCodes.stringCodes			= new Dictionary<string, string>();
			FormatCodes.noColorStringCodes  = new Dictionary<string, string>();

			FormatCodes.colorCodes.Add("red", EntityColors.Red); //#FFF80828
			FormatCodes.colorCodes.Add("green", EntityColors.Green); //#FF10A840
			FormatCodes.colorCodes.Add("blue", EntityColors.Blue); //#FF1880F8
			FormatCodes.colorCodes.Add("orange", EntityColors.Orange); //#FFF87808

			FormatCodes.stringCodes.Add("triangle",		"" + (char) 1);
			FormatCodes.stringCodes.Add("square",		"" + (char) 2);
			FormatCodes.stringCodes.Add("heart",		"" + (char) 3);
			FormatCodes.stringCodes.Add("diamond",		"" + (char) 4);
			FormatCodes.stringCodes.Add("club",			"" + (char) 5);
			FormatCodes.stringCodes.Add("spade",		"" + (char) 6);
			FormatCodes.stringCodes.Add("circle",		"" + (char) 7);
			FormatCodes.stringCodes.Add("male",			"" + (char) 11);
			FormatCodes.stringCodes.Add("female",		"" + (char) 12);
			FormatCodes.stringCodes.Add("music",		"" + (char) 13);
			FormatCodes.stringCodes.Add("music-beam",	"" + (char) 14);
			FormatCodes.stringCodes.Add("rupee",		"" + (char) 15);
			FormatCodes.stringCodes.Add("right-tri",	"" + (char) 16);
			FormatCodes.stringCodes.Add("left-tri",		"" + (char) 17);
			FormatCodes.stringCodes.Add("invalid",		"" + (char) 18);
			FormatCodes.stringCodes.Add("!!",			"" + (char) 19);
			FormatCodes.stringCodes.Add("pilcrow",		"" + (char) 20);
			FormatCodes.stringCodes.Add("section",		"" + (char) 21);
			FormatCodes.stringCodes.Add("cursor",		"" + (char) 22);
			FormatCodes.stringCodes.Add("slot1",		"" + (char) 23);
			FormatCodes.stringCodes.Add("up",			"" + (char) 24);
			FormatCodes.stringCodes.Add("down",			"" + (char) 25);
			FormatCodes.stringCodes.Add("right",		"" + (char) 26);
			FormatCodes.stringCodes.Add("left",			"" + (char) 27);
			FormatCodes.stringCodes.Add("slot2",		"" + (char) 28);
			FormatCodes.stringCodes.Add("slot3",		"" + (char) 29);
			FormatCodes.stringCodes.Add("up-tri",		"" + (char) 30);
			FormatCodes.stringCodes.Add("down-tri",		"" + (char) 31);
			FormatCodes.stringCodes.Add("house",		"" + (char) 127);
			FormatCodes.stringCodes.Add("<", "<");
			FormatCodes.stringCodes.Add("n", "\n");

			FormatCodes.stringCodes.Add("a", "" + (char) 128 + (char) 129);
			FormatCodes.stringCodes.Add("b", "" + (char) 130 + (char) 131);
			FormatCodes.stringCodes.Add("x", "" + (char) 132 + (char) 133);
			FormatCodes.stringCodes.Add("y", "" + (char) 134 + (char) 135);

			FormatCodes.stringCodes.Add("p", "" + ParagraphCharacter);
			FormatCodes.stringCodes.Add("heart-piece", "" + HeartPieceCharacter);
			FormatCodes.stringCodes.Add("align-left", "" + AlignLeftCharacter);
			FormatCodes.stringCodes.Add("align-center", "" + AlignCenterCharacter);
			FormatCodes.stringCodes.Add("align-right", "" + AlignRightCharacter);
			
			FormatCodes.noColorStringCodes.Add("dpad", "" + (char) 136);
			
			FormatCodes.noColorStringCodes.Add("0", "" + (char) (MenuOptionCharactersBegin + 0));
			FormatCodes.noColorStringCodes.Add("1", "" + (char) (MenuOptionCharactersBegin + 1));
			FormatCodes.noColorStringCodes.Add("2", "" + (char) (MenuOptionCharactersBegin + 2));
			FormatCodes.noColorStringCodes.Add("3", "" + (char) (MenuOptionCharactersBegin + 3));
			FormatCodes.noColorStringCodes.Add("4", "" + (char) (MenuOptionCharactersBegin + 4));
			FormatCodes.noColorStringCodes.Add("5", "" + (char) (MenuOptionCharactersBegin + 5));
			FormatCodes.noColorStringCodes.Add("6", "" + (char) (MenuOptionCharactersBegin + 6));
			FormatCodes.noColorStringCodes.Add("7", "" + (char) (MenuOptionCharactersBegin + 7));
			FormatCodes.noColorStringCodes.Add("8", "" + (char) (MenuOptionCharactersBegin + 8));
			FormatCodes.noColorStringCodes.Add("9", "" + (char) (MenuOptionCharactersBegin + 9));
		}


		//-----------------------------------------------------------------------------
		// Codes
		//-----------------------------------------------------------------------------

		/// <summary>Gets the string code type.</summary>
		public static FormatCodeType GetFormatCodeType(string code) {
			if (colorCodes.ContainsKey(code) || code.StartsWith("color:"))
				return FormatCodeType.Color;
			else if (stringCodes.ContainsKey(code))
				return FormatCodeType.String;
			else if (noColorStringCodes.ContainsKey(code))
				return FormatCodeType.NoColorString;
			else if (code.StartsWith("var:") || code.StartsWith("fvar:"))
				return FormatCodeType.Variable;
			return FormatCodeType.None;
		}

		/// <summary>Gets the string representation of the code.</summary>
		public static string GetFormatCodeString(string code, Variables variables = null) {
			if (stringCodes.ContainsKey(code)) {
				return stringCodes[code];
			}
			else if (noColorStringCodes.ContainsKey(code)) {
				return noColorStringCodes[code];
			}
			else if (code.StartsWith("var:")) {
				// See if formatting for the variable was specified
				string varCode = code.Substring(4);
				string[] nameFormat = varCode.Split(new char[] { ':' }, 2);
				string varName = nameFormat[0];
				string format = (nameFormat.Length == 2 ? nameFormat[1] : null);
				if (variables != null && variables.Contains(varName)) {
					Variable v = variables.GetVariable(varName);
					return v.FormatValue(format);
				}
				return "<" + varName + ">";
			}
			else if (code.StartsWith("fvar:")) {
				// Apply Format Codes to the variable
				string varCode = code.Substring(4);
				string[] nameFormat = varCode.Split(new char[] { ':' }, 2);
				string varName = nameFormat[0];
				string format = (nameFormat.Length == 2 ? nameFormat[1] : null);
				if (variables != null && variables.Contains(varName)) {
					Variable v = variables.GetVariable(varName);
					return GetFormatCodeString(v.FormatValue(format), variables);
				}
				return "<" + GetFormatCodeString(varName, variables) + ">";
			}
			return "";
		}

		/// <summary>Gets the color of the color code.</summary>
		public static ColorOrPalette GetFormatCodeColor(string code) {
			if (colorCodes.ContainsKey(code)) {
				return colorCodes[code];
			}
			else if (code.StartsWith("color:")) {
				try {
					return Color.Parse(code.Substring(("color:").Length));
				}
				catch {
					return Color.White;
				}
			}
			return Letter.DefaultColor;
		}


		//-----------------------------------------------------------------------------
		// Formatting
		//-----------------------------------------------------------------------------

		/// <summary>Returns the text formatted without any format codes in.</summary>
		public static LetterString FormatString(string text, Variables variables) {
			int caretPosition = 0;
			return FormatString(text, ref caretPosition, variables);
		}

		/// <summary>Returns the text formatted without any format codes in.</summary>
		/// <param name="caretPosition">Used to convert the input caret position to the
		/// output caret position after formatting.</param>
		public static LetterString FormatString(string text, ref int caretPosition, Variables variables = null) {
			int originalCaretPosition = caretPosition;
			int currentCharacter = 0;
			ColorOrPalette currentColor = Letter.DefaultColor;
			LetterString letterString = new LetterString();

			bool inCode = false;
			string currentCode = "";

			while (currentCharacter < text.Length) {
				if (currentCharacter == originalCaretPosition)
					caretPosition = letterString.Length;
				if (!inCode && text[currentCharacter] == '<') {
					// Start string code
					inCode = true;
				}
				else if (inCode) {
					if (text[currentCharacter] == '>') {
						// End string code
						FormatCodeType codeType = FormatCodes.GetFormatCodeType(currentCode);
						if (codeType == FormatCodeType.String) {
							string stringCode = FormatCodes.GetFormatCodeString(currentCode);
							letterString.AddRange(stringCode, currentColor);
						}
						else if (codeType == FormatCodeType.NoColorString) {
							string stringCode = FormatCodes.GetFormatCodeString(currentCode);
							letterString.AddRange(stringCode, Color.White);
						}
						else if (codeType == FormatCodeType.Variable) {
							string stringCode = FormatCodes.GetFormatCodeString(currentCode, variables);
							letterString.AddRange(stringCode, currentColor);
						}
						else if (codeType == FormatCodeType.Color) {
							ColorOrPalette colorCode = FormatCodes.GetFormatCodeColor(currentCode);
							if (colorCode == currentColor)
								currentColor = Letter.DefaultColor;
							else
								currentColor = colorCode;
						}
						else {
							// Invalid code
						}
						inCode = false;
						currentCode = "";
					}
					else {
						currentCode += text[currentCharacter];
					}
				}
				else {
					letterString.Add(new Letter(text[currentCharacter], currentColor));
				}
				currentCharacter++;
			}
			if (caretPosition == -1)
				caretPosition = letterString.Length;

			return letterString;
		}


		//-----------------------------------------------------------------------------
		// Escaping
		//-----------------------------------------------------------------------------

		/// <summary>Escapes the message from an editor format to a Zelda format.</summary>
		public static string EscapeMessage(string text) {
			int caretPosition = 0;
			return EscapeMessage(text, ref caretPosition);
		}

		/// <summary>Escapes the message from an editor format to a Zelda format.
		/// Also stores updates the caret postion as the text is modified.</summary>
		public static string EscapeMessage(string text, ref int caretPosition) {
			int originalCaretPosition = caretPosition;
			string newText = "";
			bool lastWasNewLine = true;
			bool inCode = false;
			string currentCode = "";
			caretPosition = -1;
			for (int i = 0; i < text.Length; i++) {
				if (i == originalCaretPosition)
					caretPosition = newText.Length;
				char c = text[i];
				if (inCode) {
					if (c == '>') {
						if (currentCode == "n" || currentCode == "p") {
							if (!lastWasNewLine) {
								lastWasNewLine = true;
								newText += "<" + currentCode + ">";
							}
							else if (currentCode == "p" && newText.Any()) {
								newText = newText.Substring(0, newText.Length - 3);
								if (caretPosition > newText.Length)
									caretPosition = newText.Length;
								newText += "<" + currentCode + ">";
							}
						}
						else {
							newText += "<" + currentCode + ">";
							lastWasNewLine = false;
						}
						inCode = false;
					}
					else {
						currentCode += c;
					}
				}
				else if (c == '\r') {
					// Skip this character
				}
				else if (c == '\n') {
					if (!lastWasNewLine) {
						lastWasNewLine = true;
						newText += "<n>";
					}
				}
				else if (c == '<') {
					inCode = true;
					currentCode = "";
				}
				else {
					lastWasNewLine = false;
					newText += c;
				}
			}
			if (caretPosition == -1)
				caretPosition = newText.Length;

			return newText;
		}

		/// <summary>Unescapes the message so that it is readable in an editor./</summary>
		public static string UnescapeMessage(string text) {
			return text.Replace("\r", "")
						.Replace("<n>", "\n")
						.Replace("<p>", "<p>\n")
						.Replace("\n", Environment.NewLine);
		}


		//-----------------------------------------------------------------------------
		// Accessors
		//-----------------------------------------------------------------------------

		/// <summary>Gets the enumeration of color codes.</summary>
		public static IEnumerable<KeyValuePair<string, ColorOrPalette>> GetColorCodes() {
			foreach (var pair in colorCodes) {
				yield return pair;
			}
		}

		/// <summary>Gets the enumeration of string codes.</summary>
		public static IEnumerable<KeyValuePair<string, string>> GetStringCodes() {
			foreach (var pair in stringCodes) {
				yield return pair;
			}
		}

		/// <summary>Gets the enumeration of uncolored string codes.</summary>
		public static IEnumerable<KeyValuePair<string, string>> GetNoColorStringCodes() {
			foreach (var pair in noColorStringCodes) {
				yield return pair;
			}
		}
	}
}
