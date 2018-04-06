using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Common.Translation;
using ZeldaOracle.Common.Util;
using ZeldaOracle.Game.Control;
using ZeldaOracle.Game.Main;

namespace ZeldaOracle.Game.GameStates.RoomStates {

	/// <summary>The positions the text reader window can be in.</summary>
	public enum TextReaderPosition {
		Automatic = -1,
		Top,
		TopMiddle,
		BottomMiddle,
		Bottom,
	}

	/// <summary>Arguments for the room state text reader.</summary>
	public class TextReaderArgs {
		/// <summary>The spacing between the window vertical window edges and the text.
		/// </summary>
		public int Spacing { get; set; }
		/// <summary>The position of the text reader on the screen.</summary>
		public TextReaderPosition ReaderPosition { get; set; }
		/// <summary>The number of lines to use for this window.</summary>
		public int LinesPerWindow { get; set; }

		/// <summary>Default text reader arguments, with 1 character spacing, 2 lines
		/// per window, and an automatic reader positio.</summary>
		public static readonly TextReaderArgs Default = new TextReaderArgs() {
			ReaderPosition = TextReaderPosition.Automatic,
			Spacing = 1,
			LinesPerWindow = 2,
		};
	}

	public delegate void TextReaderCallback(int menuOptionIndex);

	/// <summary>A game state for displaying a message box.</summary>
	public class RoomStateTextReader : RoomState {

		//-----------------------------------------------------------------------------
		// Internal Types
		//-----------------------------------------------------------------------------

		/// <summary>The states the text reader can be in.</summary>
		private enum TextReaderState {
			WritingLine,
			PushingLine,
			PressToContinue,
			PressToEndParagraph,
			ChoosingOption,
			HeartPieceDelay,
			Finished,
		}

		/// <summary>A selectable menu option.</summary>
		private class MenuOption {
			public int OptionNumber { get; set; }
			public int CharacterIndex { get; set; }
			public int LineIndex { get; set; }
			public MenuOption Left { get; set; }
			public MenuOption Right { get; set; }
			public MenuOption Up { get; set; }
			public MenuOption Down { get; set; }
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
		/// <summary>The maximum width of the message box.</summary>
		private const int MaxWidth = 144;


		//-----------------------------------------------------------------------------
		// Members
		//-----------------------------------------------------------------------------

		// Message --------------------------------------------------------------------

		/// <summary>The game message with text and questions.</summary>
		private string message;
		/// <summary>The wrapped and formatted lines.</summary>
		private WrappedLetterString wrappedString;

		// Settings -------------------------------------------------------------------

		/// <summary>The number of lines to use for this window.</summary>
		private int linesPerWindow;
		/// <summary>The position of the text reader on the screen.</summary>
		private TextReaderPosition readerPosition;
		/// <summary>The spacing between the window vertical window edges and the text.
		/// </summary>
		private int spacing;
		/// <summary>Callback function invoked when the message is done.</summary>
		private TextReaderCallback callback;

		// Updating -------------------------------------------------------------------

		/// <summary>Manages text reader state and state transtions.</summary>
		private GenericStateMachine<TextReaderState> stateMachine;
		/// <summary>Used to prevent control presses from activating on the first step.
		/// </summary>
		private bool firstUpdate;
		/// <summary>The timer for the transitions.</summary>
		private int timer;
		/// <summary>The timer used to update the arrow sprite.</summary>
		private int arrowTimer;
		/// <summary>The lines left before the next set of lines is in the message box.
		/// </summary>
		private int windowLinesLeft;
		/// <summary>The current window line of the line being written.</summary>
		private int windowLineIndex;
		/// <summary>The current line in the wrapped string.</summary>
		private int currentLine;
		/// <summary>The current character of the current line.</summary>
		private int currentChar;
		/// <summary>Used to play the letter sound every other letter in a word.
		/// </summary>
		private int soundCounter;
		
		// Menu Options ---------------------------------------------------------------

		/// <summary>The currently selected menu option.</summary>
		private MenuOption selectedMenuOption;
		/// <summary>The menu option that is selected when pressing B.</summary>
		private MenuOption defaultMenuOption;
		/// <summary>The list of selectable menu option.</summary>
		private List<MenuOption> menuOptions;

		// Piece of Heart -------------------------------------------------------------

		/// <summary>True if the heart piece UI is being displayed.</summary>
		private bool heartPieceDisplay;
		/// <summary>The internal counter used to display the pieces of heart before
		/// and after.</summary>
		private int piecesOfHeart;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Constructs a text reader with the specified message.</summary>
		public RoomStateTextReader(string message) :
			this(message, TextReaderArgs.Default)
		{
		}

