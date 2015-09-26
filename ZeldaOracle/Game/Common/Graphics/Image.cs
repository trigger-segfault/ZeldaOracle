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
	/** <summary>
	 * An image containing a texture.
	 * </summary> */
	public class Image {

		// The texture of the image.
		private Texture2D texture;
		// The file path of the image.
		private string filePath;
		// The name of this particular image variant.
		private string variantName;
		// The id of this particular image variant.
		private int variantID;
		// A pointer to the next variant for the image.
		private Image nextVariant;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public Image() {
			this.texture		= null;
			this.filePath		= "";
			this.variantName	= "";
			this.variantID		= 0;
			this.nextVariant	= null;
		}

		// Constructs an image with the specified texture.
		public Image(Texture2D texture, string filePath = "") {
			this.texture		= texture;
			this.filePath		= filePath;
			this.variantName	= "";
			this.variantID		= 0;
			this.nextVariant	= null;
		}

		// Load an image from the specified file path.
		public Image(ContentManager content, string filePath) {
			if (filePath.Length != 0)
				this.texture	= content.Load<Texture2D>(filePath);
			else
				this.texture	= null;
			this.filePath		= filePath;
			this.variantName	= "";
			this.variantID		= 0;
			this.nextVariant	= null;
		}

		// Constructs an new image with the specified texture information.
		public Image(GraphicsDevice graphicsDevice, int width, int height) {
			this.texture		= new Texture2D(graphicsDevice, width, height);
			this.filePath		= "";
			this.variantName	= "";
			this.variantID		= 0;
			this.nextVariant	= null;
		}

		// Constructs an new image with the specified texture information.
		public Image(GraphicsDevice graphicsDevice, Point2I size) {
			this.texture		= new Texture2D(graphicsDevice, size.X, size.Y);
			this.filePath		= "";
			this.variantName	= "";
			this.variantID		= 0;
			this.nextVariant	= null;
		}

		// Constructs an new image with the specified texture information.
		public Image(GraphicsDevice graphicsDevice, int width, int height, bool mipMap, SurfaceFormat format) {
			this.texture		= new Texture2D(graphicsDevice, width, height, mipMap, format);
			this.filePath		= "";
			this.variantName	= "";
			this.variantID		= 0;
			this.nextVariant	= null;
		}

		// Constructs an new image with the specified texture information.
		public Image(GraphicsDevice graphicsDevice, Point2I size, bool mipMap, SurfaceFormat format) {
			this.texture		= new Texture2D(graphicsDevice, size.X, size.Y, mipMap, format);
			this.filePath		= "";
			this.variantName	= "";
			this.variantID		= 0;
			this.nextVariant	= null;
		}


		//-----------------------------------------------------------------------------
		// Accessors
		//-----------------------------------------------------------------------------
		
		// Return the variant for this image with the given id.
		public Image GetVariant(int variantID) {
			if (variantID == 0)
				return this;
			Image image = this;
			while (image != null) {
				if (image.variantID == variantID)
					return image;
				image = image.nextVariant;
			}
			return this;
		}

		// Return the variant for this image with the given name.
		public Image GetVariant(string variantName) {
			Image image = this;
			while (image != null) {
				if (image.variantName == variantName)
					return image;
				image = image.nextVariant;
			}
			return this;
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

		// Gets the name of this particular image variant.
		public string VariantName {
			get { return variantName; }
			set { variantName = value; }
		}

		// Gets the id of this particular image variant.
		public int VariantID {
			get { return variantID; }
			set { variantID = value; }
		}

		// Gets the next varient for this image.
		public Image NextVariant {
			get { return nextVariant; }
			set { nextVariant = value; }
		}

		// Does this image have varaints?
		public bool HasVariants {
			get  { return (nextVariant != null); }
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
