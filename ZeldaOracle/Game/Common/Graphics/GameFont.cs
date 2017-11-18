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
			int caretLine = 0;
			return WrapString(text, width, 0, out caretLine);
		}

		// Returns the wrapped and formatted string of the text.
		public WrappedLetterString WrapString(string text, int width, int caretPosition, out int caretLine) {
			try {
				caretLine = -1;

				List<LetterString> lines = new List<LetterString>();
				List<int> lineLengths = new List<int>();
				int currentLine = 0;
				int currentCharacter = 0;

				LetterString word = new LetterString();
				int wordStart = 0;
				int wordLength = 0;
				int wordLineCount = 0;
				bool firstChar = true;

				//caretPosition = Math.Min(text.Length, caretPosition + 1);

				string caretChar = (caretPosition >= text.Length ? "end" : "" + text[caretPosition]);
				char[] charArray = text.ToCharArray();

				LetterString letterString = FormatCodes.FormatString(text, ref caretPosition);
				string caret2Char = (caretPosition >= letterString.Length ? "end" : "" + letterString[caretPosition].Char);
				Console.WriteLine("'" + caretChar + "' - '" + caret2Char + "'");

				while (currentCharacter < letterString.Length) {
					lines.Add(new LetterString());
					lineLengths.Add(0);

					// Remove starting spaces in the line.
					while (currentCharacter < letterString.Length && letterString[currentCharacter].Char == ' ') {
						if (currentCharacter == caretPosition)
							caretLine = currentLine;
						currentCharacter++;
					}
					if (currentCharacter >= letterString.Length) {
						break;
					}

					wordStart = currentCharacter;
					word.Clear();
					wordLength = 0;
					wordLineCount = 0;
					firstChar = true;

					do {
						if (currentCharacter >= letterString.Length || letterString[currentCharacter].Char == ' ' ||
							letterString[currentCharacter].Char == FormatCodes.ParagraphCharacter || letterString[currentCharacter].Char == '\n') {
							if (wordLineCount > 0)
								lines[currentLine].Add(' ');
							lines[currentLine].AddRange(word);
							lineLengths[currentLine] += (wordLineCount > 0 ? (characterSpacing + spriteSheet.CellSize.X) : 0) + wordLength;

							wordLineCount++;
							wordLength = 0;
							wordStart = currentCharacter + 1;
							word.Clear();
							if (currentCharacter < letterString.Length &&
								(letterString[currentCharacter].Char == FormatCodes.ParagraphCharacter || letterString[currentCharacter].Char == '\n')) {
								if (letterString[currentCharacter].Char == FormatCodes.ParagraphCharacter)
									lines[currentLine].Add(letterString[currentCharacter]);
								if (currentCharacter == caretPosition)
									caretLine = currentLine;// + (letterString[currentCharacter].Char == '\n' ? 1 : 0);
								currentCharacter++;
								break;
							}
						}
						/*else if (lineLengths[currentLine] + wordLength + characterSpacing + spriteSheet.CellSize.X > width && wordStart == lineStart) {
							// Cuttoff a word if it has continued since the beginning of the line
							word[word.Length - 1] = new Letter('-', word[word.Length - 2].Color);
							wordStart = currentCharacter - 1;
						}*/
						else {
							word.Add(letterString[currentCharacter]);
							wordLength += (firstChar ? 0 : characterSpacing) + spriteSheet.CellSize.X;
							firstChar = false;
						}
						if (currentCharacter == caretPosition)
							caretLine = currentLine;
						currentCharacter++;
					} while (lineLengths[currentLine] + wordLength + characterSpacing + spriteSheet.CellSize.X <= width);

					if (lineLengths[currentLine] + wordLength + characterSpacing + spriteSheet.CellSize.X > width && wordLineCount == 0) {
						// Finish the word if it lasted the length if the line
						if (currentCharacter >= letterString.Length || letterString[currentCharacter].Char == ' ' ||
							letterString[currentCharacter].Char == FormatCodes.ParagraphCharacter || letterString[currentCharacter].Char == '\n') {
							if (wordLineCount > 0)
								lines[currentLine].Add(' ');
							lines[currentLine].AddRange(word);
							lineLengths[currentLine] += (wordLineCount > 0 ? (characterSpacing + spriteSheet.CellSize.X) : 0) + wordLength;

							wordLineCount++;
							wordLength = 0;
							wordStart = currentCharacter + 1;
						}
						else {
							// Cuttoff a word if it has continued since the beginning of the line
							word[word.Length - 1] = new Letter('-', word[word.Length - 2].Color);
							wordStart = currentCharacter - 1;
							lines[currentLine].AddRange(word);
							lineLengths[currentLine] = wordLength;
						}
					}

					currentCharacter = wordStart;
					currentLine++;
				}

				if (caretLine == -1 || caretLine >= lines.Count) {
					caretLine = lines.Count - 1;
				}

				WrappedLetterString wrappedString = new WrappedLetterString();
				wrappedString.Lines = lines.ToArray();
				wrappedString.LineLengths = lineLengths.ToArray();
				wrappedString.Bounds = new Rectangle2I(width, (lines.Count - 1) * lineSpacing + spriteSheet.CellSize.Y);
				return wrappedString;
			}
			catch (Exception e) {
				throw e;
			}
		}

	}
}
