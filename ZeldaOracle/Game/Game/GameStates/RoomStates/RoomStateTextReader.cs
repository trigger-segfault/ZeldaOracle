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

	/// <summary>The positions the text reader window can be in.</summary>
	public enum TextReaderPosition {
		Unset = -1,
		Top,
		TopMiddle,
		BottomMiddle,
		Bottom
	}

	/// <summary>A game state for displaying a message box.</summary>
	public class RoomStateTextReader : RoomState {

		//-----------------------------------------------------------------------------
		// Enumerations
		//-----------------------------------------------------------------------------

		/// <summary>The states the text reader can be in.</summary>
		private enum TextReaderState {
			WritingLine,
			PushingLine,
			PressToContinue,
			PressToEndParagraph,
			HeartPieceDelay,
			Finished
		}


		//-----------------------------------------------------------------------------
		// Constants
		//-----------------------------------------------------------------------------

		/// <summary>The default color used by the text reader.</summary>
		private readonly ColorOrPalette TextColor = EntityColors.Tan;
		/// <summary>The delay before the heart piece increments.</summary>
		private const int HeartPieceDelay = 30;
		/// <summary>The delay after the heart piece increments.</summary>
		private const int HeartPieceParagraphDelay = 2;


		//-----------------------------------------------------------------------------
		// Members
		//-----------------------------------------------------------------------------

		// Message
		/// <summary>The game message with text and questions.</summary>
		private Message message;
		/// <summary>The wrapped and formatted lines.</summary>
		private WrappedLetterString wrappedString;

		// Settings
		/// <summary>The number of lines to use for this window.</summary>
		private int linesPerWindow;
		/// <summary>The position of the text reader on the screen.</summary>
		private TextReaderPosition readerPosition;

		// Updating
		/// <summary>The current state of the text reader.</summary>
		private TextReaderState state;
		/// <summary>Used to prevent control presses from activating on the first step.</summary>
		private bool firstUpdate;
		/// <summary>The timer for the transitions.</summary>
		private int timer;
		/// <summary>The timer used to update the arrow sprite.</summary>
		private int arrowTimer;
		/// <summary>The lines left before the next set of lines is in the message box.</summary>
		private int windowLinesLeft;
		/// <summary>The current window line of the line being written.</summary>
		private int windowLine;
		/// <summary>The current line in the wrapped string.</summary>
		private int currentLine;
		/// <summary>The current character of the current line.</summary>
		private int currentChar;
		/// <summary>Used to play the letter sound every other letter in a word.</summary>
		private int wordIndex;

		// Piece of Heart
		/// <summary>True if the heart piece UI is being displayed.</summary>
		private bool heartPieceDisplay;
		/// <summary>The internal counter used to display
		/// the pieces of heart before and after.</summary>
		private int piecesOfHeart;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Constructs a text reader with the specified message.</summary>
		public RoomStateTextReader(Message message, int linesPerWindow = 2, int piecesOfHeart = 0,
			TextReaderPosition readerPosition = TextReaderPosition.Unset)
		{
			this.updateRoom			= false;
			this.animateRoom		= true;

			this.message			= message;
			this.wrappedString		= null;

			this.linesPerWindow		= linesPerWindow;
			this.readerPosition		= readerPosition;

			this.state				= TextReaderState.WritingLine;
			this.firstUpdate		= true;
			this.timer				= 0;
			this.arrowTimer			= 0;
			this.windowLinesLeft	= this.linesPerWindow;
			this.windowLine			= 0;
			this.currentLine		= 0;
			this.currentChar        = 0;
			this.wordIndex			= 0;
			
			this.heartPieceDisplay	= false;
			this.piecesOfHeart		= piecesOfHeart;
		}

		/// <summary>Constructs a text reader with the specified message text.</summary>
		public RoomStateTextReader(string text, int linesPerWindow = 2, int piecesOfHeart = 0,
			TextReaderPosition readerPosition = TextReaderPosition.Unset)
			: this(new Message(text), linesPerWindow, piecesOfHeart, readerPosition) { }
		

		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnBegin() {
			wrappedString	= GameData.FONT_LARGE.WrapString(
				message.Text, 128, GameControl.Variables);
			timer = 1;
			state = TextReaderState.WritingLine;
			windowLinesLeft	= linesPerWindow;
			windowLine		= 0;
			currentLine		= 0;
			currentChar		= 0;
			wordIndex		= 0;

			if (readerPosition == TextReaderPosition.Unset) {

				if (GameControl.Player.ViewPosition.Y <
					((GameSettings.VIEW_HEIGHT) / 2 + 8))
					readerPosition = TextReaderPosition.Bottom;
				else
					readerPosition = TextReaderPosition.Top;
			}
		}

		public override void Update() {
			if (timer > 0 && (state != TextReaderState.WritingLine || firstUpdate ||
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

			firstUpdate = false;
		}

		public override void Draw(Graphics2D g) {
			g.PushTranslation(0, GameSettings.HUD_HEIGHT);

			Point2I pos = new Point2I(8, 0);
			switch (readerPosition) {
			case TextReaderPosition.Top:
				pos.Y = 8;
				break;
			case TextReaderPosition.TopMiddle:
				pos.Y = (GameSettings.VIEW_HEIGHT - 16 * (linesPerWindow + 1)) / 2;
				break;
			case TextReaderPosition.BottomMiddle:
				pos.Y = (GameSettings.VIEW_HEIGHT - 16 * linesPerWindow) / 2;
				break;
			case TextReaderPosition.Bottom:
				pos.Y = (GameSettings.VIEW_HEIGHT - 16 * (linesPerWindow + 1));
				break;
			}

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

			g.PopTranslation();
		}

	}
}
