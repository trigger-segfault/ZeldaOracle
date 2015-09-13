using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Common.Graphics {

	public struct WrappedString {
		public string[] Lines;
		public int[] LineLengths;
		public Rectangle2I Bounds;
	}

	/** <summary>
	 * An font containing a sprite font.
	 * </summary> */
	public class GameFont {

		//=========== MEMBERS ============

		/** <summary> The sprite sheet of the font. </summary> */
		private SpriteSheet spriteSheet;

		private Point2I characterSize;
		private int characterSpacing;
		private int lineSpacing;

		private Dictionary<string, Color> colorCodes;
		private Dictionary<string, string> letterCodes;

		//========= CONSTRUCTORS =========

		/** <summary> Constructs a font with the specified sprite font. </summary> */
		public GameFont(SpriteSheet spriteSheet, Point2I characterSize, int characterSpacing, int lineSpacing) {
			this.spriteSheet		= spriteSheet;
			this.characterSize		= characterSize;
			this.characterSpacing	= characterSpacing;
			this.lineSpacing		= lineSpacing;
		}

		//========== PROPERTIES ==========

		/** <summary> Gets or sets the size of the font characters. </summary> */
		public Point2I CharacterSize {
			get { return characterSize; }
			set { CharacterSize = value; }
		}
		/** <summary> Gets or sets the spacing of the font characters. </summary> */
		public int CharacterSpacing {
			get { return characterSpacing; }
			set { characterSpacing = value; }
		}
		/** <summary> Gets or sets the vertical distance (in pixels) between the base lines of two consecutive lines of text. </summary> */
		public int LineSpacing {
			get { return lineSpacing; }
			set { lineSpacing = value; }
		}

		//========== MANAGEMENT ==========

		public WrappedString MeasureWrappedString(string text, int width) {
			List<string> lines = new List<string>();
			List<int> lineLengths = new List<int>();
			int currentLine = 0;
			int currentCharacter = 0;

			int wordStart = 0;
			int wordLength = 0;
			int wordLineCount = 0;
			string word = "";


			while (currentCharacter < text.Length) {
				lines.Add("");
				lineLengths.Add(0);

				// Remove starting spaces.
				while (text[currentCharacter] == ' ') {
					currentCharacter++;
				}

				wordStart = currentCharacter;
				word = "" + text[currentCharacter];
				wordLength = characterSize.X;
				currentCharacter++;

				while (lineLengths[currentLine] + characterSpacing + characterSize.X <= width) {
					if (currentCharacter >= text.Length || text[currentCharacter] == ' ') {
						lines[currentLine] += (wordLineCount > 0 ? " " : "") + word;
						lineLengths[currentLine] += wordLength;
						wordLineCount++;
						currentCharacter++;
						wordStart = currentCharacter;
						break;
					}
					word += text[currentCharacter];
					wordLength += characterSpacing + characterSize.X;
					currentCharacter++;
				}
				currentCharacter = wordStart;
				wordLineCount = 0;
			}


			WrappedString wrappedString = new WrappedString();
			wrappedString.Lines = lines.ToArray();
			wrappedString.LineLengths = lineLengths.ToArray();
			wrappedString.Bounds = new Rectangle2I(width, (lines.Count - 1) * lineSpacing + characterSize.Y);
			return wrappedString;
		}

	}
}
