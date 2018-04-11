using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Threading;
using System.Windows.Forms;
using ZeldaOracle.Common.Util;

namespace ZeldaWpf.Util {
	/// <summary>A static class for helper methods for loading WinForms color cursors.</summary>
	public static class CursorLoader {

		/// <summary>Loads a WinForms color cursor from file.</summary>
		public static Cursor FromFile(string path) {
			IntPtr hCurs = WpfNativeMethods.LoadCursorFromFile(path);
			if (hCurs == IntPtr.Zero) throw new Win32Exception();
			Cursor curs = new Cursor(hCurs);
			// Note: force the cursor to own the handle so it gets released properly
			FieldInfo fieldInfo = typeof(Cursor).GetField("ownHandle",
				BindingFlags.NonPublic | BindingFlags.Instance);
			fieldInfo.SetValue(curs, true);
			return curs;
		}

		/// <summary>Loads a WinForms color cursor from a stream.</summary>
		public static Cursor FromStream(Stream stream) {
			// Color cursors can only be loaded from file with P/Invoke,
			// so we save the stream to a file first before loading it.
			BinaryReader reader = new BinaryReader(stream);
			string path = "";
			// Retry up to 5 times
			const int LoadRetries = 5;
			for (int i = 0; i < LoadRetries; i++) {
				path = "CustomCursor" + (i + 1) + ".cur";
				try {
					using (Stream output = File.OpenWrite(path)) {
						BinaryWriter writer = new BinaryWriter(output);
						writer.Write(reader.ReadRemaining());
						break;
					}
				}
				catch (IOException ex) {
					if (i + 1 == LoadRetries)
						throw ex;
				}
			}

			// Load the color cursor from file.
			Cursor cursor = FromFile(path);

			// Retry to delete the file up to 5 times
			const int DeleteRetries = 5;
			for (int i = 0; i < DeleteRetries; i++) {
				try {
					File.Delete(path);
					break;
				}
				catch (IOException ex) {
					if (i + 1 == DeleteRetries)
						throw ex;
					Thread.Sleep(50);
				}
			}
			return cursor;
		}

		/// <summary>Loads a WinForms color cursor from an embedded resource of the
		/// specified assembly.</summary>
		public static Cursor FromEmbeddedResource(string rootNamespace,
			Assembly assembly, string path)
		{
			ResourceManager rm = new ResourceManager(rootNamespace + ".g", assembly);
			Stream stream = (Stream) rm.GetObject(path.ToLower());
			stream.Position = 0;
			return FromStream(stream);
		}

		/// <summary>Loads a WinForms color cursor from an embedded resource of the
		/// type's assembly.</summary>
		public static Cursor FromEmbeddedResource(string rootNamespace,
			Type type, string path)
		{
			return FromEmbeddedResource(rootNamespace, type.Assembly, path);
		}

		/// <summary>Loads a WinForms color cursor from an embedded resource of the
		/// entry assembly.</summary>
		public static Cursor FromEmbeddedResource(string rootNamespace, string path) {
			return FromEmbeddedResource(rootNamespace,
				Assembly.GetEntryAssembly(), path);
		}
	}
}
