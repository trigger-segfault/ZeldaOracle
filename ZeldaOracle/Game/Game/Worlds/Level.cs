using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.API;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Tiles.ActionTiles;
using ZeldaOracle.Game.Worlds.Editing;

namespace ZeldaOracle.Game.Worlds {
	/// <summary>A single floor in a world containing a grid of rooms.</summary>
	public partial class Level : IEventObjectContainer, IEventObject, IIDObject,
		IVariableObjectContainer, IVariableObject
	{
		private World		world;
		private Point2I		roomSize;		// The size in tiles of each room in the level.
		private int			roomLayerCount; // The number of tile layers for each room in the level.
		private Point2I		dimensions;		// The dimensions of the grid of rooms.
		private Room[,]		rooms;			// The grid of rooms.
		//private Zone		zone;
		private Properties	properties;
		private Variables	variables;
		private EventCollection events;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Constructs an empty level.</summary>
		public Level() {
			world			= null;
			roomSize		= Point2I.Zero;
			dimensions		= Point2I.Zero;

			events			= new EventCollection(this);
			properties		= new Properties(this);
			properties.BaseProperties	= new Properties();
			variables		= new Variables(this);

			properties.BaseProperties.Set("id", "")
				.SetDocumentation("ID", "", "", "General", "The id used to refer to this level.", false, false);

			properties.BaseProperties.Set("area", "")
				.SetDocumentation("Area", "area", "", "Area", "The default area this level belongs to.");
			properties.BaseProperties.Set("floor_number", 0)
				.SetDocumentation("Floor Number", "", "", "Area", "The floor number that will diplay in the dungeon map. Use 0 or higher for above floors and -1 or lower for below floors.");

			properties.BaseProperties.Set("discovered", false)
				.SetDocumentation("Discovered", "", "", "Progress", "True if the level has been visited at least once.");

			properties.BaseProperties.Set("zone", "")
				.SetDocumentation("Zone", "zone", "", "Level", "The zone type for this room.");
			properties.BaseProperties.Set("connected_level_above", "")
				.SetDocumentation("Connected Level Above", "level", "", "Level", "The level that is above this one.");
			properties.BaseProperties.Set("connected_level_below", "")
				.SetDocumentation("Connected Level Below", "level", "", "Level", "The level that is below this one.");
			properties.BaseProperties.Set("parent_level", "")
				.SetDocumentation("Parent Level", "level", "", "Parenting", "The level that this level shares certain settings with like room clearing.");
		}

		public Level(int width, int height, Point2I roomSize) :
			this("", width, height, GameSettings.DEFAULT_TILE_LAYER_COUNT, roomSize, null) {
		}

		public Level(Point2I dimensions, Point2I roomSize) :
			this("", dimensions, GameSettings.DEFAULT_TILE_LAYER_COUNT, roomSize, null) {
		}

		public Level(int width, int height, int layerCount, Point2I roomSize) :
			this("", width, height, layerCount, roomSize, null) {
		}

		public Level(Point2I dimensions, int layerCount, Point2I roomSize) :
			this("", dimensions, layerCount, roomSize, null) {
		}

		public Level(string id, int width, int height, int layerCount, Point2I roomSize, Zone zone) :
			this(id, new Point2I(width, height), layerCount, roomSize, zone) {
		}

		public Level(string id, Point2I dimensions, int layerCount, Point2I roomSize, Zone zone) :
			this()
		{
			this.roomSize		= roomSize;
			this.roomLayerCount	= layerCount;
			this.dimensions		= Point2I.Zero;

			ID = id;

			Zone = zone;

			Resize(dimensions);
		}

		public Level(Level copy) :
			this()
		{
			properties	= new Properties(copy.properties, this);
			events		= new EventCollection(copy.events, this);

			this.world          = copy.world;
			this.roomSize       = copy.roomSize;
			this.roomLayerCount	= copy.roomLayerCount;
			this.dimensions     = copy.dimensions;

			this.rooms = new Room[Width, Height];
			for (int x = 0; x < Width; x++) {
				for (int y = 0; y < Height; y++) {
					rooms[x, y] = new Room(copy.rooms[x, y]);
					rooms[x, y].Level = this;
				}
			}
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

		public IEnumerable<IVariableObject> GetVariableObjects() {
			yield return this;
			foreach (Room room in rooms) {
				yield return room;
				/*foreach (IVariableObject variableObject in room.GetVariableObjects()) {
					yield return variableObject;
				}*/
			}
		}


		//-----------------------------------------------------------------------------
		// Tile Grids
		//-----------------------------------------------------------------------------

		/// <summary>Take tiles from the level and put them into a tile grid.</summary>
		public TileGrid CreateFullTileGrid(Rectangle2I area, CreateTileGridMode mode) {
			TileGrid tileGrid = TileGrid.CreateFullTileGrid(area.Size, roomLayerCount);
			return SetupTileGrid(area, tileGrid, mode);
		}

		/// <summary>Take tiles from the level and put them into a tile grid.</summary>
		public TileGrid CreateSingleLayerTileGrid(Rectangle2I area, int startLayer, CreateTileGridMode mode) {
			TileGrid tileGrid = TileGrid.CreateSingleLayerTileGrid(area.Size, startLayer);
			return SetupTileGrid(area, tileGrid, mode);
		}

		/// <summary>Take tiles from the level and put them into a tile grid.</summary>
		public TileGrid CreateActionGrid(Rectangle2I area, CreateTileGridMode mode) {
			TileGrid tileGrid = TileGrid.CreateActionGrid(area.Size);
			return SetupTileGrid(area, tileGrid, mode);
		}

		/// <summary>Take tiles from the level (or duplicate them) and put them into
		/// a tile grid.</summary>
		private TileGrid SetupTileGrid(Rectangle2I area, TileGrid tileGrid, CreateTileGridMode mode) {
			//TileGrid tileGrid = new TileGrid(area.Size, roomLayerCount);
			BaseTileDataInstance[] tiles = GetTilesInArea(area, tileGrid).ToArray();

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
					Point2I location = tile.LevelCoord - area.Point;
					tileGrid.PlaceTile(tile, location, tile.Layer);

				}
				else if (baseTile is ActionTileDataInstance) {
					ActionTileDataInstance actionTile = (ActionTileDataInstance) baseTile;
					Point2I position = (actionTile.Room.LevelCoord - area.Point) * GameSettings.TILE_SIZE;
					tileGrid.PlaceActionTile(actionTile, position + actionTile.Position);
				}
			}

			return tileGrid;
		}

