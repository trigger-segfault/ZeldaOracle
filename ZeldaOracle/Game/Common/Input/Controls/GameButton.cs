using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Common.Input.Controls {
	/// <summary>An accessor for key, mouse button, and gamepad button controls.</summary>
	public class GameButton {

		/// <summary>The key this game button represents.</summary>
		private Keys keyCode;
		/// <summary>The mouse button this game button represents.</summary>
		private MouseButtons mouseCode;
		/// <summary>The gamepad button this game button represents.</summary>
		private Buttons buttonCode;
		/// <summary>The type of input control this game button represents.</summary>
		private InputType inputType;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		/// <summary>Constructs an unassigned game button.</summary>
		public GameButton() {
			this.keyCode = Keys.None;
			this.mouseCode = MouseButtons.None;
			this.buttonCode = Buttons.None;
			this.inputType = InputType.None;
		}

		/// <summary>Constructs a key game button.</summary>
		public GameButton(Keys keyCode) {
			this.keyCode = keyCode;
			this.mouseCode = MouseButtons.None;
			this.buttonCode = Buttons.None;
			this.inputType = InputType.Key;
		}

		/// <summary>Constructs a mouse game button.</summary>
		public GameButton(MouseButtons mouseCode) {
			this.keyCode = Keys.None;
			this.mouseCode = mouseCode;
			this.buttonCode = Buttons.None;
			this.inputType = InputType.MouseButton;
		}

		/// <summary>Constructs a gamepad game button.</summary>
		public GameButton(Buttons buttonCode) {
			this.keyCode = Keys.None;
			this.mouseCode = MouseButtons.None;
			this.buttonCode = buttonCode;
			this.inputType = InputType.Button;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets the button if it is the active control.</summary>
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


		//-----------------------------------------------------------------------------
		// General
		//-----------------------------------------------------------------------------

		/// <summary>Returns true if the game button is equal to the other game button.</summary>
		public override bool Equals(object obj) {
			if (obj.GetType() == GetType())
				return (this == ((GameButton) obj));
			return false;
		}

		/// <summary>Gets the hash code for the game button.</summary>
		public override int GetHashCode() {
			unchecked {
				return (int) keyCode ^ (int) mouseCode ^
					(int) buttonCode ^ (int) inputType;
			}
		}
	}
}
