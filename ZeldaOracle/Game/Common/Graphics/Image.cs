using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using XnaColor = Microsoft.Xna.Framework.Color;

using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Util;
using System.IO;

using Rectangle = System.Drawing.Rectangle;
using Bitmap = System.Drawing.Bitmap;
using BitmapData = System.Drawing.Imaging.BitmapData;
using ImageLockMode = System.Drawing.Imaging.ImageLockMode;
using PixelFormat = System.Drawing.Imaging.PixelFormat;
using ZeldaOracle.Common.Content;

namespace ZeldaOracle.Common.Graphics {
	/// <summary>An image containing a texture.</summary>
	public class Image : IDisposable {

		/// <summary>The texture of the image.</summary>
		protected Texture2D texture;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		/// <summary>Constructs an unassigned image and does not add it to the
		/// content database's independent resources.</summary>
		protected Image() {
			texture = null;
		}

		/// <summary>Constructs an image with the specified texture.</summary>
		protected Image(Texture2D texture) {
			if (texture == null)
				throw new ArgumentNullException("Image's texture cannot be null!");
			this.texture = texture;
			Resources.AddDisposable(texture);
		}

		/// <summary>Constructs an new image with the specified texture size.</summary>
		public Image(int width, int height) {
			texture = new Texture2D(Resources.GraphicsDevice, width, height);
			Resources.AddDisposable(texture);
		}

		/// <summary>Constructs an new image with the specified texture size.</summary>
		public Image(Point2I size) {
			texture = new Texture2D(Resources.GraphicsDevice, size.X, size.Y);
			Resources.AddDisposable(texture);
		}

		/// <summary>Constructs an new image with the specified texture information.</summary>
		public Image(int width, int height, SurfaceFormat format) {
			texture = new Texture2D(Resources.GraphicsDevice,
				width, height, false, format);
			Resources.AddDisposable(texture);
		}

		/// <summary>Constructs an new image with the specified texture information.</summary>
		public Image(Point2I size, SurfaceFormat format) {
			texture = new Texture2D(Resources.GraphicsDevice,
				size.X, size.Y, false, format);
			Resources.AddDisposable(texture);
		}


		//-----------------------------------------------------------------------------
		// Disposing
		//-----------------------------------------------------------------------------

		/// <summary>Immediately releases the unmanaged resources used by the texture.</summary>
		public void Dispose() {
			if (texture != null && !texture.IsDisposed)
				texture.Dispose();
		}


		//-----------------------------------------------------------------------------
		// Operators
		//-----------------------------------------------------------------------------

		/// <summary>Used to auto-convert Images into XNA Texture2Ds.</summary>
		public static implicit operator Texture2D(Image image) {
			return image.texture;
		}


		//-----------------------------------------------------------------------------
		// Saving
		//-----------------------------------------------------------------------------

		/// <summary>Saves the texture data as a .png to the specified file path.</summary>
		public void SaveAsPng(string filePath) {
			using (FileStream stream = File.OpenWrite(filePath)) {
				stream.SetLength(0);
				SaveAsPng(stream);
			}
		}

		/// <summary>Saves the texture data as a .png to the specified stream.</summary>
		public void SaveAsPng(Stream stream) {
			texture.SaveAsPng(stream, Width, Height);
		}

		/// <summary>Saves the texture data as a .png to the specified stream and
		/// writes the file size of the .png before the data.</summary>
		public void SaveAsPngAndSize(Stream stream) {
			var counter = BinaryCounter.Start(stream);
			SaveAsPng(stream);
			counter.WriteSizeAndReturn();
		}


		//-----------------------------------------------------------------------------
		// Static Methods
		//-----------------------------------------------------------------------------

		/// <summary>Loads the texture from content.</summary>
		public static Image FromContent(string assetName) {
			Image image = new Image();
			// Assign the texture this way so that is not added as
			// an independent resource to the content database.
			image.texture = Resources.ContentManager.Load<Texture2D>(assetName);
			return image;
		}

		/// <summary>Loads the texture from the stream with the specified file size.</summary>
		public static Image FromFile(string filePath, bool premultiply = false) {
			using (FileStream stream = File.Open(filePath, FileMode.Open))
				return FromStream(stream, premultiply);
		}

		/// <summary>Loads the texture from the stream with the specified file size.</summary>
		public static Image FromStreamAndSize(Stream stream,
			bool premultiply = false)
		{
			using (Stream memory = BinaryCounter.ReadStream(stream)) {
				if (memory.Length == 0)
					return null;
				return FromStream(memory, premultiply);
			}
		}

		/// <summary>Loads the texture from the stream.</summary>
		public static Image FromStream(Stream stream,
			bool premultiply = false)
		{
			Texture2D texture = Texture2DHelper.FromStream<Texture2D>(stream);
			if (premultiply)
				Texture2DHelper.PremultiplyAlpha(texture);
			return new Image(texture);
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		// Dimensions -----------------------------------------------------------------

		/// <summary>Gets the bounding box of the image.</summary>
		public Rectangle2I Bounds {
			get { return texture.Bounds.ToRectangle2I(); }
		}

		/// <summary>Gets the size of the image.</summary>
		public Point2I Size {
			get { return new Point2I(texture.Width, texture.Height); }
		}

		/// <summary>Gets the width of the image.</summary>
		public int Width {
			get { return texture.Width; }
		}

		/// <summary>Gets the height of the image.</summary>
		public int Height {
			get { return texture.Height; }
		}
		
		// Information ----------------------------------------------------------------

		/// <summary>Gets the texture of the image.</summary>
		public Texture2D Texture2D {
			get { return texture; }
		}

		/// <summary>Gets the format of the image.</summary>
		public SurfaceFormat Format {
			get { return (texture != null ? texture.Format : SurfaceFormat.Color); }
		}

		/// <summary>Gets the graphics device associated with the image.</summary>
		public GraphicsDevice GraphicsDevice {
			get { return (texture != null ? texture.GraphicsDevice : null); }
		}

		/// <summary>Returns true if the image has been disposed.</summary>
		public bool IsDisposed {
			get { return (texture != null ? texture.IsDisposed : true); }
		}
	}
}
