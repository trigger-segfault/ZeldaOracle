using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using XnaMouse		= Microsoft.Xna.Framework.Input.Mouse;

using ZeldaOracle.Common.Geometry;
using Mouse			= ZeldaOracle.Common.Input.Mouse;

namespace ZeldaOracle.Common.Input {
/** <summary>
 * A static class for keeping track of mouse button states.
 * </summary> */
public static class Mouse {

	//========== CONSTANTS ===========
	#region Constants

	/** <summary> The number of mouse button states in the list. </summary> */
	private const int NumButtons	= 6;

	#endregion
	//========== VARIABLES ===========
	#region Variables

	// States
	/** <summary> The list of mouse buttons. </summary> */
	private static InputControl[] buttons;
	/** <summary> The list of raw mouse button down states. </summary> */
	private static bool[] rawButtonDown;
	/** <summary> The list of raw mouse button double clicked states. </summary> */
	private static bool[] rawButtonDoubleClick;
	/** <summary> The list of raw mouse button clicked states. </summary> */
	private static bool[] rawButtonClick;
	/** <summary> The delta position of the mouse wheel. </summary> */
	private static int wheelDelta;
	/** <summary> The raw delta position of the mouse wheel. </summary> */
	private static int rawWheelDelta;
	/** <summary> The client position of the mouse. </summary> */
	private static Vector2F mousePos;
	/** <summary> The last client position of the mouse. </summary> */
	private static Vector2F mousePosLast;
	/** <summary> True if the mouse is disabled. </summary> */
	private static bool disabled;
	/** <summary> The scale used to multiply the mouse position by. </summary> */
	private static double gameScale;

	#endregion
	//========= CONSTRUCTORS =========
	#region Constructors

	/** <summary> Initializes the mouse listener. </summary> */
	public static void Initialize() {
		// States
		buttons					= new InputControl[NumButtons];
		rawButtonDown			= new bool[NumButtons];
		rawButtonDoubleClick	= new bool[NumButtons];
		rawButtonClick			= new bool[NumButtons];
		wheelDelta				= 0;
		rawWheelDelta			= 0;
		mousePos				= Vector2F.Zero;
		mousePosLast			= Vector2F.Zero;
		disabled				= false;
		gameScale				= 1.0;

		// Setup
		for (int i = 0; i < NumButtons; i++) {
			buttons[i]				= new InputControl();
			rawButtonDown[i]		= false;
			rawButtonDoubleClick[i]	= false;
			rawButtonClick[i]		= false;
		}
	}
	/** <summary> Uninitializes the mouse listener. </summary> */
	public static void Uninitialize() {
		// States
		buttons					= null;
		rawButtonDown			= null;
		rawButtonDoubleClick	= null;
		rawButtonClick			= null;
	}

	#endregion
	//=========== UPDATING ===========
	#region Updating

	/** <summary> Simplifies the updating procedure for each button. </summary> */
	private static void UpdateButton(int time, MouseButtons mouseButton, ButtonState buttonState) {
		buttons[(int)mouseButton].Update(time, buttonState == ButtonState.Pressed, false, rawButtonDoubleClick[(int)mouseButton], rawButtonClick[(int)mouseButton]);
	}
	/** <summary> Called every step to update the mouse button states. </summary> */
	public static void Update(GameTime gameTime, Vector2F mouseOffset) {
		// Update each of the buttons
		UpdateButton(1, MouseButtons.None,		ButtonState.Released);
		UpdateButton(1, MouseButtons.Left, XnaMouse.GetState().LeftButton);
		UpdateButton(1, MouseButtons.Middle, XnaMouse.GetState().MiddleButton);
		UpdateButton(1, MouseButtons.Right, XnaMouse.GetState().RightButton);
		UpdateButton(1, MouseButtons.XButton1, XnaMouse.GetState().XButton1);
		UpdateButton(1, MouseButtons.XButton2, XnaMouse.GetState().XButton2);

		for (int i = 0; i < NumButtons; i++) {
			rawButtonDoubleClick[i]	= false;
			rawButtonClick[i]		= false;
		}

		int rawWheelDeltaLast	= rawWheelDelta;
		rawWheelDelta			= XnaMouse.GetState().ScrollWheelValue;
		if (!disabled) {
			// Update the mouse wheel
			wheelDelta			= rawWheelDeltaLast - rawWheelDelta;

			// Update the mouse position
			mousePosLast		= mousePos;
			mousePos			= (new Vector2F(XnaMouse.GetState().X, XnaMouse.GetState().Y) - mouseOffset) / (float)gameScale;
		}
	}
	/** <summary> Resets all the button states. </summary> */
	public static void Reset(bool release) {
		// Reset each of the buttons
		for (int i = 0; i < NumButtons; i++) {
			buttons[i].Reset(release);
			rawButtonDown[i]		= false;
			rawButtonDoubleClick[i]	= false;
			rawButtonClick[i]		= false;
		}

		wheelDelta	= 0;
	}
	/** <summary> Enables all the mouse buttons. </summary> */
	public static void Enable() {
		// Reset each of the buttons
		for (int i = 0; i < NumButtons; i++) {
			buttons[i].Enable();
		}

		disabled = false;
	}
	/** <summary> Disables all the mouse buttons. </summary> */
	public static void Disable(bool untilRelease) {
		// Reset each of the buttons
		for (int i = 0; i < NumButtons; i++) {
			buttons[i].Disable(untilRelease);
		}

		wheelDelta	= 0;

		if (!untilRelease)
			disabled = true;
	}

