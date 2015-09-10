using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;

using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Common.Input {
/** <summary>
 * A class to store analog stick data.
 * </summary> */
public class AnalogStick {

	//========== CONSTANTS ===========

	/** <summary> The dead zone for the directional controls. </summary> */
	private static Vector2F DirectionDeadZone		= new Vector2F(0.83, 0.83);

	//=========== MEMBERS ============

	/** <summary> The 4 directional controls. </summary> */
	private InputControl[] directions;
	/** <summary> The disabled state of the control. </summary> */
	private DisableState disabledState;
	/** <summary> The position of the analog stick. </summary> */
	private Vector2F position;
	/** <summary> The dead zone of the analog stick. </summary> */
	private double deadZone;

	//========= CONSTRUCTORS =========

	/** <summary> Constructs the default control. </summary> */
	public AnalogStick() {
		this.directions		= new InputControl[4];
		this.disabledState	= DisableState.Enabled;
		this.position		= Vector2F.Zero;
		this.deadZone		= 0.28;

		for (int i = 0; i < 4; i++)
			this.directions[i] = new InputControl();
	}

	//========== PROPERTIES ==========

	/** <summary> The position of the analog stick. </summary> */
	public Vector2F Position {
		get { return position; }
	}
	/** <summary> The dead zone of the analog stick. </summary> */
	public double DeadZone {
		get { return deadZone; }
		set { deadZone = GMath.Max(0.0, GMath.Min(1.0, GMath.Abs(value))); }
	}
	/** <summary> The right control of the analog stick. </summary> */
	public InputControl Right {
		get { return directions[0]; }
	}
	/** <summary> The down control of the analog stick. </summary> */
	public InputControl Down {
		get { return directions[1]; }
	}
	/** <summary> The Left control of the analog stick. </summary> */
	public InputControl Left {
		get { return directions[2]; }
	}
	/** <summary> The up control of the analog stick. </summary> */
	public InputControl Up {
		get { return directions[3]; }
	}

	//=========== UPDATING ===========

	/** <summary> Called every step to update the control state. </summary> */
	public void Update(double time, Vector2F position) {
		// Apply the dead zone to the position
		if (position.Length <= deadZone) {
			position = Vector2F.Zero;
		}

		// Update the control state based on its position
		if (disabledState == DisableState.Enabled) {
			this.position = position;

			// Update the directional controls
			directions[0].Update(time, position.X > DirectionDeadZone.X);
			directions[1].Update(time, position.Y > DirectionDeadZone.Y);
			directions[2].Update(time, position.X < -DirectionDeadZone.X);
			directions[3].Update(time, position.Y < -DirectionDeadZone.Y);
		}
		else if (disabledState == DisableState.DisabledUntilRelease) {
			if (position.IsZero) {
				disabledState	= DisableState.Enabled;
				for (int i = 0; i < 4; i++)
					directions[i].Enable();
			}
		}
	}

	//========== MANAGEMENT ==========

	/** <summary> Resets the control state. </summary> */
	public void Reset(bool release = false) {
		position = Vector2F.Zero;
		for (int i = 0; i < 4; i++)
			directions[i].Reset(release);
	}
	/** <summary> Enables the control. </summary> */
	public void Enable() {
		disabledState = DisableState.Enabled;
		for (int i = 0; i < 4; i++)
			directions[i].Enable();
	}
	/** <summary> Disables the control. </summary> */
	public void Disable(bool untilRelease = false) {
		if (untilRelease) {
			if (!position.IsZero)
				disabledState = DisableState.DisabledUntilRelease;
			else
				position	= Vector2F.Zero;
		}
		else {
			position		= Vector2F.Zero;
			disabledState	= DisableState.Disabled;
		}
		for (int i = 0; i < 4; i++)
			directions[i].Disable(untilRelease);
	}

	//======== CONTROL STATES ========

	/** <summary> Returns true if the analog stick is centered. </summary> */
	public bool IsZero() {
		return position.IsZero;
	}
	/** <summary> Returns true if the analog stick is disabled. </summary> */
	public bool IsDisabled() {
		return (disabledState != DisableState.Enabled);
	}
}
} // End namespace
