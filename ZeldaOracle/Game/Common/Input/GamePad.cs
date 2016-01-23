using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using XnaGamePad	= Microsoft.Xna.Framework.Input.GamePad;
using XnaButtons	= Microsoft.Xna.Framework.Input.Buttons;
using XnaPlayer		= Microsoft.Xna.Framework.PlayerIndex;

using ZeldaOracle.Common.Input.Controls;
using ZeldaOracle.Common.Geometry;
using GamePad		= ZeldaOracle.Common.Input.GamePad;

namespace ZeldaOracle.Common.Input {
/** <summary>
 * A static class for keeping track of gamepad button states.
 * </summary> */
public static class GamePad {

	//========== CONSTANTS ===========
	#region Constants

	/** <summary> The number of button states in the list. </summary> */
	private const int NumButtons	= 26;
	/** <summary> The number of analog sticks in the list. </summary> */
	private const int NumSticks		= 4;
	/** <summary> The number of triggers in the list. </summary> */
	private const int NumTriggers	= 3;

	#endregion
	//========== VARIABLES ===========
	#region Variables

	// States
	/** <summary> The list of buttons for each gamepad. </summary> */
	private static InputControl[,] buttons;
	/** <summary> The list of analog sticks for each gamepad. </summary> */
	private static AnalogStick[,] sticks;
	/** <summary> The list of triggers for each gamepad. </summary> */
	private static Trigger[,] triggers;

	/** <summary> True if the gamepad is disabled. </summary> */
	//private static bool disabled; // I commented 'disabled' out because it is unused.

	#endregion
	//========= CONSTRUCTORS =========
	#region Constructors

	/** <summary> Initializes the gamepad listener. </summary> */
	public static void Initialize() {
		// States
		buttons			= new InputControl[4, NumButtons];
		sticks			= new AnalogStick[4, NumSticks];
		triggers		= new Trigger[4, NumTriggers];
		//disabled		= false;

		// Setup
		for (int i = 0; i < 4; i++) {
			for (int j = 0; j < NumButtons; j++)
				buttons[i, j] = new InputControl();
			for (int j = 0; j < NumSticks; j++)
				sticks[i, j] = new AnalogStick();
			for (int j = 0; j < NumTriggers; j++)
				triggers[i, j] = new Trigger();

			buttons[i, (int)Buttons.LeftStickRight]		= sticks[i, (int)Buttons.LeftStick].Right;
			buttons[i, (int)Buttons.LeftStickDown]		= sticks[i, (int)Buttons.LeftStick].Down;
			buttons[i, (int)Buttons.LeftStickLeft]		= sticks[i, (int)Buttons.LeftStick].Left;
			buttons[i, (int)Buttons.LeftStickUp]		= sticks[i, (int)Buttons.LeftStick].Up;

			buttons[i, (int)Buttons.RightStickRight]	= sticks[i, (int)Buttons.RightStick].Right;
			buttons[i, (int)Buttons.RightStickDown]		= sticks[i, (int)Buttons.RightStick].Down;
			buttons[i, (int)Buttons.RightStickLeft]		= sticks[i, (int)Buttons.RightStick].Left;
			buttons[i, (int)Buttons.RightStickUp]		= sticks[i, (int)Buttons.RightStick].Up;

			buttons[i, (int)Buttons.DPadRight]			= sticks[i, (int)Buttons.DPad].Right;
			buttons[i, (int)Buttons.DPadDown]			= sticks[i, (int)Buttons.DPad].Down;
			buttons[i, (int)Buttons.DPadLeft]			= sticks[i, (int)Buttons.DPad].Left;
			buttons[i, (int)Buttons.DPadUp]				= sticks[i, (int)Buttons.DPad].Up;
			sticks[i, (int)Buttons.DPad].DirectionDeadZone	= Vector2F.Zero;

			buttons[i, (int)Buttons.LeftTriggerButton]	= triggers[i, (int)Buttons.LeftTrigger].Button;
			buttons[i, (int)Buttons.RightTriggerButton]	= triggers[i, (int)Buttons.RightTrigger].Button;
		}
	}
	/** <summary> Uninitializes the keyboard listener. </summary> */
	public static void Uninitialize() {
		// States
		buttons			= null;
		sticks			= null;
		triggers		= null;
	}

	#endregion
	//=========== UPDATING ===========
	#region Updating

