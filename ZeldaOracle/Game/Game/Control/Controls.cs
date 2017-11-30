using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Input;
using ZeldaOracle.Common.Input.Controls;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Game.Main {
	/// <summary>The controls for the game.</summary>
	public class Controls {

		private static MultiGameButton[] arrows;
		private static AnalogStick analogMovement;

		private static MultiGameButton a;
		private static MultiGameButton b;
		private static MultiGameButton x;
		private static MultiGameButton y;

		private static MultiGameButton start;
		private static MultiGameButton select;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Loads the controls from settings.</summary>
		public static void LoadControls(UserSettings userSettings) {
			Controls.arrows = new MultiGameButton[4];
			Controls.arrows[Directions.Up]      = CreateMultiGameButton(userSettings,
				userSettings.Keyboard.Up, userSettings.GamePad.Up);
			Controls.arrows[Directions.Down]    = CreateMultiGameButton(userSettings,
				userSettings.Keyboard.Down, userSettings.GamePad.Down);
			Controls.arrows[Directions.Left]    = CreateMultiGameButton(userSettings,
				userSettings.Keyboard.Left, userSettings.GamePad.Left);
			Controls.arrows[Directions.Right]   = CreateMultiGameButton(userSettings,
				userSettings.Keyboard.Right, userSettings.GamePad.Right);

			if (userSettings.GamePad.Enabled)
				Controls.analogMovement = GamePad.GetStick(userSettings.GamePad.AnalogStick);
			else
				Controls.analogMovement = GamePad.GetStick(AnalogSticks.None);

			Controls.a              = CreateMultiGameButton(userSettings,
				userSettings.Keyboard.A, userSettings.GamePad.A);
			Controls.b              = CreateMultiGameButton(userSettings,
				userSettings.Keyboard.B, userSettings.GamePad.B);
			Controls.x              = CreateMultiGameButton(userSettings,
				userSettings.Keyboard.X, userSettings.GamePad.X);
			Controls.y              = CreateMultiGameButton(userSettings,
				userSettings.Keyboard.Y, userSettings.GamePad.Y);

			Controls.start          = CreateMultiGameButton(userSettings,
				userSettings.Keyboard.Start, userSettings.GamePad.Start);
			Controls.select         = CreateMultiGameButton(userSettings,
				userSettings.Keyboard.Select, userSettings.GamePad.Select);
		}


		//-----------------------------------------------------------------------------
		// Updating
		//-----------------------------------------------------------------------------

		/// <summary>Updates the controls.</summary>
		public static void Update() {
			for (int i = 0; i < 4; i++) {
				arrows[i].Update();
			}

			a.Update();
			b.Update();
			x.Update();
			y.Update();

			start.Update();
			select.Update();
		}


		//-----------------------------------------------------------------------------
		// Accessors
		//-----------------------------------------------------------------------------

		/// <summary>Gets the arrow controls.</summary>
		public static InputControl GetArrowControl(int direction) {
			return arrows[direction].Button;
		}

		/// <summary>Gets the analog direction controls.</summary>
		public static bool GetAnalogDirection(int direction) {
			switch (direction) {
			case Directions.Right:	return analogMovement.Position.X > 0f;
			case Directions.Down:	return analogMovement.Position.Y > 0f;
			case Directions.Left:	return analogMovement.Position.X < 0f;
			case Directions.Up:		return analogMovement.Position.Y < 0f;
			}
			return false;
		}


		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------

		/// <summary>Creates a multi game button from the user settings and specified inputs.</summary>
		public static MultiGameButton CreateMultiGameButton(UserSettings userSettings,
			Keys key, Buttons button)
		{
			GameButton[] gameButtons;
			if (userSettings.Keyboard.Enabled && userSettings.GamePad.Enabled) {
				gameButtons = new GameButton[] { new GameButton(key), new GameButton(button) };
			}
			else if (userSettings.GamePad.Enabled) {
				gameButtons = new GameButton[] { new GameButton(button) };
			}
			else { // Also enable keyboard if both inputs are disabled
				gameButtons = new GameButton[] { new GameButton(key) };
			}
			return new MultiGameButton(gameButtons);
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		// Movement -------------------------------------------------------------------

		/// <summary>Gets the arrow buttons.</summary>
		public static InputControl[] Arrows {
			get {
				InputControl[] arrows = new InputControl[4];
				for (int i = 0; i < 4; i++) {
					arrows[i] = Controls.arrows[i].Button;
				}
				return arrows;
			}
		}

		/// <summary>Gets the up button.</summary>
		public static InputControl Up {
			get { return arrows[Directions.Up].Button; }
		}
		/// <summary>Gets the down button.</summary>
		public static InputControl Down {
			get { return arrows[Directions.Down].Button; }
		}

		/// <summary>Gets the left button.</summary>
		public static InputControl Left {
			get { return arrows[Directions.Left].Button; }
		}

		/// <summary>Gets the right button.</summary>
		public static InputControl Right {
			get { return arrows[Directions.Right].Button; }
		}

		/// <summary>Gets the analog movement control.</summary>
		public static AnalogStick AnalogMovement {
			get { return analogMovement; }
		}

		// Buttons --------------------------------------------------------------------

		/// <summary>Gets the A button.</summary>
		public static InputControl A {
			get { return a.Button; }
		}

		/// <summary>Gets the B button.</summary>
		public static InputControl B {
			get { return b.Button; }
		}

		/// <summary>Gets the X button.</summary>
		public static InputControl X {
			get { return x.Button; }
		}

		/// <summary>Gets the Y button.</summary>
		public static InputControl Y {
			get { return y.Button; }
		}
		
		// Menus ----------------------------------------------------------------------

		/// <summary>Gets the start button.</summary>
		public static InputControl Start {
			get { return start.Button; }
		}

		/// <summary>Gets the select button.</summary>
		public static InputControl Select {
			get { return select.Button; }
		}
	}
}
