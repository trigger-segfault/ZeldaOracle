using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Game.Worlds {
	public class World {

		private List<Level> levels;
		private int currentLevel;
		private int startLevelIndex;
		private Point2I startRoomLocation;
		private Point2I startTileLocation;

		
		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public World() {
			levels = new List<Level>();
			currentLevel = 0;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------
		
		public Level CurrentLevel {
			get { return levels[currentLevel]; }
		}

		public List<Level> Levels {
			get { return levels; }
		}

		public Room StartRoom {
			get { return levels[startLevelIndex].GetRoom(startRoomLocation); }
		}

		public int StartLevelIndex {
			get { return startLevelIndex; }
			set { startLevelIndex = value; }
		}

		public Point2I StartRoomLocation {
			get { return startRoomLocation; }
			set { startRoomLocation = value; }
		}

		public Point2I StartTileLocation {
			get { return startTileLocation; }
			set { startTileLocation = value; }
		}
	}
}
