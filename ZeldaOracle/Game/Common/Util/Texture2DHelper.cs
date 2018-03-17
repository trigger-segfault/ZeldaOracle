using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Rectangle = System.Drawing.Rectangle;
using Bitmap = System.Drawing.Bitmap;
using BitmapData = System.Drawing.Imaging.BitmapData;
using ImageLockMode = System.Drawing.Imaging.ImageLockMode;
using PixelFormat = System.Drawing.Imaging.PixelFormat;
using XnaColor = Microsoft.Xna.Framework.Color;
using ZeldaOracle.Common.Content;

namespace ZeldaOracle.Common.Util {
	/// <summary>A static helper class for Texture2D functions.</summary>
	public static class Texture2DHelper {

		//-----------------------------------------------------------------------------
		// Constants
		//-----------------------------------------------------------------------------

		/// <summary>The blend state for writing the RGB during alpha premultiplying.</summary>
		private static readonly BlendState BlendColorBlendState = new BlendState {
			ColorDestinationBlend = Blend.Zero,
			ColorWriteChannels = ColorWriteChannels.Red | ColorWriteChannels.Green |
				ColorWriteChannels.Blue,
			AlphaDestinationBlend = Blend.Zero,
			AlphaSourceBlend = Blend.SourceAlpha,
			ColorSourceBlend = Blend.SourceAlpha
		};

		/// <summary>The blend state for writing the alpha during alpha premultiplying.</summary>
		private static readonly BlendState BlendAlphaBlendState = new BlendState {
			ColorWriteChannels = ColorWriteChannels.Alpha,
			AlphaDestinationBlend = Blend.Zero,
			ColorDestinationBlend = Blend.Zero,
			AlphaSourceBlend = Blend.One,
			ColorSourceBlend = Blend.One
		};


		//-----------------------------------------------------------------------------
		// Static Methods
		//-----------------------------------------------------------------------------

		/// <summary>Writes pixel data from an XNA Texture2D to a GDI Bitmap.</summary>
		/// <exception cref="ArgumentNullException">
		/// Bitmap or texture are null.</exception>
		/// <exception cref="ArgumentException">
		/// Bitmap and texture dimensions do not match.-or-
		/// Bitmap or texture formats are not 32-bit RGBA or 24-bit RGB.</exception>
		public static unsafe void WriteBitmapToTexture2D(Bitmap bitmap,
			Texture2D texture)
		{
			// Safety checks
			if (bitmap == null)
				throw new ArgumentNullException("Bitmap cannot be null!");
			if (texture == null)
				throw new ArgumentNullException("Texture cannot be null!");
			if (texture.Width != bitmap.Width || texture.Height != bitmap.Height)
				throw new ArgumentException("Texture dimensions do not match " +
					"bitmap dimensions!");
			if (bitmap.PixelFormat != PixelFormat.Format32bppArgb &&
				bitmap.PixelFormat != PixelFormat.Format24bppRgb)
				throw new ArgumentException("Bitmap must use Format32bppArgb or " +
					"Format24bppRgb PixelFormat!");
			if (texture.Format != SurfaceFormat.Color)
				throw new ArgumentException("Texture must use Color SurfaceFormat!");

			// Create the data to write to the texture
			XnaColor[] data = new XnaColor[bitmap.Width * bitmap.Height];

			// Get the pixels from the bitmap
			Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
			BitmapData bmpData = bitmap.LockBits(rect, ImageLockMode.ReadOnly,
					PixelFormat.Format32bppArgb);

			// Write the pixels to the data buffer
			byte* ptr = (byte*) bmpData.Scan0;
			if (bitmap.PixelFormat == PixelFormat.Format32bppArgb) {
				for (int i = 0; i < data.Length; i++) {
					// Go through every color and reverse red and blue channels
					data[i] = new XnaColor(ptr[2], ptr[1], ptr[0], ptr[3]);
					ptr += 4;
				}
			}
			else {
				for (int i = 0; i < data.Length; i++) {
					// Go through every color and reverse red and blue channels
					data[i] = new XnaColor(ptr[2], ptr[1], ptr[0]);
					ptr += 3;
				}
			}
			
			bitmap.UnlockBits(bmpData);

			// Assign the data to the texture
			texture.SetData<XnaColor>(data);

			// Fun fact: All this extra work is actually 50% faster than
			// Texture2D.FromStream! It's not only broken, but slow as well.
		}

