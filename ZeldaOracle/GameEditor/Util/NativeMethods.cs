using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ComponentModel;
using System.Reflection;

namespace ZeldaEditor.Util {
	public static class NativeMethods {
		/**<summary>The DeleteObject function deletes a logical pen, brush, font, bitmap,
		 * region, or palette, freeing all system resources associated with the object.
		 * After the object is deleted, the specified handle is no longer valid.</summary>*/
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
	}
}
