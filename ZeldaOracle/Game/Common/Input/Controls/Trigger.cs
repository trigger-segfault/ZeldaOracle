using System;
using System.Collections.Generic;
using System.Linq;

using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Common.Input.Controls {
	/// <summary>A class to store trigger data.</summary>
	public class Trigger {
		
		/// <summary>The dead zone for the button control.</summary>
		private static float ButtonDeadZone		= 0.24f;
		
		/// <summary>The control for the button.</summary>
		private InputControl button;
		/// <summary>The disabled state of the control.</summary>
		private DisableState disabledState;
		/// <summary>The pressure of the trigger.</summary>
		private float pressure;
		/// <summary>The dead zone of the trigger.</summary>
		private float deadZone;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Constructs the default control.</summary>
		public Trigger() {
			this.button			= new InputControl();
			this.disabledState	= DisableState.Enabled;
			this.pressure		= 0.0f;
			this.deadZone		= 0.12f;
		}


		//-----------------------------------------------------------------------------
		// Updating
		//-----------------------------------------------------------------------------

		/// <summary>Called every step to update the control state.</summary>
		public void Update(int time, float pressure) {
			// Apply the dead zone to the position
			if (pressure <= deadZone) {
				pressure = 0f;
			}

			// Update the control state based on its position
			if (disabledState == DisableState.Enabled) {
				this.pressure = pressure;

				// Update the button control
				button.Update(time, pressure > ButtonDeadZone);
			}
			else if (disabledState == DisableState.DisabledUntilRelease) {
				if (pressure == 0f) {
					disabledState	= DisableState.Enabled;
					for (int i = 0; i < 4; i++)
						button.Enable();
				}
			}
		}


		//-----------------------------------------------------------------------------
		// Management
		//-----------------------------------------------------------------------------

		/// <summary>Resets the control state.</summary>
		public void Reset(bool release = false) {
			pressure = 0f;
			button.Reset(release);
		}
		/// <summary>Enables the control.</summary>
		public void Enable() {
			disabledState = DisableState.Enabled;
			button.Enable();
		}
		/// <summary>Disables the control.</summary>
		public void Disable(bool untilRelease = false) {
			if (untilRelease) {
				if (pressure != 0f)
					disabledState = DisableState.DisabledUntilRelease;
				else
					pressure = 0f;
			}
			else {
				pressure = 0f;
				disabledState = DisableState.Disabled;
			}
			button.Disable(untilRelease);
		}


		//-----------------------------------------------------------------------------
		// Control States
		//-----------------------------------------------------------------------------

		/// <summary>Returns true if the trigger is centered.</summary>
		public bool IsZero() {
			return (pressure <= deadZone);
		}

		/// <summary>Returns true if the trigger is disabled.</summary>
		public bool IsDisabled() {
			return (disabledState != DisableState.Enabled);
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>The position of the trigger.</summary>
		public float Pressure {
			get { return pressure; }
		}
		/// <summary>The dead zone of the trigger.</summary>
		public float DeadZone {
			get { return deadZone; }
			set { deadZone = GMath.Clamp(GMath.Abs(value), 0f, 1f); }
		}
		/// <summary>The button of the trigger.</summary>
		public InputControl Button {
			get { return button; }
		}


	}
}
