using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
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

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		// Constructs a text reader with the specified message
		public RoomStateTextReader(Message message, int linesPerWindow = 2) {
			this.updateRoom = false;
			this.animateRoom = true;

			this.message = message;
			this.wrappedString = GameData.FONT_LARGE.WrapString(message.Text, 128);
			this.timer = 0;

			this.linesPerWindow = linesPerWindow;
			this.windowLinesLeft = this.linesPerWindow;
			this.windowLine = 0;
			this.currentLine = 0;
			this.currentChar = 0;
			this.state = TextReaderState.WritingLine;
		}

		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnBegin() {
			timer = 1;
			state = TextReaderState.WritingLine;
			windowLinesLeft = linesPerWindow;
		}

		public override void Update() {
			if (timer > 0 && (state != TextReaderState.WritingLine || (!Controls.A.IsPressed() && !Controls.B.IsPressed()))) {
				timer -= 1;
			}
			else {
				switch (state) {
				case TextReaderState.WritingLine:
					currentChar++;
					if (Controls.A.IsPressed() || Controls.B.IsPressed())
						currentChar = wrappedString.Lines[currentLine].Length;

					if (currentChar >= wrappedString.Lines[currentLine].Length) {
						windowLinesLeft--;
						if (currentLine + 1 == wrappedString.NumLines) {
							state = TextReaderState.Finished;
						}
						else if (wrappedString.Lines[currentLine].EndsWith(FormatCodes.ParagraphCharacter)) {
							state = TextReaderState.PressToEndParagraph;
						}
						else if (windowLinesLeft == 0) {
							state = TextReaderState.PressToContinue;
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

				case TextReaderState.PushingLine:
					state = TextReaderState.WritingLine;
					break;

				case TextReaderState.PressToContinue:
					if (Controls.A.IsPressed() || Controls.B.IsPressed()) {
						state = TextReaderState.PushingLine;
						timer = 4;
						windowLinesLeft = linesPerWindow;
						currentChar = 0;
						currentLine++;
					}
					break;
				case TextReaderState.PressToEndParagraph:
					if (Controls.A.IsPressed() || Controls.B.IsPressed()) {
						state = TextReaderState.WritingLine;
						windowLinesLeft = linesPerWindow;
						currentChar = 0;
						windowLine = 0;
						currentLine++;
					}
					break;
				case TextReaderState.Finished:
					// TODO: Switch to any key
					if (Controls.A.IsPressed() || Controls.B.IsPressed() ||
						Controls.Start.IsPressed() || Controls.Select.IsPressed())
						End();
					break;
				}
			}
		}

		public override void Draw(Graphics2D g) {
			Point2I pos = new Point2I(8, 24);
			if (GameControl.Player.Y < ((GameSettings.VIEW_HEIGHT) / 2 + 1))
				pos.Y = 96;
			// TODO: Apply Player position based on view
			g.FillRectangle(new Rectangle2I(pos, new Point2I(144, 8 + 16 * linesPerWindow)), Color.Black);

			for (int i = 0; i < windowLine; i++) {
				if (state == TextReaderState.PushingLine && timer >= 2)
					g.DrawLetterString(GameData.FONT_LARGE, wrappedString.Lines[currentLine - windowLine + i], pos + new Point2I(8, 6 + 16 * i + 8), new Color(248, 208, 136));
				else
					g.DrawLetterString(GameData.FONT_LARGE, wrappedString.Lines[currentLine - windowLine + i], pos + new Point2I(8, 6 + 16 * i), new Color(248, 208, 136));
			}
			g.DrawLetterString(GameData.FONT_LARGE, wrappedString.Lines[currentLine].Substring(0, currentChar), pos + new Point2I(8, 6 + 16 * windowLine), new Color(248, 208, 136));
		}

	}
}
