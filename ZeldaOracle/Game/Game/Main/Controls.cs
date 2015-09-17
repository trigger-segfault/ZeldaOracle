using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Input;
using ZeldaOracle.Common.Input.Controls;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Game.Main {

	// The controls for the game
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

		// Initializes the controls for the game
		public static void Initialize() {

			Controls.arrows = new MultiGameButton[4];
			Controls.arrows[Directions.Up]		= new MultiGameButton(new GameButton(Keys.Up), new GameButton(Buttons.LeftStickUp));
			Controls.arrows[Directions.Down]	= new MultiGameButton(new GameButton(Keys.Down), new GameButton(Buttons.LeftStickDown));
			Controls.arrows[Directions.Left]	= new MultiGameButton(new GameButton(Keys.Left), new GameButton(Buttons.LeftStickLeft));
			Controls.arrows[Directions.Right]	= new MultiGameButton(new GameButton(Keys.Right), new GameButton(Buttons.LeftStickRight));

			Controls.analogMovement	= GamePad.GetStick(Buttons.LeftStick);

			Controls.a				= new MultiGameButton(new GameButton(Keys.X), new GameButton(Buttons.A));
			Controls.b				= new MultiGameButton(new GameButton(Keys.Z), new GameButton(Buttons.B));
			Controls.x				= new MultiGameButton(new GameButton(Keys.S), new GameButton(Buttons.X));
			Controls.y				= new MultiGameButton(new GameButton(Keys.A), new GameButton(Buttons.Y));

			Controls.start			= new MultiGameButton(new GameButton(Keys.Enter), new GameButton(Buttons.Start));
			Controls.select			= new MultiGameButton(new GameButton(Keys.Backslash), new GameButton(Buttons.Back));
		}

		//-----------------------------------------------------------------------------
		// Updating
		//-----------------------------------------------------------------------------

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

		// Gets the arrow controls
		public static InputControl GetArrowControl(int direction) {
			return arrows[direction].Button;
		}

		// Gets the analog direction controls
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
		// Properties
		//-----------------------------------------------------------------------------

		// Gets the up button
		public static InputControl Up {
			get { return arrows[Directions.Up].Button; }
		}
		// Gets the down button
		public static InputControl Down {
			get { return arrows[Directions.Down].Button; }
		}
		// Gets the left button
		public static InputControl Left {
			get { return arrows[Directions.Left].Button; }
		}
		// Gets the right button
		public static InputControl Right {
			get { return arrows[Directions.Right].Button; }
		}
		// Gets the analog movement control
		public static AnalogStick AnalogMovement {
			get { return analogMovement; }
		}

		// Gets the A button
		public static InputControl A {
			get { return a.Button; }
		}
		// Gets the B button
		public static InputControl B {
			get { return b.Button; }
		}
		// Gets the X button
		public static InputControl X {
			get { return x.Button; }
		}
		// Gets the Y button
		public static InputControl Y {
			get { return y.Button; }
		}

		// Gets the start button
		public static InputControl Start {
			get { return start.Button; }
		}
		// Gets the select button
		public static InputControl Select {
			get { return select.Button; }
		}
		
	}
}
