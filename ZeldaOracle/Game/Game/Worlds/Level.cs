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
	public class Level : IPropertyObject {

		private World		world;
		private Point2I		roomSize;		// The size in tiles of each room in the level.
		private int			roomLayerCount; // The number of tile layers for each room in the level.
		private Point2I		dimensions;		// The dimensions of the grid of rooms.
		private Room[,]		rooms;			// The grid of rooms.
		//private Zone		zone;
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
			//this.zone			= zone;


			properties = new Properties(this);
			properties.BaseProperties = new Properties();

			properties.BaseProperties.Set("id", "")
				.SetDocumentation("ID", "", "", "", "The id used to refer to this level.", false, true);

			properties.BaseProperties.Set("dungeon", "")
				.SetDocumentation("Dungeon", "dungeon", "", "", "The dungeon this level belongs to.");
			properties.BaseProperties.Set("dungeon_floor", 0)
				.SetDocumentation("Dungeon Floor", "", "", "", "The floor in the dungeon this level belongs to.");

			properties.BaseProperties.Set("discovered", false);

			properties.Set("id", name);

			properties.BaseProperties.Set("zone", "")
				.SetDocumentation("Zone", "zone", "", "", "The zone type for this room.", true, false);

			Zone = zone;
			
			Resize(new Point2I(width, height));
		}


		//-----------------------------------------------------------------------------
		// Level Coordinates
		//-----------------------------------------------------------------------------

		public bool ContainsRoom(LevelTileCoord levelCoord) {
			return ContainsRoom(GetRoomLocation(levelCoord));
		}

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

		public Point2I GetRoomLocationClamped(LevelTileCoord levelCoord) {
			return new Point2I(
				GMath.Clamp(levelCoord.X / roomSize.X, 0, dimensions.X - 1),
				GMath.Clamp(levelCoord.Y / roomSize.Y, 0, dimensions.Y - 1));
		}

		public Room GetRoom(LevelTileCoord levelCoord) {
			Point2I loc = GetRoomLocation(levelCoord);
			if (ContainsRoom(loc))
				return rooms[loc.X, loc.Y];
			return null;
		}
		
		public Point2I GetTileLocation(LevelTileCoord levelCoord) {
			Point2I roomLocation = GetRoomLocation(levelCoord);
			return new Point2I(levelCoord.X - (roomLocation.X * roomSize.X),
							   levelCoord.Y - (roomLocation.Y * roomSize.Y));
		}

		public TileGrid CreateTileGrid(Rectangle2I area) {
			return CreateTileGrid(area, false);
		}

		public IEnumerable<BaseTileDataInstance> GetTilesInArea(Rectangle2I area) {
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

		public void RemoveArea(Rectangle2I area) {
			BaseTileDataInstance[] tiles = GetTilesInArea(area).ToArray();
			foreach (BaseTileDataInstance tile in tiles) {
				tile.Room.Remove(tile);
			}
		}

		public TileGrid CreateTileGrid(Rectangle2I area, bool duplicate) {
			TileGrid tileGrid = new TileGrid(area.Size, roomLayerCount);
			BaseTileDataInstance[] tiles = GetTilesInArea(area).ToArray();

			foreach (BaseTileDataInstance baseTile in tiles) {
				baseTile.Room.Remove(baseTile);

				if (baseTile is TileDataInstance) {
					TileDataInstance tile = (TileDataInstance) baseTile;
					tile.Location += tile.Room.Location * roomSize;
					tile.Location -= area.Point;
					tileGrid.PlaceTile(tile, tile.Location, tile.Layer);

				}
				else if (baseTile is EventTileDataInstance) {
					EventTileDataInstance eventTile = (EventTileDataInstance) baseTile;
					eventTile.Position += eventTile.Room.Location * roomSize * GameSettings.TILE_SIZE;
					eventTile.Position -= area.Point * GameSettings.TILE_SIZE;
					tileGrid.AddEventTile(eventTile);
				}
			}

			return tileGrid;

			/*
			// Add tiles.
			for (int x = 0; x < area.Width; x++) {
				for (int y = 0; y < area.Height; y++) {
					LevelTileCoord coord = (LevelTileCoord) (area.Point + new Point2I(x, y));
					Room room = GetRoom(coord);

					if (room != null) {
						Point2I tileLocation = GetTileLocation(coord);

						for (int i = 0; i < roomLayerCount; i++) {
							TileDataInstance tile = room.GetTile(tileLocation, i);

							if (tile != null && tile.Location == tileLocation) {
								if (duplicate) {
									TileDataInstance newTile = new TileDataInstance();
									newTile.Clone(tile);
									tileGrid.PlaceTile(newTile, x, y, i);
								}
								else {
									tileGrid.PlaceTile(tile, x, y, i);
									room.RemoveTile(tile);
								}
							}
						}
					}
				}
			}
			
			// TODO: this will fail on negatives.
			Point2I roomAreaMin = area.Min / roomSize;
			Point2I roomAreaMax = (area.Max - Point2I.One) / roomSize;
			Rectangle2I roomArea = new Rectangle2I(
				roomAreaMin, roomAreaMax - roomAreaMin + Point2I.One);
			Rectangle2I pixelArea = area;
			pixelArea.Point *= GameSettings.TILE_SIZE;
			pixelArea.Size *= GameSettings.TILE_SIZE;
			
			// Add event tiles.
			for (int x = roomArea.X; x < roomArea.Max.X; x++) {
				for (int y = roomArea.Y; y < roomArea.Max.Y; y++) {
					Room room = rooms[x, y];
					for (int i = 0; i < room.EventData.Count; i++) {
						EventTileDataInstance eventTile = room.EventData[i];
						Rectangle2I tileBounds = eventTile.GetBounds();
						tileBounds.Point += room.Location * room.Size * GameSettings.TILE_SIZE;

						if (pixelArea.Contains(tileBounds.Point)) {
							eventTile.Position += room.Location * room.Size * GameSettings.TILE_SIZE;
							eventTile.Position -= area.Point * GameSettings.TILE_SIZE;
							tileGrid.AddEventTile(eventTile);
							room.EventData.RemoveAt(i--);
						}
					}
				}
			}
			
			return tileGrid;*/
		}

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
			/*


			int numLayers = Math.Min(roomLayerCount, tileGrid.LayerCount);
			
			// Remove tiles.
			for (int x = 0; x < tileGrid.Width; x++) {
				for (int y = 0; y < tileGrid.Height; y++) {
					LevelTileCoord coord = location;
					coord.X += x;
					coord.Y += y;

					if (ContainsRoom(coord)) {
						Room room = GetRoom(coord);
						Point2I tileLocation = GetTileLocation(coord);
						for (int i = 0; i < numLayers; i++)
							room.RemoveTile(tileLocation.X, tileLocation.Y, i);
					}
				}
			}
			
			// TODO: this will fail on negatives.
			Point2I roomAreaMin = (Point2I) location / roomSize;
			Point2I roomAreaMax = ((Point2I) location + tileGrid.Size - Point2I.One) / roomSize;
			Rectangle2I roomArea = new Rectangle2I(
				roomAreaMin, roomAreaMax - roomAreaMin + Point2I.One);
			Rectangle2I pixelArea = new Rectangle2I((Point2I) location, tileGrid.Size);
			pixelArea.Point *= GameSettings.TILE_SIZE;
			pixelArea.Size *= GameSettings.TILE_SIZE;
			
			// Remove event tiles.
			for (int x = roomArea.X; x < roomArea.Max.X; x++) {
				for (int y = roomArea.Y; y < roomArea.Max.Y; y++) {
					Room room = rooms[x, y];
					for (int i = 0; i < room.EventData.Count; i++) {
						EventTileDataInstance eventTile = room.EventData[i];
						Rectangle2I tileBounds = eventTile.GetBounds();
						tileBounds.Point += room.Location * room.Size * GameSettings.TILE_SIZE;

						if (pixelArea.Contains(tileBounds.Point)) {
							room.EventData.RemoveAt(i--);
						}
					}
				}
			}

			// Place tiles.
			for (int x = 0; x < tileGrid.Width; x++) {
				for (int y = 0; y < tileGrid.Height; y++) {
					LevelTileCoord coord = location;
					coord.X += x;
					coord.Y += y;

					if (ContainsRoom(coord)) {
						Room room = GetRoom(coord);
						Point2I tileLocation = GetTileLocation(coord);

						for (int i = 0; i < numLayers; i++) {
							TileDataInstance tile = tileGrid.GetTileIfAtLocation(x, y, i);
							if (tile != null)
								room.PlaceTile(tile, tileLocation, i);
						}
					}
				}
			}

			// Place event tiles.
			foreach (EventTileDataInstance eventTile in tileGrid.EventTiles) {
				eventTile.Position += (Point2I) location * GameSettings.TILE_SIZE;
				Point2I roomLocation = eventTile.Position / (roomSize * GameSettings.TILE_SIZE);
				Room room = GetRoomAt(roomLocation);
				if (room != null) {
					eventTile.Position -= roomLocation * roomSize * GameSettings.TILE_SIZE;
					room.AddEventTile(eventTile);
				}
			}*/
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

		// Shift the room grid.
		public void Shift(Point2I distance) {
			Room[,] oldRooms = rooms;
			rooms = new Room[dimensions.X, dimensions.Y];

			for (int x = 0; x < dimensions.X; x++) {
				for (int y = 0; y < dimensions.Y; y++) {
					if (x - distance.X >= 0 && x - distance.X < dimensions.X &&
						y - distance.Y >= 0 && y - distance.Y < dimensions.Y)
					{
						rooms[x, y] = oldRooms[x - distance.X, y - distance.Y];
						rooms[x, y].Location = new Point2I(x, y);
					}
					else
						rooms[x, y] = new Room(this, x, y, Zone ?? Resources.GetResource<Zone>(""));
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
			get { return properties.GetResource<Zone>("zone", null); }
			set {
				if (value != null)
					properties.Set("zone", value.ID);
				else
					properties.Set("zone", "");
			}
			//get { return zone; }
			//set { zone = value; }
		}

		public string Id {
			get { return properties.GetString("id"); }
			set { properties.Set("id", value); }
		}
		
		public Dungeon Dungeon {
			get { return world.GetDungoen(properties.GetString("dungeon", "")); }
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
	}
}
