using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Bitmap = System.Drawing.Bitmap;
using Graphics = System.Drawing.Graphics;
using GDI = System.Drawing;
using System.Reflection;
using Rectangle = System.Drawing.Rectangle;
using InterpolationMode = System.Drawing.Drawing2D.InterpolationMode;

namespace ZeldaEditor.Util {
	public static class BitmapFactory {

		public static Bitmap CreateBitmapFromLockedWriteable(WriteableBitmap writeable, bool alpha = true) {
			return new Bitmap(writeable.PixelWidth, writeable.PixelHeight, writeable.BackBufferStride,
				alpha ? GDI.Imaging.PixelFormat.Format32bppArgb : GDI.Imaging.PixelFormat.Format24bppRgb, writeable.BackBuffer);
		}
		public static BitmapSource ConvertBitmapToSource(Bitmap bitmap) {
			IntPtr handle = bitmap.GetHbitmap();
			try {
				return Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
			}
			finally { NativeMethods.DeleteObject(handle); }
		}
		public static Graphics CreateGraphics(Bitmap bitmap) {
			Graphics g = Graphics.FromImage(bitmap);
			g.InterpolationMode = InterpolationMode.NearestNeighbor;
			return g;
		}
		public static BitmapSource LoadSourceFromResource(string resourcePath, Assembly assembly = null) {
			if (assembly == null) {
				assembly = Assembly.GetExecutingAssembly();
			}
			// Pull out the short name.
			string assemblyShortName = assembly.ToString().Split(',')[0];
			BitmapImage bitmapImage = new BitmapImage();
			bitmapImage.BeginInit();
			bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
			bitmapImage.UriSource = MakePackUri(resourcePath, assemblyShortName);
			bitmapImage.EndInit();
			return bitmapImage;
		}
		public static BitmapSource LoadSourceFromStream(Stream stream) {
			BitmapImage bitmapImage = new BitmapImage();
			bitmapImage.BeginInit();
			bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
			bitmapImage.StreamSource = stream;
			bitmapImage.EndInit();
			return bitmapImage;
		}
		private static Uri MakePackUri(string resourcePath, string assemblyShortName) {
			string uriString = "pack://application:,,,/" + assemblyShortName + ";component/" + resourcePath;
			return new Uri(uriString);
		}
		public static WriteableBitmap ConverBitmapToWriteable(Bitmap bitmap) {
			return new WriteableBitmap(ConvertBitmapToSource(bitmap));
		}
		public static WriteableBitmap CreateWriteable(int width, int height) {
			return CreateWriteable(width, height, PixelFormats.Bgra32);
		}
		public static WriteableBitmap CreateWriteable(int width, int height, PixelFormat format) {
			return new WriteableBitmap(width, height, 96, 96, format, null);
		}

		// TODO: FromFile
	}
}
