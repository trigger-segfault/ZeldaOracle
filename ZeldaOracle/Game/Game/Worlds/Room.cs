using ZeldaOracle.Common.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Game.Tiles;

namespace ZeldaOracle.Game.Worlds {
	public class Room {
		
		private Level			level;		// The level this room is in.
		private Point2I			location;	// Location within the level.
		private Point2I			size;		// Size of the tile grid.
		private int				layerCount;	// Number of tile layers.
		private TileData[,,]	tileData;	// 3D grid of tile data (x, y, layer)
		private Zone			zone;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public Room(Level level, int x, int y) {
			this.level		= level;
			this.location	= new Point2I(x, y);
			this.size		= level.RoomSize;
			this.layerCount	= GameSettings.DEFAULT_TILE_LAYER_COUNT; // Default tile layers.
			this.tileData	= new TileData[size.X, size.Y, layerCount];
			this.zone		= null;
		}
		

		//-----------------------------------------------------------------------------
		// Mutators
		//-----------------------------------------------------------------------------


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public TileData[,,] TileData {
			get { return tileData; }
			set { tileData = value; }
		}

		public Level Level {
			get { return level; }
			set { level = value; }
		}

		public Point2I Location {
			get { return location; }
			set { location = value; }
		}

		public Point2I Size {
			get { return size; }
		}

		public int Width {
			get { return size.X; }
		}

		public int Height {
			get { return size.Y; }
		}

		public int LayerCount {
			get { return layerCount; }
		}

		public Zone Zone {
			get { return zone; }
			set { zone = value; }
		}
	}
}
