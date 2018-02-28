using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Control.Scripting;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Tiles.ActionTiles;
using Clipboard = System.Windows.Forms.Clipboard;

namespace ZeldaOracle.Game.Worlds {
	
	public partial class TileGrid {

		//-----------------------------------------------------------------------------
		// Constants
		//-----------------------------------------------------------------------------

		/// <summary>The clipboard format identifier for tile grids.</summary>
		public const string ClipboardFormat = "CF_ZELDA_ORACLE_TILE_GRID";


		//-----------------------------------------------------------------------------
		// Classes
		//-----------------------------------------------------------------------------
		
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

		private struct TileGridAction {
			public ActionTileDataInstance ActionTile { get; set; }
			public Point2I Position { get; set; }
			
			public TileGridAction(ActionTileDataInstance actionTile, int x, int y) {
				this.ActionTile	= actionTile;
				this.Position	= new Point2I(x, y);
			}

			public void Set(ActionTileDataInstance actionTile, int x, int y) {
				this.ActionTile	= actionTile;
				this.Position	= new Point2I(x, y);
			}

			public void Clear(int x, int y, int layer) {
				this.ActionTile	= null;
				this.Position	= new Point2I(x, y);
			}
		}

		private Point2I						size;
		private int							startLayer;
		private int							layerCount;
		private bool						includeTiles;
		private bool						includeActions;
		private TileGridTile[,,]			tiles;
		private List<TileGridAction>		actionTiles;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------
		
		/// <summary>Constructs a tile grid.</summary>
		public TileGrid(Point2I size, int startLayer, int layerCount,
			bool includeTiles, bool includeActions)
		{
			this.size			= size;
			this.startLayer		= startLayer;
			this.layerCount		= layerCount;
			this.includeTiles	= includeTiles;
			this.includeActions	= includeActions;
			this.tiles			= new TileGridTile[size.X, size.Y, layerCount];
			this.actionTiles	= new List<TileGridAction>();

			Clear();
		}

		/// <summary>Constructs a copy of the existing tile grid settings.</summary> 
		private TileGrid(TileGrid copy) {
			this.size			= copy.size;
			this.startLayer		= copy.startLayer;
			this.layerCount		= copy.layerCount;
			this.includeTiles	= copy.includeTiles;
			this.includeActions	= copy.includeActions;
			this.tiles          = new TileGridTile[size.X, size.Y, layerCount];
			this.actionTiles    = new List<TileGridAction>();

			Clear();
		}


		//-----------------------------------------------------------------------------
		// Static Constructors
		//-----------------------------------------------------------------------------

		/// <summary>Creates a tile grid with all tiles and action tiles.</summary>
		public static TileGrid CreateFullTileGrid(Point2I size, int layerCount) {
			return new TileGrid(size, 0, layerCount, true, true);
		}

		/// <summary>Creates a tile grid with only a single layer of tiles.</summary>
		public static TileGrid CreateSingleLayerTileGrid(Point2I size, int startLayer) {
			return new TileGrid(size, startLayer, 1, true, false);
		}

		/// <summary>Creates a tile grid with only action tiles.</summary>
		public static TileGrid CreateActionGrid(Point2I size) {
			return new TileGrid(size, -1, 0, false, true);
		}


		//-----------------------------------------------------------------------------
		// Accessors
		//-----------------------------------------------------------------------------

		public TileDataInstance GetTile(Point2I location, int layer) {
			return tiles[location.X, location.Y, layer - startLayer].Tile;
		}

		public TileDataInstance GetTile(int x, int y, int layer) {
			return tiles[x, y, layer].Tile;
		}

		public TileDataInstance GetTileIfAtLocation(Point2I location, int layer) {
			TileGridTile tile = tiles[location.X, location.Y, layer - startLayer];
			if (tile.Location.X == location.X && tile.Location.Y == location.Y && tile.Layer == layer)
				return tile.Tile;
			return null;
		}

		public TileDataInstance GetTileIfAtLocation(int x, int y, int layer) {
			TileGridTile tile = tiles[x, y, layer - startLayer];
			if (tile.Location.X == x && tile.Location.Y == y && tile.Layer == layer)
				return tile.Tile;
			return null;
		}

		public IEnumerable<BaseTileDataInstance> GetAllTiles() {
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

			// Iterate action tiles.
			foreach (TileGridAction actionTile in actionTiles) {
				yield return actionTile.ActionTile;
			}
		}

		public IEnumerable<BaseTileDataInstance> GetAllTilesAtLocation() {
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

			// Iterate action tiles.
			foreach (TileGridAction actionTile in actionTiles) {
				actionTile.ActionTile.Position = actionTile.Position;
				yield return actionTile.ActionTile;
			}
		}

		public IEnumerable<TileDataInstance> GetTiles() {
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
		}

		public IEnumerable<TileDataInstance> GetTilesAtLocation() {
			// Iterate tiles.
			for (int x = size.X - 1; x >= 0; x--) {
				for (int y = size.Y - 1; y >= 0; y--) {
					for (int i = 0; i < layerCount; i++) {
						TileGridTile tile = tiles[x, y, i];
						if (tile.Tile != null && tile.Location == new Point2I(x, y)) {
							tile.Tile.Location = tile.Location;
							yield return tile.Tile;
						}
					}
				}
			}
		}

		public IEnumerable<KeyValuePair<Point2I, TileDataInstance>> GetTilesAndLocations() {
			// Iterate tiles.
			for (int x = size.X - 1; x >= 0; x--) {
				for (int y = size.Y - 1; y >= 0; y--) {
					for (int i = 0; i < layerCount; i++) {
						TileGridTile tile = tiles[x, y, i];
						if (tile.Tile != null && tile.Location == new Point2I(x, y)) {
							tile.Tile.Location = tile.Location;
							yield return new KeyValuePair<Point2I, TileDataInstance>(tile.Location, tile.Tile);
						}
					}
				}
			}
		}