		/// <summary>Place the tiles in a tile grid starting at the given location.</summary>
		public void PlaceTileGrid(TileGrid tileGrid, Point2I levelCoord,
			bool merge)
		{
			Rectangle2I area = new Rectangle2I(levelCoord, tileGrid.Size);
			
			if (!merge) // Remove tiles
				RemoveArea(area, tileGrid);

			if (tileGrid.IncludesTiles) {
				// Place tiles
				foreach (TileDataInstance tile in tileGrid.GetTilesAtLocation()) {
					this.PlaceTile(tile, tile.Location + levelCoord, tile.Layer, true);
					/*LevelTileCoord coord = (LevelTileCoord) ((Point2I) levelCoord + tile.Location);
					Room room = GetRoom(coord);

					if (room != null) {
						tile.Location = GetTileLocation(coord);
						room.PlaceTile(tile, tile.Location, tile.Layer);
					}*/
				}
			}

			if (tileGrid.IncludesActions) {
				Point2I levelPosition = this.LevelCoordToPosition(levelCoord);
				// Place action tiles
				foreach (ActionTileDataInstance action in tileGrid.GetActionTilesAtPosition()) {
					this.PlaceActionTile(action, action.Position + levelPosition, true);
					/*action.Position += (Point2I) levelCoord * GameSettings.TILE_SIZE;
					if (!(action.Position >= Point2I.Zero))
						continue;
					Point2I roomLocation = action.Position / (roomSize * GameSettings.TILE_SIZE);
					Room room = GetRoomAt(roomLocation);
					if (room != null) {
						action.Position -= roomLocation * roomSize * GameSettings.TILE_SIZE;
						room.AddActionTile(action);
					}*/
				}
			}
		}

		/// <summary>Remove all tiles within the given area.</summary>
		public void RemoveArea(Rectangle2I area, TileGrid tileGrid) {
			RemoveArea(area, tileGrid.StartLayer, tileGrid.LayerCount,
				tileGrid.IncludesTiles, tileGrid.IncludesActions);
		}

		/// <summary>Remove all tiles within the given area.</summary>
		public void RemoveArea(Rectangle2I area,
			int startLayer = 0, int layerCount = -1,
			bool includeTiles = true, bool includeActions = true)
		{
			BaseTileDataInstance[] tiles = GetTilesInArea(area, startLayer,
				layerCount, includeTiles, includeActions).ToArray();
			foreach (BaseTileDataInstance tile in tiles) {
				tile.Room.Remove(tile);
			}
		}


