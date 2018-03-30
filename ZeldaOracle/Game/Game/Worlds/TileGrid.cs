using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Tiles.ActionTiles;
using Clipboard = System.Windows.Forms.Clipboard;

namespace ZeldaOracle.Game.Worlds {
	/// <summary>The different modes for creating a tile grid.</summary>
	public enum CreateTileGridMode {
		/// <summary>The tile grid will contain the actual tile instances and remove
		/// them from the level.</summary>
		Remove,
		/// <summary>The tile grid will contain copies of the tile instances without
		/// removing them from the level.</summary>
		Duplicate,
		/// <summary>The tile grid will contain the actual tile instances without
		/// removing them from the level.</summary>
		Twin,
	}

	/// <summary>A grid of tiles that can be shifted around between levels.</summary>
	public partial class TileGrid {

		//-----------------------------------------------------------------------------
		// Constants
		//-----------------------------------------------------------------------------

		/// <summary>The clipboard format identifier for tile grids.</summary>
		public const string ClipboardFormat = "CF_ZELDA_ORACLE_TILE_GRID";


		//-----------------------------------------------------------------------------
		// Classes
		//-----------------------------------------------------------------------------
		
		/// <summary>Stores information on a tile and location in the grid.</summary>
		/*private struct TileInstanceLocation {
			/// <summary>The tile contained at this location. Can be null.</summary>
			public TileDataInstance Tile { get; set; }
			/// <summary>The location of this tile in the grid.</summary>
			public Point2I Location { get; set; }
			/// <summary>The layer of this tile in the grid.</summary>
			public int Layer { get; set; }

			/// <summary>Sets the tile instance for this location.</summary>
			public void Set(TileDataInstance tile, int x, int y, int layer) {
				Tile		= tile;
				Location	= new Point2I(x, y);
				Layer		= layer;
			}

			/// <summary>Sets the tile instance for this location.</summary>
			public void Set(TileDataInstance tile, Point2I location, int layer) {
				Tile		= tile;
				Location	= location;
				Layer		= layer;
			}

			/// <summary>Clears the tile instance from this location.</summary>
			public void Clear() {
				Tile		= null;
			}
		}*/

		/// <summary>Stores information on an action tile and position in the grid.</summary>
		/*private struct ActionTileInstancePosition {
			/// <summary>The action tile. This should never be null.</summary>
			public ActionTileDataInstance ActionTile { get; set; }
			/// <summary>The position of the action tile.</summary>
			public Point2I Position { get; set; }

			/// <summary>Constructs the action tile and it's position.</summary>
			public ActionTileInstancePosition(ActionTileDataInstance actionTile, int x, int y) {
				this.ActionTile	= actionTile;
				this.Position	= new Point2I(x, y);
			}

			public ActionTileInstancePosition(ActionTileDataInstance actionTile,
				Point2I position)
			{
				this.ActionTile = actionTile;
				this.Position   = position;
			}
		}*/


		//-----------------------------------------------------------------------------
		// Members
		//-----------------------------------------------------------------------------

		private Point2I		size;
		private int			startLayer;
		private int			layerCount;
		private bool		includeTiles;
		private bool		includeActions;
		private TileInstanceLocation[,,]			tiles;
		private List<ActionTileInstancePosition>	actionTiles;


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

			InitTiles();
		}

		/// <summary>Constructs a copy of the existing tile grid settings.</summary> 
		private TileGrid(TileGrid copy) {
			size			= copy.size;
			startLayer		= copy.startLayer;
			layerCount		= copy.layerCount;
			includeTiles	= copy.includeTiles;
			includeActions	= copy.includeActions;

			InitTiles();
		}

		/// <summary>Initializes the tile grid and action tile list.</summary>
		private void InitTiles() {
			tiles		= new TileInstanceLocation[size.X, size.Y, layerCount];
			actionTiles	= new List<ActionTileInstancePosition>();

			for (int x = 0; x < size.X; x++) {
				for (int y = 0; y < size.Y; y++) {
					for (int layer = startLayer; layer <= EndLayer; layer++) {
						tiles[x, y, layer - startLayer] = new TileInstanceLocation(x, y, layer);
					}
				}
			}
		}


