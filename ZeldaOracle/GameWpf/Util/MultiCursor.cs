using System;
using System.IO;
using System.Reflection;
using System.Resources;
using WinFormsCursor = System.Windows.Forms.Cursor;
using WpfCursor = System.Windows.Input.Cursor;

namespace ZeldaWpf.Util {
	/// <summary>A cursor that can be used in both WinForms and Wpf controls.</summary>
	public class MultiCursor {

		/// <summary>The WinForms version of the cursor.</summary>
		private WinFormsCursor winFormsCursor;
		/// <summary>The Wpf version of the cursor.</summary>
		private WpfCursor wpfCursor;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		/// <summary>Constructs the multi cursor from the WinForms cursors.</summary>
		public MultiCursor(WinFormsCursor winForms) {
			winFormsCursor = winForms;
		}

		/// <summary>Constructs the multi cursor from the Wpf cursor.</summary>
		public MultiCursor(WpfCursor wpf) {
			wpfCursor = wpf;
		}

		/// <summary>Constructs the multi cursor from the specified cursors.</summary>
		public MultiCursor(WinFormsCursor winForms, WpfCursor wpf) {
			winFormsCursor = winForms;
			wpfCursor = wpf;
		}


		//-----------------------------------------------------------------------------
		// Static Loaders
		//-----------------------------------------------------------------------------

		/// <summary>Loads a multi cursor from the specified resource path and
		/// assembly.</summary>
		public static MultiCursor FromResource(string path, string rootNamespace,
			Assembly assembly = null)
		{
			assembly = assembly ?? Assembly.GetEntryAssembly();
			ResourceManager rm = new ResourceManager(rootNamespace + ".g", assembly);
			Stream stream = (Stream) rm.GetObject(path.ToLower());
			stream.Position = 0;
			return FromStream(stream);
		}

		/// <summary>Loads a multi cursor from the specified resource path and
		/// type assembly.</summary>
		public static MultiCursor FromResource(string path, string rootNamespace,
			Type typeAssembly)
		{
			return FromResource(path, rootNamespace, typeAssembly.Assembly);
		}

		/// <summary>Loads a multi cursor from the specified stream.</summary>
		public static MultiCursor FromStream(Stream stream) {
			long startPosition = stream.Position;
			WinFormsCursor winForms = WinFormsCursorLoader.FromStream(stream);
			stream.Position = startPosition;
			WpfCursor wpf = new WpfCursor(stream);
			return new MultiCursor(winForms, wpf);
		}

		/// <summary>Loads a multi cursor from the specified file path.</summary>
		public static MultiCursor FromFile(string filePath) {
			WinFormsCursor winForms = WinFormsCursorLoader.FromFile(filePath);
			WpfCursor wpf = new WpfCursor(filePath);
			return new MultiCursor(winForms, wpf);
		}


		//-----------------------------------------------------------------------------
		// Casting
		//-----------------------------------------------------------------------------

		/// <summary>Casts the multi cursor to its WinForms cursor.</summary>
		public static implicit operator WinFormsCursor(MultiCursor cursor) {
			return cursor.winFormsCursor;
		}

		/// <summary>Casts the multi cursor to its Wpf cursor.</summary>
		public static implicit operator WpfCursor(MultiCursor cursor) {
			return cursor.wpfCursor;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets the WinForms version of the cursor.</summary>
		public WinFormsCursor WinFormsCursor {
			get { return winFormsCursor; }
		}

		/// <summary>Gets the Wpf version of the cursor.</summary>
		public WpfCursor WpfCursor {
			get { return wpfCursor; }
		}
	}
}
