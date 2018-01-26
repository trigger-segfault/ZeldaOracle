using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using ConscriptDesigner.Util;

namespace ConscriptDesigner.Control {
	public static class DesignerImages {
		public static readonly BitmapSource Merge			= LoadIcon("Merge");
		public static readonly BitmapSource Overwrite		= LoadIcon("Overwrite");
		public static readonly BitmapSource FolderOpen		= LoadIcon("FolderOpen");
		public static readonly BitmapSource FolderClosed	= LoadIcon("FolderClosed");
		public static readonly BitmapSource FolderAdd		= LoadIcon("FolderClosed");

		public static readonly BitmapSource FolderBlueOpen  = LoadIcon("FolderBlueOpen");
		public static readonly BitmapSource FolderBlueClosed= LoadIcon("FolderBlueClosed");
		public static readonly BitmapSource Rename			= LoadIcon("Rename");
		public static readonly BitmapSource Delete          = LoadIcon("Delete");
		public static readonly BitmapSource Plus			= LoadIcon("Plus");

		public static readonly BitmapSource ConscriptFile	= LoadIcon("ConscriptFile");
		public static readonly BitmapSource ConscriptFileAdd= LoadIcon("ConscriptFileAdd");
		public static readonly BitmapSource SpriteFontFile	= LoadIcon("SpriteFontFile");
		public static readonly BitmapSource Content         = LoadIcon("Content");
		public static readonly BitmapSource File			= LoadIcon("File");
		public static readonly BitmapSource FileAdd			= LoadIcon("FileAdd");
		public static readonly BitmapSource ContentFile		= LoadIcon("ContentFile");
		public static readonly BitmapSource ContentProject	= LoadIcon("ContentProject");
		public static readonly BitmapSource ContentProjectRemove= LoadIcon("ContentProjectRemove");
		public static readonly BitmapSource ContentAdd		= LoadIcon("ContentAdd");
		public static readonly BitmapSource ImageFile		= LoadIcon("ImageFile");
		public static readonly BitmapSource SoundFile		= LoadIcon("SoundFile");
		public static readonly BitmapSource ShaderFile		= LoadIcon("ShaderFile");

		public static readonly BitmapSource SelectAll		= LoadIcon("SelectAll");
		public static readonly BitmapSource Copy			= LoadIcon("Copy");
		public static readonly BitmapSource Cut				= LoadIcon("Cut");
		public static readonly BitmapSource Paste			= LoadIcon("Paste");

		public static readonly BitmapSource Open			= LoadIcon("Open");

		public static readonly BitmapSource MoveUp			= LoadIcon("MoveUp");
		public static readonly BitmapSource MoveDown		= LoadIcon("MoveDown");
		
		private static BitmapSource LoadIcon(string name) {
			return BitmapFactory.LoadSourceFromResource("Resources/Icons/" + name + ".png");
		}
		private static BitmapSource LoadResource(string name) {
			return BitmapFactory.LoadSourceFromResource("Resources/" + name + ".png");
		}
	}
}
