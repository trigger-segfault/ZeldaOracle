using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
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

		private Point2I						size;
		private int							layerCount;
		private TileGridTile[,,]			tiles;
		private List<EventTileDataInstance>	eventTiles;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public TileGrid(Point2I size, int layerCount) {
			this.size		= size;
			this.layerCount	= layerCount;
			this.tiles		= new TileGridTile[size.X, size.Y, layerCount];
			this.eventTiles	= new List<EventTileDataInstance>();

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
						if (tile.Tile != null && tile.Location == new Point2I(x, y))
							yield return tile.Tile;
					}
				}
			}

			// Iterate event tiles.
			foreach (EventTileDataInstance eventTile in eventTiles) {
				yield return eventTile;
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

		public void AddEventTile(EventTileDataInstance eventTile) {
			eventTiles.Add(eventTile);
		}
		
		public void RemoveEventTile(EventTileDataInstance eventTile) {
			eventTiles.Remove(eventTile);
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
			foreach (EventTileDataInstance eventTile in eventTiles) {
				EventTileDataInstance copy = new EventTileDataInstance();
				copy.Clone(eventTile);
				duplicate.AddEventTile(copy);
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
		
		public List<EventTileDataInstance> EventTiles {
			get { return eventTiles; }
		}
	}
}
