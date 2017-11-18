using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Graphics;

namespace ZeldaOracle.Common.Translation {

	// The string code types available.
	public enum FormatCodeType {
		None,
		Color,
		String,
		Variable
	}

	// A static class for managing string format codes.
	public class FormatCodes {
		
		// The character used to mark a new paragraph.
		public const char ParagraphCharacter = (char)128;

		// The list of color codes.
		private static Dictionary<string, Color> colorCodes;
		// The list of string codes.
		private static Dictionary<string, string> stringCodes;

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		// Initializes the format codes
		public static void Initialize() {

			FormatCodes.colorCodes			= new Dictionary<string, Color>();
			FormatCodes.stringCodes			= new Dictionary<string, string>();

			FormatCodes.colorCodes.Add("red", new Color(248, 8, 40)); //#FFF80828
			FormatCodes.colorCodes.Add("green", new Color(16, 168, 64)); //#FF10A840
			FormatCodes.colorCodes.Add("blue", new Color(24, 128, 248)); //#FF1880F8
			FormatCodes.colorCodes.Add("orange", new Color(248, 120, 8)); //#FFF87808

			FormatCodes.stringCodes.Add("triangle", "" + (char)1);
			FormatCodes.stringCodes.Add("square", "" + (char)2);
			FormatCodes.stringCodes.Add("heart", "" + (char)3);
			FormatCodes.stringCodes.Add("diamond", "" + (char)4);
			FormatCodes.stringCodes.Add("club", "" + (char)5);
			FormatCodes.stringCodes.Add("spade", "" + (char)6);
			FormatCodes.stringCodes.Add("circle", "" + (char)7);
			FormatCodes.stringCodes.Add("male", "" + (char)11);
			FormatCodes.stringCodes.Add("female", "" + (char)12);
			FormatCodes.stringCodes.Add("music", "" + (char)13);
			FormatCodes.stringCodes.Add("music-beam", "" + (char)14);
			FormatCodes.stringCodes.Add("rupee", "" + (char)15);
			FormatCodes.stringCodes.Add("right-tri", "" + (char)16);
			FormatCodes.stringCodes.Add("left-tri", "" + (char)17);
			FormatCodes.stringCodes.Add("invalid", "" + (char)18);
			FormatCodes.stringCodes.Add("!!", "" + (char)19);
			FormatCodes.stringCodes.Add("pilcrow", "" + (char)20);
			FormatCodes.stringCodes.Add("section", "" + (char)21);
			FormatCodes.stringCodes.Add("cursor", "" + (char)22);
			FormatCodes.stringCodes.Add("1", "" + (char)23);
			FormatCodes.stringCodes.Add("up", "" + (char)24);
			FormatCodes.stringCodes.Add("down", "" + (char)25);
			FormatCodes.stringCodes.Add("right", "" + (char)26);
			FormatCodes.stringCodes.Add("left", "" + (char)27);
			FormatCodes.stringCodes.Add("2", "" + (char)28);
			FormatCodes.stringCodes.Add("3", "" + (char)29);
			FormatCodes.stringCodes.Add("up-tri", "" + (char)30);
			FormatCodes.stringCodes.Add("down-tri", "" + (char)31);
			FormatCodes.stringCodes.Add("house", "" + (char)127);
			FormatCodes.stringCodes.Add("<", "<");
			FormatCodes.stringCodes.Add("n", "\n");
			FormatCodes.stringCodes.Add("p", "" + ParagraphCharacter);
		}
		
		//-----------------------------------------------------------------------------
		// Codes
		//-----------------------------------------------------------------------------

		// Gets the string code type.
		public static FormatCodeType GetFormatCodeType(string code) {
			if (colorCodes.ContainsKey(code) || code.StartsWith("color:"))
				return FormatCodeType.Color;
			else if (stringCodes.ContainsKey(code))
				return FormatCodeType.String;
			else if (code.StartsWith("var:"))
				return FormatCodeType.Variable;
			return FormatCodeType.None;
		}

		// Gets the string representation of the code.
		public static string GetFormatCodeString(string code) {
			if (stringCodes.ContainsKey(code)) {
				return stringCodes[code];
			}
			else if (code.StartsWith("var:")) {
				// return the variable as text
			}
			return "";
		}

		// Gets the color of the color code.
		public static Color GetFormatCodeColor(string code) {
			if (colorCodes.ContainsKey(code)) {
				return colorCodes[code];
			}
			else if (code.StartsWith("color:")) {
				return Color.Parse(code.Substring(("color:").Length));
			}
			return Letter.DefaultColor;
		}

		// Returns the text formatted without any format codes in.
		public static LetterString FormatString(string text) {
			int caretPosition = 0;
			return FormatString(text, ref caretPosition);
		}
		// Returns the text formatted without any format codes in.
		public static LetterString FormatString(string text, ref int caretPosition) {
			int originalCaretPosition = caretPosition;
			int currentCharacter = 0;
			Color currentColor = Letter.DefaultColor;
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
						if (codeType == FormatCodeType.String || codeType == FormatCodeType.Variable) {
							string stringCode = FormatCodes.GetFormatCodeString(currentCode);
							letterString.AddRange(stringCode, currentColor);
						}
						else if (codeType == FormatCodeType.Color) {
							Color colorCode = FormatCodes.GetFormatCodeColor(currentCode);
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

		public static IEnumerable<KeyValuePair<string, Color>> GetColorCodes() {
			foreach (var pair in colorCodes) {
				yield return pair;
			}
		}
		public static IEnumerable<KeyValuePair<string, string>> GetStringCodes() {
			foreach (var pair in stringCodes) {
				yield return pair;
			}
		}
	}
}
