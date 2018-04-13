using System.Windows.Media.Imaging;
using ZeldaWpf.Resources;

namespace ConscriptDesigner.Control {
	/// <summary>A static class for storing pre-loaded images.</summary>
	public class DesignerImages : WpfImages {

		//-----------------------------------------------------------------------------
		// Images
		//-----------------------------------------------------------------------------

		public static readonly BitmapImage Merge;
		public static readonly BitmapImage Overwrite;
		public static readonly BitmapImage FolderOpen;
		public static readonly BitmapImage FolderClosed;
		public static readonly BitmapImage FolderAdd;

		public static readonly BitmapImage Rename;
		public static readonly BitmapImage Delete;
		public static readonly BitmapImage Plus;

		public static readonly BitmapImage ConscriptFile;
		public static readonly BitmapImage ConscriptFileAdd;
		public static readonly BitmapImage SpriteFontFile;
		public static readonly BitmapImage Content;
		public static readonly BitmapImage File;
		public static readonly BitmapImage FileAdd;
		public static readonly BitmapImage ContentFile;
		public static readonly BitmapImage ContentProject;
		public static readonly BitmapImage ContentProjectRemove;
		public static readonly BitmapImage ContentAdd;
		public static readonly BitmapImage ImageFile;
		public static readonly BitmapImage SoundFile;
		public static readonly BitmapImage ShaderFile;

		public static readonly BitmapImage SelectAll;
		public static readonly BitmapImage Copy;
		public static readonly BitmapImage Cut;
		public static readonly BitmapImage Paste;

		public static readonly BitmapImage Open;

		public static readonly BitmapImage MoveUp;
		public static readonly BitmapImage MoveDown;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Initializes the Designer images.</summary>
		static DesignerImages() {
			LoadImages(typeof(DesignerImages));
		}
	}
}
