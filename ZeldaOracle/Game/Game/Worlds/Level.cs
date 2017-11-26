using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Tiles.EventTiles;

namespace ZeldaOracle.Game.Worlds {

	public enum CreateTileGridMode {
		/**<summary>The tile grid will contain the actual tile instances and remove them from the level.</summary>*/
		Remove,
		/**<summary>The tile grid will contain copies of the tile instances without removing them.</summary>*/
		Duplicate,
		/**<summary>The tile grid will contain the actual tile instances without removing them.</summary>*/
		Twin
	}

	public class Level : IEventObjectContainer, IIDObject {
		private World		world;
		private Point2I		roomSize;		// The size in tiles of each room in the level.
		private int			roomLayerCount; // The number of tile layers for each room in the level.
		private Point2I		dimensions;		// The dimensions of the grid of rooms.
		private Room[,]		rooms;			// The grid of rooms.
		//private Zone		zone;
		private Properties	properties;
		private EventCollection events;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public Level(int width, int height, Point2I roomSize) :
			this("", width, height, GameSettings.DEFAULT_TILE_LAYER_COUNT, roomSize, null) {
		}

		public Level(Point2I dimensions, Point2I roomSize) :
			this("", dimensions, GameSettings.DEFAULT_TILE_LAYER_COUNT, roomSize, null) {
		}

		public Level(string id, int width, int height, int layerCount, Point2I roomSize, Zone zone) :
			this(id, new Point2I(width, height), layerCount, roomSize, zone) {
		}

		public Level(string id, Point2I dimensions, int layerCount, Point2I roomSize, Zone zone) {
			this.world          = null;
			this.roomSize       = roomSize;
			this.roomLayerCount = layerCount;
			this.dimensions     = Point2I.Zero;
			//this.zone			= zone;
			
			this.events			= new EventCollection(this);
			this.properties		= new Properties(this);
			this.properties.BaseProperties	= new Properties();

			properties.BaseProperties.Set("id", "")
				.SetDocumentation("ID", "", "", "General", "The id used to refer to this level.", false, false);

			properties.BaseProperties.Set("dungeon", "")
				.SetDocumentation("Dungeon", "dungeon", "", "Dungeon", "The dungeon this level belongs to.");
			properties.BaseProperties.Set("dungeon_floor", 0)
				.SetDocumentation("Dungeon Floor", "", "", "Dungeon", "The floor in the dungeon this level belongs to.");

			properties.BaseProperties.Set("discovered", false)
				.SetDocumentation("Discovered", "", "", "Progress", "True if the level has been visited at least once.");

			properties.Set("id", id);

			properties.BaseProperties.Set("zone", "")
				.SetDocumentation("Zone", "zone", "", "Level", "The zone type for this room.");

			Zone = zone;

			Resize(dimensions);
		}

		//-----------------------------------------------------------------------------
		// Property/Event objects
		//-----------------------------------------------------------------------------

		public IEnumerable<IPropertyObject> GetPropertyObjects() {
			yield return this;
			foreach (Room room in rooms) {
				foreach (IPropertyObject propertyObject in room.GetPropertyObjects()) {
					yield return propertyObject;
				}
			}
		}

		public IEnumerable<IEventObject> GetEventObjects() {
			yield return this;
			for (int x = 0; x < Width; x++) {
				for (int y = 0; y < Height; y++) {
					//foreach (Room room in rooms) {
					foreach (IEventObject eventObject in rooms[x, y].GetEventObjects()) {
						yield return eventObject;
					}
				}
			}
		}

		//-----------------------------------------------------------------------------
		// Level Coordinates
		//-----------------------------------------------------------------------------

		// Return true if the level tile coordinate is within the level's bounds.
		public bool IsInBounds(LevelTileCoord levelCoord) {
			return ContainsRoom(GetRoomLocation(levelCoord));
		}
		
