using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Game.Tiles.Clipboard {
	[Serializable]
	public class TileDataReference :
		BaseTileDataReference<TileData, TileDataInstance>
	{
		/// <summary>The location of the tile in the clipboard.</summary>
		private Point2I location;
		/// <summary>The layer of the tile in the clipboard.</summary>
		private int layer;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public TileDataReference(TileDataInstance tileData) : base(tileData) {
			this.location	= tileData.Location;
			this.layer		= tileData.Layer;
		}


		//-----------------------------------------------------------------------------
		// Override Methods
		//-----------------------------------------------------------------------------

		/// <summary>Creates the tile data instance and sets up any extended members.</summary>
		protected override TileDataInstance SetupInstance(TileData data) {
			TileDataInstance tileData = new TileDataInstance(data);
			tileData.Location	= location;
			tileData.Layer		= layer;
			return tileData;
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