		//-----------------------------------------------------------------------------
		// Static Constructors
		//-----------------------------------------------------------------------------

		/// <summary>Creates a tile grid with all tiles and action tiles.</summary>
		public static TileGrid CreateFullTileGrid(Point2I size, int layerCount) {
			return new TileGrid(size, 0, layerCount, true, true);
		}

		/// <summary>Creates a tile grid with only a single layer of tiles.</summary>
		public static TileGrid CreateSingleLayerTileGrid(Point2I size, int layer) {
			return new TileGrid(size, layer, 1, true, false);
		}

		/// <summary>Creates a tile grid with only action tiles.</summary>
		public static TileGrid CreateActionGrid(Point2I size) {
			return new TileGrid(size, -1, 0, false, true);
		}


		//-----------------------------------------------------------------------------
		// Singluar Accessors
		//-----------------------------------------------------------------------------
		
		/// <summary>Returns true if the grid contains a tile at the specified
		/// location.</summary>
		public bool ContainsTile(int x, int y, int layer) {
			if (Bounds.Contains(new Point2I(x, y)))
				return (tiles[x, y, layer - startLayer].Tile != null);
			return false;
		}

		/// <summary>Returns true if the grid contains a tile at the specified
		/// location.</summary>
		public bool ContainsTile(Point2I location, int layer) {
			if (Bounds.Contains(location))
				return tiles[location.X, location.Y, layer - startLayer].HasTile;
			return false;
		}

		/// <summary>Gets the tile in the grid at the specified location.</summary>
		public TileDataInstance GetTile(int x, int y, int layer) {
			if (Bounds.Contains(new Point2I(x, y)))
				return tiles[x, y, layer].Tile;
			return null;
		}

		/// <summary>Gets the tile in the grid at the specified location.</summary>
		public TileDataInstance GetTile(Point2I location, int layer) {
			if (Bounds.Contains(location))
				return tiles[location.X, location.Y, layer - startLayer].Tile;
			return null;
		}

		/// <summary>Gets the action tile at the specified index in the lists.</summary>
		public ActionTileDataInstance GetActionTileAt(int index) {
			return actionTiles[index].Action;
		}
		


		/// <summary>Gets all tiles and action tiles in the grid.</summary>
		public IEnumerable<BaseTileDataInstance> GetAllTiles() {
			// Iterate tiles.
			for (int x = 0; x < size.X; x++) {
				for (int y = 0; y < size.Y; y++) {
					for (int i = 0; i < layerCount; i++) {
						TileInstanceLocation tile = tiles[x, y, i];
						if (tile.Tile != null)
							yield return tile.Tile;
					}
				}
			}

			// Iterate action tiles.
			foreach (ActionTileInstancePosition actionTile in actionTiles) {
				yield return actionTile.Action;
			}
		}

		/// <summary>Gets all tiles and action tiles in the grid and positions them at
		/// their tile grid location and position.</summary>
		public IEnumerable<BaseTileDataInstance> GetAllTilesAtLocation() {
			// Iterate tiles.
			for (int x = 0; x < size.X; x++) {
				for (int y = 0; y < size.Y; y++) {
					for (int i = 0; i < layerCount; i++) {
						TileInstanceLocation tile = tiles[x, y, i];
						if (tile.Tile != null) {
							tile.Tile.Location = tile.Location;
							yield return tile.Tile;
						}
					}
				}
			}

			// Iterate action tiles.
			foreach (ActionTileInstancePosition actionTile in actionTiles) {
				actionTile.Action.Position = actionTile.Position;
				yield return actionTile.Action;
			}
		}

		/// <summary>Gets all tiles in the grid.</summary>
		public IEnumerable<TileDataInstance> GetTiles() {
			// Iterate tiles.
			for (int x = 0; x < size.X; x++) {
				for (int y = 0; y < size.Y; y++) {
					for (int i = 0; i < layerCount; i++) {
						TileInstanceLocation tile = tiles[x, y, i];
						if (tile.Tile != null)
							yield return tile.Tile;
					}
				}
			}
		}