		//-----------------------------------------------------------------------------
		// Internal Iteration
		//-----------------------------------------------------------------------------
		
		/// <summary>Gets all tiles in the area using the tile grid's settings.</summary>
		private IEnumerable<BaseTileDataInstance> GetTilesInArea(Rectangle2I area,
			TileGrid tileGrid)
		{
			return GetTilesInArea(area, tileGrid.StartLayer, tileGrid.LayerCount,
				tileGrid.IncludesTiles, tileGrid.IncludesActions);
		}

		/// <summary>Gets all tiles in the specified area.</summary>
		private IEnumerable<BaseTileDataInstance> GetTilesInArea(Rectangle2I area,
			int startLayer = 0, int layerCount = -1,
			bool includeTiles = true, bool includeActions = true)
		{
			if (layerCount == -1)
				layerCount = roomLayerCount;
			// Make sure the area is within the level bounds.
			area = Rectangle2I.Intersect(area, TileBounds);

			if (includeTiles) {
				// Iterate the tile grid. (Backwards to prevent large tiles from getting overwritten
				for (int x = area.Width - 1; x >= 0; x--) {
					for (int y = area.Height - 1; y >= 0; y--) {
						//LevelTileCoord coord = (LevelTileCoord) (area.Point + new Point2I(x, y));
						Point2I levelCoord = area.Point + new Point2I(x, y);
						Room room;
						Point2I location = this.LevelToRoomCoord(levelCoord, out room);
						//Room room = GetRoom(coord);
						if (room != null) {
							//Point2I tileLocation = GetTileLocation(coord);
							for (int i = startLayer; i < startLayer + layerCount; i++) {
								TileDataInstance tile = room.GetTile(location, i);
								if (tile != null && tile.Location == location)
									yield return tile;
							}
						}
					}
				}
			}

			if (includeActions) {
				// Determine the collection of rooms that will contain the action tiles.
				//Point2I roomAreaMin = this.LevelCoordToRoomLocation(area.Min);
				//Point2I roomAreaMax = this.LevelCoordToRoomLocation(area.Max);
				Rectangle2I roomArea = this.LevelCoordToRoomLocation(area);
				//Point2I roomAreaMin = GetRoomLocation((LevelTileCoord) area.Min);
				//Point2I roomAreaMax = GetRoomLocation((LevelTileCoord) area.Max);
				//Rectangle2I roomArea = Rectangle2I.FromEndPointsOne(
				//	roomAreaMin, roomAreaMax);
				roomArea = Rectangle2I.Intersect(roomArea, RoomBounds);
				Rectangle2I pixelArea = this.LevelCoordToPosition(area);
				/*	new Rectangle2I(
					area.Point * GameSettings.TILE_SIZE,
					area.Size  * GameSettings.TILE_SIZE);*/

				// Iterate action tiles.
				for (int x = roomArea.Left; x < roomArea.Right; x++) {
					for (int y = roomArea.Top; y < roomArea.Bottom; y++) {
						Room room = rooms[x, y];
						for (int i = 0; i < room.ActionCount; i++) {
							ActionTileDataInstance actionTile = room.GetActionTileAt(i);
							if (pixelArea.Contains(actionTile.LevelPosition))
								yield return actionTile;
						}
					}
				}
			}
		}


		//-----------------------------------------------------------------------------
		// Accessors
		//-----------------------------------------------------------------------------
		
		/// <summary>Returns true if a room at the specified location exists.</summary>
		public bool ContainsRoom(Point2I location) {
			return RoomBounds.Contains(location);
		}

		/// <summary>Gets the room at the specified location.</summary>
		public Room GetRoomAt(int x, int y, bool clamp = false) {
			return GetRoomAt(new Point2I(x, y), clamp);
		}

		/// <summary>Gets the room at the specified location.</summary>
		public Room GetRoomAt(Point2I location, bool clamp = false) {
			if (clamp)
				location = GMath.Clamp(location, RoomBounds);
			else if (!RoomBounds.Contains(location) || roomSize.IsAnyZero)
				return null;
			return rooms[location.X, location.Y];
		}

		/// <summary>Gets all rooms in the level.</summary>
		public IEnumerable<Room> GetRooms() {
			for (int x = 0; x < dimensions.X; x++) {
				for (int y = 0; y < dimensions.Y; y++) {
					yield return rooms[x, y];
				}
			}
		}