	#endregion
	//========= MOUSE EVENTS =========
	#region Mouse Events
	//--------------------------------
	#region Single Button States

	/** <summary> Returns true if the specified mouse button was double clicked. </summary> */
	public static bool IsButtonDoubleClicked(MouseButtons buttonCode) {
		return buttons[(int)buttonCode].IsDoubleClicked();
	}
	/** <summary> Returns true if the specified mouse button was double clicked. </summary> */
	public static bool IsButtonDoubleClicked(int buttonCode) {
		return buttons[buttonCode].IsDoubleClicked();
	}
	/** <summary> Returns true if the specified mouse button was clicked. </summary> */
	public static bool IsButtonClicked(MouseButtons buttonCode) {
		return buttons[(int)buttonCode].IsClicked();
	}
	/** <summary> Returns true if the specified mouse button was clicked. </summary> */
	public static bool IsButtonClicked(int buttonCode) {
		return buttons[buttonCode].IsClicked();
	}
	/** <summary> Returns true if the specified mouse button was pressed. </summary> */
	public static bool IsButtonPressed(MouseButtons buttonCode) {
		return buttons[(int)buttonCode].IsPressed();
	}
	/** <summary> Returns true if the specified mouse button was pressed. </summary> */
	public static bool IsButtonPressed(int buttonCode) {
		return buttons[buttonCode].IsPressed();
	}
	/** <summary> Returns true if the specified mouse button is down. </summary> */
	public static bool IsButtonDown(MouseButtons buttonCode) {
		return buttons[(int)buttonCode].IsDown();
	}
	/** <summary> Returns true if the specified mouse button is down. </summary> */
	public static bool IsButtonDown(int buttonCode) {
		return buttons[buttonCode].IsDown();
	}
	/** <summary> Returns true if the specified mouse button was released. </summary> */
	public static bool IsButtonReleased(MouseButtons buttonCode) {
		return buttons[(int)buttonCode].IsReleased();
	}
	/** <summary> Returns true if the specified mouse button was released. </summary> */
	public static bool IsButtonReleased(int buttonCode) {
		return buttons[buttonCode].IsReleased();
	}
	/** <summary> Returns true if the specified mouse button is up. </summary> */
	public static bool IsButtonUp(MouseButtons buttonCode) {
		return buttons[(int)buttonCode].IsUp();
	}
	/** <summary> Returns true if the specified mouse button is up. </summary> */
	public static bool IsButtonUp(int buttonCode) {
		return buttons[buttonCode].IsUp();
	}

	#endregion
	//--------------------------------
	#region Any Button State

