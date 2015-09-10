using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using ZeldaOracle.Common;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Input.Controls;

namespace ZeldaOracle.Common.Debug {
	public class ToggleMenuItem : DebugMenuItem {
		public delegate void MenuItemToggleAction(bool isEnabled);

		private bool isEnabled;
		private MenuItemToggleAction toggleAction;
		private MenuItemAction enableAction;
		private MenuItemAction disableAction;



		// ================== CONSTRUCTORS ================== //

		public ToggleMenuItem(string text, HotKey hotkey, bool startsEnabled, MenuItemToggleAction toggleAction,
			MenuItemAction enableAction, MenuItemAction disableAction) :
			base(text, hotkey, null) {
			this.isEnabled     = startsEnabled;
			this.toggleAction  = toggleAction;
			this.enableAction  = enableAction;
			this.disableAction = disableAction;

			Action = delegate() { Toggle(); };
		}



		// ==================== METHODS ==================== //

		public void Enable() {
			if (!isEnabled) {
				isEnabled = true;

				if (enableAction != null)
					enableAction();
				if (toggleAction != null)
					toggleAction(isEnabled);
			}
		}

		public void Disable() {
			if (isEnabled) {
				isEnabled = false;

				if (disableAction != null)
					disableAction();
				if (toggleAction != null)
					toggleAction(isEnabled);
			}
		}

		public void Toggle() {
			if (isEnabled)
				Disable();
			else
				Enable();
		}



		// ================== PROPERTIES =================== //

		public bool IsEnabled {
			get { return isEnabled; }
		}
	}
}