		/// <summary>Gets all tiles in the grid and positions them at their tile
		/// grid location.</summary>
		public IEnumerable<TileDataInstance> GetTilesAtLocation() {
			// Iterate tiles.
			for (int x = size.X - 1; x >= 0; x--) {
				for (int y = size.Y - 1; y >= 0; y--) {
					for (int i = 0; i < layerCount; i++) {
						TileInstanceLocation tile = tiles[x, y, i];
						if (tile.Tile != null) {
							tile.Tile.Location = tile.Location;
							yield return tile.Tile;
						}
					}
				}
			}
		}

		/// <summary>Gets all tiles in the grid along with their locations.</summary>
		public IEnumerable<TileInstanceLocation> GetTilesAndLocations() {
			// Iterate tiles.
			for (int x = size.X - 1; x >= 0; x--) {
				for (int y = size.Y - 1; y >= 0; y--) {
					for (int i = 0; i < layerCount; i++) {
						TileInstanceLocation tile = tiles[x, y, i];
						if (tile.Tile != null)
							yield return tile;
					}
				}
			}
		}

		/// <summary>Gets all action tiles in the grid.</summary>
		public IEnumerable<ActionTileDataInstance> GetActionTiles() {
			// Iterate action tiles.
			foreach (ActionTileInstancePosition actionTile in actionTiles) {
				yield return actionTile.Action;
			}
		}

		/// <summary>Gets all action tiles in the grid and positions them at their
		/// tile grid position.</summary>
		public IEnumerable<ActionTileDataInstance> GetActionTilesAtPosition() {
			// Iterate action tiles.
			foreach (ActionTileInstancePosition actionTile in actionTiles) {
				actionTile.Action.Position = actionTile.Position;
				yield return actionTile.Action;
			}
		}

		/// <summary>Gets all action tiles in the grid along with their position
		/// information.</summary>
		public IEnumerable<ActionTileInstancePosition> GetActionTilesAndPositions() {
			return actionTiles;
		}


		//-----------------------------------------------------------------------------
		// Mutators
		//-----------------------------------------------------------------------------

		// Tiles ----------------------------------------------------------------------

		/// <summary>Places the tile at the specified location in the tile grid.</summary>
		public void PlaceTile(TileDataInstance tile, int x, int y, int layer) {
			PlaceTile(tile, new Point2I(x, y), layer);
		}

		/// <summary>Places the tile at the specified location in the tile grid.</summary>
		public void PlaceTile(TileDataInstance tile, Point2I location, int layer) {
			// Remove existing tile
			TileInstanceLocation t = tiles[location.X, location.Y, layer - startLayer];
			if (t.Tile != null)
				RemoveTile(t);
			tiles[location.X, location.Y, layer - startLayer].Tile = tile;
		}

		/// <summary>Removes the tile at the specified location from the tile grid.</summary>
		public void RemoveTile(int x, int y, int layer) {
			TileInstanceLocation tile = tiles[x, y, layer - startLayer];
			if (tile.Tile != null)
				RemoveTile(tile);
		}

		/// <summary>Removes the tile at the specified location from the tile grid.</summary>
		public void RemoveTile(Point2I location, int layer) {
			TileInstanceLocation tile = tiles[location.X, location.Y, layer - startLayer];
			if (tile.Tile != null)
				RemoveTile(tile);
		}

		/// <summary>Removes the specified tile from the tile grid.</summary>
		public void RemoveTile(TileDataInstance tile) {
			if (tile == null)
				return;
			for (int x = 0; x < size.X; x++) {
				for (int y = 0; y < size.Y; y++) {
					for (int i = 0; i < layerCount; i++) {
						TileInstanceLocation t = tiles[x, y, i];
						if (t.Tile == tile) {
							RemoveTile(t);
							return;
						}
					}
				}
			}
		}

