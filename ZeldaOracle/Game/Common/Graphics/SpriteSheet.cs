using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Common.Graphics {
	public class SpriteSheet {
		
		private Image	image;
		private Point2I	cellSize;	// The size of each cell.
		private Point2I	offset;		// Position where the cells begin.
		private Point2I	spacing;	// Pixels between cells.


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------
		
		public SpriteSheet(Image image, Point2I cellSize, Point2I offset, Point2I spacing) {
			this.image		= image;
			this.cellSize	= cellSize;
			this.offset		= offset;
			this.spacing	= spacing;
		}

		public SpriteSheet(Image image, int cellWidth, int cellHeight, int offsetX, int offsetY, int spacing) :
			this(image, cellWidth, cellHeight, offsetX, offsetY, spacing, spacing)
		{
		}

		public SpriteSheet(Image image, int cellWidth, int cellHeight, int offsetX, int offsetY, int spacingX, int spacingY) {
			this.image		= image;
			this.cellSize	= new Point2I(cellWidth, cellHeight);
			this.offset		= new Point2I(offsetX, offsetY);
			this.spacing	= new Point2I(spacingX, spacingY);
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public Image Image {
			get { return image; }
			set { image = value; }
		}

		public Point2I CellSize {
			get { return cellSize; }
			set { cellSize = value; }
		}

		public Point2I Offset {
			get { return offset; }
			set { offset = value; }
		}

		public Point2I Spacing {
			get { return spacing; }
			set { spacing = value; }
		}
	}
}
