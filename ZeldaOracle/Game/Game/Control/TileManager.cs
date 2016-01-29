using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Worlds;

namespace ZeldaOracle.Game.Control {

	public class TileManager {

		private RoomControl roomControl;
		private Tile[,,]	tiles;
		private Point2I		tileGridCellSize;
		private int			layerCount;
		private Point2I		gridDimensions;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public TileManager(RoomControl roomControl) {
			this.roomControl		= roomControl;
			this.tileGridCellSize	= new Point2I(16, 16);
		}

		public void Initialize(Room room) {
			layerCount = room.LayerCount;
			gridDimensions = (Point2I) GMath.Ceiling((Vector2F)
				(room.Size * GameSettings.TILE_SIZE) / tileGridCellSize);
			tiles = new Tile[gridDimensions.X, gridDimensions.Y, layerCount];
		}


		//-----------------------------------------------------------------------------
		// Tile Accessors
		//-----------------------------------------------------------------------------
		
		// Return an enumerable list of all tiles touching the given position.
		public IEnumerable<Tile> GetTilesAtPosition(Vector2F position) {
			Point2I location = (Point2I) (position / tileGridCellSize);
			if (IsTileInBounds(location)) {
				for (int i = 0; i < layerCount; i++) {
					Tile tile = tiles[location.X, location.Y, i];
					if (tile != null && GetTileBounds(tile).Contains(position))
						yield return tile;
				}
			}
		}
		
		// Return an enumerable list of tiles touching the given rectangular bounds.
		public IEnumerable<Tile> GetTilesTouching(Rectangle2F bounds) {
			Rectangle2I area = GetTileAreaFromRect(bounds);
			foreach (Tile tile in GetTilesInArea(area)) {
				if (GetTileBounds(tile).Intersects(bounds))
					yield return tile;
			}
		}

		// Return an enumerable list of tiles.
		public IEnumerable<Tile> GetTiles() {
			for (int i = 0; i < layerCount; i++) {
				for (int x = 0; x < GridWidth; x++) {
					for (int y = 0; y < GridHeight; y++) {
						Tile tile = tiles[x, y, i];
						if (tile != null && IsTileAtGridLocation(tile, x, y))
							yield return tile;
					}
				}
			}
		}
		
		// Return an enumerable list of tiles in the given grid based area.
		public IEnumerable<Tile> GetTilesInArea(Rectangle2I area) {
			Rectangle2I clippedArea = Rectangle2I.Intersect(area,
				new Rectangle2I(Point2I.Zero, gridDimensions));

			for (int i = 0; i < layerCount; i++) {
				for (int x = clippedArea.Left; x < clippedArea.Right; x++) {
					for (int y = clippedArea.Top; y < clippedArea.Bottom; y++) {
						Tile tile = tiles[x, y, i];
						if (tile != null) {
							Point2I loc = tile.TileGridLocation.Point;
							if (!clippedArea.Contains(loc))
								loc = Point2I.Clamp(loc, clippedArea);
							if (x == loc.X && y == loc.Y)
								yield return tile;
						}
					}
				}
			}
		}
		
		// Return the tile at the given location (can return null).
		public Tile GetTile(Point2I location, int layer) {
			return tiles[location.X, location.Y, layer];
		}

		// Return the tile at the given location (can return null).
		public Tile GetTile(int x, int y, int layer) {
			return tiles[x, y, layer];
		}

		// Return the tile at the given location that's on the highest layer.
		public Tile GetTopTile(int x, int y) {
			for (int i = layerCount - 1; i >= 0; i--) {
				if (tiles[x, y, i] != null)
					return tiles[x, y, i];
			}
			return null;
		}

		// Return the tile at the given location that's on the highest layer.
		public Tile GetTopTile(Point2I location) {
			for (int i = layerCount - 1; i >= 0; i--) {
				if (tiles[location.X, location.Y, i] != null)
					return tiles[location.X, location.Y, i];
			}
			return null;
		}
		