	/** <summary> Called every step to update the button states. </summary> */
	public static void Update(GameTime gameTime) {

		for (int i = 0; i < 4; i++) {
			XnaPlayer player = (XnaPlayer)i;
			buttons[i, (int)Buttons.A].Update(1, XnaGamePad.GetState(player).IsButtonDown(XnaButtons.A));
			buttons[i, (int)Buttons.B].Update(1, XnaGamePad.GetState(player).IsButtonDown(XnaButtons.B));
			buttons[i, (int)Buttons.X].Update(1, XnaGamePad.GetState(player).IsButtonDown(XnaButtons.X));
			buttons[i, (int)Buttons.Y].Update(1, XnaGamePad.GetState(player).IsButtonDown(XnaButtons.Y));

			buttons[i, (int)Buttons.Start].Update(1, XnaGamePad.GetState(player).IsButtonDown(XnaButtons.Start));
			buttons[i, (int)Buttons.Back].Update(1, XnaGamePad.GetState(player).IsButtonDown(XnaButtons.Back));

			buttons[i, (int)Buttons.Home].Update(1, XnaGamePad.GetState(player).IsButtonDown(XnaButtons.BigButton));
			buttons[i, (int)Buttons.LeftShoulder].Update(1, XnaGamePad.GetState(player).IsButtonDown(XnaButtons.LeftShoulder));
			buttons[i, (int)Buttons.RightShoulder].Update(1, XnaGamePad.GetState(player).IsButtonDown(XnaButtons.RightShoulder));
			buttons[i, (int)Buttons.LeftStickButton].Update(1, XnaGamePad.GetState(player).IsButtonDown(XnaButtons.LeftStick));
			buttons[i, (int)Buttons.RightStickButton].Update(1, XnaGamePad.GetState(player).IsButtonDown(XnaButtons.RightStick));

			Vector2F dPad = Vector2F.Zero;
			if (XnaGamePad.GetState(player).IsButtonDown(XnaButtons.DPadRight))
				dPad.X = 1;
			if (XnaGamePad.GetState(player).IsButtonDown(XnaButtons.DPadDown))
				dPad.Y = 1;
			if (XnaGamePad.GetState(player).IsButtonDown(XnaButtons.DPadLeft))
				dPad.X = -1;
			if (XnaGamePad.GetState(player).IsButtonDown(XnaButtons.DPadUp))
				dPad.Y = -1;
			if (dPad.X != 0.0f && dPad.Y != 0.0f) {
				dPad.X /= GMath.Sqrt(2.0f);
				dPad.Y /= GMath.Sqrt(2.0f);
			}
			sticks[i, (int)Buttons.DPad].Update(1, dPad);

			Vector2F stick = Vector2F.Zero;

			stick = XnaGamePad.GetState(player).ThumbSticks.Left;
			stick.Y *= -1;
			sticks[i, (int)Buttons.LeftStick].Update(1, stick);

			stick = XnaGamePad.GetState(player).ThumbSticks.Right;
			stick.Y *= -1;
			sticks[i, (int)Buttons.RightStick].Update(1, stick);


			float trigger = XnaGamePad.GetState(player).Triggers.Left;
			triggers[i, (int)Buttons.LeftTrigger].Update(1, trigger);
			trigger = XnaGamePad.GetState(player).Triggers.Right;
			triggers[i, (int)Buttons.RightTrigger].Update(1, trigger);
		}
	}
	/** <summary> Resets all the button states. </summary> */
	public static void Reset(bool release) {
		// Reset each of the buttons
		for (int i = 0; i < 4; i++) {
			for (int j = 0; j < NumButtons; j++)
				buttons[i, j].Reset(release);
			for (int j = 0; j < NumSticks; j++)
				sticks[i, j].Reset(release);
			for (int j = 0; j < NumTriggers; j++)
				triggers[i, j].Reset(release);
		}
	}
	/** <summary> Enables all the buttons. </summary> */
	public static void Enable() {
		// Reset each of the buttons
		for (int i = 0; i < 4; i++) {
			for (int j = 0; j < NumButtons; j++)
				buttons[i, j].Enable();
			for (int j = 0; j < NumSticks; j++)
				sticks[i, j].Enable();
			for (int j = 0; j < NumTriggers; j++)
				triggers[i, j].Enable();
		}

		//disabled = false;
	}
	/** <summary> Disables all the buttons. </summary> */
	public static void Disable(bool untilRelease) {
		// Reset each of the buttons
		for (int i = 0; i < 4; i++) {
			for (int j = 0; j < NumButtons; j++)
				buttons[i, j].Disable(untilRelease);
			for (int j = 0; j < NumSticks; j++)
				sticks[i, j].Disable(untilRelease);
			for (int j = 0; j < NumTriggers; j++)
				triggers[i, j].Disable(untilRelease);
		}

		//if (!untilRelease)
			//disabled = true;
	}

	#endregion
	//======== GAMEPAD EVENTS ========
	#region GamePad Events
	//--------------------------------
	#region Single Button Events

