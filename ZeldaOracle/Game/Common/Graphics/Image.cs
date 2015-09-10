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

	//========== CONSTANTS ===========
	#region Constants

	#endregion
	//=========== MEMBERS ============
	#region Members
	
	/** <summary> The texture of the image. </summary> */
	private Texture2D texture;
	/** <summary> The file path of the image. </summary> */
	private string filePath;

	#endregion
	//========= CONSTRUCTORS =========
	#region Constructors

	/** <summary> Constructs an image with the specified texture. </summary> */
	public Image(Texture2D texture, string filePath = "") {
		this.texture		= texture;
		this.filePath		= filePath;
	}
	/** <summary> Load an image from the specified file path. </summary> */
	public Image(ContentManager content, string filePath) {
		if (filePath.Length != 0)
			this.texture	= content.Load<Texture2D>(filePath);
		else
			this.texture	= null;
		this.filePath		= filePath;
	}
	/** <summary> Constructs an new image with the specified texture information. </summary> */
	public Image(GraphicsDevice graphicsDevice, int width, int height) {
		this.texture		= new Texture2D(graphicsDevice, width, height);
		this.filePath		= "";
	}
	/** <summary> Constructs an new image with the specified texture information. </summary> */
	public Image(GraphicsDevice graphicsDevice, Point2I size) {
		this.texture		= new Texture2D(graphicsDevice, size.X, size.Y);
		this.filePath		= "";
	}
	/** <summary> Constructs an new image with the specified texture information. </summary> */
	public Image(GraphicsDevice graphicsDevice, int width, int height, bool mipMap, SurfaceFormat format) {
		this.texture		= new Texture2D(graphicsDevice, width, height, mipMap, format);
		this.filePath		= "";
	}
	/** <summary> Constructs an new image with the specified texture information. </summary> */
	public Image(GraphicsDevice graphicsDevice, Point2I size, bool mipMap, SurfaceFormat format) {
		this.texture		= new Texture2D(graphicsDevice, size.X, size.Y, mipMap, format);
		this.filePath		= "";
	}

	#endregion
	//========== OPERATORS ===========
	#region Operators

	public static implicit operator Texture2D(Image image) {
		return image.texture;
	}

	#endregion
	//========== PROPERTIES ==========
	#region Properties
	//--------------------------------
	#region Dimensions

	/** <summary> Gets the bounding box of the image. </summary> */
	public Rectangle2I Bounds {
		get { return texture.Bounds; }
	}
	/** <summary> Gets the size of the image. </summary> */
	public Point2I Size {
		get { return new Point2I(texture.Width, texture.Height); }
	}
	/** <summary> Gets the width of the image. </summary> */
	public int Width {
		get { return texture.Width; }
	}
	/** <summary> Gets the height of the image. </summary> */
	public int Height {
		get { return texture.Height; }
	}

	#endregion
	//--------------------------------
	#region Information

	/** <summary> Gets the format of the image. </summary> */
	public SurfaceFormat Format {
		get { return (texture != null ? texture.Format : SurfaceFormat.Color); }
	}
	/** <summary> Gets the texture of the image. </summary> */
	public Texture2D Texture {
		get { return texture; }
	}
	/** <summary> Gets the file path of the image. </summary> */
	public string FilePath {
		get { return filePath; }
	}
	/** <summary> Gets the graphics device associated with the image. </summary> */
	public GraphicsDevice GraphicsDevice {
		get { return (texture != null ? texture.GraphicsDevice : null); }
	}
	/** <summary> Returns true if the image has been disposed. </summary> */
	public bool IsDisposed {
		get { return (texture != null ? texture.IsDisposed : true); }
	}

	#endregion
	//--------------------------------
	#endregion
	//========== MANAGEMENT ==========
	#region Management

	/** <summary> Loads the image from the file path. </summary> */
	public void Load(ContentManager content) {
		if ((texture == null || texture.IsDisposed) && filePath.Length != 0)
			texture = content.Load<Texture2D>(filePath);
	}
	/** <summary> Immediately releases the unmanaged resources used by the texture. </summary> */
	public void Dispose() {
		if (texture != null)
			texture.Dispose();
	}

	#endregion
}
} // End namespace
