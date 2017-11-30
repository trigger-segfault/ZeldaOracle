using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeldaOracle.Common.Input.Controls {
	public class MultiGameButton {

		private GameButton[] hotKeys;
		private InputControl inputControl;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public MultiGameButton(params GameButton[] hotKeys) {
			this.hotKeys		= hotKeys;
			this.inputControl	= new InputControl();
		}


		//-----------------------------------------------------------------------------
		// Updating
		//-----------------------------------------------------------------------------

		public void Update() {
			for (int i = 0; i < hotKeys.Length; i++) {
				if (hotKeys[i].Button.IsReleased()) {
					inputControl.Update(1, false);
					return;
				}
			}
			for (int i = 0; i < hotKeys.Length; i++) {
				if (hotKeys[i].Button.IsDown()) {
					inputControl.Update(1, true);
					return;
				}
			}
			inputControl.Update(1, false);
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets the button if it is the active control</summary>
		public InputControl Button {
			get { return inputControl; }
		}
	}
}
