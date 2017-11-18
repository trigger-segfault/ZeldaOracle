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
	public class ImageToggleButton : ToggleButton {

		public static readonly DependencyProperty SourceProperty =
			DependencyProperty.RegisterAttached(
			"Source", typeof(ImageSource), typeof(ImageToggleButton));

		public ImageSource Source {
			get { return (ImageSource)GetValue(SourceProperty); }
			set { SetValue(SourceProperty, value); }
		}

		static ImageToggleButton() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ImageToggleButton),
					   new FrameworkPropertyMetadata(typeof(ImageToggleButton)));
		}
		public ImageToggleButton() {
			Content = null;
		}

	}
}