		/// <summary>Constructs a text reader with the specified message.</summary>
		public RoomStateTextReader(string message, TextReaderArgs args,
			Action callback, int piecesOfHeart = 0) :
			this(message, args, (int option) => { callback?.Invoke(); },
				piecesOfHeart)
		{
		}

		/// <summary>Constructs a text reader with the specified message.</summary>
		public RoomStateTextReader(string message, TextReaderArgs args,
			TextReaderCallback callback = null, int piecesOfHeart = 0)
		{
			menuOptions = new List<MenuOption>();

			// RoomState properties
			UpdateRoom	= false;
			AnimateRoom	= true;
			
			// TextReader Settings
			this.message		= message;
			this.piecesOfHeart	= piecesOfHeart;
			this.callback		= callback;
			linesPerWindow		= args.LinesPerWindow;
			readerPosition		= args.ReaderPosition;
			spacing				= args.Spacing;

			// Setup the state machine
			stateMachine = new GenericStateMachine<TextReaderState>();
			stateMachine.AddState(TextReaderState.WritingLine)
				.OnUpdate(UpdateWritingLineState);
			stateMachine.AddState(TextReaderState.PushingLine)
				.OnBegin(BeginPushingLineState)
				.OnUpdate(UpdatePushingLineState);
			stateMachine.AddState(TextReaderState.PressToContinue)
				.OnBegin(BeginPressToContinueState)
				.OnUpdate(UpdatePressToContinueState);
			stateMachine.AddState(TextReaderState.PressToEndParagraph)
				.OnBegin(BeginPressToEndParagraphState)
				.OnUpdate(UpdatePressToEndParagraphState);
			stateMachine.AddState(TextReaderState.ChoosingOption)
				.OnBegin(BeginChoosingOptionState)
				.OnUpdate(UpdateChoosingOptionState);
			stateMachine.AddState(TextReaderState.HeartPieceDelay)
				.OnBegin(BeginHeartPieceDelayState)
				.OnUpdate(UpdateHeartPieceDelayState);
			stateMachine.AddState(TextReaderState.Finished)
				.OnUpdate(UpdateFinishedState);
		}
		

		//-----------------------------------------------------------------------------
		// State Methods
		//-----------------------------------------------------------------------------

		private void UpdateWritingLineState() {
			char character = wrappedString.Lines[currentLine][currentChar].Char;
			currentChar++;

			// Update the text sound, which plays every other non-whitespace character
			if (AudioSystem.IsSoundPlaying(GameData.SOUND_TEXT_CONTINUE))
				soundCounter = 0;
			else if (character != ' ') {
				if (soundCounter % 2 == 0)
					AudioSystem.PlaySound(GameData.SOUND_TEXT_LETTER);
				soundCounter++;
			}

			// Pressing A or B will skip to the end of th eline
			if (Controls.A.IsPressed() || Controls.B.IsPressed())
				currentChar = wrappedString.Lines[currentLine].Length;

			// Check if we reached the end of the line
			if (currentChar >= wrappedString.Lines[currentLine].Length) {
				windowLinesLeft--;
				if (currentLine + 1 == wrappedString.LineCount) {
					//if (currentLine
					if (menuOptions.Count > 0)
						stateMachine.BeginState(TextReaderState.ChoosingOption);
					else
						stateMachine.BeginState(TextReaderState.Finished);
				}
				else if (wrappedString.Lines[currentLine].EndsWith(
					FormatCodes.ParagraphCharacter))
				{
					stateMachine.BeginState(TextReaderState.PressToEndParagraph);
				}
				else if (wrappedString.Lines[currentLine].EndsWith(
					FormatCodes.HeartPieceCharacter))
				{
					stateMachine.BeginState(TextReaderState.HeartPieceDelay);
				}
				else if (windowLinesLeft == 0) {
					stateMachine.BeginState(TextReaderState.PressToContinue);
				}
				else if (windowLineIndex + 1 < linesPerWindow) {
					// Begin displaying the next line in the window
					windowLineIndex++;
					currentLine++;
					currentChar = 0;
				}
				else {
					stateMachine.BeginState(TextReaderState.PushingLine);
				}
			}
			else {
				timer = 1;
			}
		}

		private void BeginHeartPieceDelayState() {
			heartPieceDisplay = true;
			timer = HeartPieceDelay;
		}

		private void UpdateHeartPieceDelayState() {
			AudioSystem.PlaySound(GameData.SOUND_TEXT_CONTINUE);
			piecesOfHeart++;
			timer = 2;
			if (currentLine + 1 == wrappedString.LineCount)
				stateMachine.BeginState(TextReaderState.Finished);
			else
				stateMachine.BeginState(TextReaderState.PressToEndParagraph);
		}

