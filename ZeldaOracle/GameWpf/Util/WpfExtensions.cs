using System.Windows;
using System.Windows.Media;

namespace ZeldaWpf.Util {
	/// <summary>A static class for Wpf control extensions.</summary>
	public static class WpfExtensions {

		//-----------------------------------------------------------------------------
		// Visibility
		//-----------------------------------------------------------------------------

		/// <summary>Gets if the element's visibility is visible.</summary>
		public static bool GetVisible(this UIElement element) {
			return element.Visibility == Visibility.Visible;
		}

		/// <summary>Sets the element's visibility to visible.</summary>
		public static void SetVisible(this UIElement element) {
			element.Visibility = Visibility.Visible;
		}

		/// <summary>Sets the element's visibility to hidden.</summary>
		public static void SetHidden(this UIElement element) {
			element.Visibility = Visibility.Hidden;
		}

		/// <summary>Sets the element's visibility to collapsed.</summary>
		public static void SetCollapsed(this UIElement element) {
			element.Visibility = Visibility.Collapsed;
		}


		//-----------------------------------------------------------------------------
		// Brushes
		//-----------------------------------------------------------------------------

		/// <summary>Returns the freezable object as a frozen version with the same
		/// type.</summary>
		public static TFreezable AsFrozen<TFreezable>(this TFreezable freezable)
			where TFreezable : Freezable
		{
			return (TFreezable) freezable.GetAsFrozen();
		}
	}
}
