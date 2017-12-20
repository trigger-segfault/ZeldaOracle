using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ComponentModel;
using System.Reflection;

namespace ConscriptDesigner.Util {
	public static class NativeMethods {
		/// <summary>The DeleteObject function deletes a logical pen, brush, font, bitmap,
		/// region, or palette, freeing all system resources associated with the object.
		/// After the object is deleted, the specified handle is no longer valid.</summary>
		[DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool DeleteObject([In] IntPtr hObject);

		[DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		private static extern IntPtr LoadCursorFromFile(string path);

		public static Cursor LoadCustomCursor(string path) {
			IntPtr hCurs = LoadCursorFromFile(path);
			if (hCurs == IntPtr.Zero) throw new Win32Exception();
			var curs = new Cursor(hCurs);
			// Note: force the cursor to own the handle so it gets released properly
			var fi = typeof(Cursor).GetField("ownHandle", BindingFlags.NonPublic | BindingFlags.Instance);
			fi.SetValue(curs, true);
			return curs;
		}

		public const int WM_MOUSEWHEEL = 0x020A;

		[DllImport("user32.dll")]
		public static extern int SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

		/// <summary>Places the given window in the system-maintained clipboard
		/// format listener list.</summary>
		[DllImport("user32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool AddClipboardFormatListener(IntPtr hwnd);

		/// <summary>Removes the given window from the system-maintained clipboard
		/// format listener list.</summary>
		[DllImport("user32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool RemoveClipboardFormatListener(IntPtr hwnd);

		/// <summary>Sent when the contents of the clipboard have changed.</summary>
		public const int WM_CLIPBOARDUPDATE = 0x031D;

		/// <summary>To find message-only windows, specify HWND_MESSAGE in the
		/// hwndParent parameter of the FindWindowEx function.</summary>
		public static IntPtr HWND_MESSAGE = new IntPtr(-3);
	}
}
