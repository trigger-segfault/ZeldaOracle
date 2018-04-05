using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Common.Translation;
using ZeldaOracle.Game;

namespace ZeldaOracle.Common.Graphics {
	/// <summary>A monospaced sprite font.</summary>
	public class GameFont {

		/// <summary>The sprite sheet of the font.</summary>
		private SpriteSheet spriteSheet;

		/// <summary>The number of characters in each row in the sprite sheet.</summary>
		private int charactersPerRow;
		/// <summary>The spacing between characters.</summary>
		private int characterSpacing;
		/// <summary>The spacing between lines.</summary>
		private int lineSpacing;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Constructs a font with the specified sprite sheet.</summary>
		public GameFont(SpriteSheet spriteSheet) {
			this.spriteSheet		= spriteSheet;
			this.charactersPerRow	= 0;
			this.characterSpacing	= 0;
			this.lineSpacing		= 0;
		}

		/// <summary>Constructs a font with the specified sprite sheet.</summary>
		public GameFont(SpriteSheet spriteSheet, int charactersPerRow, int characterSpacing, int lineSpacing) {
			this.spriteSheet		= spriteSheet;
			this.charactersPerRow	= charactersPerRow;
			this.characterSpacing	= characterSpacing;
			this.lineSpacing		= lineSpacing;
		}


		//-----------------------------------------------------------------------------
		// Characters
		//-----------------------------------------------------------------------------

		/// <summary>A helper method to get the source rect for the specified character.</summary>
		public Rectangle2I GetCharacterCell(char character) {
			int index = (int) character;
			if (index < charactersPerRow * spriteSheet.Height)
				return spriteSheet.GetSourceRect(index % charactersPerRow, index / charactersPerRow);
			return spriteSheet.GetSourceRect(Point2I.Zero);
		}
		

		//-----------------------------------------------------------------------------
		// Sprites
		//-----------------------------------------------------------------------------

		/// <summary>Gets the sprite for the specified character.</summary>
		public BasicSprite GetSprite(char character) {
			int index = (int) character;
			if (index < charactersPerRow * spriteSheet.Height)
				return spriteSheet.GetSprite(index % charactersPerRow, index / charactersPerRow);
			return spriteSheet.GetSprite(Point2I.Zero);
		}

		/// <summary>Gets the colored sprite for the specified character.</summary>
		public CanvasSprite GetSprite(char character, ColorOrPalette color) {
			return GetSprite("" + character, color);
		}

		/// <summary>Gets the colored sprite for the specified string.</summary>
		public CanvasSprite GetSprite(DrawableString text, ColorOrPalette color) {
			CanvasSprite canvas = new CanvasSprite(MeasureString(text));
			Graphics2D g = canvas.Begin(GameSettings.DRAW_MODE_PALLETE);
			g.DrawString(this, text, Vector2F.Zero, color);
			canvas.End(g);
			return canvas;
		}

		/// <summary>Gets the colored sprite for the specified formatted string.</summary>
		public CanvasSprite GetFormattedSprite(string text, ColorOrPalette color) {
			LetterString letterStr = FormatCodes.FormatString(text, null);
			return GetSprite(letterStr, color);
		}


		//-----------------------------------------------------------------------------
		// Measuring
		//-----------------------------------------------------------------------------

		/// <summary>Returns the dimensions of any type of drawable string.</summary>
		public Point2I MeasureString(DrawableString text) {
			if (text.IsString) {
				return new Point2I(
					GMath.Max(0, text.String.Length * (CharacterWidth +
						characterSpacing) - characterSpacing),
					CharacterHeight);
			}
			else if (text.IsLetterString) {
				return new Point2I(
					GMath.Max(0, text.LetterString.Length * (CharacterWidth +
						characterSpacing) - characterSpacing),
					CharacterHeight);
			}
			else if (text.IsWrappedLetterString) {
				WrappedLetterString wrapped = text.WrappedLetterString;
				int maxLength = 0;
				for (int i = 0; i < wrapped.LineCount; i++) {
					maxLength = GMath.Max(maxLength, wrapped.LineLengths[i]);
				}
				return new Point2I(
					maxLength,
					GMath.Max(0, wrapped.LineCount * (CharacterHeight +
						lineSpacing) - lineSpacing));
			}
			return Point2I.Zero;
		}


		//-----------------------------------------------------------------------------
		// Wrapping
		//-----------------------------------------------------------------------------

		/// <summary>Returns the wrapped and formatted string of the text.</summary>
		public WrappedLetterString WrapString(string text, int width,
			Variables vars = null)
		{
			int caretLine = 0;
			return WrapString(text, width, 0, out caretLine, vars);
		}

