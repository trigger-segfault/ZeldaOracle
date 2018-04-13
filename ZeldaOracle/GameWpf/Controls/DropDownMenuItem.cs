using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace ZeldaWpf.Controls {
	/// <summary>A menu item for a dropdown button.</summary>
	public class DropDownMenuItem : MenuItem {

		private Popup part_popup;

		public object GetPopupTemplateChild(string childName) {
			return base.GetTemplateChild(childName);
		}

		public void Close() {
			part_popup.IsOpen = false;
			IsHighlighted = false;
			UIElement parent = Parent as UIElement;
			parent.RaiseEvent(
			  new MouseButtonEventArgs(
				 Mouse.PrimaryDevice, 0, MouseButton.Left
			  ) { RoutedEvent=Mouse.MouseUpEvent }
		   );
		}

		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			part_popup = GetTemplateChild("PART_Popup") as Popup;
		}
	}
}
