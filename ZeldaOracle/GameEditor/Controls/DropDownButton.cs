using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace ZeldaEditor.Controls {
	public class DropDownButton : Menu {


		/**<summary>The dependency property for RCT remap colors.</summary>*/
		public static readonly DependencyProperty HeaderProperty =
			MenuItem.HeaderProperty.AddOwner(typeof(DropDownButton));
		
		public object Header {
			get { return (ImageSource)GetValue(HeaderProperty); }
			set { SetValue(HeaderProperty, value); }
		}

		static DropDownButton() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(DropDownButton),
					   new FrameworkPropertyMetadata(typeof(DropDownButton)));
		}

	}
}
