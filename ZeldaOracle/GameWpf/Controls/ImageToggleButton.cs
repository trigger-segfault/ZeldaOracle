using System.ComponentModel;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace ZeldaWpf.Controls {
	/// <summary>A toggle button that also displays an image.</summary>
	public class ImageToggleButton : ToggleButton {

		/// <summary>The dependency property for the button's image.</summary>
		public static readonly DependencyProperty SourceProperty =
			DependencyProperty.RegisterAttached(
			"Source", typeof(ImageSource), typeof(ImageToggleButton));

		/// <summary>Gets or sets the source of the button's image.</summary>
		[Category("Common")]
		public ImageSource Source {
			get { return (ImageSource)GetValue(SourceProperty); }
			set { SetValue(SourceProperty, value); }
		}

		/// <summary>Initializes the image toggle button default style.</summary>
		static ImageToggleButton() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ImageToggleButton),
					   new FrameworkPropertyMetadata(typeof(ImageToggleButton)));
		}

		/// <summary>Constructs the image toggle button and sets the content to null.</summary>
		public ImageToggleButton() {
			Content = null;
		}

	}
}