		// Get the room location containing the given level tile coordinate.
		public Point2I GetRoomLocation(LevelTileCoord levelCoord) {
			Point2I roomLocation = new Point2I(
					levelCoord.X / roomSize.X,
					levelCoord.Y / roomSize.Y);
			if (levelCoord.X < 0)
				roomLocation.X--;
			if (levelCoord.Y < 0)
				roomLocation.Y--;
			return roomLocation;
		}
		
		// Get the room location containing the given level tile coordinate, clamped to the level's bounds.
		public Point2I GetRoomLocationClamped(LevelTileCoord levelCoord) {
			return new Point2I(
				GMath.Clamp(levelCoord.X / roomSize.X, 0, dimensions.X - 1),
				GMath.Clamp(levelCoord.Y / roomSize.Y, 0, dimensions.Y - 1));
		}
		
		// Get the room containing the given level tile coordinate.
		public Room GetRoom(LevelTileCoord levelCoord) {
			Point2I loc = GetRoomLocation(levelCoord);
			if (ContainsRoom(loc))
				return rooms[loc.X, loc.Y];
			return null;
		}
		
		// Get the room tile location of the given level tile coordinate.
		public Point2I GetTileLocation(LevelTileCoord levelCoord) {
			Point2I roomLocation = GetRoomLocation(levelCoord);
			return new Point2I(levelCoord.X - (roomLocation.X * roomSize.X),
							   levelCoord.Y - (roomLocation.Y * roomSize.Y));
		}

		// Take tiles from the level and put them into a tile grid.
		public TileGrid CreateTileGrid(Rectangle2I area) {
			return CreateTileGrid(area, CreateTileGridMode.Remove);
		}
		
		// Take tiles from the level (or duplicate them) and put them into a tile grid.
		public TileGrid CreateTileGrid(Rectangle2I area, CreateTileGridMode mode) {
			TileGrid tileGrid = new TileGrid(area.Size, roomLayerCount);
			BaseTileDataInstance[] tiles = GetTilesInArea(area).ToArray();

			foreach (BaseTileDataInstance baseTileOriginal in tiles) {
				// Duplicate the tile if specified, else remove the original.
				BaseTileDataInstance baseTile;
				if (mode == CreateTileGridMode.Duplicate) {
					baseTile = baseTileOriginal.Duplicate();
				}
				else {
					baseTile = baseTileOriginal;
					if (mode == CreateTileGridMode.Remove)
						baseTileOriginal.Room.Remove(baseTileOriginal);
				}

				// Add the tile to the tile grid.
				if (baseTile is TileDataInstance) {
					TileDataInstance tile = (TileDataInstance) baseTile;
					Point2I location = tile.Location + tile.Room.Location * roomSize - area.Point;
					tileGrid.PlaceTile(tile, location, tile.Layer);

				}
				else if (baseTile is EventTileDataInstance) {
					EventTileDataInstance eventTile = (EventTileDataInstance) baseTile;
					Point2I position = (eventTile.Room.Location * roomSize - area.Point) * GameSettings.TILE_SIZE;
					tileGrid.PlaceEventTile(eventTile, position + eventTile.Position);
				}
			}

			return tileGrid;
		}

		// Place the tiles in a tile grid starting at the given location.
		public void PlaceTileGrid(TileGrid tileGrid, LevelTileCoord location) {
			// Remove tiles.
			Rectangle2I area = new Rectangle2I((Point2I) location, tileGrid.Size);
			RemoveArea(area);

			// Place tiles.
			foreach (BaseTileDataInstance baseTile in tileGrid.GetTiles()) {
				if (baseTile is TileDataInstance) {
					TileDataInstance tile = (TileDataInstance) baseTile;
					LevelTileCoord coord = (LevelTileCoord) ((Point2I) location + tile.Location);
					Room room = GetRoom(coord);

					if (room != null) {
						tile.Location = GetTileLocation(coord);
						room.PlaceTile(tile, tile.Location, tile.Layer);
					}
				}
				else if (baseTile is EventTileDataInstance) {
					EventTileDataInstance eventTile = (EventTileDataInstance) baseTile;
					eventTile.Position += (Point2I) location * GameSettings.TILE_SIZE;
					Point2I roomLocation = eventTile.Position / (roomSize * GameSettings.TILE_SIZE);
					Room room = GetRoomAt(roomLocation);
					if (room != null) {
						eventTile.Position -= roomLocation * roomSize * GameSettings.TILE_SIZE;
						room.AddEventTile(eventTile);
					}
				}
			}
		}

