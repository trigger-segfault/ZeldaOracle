using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;

namespace ZeldaOracle.Game.Tiles {

	public abstract class BaseTileset<T> : ITileset where T : BaseTileData {
		
		// The string identifier for the tileset.
		protected string		id;
		// The dimensions of the tileset.
		protected Point2I		size;
		// The sprite sheet that represents the tileset.
		protected SpriteSheet	sheet;
		// 2D Array of the tile-data contained in the tileset.
		protected T[,]			tileData;
		// The coordinate in the tile data grid of the default tile.
		protected Point2I		defaultTile;
		/// <summary>True if the existing 16x16 preview sprites are used to display tiles.</summary>
		protected bool			usePreviewSprites;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public BaseTileset(string id, SpriteSheet sheet, Point2I size) :
			this(id, sheet, size.X, size.Y)
		{
		}

		public BaseTileset(string id, SpriteSheet sheet, int width, int height) {
			this.id				= id;
			this.sheet			= sheet;
			this.size			= new Point2I(width, height);
			this.defaultTile	= Point2I.Zero;
			this.tileData		= new T[width, height];
			this.usePreviewSprites = true;

			// Create default tile data.
			Point2I location;
			for (location.X = 0; location.X < size.X; location.X++) {
				for (location.Y = 0; location.Y < size.Y; location.Y++) {
					tileData[location.X, location.Y] = CreateDefaultTileData(location);
				}
			}
		}


		//-----------------------------------------------------------------------------
		// Abstract Methods
		//-----------------------------------------------------------------------------
		
		protected abstract T CreateDefaultTileData(Point2I location);



		//-----------------------------------------------------------------------------
		// Implementations
		//-----------------------------------------------------------------------------
		
		public BaseTileData GetTileData(int x, int y) {
			return tileData[x, y];
		}

		public BaseTileData GetTileData(Point2I location) {
			return tileData[location.X, location.Y];
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------
		
		public string ID {
			get { return id; }
			set { id = value; }
		}

		public T[,] TileData {
			get { return tileData; }
		}
		
		public T DefaultTileData {
			get { return tileData[defaultTile.X, defaultTile.Y]; }
		}
		
		public Point2I Dimensions {
			get { return size; }
			set { size = value; }
		}
		
		public int Width {
			get { return size.X; }
			set { size.X = value; }
		}
		
		public int Height {
			get { return size.Y; }
			set { size.Y = value; }
		}
		
		public Point2I DefaultTile {
			get { return defaultTile; }
			set { defaultTile = value; }
		}

		public SpriteSheet SpriteSheet {
			get { return sheet; }
			set { sheet = value; }
		}
		
		public Point2I CellSize {
			get {
				if (sheet == null)
					return new Point2I(16, 16);
				return sheet.CellSize;
			}
		}
		
		public Point2I Spacing {
			get {
				if (sheet == null)
					return new Point2I(1, 1);
				return sheet.Spacing;
			}
		}

		public bool UsePreviewSprites {
			get { return usePreviewSprites; }
			set { usePreviewSprites = value; }
		}
	}
}
