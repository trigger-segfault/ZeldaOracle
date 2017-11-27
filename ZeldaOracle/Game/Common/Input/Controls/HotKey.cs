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
	public class HotKey : ControlHandler {

		//=========== MEMBERS ============
		#region Members

		/**<summary>The main key to be pressed.</summary>*/
		public Keys KeyCode;
		/**<summary>True if the ctrl key has to be down.</summary>*/
		public bool Ctrl;
		/**<summary>True if the shift key has to be down.</summary>*/
		public bool Shift;
		/**<summary>True if the Key key has to be down.</summary>*/
		public bool Alt;

		#endregion
		//========= CONSTRUCTORS =========
		#region Constructors

		/**<summary>Constructs the default hotkey.</summary>*/
		public HotKey() {
			this.KeyCode		= Keys.None;
			this.Ctrl			= false;
			this.Shift			= false;
			this.Alt			= false;
		}
		/**<summary>Constructs a hotkey with the specified modifiers.</summary>*/
		public HotKey(Keys keyCode, bool ctrl = false, bool shift = false, bool alt = false) {
			this.KeyCode		= keyCode;
			this.Ctrl			= ctrl;
			this.Shift			= shift;
			this.Alt			= alt;
		}

		#endregion
		//========== PROPERTIES ==========
		#region Properties

		/**<summary>Gets the name of the control.</summary>*/
		public override string Name {
			get {
				if (KeyCode == Keys.None)
					return "";
				string name = "";
				if (Ctrl)
					name += "Ctrl+";
				if (Shift)
					name += "Shift+";
				if (Alt)
					name += "Shift+";
				return name + Keyboard.GetKeyName(KeyCode);
			}
		}

		#endregion
		//============ EVENTS ============
		#region Events

		/**<summary>Returns true if the control was pressed.</summary>*/
		public override bool Pressed() {
			return (Ctrl == (Keyboard.IsKeyDown(Keys.LCtrl) || Keyboard.IsKeyDown(Keys.RCtrl))) &&
				(Shift == (Keyboard.IsKeyDown(Keys.LShift) || Keyboard.IsKeyDown(Keys.RShift))) &&
				(Alt == (Keyboard.IsKeyDown(Keys.LAlt) || Keyboard.IsKeyDown(Keys.RAlt))) &&
				Keyboard.IsKeyPressed(KeyCode);
		}
		/**<summary>Returns true if the control was released.</summary>*/
		public override bool Released() {
			return (Ctrl == (Keyboard.IsKeyDown(Keys.LCtrl) || Keyboard.IsKeyDown(Keys.RCtrl))) &&
				(Shift == (Keyboard.IsKeyDown(Keys.LShift) || Keyboard.IsKeyDown(Keys.RShift))) &&
				(Alt == (Keyboard.IsKeyDown(Keys.LAlt) || Keyboard.IsKeyDown(Keys.RAlt))) &&
				Keyboard.IsKeyReleased(KeyCode);
		}
		/**<summary>Returns true if the control is down.</summary>*/
		public override bool Down() {
			return (Ctrl == (Keyboard.IsKeyDown(Keys.LCtrl) || Keyboard.IsKeyDown(Keys.RCtrl))) &&
				(Shift == (Keyboard.IsKeyDown(Keys.LShift) || Keyboard.IsKeyDown(Keys.RShift))) &&
				(Alt == (Keyboard.IsKeyDown(Keys.LAlt) || Keyboard.IsKeyDown(Keys.RAlt))) &&
				Keyboard.IsKeyDown(KeyCode);
		}
		/**<summary>Returns true if the control is up.</summary>*/
		public override bool Up() {
			return (Ctrl != (Keyboard.IsKeyDown(Keys.LCtrl) || Keyboard.IsKeyDown(Keys.RCtrl))) ||
				(Shift != (Keyboard.IsKeyDown(Keys.LShift) || Keyboard.IsKeyDown(Keys.RShift))) ||
				(Alt != (Keyboard.IsKeyDown(Keys.LAlt) || Keyboard.IsKeyDown(Keys.RAlt))) ||
				Keyboard.IsKeyUp(KeyCode);
		}

		#endregion
	}
} // end namespace
