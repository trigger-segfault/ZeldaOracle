using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Game.Worlds {
	
	// A tile coordinate inside a level.
	public struct LevelTileCoord {

		public int X;
		public int Y;

		public LevelTileCoord(int x, int y) {
			this.X = x;
			this.Y = y;
		}
		
		public static explicit operator Point2I(LevelTileCoord levelCoord) {
			return new Point2I(levelCoord.X, levelCoord.Y);
		}
		
		public static explicit operator LevelTileCoord(Point2I point) {
			return new LevelTileCoord(point.X, point.Y);
		}

		public Point2I AbsoluteTileLocation {
			get { return new Point2I(X, Y); }
		}
	}

}
