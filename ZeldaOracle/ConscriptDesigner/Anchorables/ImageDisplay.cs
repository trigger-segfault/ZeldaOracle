using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ConscriptDesigner.Content;
using ZeldaWpf.Util;

namespace ConscriptDesigner.Anchorables {
	public class ImageDisplay : ContentFileDocument {
		
		/// <summary>The image to display the texture.</summary>
		private Image image;
		/// <summary>The scroll viewer for the image.</summary>
		private ScrollViewer scrollViewer;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Constructs the image display for serialization.</summary>
		public ImageDisplay() {
			Border border = CreateBorder();
			this.scrollViewer = new ScrollViewer();
			this.scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
			this.scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
			this.scrollViewer.Background = Brushes.White;
			border.Child = this.scrollViewer;
			
			this.image = new Image();
			this.scrollViewer.Content = this.image;
			
			Content = border;
		}

		/// <summary>Constructs the image display.</summary>
		public ImageDisplay(ContentImage file) :
			this()
		{
			LoadFile(file);
		}


		//-----------------------------------------------------------------------------
		// Overrides
		//-----------------------------------------------------------------------------

		/// <summary>Completes setup after loading the content file.</summary>
		protected override void OnLoadFile(ContentFile loadFile) {
			ContentImage file = loadFile as ContentImage;
			if (file == null)
				Close();

			Reload();
			Title = file.Name;
		}

		/// <summary>Focuses on the anchorable's content.</summary>
		public override void Focus() {
			scrollViewer.Focus();
		}


		//-----------------------------------------------------------------------------
		// Actions
		//-----------------------------------------------------------------------------

		/// <summary>Reloads the image.</summary>
		public void Reload() {
			BitmapSource source = BitmapFactory.FromFile(File.FilePath);
			this.image.Source = source;
			this.image.Width = source.PixelWidth;
			this.image.Height = source.PixelHeight;
		}
	}
}
