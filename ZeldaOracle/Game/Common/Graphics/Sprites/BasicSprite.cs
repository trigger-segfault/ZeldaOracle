using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Common.Graphics.Sprites {
	/// <summary>The base sprite that all sprites are composed of.</summary>
	public class BasicSprite : ISprite {
		/// <summary>The image used by the sprite.</summary>
		private Image image;
		/// <summary>The source rectangle of the sprite.</summary>
		private Rectangle2I sourceRect;
		/// <summary>The draw offset of the sprite.</summary>
		private Point2I drawOffset;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public BasicSprite() {
			this.image			= null;
			this.sourceRect		= Rectangle2I.Zero;
			this.drawOffset		= Point2I.Zero;
		}

		public BasicSprite(SpriteSheet sheet, int indexX, int indexY) :
			this(sheet, new Point2I(indexX, indexY), Point2I.Zero)
		{
		}

		public BasicSprite(SpriteSheet sheet, int indexX, int indexY, int drawOffsetX, int drawOffsetY) :
			this(sheet, new Point2I(indexX, indexY), new Point2I(drawOffsetX, drawOffsetY))
		{
		}

		public BasicSprite(SpriteSheet sheet, Point2I index) :
			this(sheet, index, Point2I.Zero)
		{
		}

		public BasicSprite(SpriteSheet sheet, Point2I index, Point2I drawOffset) {
			this.image          = sheet.Image;
			this.sourceRect     = new Rectangle2I(
				sheet.Offset.X + (index.X * (sheet.CellSize.X + sheet.Spacing.X)),
				sheet.Offset.Y + (index.Y * (sheet.CellSize.Y + sheet.Spacing.Y)),
				sheet.CellSize.X,
				sheet.CellSize.Y
			);
			this.drawOffset     = drawOffset;
		}



		public BasicSprite(Image image, int sourceX, int sourceY, int sourceWidth, int sourceHeight) :
			this(image, new Rectangle2I(sourceX, sourceY, sourceWidth, sourceHeight), Point2I.Zero)
		{
		}

		public BasicSprite(Image image, int sourceX, int sourceY, int sourceWidth, int sourceHeight, int drawOffsetX, int drawOffsetY) :
			this(image, new Rectangle2I(sourceX, sourceY, sourceWidth, sourceHeight), new Point2I(drawOffsetX, drawOffsetY))
		{
		}

		public BasicSprite(Image image, Rectangle2I sourceRect) :
			this(image, sourceRect, Point2I.Zero)
		{
		}

		public BasicSprite(Image image, Rectangle2I sourceRect, Point2I drawOffset) {
			this.image          = image;
			this.sourceRect     = sourceRect;
			this.drawOffset     = drawOffset;
		}

		public BasicSprite(BasicSprite copy) {
			this.image			= copy.image;
			this.sourceRect		= copy.sourceRect;
			this.drawOffset		= copy.drawOffset;
		}


		//-----------------------------------------------------------------------------
		// ISprite Overrides
		//-----------------------------------------------------------------------------

		/// <summary>Gets the drawable parts for the sprite.</summary>
		public IEnumerable<SpritePart> GetParts(SpriteDrawSettings settings) {
			yield return new SpritePart(image, sourceRect, drawOffset);
		}

		/// <summary>Clones the sprite.</summary>
		public ISprite Clone() {
			return new BasicSprite(this);
		}

		/// <summary>Gets the draw boundaries of the sprite.</summary>
		public Rectangle2I GetBounds(SpriteDrawSettings settings) {
			return new Rectangle2I(drawOffset, sourceRect.Size);
		}

		/// <summary>Gets the draw boundaries of the sprite.</summary>
		public Rectangle2I Bounds {
			get { return new Rectangle2I(drawOffset, sourceRect.Size); }
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets or sets the image used by the sprite.</summary>
		public Image Image {
			get { return image; }
			set { image = value; }
		}

		/// <summary>Gets or sets the source rectangle of the sprite.</summary>
		public Rectangle2I SourceRect {
			get { return sourceRect; }
			set { sourceRect = value; }
		}

		/// <summary>Gets or sets the draw offset of the sprite.</summary>
		public Point2I DrawOffset {
			get { return drawOffset; }
			set { drawOffset = value; }
		}
	}
}
