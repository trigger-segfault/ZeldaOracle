using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace ZeldaWpf.Controls {
	/// <summary>An image button that displays a differnt image on hover.</summary>
	public class HoverImageButton : ImageButton {

		/// <summary>The dependency property for the button's hover image.</summary>
		public static readonly DependencyProperty SourceHoverProperty =
			DependencyProperty.RegisterAttached(
			"SourceHover", typeof(ImageSource), typeof(ImageButton));

		/// <summary>Gets or sets the hover source of the button's image.</summary>
		[Category("Common")]
		public ImageSource SourceHover {
			get { return (ImageSource) GetValue(SourceHoverProperty); }
			set { SetValue(SourceHoverProperty, value); }
		}

		/// <summary>Initializes the hover image button default style.</summary>
		static HoverImageButton() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(HoverImageButton),
					   new FrameworkPropertyMetadata(typeof(HoverImageButton)));
		}
	}
}
