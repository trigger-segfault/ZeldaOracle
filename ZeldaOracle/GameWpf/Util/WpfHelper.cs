using System;
using System.Reflection;
using System.Windows;
using System.Windows.Media;

namespace ZeldaWpf.Util {
	/// <summary>A static class for WPF helper methods.</summary>
	public static class WpfHelper {

		/// <summary>Returns true if the parent contains the specified element.</summary>
		public static bool IsDescendant(DependencyObject parent,
			DependencyObject element)
		{
			if (element == null)
				return false;
			int count = VisualTreeHelper.GetChildrenCount(parent);
			for (int i = 0; i < count; i++) {
				var child = VisualTreeHelper.GetChild(parent, i);
				if (child == element || IsDescendant(child, element))
					return true;
			}
			return false;
		}

		/// <summary>Creates a pack Uri for loading resource images.</summary>
		public static Uri MakePackUri(string resourcePath, Assembly assembly = null) {
			assembly = assembly ?? Assembly.GetEntryAssembly();
			// Pull out the short name.
			string assemblyShortName = assembly.ToString().Split(',')[0];
			string uriString = "pack://application:,,,/" + assemblyShortName +
				";component/" + resourcePath;
			return new Uri(uriString);
		}

		/// <summary>A shorthand method for creating solid color brushes.</summary>
		public static SolidColorBrush ColorBrush(byte r, byte g, byte b,
			byte a = 255)
		{
			return new SolidColorBrush(Color.FromArgb(a, r, g, b));
		}

		/// <summary>A shorthand method for creating solid color pens.</summary>
		public static Pen ColorPen(byte r, byte g, byte b, byte a = 255,
			double thickness = 1d)
		{
			return new Pen(
				new SolidColorBrush(Color.FromArgb(a, r, g, b)).AsFrozen(),
				thickness);
		}
	}
}
