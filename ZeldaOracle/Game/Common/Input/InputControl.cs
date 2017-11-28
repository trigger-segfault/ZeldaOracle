using System;

namespace ZeldaOracle.Common.Input {
	/// <summary>A class to store control data.</summary>
	public class InputControl {

		//-----------------------------------------------------------------------------
		// Constants
		//-----------------------------------------------------------------------------

		public const double DoubleClickTime = 0.150;


		//-----------------------------------------------------------------------------
		// Members
		//-----------------------------------------------------------------------------

		// The pressed state of the control.
		protected InputState state;
		// The disabled state of the control.
		protected DisableState disabledState;
		// True if the control was double clicked.
		protected bool isDoubleClicked;
		// True if the control was clicked.
		protected bool isClicked;
		// True if the control was typed.
		protected bool isTyped;
		// The time the key has been held for since being pressed, or released.
		protected int holdTime;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		// Constructs the default control.
		public InputControl() {
			this.state				= InputState.Up;
			this.disabledState		= DisableState.Enabled;
			this.isDoubleClicked	= false;
			this.isClicked			= false;
			this.isTyped			= false;
			this.holdTime			= 0;
		}


		//-----------------------------------------------------------------------------
		// Updating
		//-----------------------------------------------------------------------------

		// Called every step to update the control state.
		public void Update(int time, bool down, bool typed = false, bool doubleClicked = false, bool clicked = false) {
			// Update the duration of the control state
			holdTime += time;

			// Update the double pressed and typed state
			if (disabledState == DisableState.Enabled) {
				this.isDoubleClicked	= doubleClicked;
				this.isClicked			= clicked;
				this.isTyped			= typed;
			}

			// Update the control state based on its down state
			if (down) {
				if (disabledState == DisableState.Enabled) {
					if (state == InputState.Pressed)
						state = InputState.Down;
					if (state == InputState.Released || state == InputState.Up) {
						state = InputState.Pressed;
						if (holdTime < DoubleClickTime)
							this.isDoubleClicked = true;
						holdTime = 0;
					}
				}
			}
			else {
				if (disabledState == DisableState.Enabled) {
					if (state == InputState.Released)
						state = InputState.Up;
					if (state == InputState.Pressed || state == InputState.Down) {
						state = InputState.Released;
						holdTime = 0;
					}
				}
				else if (disabledState == DisableState.DisabledUntilRelease) {
					disabledState	= DisableState.Enabled;
					state			= InputState.Up;
				}
			}
		}
		

		//-----------------------------------------------------------------------------
		// Management
		//-----------------------------------------------------------------------------

		// Resets the control state.
		public void Reset(bool release = false) {
			if ((state == InputState.Pressed || state == InputState.Down) && release)
				state = InputState.Released;
			else
				state = InputState.Up;
			isDoubleClicked	= false;
			isClicked		= false;
			isTyped			= false;
			holdTime		= 0;
		}
		// Enables the control.
		public void Enable() {
			disabledState = DisableState.Enabled;
		}
		// Disables the control.
		public void Disable(bool untilRelease = false) {
			if (untilRelease) {
				if (state != InputState.Up && state != InputState.Released)
					disabledState = DisableState.DisabledUntilRelease;
				else
					state = InputState.Up;
			}
			else {
				state			= InputState.Up;
				disabledState	= DisableState.Disabled;
			}
			isDoubleClicked	= false;
			isClicked		= false;
			isTyped			= false;
			holdTime		= 0;
		}

		

		//-----------------------------------------------------------------------------
		// Control State
		//-----------------------------------------------------------------------------

		// Returns true if the control was double clicked.
		public bool IsDoubleClicked() {
			return (isDoubleClicked && disabledState == DisableState.Enabled);
		}
		// Returns true if the control was clicked.
		public bool IsClicked() {
			return ((isDoubleClicked || isClicked) && disabledState == DisableState.Enabled);
		}
		// Returns true if the control was pressed.
		public bool IsPressed() {
			return (state == InputState.Pressed && disabledState == DisableState.Enabled);
		}
		// Returns true if the control is down.
		public bool IsDown() {
			return ((state == InputState.Pressed || state == InputState.Down) &&
					disabledState == DisableState.Enabled);
		}
		// Returns true if the control was released.
		public bool IsReleased() {
			return (state == InputState.Released && disabledState == DisableState.Enabled);
		}
		// Returns true if the control is up.
		public bool IsUp() {
			return (state == InputState.Released || state == InputState.Up ||
					disabledState != DisableState.Enabled);
		}
		// Returns true if the control was typed.
		public bool IsTyped() {
			return (isTyped && disabledState == DisableState.Enabled);
		}
		// Returns true if the control is disabled.
		public bool IsDisabled() {
			return (disabledState != DisableState.Enabled);
		}
		

		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		// The time the key has been held for since being pressed, or released.
		public int HoldTime {
			get { return holdTime; }
		}
	}
}