		/// <summary>Finds the first action tile with the given ID.</summary>
		public ActionTileDataInstance FindActionTileByID(string id) {
			for (int x = 0; x < dimensions.X; x++) {
				for (int y = 0; y < dimensions.Y; y++) {
					ActionTileDataInstance actionTile = rooms[x, y].FindActionTileByID(id);
					if (actionTile != null)
						return actionTile;
				}
			}
			return null;
		}


		//-----------------------------------------------------------------------------
		// Mutators
		//-----------------------------------------------------------------------------

		/// <summary>Sets the room at the specified location.</summary>
		public void SetRoom(Room room, int x, int y) {
			SetRoom(room, new Point2I(x, y));
		}

		/// <summary>Sets the room at the specified location.</summary>
		public void SetRoom(Room room, Point2I location) {
			rooms[location.X, location.Y] = room;
			room.Level = this;
		}

		/// <summary>Resize the dimensions of the room grid.</summary>
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

		/// <summary>Resize the dimensions of the room grid. And restores the cuttoff
		/// rooms to their original positions.</summary>
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
						rooms[x, y] = new Room(this, x, y);
				}
			}

			dimensions = size;
		}

		/// <summary>Shift the room grid.</summary>
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
						rooms[x, y] = new Room(this, x, y);
				}
			}
		}

		/// <summary>Shifts the room grid. And restores the cuttoff rooms to their
		/// original positions.</summary>
		public void ShiftRooms(Point2I distance,
			Dictionary<Point2I, Room> restoredRooms)
		{
			Room[,] oldRooms = rooms;
			rooms = new Room[dimensions.X, dimensions.Y];

			for (int x = 0; x < dimensions.X; x++) {
				for (int y = 0; y < dimensions.Y; y++) {
					Point2I location = new Point2I(x, y);
					if (x - distance.X >= 0 && x - distance.X < dimensions.X &&
						y - distance.Y >= 0 && y - distance.Y < dimensions.Y)
					{
						rooms[x, y] = oldRooms[x - distance.X, y - distance.Y];
						rooms[x, y].Location = location;
					}
					else if (restoredRooms.ContainsKey(location)) {
						rooms[x, y] = restoredRooms[location];
						rooms[x, y].Location = location;
					}
					else
						rooms[x, y] = new Room(this, location);
				}
			}
		}

		/// <summary>Resizes the number of tile layers in the room.</summary>
		public void ResizeLayerCount(int newLayerCount) {
			newLayerCount = GMath.Max(1, newLayerCount);
			if (newLayerCount != roomLayerCount) {
				roomLayerCount = newLayerCount;
				foreach (Room room in rooms) {
					room.ResizeLayerCount(newLayerCount);
				}
			}
		}

		/// <summary>Fills all rooms in the level with the zone's default tile.</summary>
		public void FillWithDefaultTiles() {
			for (int x = 0; x < dimensions.X; x++) {
				for (int y = 0; y < dimensions.Y; y++) {
					FillRoomWithDefaultTiles(rooms[x, y]);
				}
			}
		}

		/// <summary>Fills a room with the zone's default tile.</summary>
		public void FillRoomWithDefaultTiles(Room room) {
			for (int x = 0; x < room.Width; x++) {
				for (int y = 0; y < room.Height; y++) {
					room.PlaceTile(new TileDataInstance(room.Zone.DefaultTileData), x, y, 0);
				}
			}
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets or sets the world containing this level.</summary>
		public World World {
			get { return world; }
			set { world = value; }
		}
		
		/// <summary>Gets the size of each room in tiles.</summary>
		public Point2I RoomSize {
			get { return roomSize; }
		}

		/// <summary>Gets the size of each room in pixels.</summary>
		public Point2I RoomPixelSize {
			get { return roomSize * GameSettings.TILE_SIZE; }
		}

		/// <summary>Gets the dimensions of the level in rooms.</summary>
		public Point2I Dimensions {
			get { return dimensions; }
		}

		/// <summary>Gets the dimensions of the level in tiles.</summary>
		public Point2I TileDimensions {
			get { return dimensions * roomSize; }
		}

		/// <summary>Gets the dimensions of the level in pixels.</summary>
		public Point2I PixelDimensions {
			get { return dimensions * roomSize * GameSettings.TILE_SIZE; }
		}

		/// <summary>Gets the boundaires of the level in rooms.</summary>
		public Rectangle2I RoomBounds {
			get { return new Rectangle2I(Dimensions); }
		}

		/// <summary>Gets the boundaires of the level in tiles.</summary>
		public Rectangle2I TileBounds {
			get { return new Rectangle2I(TileDimensions); }
		}

		/// <summary>Gets the boundaires of the level in pixels.</summary>
		public Rectangle2I PixelBounds {
			get { return new Rectangle2I(PixelDimensions); }
		}

		/// <summary>Gets the width of the level in rooms.</summary>
		public int Width {
			get { return dimensions.X; }
			set { dimensions.X = value; }
		}

		/// <summary>Gets the height of the level in rooms.</summary>
		public int Height {
			get { return dimensions.Y; }
			set { dimensions.Y = value; }
		}

		/// <summary>Gets the width of each room in tiles.</summary>
		public int RoomWidth {
			get { return roomSize.X; }
			set { roomSize.X = value; }
		}

		/// <summary>Gets the height of each room in tiles.</summary>
		public int RoomHeight {
			get { return roomSize.Y; }
			set { roomSize.Y = value; }
		}

		/// <summary>Gets the number of tile grid layers in each room.</summary>
		public int RoomLayerCount {
			get { return roomLayerCount; }
			set { roomLayerCount = value; }
		}

		/// <summary>Gets or sets the properties for the level.</summary>
		public Properties Properties {
			get { return properties; }
			set {
				properties = value;
				properties.PropertyObject = this;
			}
		}

		/// <summary>Gets the variables for the level.</summary>
		public Variables Vars {
			get { return variables; }
		}

		/// <summary>Gets or sets the zone for the level.</summary>
		public Zone Zone {
			get { return properties.GetResource<Zone>("zone", GameData.ZONE_DEFAULT); }
			set {
				if (value != null)
					properties.Set("zone", value.ID);
				else
					properties.Set("zone", "");
			}
		}

		/// <summary>Gets or sets the ID of the level.</summary>
		public string ID {
			get { return properties.GetString("id"); }
			set { properties.Set("id", value); }
		}

		/// <summary>Gets or sets the area for the level.</summary>
		public Area Area {
			get {
				Area area = world.GetArea(properties.GetString("area", ""));
				return area ?? world.DefaultArea;
			}
			set {
				if (value == null)
					properties.Set("area", "");
				else
					properties.Set("area", value.ID);
			}
		}

		/// <summary>Gets or sets the area ID for the level.</summary>
		public string AreaID {
			get { return properties.GetString("area", ""); }
			set { properties.Set("area", value); }
		}

		/// <summary>Gets or sets the parent level that this level shares
		/// certain settings with like room clearing.</summary>
		public Level ParentLevel {
			get { return world.GetLevel(properties.GetString("parent_level", "")); }
			set {
				if (value == null)
					properties.Set("parent_level", "");
				else
					properties.Set("parent_level", value.ID);
			}
		}

		/// <summary>Returns the root parented level for this level.
		/// This value can be itself.</summary>
		public Level RootLevel {
			get {
				Level parentLevel = ParentLevel;
				if (parentLevel != null)
					return parentLevel;//.RootLevel;
				return this;
			}
		}

		/// <summary>Gets or sets the level connected above this level.</summary>
		public Level ConnectedLevelAbove {
			get { return world.GetLevel(properties.GetString("connected_level_above", "")); }
			set {
				if (value == null)
					properties.Set("connected_level_above", "");
				else
					properties.Set("connected_level_above", value.ID);
			}
		}

		/// <summary>Gets or sets the level connected below this level.</summary>
		public Level ConnectedLevelBelow {
			get { return world.GetLevel(properties.GetString("connected_level_below", "")); }
			set {
				if (value == null)
					properties.Set("connected_level_below", "");
				else
					properties.Set("connected_level_below", value.ID);
			}
		}

		/// <summary>Gets or sets the floor number of this level.
		/// Used for dungeon maps.</summary>
		public int FloorNumber {
			get { return properties.GetInteger("floor_number", 0); }
			set { properties.Set("floor_number", value); }
		}
		
		/// <summary>Gets or sets if the level has been discovered.</summary>
		public bool IsDiscovered {
			get { return properties.GetBoolean("discovered", false); }
			set { properties.Set("discovered", value); }
		}

		/// <summary>Gets the events for the level.</summary>
		public EventCollection Events {
			get { return events; }
		}
	}
}
