using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Control.Scripting;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Tiles.EventTiles;

namespace ZeldaOracle.Game.Worlds {
	
	public class TileGrid {
		
		private struct TileGridTile {
			public TileDataInstance Tile { get; set; }
			public Point2I Location { get; set; }
			public int Layer { get; set; }

			
			public void Set(TileDataInstance tile, int x, int y, int layer) {
				this.Tile		= tile;
				this.Location	= new Point2I(x, y);
				this.Layer		= layer;
			}

			public void Clear(int x, int y, int layer) {
				this.Tile		= null;
				this.Location	= new Point2I(x, y);
				this.Layer		= layer;
			}
		}

		private struct TileGridEvent {
			public EventTileDataInstance EventTile { get; set; }
			public Point2I Position { get; set; }
			
			public TileGridEvent(EventTileDataInstance eventTile, int x, int y) {
				this.EventTile	= eventTile;
				this.Position	= new Point2I(x, y);
			}

			public void Set(EventTileDataInstance eventTile, int x, int y) {
				this.EventTile	= eventTile;
				this.Position	= new Point2I(x, y);
			}

			public void Clear(int x, int y, int layer) {
				this.EventTile	= null;
				this.Position	= new Point2I(x, y);
			}
		}

		private Point2I						size;
		private int							layerCount;
		private TileGridTile[,,]			tiles;
		private List<TileGridEvent>			eventTiles;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------
		
		public TileGrid(Point2I size, int layerCount) {
			this.size		= size;
			this.layerCount	= layerCount;
			this.tiles		= new TileGridTile[size.X, size.Y, layerCount];
			this.eventTiles	= new List<TileGridEvent>();

			Clear();
		}
		

		//-----------------------------------------------------------------------------
		// Accessors
		//-----------------------------------------------------------------------------
		
		public TileDataInstance GetTile(Point2I location, int layer) {
			return tiles[location.X, location.Y, layer].Tile;
		}

		public TileDataInstance GetTile(int x, int y, int layer) {
			return tiles[x, y, layer].Tile;
		}

		public TileDataInstance GetTileIfAtLocation(Point2I location, int layer) {
			TileGridTile tile = tiles[location.X, location.Y, layer];
			if (tile.Location.X == location.X && tile.Location.Y == location.Y && tile.Layer == layer)
				return tile.Tile;
			return null;
		}
		public TileDataInstance GetTileIfAtLocation(int x, int y, int layer) {
			TileGridTile tile = tiles[x, y, layer];
			if (tile.Location.X == x && tile.Location.Y == y && tile.Layer == layer)
				return tile.Tile;
			return null;
		}

		public IEnumerable<BaseTileDataInstance> GetTiles() {
			// Iterate tiles.
			for (int x = 0; x < size.X; x++) {
				for (int y = 0; y < size.Y; y++) {
					for (int i = 0; i < layerCount; i++) {
						TileGridTile tile = tiles[x, y, i];
						if (tile.Tile != null && tile.Location == new Point2I(x, y)) {
							tile.Tile.Location = tile.Location;
							yield return tile.Tile;
						}
					}
				}
			}

			// Iterate event tiles.
			foreach (TileGridEvent tile in eventTiles) {
				tile.EventTile.Position = tile.Position;
				yield return tile.EventTile;
			}
		}
		public IEnumerable<EventTileDataInstance> GetEventTiles() {
			// Iterate event tiles.
			foreach (TileGridEvent tile in eventTiles) {
				//tile.EventTile.Position = tile.Position;
				yield return tile.EventTile;
			}
		}
		public IEnumerable<KeyValuePair<Point2I, EventTileDataInstance>> GetEventTilePositions() {
			// Iterate event tiles.
			foreach (TileGridEvent tile in eventTiles) {
				yield return new KeyValuePair<Point2I, EventTileDataInstance>(tile.Position, tile.EventTile);
			}
		}

		//-----------------------------------------------------------------------------
		// Mutators
		//-----------------------------------------------------------------------------

		public void Clear() {
			for (int x = 0; x < size.X; x++) {
				for (int y = 0; y < size.Y; y++) {
					for (int i = 0; i < layerCount; i++) {
						tiles[x, y, i].Set(null, x, y, i);
					}
				}
			}
			eventTiles.Clear();
		}

