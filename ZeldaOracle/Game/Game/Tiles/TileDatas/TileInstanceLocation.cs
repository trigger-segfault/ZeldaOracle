using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Game.Tiles {
	/// <summary>Stores information on a tile at a temporary location.</summary>
	public struct TileInstanceLocation {
		/// <summary>The tile contained at this location. Can be null.</summary>
		public TileDataInstance Tile { get; set; }
		/// <summary>The temporary location of this tile.</summary>
		public Point2I Location { get; set; }
		/// <summary>The temporary layer of this tile.</summary>
		public int Layer { get; set; }


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		/// <summary>Constructs the tile instance location with no tile.</summary>
		public TileInstanceLocation(int x, int y, int layer) {
			Tile		= null;
			Location	= new Point2I(x, y);
			Layer		= layer;
		}

		/// <summary>Constructs the tile instance location with no tile.</summary>
		public TileInstanceLocation(Point2I location, int layer) {
			Tile		= null;
			Location	= location;
			Layer		= layer;
		}

		/// <summary>Constructs the tile instance location with the specified tile.</summary>
		public TileInstanceLocation(TileDataInstance tile, int x, int y, int layer) {
			Tile		= tile;
			Location	= new Point2I(x, y);
			Layer		= layer;
		}

		/// <summary>Constructs the tile instance location with the specified tile.</summary>
		public TileInstanceLocation(TileDataInstance tile, Point2I location,
			int layer)
		{
			Tile		= tile;
			Location	= location;
			Layer		= layer;
		}


		//-----------------------------------------------------------------------------
		// Mutators
		//-----------------------------------------------------------------------------

		/// <summary>Sets the tile instance for this location.</summary>
		public void Set(TileDataInstance tile) {
			Tile		= tile;
		}

		/// <summary>Clears the tile instance from this location.</summary>
		public void Clear() {
			Tile		= null;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------
		
		/// <summary>Gets if the tile is non-null.</summary>
		public bool HasTile {
			get { return Tile != null; }
		}
	}
}