		/// <summary>Internally clears the tile grid tile.</summary>
		private void RemoveTile(TileInstanceLocation tile) {
			tiles[tile.Location.X, tile.Location.Y, tile.Layer - startLayer].Clear();
		}
		
		// Actions --------------------------------------------------------------------

		/// <summary>Places the action tile at the specified position in the tile grid.</summary>
		public void PlaceActionTile(ActionTileDataInstance actionTile, int x, int y) {
			PlaceActionTile(actionTile, new Point2I(x, y));
		}

		/// <summary>Places the action tile at the specified position in the tile grid.</summary>
		public void PlaceActionTile(ActionTileDataInstance actionTile,
			Point2I position)
		{
			actionTiles.Add(new ActionTileInstancePosition(actionTile, position.X, position.Y));
		}

		/// <summary>Removes the specified action tile from the tile grid.</summary>
		public void RemoveActionTile(ActionTileDataInstance actionTile) {
			int index = actionTiles.FindIndex(tile => tile.Action == actionTile);
			if (index != -1)
				actionTiles.RemoveAt(index);
		}

		// General --------------------------------------------------------------------

		/// <summary>Removes the tile or action tile from the tile grid.</summary>
		public void Remove(BaseTileDataInstance tile) {
			if (tile is TileDataInstance)
				RemoveTile((TileDataInstance) tile);
			else if (tile is ActionTileDataInstance)
				RemoveActionTile((ActionTileDataInstance) tile);
		}

		/// <summary>Clears the tile grid.</summary>
		public void Clear() {
			for (int x = 0; x < size.X; x++) {
				for (int y = 0; y < size.Y; y++) {
					for (int i = 0; i < layerCount; i++) {
						tiles[x, y, i].Clear();
					}
				}
			}
			actionTiles.Clear();
		}

		/// <summary>Return a copy of this tile grid.</summary>
		public TileGrid Duplicate() {
			TileGrid duplicate = new TileGrid(this);

			if (duplicate.includeTiles) {
				// Duplicate tiles
				for (int x = 0; x < size.X; x++) {
					for (int y = 0; y < size.Y; y++) {
						for (int i = 0; i < layerCount; i++) {
							TileInstanceLocation tile = tiles[x, y, i];
							if (tile.Tile != null) {
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
				foreach (ActionTileInstancePosition actionTile in actionTiles) {
					ActionTileDataInstance actionTileCopy =
						new ActionTileDataInstance();
					actionTileCopy.Clone(actionTile.Action);
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

		/// <summary>Loads the TileGridReference from the clipboard and dereferences
		/// it.</summary>
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

		/// <summary>Gets if this tile grid includes tiles in its selection.</summary>
		public bool IncludesTiles {
			get { return includeTiles; }
		}

		/// <summary>Gets if this tile grid includes action tiles in its selection.</summary>
		public bool IncludesActions {
			get { return includeActions; }
		}

		/// <summary>Gets the starting layer of the tiles in the tile grid.</summary>
		public int StartLayer {
			get { return startLayer; }
		}

		/// <summary>Gets the ending layer of the tiles in the tile grid.</summary>
		public int EndLayer {
			get { return startLayer + layerCount - 1; }
		}

		/// <summary>Gets the number of layers of the tiles in the tile grid.</summary>
		public int LayerCount {
			get { return layerCount; }
		}

		/// <summary>Gets the number of actions in the tile grid.</summary>
		public int ActionCount {
			get { return actionTiles.Count; }
		}

		/// <summary>Gets the tile width of the tile grid.</summary>
		public int Width {
			get { return size.X; }
		}

		/// <summary>Gets the tile height of the tile grid.</summary>
		public int Height {
			get { return size.Y; }
		}

		/// <summary>Gets the tile size of the tile grid.</summary>
		public Point2I Size {
			get { return size; }
		}

		/// <summary>Gets the tile boundaries of the tile grid.</summary>
		public Rectangle2I Bounds {
			get { return new Rectangle2I(size); }
		}
	}
}
