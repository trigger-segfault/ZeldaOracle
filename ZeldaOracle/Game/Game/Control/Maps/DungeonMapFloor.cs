using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.GameStates;
using ZeldaOracle.Game.Main;
using ZeldaOracle.Game.Worlds;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Tiles.Custom;

namespace ZeldaOracle.Game.Control.Maps {
	
	public class DungeonMapFloor {

		private DungeonMapRoom[,] rooms;
		private Level level;
		private Point2I size;
		private int floorNumber; // 2 = "3F", 1 = "2F", 0 = "1F", -1 = "B1F", -2 = "B2F", etc.
		private bool isBossFloor;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public DungeonMapFloor(Level level, int floorNumber) {
			this.level			= level;
			this.floorNumber	= floorNumber;
			this.size			= new Point2I(8, 8);
			this.isBossFloor	= false;

			// Create rooms.
			rooms = new DungeonMapRoom[size.X, size.Y];
			for (int x = 0; x < size.X; x++) {
				for (int y = 0; y < size.Y; y++) {
					Point2I loc = new Point2I(x, y);
					if (level.ContainsRoom(loc))
						rooms[x, y] = DungeonMapRoom.Create(level.GetRoomAt(x, y));
					else
						rooms[x, y] = null;
				}
			}
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public DungeonMapRoom[,] Rooms {
			get { return rooms; }
		}

		public int FloorNumber {
			get { return floorNumber; }
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
		
		public bool IsBossFloor {
			get { return isBossFloor; }
		}
		
		public string FloorNumberText {
			get {
				if (floorNumber < 0)
					return ("B" + (-floorNumber) + "F");
				else
					return  (" " + (floorNumber + 1) + "F");
			}
		}
	}

}
