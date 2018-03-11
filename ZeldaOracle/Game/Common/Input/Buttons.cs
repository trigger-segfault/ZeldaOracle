using System;

namespace ZeldaOracle.Common.Input {
	/// <summary>The control codes for each of the analog stick on a gamepad.</summary>
	public enum AnalogSticks {

		None = 0,

		// Analog Sticks
		LeftStick = 1,
		RightStick = 2,
		DPad = 3,
	}

	/// <summary>The control codes for each of the trigger on a gamepad.</summary>
	public enum Triggers {

		None = 0,

		// Triggers
		LeftTrigger = 1,
		RightTrigger = 2,
	}

	/// <summary>The control codes for each of the buttons on a gamepad.</summary>
	public enum Buttons {
		
		None				= 0,

		// Basic
		A					= 1,
		B					= 2,
		X					= 3,
		Y					= 4,
		Start				= 5,
		Back				= 6,

		// Special
		Home				= 7,
		LeftShoulder		= 8,
		RightShoulder		= 9,
		LeftStickButton		= 10,
		RightStickButton	= 11,

		// DPad
		DPadRight			= 12,
		DPadDown			= 13,
		DPadLeft			= 14,
		DPadUp				= 15,

		// Analog Buttons
		LeftStickRight		= 16,
		LeftStickDown		= 17,
		LeftStickLeft		= 18,
		LeftStickUp			= 19,

		RightStickRight		= 20,
		RightStickDown		= 21,
		RightStickLeft		= 22,
		RightStickUp		= 23,

		LeftTriggerButton	= 24,
		RightTriggerButton	= 25,

	}
}
