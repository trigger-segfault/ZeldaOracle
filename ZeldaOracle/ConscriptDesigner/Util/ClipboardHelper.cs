using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace ConscriptDesigner.Util {
	/// <summary>A helper for clipboard related functions.</summary>
	public static class ClipboardHelper {

		//-----------------------------------------------------------------------------
		// Classes
		//-----------------------------------------------------------------------------

		/// <summary>The contained clipboard listener class.</summary>
		private class ClipboardListener {

			private HwndSource hwndSource = new HwndSource(0, 0, 0, 0, 0, 0, 0, null, NativeMethods.HWND_MESSAGE);

			public ClipboardListener() {
				hwndSource.AddHook(WndProc);
				NativeMethods.AddClipboardFormatListener(hwndSource.Handle);
			}

			~ClipboardListener() {
				NativeMethods.RemoveClipboardFormatListener(hwndSource.Handle);
				hwndSource.RemoveHook(WndProc);
				hwndSource.Dispose();
			}

			private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled) {
				if (msg == NativeMethods.WM_CLIPBOARDUPDATE) {
					ClipboardChanged?.Invoke(this, EventArgs.Empty);
				}

				return IntPtr.Zero;
			}
			
			public event EventHandler ClipboardChanged;
		}


		//-----------------------------------------------------------------------------
		// Members
		//-----------------------------------------------------------------------------

		/// <summary>The clipboard listener instance.</summary>
		private static ClipboardListener listener;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		/// <summary>Initializes the clipboard listener.</summary>
		static ClipboardHelper() {
			listener = new ClipboardListener();
		}


		//-----------------------------------------------------------------------------
		// Accessors
		//-----------------------------------------------------------------------------

		/// <summary>Returns true if the clipboard is empty.</summary>
		public static bool IsEmpty() {
			var data = Clipboard.GetDataObject();
			if (data == null)
				return true;
			else
				return !data.GetFormats().Any();
		}

		//-----------------------------------------------------------------------------
		// Events
		//-----------------------------------------------------------------------------

		/// <summary>Called when the clipboards' contents have been changed.</summary>
		public static event EventHandler ClipboardChanged;
	}
}
