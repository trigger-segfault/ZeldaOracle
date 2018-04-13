using System;
using System.IO;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Bitmap = System.Drawing.Bitmap;
using GdiImage = System.Drawing.Image;
using Graphics = System.Drawing.Graphics;
using Gdi = System.Drawing;
using System.Reflection;
using Rectangle = System.Drawing.Rectangle;
using InterpolationMode = System.Drawing.Drawing2D.InterpolationMode;
using ZeldaOracle.Common.Geometry;
using GdiPixelFormat = System.Drawing.Imaging.PixelFormat;

namespace ZeldaWpf.Util {
	/// <summary>A static helper class for creating Wpf bitmaps.</summary>
	public static class BitmapFactory {

		//-----------------------------------------------------------------------------
		// Constants
		//-----------------------------------------------------------------------------

		/// <summary>The dpi to use for all created bitmaps.</summary>
		public const int Dpi = 96;


		//-----------------------------------------------------------------------------
		// Gdi Bitmap
		//-----------------------------------------------------------------------------

		public static Bitmap CreateBitmapFromLockedWriteable(WriteableBitmap writeable,
			bool alpha = true)
		{
			GdiPixelFormat format = (alpha ?
				GdiPixelFormat.Format32bppArgb :
				GdiPixelFormat.Format24bppRgb);
			return new Bitmap(writeable.PixelWidth, writeable.PixelHeight,
				writeable.BackBufferStride, format, writeable.BackBuffer);
		}

		public static BitmapSource ConvertBitmapToSource(Bitmap bitmap) {
			IntPtr handle = bitmap.GetHbitmap();
			try {
				return Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(),
					IntPtr.Zero, Int32Rect.Empty,
					BitmapSizeOptions.FromEmptyOptions());
			}
			finally { WpfNativeMethods.DeleteObject(handle); }
		}

		public static WriteableBitmap ConverBitmapToWriteable(Bitmap bitmap) {
			return new WriteableBitmap(ConvertBitmapToSource(bitmap));
		}
		

		//-----------------------------------------------------------------------------
		// Bitmap Image Loading
		//-----------------------------------------------------------------------------

		/// <summary>Loads a bitmap from the specified resource path and assembly.</summary>
		public static BitmapImage FromResource(string resourcePath,
			Assembly assembly = null)
		{
			BitmapImage bitmapImage = new BitmapImage();
			bitmapImage.BeginInit();
			bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
			bitmapImage.UriSource = WpfHelper.MakePackUri(resourcePath, assembly);
			bitmapImage.EndInit();
			return bitmapImage;
		}
		
		/// <summary>Loads a bitmap from the specified resource path and type's
		/// assembly.</summary>
		public static BitmapImage FromResource(string resourcePath,
			Type assemblyType)
		{
			return FromResource(resourcePath, assemblyType.Assembly);
		}

		/// <summary>Loads a bitmap from the specified stream.</summary>
		public static BitmapImage FromStream(Stream stream) {
			BitmapImage bitmapImage = new BitmapImage();
			bitmapImage.BeginInit();
			bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
			bitmapImage.StreamSource = stream;
			bitmapImage.EndInit();
			return bitmapImage;
		}

		/// <summary>Loads a bitmap from the specified file path.</summary>
		public static BitmapImage FromFile(string filePath) {
			using (Stream stream = File.OpenRead(filePath)) {
				return FromStream(stream);
			}
		}


		//-----------------------------------------------------------------------------
		// Writeable Bitmap
		//-----------------------------------------------------------------------------

		/// <summary>Creates a writeable bitmap from the specified size and format.</summary>
		public static WriteableBitmap CreateWriteable(int width, int height,
			bool alpha = true)
		{
			return CreateWriteable(new Point2I(width, height), alpha);
		}

		/// <summary>Creates a writeable bitmap from the specified size and format.</summary>
		public static WriteableBitmap CreateWriteable(Point2I size,
			bool alpha = true)
		{
			PixelFormat format = (alpha ? PixelFormats.Bgra32 : PixelFormats.Bgr24);
			return CreateWriteable(size, format);
		}

		/// <summary>Creates a writeable bitmap from the specified size and format.</summary>
		public static WriteableBitmap CreateWriteable(int width, int height,
			PixelFormat format)
		{
			return CreateWriteable(new Point2I(width, height), format);
		}

		/// <summary>Creates a writeable bitmap from the specified size and format.</summary>
		public static WriteableBitmap CreateWriteable(Point2I size,
			PixelFormat format)
		{
			return new WriteableBitmap(size.X, size.Y, Dpi, Dpi, format, null);
		}


		//-----------------------------------------------------------------------------
		// Extensions
		//-----------------------------------------------------------------------------

		/// <summary>Creates Gdi Graphics from a Gdi bitmap.</summary>
		public static Graphics CreateGraphics(this Bitmap bitmap) {
			Graphics g = Graphics.FromImage(bitmap);
			g.InterpolationMode = InterpolationMode.NearestNeighbor;
			return g;
		}

		/// <summary>Gets the pixel size of the Gdi image.</summary>
		public static Point2I Size(this GdiImage image) {
			return new Point2I(image.Width, image.Height);
		}

		/// <summary>Gets the pixel size of the Wpf bitmap.</summary>
		public static Point2I Size(this BitmapSource bitmap) {
			return new Point2I(bitmap.PixelWidth, bitmap.PixelHeight);
		}
	}
}
