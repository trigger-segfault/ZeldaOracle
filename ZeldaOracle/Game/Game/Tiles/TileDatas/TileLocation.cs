using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Game.Tiles {
	/// <summary>A simple structure for storing a tile's location and layer.</summary>
	public struct TileLocation {
		/// <summary>Gets the tile location.</summary>
		public Point2I Location;
		/// <summary>Gets the tile layer.</summary>
		public int Layer;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		/// <summary>Constructs the tile location with the specified location and
		/// a layer of 0.</summary>
		public TileLocation(int x, int y) {
			Location	= new Point2I(x, y);
			Layer		= 0;
		}

		/// <summary>Constructs the tile location with the specified location and
		/// a layer of 0.</summary>
		public TileLocation(Point2I location) {
			Location	= location;
			Layer		= 0;
		}

		/// <summary>Constructs the tile location with the specified location and
		/// layer.</summary>
		public TileLocation(int x, int y, int layer) {
			Location	= new Point2I(x, y);
			Layer		= layer;
		}

		/// <summary>Constructs the tile location with the specified location and
		/// layer.</summary>
		public TileLocation(Point2I location, int layer) {
			Location	= location;
			Layer		= layer;
		}

		/// <summary>Constructs the tile location with the tile's location and
		/// layer.</summary>
		public TileLocation(TileInstanceLocation tile) {
			Location	= tile.Location;
			Layer		= tile.Layer;
		}


		//-----------------------------------------------------------------------------
		// Static Constructors
		//-----------------------------------------------------------------------------

		/// <summary>Constructs a tile location from the tile's room location.</summary>
		public static TileLocation FromRoom(TileDataInstance tile) {
			return new TileLocation(tile.Location, tile.Layer);
		}

		/// <summary>Constructs a tile location from the tile's level coord.</summary>
		public static TileLocation FromLevel(TileDataInstance tile) {
			return new TileLocation(tile.LevelCoord, tile.Layer);
		}


		//-----------------------------------------------------------------------------
		// General
		//-----------------------------------------------------------------------------

		/// <summary>Gets a string representation of the tile location.</summary>
		public override string ToString() {
			return "[" + Location + ", " + Layer + "]";
		}

		/// <summary>Returns true if this tile location is equal to the object.</summary>
		public override bool Equals(object obj) {
			if (obj is TileLocation)
				return this == ((TileLocation) obj);
			return base.Equals(obj);
		}

		/// <summary>Gets the hash code for this tile location.</summary>
		public override int GetHashCode() {
			return Location.GetHashCode() ^ ~Layer;
		}


		//-----------------------------------------------------------------------------
		// Operators
		//-----------------------------------------------------------------------------

		/// <summary>Returns true if the two tile locations are equal.</summary>
		public static bool operator ==(TileLocation a, TileLocation b) {
			return (a.Location == b.Location && a.Layer == b.Layer);
		}

		/// <summary>Returns true if the two tile locations are not equal.</summary>
		public static bool operator !=(TileLocation a, TileLocation b) {
			return (a.Location != b.Location || a.Layer != b.Layer);
		}
	}
}