	/** <summary> Returns true if the specified button was pressed. </summary> */
	public static bool IsButtonPressed(Buttons buttonCode, int player = 0) {
		return buttons[player, (int)buttonCode].IsPressed();
	}
	/** <summary> Returns true if the specified button was pressed. </summary> */
	public static bool IsButtonPressed(int buttonCode, int player = 0) {
		return buttons[player, buttonCode].IsPressed();
	}
	/** <summary> Returns true if the specified button is down. </summary> */
	public static bool IsButtonDown(Buttons buttonCode, int player = 0) {
		return buttons[player, (int)buttonCode].IsDown();
	}
	/** <summary> Returns true if the specified button is down. </summary> */
	public static bool IsButtonDown(int buttonCode, int player = 0) {
		return buttons[player, buttonCode].IsDown();
	}
	/** <summary> Returns true if the specified button was released. </summary> */
	public static bool IsButtonReleased(Buttons buttonCode, int player = 0) {
		return buttons[player, (int)buttonCode].IsReleased();
	}
	/** <summary> Returns true if the specified button was released. </summary> */
	public static bool IsButtonReleased(int buttonCode, int player = 0) {
		return buttons[player, buttonCode].IsReleased();
	}
	/** <summary> Returns true if the specified button is up. </summary> */
	public static bool IsButtonUp(Buttons buttonCode, int player = 0) {
		return buttons[player, (int)buttonCode].IsUp();
	}
	/** <summary> Returns true if the specified button is up. </summary> */
	public static bool IsButtonUp(int buttonCode, int player = 0) {
		return buttons[player, buttonCode].IsUp();
	}

	#endregion
	//--------------------------------
	#region Any Button Events

	/** <summary> Returns true if any button was pressed. </summary> */
	public static bool IsAnyButtonPressed(int player = 0) {
		for (int i = 1; i < NumButtons; i++) {
			if (buttons[player, i].IsPressed())
				return true;
		}
		return false;
	}
	/** <summary> Returns true if any button is down. </summary> */
	public static bool IsAnyButtonDown(int player = 0) {
		for (int i = 1; i < NumButtons; i++) {
			if (buttons[player, i].IsDown())
				return true;
		}
		return false;
	}
	/** <summary> Returns true if any button was released. </summary> */
	public static bool IsAnyButtonReleased(int player = 0) {
		for (int i = 1; i < NumButtons; i++) {
			if (buttons[player, i].IsReleased())
				return true;
		}
		return false;
	}
	/** <summary> Returns true if any button is up. </summary> */
	public static bool IsAnyButtonUp(int player = 0) {
		for (int i = 1; i < NumButtons; i++) {
			if (buttons[player, i].IsUp())
				return true;
		}
		return false;
	}

	#endregion
	//--------------------------------
	#region Special Control Events

	/** <summary> Gets the position of the specified analog stick. </summary> */
	public static Vector2F GetStickPosition(Buttons buttonCode, int player = 0) {
		return sticks[player, (int)buttonCode].Position;
	}
	/** <summary> Gets the position of the specified analog stick. </summary> */
	public static Vector2F GetStickPosition(int buttonCode, int player = 0) {
		return sticks[player, buttonCode].Position;
	}
	/** <summary> Gets the position of the specified trigger. </summary> */
	public static double GetTriggerPressure(Buttons buttonCode, int player = 0) {
		return triggers[player, (int)buttonCode].Pressure;
	}
	/** <summary> Gets the position of the specified trigger. </summary> */
	public static double GetTriggerPressure(int buttonCode, int player = 0) {
		return triggers[player, buttonCode].Pressure;
	}

	#endregion
	//--------------------------------
	#region GamePad Information

	/** <summary> Gets the control for the specified gamepad button. </summary> */
	public static InputControl GetButton(Buttons buttonCode, int player = 0) {
		return buttons[player, (int)buttonCode];
	}
	/** <summary> Gets the control for the specified gamepad button. </summary> */
	public static InputControl GetButton(int buttonCode, int player = 0) {
		return buttons[player, buttonCode];
	}
	/** <summary> Gets the control for the specified gamepad analog stick. </summary> */
	public static AnalogStick GetStick(Buttons buttonCode, int player = 0) {
		return sticks[player, (int)buttonCode];
	}
	/** <summary> Gets the control for the specified gamepad analog stick. </summary> */
	public static AnalogStick GetStick(int buttonCode, int player = 0) {
		return sticks[player, buttonCode];
	}
	/** <summary> Gets the control for the specified gamepad trigger. </summary> */
	public static Trigger GetTrigger(Buttons buttonCode, int player = 0) {
		return triggers[player, (int)buttonCode];
	}
	/** <summary> Gets the control for the specified gamepad trigger. </summary> */
	public static Trigger GetTrigger(int buttonCode, int player = 0) {
		return triggers[player, buttonCode];
	}
	/** <summary> Gets the name of the specified gamepad button. </summary> */
	public static string GetButtonName(Buttons buttonCode) {
		return Enum.GetName(typeof(Buttons), buttonCode);
	}
	/** <summary> Gets the name of the specified gamepad button. </summary> */
	public static string GetButtonName(int buttonCode) {
		return Enum.GetName(typeof(Buttons), (Buttons)buttonCode);
	}

	#endregion
	//--------------------------------
	#endregion
}
} // End namespace
