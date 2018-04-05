using System;
using System.IO;

using Microsoft.Xna.Framework.Graphics;
using XnaColor = Microsoft.Xna.Framework.Color;
using XnaRectangle = Microsoft.Xna.Framework.Rectangle;

using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Util;

namespace ZeldaOracle.Common.Graphics {
	/// <summary>An image containing a texture.</summary>
	public class Image : IDisposable {

		/// <summary>The texture of the image.</summary>
		protected Texture2D texture;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		/// <summary>Constructs an unassigned image and does not add it to the
		/// content database's independent ContentContainer.</summary>
		protected Image() {
			texture = null;
		}

		/// <summary>Constructs an image with the specified texture.</summary>
		protected Image(Texture2D texture) {
			if (texture == null)
				throw new ArgumentNullException("Image's texture cannot be null!");
			this.texture = texture;
			ContentContainer.AddDisposable(texture);
		}

		/// <summary>Constructs an new image with the specified texture size.</summary>
		public Image(int width, int height) {
			texture = new Texture2D(ContentContainer.GraphicsDevice, width, height);
			ContentContainer.AddDisposable(texture);
		}

		/// <summary>Constructs an new image with the specified texture size.</summary>
		public Image(Point2I size) {
			texture = new Texture2D(ContentContainer.GraphicsDevice, size.X, size.Y);
			ContentContainer.AddDisposable(texture);
		}

		/// <summary>Constructs an new image with the specified texture information.</summary>
		public Image(int width, int height, SurfaceFormat format) {
			texture = new Texture2D(ContentContainer.GraphicsDevice,
				width, height, false, format);
			ContentContainer.AddDisposable(texture);
		}

		/// <summary>Constructs an new image with the specified texture information.</summary>
		public Image(Point2I size, SurfaceFormat format) {
			texture = new Texture2D(ContentContainer.GraphicsDevice,
				size.X, size.Y, false, format);
			ContentContainer.AddDisposable(texture);
		}


		//-----------------------------------------------------------------------------
		// Disposing
		//-----------------------------------------------------------------------------

		/// <summary>Immediately releases the unmanaged ContentContainer used by the texture.</summary>
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
		// Texture Data
		//-----------------------------------------------------------------------------

		/// <summary>Creates an empty array of color data to use.</summary>
		public XnaColor[] CreateData() {
			return new XnaColor[Width * Height];
		}

		/// <summary>Creates an empty array of color data to use.</summary>
		public XnaColor[] CreateData(Rectangle2I source) {
			return new XnaColor[source.Area];
		}

		/// <summary>Returns a pixel array with the entire image.</summary>
		public XnaColor[] GetData() {
			XnaColor[] colorData = new XnaColor[Width * Height];
			return GetData(colorData, Bounds);
		}

		/// <summary>Assigns the entire image to the initialized pixel array.</summary>
		public XnaColor[] GetData(XnaColor[] colorData) {
			return GetData(colorData, Bounds);
		}

		/// <summary>Returns a pixel array with the source of the image.</summary>
		public XnaColor[] GetData(Rectangle2I source) {
			XnaColor[] colorData = new XnaColor[source.Area];
			return GetData(colorData, source);
		}

		/// <summary>Assigns the source of the image to the initialized pixel array.</summary>
		public XnaColor[] GetData(XnaColor[] colorData, Rectangle2I source) {
			XnaRectangle rect = source.ToXnaRectangle();
			texture.GetData<XnaColor>(0, rect, colorData, 0, colorData.Length);
			return colorData;
		}

		/// <summary>Sets the pixel array for the entire image.</summary>
		public void SetData(XnaColor[] colorData) {
			SetData(colorData, Bounds);
		}

		/// <summary>Sets the pixel array for the source of the image.</summary>
		public void SetData(XnaColor[] colorData, Rectangle2I source) {
			XnaRectangle rect = source.ToXnaRectangle();
			texture.SetData<XnaColor>(0, rect, colorData, 0, colorData.Length);
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

		/// <summary>Wraps the XNA Texture2D into an Image.</summary>
		public static Image Wrap(Texture2D texture) {
			Image wrapper = new Image();
			// Assign the render target this way so that is not added
			// as an independent resource to the content database.
			wrapper.texture = texture;
			return wrapper;
		}

		/// <summary>Loads the texture from content.</summary>
		public static Image FromContent(string assetName) {
			Image image = new Image();
			// Assign the texture this way so that is not added as
			// an independent resource to the content database.
			image.texture = ContentContainer.ContentManager.Load<Texture2D>(assetName);
			return image;
		}

		/// <summary>Loads the texture from the stream with the specified file size.</summary>
		public static Image FromFile(string filePath, bool premultiply = false) {
			using (Stream stream = File.OpenRead(filePath))
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
			get { return texture?.Format ?? SurfaceFormat.Color; }
		}

		/// <summary>Gets the graphics device associated with the image.</summary>
		public GraphicsDevice GraphicsDevice {
			get { return texture?.GraphicsDevice; }
		}

		/// <summary>Returns true if the image has been disposed.</summary>
		public bool IsDisposed {
			get { return texture?.IsDisposed ?? true; }
		}
	}
}
