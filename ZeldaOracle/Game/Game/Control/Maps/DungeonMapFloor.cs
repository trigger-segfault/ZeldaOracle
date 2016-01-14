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
		private DungeonFloor dungeonFloor;
		private Point2I size;
		private int floorNumber; // 2 = "3F", 1 = "2F", 0 = "1F", -1 = "B1F", -2 = "B2F", etc.
		private bool isBossFloor;
		private bool isDiscovered;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public DungeonMapFloor(DungeonFloor dungeonFloor) {
			this.dungeonFloor	= dungeonFloor;
			this.floorNumber	= dungeonFloor.FloorNumber;
			this.isDiscovered	= dungeonFloor.IsDiscovered;
			this.isBossFloor	= dungeonFloor.IsBossFloor;
			this.size			= new Point2I(8, 8);


			// Create rooms.
			rooms = new DungeonMapRoom[size.X, size.Y];
			
			if (dungeonFloor.Level !=  null) {
				for (int x = 0; x < size.X; x++) {
					for (int y = 0; y < size.Y; y++) {
						Point2I loc = new Point2I(x, y);
						if (dungeonFloor.Level.ContainsRoom(loc))
							rooms[x, y] = DungeonMapRoom.Create(dungeonFloor.Level.GetRoomAt(loc), this);
						else
							rooms[x, y] = null;
					}
				}
			}
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public DungeonMapRoom[,] Rooms {
			get { return rooms; }
		}

		public DungeonFloor DungeonFloor {
			get { return dungeonFloor; }
		}

		public int FloorNumber {
			get { return floorNumber; }
		}
		
		public bool IsDiscovered {
			get { return isDiscovered; }
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
