using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Common.Translation;
using ZeldaOracle.Game.Control;
using ZeldaOracle.Game.GameStates;
using ZeldaOracle.Game.Main;

namespace ZeldaOracle.Game.GameStates.RoomStates {

	// A game state for displaying a message box
	public class RoomStateTextReader : RoomState {

		// The states the text reader can be in.
		private enum TextReaderState {
			WritingLine,
			PushingLine,
			PressToContinue,
			PressToEndParagraph,
			HeartPieceDelay,
			Finished
		}

		// The game message with text and questions.
		private Message message;
		// The wrapped and formatted lines.
		private WrappedLetterString wrappedString;
		// The timer for the transitions.
		private int timer;
		// The number of lines to use for this window.
		private int linesPerWindow;
		// The lines left before the next set of lines is in the message box.
		private int windowLinesLeft;
		// The current window line of the line being written.
		private int windowLine;
		// The current line in the wrapped string.
		private int currentLine;
		// The current character of the current line.
		private int currentChar;
		// The current state of the text reader.
		private TextReaderState state;
		// The timer used to update the arrow sprite.
		private int arrowTimer;

		private int wordIndex;
		/// <summary>True if the heart piece UI is being displayed.</summary>
		private bool heartPieceDisplay;
		/// <summary>The internal counter used to display
		/// the pieces of heart before and after.</summary>
		private int piecesOfHeart;


		//-----------------------------------------------------------------------------
		// Constants
		//-----------------------------------------------------------------------------

		// The default color used by the text reader.
		private readonly ColorOrPalette TextColor = EntityColors.Tan;
		/// <summary>The delay before the heart piece increments.</summary>
		private const int HeartPieceDelay = 30;
		/// <summary>The delay after the heart piece increments.</summary>
		private const int HeartPieceParagraphDelay = 2;

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		// Constructs a text reader with the specified message
		public RoomStateTextReader(Message message, int linesPerWindow = 2, int piecesOfHeart = 0) {
			this.updateRoom			= false;
			this.animateRoom		= true;

			this.message			= message;
			this.wrappedString		= GameData.FONT_LARGE.WrapString(message.Text, 128);
			this.timer				= 0;
			this.arrowTimer			= 0;

			this.linesPerWindow		= linesPerWindow;
			this.windowLinesLeft	= this.linesPerWindow;
			this.windowLine			= 0;
			this.currentLine		= 0;
			this.currentChar		= 0;
			this.state				= TextReaderState.WritingLine;

			this.heartPieceDisplay	= false;
			this.piecesOfHeart	= piecesOfHeart;
		}

		// Constructs a text reader with the specified message text
		public RoomStateTextReader(string text, int linesPerWindow = 2, int piecesOfHeart = 0) 
			: this(new Message(text), linesPerWindow, piecesOfHeart) { }
		

		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnBegin() {
			timer = 1;
			state = TextReaderState.WritingLine;
			windowLinesLeft = linesPerWindow;
			wordIndex = 0;
		}

