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

		private static GameControl up;
		private static GameControl down;
		private static GameControl left;
		private static GameControl right;
		private static GameControl analogMovement;
		private static GameControl[] arrows;

		private static GameControl a;
		private static GameControl b;
		private static GameControl x;
		private static GameControl y;

		private static GameControl start;
		private static GameControl select;



		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		// Initializes the controls for the game
		public static void Initialize() {
			Controls.up				= new GameControl(Keys.Up);
			Controls.down			= new GameControl(Keys.Down);
			Controls.left			= new GameControl(Keys.Left);
			Controls.right			= new GameControl(Keys.Right);
			Controls.analogMovement	= new GameControl(Buttons.LeftStick);
			
			Controls.arrows = new GameControl[4];
			Controls.arrows[Directions.Up]		= Controls.up;
			Controls.arrows[Directions.Down]	= Controls.down;
			Controls.arrows[Directions.Left]	= Controls.left;
			Controls.arrows[Directions.Right]	= Controls.right;

			Controls.a				= new GameControl(Keys.X);
			Controls.b				= new GameControl(Keys.Z);
			Controls.x				= new GameControl(Keys.A);
			Controls.y				= new GameControl(Keys.S);

			Controls.start			= new GameControl(Keys.Enter);
			Controls.select			= new GameControl(Keys.Backslash);
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
