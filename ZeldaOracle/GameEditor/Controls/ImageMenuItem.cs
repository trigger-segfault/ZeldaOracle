using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ZeldaEditor.Controls {
	public class ImageMenuItem : MenuItem {
		
		public static readonly DependencyProperty SourceProperty =
			DependencyProperty.RegisterAttached(
			"Source", typeof(ImageSource), typeof(ImageMenuItem),
				new FrameworkPropertyMetadata(OnSourceChanged));
		
		public ImageSource Source {
			get { return (ImageSource)GetValue(SourceProperty); }
			set { SetValue(SourceProperty, value); }
		}

		private static void OnSourceChanged(object sender, DependencyPropertyChangedEventArgs e) {
			ImageMenuItem element = sender as ImageMenuItem;
			if (element != null) {
				element.image.Source = element.Source;
			}
		}
		private static void OnIconChanged(object sender, DependencyPropertyChangedEventArgs e) {
			ImageMenuItem element = sender as ImageMenuItem;
			if (element != null && element.Icon != element.image) {
				element.Icon = element.image;
			}
		}

		private Image image;

		static ImageMenuItem() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ImageMenuItem),
					   new FrameworkPropertyMetadata(typeof(ImageMenuItem)));
			IconProperty.OverrideMetadata(typeof(ImageMenuItem),
				new FrameworkPropertyMetadata(OnIconChanged));
		}

		public ImageMenuItem() {
			image = new Image();
			image.Stretch = Stretch.None;
			Icon = image;
		}
		public ImageMenuItem(ImageSource source, string name) {
			image = new Image();
			image.Stretch = Stretch.None;
			image.Source = source;
			Icon = image;
			Header = name;
		}
	}
}
