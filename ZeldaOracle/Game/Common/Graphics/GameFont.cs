using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Translation;

namespace ZeldaOracle.Common.Graphics {


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

		//========= CONSTRUCTORS =========

		/** <summary> Constructs a font with the specified sprite font. </summary> */
		public GameFont(SpriteSheet spriteSheet, Point2I characterSize, int characterSpacing, int lineSpacing) {
			this.spriteSheet		= spriteSheet;
			this.characterSize		= characterSize;
			this.characterSpacing	= characterSpacing;
			this.lineSpacing		= lineSpacing;
		}

		//========== PROPERTIES ==========

		/** <summary> Gets the sprite sheet of the font </summary> */
		public SpriteSheet SpriteSheet {
			get { return spriteSheet; }
		}
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
			List<FormattedString> lines = new List<FormattedString>();
			List<Color> lineColors = new List<Color>();
			List<Color> wordColors = new List<Color>();
			List<int> lineLengths = new List<int>();
			int currentLine = 0;
			int currentCharacter = 0;

			int wordStart = 0;
			int wordLength = 0;
			int wordLineCount = 0;
			string word = "";
			bool firstChar = true;
			FormattedString formattedString = StringCodes.FormatText(text);

			while (currentCharacter < formattedString.Text.Length) {
				lines.Add(new FormattedString());
				lineLengths.Add(0);

				// Remove starting spaces in the line.
				while (formattedString.Text[currentCharacter] == ' ') {
					currentCharacter++;
				}

				wordStart = currentCharacter;
				word = "";
				wordLength = 0;
				wordLineCount = 0;
				firstChar = true;
				lineColors.Clear();
				wordColors.Clear();

				do {
					if (currentCharacter >= formattedString.Text.Length || formattedString.Text[currentCharacter] == ' ' ||
						formattedString.Text[currentCharacter] == StringCodes.ParagraphCharacter) {

						lines[currentLine].Text += (wordLineCount > 0 ? " " : "") + word;
						lineLengths[currentLine] += (wordLineCount > 0 ? (characterSpacing + characterSize.X) : 0) + wordLength;
						if (wordLineCount > 0)
							lineColors.Add(Color.Black);
						lineColors.AddRange(wordColors);

						wordColors.Clear();
						wordLineCount++;
						wordLength = 0;
						wordStart = currentCharacter + 1;
						word = "";
						if (currentCharacter < formattedString.Text.Length && formattedString.Text[currentCharacter] == StringCodes.ParagraphCharacter) {
							lines[currentLine].Text += StringCodes.ParagraphCharacter;
							lineColors.Add(Color.Black);
							currentCharacter++;
							break;
						}
					}
					else {
						word += formattedString.Text[currentCharacter];
						wordLength += (firstChar ? 0 : characterSpacing) + characterSize.X;
						wordColors.Add(formattedString.Colors[currentCharacter]);
						firstChar = false;
					}
					currentCharacter++;
				} while (lineLengths[currentLine] + wordLength + characterSpacing + characterSize.X <= width);

				currentCharacter = wordStart;
				lines[currentLine].Colors = lineColors.ToArray();
				currentLine++;
			}

			WrappedString wrappedString = new WrappedString();
			wrappedString.Lines = lines.ToArray();
			wrappedString.LineLengths = lineLengths.ToArray();
			wrappedString.Bounds = new Rectangle2I(width, (lines.Count - 1) * lineSpacing + characterSize.Y);
			return wrappedString;
		}

	}
}