		// Remove all tiles within the given area.
		public void RemoveArea(Rectangle2I area) {
			BaseTileDataInstance[] tiles = GetTilesInArea(area).ToArray();
			foreach (BaseTileDataInstance tile in tiles) {
				tile.Room.Remove(tile);
			}
		}


		//-----------------------------------------------------------------------------
		// Internal Iteration
		//-----------------------------------------------------------------------------

		private IEnumerable<BaseTileDataInstance> GetTilesInArea(Rectangle2I area) {
			// Make sure the area is within the level bounds.
			area = Rectangle2I.Intersect(area,
				new Rectangle2I(Point2I.Zero, roomSize * dimensions));

			// Iterate the tile grid.
			for (int x = 0; x < area.Width; x++) {
				for (int y = 0; y < area.Height; y++) {
					LevelTileCoord coord = (LevelTileCoord) (area.Point + new Point2I(x, y));
					Room room = GetRoom(coord);
					if (room != null) {
						Point2I tileLocation = GetTileLocation(coord);
						for (int i = 0; i < roomLayerCount; i++) {
							TileDataInstance tile = room.GetTile(tileLocation, i);
							if (tile != null && tile.Location == tileLocation)
								yield return tile;
						}
					}
				}
			}

			// Determine the collection of rooms that will contain the event tiles.
			Point2I roomAreaMin = GetRoomLocation((LevelTileCoord) area.Min);
			Point2I roomAreaMax = GetRoomLocation((LevelTileCoord) area.Max);
			Rectangle2I roomArea = new Rectangle2I(roomAreaMin, roomAreaMax - roomAreaMin + Point2I.One);
			roomArea = Rectangle2I.Intersect(roomArea, new Rectangle2I(Point2I.Zero, dimensions));
			Rectangle2I pixelArea = new Rectangle2I(
				area.Point * GameSettings.TILE_SIZE,
				area.Size  * GameSettings.TILE_SIZE);

			// Iterate event tiles.
			for (int x = roomArea.Left; x < roomArea.Right; x++) {
				for (int y = roomArea.Top; y < roomArea.Bottom; y++) {
					Room room = rooms[x, y];
					for (int i = 0; i < room.EventData.Count; i++) {
						EventTileDataInstance eventTile = room.EventData[i];
						Rectangle2I tileBounds = eventTile.GetBounds();
						tileBounds.Point += room.Location * roomSize * GameSettings.TILE_SIZE;
						if (pixelArea.Contains(tileBounds.Point))
							yield return eventTile;
					}
				}
			}
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

		public IEnumerable<Room> GetRooms() {
			for (int x = 0; x < dimensions.X; x++) {
				for (int y = 0; y < dimensions.Y; y++) {
					yield return rooms[x, y];
				}
			}
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
						rooms[x, y] = new Room(this, x, y, Zone ?? GameData.ZONE_DEFAULT);
				}
			}

			dimensions = size;
		}

		// Resize the dimensions of the room grid.
		public void Resize(Point2I size, Dictionary<Point2I, Room> restoredRooms) {
			Room[,] oldRooms = rooms;
			rooms = new Room[size.X, size.Y];

			for (int x = 0; x < size.X; x++) {
				for (int y = 0; y < size.Y; y++) {
					if (oldRooms != null && x < dimensions.X && y < dimensions.Y)
						rooms[x, y] = oldRooms[x, y];
					else if (restoredRooms.ContainsKey(new Point2I(x, y)))
						rooms[x, y] = restoredRooms[new Point2I(x, y)];
					else
						rooms[x, y] = new Room(this, x, y, Zone ?? GameData.ZONE_DEFAULT);
				}
			}

			dimensions = size;
		}

