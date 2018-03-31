using ZeldaOracle.Common.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Tiles.ActionTiles;
using ZeldaOracle.Game.Tiles.Custom;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Game.Control.Scripting;
using ZeldaOracle.Game.API;
using ZeldaOracle.Game.Tiles.Custom.Monsters;
using ZeldaOracle.Game.Entities.Monsters;
using ZeldaOracle.Common.Util;

namespace ZeldaOracle.Game.Worlds {
	public class Room : IEventObjectContainer, IEventObject, IVariableObject,
		ITriggerObject, ZeldaAPI.RoomTODO
	{

		private Level							level;		// The level this room is in.
		private Point2I							location;	// Location within the level.
		private TileDataInstance[,,]			tileData;	// 3D grid of tile data (x, y, layer)
		private List<ActionTileDataInstance>	actionData;
		private Properties						properties;
		private Variables						variables;
		private EventCollection					events;
		private TriggerCollection				triggers;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public Room() {
			level		= null;
			location	= Point2I.Zero;
			tileData	= new TileDataInstance[0, 0, 0];
			actionData	= new List<ActionTileDataInstance>();

			properties	= new Properties(this);
			properties.BaseProperties = new Properties();
			variables	= new Variables(this);
			events		= new EventCollection(this);
			triggers	= new TriggerCollection(this);

			properties.BaseProperties.Set("id", "")
				.SetDocumentation("ID", "", "", "General", "The id used to refer to this room.");
			properties.BaseProperties.Set("music", "")
				.SetDocumentation("Music", "song", "", "General", "The music to play in this room. Select none to choose the default music.", true, false);
			properties.BaseProperties.Set("zone", "")
				.SetDocumentation("Zone", "zone", "", "General", "The zone type for this room.");
			properties.BaseProperties.SetEnumStr("spawn_mode", MonsterSpawnMode.Normal)
				.SetDocumentation("Monster Spawn Mode", "enum", typeof(MonsterSpawnMode), "General", "The method for spawning monsters in this room.");
			//properties.BaseProperties.Set("area", "")
			//	.SetDocumentation("Area", "area", "", "Area", "The area this room belongs to.");

			properties.BaseProperties.Set("discovered", false)
				.SetDocumentation("Discovered", "Progress", "True if the room has been visited at least once.");
			properties.BaseProperties.Set("hidden_from_map", false)
				.SetDocumentation("Hidden From Map", "Dungeon", "True if this room does not appear on the map even when visited.");
			properties.BaseProperties.Set("boss_room", false)
				.SetDocumentation("Is Boss Room", "Dungeon", "True if this room is shown as the boss room in the dungeon map.");

			properties.BaseProperties.Set("death_out_of_bounds", false)
				.SetDocumentation("Death out of Bounds", "Side-Scrolling", "True if the player dies and respawns when falling off the edge of the map.");

			properties.BaseProperties.Set("parent_level", "")
				.SetDocumentation("Parent Level", "level", "", "Parenting", "The level that this level shares certain settings with like room clearing.");
			properties.BaseProperties.Set("parent_location", -Point2I.One)
				.SetDocumentation("Parent Location", "Parenting", "The location that this room shares certain settings with like room clearing.");
			properties.BaseProperties.Set("disable_parent", false)
				.SetDocumentation("Disable Parenting", "Parenting", "All parenting inherited from level will be disabled.");


			events.AddEvent("room_start", "Room Start", "Transition", "Occurs when the room begins.");
			events.AddEvent("all_monsters_dead", "All Monsters Dead", "Monster", "Occurs when all monsters are dead.");
		}

		public Room(Level level, int x, int y, Zone zone = null) :
			this(level, new Point2I(x, y), zone)
		{
		}

		public Room(Level level, Point2I location, Zone zone = null) :
			this()
		{
			this.level		= level;
			this.location	= location;
			this.tileData	= new TileDataInstance[level.RoomSize.X, level.RoomSize.Y, level.RoomLayerCount];
			//this.zone		= zone;

			//properties.BaseProperties.Set("event_all_monsters_dead", "")
			//	.SetDocumentation("All Monsters Dead", "script", "", "Events", "Occurs when all monsters are dead.");

			// Room Flags:
			// - discovered
			// - hiddenFromMap
			// - boss
			// - trasure
			// - signal

			/*if (zone != null)
				this.properties.Set("zone", zone.ID);*/
		}

		public Room(Room copy) :
			this()
		{
			properties	= new Properties(copy.properties, this);
			events		= new EventCollection(copy.events, this);

			this.level		= copy.level;
			this.location	= copy.location;
			this.tileData	= new TileDataInstance[level.RoomSize.X, level.RoomSize.Y, level.RoomLayerCount];
			for (int layer = 0; layer < LayerCount; layer++) {
				for (int x = 0; x < Width; x++) {
					for (int y = 0; y < Height; y++) {
						TileDataInstance tile = copy.tileData[x, y, layer];
						if (tile != null && tile.IsAtLocation(x, y))
							PlaceTile(new TileDataInstance(tile), x, y, layer);
					}
				}
			}
			foreach (ActionTileDataInstance actionTile in copy.actionData) {
				AddActionTile(new ActionTileDataInstance(actionTile));
			}
		}


		//-----------------------------------------------------------------------------
		// Property objects
		//-----------------------------------------------------------------------------

		public IEnumerable<IPropertyObject> GetPropertyObjects() {
			yield return this;
			for (int layer = 0; layer < LayerCount; layer++) {
				for (int x = 0; x < Width; x++) {
					for (int y = 0; y < Height; y++) {
						TileDataInstance tile = tileData[x, y, layer];
						if (tile != null && tile.IsAtLocation(x, y))
							yield return tile;
					}
				}
			}
			foreach (ActionTileDataInstance actionTile in actionData) {
				yield return actionTile;
			}
		}

		public IEnumerable<ITriggerObject> GetEventObjects() {
			yield return this;
			for (int layer = 0; layer < LayerCount; layer++) {
				for (int x = 0; x < Width; x++) {
					for (int y = 0; y < Height; y++) {
						TileDataInstance tile = tileData[x, y, layer];
						if (tile != null && tile.IsAtLocation(x, y))
							yield return tile;
					}
				}
			}
			foreach (ActionTileDataInstance actionTile in actionData) {
				yield return actionTile;
			}
		}


		//-----------------------------------------------------------------------------
		// Accessors
		//-----------------------------------------------------------------------------

		// Tiles ----------------------------------------------------------------------

		public TileDataInstance GetTile(Point2I location, int layer,
			bool includeShared = false)
		{
			TileDataInstance tile = tileData[location.X, location.Y, layer];
			if (tile != null || !includeShared)
				return tile;

			Room parentRoom = ParentRoom;
			if (parentRoom != null)
				return parentRoom.GetSharedTile(location, layer);

			return null;
		}

		public TileDataInstance GetTile(int x, int y, int layer,
			bool includeShared = false)
		{
			return GetTile(new Point2I(x, y), layer);
		}

		public TileDataInstance GetSharedTile(Point2I location, int layer) {
			TileDataInstance tile = tileData[location.X, location.Y, layer];
			if (tile != null && tile.IsShared)
				return tile;

			/*Room parentRoom = ParentRoom;
			if (parentRoom != null)
				return parentRoom.GetSharedTile(location, layer);*/

			return null;
		}

		public IEnumerable<TileDataInstance> GetTilesInArea(Rectangle2I area, int layer) {
			HashSet<TileDataInstance> encounteredTiles = new HashSet<TileDataInstance>();
			for (int x = GMath.Max(0, -area.X); x < area.Width && x + area.X < Width; x++) {
				for (int y = GMath.Max(0, -area.Y); y < area.Height && y + area.Y < Height; y++) {
					TileDataInstance tile = tileData[area.X + x, area.Y + y, layer];
					if (tile != null && !encounteredTiles.Contains(tile)) {
						encounteredTiles.Add(tile);
						yield return tile;
					}
				}
			}
		}

		public bool ContainsTile(TileDataInstance tile,
			bool includeShared = false)
		{
			if (tile == tileData[tile.Location.X, tile.Location.Y, tile.Layer])
				return true;

			if (!includeShared || !tile.IsShared)
				return false;
			
			Room parentRoom = ParentRoom;
			if (parentRoom != null) {
				return parentRoom.ContainsSharedTile(tile);
			}
			return false;
		}

		/// <summary>Returns true if this room contains a shared tile at the location.</summary>
		public bool ContainsSharedTile(TileDataInstance tile) {
			if (!tile.IsShared)
				return false;

			return (tile == tileData[tile.Location.X, tile.Location.Y, tile.Layer]);
			//	return true;

			/*Room parentRoom = ParentRoom;
			if (parentRoom != null) {
				return parentRoom.ContainsSharedTile(tile);
			}
			return false;*/
		}

		public TileDataInstance FindTileOfTypeByID<TileType>(string tileID,
			bool includeShared = true)
		{
			foreach (TileDataInstance tile in GetTiles(includeShared)) {
				if (tile.ID == tileID && TypeHelper.TypeHasBase<TileType>(tile.Type))
					return tile;
			}
			return null;
		}

		public TileDataInstance FindTileByID(string tileID,
			bool includeShared = true)
		{
			foreach (TileDataInstance tile in GetTiles(includeShared)) {
				if (tile.ID == tileID)
					return tile;
			}
			return null;
		}

		public IEnumerable<TileDataInstance> FindTilesByID(string tileID,
			bool includeShared = true)
		{
			foreach (TileDataInstance tile in GetTiles(includeShared)) {
				if (tile.ID == tileID)
					yield return tile;
			}
		}

		/// <summary>Gets all tiles in the room.</summary>
		public IEnumerable<TileDataInstance> GetTiles(bool includeShared = false) {
			// Get this rooms tiles
			for (int layer = 0; layer < LayerCount; layer++) {
				for (int x = 0; x < Width; x++) {
					for (int y = 0; y < Height; y++) {
						TileDataInstance tile = tileData[x, y, layer];
						if (tile != null && tile.IsAtLocation(x, y))
							yield return tile;
					}
				}
			}

			if (!includeShared)
				yield break;

			// Get the parent room's shared tiles
			Room parentRoom = ParentRoom;
			if (parentRoom != null) {
				foreach (TileDataInstance tile in parentRoom.GetSharedTiles()) {
					if (IsTileAreaClear(tile))
						yield return tile;
				}
			}
		}

		/// <summary>Gets all tiles in the room's layer.</summary>
		public IEnumerable<TileDataInstance> GetTileLayer(int layer,
			bool includeShared = false)
		{
			// Get this rooms tiles
			for (int x = 0; x < Width; x++) {
				for (int y = 0; y < Height; y++) {
					TileDataInstance tile = tileData[x, y, layer];
					if (tile != null && tile.IsAtLocation(x, y))
						yield return tile;
				}
			}

			if (!includeShared)
				yield break;

			// Get the parent room's shared tiles
			Room parentRoom = ParentRoom;
			if (parentRoom != null) {
				foreach (TileDataInstance tile in
					parentRoom.GetSharedTileLayer(layer))
				{
					if (IsTileAreaClear(tile))
						yield return tile;
				}
			}
		}

		/// <summary>Gets all shared parented tiles in the room.</summary>
		public IEnumerable<TileDataInstance> GetParentTiles() {
			Room parentRoom = ParentRoom;
			if (parentRoom != null) {
				foreach (TileDataInstance tile in parentRoom.GetSharedTiles()) {
					if (IsTileAreaClear(tile))
						yield return tile;
				}
			}
		}

		/// <summary>Gets all shared parented tiles in the room.</summary>
		public IEnumerable<TileDataInstance> GetParentTileLayer(int layer) {
			Room parentRoom = ParentRoom;
			if (parentRoom != null) {
				foreach (TileDataInstance tile in
					parentRoom.GetSharedTileLayer(layer))
				{
					if (IsTileAreaClear(tile))
						yield return tile;
				}
			}
		}

		/// <summary>Gets all shared current and parented tiles in the room.</summary>
		public IEnumerable<TileDataInstance> GetSharedTiles() {
			// Get the this room's non-unique tiles
			foreach (TileDataInstance tile in GetTiles()) {
				if (tile.IsShared)
					yield return tile;
			}

			// Get the parent room's shared tiles
			/*Room parentRoom = ParentRoom;
			if (parentRoom != null) {
				foreach (TileDataInstance tile in
					parentRoom.GetSharedTiles())
				{
					if (IsTileAreaClear(tile))
						yield return tile;
				}
			}*/
		}

		/// <summary>Gets all shared current and parented tiles in the room's layer.</summary>
		public IEnumerable<TileDataInstance> GetSharedTileLayer(int layer) {
			// Get the this room's non-unique tiles
			foreach (TileDataInstance tile in GetTileLayer(layer)) {
				if (tile.IsShared)
					yield return tile;
			}

			// Get the parent room's shared tiles
			/*Room parentRoom = ParentRoom;
			if (parentRoom != null) {
				foreach (TileDataInstance tile in
					parentRoom.GetSharedTileLayer(layer)) {
					if (IsTileAreaClear(tile))
						yield return tile;
				}
			}*/
		}

		/// <summary>Checks if the area used by this tile is unoccupied.</summary>
		private bool IsTileAreaClear(TileDataInstance tile) {
			Point2I size = tile.TileSize;
			for (int x = 0; x < size.X; x++) {
				for (int y = 0; y < size.Y; y++) {
					if (tileData[x, y, tile.Layer] != null)
						return false;
				}
			}
			return true;
		}

		// Action Tiles ---------------------------------------------------------------

		/// <summary>Gets all action tiles in the room.</summary>
		public IEnumerable<ActionTileDataInstance> GetActionTiles(
			bool includeShared = false)
		{
			// Get this rooms action tiles
			foreach (ActionTileDataInstance action in actionData) {
				yield return action;
			}

			if (!includeShared)
				yield break;

			// Get the parent room's shared action tiles
			Room parentRoom = ParentRoom;
			if (parentRoom != null) {
				foreach (ActionTileDataInstance action in
					parentRoom.GetSharedActionTiles())
				{
					yield return action;
				}
			}
		}

		/// <summary>Gets the index of the action tile in the room's list.</summary>
		public int IndexOfActionTile(ActionTileDataInstance actionTile) {
			return actionData.IndexOf(actionTile);
		}

		/// <summary>Gets the action tile at the specified index in the list.</summary>
		public ActionTileDataInstance GetActionTileAt(int index) {
			return actionData[index];
		}

		/// <summary>Gets all action tiles at the specified position in the room.</summary>
		public IEnumerable<ActionTileDataInstance> GetActionTilesAt(Point2I position,
			bool includeShared = false) {
			foreach (ActionTileDataInstance action in GetActionTiles(includeShared)) {
				if (action.Bounds.Contains(position))
					yield return action;
			}
		}

		/// <summary>Gets the first action tile at the specified position in the room.</summary>
		public ActionTileDataInstance GetActionTileAt(Point2I position,
			bool includeShared = false)
		{
			foreach (ActionTileDataInstance action in GetActionTiles(includeShared)) {
				if (action.Bounds.Contains(position))
					return action;
			}
			return null;
		}

		/// <summary>Gets all shared parented action tiles in the room.</summary>
		public IEnumerable<ActionTileDataInstance> GetParentActionTiles() {
			Room parentRoom = ParentRoom;
			if (parentRoom != null) {
				return parentRoom.GetSharedActionTiles();
			}
			return Enumerable.Empty<ActionTileDataInstance>();
		}

		/// <summary>Gets all shared current and parented action tiles in the room.</summary>
		public IEnumerable<ActionTileDataInstance> GetSharedActionTiles() {
			// Get the this room's shared action tiles
			foreach (ActionTileDataInstance action in actionData) {
				if (action.IsShared)
					yield return action;
			}

			// Get the parent room's shared action tiles
			/*Room parentRoom = ParentRoom;
			if (parentRoom != null) {
				foreach (ActionTileDataInstance action in
					parentRoom.GetSharedActionTiles())
				{
					yield return action;
				}
			}*/
		}

		/// <summary>Returns true if this room contains the action tile.</summary>
		public bool ContainsActionTile(ActionTileDataInstance action,
			bool includeShared = false)
		{
			if (actionData.Contains(action))
				return true;

			if (!includeShared || !action.IsShared)
				return false;

			Room parentRoom = ParentRoom;
			if (parentRoom != null)
				return parentRoom.ContainsSharedActionTile(action);

			return false;
		}

		/// <summary>Returns true if this room contains the shared action tile.</summary>
		public bool ContainsSharedActionTile(ActionTileDataInstance action) {
			if (!action.IsShared)
				return false;
			
			return actionData.Contains(action);

			/*Room parentRoom = ParentRoom;
			if (parentRoom != null) {
				return parentRoom.ContainsSharedActionTile(tile);
			}
			return false;*/
		}

		public ActionTileDataInstance FindActionTileByID(string actionTileID,
			bool includeShared = true)
		{
			var action = actionData.Find(actionTile => actionTile.ID == actionTileID);
			if (action != null)
				return action;

			if (!includeShared)
				return null;

			Room parentRoom = ParentRoom;
			if (parentRoom != null) {
				return parentRoom.actionData.Find(actionTile =>
					actionTile.ID == actionTileID && actionTile.IsShared);
			}

			return null;
		}

		// All Tiles ------------------------------------------------------------------

		/// <summary>Gets all tiles and action tiles in the room.</summary>
		public IEnumerable<BaseTileDataInstance> GetAllTiles(
			bool includeShared = false)	
		{
			foreach (TileDataInstance tile in GetTiles(includeShared)) {
				yield return tile;
			}

			foreach (ActionTileDataInstance action in GetActionTiles(includeShared)) {
				yield return action;
			}
		}

		/// <summary>Returns true if any unlooted reward tiles exist in the room.</summary>
		public bool HasUnlootedRewards() {
			return GetAllTiles(true).Any(t => t.IsUnlootedReward);
		}

		
		//-----------------------------------------------------------------------------
		// Mutators
		//-----------------------------------------------------------------------------

		// Tiles ----------------------------------------------------------------------
		
		/// <summary>Places the tile at the specified location.</summary>
		public void PlaceTile(TileInstanceLocation tile) {
			PlaceTile(tile.Tile, tile.Location, tile.Layer);
		}

		/// <summary>Places the tile at the specified location.</summary>
		public void PlaceTile(TileDataInstance tile, int x, int y, int layer) {
			PlaceTile(tile, new Point2I(x, y), layer);
		}

		/// <summary>Places the tile at the specified location.</summary>
		public void PlaceTile(TileDataInstance tile, Point2I location, int layer) {
			Point2I size = (tile != null ? tile.TileSize : Point2I.One);
			for (int x = 0; x < size.X; x++) {
				for (int y = 0; y < size.Y; y++) {
					Point2I loc = new Point2I(location.X + x, location.Y + y);
					if (loc.X < Width && loc.Y < Height) {
						// Remove existing tile.
						TileDataInstance t = tileData[loc.X, loc.Y, layer];
						if (t != null)
							RemoveTile(t);
						tileData[loc.X, loc.Y, layer] = tile;
					}
				}
			}
			if (tile != null) {
				tile.Location	= location;
				tile.Layer		= layer;
				tile.Room		= this;
			}
		}

		/// <summary>Removes the tile from the room.</summary>
		public void RemoveTile(TileDataInstance tile) {
			if (tile.Room == this) {
				Point2I size = tile.TileSize;
				for (int x = 0; x < size.X; x++) {
					for (int y = 0; y < size.Y; y++) {
						Point2I loc = new Point2I(tile.Location.X + x, tile.Location.Y + y);
						if (loc.X < Width && loc.Y < Height && tileData[loc.X, loc.Y, tile.Layer] == tile)
							tileData[loc.X, loc.Y, tile.Layer] = null;
					}
				}
			}
		}

		/// <summary>Removes the tile at the specified location from the room.</summary>
		public void RemoveTile(int x, int y, int layer) {
			TileDataInstance tile = tileData[x, y, layer];
			if (tile != null)
				RemoveTile(tile);
		}

		/// <summary>Removes the tile at the specified location from the room.</summary>
		public void RemoveTile(Point2I location, int layer) {
			TileDataInstance tile = tileData[location.X, location.Y, layer];
			if (tile != null)
				RemoveTile(tile);
		}

		/// <summary>Removes the tile or action tile at the specified location from the room.</summary>
		public void Remove(BaseTileDataInstance tile) {
			if (tile is TileDataInstance)
				RemoveTile((TileDataInstance) tile);
			else if (tile is ActionTileDataInstance)
				RemoveActionTile((ActionTileDataInstance) tile);
		}

		/// <summary>Creates a tile at the specified location in the room.</summary>
		public TileDataInstance CreateTile(TileData data, int x, int y, int layer) {
			return CreateTile(data, new Point2I(x, y), layer);
		}

		/// <summary>Creates a tile at the specified location in the room.</summary>
		public TileDataInstance CreateTile(TileData data, Point2I location, int layer) {
			TileDataInstance dataInstance = null;
			if (data != null) {
				dataInstance = new TileDataInstance(data, location, layer);
				dataInstance.Room = this;
			}
			PlaceTile(dataInstance, location, layer);
			return dataInstance;
		}

		/// <summary>Updates the area the tile contains in the room based on its size.</summary>
		public void UpdateTileSize(TileDataInstance tile, Point2I oldSize) {
			if (ContainsTile(tile)) {
				Point2I newSize = tile.TileSize;
				tile.TileSize = oldSize;
				RemoveTile(tile);
				tile.TileSize = newSize;
				PlaceTile(tile, tile.Location, tile.Layer);
			}
		}

		// Action Tiles ---------------------------------------------------------------
		
		/// <summary>Places an action tile at the specified position in the room.</summary>
		public void PlaceActionTile(ActionTileInstancePosition action) {
			PlaceActionTile(action.Action, action.Position);
		}

		/// <summary>Places an action tile at the specified position in the room.</summary>
		public void PlaceActionTile(ActionTileDataInstance action, int x, int y) {
			PlaceActionTile(action, new Point2I(x, y));
		}

		/// <summary>Places an action tile at the specified position in the room.</summary>
		public void PlaceActionTile(ActionTileDataInstance action, Point2I position) {
			action.Position = position;
			AddActionTile(action);
		}

		/// <summary>Creates an action tile at the specified position in the room.</summary>
		public ActionTileDataInstance CreateActionTile(ActionTileData data,
			int x, int y)
		{
			return CreateActionTile(data, new Point2I(x, y));
		}

		/// <summary>Creates an action tile at the specified position in the room.</summary>
		public ActionTileDataInstance CreateActionTile(ActionTileData data,
			Point2I position)
		{
			ActionTileDataInstance action = new ActionTileDataInstance(data, position);
			AddActionTile(action);
			return action;
		}

		/// <summary>Adds an action tile to the room.</summary>
		public void AddActionTile(ActionTileDataInstance action) {
			actionData.Add(action);
			action.Room = this;
		}

		/// <summary>Inserts an action tile into the room.</summary>
		public void InsertActionTile(int index, ActionTileDataInstance action) {
			actionData.Insert(index, action);
			action.Room = this;
		}

		/// <summary>Removes the action tile from the room.</summary>
		public void RemoveActionTile(ActionTileDataInstance action) {
			actionData.Remove(action);
		}


		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------

		/// <summary>Resizes the number of tile layers in the room.</summary>
		internal void ResizeLayerCount(int newLayerCount) {
			TileDataInstance[,,] oldTileData = tileData;
			tileData = new TileDataInstance[level.RoomSize.X, level.RoomSize.Y, newLayerCount];
			int minLayerCount = GMath.Min(LayerCount, newLayerCount);
			for (int x = 0; x < level.RoomSize.X; x++) {
				for (int y = 0; y < level.RoomSize.Y; y++) {
					for (int layer = 0; layer < minLayerCount; layer++) {
						tileData[x, y, layer] = oldTileData[x, y, layer];
					}
				}
			}
		}


		//-----------------------------------------------------------------------------
		// Special In-Game Methods
		//-----------------------------------------------------------------------------
		
		/// <summary>Called when the room is left by the player.</summary>
		public void OnLeaveRoom() {
			// Reset tile states and respawn "Always" monsters
			foreach (TileDataInstance tile in GetTiles()) {
				if (tile.ResetCondition == TileResetCondition.LeaveRoom)
					tile.ResetState();
			}

			// Reset action tile states and respawn "Always" monsters
			foreach (ActionTileDataInstance tile in actionData) {
				if (tile.ResetCondition == TileResetCondition.LeaveRoom)
					tile.ResetState();
			}
		}

		/// <summary>Called when the area is left by the player.</summary>
		public void OnLeaveArea() {
			// Reset tile states and respawn "Normal" monsters
			foreach (TileDataInstance tile in GetTiles()) {
				if (tile.ResetCondition == TileResetCondition.LeaveArea)
					tile.ResetState();
			}

			// Reset action tile states and respawn "Normal" monsters
			foreach (ActionTileDataInstance tile in actionData) {
				if (tile.ResetCondition == TileResetCondition.LeaveArea)
					tile.ResetState();
			}
		}

		/// <summary>Assigns a unique ID to all monsters in the room.</summary>
		internal void AssignMonsterIDs() {
			foreach (BaseTileDataInstance tile in GetAllTiles()) {
				if (tile.IsMonster && tile.MonsterID == 0)
					tile.MonsterID = level.World.NextMonsterID();
			}
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------
		
		/// <summary>Gets or sets the level containing this room.</summary>
		public Level Level {
			get { return level; }
			set { level = value; }
		}
		
		/// <summary>Gets or sets the location of the room in the level.</summary>
		public Point2I Location {
			get { return location; }
			set { location = value; }
		}

		/// <summary>Gets the coordinates of the room in tiles from the start of the
		/// level.</summary>
		public Point2I LevelCoord {
			get { return location * level.RoomSize; }
		}

		/// <summary>Gets the pixel position of the room from the start of the
		/// level.</summary>
		public Point2I LevelPosition {
			get { return LevelCoord * GameSettings.TILE_SIZE; }
		}

		/// <summary>Gets or sets the zone assigned to this room.</summary>
		public Zone Zone {
			get {
				Zone zone = properties.GetResource<Zone>("zone", null);
				return zone ?? level.Zone;
			}
			set {
				if (value != null)
					properties.Set("zone", value.ID);
				else
					properties.Set("zone", "");
			}
		}

		/// <summary>Gets the area assigned to this room.</summary>
		public Area Area {
			get { return level.Area; }
		}

		/// <summary>Gets or sets the ID of this room.</summary>
		public string ID {
			get { return properties.GetString("id"); }
			set { properties.Set("id", value); }
		}

		// Dimensions -----------------------------------------------------------------

		/// <summary>Gets the size of the room in tiles.</summary>
		public Point2I Size {
			get { return new Point2I(tileData.GetLength(0), tileData.GetLength(1)); }
		}

		/// <summary>Gets the width of the room in tiles.</summary>
		public int Width {
			get { return tileData.GetLength(0); }
		}

		/// <summary>Gets the height of the room in tiles.</summary>
		public int Height {
			get { return tileData.GetLength(1); }
		}

		/// <summary>Gets the size of the room in pixels.</summary>
		public Point2I PixelSize {
			get { return Size * GameSettings.TILE_SIZE; }
		}

		/// <summary>Gets the boundaries of the room in pixels.</summary>
		public Rectangle2I Bounds {
			get { return new Rectangle2I(PixelSize); }
		}

		/// <summary>Gets the boundaries of the room in the level in tiles.</summary>
		public Rectangle2I LevelTileBounds {
			get { return new Rectangle2I(Location, Size); }
		}

		/// <summary>Gets the boundaries of the room in the level in pixels.</summary>
		public Rectangle2I LevelBounds {
			get { return new Rectangle2I(LevelPosition, PixelSize); }
		}

		// Tiles ----------------------------------------------------------------------

		/// <summary>Gets the number of tile layers in the room.</summary>
		public int LayerCount {
			get { return tileData.GetLength(2); }
		}

		/// <summary>Gets the index of the bottom tile layer in the room.</summary>
		public int BottomLayer {
			get { return 0; }
		}

		/// <summary>Gets the index of the top tile layer in the room.</summary>
		public int TopLayer {
			get { return (level.RoomLayerCount - 1); }
		}

		/// <summary>Gets the number of action tiles in the room.</summary>
		public int ActionCount {
			get { return actionData.Count; }
		}
		
		// Parenting ------------------------------------------------------------------

		/// <summary>Gets or sets if the room should disable any parenting inherited
		/// from its level.</summary>
		public bool DisableParenting {
			get { return properties.Get("disable_parent", false); }
			set { properties.Set("disable_parent", true); }
		}

		/// <summary>Gets or sets the parent level that this level shares
		/// certain settings with like room clearing.</summary>
		public Level ParentLevel {
			get {
				if (DisableParenting)
					return null;
				Level parentLevel = level.World.GetLevel(
					properties.GetString("parent_level", ""));
				return parentLevel ?? level.ParentLevel;
			}
			set {
				if (value == null)
					properties.Set("parent_level", "");
				else
					properties.Set("parent_level", value.ID);
			}
		}

		/// <summary>Gets or sets the parent room location that this room
		/// shares certain settings with like room clearing.</summary>
		public Point2I ParentLocation {
			get {
				if (DisableParenting)
					return -Point2I.One;
				return properties.GetPoint("parent_location", -Point2I.One);
			}
			set { properties.Set("parent_location", value); }
		}

		/// <summary>Gets the root location of the room.</summary>
		public Point2I RootLocation {
			get {
				Point2I parentLocation = ParentLocation;
				if (parentLocation != -Point2I.One)
					return parentLocation;
				return location;
			}
		}

		/// <summary>Gets the parent room identifier that this room shares
		/// certain settings with like room clearing.</summary>
		public Room ParentRoom {
			get {
				Level parentLevel = ParentLevel ?? level;
				Point2I parentLocation = ParentLocation;
				if (parentLocation == -Point2I.One)
					parentLocation = location;

				if ((parentLevel != level || parentLocation != location) &&
					parentLevel.ContainsRoom(parentLocation))
					return parentLevel.GetRoomAt(parentLocation);
				return null;
			}
		}

		/// <summary>Gets the root room identifier that this room shares
		/// certain settings with like room clearing.</summary>
		public Room RootRoom {
			get {
				Room parentRoom = ParentRoom;
				if (parentRoom != null)
					return parentRoom;//.RootRoom;
				return this;
			}
		}

		// Properties -----------------------------------------------------------------

		/// <summary>Gets or sets the properties for the room.</summary>
		public Properties Properties {
			get { return properties; }
			set {
				properties = value;
				properties.PropertyObject = this;
			}
		}

		/// <summary>Gets the variables for the room.</summary>
		public Variables Vars {
			get { return variables; }
		}

		/// <summary>Gets the events for the room.</summary>
		public EventCollection Events {
			get { return events; }
		}
		
		public TriggerCollection Triggers {
			get { return triggers; }
		}

		public Type TriggerObjectType {
			get { return typeof(ZeldaAPI.RoomTODO); }
		}

		// Settings -------------------------------------------------------------------

		/// <summary>Gets or sets if the root room has been discovered.</summary>
		public bool IsDiscovered {
			get { return RootRoom.properties.GetBoolean("discovered", false); }
			set { RootRoom.properties.Set("discovered", value); }
		}

		/// <summary>Gets or sets if the root room is hidden from the map.</summary>
		public bool IsHiddenFromMap {
			get { return RootRoom.properties.GetBoolean("hidden_from_map", false); }
			set { RootRoom.properties.Set("hidden_from_map", value); }
		}

		/// <summary>Gets or sets if the root room is a boss room.</summary>
		public bool IsBossRoom {
			get { return RootRoom.properties.GetBoolean("boss_room", false); }
			set { RootRoom.properties.Set("boss_room", value); }
		}

		/// <summary>Gets or sets if the player dies when falling out of bounds
		/// while in side-scrolling mode.</summary>
		public bool DeathOutOfBounds {
			get { return properties.GetBoolean("death_out_of_bounds", false); }
			set { properties.Set("death_out_of_bounds", value); }
		}

		/// <summary>Gets or sets the spawn methods for monsters in this room.</summary>
		public MonsterSpawnMode SpawnMode {
			get {
				var spawnMode = properties.GetEnum("spawn_mode", MonsterSpawnMode.Random);
				if (spawnMode != MonsterSpawnMode.Normal)
					return spawnMode;
				return Area.SpawnMode;
			}
			set { properties.SetEnum("spawn_mode", value); }
		}
	}
}
