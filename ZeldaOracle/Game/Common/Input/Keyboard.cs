using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using XnaKeyboard = Microsoft.Xna.Framework.Input.Keyboard;
using XnaKeys = Microsoft.Xna.Framework.Input.Keys;

using ZeldaOracle.Common.Geometry;
using Keyboard = ZeldaOracle.Common.Input.Keyboard;
using Keys = ZeldaOracle.Common.Input.Keys;
using ZeldaOracle.Game.Main;

namespace ZeldaOracle.Common.Input {
	/// <summary>A static class for keeping track of key states.</summary>
	public static class Keyboard {

		//-----------------------------------------------------------------------------
		// Constants
		//-----------------------------------------------------------------------------

		/// <summary>The number of key states in the list.</summary>
		private const int NumKeys		= 256;


		//-----------------------------------------------------------------------------
		// Members
		//-----------------------------------------------------------------------------

		// States
		/// <summary>The list of keys.</summary>
		private static InputControl[] keys;
		/// <summary>The list of raw key down states.</summary>
		private static bool[] rawKeyDown;
		/// <summary>The list of raw key typed states.</summary>
		private static bool[] rawKeyTyped;
		/// <summary>The character that was typed.</summary>
		private static char charTyped;
		/// <summary>The raw character that was typed.</summary>
		private static char rawCharTyped;
		/// <summary>True if the keyboard is disabled.</summary>
		private static bool disabled;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Initializes the keyboard listener.</summary>
		public static void Initialize() {
			// States
			keys			= new InputControl[NumKeys];
			rawKeyDown		= new bool[NumKeys];
			rawKeyTyped		= new bool[NumKeys];
			charTyped		= '\0';
			rawCharTyped	= '\0';
			disabled		= false;

			// Setup
			for (int i = 0; i < NumKeys; i++) {
				keys[i]			= new InputControl();
				rawKeyDown[i]	= false;
				rawKeyTyped[i]	= false;
			}

			EventInput.CharEntered += delegate(object sender, CharacterEventArgs e) {
				rawCharTyped = e.Character;
			};
			EventInput.KeyDown += delegate(object sender, KeyEventArgs e) {
				rawKeyTyped[(int)e.KeyCode] = true;
			};
		}

		/// <summary>Uninitializes the keyboard listener.</summary>
		public static void Uninitialize() {
			// States
			keys			= null;
			rawKeyDown		= null;
			rawKeyTyped		= null;
		}


		//-----------------------------------------------------------------------------
		// Updating
		//-----------------------------------------------------------------------------

		/// <summary>Called every step to update the key states.</summary>
		public static void Update(GameTime gameTime) {
			// Update each of the keys
			for (int i = 0; i < NumKeys; i++) {
				keys[i].Update(1, XnaKeyboard.GetState().IsKeyDown((XnaKeys)i), rawKeyTyped[i]);

				rawKeyTyped[i] = false;
			}

			// Update the typed character
			charTyped		= rawCharTyped;
			rawCharTyped	= '\0';
		}

		/// <summary>Resets all the key states.</summary>
		public static void Reset(bool release) {
			// Reset each of the keys
			for (int i = 0; i < NumKeys; i++) {
				keys[i].Reset(release);
			}

			// Reset the typed character
			charTyped		= '\0';
			rawCharTyped	= '\0';
		}

		/// <summary>Enables all the keys.</summary>
		public static void Enable() {
			// Reset each of the keys
			for (int i = 0; i < NumKeys; i++) {
				keys[i].Enable();
			}

			disabled = false;
		}

		/// <summary>Disables all the keys.</summary>
		public static void Disable(bool untilRelease) {
			// Reset each of the keys
			for (int i = 0; i < NumKeys; i++) {
				keys[i].Disable(untilRelease);
			}

			// Reset the typed character
			charTyped		= '\0';
			rawCharTyped	= '\0';

			if (!untilRelease)
				disabled = true;
		}


		//-----------------------------------------------------------------------------
		// Key States
		//-----------------------------------------------------------------------------
		
		// Single Key States ----------------------------------------------------------

		/// <summary>Returns true if the specified key was pressed.</summary>
		public static bool IsKeyPressed(Keys keyCode) {
			return keys[(int)keyCode].IsPressed();
		}