		/// <summary>Writes pixel data from a GDI Bitmap to an XNA Texture2D.</summary>
		/// <exception cref="ArgumentNullException">
		/// Texture or bitmap are null.</exception>
		/// <exception cref="ArgumentException">
		/// Texture and bitmap dimensions do not match.-or-
		/// Texture or bitmap formats are not 32-bit RGBA or 24-bit RGB.</exception>
		public static unsafe void WriteTexture2DToBitmap(Texture2D texture,
			Bitmap bitmap)
		{
			// Safety checks
			if (texture == null)
				throw new ArgumentNullException("Texture cannot be null!");
			if (bitmap == null)
				throw new ArgumentNullException("Bitmap cannot be null!");
			if (texture.Width != bitmap.Width || texture.Height != bitmap.Height)
				throw new ArgumentException("Bitmap dimensions do not match " +
					"texture dimensions!");
			if (texture.Format != SurfaceFormat.Color)
				throw new ArgumentException("Texture must use Color SurfaceFormat!");
			if (bitmap.PixelFormat != PixelFormat.Format32bppArgb &&
				bitmap.PixelFormat != PixelFormat.Format24bppRgb)
				throw new ArgumentException("Bitmap must use Format32bppArgb or " +
					"Format24bppRgb PixelFormat!");

			// Get the data from the texture
			XnaColor[] data = new XnaColor[texture.Width * texture.Height];
			texture.GetData<XnaColor>(data);

			// Create the pixels to write to the bitmap
			Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
			BitmapData bmpData = bitmap.LockBits(rect, ImageLockMode.ReadOnly,
					PixelFormat.Format32bppArgb);

			// Write the data to the pixel buffer
			byte* ptr = (byte*) bmpData.Scan0;
			if (bitmap.PixelFormat == PixelFormat.Format32bppArgb) {
				for (int i = 0; i < data.Length; i++) {
					// Go through every color and reverse red and blue channels
					XnaColor color = data[i];
					ptr[2] = color.R;
					ptr[1] = color.G;
					ptr[0] = color.B;
					ptr[3] = color.A;
					ptr += 4;
				}
			}
			else {
				for (int i = 0; i < data.Length; i++) {
					// Go through every color and reverse red and blue channels
					XnaColor color = data[i];
					ptr[2] = color.R;
					ptr[1] = color.G;
					ptr[0] = color.B;
					ptr += 3;
				}
			}

			bitmap.UnlockBits(bmpData);
		}

		/// <summary>Loads a Texture2D or RenderTarget2D from a stream.</summary>
		public static unsafe TextureType FromStream<TextureType>(Stream stream,
			RenderTargetUsage usage = RenderTargetUsage.DiscardContents)
			where TextureType : Texture2D
		{
			// Load through GDI Bitmap because it doesn't cause issues with alpha
			using (Bitmap bitmap = (Bitmap) Bitmap.FromStream(stream)) {
				// Create a texture to output the bitmap to
				Texture2D texture;
				if (typeof(Texture).Equals(typeof(Texture2D))) {
					texture = new Texture2D(Resources.GraphicsDevice,
						bitmap.Width, bitmap.Height, false, SurfaceFormat.Color);
				}
				else if (typeof(Texture).Equals(typeof(RenderTarget2D))) {
					texture = new RenderTarget2D(Resources.GraphicsDevice,
						bitmap.Width, bitmap.Height, false, SurfaceFormat.Color,
						DepthFormat.None, 0, usage);
				}
				else {
					throw new ArgumentException("TextureType must be Texture2D or " +
						"RenderTarget2D!");
				}

				// Write the bitmap to the texture
				WriteBitmapToTexture2D(bitmap, texture);

				return texture as TextureType;
			}
		}

		/// <summary>Premultiplies the alpha in the texture.<para/>
		/// Based on http://jakepoz.com/jake_poznanski__background_load_xna.html </summary>
		public static void PremultiplyAlpha(Texture2D texture) {
			// Setup a render target to hold our final texture
			// which will have premulitplied alpha values
			using (RenderTarget2D renderTarget = new RenderTarget2D(
				Resources.GraphicsDevice, texture.Width, texture.Height))
			{
				Resources.GraphicsDevice.SetRenderTarget(renderTarget);
				Resources.GraphicsDevice.Clear(XnaColor.Black);

				// Multiply each color by the source alpha, and write
				// in just the color values into the final texture
				Resources.SpriteBatch.Begin(SpriteSortMode.Immediate,
					BlendColorBlendState);
				Resources.SpriteBatch.Draw(texture, texture.Bounds, XnaColor.White);
				Resources.SpriteBatch.End();

				// Now copy over the alpha values from the source
				// texture to the final one, without multiplying them
				Resources.SpriteBatch.Begin(SpriteSortMode.Immediate,
					BlendAlphaBlendState);
				Resources.SpriteBatch.Draw(texture, texture.Bounds, XnaColor.White);
				Resources.SpriteBatch.End();

				// Release the GPU back to drawing to the screen
				Resources.GraphicsDevice.SetRenderTarget(null);

				// Store data from render target because the RenderTarget2D is volatile
				XnaColor[] data = new XnaColor[texture.Width * texture.Height];
				renderTarget.GetData<XnaColor>(data);

				// Unset texture from graphic device and set modified data back to it
				Resources.GraphicsDevice.Textures[0] = null;
				texture.SetData<XnaColor>(data);
			}
		}
	}
}