		// Shift the room grid.
		public void ShiftRooms(Point2I distance) {
			Room[,] oldRooms = rooms;
			rooms = new Room[dimensions.X, dimensions.Y];

			for (int x = 0; x < dimensions.X; x++) {
				for (int y = 0; y < dimensions.Y; y++) {
					if (x - distance.X >= 0 && x - distance.X < dimensions.X &&
						y - distance.Y >= 0 && y - distance.Y < dimensions.Y) {
						rooms[x, y] = oldRooms[x - distance.X, y - distance.Y];
						rooms[x, y].Location = new Point2I(x, y);
					}
					else
						rooms[x, y] = new Room(this, x, y, Zone ?? Resources.GetResource<Zone>(""));
				}
			}
		}

		// Shift the room grid.
		public void ShiftRooms(Point2I distance, Dictionary<Point2I, Room> restoredRooms) {
			Room[,] oldRooms = rooms;
			rooms = new Room[dimensions.X, dimensions.Y];

			for (int x = 0; x < dimensions.X; x++) {
				for (int y = 0; y < dimensions.Y; y++) {
					Point2I location = new Point2I(x, y);
					if (x - distance.X >= 0 && x - distance.X < dimensions.X &&
						y - distance.Y >= 0 && y - distance.Y < dimensions.Y) {
						rooms[x, y] = oldRooms[x - distance.X, y - distance.Y];
						rooms[x, y].Location = location;
					}
					else if (restoredRooms.ContainsKey(location)) {
						rooms[x, y] = restoredRooms[location];
						rooms[x, y].Location = location;
					}
					else
						rooms[x, y] = new Room(this, location, Zone ?? Resources.GetResource<Zone>(""));
				}
			}
		}

		public void ResizeLayerCount(int newLayerCount) {
			newLayerCount = Math.Max(1, newLayerCount);
			if (newLayerCount != roomLayerCount) {
				roomLayerCount = newLayerCount;
				foreach (Room room in rooms) {
					room.ResizeLayerCount(newLayerCount);
				}
			}
		}

		public void FillWithDefaultTiles() {
			for (int x = 0; x < dimensions.X; x++) {
				for (int y = 0; y < dimensions.Y; y++) {
					FillRoomWithDefaultTiles(rooms[x, y]);
				}
			}
		}

		public void FillRoomWithDefaultTiles(Room room) {
			for (int x = 0; x < room.Width; x++) {
				for (int y = 0; y < room.Height; y++) {
					room.PlaceTile(new TileDataInstance(Zone.DefaultTileData), x, y, 0);
				}
			}
		}

		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public World World {
			get { return world; }
			set { world = value; }
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

		public Point2I Span {
			get { return dimensions * roomSize; }
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
			get { return properties.GetResource<Zone>("zone", GameData.ZONE_DEFAULT); }
			set {
				if (value != null)
					properties.Set("zone", value.ID);
				else
					properties.Set("zone", "");
			}
			//get { return zone; }
			//set { zone = value; }
		}

		public string ID {
			get { return properties.GetString("id"); }
			set { properties.Set("id", value); }
		}
		
		public Dungeon Dungeon {
			get { return world.GetDungeon(properties.GetString("dungeon", "")); }
			set {
				if (value == null)
					properties.Set("dungeon", "");
				else
					properties.Set("dungeon", value.ID);
			}
		}
		
		public int DungeonFloor {
			get { return properties.GetInteger("dungeon_floor", 0); }
			set { properties.Set("dungeon_floor", value); }
		}
		
		public bool IsDiscovered {
			get { return properties.GetBoolean("discovered", false); }
			set { properties.Set("discovered", value); }
		}
		
		public EventCollection Events {
			get { return events; }
		}
	}
}
