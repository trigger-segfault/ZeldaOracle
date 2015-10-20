using ZeldaOracle.Common.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Tiles.EventTiles;

namespace ZeldaOracle.Game.Worlds {
	public class Level : IPropertyObject {
		private World		world;
		private Point2I		roomSize;		// The size in tiles of each room in the level.
		private int			roomLayerCount; // The number of tile layers for each room in the level.
		private Point2I		dimensions;		// The dimensions of the grid of rooms.
		private Room[,]		rooms;			// The grid of rooms.
		private Zone		zone;
		private Properties	properties;

		
		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public Level(int width, int height, Point2I roomSize) :
			this("", width, height, GameSettings.DEFAULT_TILE_LAYER_COUNT, roomSize, null)
		{
		}
		
		public Level(string name, int width, int height, int layerCount, Point2I roomSize, Zone zone) {
			this.world			= null;
			this.roomSize		= roomSize;
			this.roomLayerCount = layerCount;
			this.dimensions		= Point2I.Zero;
			this.zone			= zone;
			this.properties		= new Properties();
			this.properties.PropertyObject = this;
			this.properties.BaseProperties = new Properties();

			this.properties.BaseProperties.Set("id", "")
				.SetDocumentation("ID", "", "", "", "The id used to refer to this level.", true, false);

			this.properties.Set("id", name);

			Resize(new Point2I(width, height));
		}


		//-----------------------------------------------------------------------------
		// Accessors
		//-----------------------------------------------------------------------------
		
		public bool ContainsRoom(Point2I location) {
			return (location.X >= 0 && location.Y >= 0 && location.X < dimensions.X && location.Y < dimensions.Y);
		}

		public Room GetRoomAt(int x, int y) {
			return GetRoomAt(new Point2I(x, y));
		}

		public Room GetRoomAt(Point2I location) {
			if (!ContainsRoom(location))
				return null;
			return rooms[location.X, location.Y];
		}

		public EventTileDataInstance FindEventTileByID(string id) {
			for (int x = 0; x < dimensions.X; x++) {
				for (int y = 0; y < dimensions.Y; y++) {
					EventTileDataInstance eventTile = rooms[x, y].FindEventTileByID(id);
					if (eventTile != null)
						return eventTile;
				}
			}
			return null;
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
						rooms[x, y] = new Room(this, x, y, zone ?? GameData.ZONE_DEFAULT);
				}
			}

			dimensions = size;
		}

		// Shift the room grid.
		public void Shift(Point2I distance) {
			Room[,] oldRooms = rooms;
			rooms = new Room[dimensions.X, dimensions.Y];

			for (int x = 0; x < dimensions.X; x++) {
				for (int y = 0; y < dimensions.Y; y++) {
					if (x - distance.X >= 0 && x - distance.X < dimensions.X &&
						y - distance.Y >= 0 && y - distance.Y < dimensions.Y)
						rooms[x, y] = oldRooms[x - distance.X, y - distance.Y];
					else
						rooms[x, y] = new Room(this, x, y, zone ?? Resources.GetResource<Zone>(""));
				}
			}
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public World World {
			get { return world; }
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
		
		public int RoomWidth {
			get { return roomSize.X; }
			set { roomSize.X = value; }
		}
		
		public int RoomHeight {
			get { return roomSize.Y; }
			set { roomSize.Y = value; }
		}
		
		public int RoomLayerCount {
			get { return roomLayerCount; }
			set { roomLayerCount = value; }
		}

		public Properties Properties {
			get { return properties; }
			set {
				properties = value;
				properties.PropertyObject = this;
			}
		}

		public Zone Zone {
			get { return zone; }
			set { zone = value; }
		}
	}
}
