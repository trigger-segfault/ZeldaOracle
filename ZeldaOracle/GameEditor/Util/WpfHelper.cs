using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace ZeldaEditor.Util {
	/// <summary>A static class for WPF helper methods.</summary>
	public static class WpfHelper {

		/// <summary>Returns true if the parent contains the specified element.</summary>
		public static bool IsDescendant(DependencyObject parent, DependencyObject element) {
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
	}
}
