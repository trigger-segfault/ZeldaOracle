using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Game.Worlds {
	public class World {

		private List<Level> levels;
		private int startLevelIndex;
		private Point2I startRoomLocation;
		private Point2I startTileLocation;

		
		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public World() {
			levels = new List<Level>();
		}


		//-----------------------------------------------------------------------------
		// Accessors
		//-----------------------------------------------------------------------------
		
		public bool ExistsLevel(string levelID) {
			return (GetLevel(levelID) != null);
		}

		public Level GetLevel(int index) {
			return levels[index];
		}

		public Level GetLevel(string levelID) {
			for (int i = 0; i < levels.Count; i++) {
				if (levels[i].Name == levelID)
					return levels[i];
			}
			return null;
		}

		public int GetLevelIndex(string levelID) {
			for (int i = 0; i < levels.Count; i++) {
				if (levels[i].Name == levelID)
					return i;
			}
			return -1;
		}


		//-----------------------------------------------------------------------------
		// Mutators
		//-----------------------------------------------------------------------------

		public void AddLevel(Level level) {
			levels.Add(level);
		}

		public void RemoveLevel(string levelID) {
			for (int i = 0; i < levels.Count; i++) {
				if (levels[i].Name == levelID) {
					levels.RemoveAt(i);
					break;
				}
			}
		}
			
		public void RemoveLevel(Level level) {
			levels.Remove(level);
		}

		public void RemoveLevelAt(int index) {
			levels.RemoveAt(index);
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------
		
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

		public int levelCount {
			get { return levels.Count; }
		}
	}
}
