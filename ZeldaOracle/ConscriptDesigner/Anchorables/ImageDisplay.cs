using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ConscriptDesigner.Content;
using ConscriptDesigner.Util;

namespace ConscriptDesigner.Anchorables {
	public class ImageDisplay : ContentFileDocument {
		
		/// <summary>The image to display the texture.</summary>
		private Image image;
		/// <summary>The scroll viewer for the image.</summary>
		private ScrollViewer scrollViewer;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Constructs the image display.</summary>
		public ImageDisplay(ContentImage file) :
			base(file)
		{
			Border border = CreateBorder();
			this.scrollViewer = new ScrollViewer();
			this.scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
			this.scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
			this.scrollViewer.Background = Brushes.White;
			border.Child = this.scrollViewer;

			BitmapSource source = BitmapFactory.LoadSourceFromFile(file.FilePath);
			this.image = new Image();
			this.image.Source = source;
			this.image.Width = source.PixelWidth;
			this.image.Height = source.PixelHeight;
			this.scrollViewer.Content = this.image;

			Title = file.Name;
			Content = border;
		}

		//-----------------------------------------------------------------------------
		// Actions
		//-----------------------------------------------------------------------------

		/// <summary>Reloads the image.</summary>
		public void Reload() {
			BitmapSource source = BitmapFactory.LoadSourceFromFile(File.FilePath);
			this.image.Source = source;
			this.image.Width = source.PixelWidth;
			this.image.Height = source.PixelHeight;
		}
	}
}