	/** <summary> Returns true if any mouse button was double clicked. </summary> */
	public static bool IsAnyButtonDoubleClicked() {
		for (int i = 1; i < NumButtons; i++) {
			if (buttons[i].IsDoubleClicked())
				return true;
		}
		return false;
	}
	/** <summary> Returns true if any mouse button was clicked. </summary> */
	public static bool IsAnyButtonClicked() {
		for (int i = 1; i < NumButtons; i++) {
			if (buttons[i].IsClicked())
				return true;
		}
		return false;
	}
	/** <summary> Returns true if any mouse button was pressed. </summary> */
	public static bool IsAnyButtonPressed() {
		for (int i = 1; i < NumButtons; i++) {
			if (buttons[i].IsPressed())
				return true;
		}
		return false;
	}
	/** <summary> Returns true if any mouse button is down. </summary> */
	public static bool IsAnyButtonDown() {
		for (int i = 1; i < NumButtons; i++) {
			if (buttons[i].IsDown())
				return true;
		}
		return false;
	}
	/** <summary> Returns true if any mouse button was released. </summary> */
	public static bool IsAnyButtonReleased() {
		for (int i = 1; i < NumButtons; i++) {
			if (buttons[i].IsReleased())
				return true;
		}
		return false;
	}
	/** <summary> Returns true if any mouse button is up. </summary> */
	public static bool IsAnyButtonUp() {
		for (int i = 1; i < NumButtons; i++) {
			if (buttons[i].IsUp())
				return true;
		}
		return false;
	}

	#endregion
	//--------------------------------
	#region Mouse Wheel

	/** <summary> Returns true if the mouse wheel was scrolled up. </summary> */
	public static bool IsWheelUp() {
		return (wheelDelta < 0);
	}
	/** <summary> Returns true if the mouse wheel was scrolled down. </summary> */
	public static bool IsWheelDown() {
		return (wheelDelta > 0);
	}
	/** <summary> Returns true if the mouse wheel was scrolled. </summary> */
	public static bool IsWheelScrolled() {
		return (wheelDelta != 0);
	}
	/** <summary> Gets the mouse wheel delta position. </summary> */
	public static int GetWheelDelta() {
		return wheelDelta;
	}
	
	#endregion
	//--------------------------------
	#region Mouse Position

	/** <summary> Gets the mouse position. </summary> */
	public static Vector2F GetPosition() {
		return mousePos / (float)gameScale;
	}
	/** <summary> Gets the last mouse position. </summary> */
	public static Vector2F GetPositionLast() {
		return mousePosLast / (float)gameScale;
	}
	/** <summary> Gets the distance the mouse moved. </summary> */
	public static Vector2F GetDistance() {
		return (mousePos - mousePosLast) / (float)gameScale;
	}
	/** <summary> Sets the mouse position. </summary> */
	public static void SetPosition(Vector2F position) {
		XnaMouse.SetPosition((int)(position.X * gameScale), (int)(position.Y * (float)gameScale));
		mousePosLast = (position - (mousePos - mousePosLast)) * (float)gameScale;
		mousePos = position * (float)gameScale;
	}
	/** <summary> Returns true if the mouse was moved. </summary> */
	public static bool IsMouseMoved() {
		return (mousePos != mousePosLast);
	}

	#endregion
	//--------------------------------
	#region Mouse Information

	/** <summary> Returns true if the mouse is visible. </summary> */
	/*public static bool IsMouseVisible() {
		return gameBase.IsMouseVisible;
	}*/
	/** <summary> Sets the mouse visibility. </summary> */
	/*public static void SetMouseVisible(bool visible) {
		gameBase.IsMouseVisible = visible;
	}*/

	#endregion
	//--------------------------------
	#region Button Information

	/** <summary> Gets the control for the specified mouse button. </summary> */
	public static InputControl GetButton(MouseButtons buttonCode) {
		return buttons[(int)buttonCode];
	}
	/** <summary> Gets the control for the specified mouse button. </summary> */
	public static InputControl GetButton(int buttonCode) {
		return buttons[buttonCode];
	}
	/** <summary> Gets the name of the specified mouse button. </summary> */
	public static string GetButtonName(MouseButtons buttonCode) {
		return Enum.GetName(typeof(MouseButtons), buttonCode);
	}
	/** <summary> Gets the name of the specified mouse button. </summary> */
	public static string GetButtonName(int buttonCode) {
		return Enum.GetName(typeof(MouseButtons), (MouseButtons)buttonCode);
	}

	#endregion
	//--------------------------------
	#endregion
}
} // End namespace
