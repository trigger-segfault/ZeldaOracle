using System;
using System.Reflection;
using ZeldaWpf.Util;

namespace ZeldaWpf.Resources {
	/// <summary>A static class for storing pre-loaded cursors.</summary>
	public class WpfCursors {

		//-----------------------------------------------------------------------------
		// Attribute Classes
		//-----------------------------------------------------------------------------

		/// <summary>Specifies a cursor that is loaded with custom parameters.</summary>
		[AttributeUsage(AttributeTargets.Field)]
		protected class CustomCursorAttribute : Attribute {
			/// <summary>The postfix to load the cursor with.</summary>
			public string Postfix { get; set; } = "Cursor";

			/// <summary>The path to load the cursor from.</summary>
			public string Path { get; set; } = "Resources/Cursors/";

			/// <summary>True if the cursor should not be loaded automatically.</summary>
			public bool Manual { get; set; } = false;
		}


		//-----------------------------------------------------------------------------
		// Cursors
		//-----------------------------------------------------------------------------

		public static readonly MultiCursor Eraser;
		public static readonly MultiCursor Eyedropper;
		public static readonly MultiCursor Fill;
		public static readonly MultiCursor HandClosed;
		public static readonly MultiCursor HandOpen;
		public static readonly MultiCursor Pencil;
		public static readonly MultiCursor Selection;
		public static readonly MultiCursor Square;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Initializes the Wpf cursors.</summary>
		static WpfCursors() {
			LoadCursors(typeof(WpfCursors), nameof(ZeldaWpf));
		}


		//-----------------------------------------------------------------------------
		// Reflection Loading
		//-----------------------------------------------------------------------------

		/// <summary>Loads all images from fields using the names and attributes of
		/// the field.</summary> 
		protected static void LoadCursors(Type ownerType, string rootNamespace) {
			Assembly assembly = ownerType.Assembly;
			
			Type cursorType = typeof(MultiCursor);

			// Look for all static fields to assign to
			foreach (FieldInfo fieldInfo in ownerType.GetFields(
				BindingFlags.Static | BindingFlags.Public)) {
				// Is this field assignable?
				if (fieldInfo.FieldType.IsAssignableFrom(cursorType)) {
					CustomCursorAttribute attr =
						fieldInfo.GetCustomAttribute<CustomCursorAttribute>() ??
						new CustomCursorAttribute();
					if (attr.Manual)
						continue;
					MultiCursor cursor = LoadCursor(fieldInfo.Name, rootNamespace,
						assembly, attr);
					fieldInfo.SetValue(null, cursor);
				}
			}
		}


		//-----------------------------------------------------------------------------
		// Loading
		//-----------------------------------------------------------------------------

		/// <summary>Loads a cursor with the specified name.</summary>
		protected static MultiCursor LoadCursor(string name,
			string rootNamespace, Assembly assembly, CustomCursorAttribute info)
		{
			return MultiCursor.FromResource(info.Path + name + info.Postfix + ".cur",
				rootNamespace, assembly);
		}
	}
}