		public override void Update() {
			if (timer > 0 && (state != TextReaderState.WritingLine ||
				(!heartPieceDisplay && !Controls.A.IsPressed() && !Controls.B.IsPressed())))
			{
				timer -= 1;
			}
			else {
				switch (state) {
				case TextReaderState.WritingLine:

					char c = wrappedString.Lines[currentLine][currentChar].Char;
					bool isLetter = (c != ' ');
					if (AudioSystem.IsSoundPlaying(GameData.SOUND_TEXT_CONTINUE))
						wordIndex = 0;
					else {
						if (isLetter && (wordIndex % 2) == 0)
							AudioSystem.PlaySound(GameData.SOUND_TEXT_LETTER);
						if (isLetter)
							wordIndex++;
					}

					currentChar++;
					if (Controls.A.IsPressed() || Controls.B.IsPressed())
						currentChar = wrappedString.Lines[currentLine].Length;

					if (currentChar >= wrappedString.Lines[currentLine].Length) {
						windowLinesLeft--;
						if (currentLine + 1 == wrappedString.LineCount) {
							state = TextReaderState.Finished;
						}
						else if (wrappedString.Lines[currentLine].EndsWith(FormatCodes.ParagraphCharacter)) {
							state = TextReaderState.PressToEndParagraph;
							arrowTimer = 0;
						}
						else if (wrappedString.Lines[currentLine].EndsWith(FormatCodes.HeartPieceCharacter)) {
							state = TextReaderState.HeartPieceDelay;
							heartPieceDisplay = true;
							timer = HeartPieceDelay;
						}
						else if (windowLinesLeft == 0) {
							state = TextReaderState.PressToContinue;
							arrowTimer = 0;
						}
						else if (windowLine + 1 < linesPerWindow) {
							windowLine++;
							currentLine++;
							currentChar = 0;
						}
						else {
							currentLine++;
							currentChar = 0;
							state = TextReaderState.PushingLine;
							timer = 4;
						}
					}
					else {
						timer = 1;
					}
					break;

				case TextReaderState.HeartPieceDelay:
					AudioSystem.PlaySound(GameData.SOUND_TEXT_CONTINUE);
					piecesOfHeart++;
					timer = 2;
					if (currentLine + 1 == wrappedString.LineCount) {
						state = TextReaderState.Finished;
					}
					else {
						state = TextReaderState.PressToEndParagraph;
					}
					break;

				case TextReaderState.PushingLine:
					state = TextReaderState.WritingLine;
					break;

				case TextReaderState.PressToContinue:
					arrowTimer++;
					if (arrowTimer == 32)
						arrowTimer = 0;
					if (Controls.A.IsPressed() || Controls.B.IsPressed()) {
						state = TextReaderState.PushingLine;
						timer = 4;
						windowLinesLeft = linesPerWindow;
						currentChar = 0;
						currentLine++;
						heartPieceDisplay = false;
						AudioSystem.PlaySound(GameData.SOUND_TEXT_CONTINUE);
					}
					break;
				case TextReaderState.PressToEndParagraph:
					arrowTimer++;
					if (arrowTimer == 32)
						arrowTimer = 0;
					if (Controls.A.IsPressed() || Controls.B.IsPressed()) {
						state = TextReaderState.WritingLine;
						windowLinesLeft = linesPerWindow;
						currentChar = 0;
						windowLine = 0;
						currentLine++;
						heartPieceDisplay = false;
					}
					break;
				case TextReaderState.Finished:
					// TODO: Switch to any key
					if (Controls.A.IsPressed() || Controls.B.IsPressed() ||
						Controls.Start.IsPressed() || Controls.Select.IsPressed())
					{
						heartPieceDisplay = false;
						End();
					}
					break;
				}
			}
		}

		public override void Draw(Graphics2D g) {
			Point2I pos = new Point2I(8, 24);
			if (GameControl.Player.Position.Y < ((GameSettings.VIEW_HEIGHT) / 2 + 8))
				pos.Y = 96;
			// TODO: Apply Player position based on view
			g.FillRectangle(new Rectangle2I(pos, new Point2I(144, 8 + 16 * linesPerWindow)), EntityColors.Black);

			// Draw the finished writting lines.
			for (int i = 0; i < windowLine; i++) {
				if (state == TextReaderState.PushingLine && timer >= 2)
					g.DrawLetterString(GameData.FONT_LARGE, wrappedString.Lines[currentLine - windowLine + i], pos + new Point2I(8, 6 + 16 * i + 8), TextColor);
				else
					g.DrawLetterString(GameData.FONT_LARGE, wrappedString.Lines[currentLine - windowLine + i], pos + new Point2I(8, 6 + 16 * i), TextColor);
			}
			// Draw the currently writting line.
			g.DrawLetterString(GameData.FONT_LARGE, wrappedString.Lines[currentLine].Substring(0, currentChar), pos + new Point2I(8, 6 + 16 * windowLine), TextColor);

			// Draw the next line arrow.
			if ((state == TextReaderState.PressToContinue || state ==  TextReaderState.PressToEndParagraph) && arrowTimer >= 16 && timer == 0)
				g.DrawSprite(GameData.SPR_HUD_TEXT_NEXT_ARROW, pos + new Point2I(136, 16 * linesPerWindow));

			if (heartPieceDisplay) {
				Point2I heartPiecePos = pos + new Point2I(144 - 24, 8);
				for (int i = 0; i < 4; i++) {
					ISprite sprite = GameData.SPR_HUD_MESSAGE_HEART_PIECES_EMPTY[i];
					if (piecesOfHeart > i)
						sprite = GameData.SPR_HUD_MESSAGE_HEART_PIECES_FULL[i];
					g.DrawSprite(sprite, heartPiecePos);
				}
			}
		}

	}
}
