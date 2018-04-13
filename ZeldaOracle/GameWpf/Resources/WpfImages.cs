using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using ZeldaWpf.Util;

namespace ZeldaWpf.Resources {
	/// <summary>The available colors for folder resources.</summary>
	public enum FolderColor {
		Manilla,
		Blue,
	}

	/// <summary>A static class for storing pre-loaded images.</summary>
	public class WpfImages {

		//-----------------------------------------------------------------------------
		// Attribute Classes
		//-----------------------------------------------------------------------------

		/// <summary>Specifies an image that is loaded with custom parameters.</summary>
		[AttributeUsage(AttributeTargets.Field)]
		protected class CustomImageAttribute : Attribute {
			/// <summary>The extension to load the image with.</summary>
			public string Extension { get; set; } = ".png";

			/// <summary>The path to load the image from.</summary>
			public string Path { get; set; } = "Resources/Icons/";

			/// <summary>True if the image should not be loaded automatically.</summary>
			public bool Manual { get; set; } = false;
		}


		//-----------------------------------------------------------------------------
		// Images
		//-----------------------------------------------------------------------------

		public static readonly BitmapImage Empty;
		public static readonly BitmapImage InfoIcon;
		public static readonly BitmapImage QuestionIcon;
		public static readonly BitmapImage WarningIcon;
		public static readonly BitmapImage ErrorIcon;

		public static readonly BitmapImage FolderManillaHOpen;
		public static readonly BitmapImage FolderManillaHClosed;
		public static readonly BitmapImage FolderManillaVOpen;
		public static readonly BitmapImage FolderManillaVClosed;

		public static readonly BitmapImage FolderBlueHOpen;
		public static readonly BitmapImage FolderBlueHClosed;
		public static readonly BitmapImage FolderBlueVOpen;
		public static readonly BitmapImage FolderBlueVClosed;


		//-----------------------------------------------------------------------------
		// Internal Members
		//-----------------------------------------------------------------------------
		
		/// <summary>The collection of folders for each folder color.</summary>
		private static Dictionary<FolderColor, BitmapImage[]> folders;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Initializes the Wpf images.</summary>
		static WpfImages() {
			folders = new Dictionary<FolderColor, BitmapImage[]>();
			LoadImages(typeof(WpfImages));
		}


		//-----------------------------------------------------------------------------
		// Folders
		//-----------------------------------------------------------------------------

		/// <summary>Gets the folder with the specified settings.</summary>
		public static BitmapImage GetFolder(FolderColor color, Orientation orientation,
			bool isOpen)
		{
			BitmapImage[] folderColor;
			if (!folders.TryGetValue(color, out folderColor)) {
				Type type = typeof(WpfImages);
				folderColor = new BitmapImage[4];
				folderColor[0] = (BitmapImage) type.GetField(
					"Folder" + color.ToString() + "HClosed").GetValue(null);
				folderColor[1] = (BitmapImage) type.GetField(
					"Folder" + color.ToString() + "HOpen").GetValue(null);
				folderColor[2] = (BitmapImage) type.GetField(
					"Folder" + color.ToString() + "VClosed").GetValue(null);
				folderColor[3] = (BitmapImage) type.GetField(
					"Folder" + color.ToString() + "VOpen").GetValue(null);
				folders.Add(color, folderColor);
			}
			int index = 0;
			if (orientation == Orientation.Vertical)
				index += 2;
			if (isOpen)
				index++;
			return folderColor[index];
		}


		//-----------------------------------------------------------------------------
		// Reflection Loading
		//-----------------------------------------------------------------------------

		/// <summary>Loads all images from fields using the names and attributes of
		/// the field.</summary> 
		protected static void LoadImages(Type ownerType) {
			Assembly assembly = ownerType.Assembly;

			Type imageType = typeof(BitmapImage);

			// Look for all static fields to assign to
			foreach (FieldInfo fieldInfo in ownerType.GetFields(
				BindingFlags.Static | BindingFlags.Public))
			{
				// Is this field assignable?
				if (fieldInfo.FieldType.IsAssignableFrom(imageType)) {
					CustomImageAttribute attr =
						fieldInfo.GetCustomAttribute<CustomImageAttribute>() ??
						new CustomImageAttribute();
					if (attr.Manual)
						continue;
					BitmapImage image = LoadImage(fieldInfo.Name, assembly, attr);
					fieldInfo.SetValue(null, image);
				}
			}
		}
		

		//-----------------------------------------------------------------------------
		// Loading
		//-----------------------------------------------------------------------------
		
		/// <summary>Loads a .png icon with the specified name.</summary>
		protected static BitmapImage LoadIcon(string name, Assembly assembly) {
			assembly = assembly ?? Assembly.GetEntryAssembly();
			return BitmapFactory.FromResource("Resources/Icons/" + name + ".png",
				assembly).AsFrozen();
		}

		/// <summary>Loads a .png image with the specified name.</summary>
		protected static BitmapImage LoadImage(string name, Assembly assembly,
			CustomImageAttribute info)
		{
			return BitmapFactory.FromResource(info.Path + name + info.Extension,
				assembly).AsFrozen();
		}
	}
}
