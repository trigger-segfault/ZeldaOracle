using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ZeldaWpf.Controls {
	/// <summary>A dropdown button built from a menu.</summary>
	public class DropDownButton : Menu {
		
		/// <summary>The dependency property for the dropdown header.</summary>
		public static readonly DependencyProperty HeaderProperty =
			MenuItem.HeaderProperty.AddOwner(typeof(DropDownButton));

		/// <summary>Gets or sets the header of the dropdown button.</summary>
		[Category("Common")]
		public object Header {
			get { return (ImageSource)GetValue(HeaderProperty); }
			set { SetValue(HeaderProperty, value); }
		}

		/// <summary>Initializes the dropdown button default style.</summary>
		static DropDownButton() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(DropDownButton),
					   new FrameworkPropertyMetadata(typeof(DropDownButton)));
		}
	}
}
