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

		private static GameHotkey up;
		private static GameHotkey down;
		private static GameHotkey left;
		private static GameHotkey right;
		private static GameHotkey analogMovement;
		private static GameHotkey[] arrows;

		private static GameHotkey a;
		private static GameHotkey b;
		private static GameHotkey x;
		private static GameHotkey y;

		private static GameHotkey start;
		private static GameHotkey select;



		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		// Initializes the controls for the game
		public static void Initialize() {
			Controls.up				= new GameHotkey(Keys.Up);
			Controls.down			= new GameHotkey(Keys.Down);
			Controls.left			= new GameHotkey(Keys.Left);
			Controls.right			= new GameHotkey(Keys.Right);
			Controls.analogMovement	= new GameHotkey(Buttons.LeftStick);
			
			Controls.arrows = new GameHotkey[4];
			Controls.arrows[Directions.Up]		= Controls.up;
			Controls.arrows[Directions.Down]	= Controls.down;
			Controls.arrows[Directions.Left]	= Controls.left;
			Controls.arrows[Directions.Right]	= Controls.right;

			Controls.a				= new GameHotkey(Keys.X);
			Controls.b				= new GameHotkey(Keys.Z);
			Controls.x				= new GameHotkey(Keys.A);
			Controls.y				= new GameHotkey(Keys.S);

			Controls.start			= new GameHotkey(Keys.Enter);
			Controls.select			= new GameHotkey(Keys.Backslash);
		}
		

		//-----------------------------------------------------------------------------
		// Accessors
		//-----------------------------------------------------------------------------

		// Gets the arrow controls
		public static InputControl GetArrowControl(int direction) {
			return arrows[direction].Button;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		// Gets the up button
		public static InputControl Up {
			get { return up.Button; }
		}
		// Gets the down button
		public static InputControl Down {
			get { return down.Button; }
		}
		// Gets the left button
		public static InputControl Left {
			get { return left.Button; }
		}
		// Gets the right button
		public static InputControl Right {
			get { return right.Button; }
		}
		// Gets the analog movement control
		public static AnalogStick AnalogMovement {
			get { return analogMovement.Stick; }
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
