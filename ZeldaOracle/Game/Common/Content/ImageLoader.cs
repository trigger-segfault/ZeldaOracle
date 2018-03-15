using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Rectangle = System.Drawing.Rectangle;
using Bitmap = System.Drawing.Bitmap;
using BitmapData = System.Drawing.Imaging.BitmapData;
using ImageLockMode = System.Drawing.Imaging.ImageLockMode;
using PixelFormat = System.Drawing.Imaging.PixelFormat;
using ZeldaOracle.Common.Graphics;
using XnaColor = Microsoft.Xna.Framework.Color;

namespace ZeldaOracle.Common.Content {
	/// <summary>A specialized Texture2D loader used to fix
	/// Texture2D.FromStream's broken nature.</summary>
	public static class ImageLoader {

		/// <summary>Loads the texture from the stream with the specified file size.</summary>
		public static Image FromFile(string filePath) {
			using (FileStream stream = File.Open(filePath, FileMode.Open))
				return FromStream(stream);
		}

		/// <summary>Loads the texture from the stream with the specified file size.</summary>
		public static Image FromStream(Stream stream, int fileSize) {
			BinaryReader reader = new BinaryReader(stream);
			using (Stream memory = new MemoryStream(reader.ReadBytes(fileSize)))
				return FromStream(memory);
		}

		/// <summary>Loads the texture from the stream.</summary>
		public static unsafe Image FromStream(Stream stream) {
			// Load through GDI Bitmap because it doesn't cause issues with alpha
			using (Bitmap bitmap = (Bitmap) Bitmap.FromStream(stream)) {
				// Create a texture and array to output the bitmap to
				Image image = new Image(Resources.GraphicsDevice,
					bitmap.Width, bitmap.Height, SurfaceFormat.Color);
				//Texture2D texture = new Texture2D(Resources.GraphicsDevice,
				//	bitmap.Width, bitmap.Height, false, SurfaceFormat.Color);
				XnaColor[] data = new XnaColor[bitmap.Width * bitmap.Height];

				// Get the pixels from the bitmap
				Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
				BitmapData bmpData = bitmap.LockBits(rect, ImageLockMode.ReadOnly,
					PixelFormat.Format32bppArgb);

				// Write the pixels to the data buffer
				byte* ptr = (byte*) bmpData.Scan0;
				for (int i = 0; i < data.Length; i++) {
					// Go through every color and reverse red and blue channels
					data[i] = new XnaColor(ptr[2], ptr[1], ptr[0], ptr[3]);
					ptr += 4;
				}

				bitmap.UnlockBits(bmpData);

				// Assign the data to the texture
				image.Texture.SetData<XnaColor>(data);

				// Fun fact: All this extra work is actually 50% faster than
				// Texture2D.FromStream! It's not only broken, but slow as well.

				return image;
			}
		}
	}
}
