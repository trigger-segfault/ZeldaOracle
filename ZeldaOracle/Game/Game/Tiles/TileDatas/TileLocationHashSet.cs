using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Worlds;

namespace ZeldaOracle.Game.Tiles {
	/// <summary>A hashset for storing tile locations.</summary>
	public class TileLocationHashSet : IEnumerable<TileLocation> {

		/// <summary>The dictionary array for each tile location and layer.</summary>
		private HashSet<Point2I>[] tileLocations;
		/// <summary>The reference to a single layer dictionary.</summary>
		private HashSet<Point2I> singleLayerLocations;
		/// <summary>The starting layer for the dictionary.</summary>
		private int startLayer;
		/// <summary>The total number of layers in the dictionary.</summary>
		private int layerCount;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		/// <summary>Constructs a tile location hash set for a single layer.</summary>
		public TileLocationHashSet(int layer) : this(layer, 1) {
		}

		/// <summary>Constructs a tile location hash set with the specified layers.</summary>
		public TileLocationHashSet(int startLayer, int layerCount) {
			this.startLayer = startLayer;
			this.layerCount = layerCount;

			tileLocations = new HashSet<Point2I>[layerCount];
			for (int i = 0; i < layerCount; i++)
				tileLocations[i] = new HashSet<Point2I>();
			if (layerCount == 1)
				singleLayerLocations = tileLocations[0];
		}

		/// <summary>Constructs a tile location hash set for all layers in the level.</summary>
		public TileLocationHashSet(Level level) : this(0, level.RoomLayerCount) {
		}


		//-----------------------------------------------------------------------------
		// IEnumerable
		//-----------------------------------------------------------------------------

		/// <summary>Gets all tile locations in the hash set.</summary>
		public IEnumerable<TileLocation> GetTiles() {
			for (int layer = 0; layer < layerCount; layer++) {
				foreach (Point2I location in tileLocations[layer]) {
					yield return new TileLocation(location, layer + startLayer);
				}
			}
		}

		/// <summary>Gets all tile locations in the hash set.</summary>
		public IEnumerator<TileLocation> GetEnumerator() {
			for (int layer = 0; layer < layerCount; layer++) {
				foreach (Point2I location in tileLocations[layer]) {
					yield return new TileLocation(location, layer + startLayer);
				}
			}
		}

		/// <summary>Gets all tile locations in the hash set.</summary>
		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}


		//-----------------------------------------------------------------------------
		// Accessors
		//-----------------------------------------------------------------------------
		
		/// <summary>Returns true if a tile location and layer exists.</summary>
		public bool Contains(int x, int y, int layer) {
			return Contains(new Point2I(x, y), layer);
		}

		/// <summary>Returns true if a tile location and layer exists.</summary>
		public bool Contains(Point2I location, int layer) {
			return tileLocations[layer - startLayer].Contains(location);
		}

		/// <summary>Returns true if a tile location and layer exists.
		/// This can only be used when the hash set consists of a single layer.</summary>
		public bool Contains(int x, int y) {
			return Contains(new Point2I(x, y));
		}

		/// <summary>Returns true if a tile location and layer exists.
		/// This can only be used when the hash set consists of a single layer.</summary>
		public bool Contains(Point2I location) {
			if (singleLayerLocations == null)
				throw SingleLayerException;
			return singleLayerLocations.Contains(location);
		}


		//-----------------------------------------------------------------------------
		// Mutators
		//-----------------------------------------------------------------------------

		/// <summary>Clears all tile locations from the hash set.</summary>
		public void Clear() {
			for (int layer = 0; layer < layerCount; layer++) {
				tileLocations[layer].Clear();
			}
		}

		/// <summary>Adds the tile instance to the specified location and layer.</summary>
		public bool Add(TileInstanceLocation tile) {
			return Add(tile.Location, tile.Layer);
		}

		/// <summary>Adds the tile instance to the specified location and layer.</summary>
		public bool Add(TileDataInstance tile) {
			return Add(tile.Location, tile.Layer);
		}

		/// <summary>Adds the tile instance to the specified location and layer.</summary>
		public bool Add(int x, int y, int layer) {
			return Add(new Point2I(x, y), layer);
		}

		/// <summary>Adds the tile instance to the specified location and layer.</summary>
		public bool Add(Point2I location, int layer) {
			return tileLocations[layer - startLayer].Add(location);
		}

		/// <summary>Adds the tile instance to the specified location.
		/// This can only be used when the hash set consists of a single layer.</summary>
		public bool Add(int x, int y) {
			return Add(new Point2I(x, y));
		}

		/// <summary>Adds the tile instance to the specified location.
		/// This can only be used when the hash set consists of a single layer.</summary>
		public bool Add(Point2I location) {
			if (singleLayerLocations == null)
				throw SingleLayerException;
			return singleLayerLocations.Add(location);
		}


		//-----------------------------------------------------------------------------
		// Internal Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets an exception to throw when a single layer method cannot
		/// be called.</summary>
		private Exception SingleLayerException {
			get {
				return new InvalidOperationException("TileLocationHashSet " +
					"methods without a layer parameters can only be called " +
					"when the dictionary consists of a single layer!");
			}
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets the number of tile locations in the hash set.</summary>
		public int Count {
			get {
				int count = 0;
				for (int i = 0; i < layerCount; i++)
					count += tileLocations[i].Count;
				return count;
			}
		}

		/// <summary>Gets the layer the hash set starts on.</summary>
		public int StartLayer {
			get { return startLayer; }
		}

		/// <summary>Gets the layer the hash set ends on.</summary>
		public int EndLayer {
			get { return startLayer + layerCount - 1; }
		}

		/// <summary>Gets the total number of layers in the hash set.</summary>
		public int LayerCount {
			get { return layerCount; }
		}
	}
}
