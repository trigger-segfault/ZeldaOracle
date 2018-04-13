using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ZeldaWpf.Controls {
	/// <summary>A button that also displays an image.</summary>
	public class ImageButton : Button {

		/// <summary>The dependency property for the button's image.</summary>
		public static readonly DependencyProperty SourceProperty =
			DependencyProperty.RegisterAttached("Source", typeof(ImageSource),
				typeof(ImageButton));

		/// <summary>Gets or sets the source of the button's image.</summary>
		[Category("Common")]
		public ImageSource Source {
			get { return (ImageSource)GetValue(SourceProperty); }
			set { SetValue(SourceProperty, value); }
		}

		/// <summary>Initializes the image button default style.</summary>
		static ImageButton() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ImageButton),
					   new FrameworkPropertyMetadata(typeof(ImageButton)));
		}

		/// <summary>Constructs the image button and sets the content to null.</summary>
		public ImageButton() {
			Content = null;
		}
	}
}
