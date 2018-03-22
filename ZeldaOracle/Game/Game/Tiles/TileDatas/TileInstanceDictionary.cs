using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Worlds;

namespace ZeldaOracle.Game.Tiles {
	/// <summary>A dictionary for storing tiles at a location.</summary>
	public class TileInstanceDictionary : IEnumerable<TileInstanceLocation> {

		/// <summary>The dictionary array for each tile location and layer.</summary>
		private Dictionary<Point2I, TileDataInstance>[] tileLocations;
		/// <summary>The reference to a single layer dictionary.</summary>
		private Dictionary<Point2I, TileDataInstance> singleLayerLocations;
		/// <summary>The starting layer for the dictionary.</summary>
		private int startLayer;
		/// <summary>The total number of layers in the dictionary.</summary>
		private int layerCount;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		/// <summary>Constructs a tile instance dictionary for a single layer.</summary>
		public TileInstanceDictionary(int layer) : this(layer, 1) {
		}

		/// <summary>Constructs a tile instance dictionary with the specified layers.</summary>
		public TileInstanceDictionary(int startLayer, int layerCount) {
			this.startLayer = startLayer;
			this.layerCount = layerCount;

			tileLocations = new Dictionary<Point2I, TileDataInstance>[layerCount];
			for (int i = 0; i < layerCount; i++)
				tileLocations[i] = new Dictionary<Point2I, TileDataInstance>();
			if (layerCount == 1)
				singleLayerLocations = tileLocations[0];
		}

		/// <summary>Constructs a tile instance dictionary for all layers in the level.</summary>
		public TileInstanceDictionary(Level level) : this(0, level.RoomLayerCount) {
		}


		//-----------------------------------------------------------------------------
		// IEnumerable
		//-----------------------------------------------------------------------------

		/// <summary>Gets all tiles and their locations in the dictionary.</summary>
		public IEnumerable<TileInstanceLocation> GetTiles() {
			for (int layer = 0; layer < layerCount; layer++) {
				foreach (var pair in tileLocations[layer]) {
					yield return new TileInstanceLocation(
						pair.Value, pair.Key, layer + startLayer);
				}
			}
		}

		/// <summary>Gets all tiles and their locations in the dictionary.</summary>
		public IEnumerator<TileInstanceLocation> GetEnumerator() {
			for (int layer = 0; layer < layerCount; layer++) {
				foreach (var pair in tileLocations[layer]) {
					yield return new TileInstanceLocation(
						pair.Value, pair.Key, layer + startLayer);
				}
			}
		}

		/// <summary>Gets all tiles and their locations in the dictionary.</summary>
		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}


		//-----------------------------------------------------------------------------
		// Accessors
		//-----------------------------------------------------------------------------
		
		/// <summary>Returns true if a tile at the specified location and layer exists.</summary>
		public bool Contains(int x, int y, int layer) {
			return Contains(new Point2I(x, y), layer);
		}

		/// <summary>Returns true if a tile at the specified location and layer exists.</summary>
		public bool Contains(Point2I location, int layer) {
			return tileLocations[layer - startLayer].ContainsKey(location);
		}

		/// <summary>Returns true if a tile at the specified location exists.
		/// This can only be used when the dictionary consists of a single layer.</summary>
		public bool Contains(int x, int y) {
			return Contains(new Point2I(x, y));
		}

		/// <summary>Returns true if a tile at the specified location exists.
		/// This can only be used when the dictionary consists of a single layer.</summary>
		public bool Contains(Point2I location) {
			if (singleLayerLocations == null)
				throw SingleLayerException;
			return singleLayerLocations.ContainsKey(location);
		}

		/// <summary>Gets the tile instance at the specified location and layer.</summary>
		public TileDataInstance Get(int x, int y, int layer) {
			return Get(new Point2I(x, y), layer);
		}

		/// <summary>Gets the tile instance at the specified location and layer.</summary>
		public TileDataInstance Get(Point2I location, int layer) {
			return tileLocations[layer - startLayer][location];
		}

		/// <summary>Gets the tile instance at the specified location.
		/// This can only be used when the dictionary consists of a single layer.</summary>
		public TileDataInstance Get(int x, int y) {
			return Get(new Point2I(x, y));
		}

		/// <summary>Gets the tile instance at the specified location.
		/// This can only be used when the dictionary consists of a single layer.</summary>
		public TileDataInstance Get(Point2I location) {
			if (singleLayerLocations == null)
				throw SingleLayerException;
			return singleLayerLocations[location];
		}


		//-----------------------------------------------------------------------------
		// Mutators
		//-----------------------------------------------------------------------------

		/// <summary>Clears all tiles from the dictionary.</summary>
		public void Clear() {
			for (int layer = 0; layer < layerCount; layer++) {
				tileLocations[layer].Clear();
			}
		}

