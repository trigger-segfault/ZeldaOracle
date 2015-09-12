using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Worlds;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Game.Control {
	public class TileGrid {
		private Tile[,,] tiles;
		private Point2I size;
		private int layerCount;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public TileGrid() {
			
		}


		//-----------------------------------------------------------------------------
		// Mutators
		//-----------------------------------------------------------------------------

		public void EnterRoom(Room room) {
			size.X = room.Width;
			size.Y = room.Height;

			tiles = new Tile[size.X, size.Y, layerCount];


		}

		
		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public int LayerCount {
			get { return layerCount; }
		}
	}
}
