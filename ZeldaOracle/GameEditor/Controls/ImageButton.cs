using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ZeldaEditor.Controls {
	public class ImageButton : Button {
		
		public static readonly DependencyProperty SourceProperty =
			DependencyProperty.RegisterAttached(
			"Source", typeof(ImageSource), typeof(ImageButton));

		public ImageSource Source {
			get { return (ImageSource)GetValue(SourceProperty); }
			set { SetValue(SourceProperty, value); }
		}

		static ImageButton() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ImageButton),
					   new FrameworkPropertyMetadata(typeof(ImageButton)));
		}
		public ImageButton() {
			Content = null;
		}

	}
}
