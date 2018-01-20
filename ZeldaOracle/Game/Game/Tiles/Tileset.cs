using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Game.Tiles {
	public class Tileset {

		/// <summary>The string identifier for the tileset.</summary>
		protected string			id;
		/// <summary>2D Array of the tile-data contained in the tileset.</summary>
		protected BaseTileData[,]	tileDataGrid;
		/// <summary>True if the existing 16x16 preview sprites are used to display tiles.</summary>
		protected bool				usePreviewSprites;
		/// <summary>The amount of spacing between tiles.</summary>
		private Point2I				spacing;
		/// <summary>The number of non-null tiles in the grid.</summary>
		private int					count;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		/// <summary>Constructs an empty tileset.</summary>
		public Tileset() {
			this.id					= "";
			this.tileDataGrid		= new BaseTileData[0, 0];
			this.usePreviewSprites	= true;
			this.spacing			= Point2I.One;
			this.count				= 0;
		}

		/// <summary>Constructs a new tileset with the specified ID and dimensions.</summary>
		public Tileset(string id, int width, int height, bool usePreviewSprites = true) {
			this.id					= id;
			this.tileDataGrid		= new BaseTileData[width, height];
			this.usePreviewSprites	= usePreviewSprites;
			this.spacing			= Point2I.One;
			this.count				= 0;
		}

		/// <summary>Constructs a new tileset with the specified ID and dimensions.</summary>
		public Tileset(string id, Point2I dimensions, bool usePreviewSprites = true) {
			this.id					= id;
			this.tileDataGrid		= new BaseTileData[dimensions.X, dimensions.Y];
			this.usePreviewSprites	= usePreviewSprites;
			this.spacing			= Point2I.One;
			this.count				= 0;
		}

		/// <summary>Constructs a copy of the tileset.</summary>
		public Tileset(Tileset copy) {
			this.id					= copy.id;
			this.tileDataGrid		= new BaseTileData[copy.Width, copy.Height];
			this.usePreviewSprites	= copy.usePreviewSprites;
			this.spacing			= copy.spacing;
			this.count				= copy.count;

			for (int x = 0; x < Width; x++) {
				for (int y = 0; y < Height; y++) {
					this.tileDataGrid[x, y] = copy.tileDataGrid[x, y];
				}
			}
		}


		//-----------------------------------------------------------------------------
		// Accessors
		//-----------------------------------------------------------------------------

		/// <summary>Return the tile data at the given location.</summary>
		public BaseTileData GetTileData(int x, int y) {
			return GetTileData(new Point2I(x, y));
		}

		/// <summary>Return the tile data at the given location.</summary>
		public BaseTileData GetTileData(Point2I location) {
			Point2I finalLocation = GetTileDataOrigin(location);
			if (finalLocation != -Point2I.One)
				return tileDataGrid[finalLocation.X, finalLocation.Y];
			return null;
		}

		/// <summary>Return the tile data at the given location
		/// only if that location is the tile's origin.</summary>
		public BaseTileData GetTileDataAtOrigin(int x, int y) {
			return GetTileDataAtOrigin(new Point2I(x, y));
		}

		/// <summary>Return the tile data at the given location
		/// only if that location is the tile's origin.</summary>
		public BaseTileData GetTileDataAtOrigin(Point2I location) {
			return tileDataGrid[location.X, location.Y];
		}

		/// <summary>Gets the top-left corner of the tile data if it has a size larger than 1x1.</summary>
		public Point2I GetTileDataOrigin(int x, int y) {
			return GetTileDataOrigin(new Point2I(x, y));
		}

		/// <summary>Gets the top-left corner of the tile data if it has a size larger than 1x1.</summary>
		public Point2I GetTileDataOrigin(Point2I location) {
			BaseTileData tileData = tileDataGrid[location.X, location.Y];
			if (tileData == null) {
				if (usePreviewSprites)
					return -Point2I.One;

				// Scan for the tile data in an outward pattern centered at the location.
				int max = Math.Max(location.X, location.Y);
				for (int i = 1; i < max; i++) {
					if (i <= location.Y) {
						for (int x = 0; x < i && x <= location.X; x++) {
							tileData = tileDataGrid[location.X - x, location.Y - i];
							if (tileData != null && new Point2I(x, i) < tileData.Size)
								return location - new Point2I(x, i);
						}
					}
					if (i <= location.X) {
						for (int y = 0; y < i && y <= location.Y; y++) {
							tileData = tileDataGrid[location.X - i, location.Y - y];
							if (tileData != null && new Point2I(i, y) < tileData.Size)
								return location - new Point2I(i, y);
						}
					}
					if (new Point2I(i, i) <= location) {
						tileData = tileDataGrid[location.X - i, location.Y - i];
						if (tileData != null && new Point2I(i, i) < tileData.Size)
							return location - new Point2I(i, i);
					}
				}
				return -Point2I.One;
			}
			return location;
		}


		//-----------------------------------------------------------------------------
		// Mutators
		//-----------------------------------------------------------------------------

		/// <summary>Adds the tile data at the given location.
		/// Throw an exception if the tile data is already set.</summary>
		public void AddTileData(int x, int y, BaseTileData tileData) {
			AddTileData(new Point2I(x, y), tileData);
		}

		/// <summary>Adds the tile data at the given location.
		/// Throw an exception if the tile data is already set.</summary>
		public void AddTileData(Point2I location, BaseTileData tileData) {
			if (tileData != null) {
				count++;
				if (!usePreviewSprites) {
					Point2I size = tileData.Size;
					for (int x = 0; x < size.X && x + location.X < Width; x++) {
						for (int y = 0; y < size.Y && y + location.Y < Height; y++) {
							Point2I newLoc = location + new Point2I(x, y);
							if (GetTileData(newLoc) != null)
								throw new InvalidOperationException("Cannot add tile at " + newLoc +
									" because a tile already occupies that space!");
						}
					}
				}
				else if (tileDataGrid[location.X, location.Y] != null) {
					throw new InvalidOperationException("Cannot add tile at " + location +
									" because a tile already occupies that space!");
				}
			}

			tileDataGrid[location.X, location.Y] = tileData;
		}

		/// <summary>Sets the tile data at the given location.</summary>
		public void SetTileData(int x, int y, BaseTileData tileData) {
			SetTileData(new Point2I(x, y), tileData);
		}

		/// <summary>Sets the tile data at the given location.</summary>
		public void SetTileData(Point2I location, BaseTileData tileData) {
			if (tileData != null) {
				count++;
				if (!usePreviewSprites) {
					Point2I size = tileData.Size;
					for (int x = 0; x < size.X && x + location.X < Width; x++) {
						for (int y = 0; y < size.Y && y + location.Y < Height; y++) {
							Point2I occupiedLoc = GetTileDataOrigin(location + new Point2I(x, y));
							if (occupiedLoc != -Point2I.One) {
								count--;
								tileDataGrid[occupiedLoc.X, occupiedLoc.Y] = null;
							}
						}
					}
				}
				else if (tileDataGrid[location.X, location.Y] != null) {
					count--;
					tileDataGrid[location.X, location.Y] = null;
				}
			}

			tileDataGrid[location.X, location.Y] = tileData;
		}

		/// <summary>Removes the tile data at the given location.</summary>
		public void RemoveTileData(int x, int y) {
			RemoveTileData(new Point2I(x, y));
		}

		/// <summary>Removes the tile data at the given location.</summary>
		public void RemoveTileData(Point2I location) {
			location = GetTileDataOrigin(location);
			if (location != -Point2I.One) {
				count--;
				tileDataGrid[location.X, location.Y] = null;
			}
		}

		/// <summary>Resizes the tileset to the new dimensions.</summary>
		public void Resize(Point2I newDimensions) {
			if (!(newDimensions >= Point2I.One))
				throw new ArgumentException("Cannot resize tileset with dimensions less than one!");

			count = 0;
			BaseTileData[,] newTileDataGrid = new BaseTileData[newDimensions.X, newDimensions.Y];
			for (int x = 0; x < Math.Min(Width, newDimensions.X); x++) {
				for (int y = 0; y < Math.Min(Height, newDimensions.Y); y++) {
					newTileDataGrid[x, y] = tileDataGrid[x, y];
					if (tileDataGrid[x, y] != null)
						count++;
				}
			}
			tileDataGrid = newTileDataGrid;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets or sets the ID of the tileset.</summary>
		public string ID {
			get { return id; }
			set { id = value; }
		}

		/// <summary>Gets the dimensions of the sprite set.</summary>
		public Point2I Dimensions {
			get { return new Point2I(tileDataGrid.GetLength(0), tileDataGrid.GetLength(1)); }
		}

		/// <summary>Gets the width of the sprite set.</summary>
		public int Width {
			get { return tileDataGrid.GetLength(0); }
		}

		/// <summary>Gets the height of the sprite set.</summary>
		public int Height {
			get { return tileDataGrid.GetLength(1); }
		}

		/// <summary>Gets or sets the amount of spacing between tiles.</summary>
		public Point2I Spacing {
			get { return spacing; }
			set { spacing = value; }
		}

		/// <summary>Gets or sets if the existing 16x16 preview sprites are used to display tiles.</summary>
		public bool UsePreviewSprites {
			get { return usePreviewSprites; }
			set { usePreviewSprites = value; }
		}

		/// <summary>Gets the number of non-null tiles present in the tileset.</summary>
		public int Count {
			get { return count; }
		}
	}
}
