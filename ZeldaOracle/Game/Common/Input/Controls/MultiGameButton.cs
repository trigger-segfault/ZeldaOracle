using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeldaOracle.Common.Input.Controls {
	/// <summary>A wrapper for a control that has more than one mapping associated
	/// with it.</summary>
	public class MultiGameButton {

		/// <summary>The list of game buttons used by this multi-game button.</summary>
		private GameButton[] gameButtons;
		/// <summary>The input control handling the final state of the multi-game button.</summary>
		private InputControl inputControl;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		/// <summary>Constructs the multi-game button from a list of game buttons.</summary>
		public MultiGameButton(params GameButton[] gameButtons) {
			this.gameButtons	= gameButtons;
			this.inputControl	= new InputControl();
		}


		//-----------------------------------------------------------------------------
		// Updating
		//-----------------------------------------------------------------------------

		/// <summary>Updates the multi-game button.</summary>
		public void Update() {
			// TODO: Is the IsReleased() part necissary?
			// Wouldn't it be better to keep the state until all buttons are released?
			for (int i = 0; i < gameButtons.Length; i++) {
				if (gameButtons[i].Button.IsReleased()) {
					inputControl.Update(1, false);
					return;
				}
			}
			for (int i = 0; i < gameButtons.Length; i++) {
				if (gameButtons[i].Button.IsDown()) {
					inputControl.Update(1, true);
					return;
				}
			}
			inputControl.Update(1, false);
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets the final input for this control.</summary>
		public InputControl Button {
			get { return inputControl; }
		}
	}
}
