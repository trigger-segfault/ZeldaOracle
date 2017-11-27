using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using XnaKeyboard	= Microsoft.Xna.Framework.Input.Keyboard;
using XnaKeys		= Microsoft.Xna.Framework.Input.Keys;

using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Input;
using Keyboard		= ZeldaOracle.Common.Input.Keyboard;
using Keys			= ZeldaOracle.Common.Input.Keys;

namespace ZeldaOracle.Common.Input.Controls {
	public class Key : ControlHandler {

		public Keys KeyCode; 

		public Key(Keys keyCode) {
			this.KeyCode		= keyCode;
		}

		public override bool Pressed() {
			return Keyboard.IsKeyPressed(KeyCode);
		}
		public override bool Released() {
			return Keyboard.IsKeyReleased(KeyCode);
		}
		public override bool Down() {
			return Keyboard.IsKeyDown(KeyCode);
		}
		public override bool Up() {
			return Keyboard.IsKeyUp(KeyCode);
		}

		public override string Name {
			get { return Keyboard.GetKeyName(KeyCode); }
		}
	}
}
