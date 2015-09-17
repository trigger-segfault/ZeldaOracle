using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Common.Input.Controls {
	public class GameButton {

		private Keys keyCode;
		private MouseButtons mouseCode;
		private Buttons buttonCode;
		private InputType inputType;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public GameButton() {
			this.keyCode = Keys.None;
			this.mouseCode = MouseButtons.None;
			this.buttonCode = Buttons.None;
			this.inputType = InputType.None;
		}
		public GameButton(Keys keyCode) {
			this.keyCode = keyCode;
			this.mouseCode = MouseButtons.None;
			this.buttonCode = Buttons.None;
			this.inputType = InputType.Key;
		}
		public GameButton(MouseButtons mouseCode) {
			this.keyCode = Keys.None;
			this.mouseCode = mouseCode;
			this.buttonCode = Buttons.None;
			this.inputType = InputType.MouseButton;
		}
		public GameButton(Buttons buttonCode) {
			this.keyCode = Keys.None;
			this.mouseCode = MouseButtons.None;
			this.buttonCode = buttonCode;
			this.inputType = InputType.Button;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		// Gets the button if it is the active control
		public InputControl Button {
			get {
				switch (inputType) {
				case InputType.Key: return Keyboard.GetKey(keyCode);
				case InputType.MouseButton: return Mouse.GetButton(mouseCode);
				case InputType.Button: return GamePad.GetButton(buttonCode, 0);
				}
				return null;
			}
		}


		//-----------------------------------------------------------------------------
		// Operators
		//-----------------------------------------------------------------------------

		public static bool operator ==(GameButton c1, GameButton c2) {
			if (c1.inputType == c2.inputType) {
				switch (c1.inputType) {
				case InputType.Key:				return c1.keyCode == c2.keyCode;
				case InputType.MouseButton:		return c1.mouseCode == c2.mouseCode;
				case InputType.Button:			return c1.buttonCode == c2.buttonCode;	
				}
			}
			return false;
		}
		public static bool operator !=(GameButton c1, GameButton c2) {
			if (c1.inputType == c2.inputType) {
				switch (c1.inputType) {
				case InputType.Key:				return c1.keyCode != c2.keyCode;
				case InputType.MouseButton:		return c1.mouseCode != c2.mouseCode;
				case InputType.Button:			return c1.buttonCode != c2.buttonCode;		
				}
			}
			return true;
		}
	}
}
