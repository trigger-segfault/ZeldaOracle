using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Worlds;

namespace ZeldaOracle.Game.Control {

	public enum TileLayerOrder {
		LowestToHighest = 0,
		HighestToLowest = 1,
	}

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
			this.tileGridCellSize	= new Point2I(GameSettings.TILE_SIZE);
		}

		public void Initialize(Room room) {
			layerCount = room.LayerCount;
			gridDimensions = (Point2I) GMath.Ceiling((Vector2F)
				(room.Size * GameSettings.TILE_SIZE) / tileGridCellSize);
			tiles = new Tile[gridDimensions.X, gridDimensions.Y, layerCount];
		}


		//-----------------------------------------------------------------------------
		// General Grid Methods
		//-----------------------------------------------------------------------------
		
		// Return true if the given tile location is inside the room.
		public bool IsTileInBounds(Point2I location, int layer = 0) {
			return (location.X >= 0 && location.X < GridWidth &&
					location.Y >= 0 && location.Y < GridHeight &&
					layer >= 0 && layer < layerCount);
		}

		// Return the tile location that the given position in pixels is situated in.
		public Point2I GetTileLocation(Vector2F position) {
			return (Point2I) GMath.Floor(position / tileGridCellSize);
		}

		// inflateAmount inflates the output rectangle.
		public Rectangle2I GetTileAreaFromRect(Rectangle2F rect, int inflateAmount = 0) {
			Rectangle2I area;
			area.Point	= (Point2I) GMath.Floor(rect.TopLeft / tileGridCellSize);
			area.Size	= (Point2I) GMath.Ceiling(rect.BottomRight / tileGridCellSize) - area.Point;
			if (inflateAmount != 0)
				area.Inflate(inflateAmount, inflateAmount);
			return Rectangle2I.Intersect(area, new Rectangle2I(Point2I.Zero, gridDimensions));
		}


		//-----------------------------------------------------------------------------
		// Tile Accessors
		//-----------------------------------------------------------------------------
		
		// Return an enumerable list of tiles at the given location.
		public IEnumerable<Tile> GetTilesAtLocation(Point2I location, TileLayerOrder layerOrder = TileLayerOrder.LowestToHighest) {
			if (IsTileInBounds(location)) {
				foreach (int i in GetLayers(layerOrder)) {
					Tile tile = tiles[location.X, location.Y, i];
					if (tile != null)
						yield return tile;
				}
			}
		}
		
		// Return an enumerable list of tiles touching the given point.
		public IEnumerable<Tile> GetTilesAtPosition(Vector2F position, TileLayerOrder layerOrder = TileLayerOrder.LowestToHighest) {
			Point2I location = GetTileLocation(position);
			foreach (Tile tile in GetTilesAtLocation(location, layerOrder)) {
				if (tile.Bounds.Contains(position))
					yield return tile;
			}
		}
		
		// Return an enumerable list of tiles touching the given rectangular bounds.
		public IEnumerable<Tile> GetTilesTouching(Rectangle2F bounds, TileLayerOrder layerOrder = TileLayerOrder.LowestToHighest) {
			Rectangle2I area = GetTileAreaFromRect(bounds);
			foreach (Tile tile in GetTilesInArea(area, layerOrder)) {
				if (tile.Bounds.Intersects(bounds))
					yield return tile;
			}
		}
		
		// Return an enumerable list of solid tiles colliding with the given collision box.
		public IEnumerable<Tile> GetSolidTilesColliding(Rectangle2F collisionBox, TileLayerOrder layerOrder = TileLayerOrder.LowestToHighest) {
			Rectangle2I area = GetTileAreaFromRect(collisionBox);
			foreach (Tile tile in GetTilesInArea(area, layerOrder)) {
				if (tile.IsSolid && tile.CollisionModel != null &&
					CollisionModel.Intersecting(tile.CollisionModel, tile.Position, collisionBox))
					yield return tile;
			}
		}
		
		// Return an enumerable list of tiles contained within the given tile grid area.
		public IEnumerable<Tile> GetTilesInArea(Rectangle2I area, TileLayerOrder layerOrder = TileLayerOrder.LowestToHighest) {
			Rectangle2I clippedArea = Rectangle2I.Intersect(area,
				new Rectangle2I(Point2I.Zero, gridDimensions));
			foreach (int i in GetLayers(layerOrder)) {
				for (int y = clippedArea.Top; y < clippedArea.Bottom; y++) {
					for (int x = clippedArea.Left; x < clippedArea.Right; x++) {
						Tile tile = tiles[x, y, i];
						if (tile != null) {
							Point2I loc = tile.TileGridArea.Point;
							if (!clippedArea.Contains(loc))
								loc = Point2I.Clamp(loc, clippedArea);
							if (x == loc.X && y == loc.Y)
								yield return tile;
						}
					}
				}
			}
		}
		
		// Return an enumerable list of tiles contained within the given tile grid area.
		public IEnumerable<Tile> GetTopTilesInArea(Rectangle2I area) {
			Rectangle2I clippedArea = Rectangle2I.Intersect(area,
				new Rectangle2I(Point2I.Zero, gridDimensions));
			for (int y = clippedArea.Top; y < clippedArea.Bottom; y++) {
				for (int x = clippedArea.Left; x < clippedArea.Right; x++) {
					Tile tile = GetTopTile(x, y);
					if (tile != null) {
						// If this tile is larger than 1x1, then make sure
						// we don't return it twice
						Point2I loc = tile.TileGridArea.Point;
						if (!clippedArea.Contains(loc))
							loc = Point2I.Clamp(loc, clippedArea);
						if (x == loc.X && y == loc.Y)
							yield return tile;
					}
				}
			}
		}

		// Return an enumerable list of all tiles.
		public IEnumerable<Tile> GetTiles(TileLayerOrder layerOrder = TileLayerOrder.LowestToHighest) {
			foreach (int i in GetLayers(layerOrder)) {
				for (int x = 0; x < GridWidth; x++) {
					for (int y = 0; y < GridHeight; y++) {
						Tile tile = tiles[x, y, i];
						if (tile != null && IsTileAtGridLocation(tile, x, y))
							yield return tile;
					}
				}
			}
		}


		//-----------------------------------------------------------------------------
		// Singular Tile Accessors
		//-----------------------------------------------------------------------------
		
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
			return GetTopTile(new Point2I(x, y));
		}

		// Return the tile at the given location that's on the highest layer.
		public Tile GetTopTile(Point2I location) {
			return GetTilesAtLocation(location, TileLayerOrder.HighestToLowest).FirstOrDefault();
		}
		
		// Get the topmost tile touching the given point.
		public Tile GetTopTileAtPosition(Vector2F position) {
			return GetTilesAtPosition(position, TileLayerOrder.HighestToLowest).FirstOrDefault();
		}

		// Return the highest surface tile at the given position.
		public Tile GetSurfaceTile(Point2I location) {
			foreach (Tile tile in GetTilesAtLocation(location, TileLayerOrder.HighestToLowest)) {
				if (tile.IsSurface)
					return tile;
			}
			return null;
		}

		// Return the highest surface tile at the given position.
		public Tile GetSurfaceTileAtPosition(Vector2F position, bool includePlatforms = false) {
			// Because tiles may have moved this frame, we need to check a 3x3 area.
			Point2I location = GetTileLocation(position);
			Rectangle2I area = new Rectangle2I(location, Point2I.One);
			area.Inflate(1, 1);

			foreach (Tile tile in GetTilesInArea(area, TileLayerOrder.HighestToLowest)) {
				Rectangle2F tileBounds = tile.Bounds;
				tileBounds.Point -= tile.Velocity;
				if (tileBounds.Contains(position) && (tile.IsSurface || (includePlatforms && tile.IsPlatform)))
					return tile;
			}

			return null;
		}
		

		//-----------------------------------------------------------------------------
		// Tile Mutators
		//-----------------------------------------------------------------------------
		
		// Place a tile in highest empty layer at the given location.
		// Returns true if there was an empty space to place the tile.
		public bool PlaceTileOnHighestLayer(Tile tile, Point2I location) {
			Rectangle2F tileBounds = new Rectangle2F(
				location * GameSettings.TILE_SIZE,
				tile.Size * GameSettings.TILE_SIZE);
			Rectangle2I area = GetTileAreaFromRect(tileBounds);

			// Determine which layers are free.
			bool[] freeLayers = new bool[layerCount];
			for (int i = 0; i < layerCount; i++) {
				freeLayers[i] = true;
				for (int x = area.Left; x < area.Right && freeLayers[i]; x++) {
					for (int y = area.Top; y < area.Bottom && freeLayers[i]; y++) {
						Tile t = tiles[x, y, i];
						if (t != null)
							freeLayers[i] = false;
					}
				}
			}
			
			// Choose the highest free layer.
			int layer = -1;
			for (int i = layerCount - 1; i >= 0; i--) {
				if (freeLayers[i]) {
					layer = i;
					break;
				}
			}
			if (layer < 0)
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
			tile.Location			= location;
			tile.PreviousLocation	= location;
			tile.Layer				= layer;
			Rectangle2I area = GetTileArea(tile);
			tile.TileGridArea	= area;

			for (int x = area.Left; x < area.Right; x++) {
				for (int y = area.Top; y < area.Bottom; y++) {
					tiles[x, y, layer] = tile;
				}
			}
			if (initializeTile)
				tile.Initialize(roomControl);
			
			// Check for covered tiles.
			Rectangle2F tileBounds = tile.Bounds;
			foreach (Tile t in GetTilesTouching(tileBounds)) {
				if (t != tile) {
					t.OnCoverBegin(tile);
					if (tileBounds.Contains(t.Bounds))
						t.OnCoverComplete(tile);
				}
			}
		}

		// Remove a tile from the room.
		public void RemoveTile(Tile tile) {
			bool isLayer1 = (tile.Layer == 0);
			Point2I location = tile.Location;

			// Find the tile in the grid so it can be removed
			Rectangle2I area = tile.TileGridArea;
			for (int x = area.Left; x < area.Right; x++) {
				for (int y = area.Top; y < area.Bottom; y++) {
					tiles[x, y, tile.Layer] = null;
				}
			}
			tile.IsAlive = false;
			tile.OnRemoveFromRoom();
			
			// Check for uncovered tiles.
			Rectangle2F tileBounds = tile.Bounds;
			foreach (Tile t in GetTilesTouching(tileBounds)) {
				t.OnUncoverBegin(tile);
				t.OnUncoverComplete(tile);
			}

			// Place the determined tile underneath if on layer 1
			if (isLayer1) {
				TileData tileBelow = RoomControl.Zone.DefaultTileData;
				if (tile.TileBelow != null)
					tileBelow = tile.TileBelow;
				if (tileBelow != null)
					PlaceTile(Tile.CreateTile(tileBelow), location, 0);
			}
		}

		// Move the given tile to a new location.
		public void MoveTile(Tile tile, Point2I newLocation, int newLayer) {
			tile.Location = newLocation;
			// TODO: Is this supposed to be like this?
			/*
			if (tile.Layer != newLayer) {
				Rectangle2I area = tile.TileGridArea;
				for (int x = area.Left; x < area.Right; x++) {
					for (int y = area.Top; y < area.Bottom; y++) {
						tiles[x, y, tile.Layer] = null;
						tiles[x, y, newLayer] = tile;
					}
				}
				tile.Layer = newLayer;
			}*/
		}
		

		//-----------------------------------------------------------------------------
		// Tile Game Flow
		//-----------------------------------------------------------------------------

		private IEnumerable<Tile> IterateTilesOnce() {
			foreach (Tile t in GetTiles())
				t.IsUpdated = false;
			for (int i = 0; i < layerCount; i++) {
				for (int y = 0; y < GridHeight; y++) {
					for (int x = 0; x < GridWidth; x++) {
						Tile tile = tiles[x, y, i];
						if (tile != null && IsTileAtGridLocation(tile, x, y) && !tile.IsUpdated) {
							tile.IsUpdated = true;
							yield return tile;
						}
					}
				}
			}
		}

		// Initialize all tiles in the grid.
		public void InitializeTiles() {
			foreach (Tile tile in IterateTilesOnce()) {
				tile.Initialize(roomControl);
				UpdateTileGridArea(tile);
			}
			/*foreach (Tile t in GetTiles())
				t.IsUpdated = false;
			for (int i = 0; i < layerCount; i++) {
				for (int y = 0; y < GridHeight; y++) {
					for (int x = 0; x < GridWidth; x++) {
						Tile tile = tiles[x, y, i];
						if (tile != null && IsTileAtGridLocation(tile, x, y) && !tile.IsUpdated) {
							tile.Initialize(roomControl);
							UpdateTileGridArea(tile);
						}
					}
				}
			}*/
		}

		// Initialize all tiles in the grid.
		public void PostInitializeTiles() {
			foreach (Tile tile in IterateTilesOnce()) {
				tile.OnPostInitialize();
				UpdateTileGridArea(tile);
			}
		}
		
		// Update all tiles in the grid.
		public void UpdateTiles() {
			foreach (Tile t in IterateTilesOnce()) {
				Point2I prevLocation = t.Location;
				Vector2F prevOffset = t.Offset;

				if (GameControl.UpdateRoom)
					t.Update();
				if (GameControl.AnimateRoom)
					t.UpdateGraphics();

				t.PreviousLocation = prevLocation;
				t.PreviousOffset = prevOffset;
							
				if (t.IsAlive)
					UpdateTileGridArea(t);
			}
		}
		
		// Draw all tiles in the grid that draw with entities
		public void DrawTiles(RoomGraphics g) {
			for (int i = 0; i < layerCount; i++) {
				for (int y = 0; y < GridHeight; y++) {
					for (int x = 0; x < GridWidth; x++) {
						Tile t = tiles[x, y, i];

						if (t != null && IsTileAtGridLocation(t, x, y) && !t.DrawAsEntity) {
							t.Draw(g);

							/*
							// DEBUG: draw grid cell occupance.
							Rectangle2F tileBounds = (Rectangle2F) t.TileGridArea;
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


		// Draw all tiles in the grid that do not draw with entities
		public void DrawEntityTiles(RoomGraphics g) {
			for (int i = 0; i < layerCount; i++) {
				for (int y = 0; y < GridHeight; y++) {
					for (int x = 0; x < GridWidth; x++) {
						Tile t = tiles[x, y, i];

						if (t != null && IsTileAtGridLocation(t, x, y) && t.DrawAsEntity)
							t.Draw(g);
					}
				}
			}
		}

		// Draw all tiles in the grid.
		public void DrawTilesAbove(RoomGraphics g) {
			for (int i = 0; i < layerCount; i++) {
				for (int y = 0; y < GridHeight; y++) {
					for (int x = 0; x < GridWidth; x++) {
						Tile t = tiles[x, y, i];

						if (t != null && IsTileAtGridLocation(t, x, y))
							t.DrawAbove(g);
					}
				}
			}
		}


		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------

		private bool IsTileAtGridLocation(Tile tile, Point2I gridLocation) {
			return (tile.TileGridArea.Point == gridLocation);
		}

		private bool IsTileAtGridLocation(Tile tile, int x, int y) {
			return (tile.TileGridArea.X == x && tile.TileGridArea.Y == y);
		}
		
		private void UpdateTileGridArea(Tile tile) {
			Rectangle2F tileBounds	= new Rectangle2F(tile.Position, tile.Size * GameSettings.TILE_SIZE);
			Rectangle2I nextArea	= GetTileAreaFromRect(tileBounds);
			Rectangle2I prevArea	= tile.TileGridArea;
			
			if (nextArea != prevArea) {
				// Determine the highest free layer for the tile to move to.
				int newLayer = -1;
				for (int i = tile.Layer; i < layerCount; i++) {
					bool isLayerFree = true;
					for (int x = nextArea.Left; x < nextArea.Right && isLayerFree; x++) {
						for (int y = nextArea.Top; y < nextArea.Bottom && isLayerFree; y++) {
							Tile t = tiles[x, y, i];
							if (t != null && t != tile)
								isLayerFree = false;
						}
					}
					if (isLayerFree) {
						newLayer = i;
						break;
					}
				}
				if (newLayer < 0) {
					RemoveTile(tile);
					return;
				}

				// Remove the tile from its old area.
				for (int x = prevArea.Left; x < prevArea.Right; x++) {
					for (int y = prevArea.Top; y < prevArea.Bottom; y++) {
						tiles[x, y, tile.Layer] = null;
					}
				}

				// Place the tile into its new area.
				for (int x = nextArea.Left; x < nextArea.Right; x++) {
					for (int y = nextArea.Top; y < nextArea.Bottom; y++) {
						tiles[x, y, tile.Layer] = tile;
					}
				}

				tile.TileGridArea = nextArea;
			}
			
			// Check for covered/uncovered tiles.
			Rectangle2F tileBoundsOld = tile.PreviousBounds;
			Rectangle2F tileBoundsNew = tile.Bounds;
			if (tileBoundsOld != tileBoundsNew) {
				foreach (Tile t in GetTilesInArea(nextArea).Union(GetTilesInArea(prevArea))) {
					Rectangle2F tBounds = t.Bounds;
					if (t != tile) {
						if (tileBoundsNew.Contains(tBounds) && !tileBoundsOld.Contains(tBounds))
							t.OnCoverComplete(tile);
						else if (tileBoundsNew.Intersects(tBounds) && !tileBoundsOld.Intersects(tBounds))
							t.OnCoverBegin(tile);
						else if (tileBoundsOld.Contains(tBounds) && !tileBoundsNew.Contains(tBounds))
							t.OnUncoverBegin(tile);
						else if (tileBoundsOld.Intersects(tBounds) && !tileBoundsNew.Intersects(tBounds))
							t.OnUncoverComplete(tile);
					}
				}
			}
		}

		private Rectangle2I GetTileArea(Tile tile) {
			return GetTileAreaFromRect(tile.Bounds);
		}

		// Return an enumerable list of tile layers in the specified order.
		public IEnumerable<int> GetLayers(TileLayerOrder order) {
			if (order == TileLayerOrder.LowestToHighest) {
				for (int layer = 0; layer < layerCount; layer++)
					yield return layer;
			}
			else {
				for (int layer = layerCount - 1; layer >= 0; layer--)
					yield return layer;
			}
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
		
		public int LayerCount {
			get { return layerCount; }
		}
		
		public Rectangle2I GridArea {
			get { return new Rectangle2I(Point2I.Zero, gridDimensions); }
		}
	}
}
