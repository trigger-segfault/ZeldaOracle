using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;

namespace ConscriptDesigner.Util {
	public class ClipboardListener {

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
}
