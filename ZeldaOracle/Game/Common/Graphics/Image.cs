using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using XnaColor		= Microsoft.Xna.Framework.Color;

using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Common.Graphics {
	/// <summary>An image containing a texture.</summary>
	public class Image {

		/// <summary>The texture of the image.</summary>
		private Texture2D texture;
		/// <summary>The file path of the image.</summary>
		private string filePath;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public Image() {
			this.texture		= null;
			this.filePath		= "";
		}

		// Constructs an image with the specified texture.
		public Image(Texture2D texture, string filePath = "") {
			this.texture		= texture;
			this.filePath		= filePath;
		}

		// Load an image from the specified file path.
		public Image(ContentManager content, string filePath) {
			if (filePath.Length != 0)
				this.texture	= content.Load<Texture2D>(filePath);
			else
				this.texture	= null;
			this.filePath		= filePath;
		}

		// Constructs an new image with the specified texture information.
		public Image(GraphicsDevice graphicsDevice, int width, int height) {
			this.texture		= new Texture2D(graphicsDevice, width, height);
			this.filePath		= "";
		}

		// Constructs an new image with the specified texture information.
		public Image(GraphicsDevice graphicsDevice, Point2I size) {
			this.texture		= new Texture2D(graphicsDevice, size.X, size.Y);
			this.filePath		= "";
		}

		// Constructs an new image with the specified texture information.
		public Image(GraphicsDevice graphicsDevice, int width, int height, bool mipMap, SurfaceFormat format) {
			this.texture		= new Texture2D(graphicsDevice, width, height, mipMap, format);
			this.filePath		= "";
		}

		// Constructs an new image with the specified texture information.
		public Image(GraphicsDevice graphicsDevice, Point2I size, bool mipMap, SurfaceFormat format) {
			this.texture		= new Texture2D(graphicsDevice, size.X, size.Y, mipMap, format);
			this.filePath		= "";
		}

		
		//-----------------------------------------------------------------------------
		// Management
		//-----------------------------------------------------------------------------

		// Loads the image from the file path.
		public void Load(ContentManager content) {
			if ((texture == null || texture.IsDisposed) && filePath.Length != 0)
				texture = content.Load<Texture2D>(filePath);
		}

		// Immediately releases the unmanaged resources used by the texture.
		public void Dispose() {
			if (texture != null)
				texture.Dispose();
		}


		//-----------------------------------------------------------------------------
		// Operators
		//-----------------------------------------------------------------------------

		// Used to auto-convert Images into XNA Texture2Ds.
		public static implicit operator Texture2D(Image image) {
			return image.texture;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------
		
		// Dimensions

		// Gets the bounding box of the image.
		public Rectangle2I Bounds {
			get { return texture.Bounds; }
		}

		// Gets the size of the image.
		public Point2I Size {
			get { return new Point2I(texture.Width, texture.Height); }
		}

		// Gets the width of the image.
		public int Width {
			get { return texture.Width; }
		}

		// Gets the height of the image.
		public int Height {
			get { return texture.Height; }
		}

		// Information

		// Gets the texture of the image.
		public Texture2D Texture {
			get { return texture; }
		}

		// Gets the file path of the image.
		public string FilePath {
			get { return filePath; }
			set { filePath = value; }
		}

		// Gets the format of the image.
		public SurfaceFormat Format {
			get { return (texture != null ? texture.Format : SurfaceFormat.Color); }
		}

		// Gets the graphics device associated with the image.
		public GraphicsDevice GraphicsDevice {
			get { return (texture != null ? texture.GraphicsDevice : null); }
		}

		// Returns true if the image has been disposed.
		public bool IsDisposed {
			get { return (texture != null ? texture.IsDisposed : true); }
		}
	}
}