		/// <summary>Returns true if the specified key was pressed.</summary>
		public static bool IsKeyPressed(int keyCode) {
			return keys[keyCode].IsPressed();
		}

		/// <summary>Returns true if the specified key is down.</summary>
		public static bool IsKeyDown(Keys keyCode) {
			return keys[(int)keyCode].IsDown();
		}

		/// <summary>Returns true if the specified key is down.</summary>
		public static bool IsKeyDown(int keyCode) {
			return keys[keyCode].IsDown();
		}

		/// <summary>Returns true if the specified key was released.</summary>
		public static bool IsKeyReleased(Keys keyCode) {
			return keys[(int)keyCode].IsReleased();
		}

		/// <summary>Returns true if the specified key was released.</summary>
		public static bool IsKeyReleased(int keyCode) {
			return keys[keyCode].IsReleased();
		}

		/// <summary>Returns true if the specified key is up.</summary>
		public static bool IsKeyUp(Keys keyCode) {
			return keys[(int)keyCode].IsUp();
		}

		/// <summary>Returns true if the specified key is up.</summary>
		public static bool IsKeyUp(int keyCode) {
			return keys[keyCode].IsUp();
		}

		/// <summary>Returns true if the specified key was typed.</summary>
		public static bool IsKeyTyped(Keys keyCode) {
			return keys[(int)keyCode].IsTyped();
		}

		/// <summary>Returns true if the specified key was typed.</summary>
		public static bool IsKeyTyped(int keyCode) {
			return keys[keyCode].IsTyped();
		}
		
		// Any Key States -------------------------------------------------------------

		/// <summary>Returns true if any key was pressed.</summary>
		public static bool IsAnyKeyPressed() {
			for (int i = 0; i < NumKeys; i++) {
				if (keys[i].IsPressed())
					return true;
			}
			return false;
		}

		/// <summary>Returns true if any key is down.</summary>
		public static bool IsAnyKeyDown() {
			for (int i = 0; i < NumKeys; i++) {
				if (keys[i].IsDown())
					return true;
			}
			return false;
		}

		/// <summary>Returns true if any key was released.</summary>
		public static bool IsAnyKeyReleased() {
			for (int i = 0; i < NumKeys; i++) {
				if (keys[i].IsReleased())
					return true;
			}
			return false;
		}

		/// <summary>Returns true if any key is up.</summary>
		public static bool IsAnyKeyUp() {
			for (int i = 0; i < NumKeys; i++) {
				if (keys[i].IsUp())
					return true;
			}
			return false;
		}

		/// <summary>Returns true if any key was typed.</summary>
		public static bool IsAnyKeyTyped() {
			for (int i = 0; i < NumKeys; i++) {
				if (keys[i].IsTyped())
					return true;
			}
			return false;
		}

		// Typing Key States ----------------------------------------------------------

		/// <summary>Returns true if a character was typed.</summary>
		public static bool IsCharTyped() {
			return (charTyped != '\0' && !disabled);
		}

		/// <summary>Gets the character that was typed.</summary>
		public static char GetCharTyped() {
			if (disabled)
				return '\0';
			return charTyped;
		}


		//-----------------------------------------------------------------------------
		// Key Information
		//-----------------------------------------------------------------------------

		/// <summary>Gets the specified key.</summary>
		public static InputControl GetKey(Keys keyCode) {
			return keys[(int)keyCode];
		}

		/// <summary>Gets the specified key.</summary>
		public static InputControl GetKey(int keyCode) {
			return keys[keyCode];
		}

		/// <summary>Gets the name of the specified key.</summary>
		public static string GetKeyName(Keys keyCode) {
			string name = Enum.GetName(typeof(Keys), keyCode);
			if (name.Length == 2 && name[0] == 'D' && name[1] >= '0' && name[1] <= '9')
				return name[1].ToString();
			return name;
		}

		/// <summary>Gets the name of the specified key.</summary>
		public static string GetKeyName(int keyCode) {
			string name = Enum.GetName(typeof(Keys), keyCode);
			if (name.Length == 2 && name[0] == 'D' && name[1] >= '0' && name[1] <= '9')
				return name[1].ToString();
			return name;
		}
	}
}