		// Return true if the given tile location is inside the room.
		public bool IsTileInBounds(Point2I location, int layer = 0) {
			return (location.X >= 0 && location.X < GridWidth &&
					location.Y >= 0 && location.Y < GridHeight &&
					layer >= 0 && layer < layerCount);
		}

		// Return the tile location that the given position in pixels is situated in.
		public Point2I GetTileLocation(Vector2F position) {
			return (Point2I) (position / tileGridCellSize);
		}

		// inflateAmount inflates the output rectangle.
		public Rectangle2I GetTileAreaFromRect(Rectangle2F rect, int inflateAmount = 0) {
			Rectangle2I area;
			rect.Size = (GMath.Ceiling((rect.Point + rect.Size) / tileGridCellSize) * tileGridCellSize) - rect.Point;
			area.Point	= (Point2I) (rect.TopLeft / (Vector2F) tileGridCellSize);
			area.Size	= ((Point2I) (rect.BottomRight / (Vector2F) tileGridCellSize)) - area.Point;
			if (area.Size < Point2I.One)
				area.Size = Point2I.One;
			area.Inflate(inflateAmount, inflateAmount);
			return Rectangle2I.Intersect(area, new Rectangle2I(Point2I.Zero, gridDimensions));
		}
		

		//-----------------------------------------------------------------------------
		// Tile Mutators
		//-----------------------------------------------------------------------------
		
		// Place a tile in highest empty layer at the given location.
		// Returns true if there was an empty space to place the tile.
		public bool PlaceTileOnHighestLayer(Tile tile, Point2I location) {
			Rectangle2I area = GetTileArea(tile);

			int layer = -1;
			for (int x = area.Left; x < area.Right; x++) {
				for (int y = area.Top; y < area.Bottom; y++) {
					for (int i = 0; i < layerCount; i++) {
						Tile t = tiles[x, y, i];
						if (t != null && layer <= t.Layer)
							layer = t.Layer + 1;
					}
				}
			}
			
			if (layer < 0 || layer >= layerCount)
				return false;

			// Place the tile in that layer.
			PlaceTile(tile, location, layer);
			return true;
		}
		
		
		// Place a tile in the tile grid at the given location and layer.
		public void PlaceTile(Tile tile, int x, int y, int layer, bool initializeTile = true) {
			PlaceTile(tile, new Point2I(x, y), layer, initializeTile);
		}

		// Use this for placing tiles at runtime.
		public void PlaceTile(Tile tile, Point2I location, int layer, bool initializeTile = true) {
			tile.Location	= location;
			tile.Layer		= layer;
			Rectangle2I area = GetTileArea(tile);
			tile.TileGridLocation = area;

			for (int x = area.Left; x < area.Right; x++) {
				for (int y = area.Top; y < area.Bottom; y++) {
					tiles[x, y, layer] = tile;
				}
			}
			if (initializeTile)
				tile.Initialize(roomControl);
		}

		// Remove a tile from the room.
		public void RemoveTile(Tile tile) {
			Rectangle2I area = tile.TileGridLocation;
			for (int x = area.Left; x < area.Right; x++) {
				for (int y = area.Top; y < area.Bottom; y++) {
					tiles[x, y, tile.Layer] = null;
				}
			}
			tile.IsAlive = false;
			tile.OnRemoveFromRoom();
		}

		// Move the given tile to a new location.
		public void MoveTile(Tile tile, Point2I newLocation, int newLayer) {
			tile.Location = newLocation;
			tile.Layer = newLayer;
		}
		

		//-----------------------------------------------------------------------------
		// Tile Game Flow
		//-----------------------------------------------------------------------------

		// Initialize all tiles in the grid.
		public void InitializeTiles() {
			for (int i = 0; i < layerCount; i++) {
				for (int y = 0; y < GridHeight; y++) {
					for (int x = 0; x < GridWidth; x++) {
						Tile tile = tiles[x, y, i];
						if (tile != null) {
							tile.Initialize(roomControl);
							UpdateTileGridArea(tile);
						}
					}
				}
			}
		}
		
