using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Common.Input.Controls {
	public class GameHotkey {

		private Keys keyCode;
		private MouseButtons mouseCode;
		private Buttons buttonCode;
		private InputType inputType;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public GameHotkey() {
			this.keyCode = Keys.None;
			this.mouseCode = MouseButtons.None;
			this.buttonCode = Buttons.None;
			this.inputType = InputType.None;
		}
		public GameHotkey(Keys keyCode) {
			this.keyCode = keyCode;
			this.mouseCode = MouseButtons.None;
			this.buttonCode = Buttons.None;
			this.inputType = InputType.Key;
		}
		public GameHotkey(MouseButtons mouseCode) {
			this.keyCode = Keys.None;
			this.mouseCode = mouseCode;
			this.buttonCode = Buttons.None;
			this.inputType = InputType.MouseButton;
		}
		public GameHotkey(Buttons buttonCode) {
			this.keyCode = Keys.None;
			this.mouseCode = MouseButtons.None;
			this.buttonCode = buttonCode;
			if (buttonCode == Buttons.LeftTrigger || buttonCode == Buttons.RightTrigger)
				this.inputType = InputType.Trigger;
			else if (buttonCode == Buttons.LeftStick || buttonCode == Buttons.RightStick)
				this.inputType = InputType.Stick;
			else
				this.inputType = InputType.Button;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		// Gets the analog stick if it is the active control
		public AnalogStick Stick {
			get {
				if (inputType == InputType.Stick)
					return GamePad.GetStick(buttonCode, 0);
				return null;
			}
		}
		// Gets the trigger if it is the active control
		public Trigger Trigger {
			get {
				if (inputType == InputType.Trigger)
					return GamePad.GetTrigger(buttonCode, 0);
				return null;
			}
		}
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

		public static bool operator ==(GameHotkey c1, GameHotkey c2) {
			if (c1.inputType == c2.inputType) {
				switch (c1.inputType) {
				case InputType.Key:				return c1.keyCode == c2.keyCode;
				case InputType.MouseButton:		return c1.mouseCode == c2.mouseCode;
				case InputType.Button:
				case InputType.Trigger:
				case InputType.Stick:			return c1.buttonCode == c2.buttonCode;
				}
			}
			return false;
		}
		public static bool operator !=(GameHotkey c1, GameHotkey c2) {
			if (c1.inputType == c2.inputType) {
				switch (c1.inputType) {
				case InputType.Key:				return c1.keyCode != c2.keyCode;
				case InputType.MouseButton:		return c1.mouseCode != c2.mouseCode;
				case InputType.Button:
				case InputType.Trigger:
				case InputType.Stick:			return c1.buttonCode != c2.buttonCode;
				}
			}
			return true;
		}
	}
}
