using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;

namespace ZeldaOracle.Common.Translation {

	// The string code types available
	public enum StringCodeType {
		None = 0,
		Color = 1,
		String = 2,
		Variable = 3
	}

	// A string with formatted codes and color positions
	public class FormattedString {

		// The formatted string text
		public String Text;
		//The colors of each character
		public Color[] Colors;

		public FormattedString() {
			this.Text		= "";
			this.Colors		= null;
		}
	}

	// A string formatted and wrapped into multiple lines
	public class WrappedString {

		// The formatted lines of the wrapped string
		public FormattedString[] Lines;
		// The lengths of each line
		public int[] LineLengths;
		// The bounds of the entire wrapped string
		public Rectangle2I Bounds;

		public WrappedString() {
			this.Lines = null;
			this.LineLengths = null;
			this.Bounds = Rectangle2I.Zero;
		}
	}

	public class StringCodes {

		private static Dictionary<string, Color> colorCodes;
		private static Dictionary<string, string> stringCodes;

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public static void Initialize() {

			StringCodes.colorCodes			= new Dictionary<string, Color>();
			StringCodes.stringCodes			= new Dictionary<string, string>();

			StringCodes.colorCodes.Add("red", new Color(248, 8, 40));
			StringCodes.colorCodes.Add("green", new Color(16, 168, 64));
			StringCodes.colorCodes.Add("blue", new Color(24, 128, 248));
			StringCodes.colorCodes.Add("orange", new Color(248, 120, 8));

			StringCodes.stringCodes.Add("triangle", "" + (char)1);
			StringCodes.stringCodes.Add("square", "" + (char)2);
			StringCodes.stringCodes.Add("heart", "" + (char)3);
			StringCodes.stringCodes.Add("diamond", "" + (char)4);
			StringCodes.stringCodes.Add("club", "" + (char)5);
			StringCodes.stringCodes.Add("spade", "" + (char)6);
			StringCodes.stringCodes.Add("circle", "" + (char)7);
			StringCodes.stringCodes.Add("male", "" + (char)11);
			StringCodes.stringCodes.Add("female", "" + (char)12);
			StringCodes.stringCodes.Add("music", "" + (char)13);
			StringCodes.stringCodes.Add("music2", "" + (char)14);
			StringCodes.stringCodes.Add("rupee", "" + (char)15);
			StringCodes.stringCodes.Add("right2", "" + (char)16);
			StringCodes.stringCodes.Add("left2", "" + (char)17);
			StringCodes.stringCodes.Add("invalid", "" + (char)18);
			StringCodes.stringCodes.Add("!!", "" + (char)19);
			StringCodes.stringCodes.Add("cursor", "" + (char)22);
			StringCodes.stringCodes.Add("1", "" + (char)23);
			StringCodes.stringCodes.Add("up", "" + (char)24);
			StringCodes.stringCodes.Add("down", "" + (char)25);
			StringCodes.stringCodes.Add("right", "" + (char)26);
			StringCodes.stringCodes.Add("left", "" + (char)27);
			StringCodes.stringCodes.Add("2", "" + (char)28);
			StringCodes.stringCodes.Add("3", "" + (char)29);
			StringCodes.stringCodes.Add("up2", "" + (char)30);
			StringCodes.stringCodes.Add("down2", "" + (char)31);
			StringCodes.stringCodes.Add("<", "<");
		}

		//-----------------------------------------------------------------------------
		// Codes
		//-----------------------------------------------------------------------------

		// Gets the string code type
		public static StringCodeType GetStringCodeType(string code) {
			if (colorCodes.ContainsKey(code) || code.StartsWith("color:"))
				return StringCodeType.Color;
			else if (stringCodes.ContainsKey(code))
				return StringCodeType.String;
			else if (code.StartsWith("var:"))
				return StringCodeType.Variable;
			return StringCodeType.None;
		}

		// Gets the string representation of the code
		public static string GetStringCode(string code) {
			if (stringCodes.ContainsKey(code)) {
				return stringCodes[code];
			}
			else if (code.StartsWith("var:")) {
				// return the variable as text
			}
			return "";
		}

		public static Color GetStringColor(string code) {
			if (colorCodes.ContainsKey(code))
				return colorCodes[code];
			return Color.Black;
		}

		// Returns the text formatted with string and variable codes but still leaving color codes in
		public static FormattedString FormatText(string text) {
			int currentCharacter = 0;

			bool inCode = false;
			string currentCode = "";
			FormattedString formattedString = new FormattedString();
			List<Color> colors = new List<Color>();
			Color currentColor = Color.Black;

			while (currentCharacter < text.Length) {
				if (!inCode && text[currentCharacter] == '<') {
					// Start string code
					inCode = true;
				}
				else if (inCode) {
					if (text[currentCharacter] == '>') {
						// End string code
						if (StringCodes.GetStringCodeType(currentCode) != StringCodeType.Color) {
							string stringCode = StringCodes.GetStringCode(currentCode);
							formattedString.Text += stringCode;
							for (int i = 0; i < stringCode.Length; i++) {
								colors.Add(currentColor);
							}
						}
						else {
							Color colorCode = StringCodes.GetStringColor(currentCode);
							if (colorCode == currentColor)
								currentColor = Color.Black;
							else
								currentColor = colorCode;
						}
						inCode = false;
						currentCode = "";
					}
					else {
						currentCode += text[currentCharacter];
					}
				}
				else {
					formattedString.Text += text[currentCharacter];
					colors.Add(currentColor);
				}
				currentCharacter++;
			}

			formattedString.Colors = colors.ToArray();
			return formattedString;
		}
	}
}
