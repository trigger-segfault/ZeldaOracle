using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Translation;
using ZeldaOracle.Game.GameStates;
using ZeldaOracle.Game.Main;

namespace ZeldaOracle.Game.Control {
	public enum TextReaderState {
		WritingLine,
		PushingLine,
		PressToContinue,
		PressToEndParagraph,
		Finished
	}


	public class StateTextReader : GameState {

		private Message message;
		private WrappedString lines;
		private int timer;
		private int linesPerWindow;
		private int windowLinesLeft;
		private int windowLine;
		private int currentLine;
		private int currentChar;
		private TextReaderState state;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public StateTextReader(Message message) {
			this.message = message;
			this.lines = GameData.FONT_LARGE.MeasureWrappedString(message.Text, 128);
			this.timer = 0;


			this.linesPerWindow = 4;
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

		public override void Update(float timeDelta) {
			if (timer > 0) {
				timer -= 1;
			}
			else {
				switch (state) {
				case TextReaderState.WritingLine:
					currentChar++;
					if (Controls.A.IsPressed() || Controls.B.IsPressed()) {
						currentChar = lines.Lines[currentLine].Text.Length;
					}
					if (currentChar >= lines.Lines[currentLine].Text.Length) {
						windowLinesLeft--;
						if (currentLine + 1 == lines.Lines.Length) {
							state = TextReaderState.Finished;
						}
						else if (lines.Lines[currentLine].Text[lines.Lines[currentLine].Text.Length - 1] == StringCodes.ParagraphCharacter) {
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
					if (Controls.A.IsPressed()) {
						state = TextReaderState.PushingLine;
						timer = 4;
						windowLinesLeft = linesPerWindow;
						currentChar = 0;
						currentLine++;
					}
					break;
				case TextReaderState.PressToEndParagraph:
					if (Controls.A.IsPressed()) {
						state = TextReaderState.WritingLine;
						windowLinesLeft = linesPerWindow;
						currentChar = 0;
						windowLine = 0;
						currentLine++;
					}
					break;
				case TextReaderState.Finished:
					if (Controls.A.IsPressed())
						End();
					break;
				}
			}
		}

		public override void Draw(Graphics2D g) {
			Point2I pos = new Point2I(8, 24);
			g.FillRectangle(new Rectangle2F(8, 24, 144, 8 + 16 * linesPerWindow), Color.Black);

			for (int i = 0; i < windowLine; i++) {
				if (state == TextReaderState.PushingLine && timer >= 2)
					g.DrawFormattedGameString(GameData.FONT_LARGE, lines.Lines[currentLine - windowLine + i], pos + new Point2I(8, 6 + 16 * i + 8), new Color(248, 208, 136));
				else
					g.DrawFormattedGameString(GameData.FONT_LARGE, lines.Lines[currentLine - windowLine + i], pos + new Point2I(8, 6 + 16 * i), new Color(248, 208, 136));
			}
			string oldText = lines.Lines[currentLine].Text;
			lines.Lines[currentLine].Text = lines.Lines[currentLine].Text.Substring(0, currentChar);
			g.DrawFormattedGameString(GameData.FONT_LARGE, lines.Lines[currentLine], pos + new Point2I(8, 6 + 16 * windowLine), new Color(248, 208, 136));
			lines.Lines[currentLine].Text = oldText;
		}

	}
}
