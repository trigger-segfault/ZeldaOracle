using System;

namespace ZeldaOracle.Common.Input {
/** <summary>
 * A class to store control data.
 * </summary> */
public class InputControl {

	public const double DoubleClickTime = 0.150;

	//=========== MEMBERS ============

	/** <summary> The pressed state of the control. </summary> */
	private InputState state;
	/** <summary> The disabled state of the control. </summary> */
	private DisableState disabledState;
	/** <summary> True if the control was double clicked. </summary> */
	private bool doubleClicked;
	/** <summary> True if the control was clicked. </summary> */
	private bool clicked;
	/** <summary> True if the control was typed. </summary> */
	private bool typed;
	/** <summary> The time the key has been held for since being pressed, or released. </summary> */
	private double holdTime;

	//========= CONSTRUCTORS =========

	/** <summary> Constructs the default control. </summary> */
	public InputControl() {
		this.state			= InputState.Up;
		this.disabledState	= DisableState.Enabled;
		this.doubleClicked	= false;
		this.clicked		= false;
		this.typed			= false;
		this.holdTime		= 0.0;
	}

	//========== PROPERTIES ==========

	/** <summary> The time the key has been held for since being pressed, or released. </summary> */
	public double HoldTime {
		get { return holdTime; }
	}

	//=========== UPDATING ===========

	/** <summary> Called every step to update the control state. </summary> */
	public void Update(double time, bool down, bool typed = false, bool doubleClicked = false, bool clicked = false) {
		// Update the duration of the control state
		holdTime += time;

		// Update the double pressed and typed state
		if (disabledState == DisableState.Enabled) {
			this.doubleClicked	= doubleClicked;
			this.clicked		= clicked;
			this.typed			= typed;
		}

		// Update the control state based on its down state
		if (down) {
			if (disabledState == DisableState.Enabled) {
				if (state == InputState.Pressed)
					state = InputState.Down;
				if (state == InputState.Released || state == InputState.Up) {
					state = InputState.Pressed;
					if (holdTime < DoubleClickTime)
						this.doubleClicked = true;
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

	//========== MANAGEMENT ==========

	/** <summary> Resets the control state. </summary> */
	public void Reset(bool release = false) {
		if ((state == InputState.Pressed || state == InputState.Down) && release)
			state = InputState.Released;
		else
			state = InputState.Up;
		doubleClicked	= false;
		clicked			= false;
		typed			= false;
		holdTime		= 0;
	}
	/** <summary> Enables the control. </summary> */
	public void Enable() {
		disabledState = DisableState.Enabled;
	}
	/** <summary> Disables the control. </summary> */
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
		doubleClicked	= false;
		clicked			= false;
		typed			= false;
		holdTime		= 0;
	}

	//======== CONTROL STATES ========

	/** <summary> Returns true if the control was double clicked. </summary> */
	public bool IsDoubleClicked() {
		return (doubleClicked && disabledState == DisableState.Enabled);
	}
	/** <summary> Returns true if the control was clicked. </summary> */
	public bool IsClicked() {
		return ((doubleClicked || clicked) && disabledState == DisableState.Enabled);
	}
	/** <summary> Returns true if the control was pressed. </summary> */
	public bool IsPressed() {
		return (state == InputState.Pressed && disabledState == DisableState.Enabled);
	}
	/** <summary> Returns true if the control is down. </summary> */
	public bool IsDown() {
		return ((state == InputState.Pressed || state == InputState.Down) &&
				disabledState == DisableState.Enabled);
	}
	/** <summary> Returns true if the control was released. </summary> */
	public bool IsReleased() {
		return (state == InputState.Released && disabledState == DisableState.Enabled);
	}
	/** <summary> Returns true if the control is up. </summary> */
	public bool IsUp() {
		return (state == InputState.Released || state == InputState.Up ||
				disabledState != DisableState.Enabled);
	}
	/** <summary> Returns true if the control was typed. </summary> */
	public bool IsTyped() {
		return (typed && disabledState == DisableState.Enabled);
	}
	/** <summary> Returns true if the control is disabled. </summary> */
	public bool IsDisabled() {
		return (disabledState != DisableState.Enabled);
	}
}
} // End namespace
