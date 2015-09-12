using ZeldaOracle.Common.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeldaOracle.Game.Worlds {
	public class Level {
		private string	name;
		private World	world;
		private Point2I roomSize;	// The size in tiles of each room in the level.
		private Point2I dimensions;	// The dimensions of the grid of rooms.
		private Room[,] rooms;		// The grid of rooms.
		
		
		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public Level(int width, int height, Point2I roomSize) {
			this.name		= "";
			this.world		= null;
			this.roomSize	= roomSize;

			Resize(new Point2I(width, height));
		}


		//-----------------------------------------------------------------------------
		// Accessors
		//-----------------------------------------------------------------------------

		public Room GetRoom(Point2I location) {
			return rooms[location.X, location.Y];
		}


		//-----------------------------------------------------------------------------
		// Mutators
		//-----------------------------------------------------------------------------

		// Resize the dimensions of the room grid.
		public void Resize(Point2I size) {
			Room[,] oldRooms = rooms;
			rooms = new Room[size.X, size.Y];

			for (int x = 0; x < size.X; x++) {
				for (int y = 0; y < size.Y; y++) {
					if (oldRooms != null && x < dimensions.X && y < dimensions.Y)
						rooms[x, y] = oldRooms[x, y];
					else
						rooms[x, y] = new Room(this, x, y);
				}
			}

			dimensions = size;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public World World {
			get { return world; }
		}
		
		public string Name {
			get { return name; }
			set { name = value; }
		}
		
		public Room[,] Rooms {
			get { return rooms; }
		}
		
		public Point2I RoomSize {
			get { return roomSize; }
			set { roomSize = value; }
		}
		
		public Point2I Dimensions {
			get { return dimensions; }
			set { dimensions = value; }
		}
		
		public int Width {
			get { return dimensions.X; }
			set { dimensions.X = value; }
		}
		
		public int Height {
			get { return dimensions.Y; }
			set { dimensions.Y = value; }
		}
	}
}
