using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics.Sprites;

namespace ZeldaOracle.Common.Graphics {
	/// <summary>A grid of sprites defined in an image.</summary>
	public class SpriteSheet : ISpriteSource {

		/// <summary>The image used to get the sprites from.</summary>
		private Image	image;
		/// <summary>The size of each cell.</summary>
		private Point2I cellSize;
		/// <summary>The position where the cells begin.</summary>
		private Point2I	offset;
		/// <summary>The pixels between cells.</summary>
		private Point2I	spacing;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		/// <summary>Constructs a sprite sheet from the specified image with no cell
		/// settings.</summary>
		public SpriteSheet(Image image) {
			this.image		= image;
			cellSize		= image.Size;
			offset			= Point2I.Zero;
			spacing			= Point2I.Zero;
		}

		/// <summary>Constructs a sprite sheet with the specified settings.</summary>
		public SpriteSheet(Image image, Point2I cellSize, Point2I offset,
			Point2I spacing)
		{
			this.image		= image;
			this.cellSize	= cellSize;
			this.offset		= offset;
			this.spacing	= spacing;
		}

		/// <summary>Constructs a sprite sheet with the specified settings.</summary>
		public SpriteSheet(Image image, int cellWidth, int cellHeight, int offsetX,
			int offsetY, int spacing)
			: this(image, cellWidth, cellHeight, offsetX, offsetY, spacing, spacing)
		{
		}

		/// <summary>Constructs a sprite sheet with the specified settings.</summary>
		public SpriteSheet(Image image, int cellWidth, int cellHeight, int offsetX,
			int offsetY, int spacingX, int spacingY)
		{
			this.image		= image;
			cellSize		= new Point2I(cellWidth, cellHeight);
			offset			= new Point2I(offsetX, offsetY);
			spacing			= new Point2I(spacingX, spacingY);
		}


		//-----------------------------------------------------------------------------
		// ISpriteSheet Overrides
		//-----------------------------------------------------------------------------
		
		/// <summary>Gets the sprite at the specified index in the sheet.</summary>
		ISprite ISpriteSource.GetSprite(int indexX, int indexY) {
			return new BasicSprite(image, GetSourceRect(indexX, indexY));
		}

		/// <summary>Gets the sprite at the specified index in the sheet.</summary>
		ISprite ISpriteSource.GetSprite(Point2I index) {
			return new BasicSprite(image, GetSourceRect(index));
		}


		//-----------------------------------------------------------------------------
		// Accessors
		//-----------------------------------------------------------------------------

		/// <summary>Gets the source rect of the sprite at the specified index.</summary>
		public Rectangle2I GetSourceRect(int indexX, int indexY) {
			return new Rectangle2I(
				offset + (cellSize + spacing) * new Point2I(indexX, indexY),
				cellSize);
		}

		/// <summary>Gets the source rect of the sprite at the specified index.</summary>
		public Rectangle2I GetSourceRect(Point2I index) {
			return new Rectangle2I(
				offset + (cellSize + spacing) * index,
				cellSize);
		}

		/// <summary>Gets the sprite at the specified index.</summary>
		public BasicSprite GetSprite(int indexX, int indexY) {
			return new BasicSprite(image, GetSourceRect(indexX, indexY));
		}

		/// <summary>Gets the sprite at the specified index.</summary>
		public BasicSprite GetSprite(Point2I index) {
			return new BasicSprite(image, GetSourceRect(index));
		}

		/// <summary>Gets the sprite at the specified index with an offset.</summary>
		public BasicSprite GetSprite(int indexX, int indexY, int drawOffsetX,
			int drawOffsetY, Flip flip = Flip.None, Rotation rotation = Rotation.None)
		{
			return new BasicSprite(image, GetSourceRect(indexX, indexY),
				new Point2I(drawOffsetX, drawOffsetY), flip, rotation);
		}

		/// <summary>Gets the sprite at the specified index with an offset.</summary>
		public BasicSprite GetSprite(Point2I index, Point2I drawOffset,
			Flip flip = Flip.None, Rotation rotation = Rotation.None)
		{
			return new BasicSprite(image, GetSourceRect(index), drawOffset, flip,
				rotation);
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets or sets the image used to get the sprites from.</summary>
		public Image Image {
			get { return image; }
			set { image = value; }
		}

		/// <summary>Gets or sets the size of each cell.</summary>
		public Point2I CellSize {
			get { return cellSize; }
			set { cellSize = value; }
		}

		/// <summary>Gets or sets the position where the cells begin.</summary>
		public Point2I Offset {
			get { return offset; }
			set { offset = value; }
		}

		/// <summary>Gets or sets the pixels between cells.</summary>
		public Point2I Spacing {
			get { return spacing; }
			set { spacing = value; }
		}

		/// <summary>Calculates the dimensions of the sprite sheet.</summary>
		public Point2I Dimensions {
			get { return (image.Size - offset) / (cellSize + spacing); }
		}

		/// <summary>Gets the width of the sprite sheet.</summary>
		public int Width {
			get { return Dimensions.X; }
		}

		/// <summary>Gets the height of the sprite sheet.</summary>
		public int Height {
			get { return Dimensions.Y; }
		}
	}
}
