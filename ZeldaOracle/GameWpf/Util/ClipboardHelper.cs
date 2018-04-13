using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace ZeldaWpf.Util {
	/// <summary>A helper for clipboard related functions.</summary>
	public static class ClipboardHelper {

		//-----------------------------------------------------------------------------
		// Classes
		//-----------------------------------------------------------------------------

		/// <summary>The contained clipboard listener class. This is needed so that
		/// the deconstructor can be used.</summary>
		private class ClipboardListener {

			/// <summary>The source for adding and removing hooks.</summary>
			private HwndSource hwndSource = new HwndSource(0, 0, 0, 0, 0, 0, 0, null,
				WpfNativeMethods.HWND_MESSAGE);

			/// <summary>Constructs and sets up the clipboard listener.</summary>
			public ClipboardListener() {
				hwndSource.AddHook(WndProc);
				WpfNativeMethods.AddClipboardFormatListener(hwndSource.Handle);
			}

			/// <summary>Deconstructs and cleans up the clipboard listener.</summary>
			~ClipboardListener() {
				WpfNativeMethods.RemoveClipboardFormatListener(hwndSource.Handle);
				hwndSource.RemoveHook(WndProc);
				hwndSource.Dispose();
			}

			/// <summary>Checks for clipboard update messages.</summary>
			private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam,
				ref bool handled)
			{
				if (msg == WpfNativeMethods.WM_CLIPBOARDUPDATE)
					ClipboardChanged?.Invoke(this, EventArgs.Empty);
				return IntPtr.Zero;
			}

			/// <summary>Occurs when the clipboard has changed.</summary>
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
			listener.ClipboardChanged += OnClipboardChanged;
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
		// Event Handlers
		//-----------------------------------------------------------------------------

		/// <summary>Called to fire the clipboard changed event.</summary>
		private static void OnClipboardChanged(object sender, EventArgs e) {
			ClipboardChanged?.Invoke(null, e);
		}


		//-----------------------------------------------------------------------------
		// Events
		//-----------------------------------------------------------------------------

		/// <summary>Called when the clipboards' contents have been changed.</summary>
		public static event EventHandler ClipboardChanged;
	}
}
