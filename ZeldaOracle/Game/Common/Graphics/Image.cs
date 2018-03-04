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

		/// <summary>Constructs an unassigned image.</summary>
		public Image() {
			this.texture		= null;
			this.filePath		= "";
		}

		/// <summary>Constructs an image with the specified texture.</summary>
		public Image(Texture2D texture, string filePath = "") {
			this.texture		= texture;
			this.filePath		= filePath;
		}

		/// <summary>Load an image from the specified file path.</summary>
		public Image(ContentManager content, string filePath) {
			if (filePath.Length != 0)
				this.texture	= content.Load<Texture2D>(filePath);
			else
				this.texture	= null;
			this.filePath		= filePath;
		}

		/// <summary>Constructs an new image with the specified texture information.</summary>
		public Image(GraphicsDevice graphicsDevice, int width, int height) {
			this.texture		= new Texture2D(graphicsDevice, width, height);
			this.filePath		= "";
		}

		/// <summary>Constructs an new image with the specified texture information.</summary>
		public Image(GraphicsDevice graphicsDevice, Point2I size) {
			this.texture		= new Texture2D(graphicsDevice, size.X, size.Y);
			this.filePath		= "";
		}

		/// <summary>Constructs an new image with the specified texture information.</summary>
		public Image(GraphicsDevice graphicsDevice, int width, int height, SurfaceFormat format) {
			this.texture		= new Texture2D(graphicsDevice, width, height, false, format);
			this.filePath		= "";
		}

		/// <summary>Constructs an new image with the specified texture information.</summary>
		public Image(GraphicsDevice graphicsDevice, Point2I size, SurfaceFormat format) {
			this.texture		= new Texture2D(graphicsDevice, size.X, size.Y, false, format);
			this.filePath		= "";
		}


		//-----------------------------------------------------------------------------
		// Management
		//-----------------------------------------------------------------------------

		/// <summary>Loads the image from the file path.</summary>
		public void Load(ContentManager content) {
			if ((texture == null || texture.IsDisposed) && filePath.Length != 0)
				texture = content.Load<Texture2D>(filePath);
		}

		/// <summary>Immediately releases the unmanaged resources used by the texture.</summary>
		public void Dispose() {
			if (texture != null)
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
		public Texture2D Texture {
			get { return texture; }
		}

		/// <summary>Gets the file path of the image.</summary>
		public string FilePath {
			get { return filePath; }
			set { filePath = value; }
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