		public void PlaceTile(TileDataInstance tile, int x, int y, int layer) {
			PlaceTile(tile, new Point2I(x, y), layer);
		}

		public void PlaceTile(TileDataInstance tile, Point2I location, int layer) {
			Point2I size = tile.Size;

			for (int x = 0; x < size.X; x++) {
				for (int y = 0; y < size.Y; y++) {
					Point2I loc = new Point2I(location.X + x, location.Y + y);
					if (loc.X < Width && loc.Y < Height) {
						// Remove existing tile.
						TileGridTile t = tiles[loc.X, loc.Y, layer];
						if (t.Tile != null)
							RemoveTile(t);
						tiles[loc.X, loc.Y, layer].Set(tile, location.X, location.Y, layer);

					}
				}
			}
		}

		public void RemoveTile(TileDataInstance tile) {
			if (tile == null)
				return;
			for (int x = 0; x < size.X; x++) {
				for (int y = 0; y < size.Y; y++) {
					for (int i = 0; i < layerCount; i++) {
						TileGridTile t = tiles[x, y, i];
						if (t.Tile == tile) {
							RemoveTile(t);
							return;
						}
					}
				}
			}
		}

		private void RemoveTile(TileGridTile tile) {
			Point2I size = tile.Tile.Size;
			for (int x = 0; x < size.X; x++) {
				for (int y = 0; y < size.Y; y++) {
					Point2I loc = new Point2I(tile.Location.X + x, tile.Location.Y + y);
					if (loc.X < Width && loc.Y < Height)
						tiles[loc.X, loc.Y, tile.Layer].Set(null, loc.X, loc.Y, tile.Layer);
				}
			}
		}

		public void RemoveTile(int x, int y, int layer) {
			TileGridTile tile = tiles[x, y, layer];
			if (tile.Tile != null)
				RemoveTile(tile);
		}

		public void Remove(BaseTileDataInstance tile) {
			if (tile is TileDataInstance)
				RemoveTile((TileDataInstance) tile);
			else if (tile is EventTileDataInstance)
				RemoveEventTile((EventTileDataInstance) tile);
		}

		public void PlaceEventTile(EventTileDataInstance eventTile, int x, int y) {
			PlaceEventTile(eventTile, new Point2I(x, y));
		}
		public void PlaceEventTile(EventTileDataInstance eventTile, Point2I position) {
			eventTiles.Add(new TileGridEvent(eventTile, position.X, position.Y));
		}

		public void RemoveEventTile(EventTileDataInstance eventTile) {
			int index = eventTiles.FindIndex(tile => tile.EventTile == eventTile);
			if (index != -1)
				eventTiles.RemoveAt(index);
		}

		// Return a copy of this tile grid.
		public TileGrid Duplicate() {
			TileGrid duplicate = new TileGrid(size, layerCount);
			
			// Duplicate tiles.
			for (int x = 0; x < size.X; x++) {
				for (int y = 0; y < size.Y; y++) {
					for (int i = 0; i < layerCount; i++) {
						TileGridTile tile = tiles[x, y, i];
						if (tile.Tile != null && tile.Location == new Point2I(x, y)) {
							TileDataInstance tileCopy = new TileDataInstance();
							tileCopy.Clone(tile.Tile);
							duplicate.PlaceTile(tileCopy, x, y, i);
						}
					}
				}
			}
			
			// Duplicate event tiles.
			foreach (TileGridEvent tile in eventTiles) {
				EventTileDataInstance eventTileCopy = new EventTileDataInstance();
				eventTileCopy.Clone(tile.EventTile);
				duplicate.PlaceEventTile(eventTileCopy, tile.Position);
			}

			return duplicate;
		}


		//-----------------------------------------------------------------------------
		// Propreties
		//-----------------------------------------------------------------------------

		public int LayerCount {
			get { return layerCount; }
		}
		
		public int Width {
			get { return size.X; }
		}
		
		public int Height {
			get { return size.Y; }
		}
		
		public Point2I Size {
			get { return size; }
		}
	}
}
