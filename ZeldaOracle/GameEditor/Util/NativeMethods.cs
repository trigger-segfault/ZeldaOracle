using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ZeldaEditor.Util {
	public static class NativeMethods {
		/**<summary>The DeleteObject function deletes a logical pen, brush, font, bitmap,
		 * region, or palette, freeing all system resources associated with the object.
		 * After the object is deleted, the specified handle is no longer valid.</summary>*/
		[DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool DeleteObject([In] IntPtr hObject);
	}
}