		// Update all tiles in the grid.
		public void UpdateTiles() {
			foreach (Tile t in GetTiles())
				t.IsUpdated = false;
			for (int i = 0; i < layerCount; i++) {
				for (int y = 0; y < GridHeight; y++) {
					for (int x = 0; x < GridWidth; x++) {
						Tile t = tiles[x, y, i];
						if (t != null && IsTileAtGridLocation(t, x, y) && !t.IsUpdated) {
							t.IsUpdated = true;
							if (GameControl.UpdateRoom)
								t.Update();
							if (GameControl.AnimateRoom)
								t.UpdateGraphics();
							UpdateTileGridArea(t);
						}
					}
				}
			}
		}
		
		// Draw all tiles in the grid.
		public void DrawTiles(Graphics2D g) {
			for (int i = 0; i < layerCount; i++) {
				for (int y = 0; y < GridHeight; y++) {
					for (int x = 0; x < GridWidth; x++) {
						Tile t = tiles[x, y, i];

						if (t != null && IsTileAtGridLocation(t, x, y)) {
							t.Draw(g);

							/*
							// DEBUG: draw grid cell occupance.
							Rectangle2F tileBounds = (Rectangle2F) t.TileGridLocation;
							tileBounds.Point *= tileGridCellSize;
							tileBounds.Size *= tileGridCellSize;
							Color c = Color.Yellow;
							if (i == 1)
								c = Color.Blue;
							else if (i == 2)
								c = Color.Red;
							g.FillRectangle(tileBounds, c);

							tileBounds = new Rectangle2F(t.Position, t.Size * GameSettings.TILE_SIZE);
							c = Color.Olive;
							if (i == 1)
								c = Color.Cyan;
							else if (i == 2)
								c = Color.Maroon;

							g.DrawLine(new Line2F(tileBounds.TopLeft, tileBounds.BottomRight - new Point2I(1, 1)), 1, c);
							g.DrawLine(new Line2F(tileBounds.TopRight - new Point2I(1, 0), tileBounds.BottomLeft - new Point2I(0, 1)), 1, c);
							g.DrawRectangle(tileBounds, 1, Color.Black);
							*/
						}
					}
				}
			}
		}
		

		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------
		
		private bool IsTileAtGridLocation(Tile tile, Point2I gridLocation) {
			return (tile.TileGridLocation.Point == gridLocation);
		}

		private bool IsTileAtGridLocation(Tile tile, int x, int y) {
			return (tile.TileGridLocation.X == x && tile.TileGridLocation.Y == y);
		}
		
		private void UpdateTileGridArea(Tile tile) {
			Rectangle2F tileBounds	= new Rectangle2F(tile.Position, tile.Size * GameSettings.TILE_SIZE);
			Rectangle2I area		= GetTileAreaFromRect(tileBounds);
			Rectangle2I prevArea	= tile.TileGridLocation;
			
			if (area != prevArea) {
				for (int x = prevArea.Left; x < prevArea.Right; x++) {
					for (int y = prevArea.Top; y < prevArea.Bottom; y++) {
						tiles[x, y, tile.Layer] = null;
					}
				}
				tile.TileGridLocation = area;
				for (int x = area.Left; x < area.Right; x++) {
					for (int y = area.Top; y < area.Bottom; y++) {
						tiles[x, y, tile.Layer] = tile;
					}
				}
			}
		}

		private Rectangle2I GetTileArea(Tile tile) {
			return GetTileAreaFromRect(GetTileBounds(tile));
		}

		private Rectangle2F GetTileBounds(Tile tile) {
			return new Rectangle2F(tile.Position, tile.Size * GameSettings.TILE_SIZE);
		}

		
		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public RoomControl RoomControl {
			get { return roomControl; }
		}
		
		public GameControl GameControl {
			get { return roomControl.GameControl; }
		}

		public int GridWidth {
			get { return gridDimensions.X; }
		}
		
		public int GridHeight {
			get { return gridDimensions.Y; }
		}
	}
}
