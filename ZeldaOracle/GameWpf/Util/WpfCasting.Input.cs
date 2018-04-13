using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using FormsKeys = System.Windows.Forms.Keys;
using WpfKey = System.Windows.Input.Key;
using FormsMouseButtons = System.Windows.Forms.MouseButtons;
using WpfMouseButton = System.Windows.Input.MouseButton;
using WpfModifierKeys = System.Windows.Input.ModifierKeys;

namespace ZeldaOracle.Common.Util {
	/// <summary>Static Wpf extensions for casting geometry and colors.</summary>
	public static partial class WpfCasting {

		//-----------------------------------------------------------------------------
		// Constants
		//-----------------------------------------------------------------------------

		/// <summary>The constant for when no button is pressed.</summary>
		public const WpfMouseButton NoButton = (WpfMouseButton) byte.MaxValue;


		//-----------------------------------------------------------------------------
		// ModifierKeys
		//-----------------------------------------------------------------------------

		/// <summary>Casts the Forms Keys to Wpf ModifierKeys.</summary>
		public static WpfModifierKeys ToWpfModifierKeys(this FormsKeys keys) {
			WpfModifierKeys modifiers = WpfModifierKeys.None;
			if (keys.HasFlag(FormsKeys.Control))
				modifiers |= WpfModifierKeys.Control;
			if (keys.HasFlag(FormsKeys.Shift))
				modifiers |= WpfModifierKeys.Shift;
			if (keys.HasFlag(FormsKeys.Alt))
				modifiers |= WpfModifierKeys.Alt;
			return modifiers;
		}

		/// <summary>Casts the Wpf ModifierKeys to Forms Keys.</summary>
		public static FormsKeys ToFormsKeys(this WpfModifierKeys modifiers) {
			FormsKeys keys = FormsKeys.None;
			if (modifiers.HasFlag(WpfModifierKeys.Control))
				keys |= FormsKeys.Control;
			if (modifiers.HasFlag(WpfModifierKeys.Shift))
				keys |= FormsKeys.Shift;
			if (modifiers.HasFlag(WpfModifierKeys.Alt))
				keys |= FormsKeys.Alt;
			return keys;
		}


		//-----------------------------------------------------------------------------
		// Key
		//-----------------------------------------------------------------------------

		/// <summary>Casts the Forms Keys to a Wpf Key.</summary>
		public static WpfKey ToWpfKey(this FormsKeys keys) {
			return KeyInterop.KeyFromVirtualKey((int) keys);
		}

		/// <summary>Casts the Wpf Keys to a Forms Key.</summary>
		public static FormsKeys ToFormsKeys(this WpfKey key) {
			return (FormsKeys) KeyInterop.VirtualKeyFromKey(key);
		}


		//-----------------------------------------------------------------------------
		// MouseButton
		//-----------------------------------------------------------------------------

		/// <summary>Casts the Forms MouseButtons to a Wpf MouseButton.</summary>
		public static WpfMouseButton ToWpfMouseButton(this FormsMouseButtons buttons) {
			switch (buttons) {
			case FormsMouseButtons.Left: return WpfMouseButton.Left;
			case FormsMouseButtons.Middle: return WpfMouseButton.Middle;
			case FormsMouseButtons.Right: return WpfMouseButton.Right;
			case FormsMouseButtons.XButton1: return WpfMouseButton.XButton1;
			case FormsMouseButtons.XButton2: return WpfMouseButton.XButton2;
			}
			return NoButton;
		}

		/// <summary>Casts the Wpf MouseButton to a Forms MouseButtons.</summary>
		public static FormsMouseButtons ToFormsMouseButtons(this WpfMouseButton button) {
			switch (button) {
			case WpfMouseButton.Left: return FormsMouseButtons.Left;
			case WpfMouseButton.Middle: return FormsMouseButtons.Middle;
			case WpfMouseButton.Right: return FormsMouseButtons.Right;
			case WpfMouseButton.XButton1: return FormsMouseButtons.XButton1;
			case WpfMouseButton.XButton2: return FormsMouseButtons.XButton2;
			}
			return FormsMouseButtons.None;
		}
	}
}
