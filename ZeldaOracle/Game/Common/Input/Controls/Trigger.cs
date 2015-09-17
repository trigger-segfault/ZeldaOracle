using System;
using System.Collections.Generic;
using System.Linq;

using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Common.Input.Controls {
/** <summary>
 * A class to store trigger data.
 * </summary> */
public class Trigger {

	//========== CONSTANTS ===========

	/** <summary> The dead zone for the button control. </summary> */
	private static double ButtonDeadZone		= 0.24;

	//=========== MEMBERS ============

	/** <summary> The control for the button. </summary> */
	private InputControl button;
	/** <summary> The disabled state of the control. </summary> */
	private DisableState disabledState;
	/** <summary> The pressure of the trigger. </summary> */
	private double pressure;
	/** <summary> The dead zone of the trigger. </summary> */
	private double deadZone;

	//========= CONSTRUCTORS =========

	/** <summary> Constructs the default control. </summary> */
	public Trigger() {
		this.button			= new InputControl();
		this.disabledState	= DisableState.Enabled;
		this.pressure		= 0.0;
		this.deadZone		= 0.12;
	}

	//========== PROPERTIES ==========

	/** <summary> The position of the trigger. </summary> */
	public double Pressure {
		get { return pressure; }
	}
	/** <summary> The dead zone of the trigger. </summary> */
	public double DeadZone {
		get { return deadZone; }
		set { deadZone = GMath.Max(0.0, GMath.Min(1.0, GMath.Abs(value))); }
	}
	/** <summary> The button of the trigger. </summary> */
	public InputControl Button {
		get { return button; }
	}

	//=========== UPDATING ===========

	/** <summary> Called every step to update the control state. </summary> */
	public void Update(int time, double pressure) {
		// Apply the dead zone to the position
		if (pressure <= deadZone) {
			pressure = 0.0;
		}

		// Update the control state based on its position
		if (disabledState == DisableState.Enabled) {
			this.pressure = pressure;

			// Update the button control
			button.Update(time, pressure > ButtonDeadZone);
		}
		else if (disabledState == DisableState.DisabledUntilRelease) {
			if (pressure == 0.0) {
				disabledState	= DisableState.Enabled;
				for (int i = 0; i < 4; i++)
					button.Enable();
			}
		}
	}

	//========== MANAGEMENT ==========

	/** <summary> Resets the control state. </summary> */
	public void Reset(bool release = false) {
		pressure = 0.0;
		button.Reset(release);
	}
	/** <summary> Enables the control. </summary> */
	public void Enable() {
		disabledState = DisableState.Enabled;
		button.Enable();
	}
	/** <summary> Disables the control. </summary> */
	public void Disable(bool untilRelease = false) {
		if (untilRelease) {
			if (pressure != 0.0)
				disabledState = DisableState.DisabledUntilRelease;
			else
				pressure	= 0.0;
		}
		else {
			pressure		= 0.0;
			disabledState	= DisableState.Disabled;
		}
		button.Disable(untilRelease);
	}

	//======== CONTROL STATES ========

	/** <summary> Returns true if the trigger is centered. </summary> */
	public bool IsZero() {
		return pressure == 0.0;
	}
	/** <summary> Returns true if the trigger is disabled. </summary> */
	public bool IsDisabled() {
		return (disabledState != DisableState.Enabled);
	}
}
} // End namespace