		private void BeginPushingLineState() {
			timer = 4;
			currentChar = 0;
			currentLine++;
		}

		private void UpdatePushingLineState() {
			// 1 frame between lines
			stateMachine.BeginState(TextReaderState.WritingLine);
		}

		private void BeginPressToContinueState() {
			arrowTimer = 0;
		}

		private void UpdatePressToContinueState() {
			arrowTimer++;
			if (arrowTimer == 32)
				arrowTimer = 0;
			if (Controls.A.IsPressed() || Controls.B.IsPressed()) {
				stateMachine.BeginState(TextReaderState.PushingLine);
				windowLinesLeft = linesPerWindow;
				heartPieceDisplay = false;
				AudioSystem.PlaySound(GameData.SOUND_TEXT_CONTINUE);
			}
		}

		private void BeginPressToEndParagraphState() {
			arrowTimer = 0;
		}

		private void UpdatePressToEndParagraphState() {
			arrowTimer++;
			if (arrowTimer == 32)
				arrowTimer = 0;
			if (Controls.A.IsPressed() || Controls.B.IsPressed()) {
				stateMachine.BeginState(TextReaderState.WritingLine);
				windowLinesLeft = linesPerWindow;
				currentChar = 0;
				windowLineIndex = 0;
				currentLine++;
				heartPieceDisplay = false;
			}
		}

		private void BeginChoosingOptionState() {
			SelectOption(menuOptions[0]);
			timer = 0;
		}

		private void UpdateChoosingOptionState() {
			if (Controls.A.IsPressed()) {
				AudioSystem.PlaySound(GameData.SOUND_MENU_SELECT);
				End();
			}
			else if (Controls.B.IsPressed())
				SelectOption(defaultMenuOption);
			else if (Controls.Right.IsPressed())
				SelectOption(selectedMenuOption.Right);
			else if (Controls.Left.IsPressed())
				SelectOption(selectedMenuOption.Left);
			else if (Controls.Up.IsPressed())
				SelectOption(selectedMenuOption.Up);
			else if (Controls.Down.IsPressed())
				SelectOption(selectedMenuOption.Down);
		}

		private void UpdateFinishedState() {
			// TODO: Switch to any key
			if (Controls.A.IsPressed() || Controls.B.IsPressed() ||
				Controls.Start.IsPressed() || Controls.Select.IsPressed())
			{
				heartPieceDisplay = false;
				End();
			}
		}
		

		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnBegin() {
			wrappedString = GameData.FONT_LARGE.WrapString(
				message, MaxWidth - spacing * 16, GameControl.Variables);
			timer				= 1;
			arrowTimer			= 0;
			windowLinesLeft		= linesPerWindow;
			windowLineIndex		= 0;
			currentLine			= 0;
			currentChar			= 0;
			soundCounter		= 0;
			firstUpdate			= true;
			heartPieceDisplay	= false;

			menuOptions.Clear();
			selectedMenuOption	= null;
			defaultMenuOption	= null;

			if (readerPosition == TextReaderPosition.Automatic) {
				if (GameControl.Player.DrawPosition.Y <
					RoomControl.Camera.RoomBounds.Center.Y + 8)
					readerPosition = TextReaderPosition.Bottom;
				else
					readerPosition = TextReaderPosition.Top;
			}

			stateMachine.InitializeOnState(TextReaderState.WritingLine);

			// Create a list of menu options
			for (int lineIndex = 0;
				lineIndex < wrappedString.Lines.Length; lineIndex++)
			{
				LetterString line = wrappedString.Lines[lineIndex];
				for (int charIndex = 0; charIndex < line.String.Length; charIndex++) {
					Letter letter = line[charIndex];
					if (FormatCodes.IsMessageOption(letter.Char)) {
						MenuOption option = new MenuOption() {
							LineIndex = lineIndex,
							CharacterIndex = charIndex,
							OptionNumber = FormatCodes.GetMessageOptionIndex(letter.Char),
						};
						menuOptions.Add(option);
						if (defaultMenuOption == null || option.OptionNumber == 0)
							defaultMenuOption = option;
						line[charIndex] = new Letter(' ', letter.Color);
					}
				}
			}

			// Link up menu options
			for (int i = 0; i < menuOptions.Count; i++) {
				MenuOption option = menuOptions[i];
				option.Right = menuOptions[(i + 1) % menuOptions.Count];
				option.Left = menuOptions[
					(i + menuOptions.Count - 1) % menuOptions.Count];

				// Find the closest menu option on the opposite line
				option.Up = option;
				option.Down = option;
				for (int j = 0; j < menuOptions.Count; j++) {
					MenuOption other = menuOptions[j];
					if (other != option && other.LineIndex != option.LineIndex) {
						if (option.Down == option ||
							GMath.Abs(other.CharacterIndex - option.CharacterIndex) <
							GMath.Abs(option.Down.CharacterIndex - option.CharacterIndex))
						{
							option.Down = other;
							option.Up = other;
						}
					}
				}
			}

			// Prevent any crashes whith empty wrapped strings
			if (wrappedString.LineCount == 0)
				stateMachine.BeginState(TextReaderState.Finished);
		}