		/// <summary>Returns the wrapped and formatted string of the text.</summary>
		public WrappedLetterString WrapString(string text, int width, int caretPosition,
			out int caretLine, Variables vars = null)
		{
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
				Align align = Align.Left;

				//caretPosition = Math.Min(text.Length, caretPosition + 1);

				string caretChar = (caretPosition >= text.Length ? "end" : "" + text[caretPosition]);
				char[] charArray = text.ToCharArray();

				LetterString letterString = FormatCodes.FormatString(text, ref caretPosition, vars);
				string caret2Char = (caretPosition >= letterString.Length ? "end" : "" + letterString[caretPosition].Char);
				//Console.WriteLine("'" + caretChar + "' - '" + caret2Char + "'");

				while (currentCharacter < letterString.Length) {
					lines.Add(new LetterString());
					lines[currentLine].MessageAlignment = align;
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

					char c;

					do {
						c = (currentCharacter >= letterString.Length ?
							'\0' : letterString[currentCharacter].Char);

						if (currentCharacter >= letterString.Length || c == ' ' ||
							c == FormatCodes.ParagraphCharacter || c == '\n' ||
							c == FormatCodes.HeartPieceCharacter)
						{
							if (wordLineCount > 0)
								lines[currentLine].Add(' ');
							lines[currentLine].AddRange(word);
							lineLengths[currentLine] += (wordLineCount > 0 ? (characterSpacing + CharacterWidth) : 0) + wordLength;

							wordLineCount++;
							wordLength = 0;
							wordStart = currentCharacter + 1;
							word.Clear();
							if (currentCharacter < letterString.Length &&
								(c == FormatCodes.ParagraphCharacter || c == '\n' ||
								c == FormatCodes.HeartPieceCharacter))
							{
								if (c == FormatCodes.ParagraphCharacter ||
									c == FormatCodes.HeartPieceCharacter)
									lines[currentLine].Add(letterString[currentCharacter]);
								if (currentCharacter == caretPosition)
									caretLine = currentLine;// + (letterString[currentCharacter].Char == '\n' ? 1 : 0);
								currentCharacter++;
								break;
							}
							if (currentCharacter >= letterString.Length)
								break;
						}
						else if (c == FormatCodes.AlignLeftCharacter) {
							align = Align.Left;
							lines[currentLine].MessageAlignment = align;
						}
						else if (c == FormatCodes.AlignCenterCharacter) {
							align = Align.Center;
							lines[currentLine].MessageAlignment = align;
						}
						else if (c == FormatCodes.AlignRightCharacter) {
							align = Align.Right;
							lines[currentLine].MessageAlignment = align;
						}
						else {
							word.Add(letterString[currentCharacter]);
							wordLength += (firstChar ? 0 : characterSpacing) + CharacterWidth;
							firstChar = false;
						}
						if (currentCharacter == caretPosition)
							caretLine = currentLine;
						currentCharacter++;
					} while (lineLengths[currentLine] + wordLength + characterSpacing + CharacterWidth <= width || width < CharacterWidth);

					if (lineLengths[currentLine] + wordLength + characterSpacing + CharacterWidth > width && width >= CharacterWidth && wordLineCount == 0) {
						// Finish the word if it lasted the length if the line
						if (currentCharacter >= letterString.Length || letterString[currentCharacter].Char == ' ' ||
							letterString[currentCharacter].Char == FormatCodes.ParagraphCharacter || letterString[currentCharacter].Char == '\n' ||
							letterString[currentCharacter].Char == FormatCodes.HeartPieceCharacter) {
							if (wordLineCount > 0)
								lines[currentLine].Add(' ');
							lines[currentLine].AddRange(word);
							lineLengths[currentLine] += (wordLineCount > 0 ? (characterSpacing + CharacterWidth) : 0) + wordLength;

							wordLineCount++;
							wordLength = 0;
							wordStart = currentCharacter + 1;
						}
						else {
							// Cuttoff a word if it has continued since the beginning of the line
							word[word.Length - 1] = new Letter('-', word[word.Length - 1].Color);
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

				// Make a dummy line to prevent issues with the text reader.
				//if (lines.Count == 0)
				//	lines.Add(new LetterString());

				WrappedLetterString wrappedString = new WrappedLetterString();
				wrappedString.Lines = lines.ToArray();
				wrappedString.LineLengths = lineLengths.ToArray();
				wrappedString.Bounds = new Rectangle2I(width, (lines.Count - 1) * lineSpacing + CharacterHeight);
				return wrappedString;
			}
			catch (Exception e) {
				throw e;
			}
		}
		
		
		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets the sprite sheet of the font.</summary>
		public SpriteSheet SpriteSheet {
			get { return spriteSheet; }
		}

		/// <summary>Gets or sets the characters per row.</summary>
		public int CharactersPerRow {
			get { return charactersPerRow; }
			set { charactersPerRow = value; }
		}

		/// <summary>Gets or sets the spacing of the font characters.</summary>
		public int CharacterSpacing {
			get { return characterSpacing; }
			set { characterSpacing = value; }
		}

		/// <summary>Gets or sets the vertical distance (in pixels) between the
		/// base lines of two consecutive lines of text.</summary>
		public int LineSpacing {
			get { return lineSpacing; }
			set { lineSpacing = value; }
		}

		/// <summary>Gets the size of a single character.</summary>
		public Point2I CharacterSize {
			get { return spriteSheet.CellSize; }
		}

		/// <summary>Gets the width of a single character.</summary>
		public int CharacterWidth {
			get { return spriteSheet.CellSize.X; }
		}

		/// <summary>Gets the height of a single character.</summary>
		public int CharacterHeight {
			get { return spriteSheet.CellSize.Y; }
		}
	}
}
