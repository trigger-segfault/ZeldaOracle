using ZeldaOracle.Common.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeldaOracle.Game.Worlds {
	public class Level {
		private string id;
		private string name;

		private Point2I dimensions;
		private Point2I roomSize;
		private Room[,] rooms;

		private World world;



		public void ResizeRooms(Point2I size) {

		}
	}
}