		public override void OnEnd() {
			callback?.Invoke(selectedMenuOption != null ? selectedMenuOption.OptionNumber : -1);
		}

		public override void Update() {
			if (timer > 0 &&
				(stateMachine.CurrentState != TextReaderState.WritingLine ||
				firstUpdate || (!heartPieceDisplay && !Controls.A.IsPressed() &&
				!Controls.B.IsPressed())))
			{
				timer -= 1;
			}
			else
				stateMachine.Update();

			firstUpdate = false;
		}

		public override void Draw(Graphics2D g) {
			g.PushTranslation(0, GameSettings.HUD_HEIGHT);
			Point2I pos = new Point2I(8, 0);

			// Set the message box Y-posiiton
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

			// Fill the message box background
			g.FillRectangle(new Rectangle2I(pos,
				new Point2I(MaxWidth, 8 + 16 * linesPerWindow)), EntityColors.Black);

			// Prevent any crashes whith empty wrapped strings
			if (wrappedString.LineCount == 0) {
				g.PopTranslation();
				return;
			}

			// Draw the finished writing lines
			for (int i = 0; i < windowLineIndex; i++) {
				int lineIndex = currentLine - windowLineIndex + i;
				LetterString line = wrappedString.Lines[lineIndex];
				g.DrawLetterString(GameData.FONT_LARGE, line,
					pos + GetLineOffset(lineIndex), TextColor);
			}

			// Draw the currently writing line
			g.DrawLetterString(GameData.FONT_LARGE,
				wrappedString.Lines[currentLine].Substring(0, currentChar),
				pos + GetLineOffset(currentLine), TextColor);
			
			// Draw the next line arrow
			if ((stateMachine.CurrentState == TextReaderState.PressToContinue ||
				stateMachine.CurrentState == TextReaderState.PressToEndParagraph) &&
				arrowTimer >= 16 && timer == 0)
			{
				g.DrawSprite(GameData.SPR_HUD_TEXT_NEXT_ARROW,
					pos + new Point2I(136, 16 * linesPerWindow));
			}

			// Draw the heart piece
			if (heartPieceDisplay) {
				Point2I heartPiecePos = pos + new Point2I(MaxWidth - 24, 8);
				for (int i = 0; i < 4; i++) {
					ISprite sprite = GameData.SPR_HUD_MESSAGE_HEART_PIECES_EMPTY[i];
					if (piecesOfHeart > i)
						sprite = GameData.SPR_HUD_MESSAGE_HEART_PIECES_FULL[i];
					g.DrawSprite(sprite, heartPiecePos);
				}
			}

			g.PopTranslation();
		}


		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------
		
		/// <summary>Select the given menu option. This will move the cursor character
		/// in the message text.</summary>
		private void SelectOption(MenuOption option) {
			if (selectedMenuOption != null) {
				AudioSystem.PlaySound(GameData.SOUND_MENU_CURSOR_MOVE);
				SetCharacter(selectedMenuOption.LineIndex,
					selectedMenuOption.CharacterIndex, ' ');
			}
			selectedMenuOption = option;
			SetCharacter(selectedMenuOption.LineIndex,
				selectedMenuOption.CharacterIndex,
				FormatCodes.GetMessageOptionCharacter(option.OptionNumber));
		}

		/// <summary>Set a character in the message by line and column.</summary>
		private void SetCharacter(int line, int column, char c) {
			Letter letter = wrappedString.Lines[line][column];
			wrappedString.Lines[line][column] = new Letter(c, letter.Color);
		}

		/// <summary>Calculate the draw offset of a line based on its alignment.
		/// </summary>
		private Point2I GetLineOffset(int lineIndex) {
			int width = (MaxWidth - spacing * 16) / 8;
			int length = wrappedString.LineLengths[lineIndex] / 8;
			int winLine = lineIndex - currentLine + windowLineIndex;
			Point2I offset = new Point2I(spacing * 8, 6 + 16 * winLine);
			if (stateMachine.CurrentState == TextReaderState.PushingLine && timer >= 2)
				offset.Y += 8;
			switch (wrappedString.Lines[lineIndex].MessageAlignment) {
			case Align.Center:	offset.X += ((width - length) / 2) * 8;	break;
			case Align.Right:	offset.X += (width - length) * 8;		break;
			}
			return offset;
		}
	}
}
