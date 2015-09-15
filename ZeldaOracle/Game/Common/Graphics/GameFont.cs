using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Translation;

namespace ZeldaOracle.Common.Graphics {

	// A zelda sprite font
	public class GameFont {

		// The sprite sheet of the font.
		private SpriteSheet spriteSheet;

		// The number of characters in each row in the sprite sheet.
		private int charactersPerRow;
		// The spacing between characters.
		private int characterSpacing;
		// The spacing between lines.
		private int lineSpacing;

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		// Constructs a font with the specified sprite sheet.
		public GameFont(SpriteSheet spriteSheet) {
			this.spriteSheet		= spriteSheet;
			this.charactersPerRow	= 0;
			this.characterSpacing	= 0;
			this.lineSpacing		= 0;
		}

		// Constructs a font with the specified sprite sheet.
		public GameFont(SpriteSheet spriteSheet, int charactersPerRow, int characterSpacing, int lineSpacing) {
			this.spriteSheet		= spriteSheet;
			this.charactersPerRow	= charactersPerRow;
			this.characterSpacing	= characterSpacing;
			this.lineSpacing		= lineSpacing;
		}

		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		// Gets the sprite sheet of the font.
		public SpriteSheet SpriteSheet {
			get { return spriteSheet; }
		}
		// Gets or sets the characters per row.
		public int CharactersPerRow {
			get { return charactersPerRow; }
			set { charactersPerRow = value; }
		}
		// Gets or sets the spacing of the font characters.
		public int CharacterSpacing {
			get { return characterSpacing; }
			set { characterSpacing = value; }
		}
		// Gets or sets the vertical distance (in pixels) between the base lines of two consecutive lines of text.
		public int LineSpacing {
			get { return lineSpacing; }
			set { lineSpacing = value; }
		}

		//-----------------------------------------------------------------------------
		// Strings
		//-----------------------------------------------------------------------------

		// Returns the wrapped and formatted string of the text.
		public WrappedLetterString WrapString(string text, int width) {
			List<LetterString> lines = new List<LetterString>();
			List<int> lineLengths = new List<int>();
			int currentLine = 0;
			int currentCharacter = 0;

			LetterString word = new LetterString();
			int wordStart = 0;
			int wordLength = 0;
			int wordLineCount = 0;
			bool firstChar = true;

			LetterString letterString = FormatCodes.FormatString(text);

			while (currentCharacter < letterString.Length) {
				lines.Add(new LetterString());
				lineLengths.Add(0);

				// Remove starting spaces in the line.
				while (letterString[currentCharacter].Char == ' ') {
					currentCharacter++;
				}

				wordStart = currentCharacter;
				word.Clear();
				wordLength = 0;
				wordLineCount = 0;
				firstChar = true;

				do {
					if (currentCharacter >= letterString.Length || letterString[currentCharacter].Char == ' ' ||
						letterString[currentCharacter].Char == FormatCodes.ParagraphCharacter) {

						if (wordLineCount > 0)
							lines[currentLine].Add(' ');
						lines[currentLine].AddRange(word);
						lineLengths[currentLine] += (wordLineCount > 0 ? (characterSpacing + spriteSheet.CellSize.X) : 0) + wordLength;

						wordLineCount++;
						wordLength = 0;
						wordStart = currentCharacter + 1;
						word.Clear();
						if (currentCharacter < letterString.Length && letterString[currentCharacter].Char == FormatCodes.ParagraphCharacter) {
							lines[currentLine].Add(letterString[currentCharacter]);
							currentCharacter++;
							break;
						}
					}
					else {
						word.Add(letterString[currentCharacter]);
						wordLength += (firstChar ? 0 : characterSpacing) + spriteSheet.CellSize.X;
						firstChar = false;
					}
					currentCharacter++;
				} while (lineLengths[currentLine] + wordLength + characterSpacing + spriteSheet.CellSize.X <= width);

				currentCharacter = wordStart;
				currentLine++;
			}

			WrappedLetterString wrappedString = new WrappedLetterString();
			wrappedString.Lines = lines.ToArray();
			wrappedString.LineLengths = lineLengths.ToArray();
			wrappedString.Bounds = new Rectangle2I(width, (lines.Count - 1) * lineSpacing + spriteSheet.CellSize.Y);
			return wrappedString;
		}

	}
}