		/// <summary>Adds the tile instance to the specified location and layer.</summary>
		public void Add(TileInstanceLocation tile) {
			Add(tile.Location, tile.Layer, tile.Tile);
		}

		/// <summary>Adds the tile instance to the specified location and layer.</summary>
		public void Add(TileDataInstance tile) {
			Add(tile.Location, tile.Layer, tile);
		}

		/// <summary>Adds the tile instance to the specified location and layer.</summary>
		public void Add(int x, int y, int layer, TileDataInstance tile) {
			Add(new Point2I(x, y), layer, tile);
		}

		/// <summary>Adds the tile instance to the specified location and layer.</summary>
		public void Add(Point2I location, int layer, TileDataInstance tile) {
			tileLocations[layer - startLayer].Add(location, tile);
		}

		/// <summary>Adds the tile instance to the specified location.
		/// This can only be used when the dictionary consists of a single layer.</summary>
		public void Add(int x, int y, TileDataInstance tile) {
			Add(new Point2I(x, y), tile);
		}

		/// <summary>Adds the tile instance to the specified location.
		/// This can only be used when the dictionary consists of a single layer.</summary>
		public void Add(Point2I location, TileDataInstance tile) {
			if (singleLayerLocations == null)
				throw SingleLayerException;
			singleLayerLocations.Add(location, tile);
		}

		/// <summary>Sets the tile instance at the specified location and layer.</summary>
		public void Set(TileInstanceLocation tile) {
			Set(tile.Location, tile.Layer, tile.Tile);
		}

		/// <summary>Sets the tile instance at the specified location and layer.</summary>
		public void Set(TileDataInstance tile) {
			Set(tile.LevelCoord, tile.Layer, tile);
		}

		/// <summary>Sets the tile instance at the specified location and layer.</summary>
		public void Set(int x, int y, int layer, TileDataInstance tile) {
			Set(new Point2I(x, y), layer, tile);
		}

		/// <summary>Sets the tile instance at the specified location and layer.</summary>
		public void Set(Point2I location, int layer, TileDataInstance tile) {
			tileLocations[layer - startLayer][location] = tile;
		}

		/// <summary>Sets the tile instance at the specified location.
		/// This can only be used when the dictionary consists of a single layer.</summary>
		public void Set(int x, int y, TileDataInstance tile) {
			Set(new Point2I(x, y), tile);
		}

		/// <summary>Sets the tile instance at the specified location.
		/// This can only be used when the dictionary consists of a single layer.</summary>
		public void Set(Point2I location, TileDataInstance tile) {
			if (singleLayerLocations == null)
				throw SingleLayerException;
			singleLayerLocations[location] = tile;
		}


		//-----------------------------------------------------------------------------
		// Operators
		//-----------------------------------------------------------------------------

		/// <summary>Gets the tile instance at the specified location and layer.</summary>
		public TileDataInstance this[int x, int y, int layer] {
			get { return this[new Point2I(x, y), layer]; }
			set { this[new Point2I(x, y), layer] = value; }
		}

		/// <summary>Gets the tile instance at the specified location and layer.</summary>
		public TileDataInstance this[Point2I location, int layer] {
			get { return tileLocations[layer - startLayer][location]; }
			set { tileLocations[layer - startLayer][location] = value; }
		}

		/// <summary>Gets the tile instance at the specified location.
		/// This can only be used when the dictionary consists of a single layer.</summary>
		public TileDataInstance this[int x, int y] {
			get { return this[new Point2I(x, y)]; }
			set { this[new Point2I(x, y)] = value; }
		}

		/// <summary>Gets the tile instance at the specified location.
		/// This can only be used when the dictionary consists of a single layer.</summary>
		public TileDataInstance this[Point2I location] {
			get {
				if (singleLayerLocations == null)
					throw SingleLayerException;
				return singleLayerLocations[location];
			}
			set {
				if (singleLayerLocations == null)
					throw SingleLayerException;
				singleLayerLocations[location] = value;
			}
		}


		//-----------------------------------------------------------------------------
		// Internal Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets an exception to throw when a single layer method cannot
		/// be called.</summary>
		private Exception SingleLayerException {
			get {
				return new InvalidOperationException("TileInstanceDictionary " +
					"methods without a layer parameters can only be called " +
					"when the dictionary consists of a single layer!");
			}
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets the number of tiles in the dictionary.</summary>
		public int Count {
			get {
				int count = 0;
				for (int i = 0; i < layerCount; i++)
					count += tileLocations[i].Count;
				return count;
			}
		}

		/// <summary>Gets the layer the dictionary starts on.</summary>
		public int StartLayer {
			get { return startLayer; }
		}

		/// <summary>Gets the layer the dictionary ends on.</summary>
		public int EndLayer {
			get { return startLayer + layerCount - 1; }
		}

		/// <summary>Gets the total number of layers in the dictionary.</summary>
		public int LayerCount {
			get { return layerCount; }
		}
	}
}