		public IEnumerable<ActionTileDataInstance> GetActionTiles() {
			// Iterate action tiles.
			foreach (TileGridAction actionTile in actionTiles) {
				yield return actionTile.ActionTile;
			}
		}

		public IEnumerable<ActionTileDataInstance> GetActionTilesAtPosition() {
			// Iterate action tiles.
			foreach (TileGridAction actionTile in actionTiles) {
				actionTile.ActionTile.Position = actionTile.Position;
				yield return actionTile.ActionTile;
			}
		}

		public IEnumerable<KeyValuePair<Point2I, ActionTileDataInstance>> GetActionTilesAndPositions() {
			// Iterate action tiles.
			foreach (TileGridAction actionTile in actionTiles) {
				yield return new KeyValuePair<Point2I, ActionTileDataInstance>(actionTile.Position, actionTile.ActionTile);
			}
		}

		//-----------------------------------------------------------------------------
		// Mutators
		//-----------------------------------------------------------------------------

		public void Clear() {
			for (int x = 0; x < size.X; x++) {
				for (int y = 0; y < size.Y; y++) {
					for (int i = 0; i < layerCount; i++) {
						tiles[x, y, i].Set(null, x, y, i + startLayer);
					}
				}
			}
			actionTiles.Clear();
		}

		public void PlaceTile(TileDataInstance tile, int x, int y, int layer) {
			PlaceTile(tile, new Point2I(x, y), layer);
		}

		public void PlaceTile(TileDataInstance tile, Point2I location, int layer) {
			Point2I size = Point2I.One; //tile.Size;
			// Ignore tile size in tile grids because the level
			// handles overwritting caused by large tiles

			for (int x = 0; x < size.X; x++) {
				for (int y = 0; y < size.Y; y++) {
					Point2I loc = new Point2I(location.X + x, location.Y + y);
					if (loc.X < Width && loc.Y < Height) {
						// Remove existing tile.
						TileGridTile t = tiles[loc.X, loc.Y, layer - startLayer];
						if (t.Tile != null)
							RemoveTile(t);
						tiles[loc.X, loc.Y, layer - startLayer].Set(tile, location.X, location.Y, layer);

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
						tiles[loc.X, loc.Y, tile.Layer - startLayer].Set(null, loc.X, loc.Y, tile.Layer);
				}
			}
		}

		public void RemoveTile(int x, int y, int layer) {
			TileGridTile tile = tiles[x, y, layer - startLayer];
			if (tile.Tile != null)
				RemoveTile(tile);
		}

		public void Remove(BaseTileDataInstance tile) {
			if (tile is TileDataInstance)
				RemoveTile((TileDataInstance) tile);
			else if (tile is ActionTileDataInstance)
				RemoveActionTile((ActionTileDataInstance) tile);
		}

		public void PlaceActionTile(ActionTileDataInstance actionTile, int x, int y) {
			PlaceActionTile(actionTile, new Point2I(x, y));
		}
		public void PlaceActionTile(ActionTileDataInstance actionTile, Point2I position) {
			actionTiles.Add(new TileGridAction(actionTile, position.X, position.Y));
		}

		public void RemoveActionTile(ActionTileDataInstance actionTile) {
			int index = actionTiles.FindIndex(tile => tile.ActionTile == actionTile);
			if (index != -1)
				actionTiles.RemoveAt(index);
		}

		// Return a copy of this tile grid.
		public TileGrid Duplicate() {
			TileGrid duplicate = new TileGrid(this);

			if (duplicate.includeTiles) {
				// Duplicate tiles
				for (int x = 0; x < size.X; x++) {
					for (int y = 0; y < size.Y; y++) {
						for (int i = 0; i < layerCount; i++) {
							TileGridTile tile = tiles[x, y, i];
							if (tile.Tile != null && tile.Location == new Point2I(x, y)) {
								TileDataInstance tileCopy = new TileDataInstance();
								tileCopy.Clone(tile.Tile);
								duplicate.PlaceTile(tileCopy, x, y, i + startLayer);
							}
						}
					}
				}
			}

			if (duplicate.includeActions) {
				// Duplicate action tiles
				foreach (TileGridAction actionTile in actionTiles) {
					ActionTileDataInstance actionTileCopy = new ActionTileDataInstance();
					actionTileCopy.Clone(actionTile.ActionTile);
					duplicate.PlaceActionTile(actionTileCopy, actionTile.Position);
				}
			}

			return duplicate;
		}


		//-----------------------------------------------------------------------------
		// Clipboard
		//-----------------------------------------------------------------------------

		/// <summary>Returns true if the clipboard contains a TileGridReference.</summary>
		public static bool ContainsClipboard() {
			return Clipboard.ContainsData(ClipboardFormat);
		}

		/// <summary>Loads the TileGridReference from the clipboard and dereferences it.</summary>
		public static TileGrid LoadClipboard() {
			TileGridReference reference =
				(TileGridReference) Clipboard.GetData(ClipboardFormat);
			return reference.Dereference();
		}

		/// <summary>Saves the tile grid to the clipboard as a TileGridReference.</summary>
		public void SaveClipboard() {
			TileGridReference reference = new TileGridReference(this);
			Clipboard.SetData(ClipboardFormat, reference);
		}


		//-----------------------------------------------------------------------------
		// Propreties
		//-----------------------------------------------------------------------------

		public bool IncludesTiles {
			get { return includeTiles; }
		}

		public bool IncludesActions {
			get { return includeActions; }
		}
		
		public int StartLayer {
			get { return startLayer; }
		}

		public int EndLayer {
			get { return startLayer + layerCount - 1; }
		}

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
