using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace ZeldaEditor.Controls {
	public class HoverImageButton : ImageButton {
		public static readonly DependencyProperty SourceHoverProperty =
			DependencyProperty.RegisterAttached(
			"SourceHover", typeof(ImageSource), typeof(ImageButton));

		public ImageSource SourceHover {
			get { return (ImageSource) GetValue(SourceHoverProperty); }
			set { SetValue(SourceHoverProperty, value); }
		}

		static HoverImageButton() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(HoverImageButton),
					   new FrameworkPropertyMetadata(typeof(HoverImageButton)));
		}
	}
}
