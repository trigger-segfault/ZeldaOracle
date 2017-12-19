using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ConscriptDesigner.Content;
using ConscriptDesigner.Util;
using Xceed.Wpf.AvalonDock.Layout;

namespace ConscriptDesigner.Anchorables {
	/// <summary>
	/// Interaction logic for ImageDisplay.xaml
	/// </summary>
	public partial class ImageDisplay : UserControl, IContentAnchorable {

		private RequestCloseDocument anchorable;
		private ContentImage file;

		public ImageDisplay(ContentImage file, RequestCloseDocument anchorable) {
			InitializeComponent();
			this.file = file;
			this.anchorable = anchorable;
			BitmapSource source = BitmapFactory.LoadSourceFromFile(file.FilePath);
			this.image.Source = source;
			this.image.Width = source.PixelWidth;
			this.image.Height = source.PixelHeight;
		}

		public RequestCloseDocument Anchorable {
			get { return anchorable; }
		}
		public ContentFile ContentFile {
			get { return file; }
		}

		public bool IsModified {
			get { return false; }
		}

		public void Save() { }

		public void Undo() { }
		public void Redo() { }

		public bool CanUndo {
			get { return false; }
		}

		public bool CanRedo {
			get { return false; }
		}
	}
}
