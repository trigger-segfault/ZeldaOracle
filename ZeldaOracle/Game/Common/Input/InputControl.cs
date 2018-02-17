using System;

namespace ZeldaOracle.Common.Input {
	/// <summary>A class to store control data.</summary>
	public class InputControl {

		//-----------------------------------------------------------------------------
		// Constants
		//-----------------------------------------------------------------------------

		/// <summary>The time before a double click opportunity expires.</summary>
		public const float DoubleClickTime = 0.150f * 60;


		//-----------------------------------------------------------------------------
		// Members
		//-----------------------------------------------------------------------------

		/// <summary>The pressed state of the control.</summary>
		protected InputState state;
		/// <summary>The disabled state of the control.</summary>
		protected DisableState disabledState;
		/// <summary>True if the control was double clicked.</summary>
		protected bool isDoubleClicked;
		/// <summary>True if the control was clicked.</summary>
		protected bool isClicked;
		/// <summary>True if the control was typed.</summary>
		protected bool isTyped;
		/// <summary>The time the key has been held for since being pressed, or released.</summary>
		protected int holdTime;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		/// <summary>Constructs the default control.</summary>
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

		/// <summary>Called every step to update the control state.</summary>
		public void Update(int time, bool down, bool typed = false, bool doubleClicked = false,
			bool clicked = false)
		{
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

		/// <summary>Resets the control state.</summary>
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

		/// <summary>Enables the control.</summary>
		public void Enable() {
			disabledState = DisableState.Enabled;
		}

		/// <summary>Disables the control.</summary>
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

		/// <summary>Returns true if the control was double clicked.</summary>
		public bool IsDoubleClicked() {
			return (isDoubleClicked && disabledState == DisableState.Enabled);
		}

		/// <summary>Returns true if the control was clicked.</summary>
		public bool IsClicked() {
			return ((isDoubleClicked || isClicked) && disabledState == DisableState.Enabled);
		}

		/// <summary>Returns true if the control was pressed.</summary>
		public bool IsPressed() {
			return (state == InputState.Pressed && disabledState == DisableState.Enabled);
		}

		/// <summary>Returns true if the control is down.</summary>
		public bool IsDown() {
			return ((state == InputState.Pressed || state == InputState.Down) &&
					disabledState == DisableState.Enabled);
		}

		/// <summary>Returns true if the control was released.</summary>
		public bool IsReleased() {
			return (state == InputState.Released && disabledState == DisableState.Enabled);
		}

		/// <summary>Returns true if the control is up.</summary>
		public bool IsUp() {
			return (state == InputState.Released || state == InputState.Up ||
					disabledState != DisableState.Enabled);
		}

		/// <summary>Returns true if the control was typed.</summary>
		public bool IsTyped() {
			return (isTyped && disabledState == DisableState.Enabled);
		}

		/// <summary>Returns true if the control is disabled.</summary>
		public bool IsDisabled() {
			return (disabledState != DisableState.Enabled);
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>The time the key has been held for since being pressed, or released.</summary>
		public int HoldTime {
			get { return holdTime; }
		}
	}
}
