using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Game.Tiles.Clipboard {
	/// <summary>A reference to a tile instance that can be safetly copied
	/// to the clipboard.</summary>
	[Serializable]
	public class TileInstanceReference
		: BaseTileInstanceReference<TileData, TileDataInstance>
	{
		/// <summary>The location of the tile in the clipboard.</summary>
		private Point2I location;
		/// <summary>The layer of the tile in the clipboard.</summary>
		private int layer;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Constructs a reference to the tile instance.</summary>
		public TileInstanceReference(TileDataInstance tile) : base(tile) {
			location	= tile.Location;
			layer		= tile.Layer;
		}

		/// <summary>Constructs a reference to the tile instance with the temporary
		/// location.</summary>
		public TileInstanceReference(TileInstanceLocation tile) : base(tile.Tile) {
			location	= tile.Location;
			layer		= tile.Layer;
		}

		/// <summary>Constructs a reference to the tile instance with the custom
		/// location.</summary>
		public TileInstanceReference(TileDataInstance tile, Point2I location,
			int layer) : base(tile)
		{
			this.location	= location;
			this.layer		= layer;
		}


		//-----------------------------------------------------------------------------
		// Override Methods
		//-----------------------------------------------------------------------------

		/// <summary>Creates the tile data instance and sets up any extended members.</summary>
		protected override TileDataInstance SetupInstance(TileData data) {
			TileDataInstance tileInstance = new TileDataInstance(data);
			tileInstance.Location	= location;
			tileInstance.Layer		= layer;
			return tileInstance;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets the location of the tile in the clipboard.</summary>
		public Point2I Location {
			get { return location; }
		}

		/// <summary>Gets the layer of the tile in the clipboard.</summary>
		public int Layer {
			get { return layer; }
		}
	}
}
