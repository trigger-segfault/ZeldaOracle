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

namespace ZeldaOracle.Game.Worlds {
	public class Room : IEventObjectContainer, IEventObject {

		private Level							level;		// The level this room is in.
		private Point2I							location;	// Location within the level.
		private TileDataInstance[,,]			tileData;	// 3D grid of tile data (x, y, layer)
		private List<ActionTileDataInstance>	actionData;
		//private Zone							zone;
		private Properties						properties;
		private EventCollection					events;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public Room() {
			this.level		= null;
			this.location	= Point2I.Zero;
			this.tileData	= new TileDataInstance[0, 0, 0];
			this.actionData	= new List<ActionTileDataInstance>();

			this.events		= new EventCollection(this);
			this.properties	= new Properties(this);
			this.properties.BaseProperties = new Properties();

			properties.BaseProperties.Set("id", "")
				.SetDocumentation("ID", "", "", "General", "The id used to refer to this room.");
			properties.BaseProperties.Set("music", "")
				.SetDocumentation("Music", "song", "", "General", "The music to play in this room. Select none to choose the default music.", true, false);
			properties.BaseProperties.Set("zone", "")
				.SetDocumentation("Zone", "zone", "", "General", "The zone type for this room.");

			properties.BaseProperties.Set("discovered", false)
				.SetDocumentation("Discovered", "Progress", "True if the room has been visited at least once.");
			properties.BaseProperties.Set("hidden_from_map", false)
				.SetDocumentation("Hidden From Map", "Dungeon", "True if this room does not appear on the map even when visited.");
			properties.BaseProperties.Set("boss_room", false)
				.SetDocumentation("Is Boss Room", "Dungeon", "True if this room is shown as the boss room in the dungeon map.");

			properties.BaseProperties.Set("death_out_of_bounds", false)
				.SetDocumentation("Death out of Bounds", "Side-Scrolling", "True if the player dies and respawns when falling off the edge of the map.");


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
			properties.SetAll(copy.properties);
			events.SetAll(copy.events);

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

		public IEnumerable<IEventObject> GetEventObjects() {
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

		public TileDataInstance GetTile(Point2I location, int layer) {
			return tileData[location.X, location.Y, layer];
		}

		public TileDataInstance GetTile(int x, int y, int layer) {
			return tileData[x, y, layer];
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

		public bool ContainsTile(TileDataInstance tile) {
			return (tile == tileData[tile.Location.X, tile.Location.Y, tile.Layer]);
		}

		public bool ContainsActionTile(ActionTileDataInstance actionTile) {
			return actionData.Contains(actionTile);
		}

		public ActionTileDataInstance FindActionTileByID(string actionTileID) {
			return actionData.Find(actionTile => actionTile.ID == actionTileID);
		}

		public TileDataInstance FindTileOfTypeByID<TileType>(string tileID) {
			for (int layer = 0; layer < LayerCount; layer++) {
				for (int x = 0; x < Width; x++) {
					for (int y = 0; y < Height; y++) {
						TileDataInstance tile = tileData[x, y, layer];
						if (tile != null && tile.IsAtLocation(x, y) &&
							tile.ID == tileID && tile.Type.Equals(typeof(TileType)))
							return tile;
					}
				}
			}
			return null;
		}

		public TileDataInstance FindTileByID(string tileID) {
			for (int layer = 0; layer < LayerCount; layer++) {
				for (int x = 0; x < Width; x++) {
					for (int y = 0; y < Height; y++) {
						TileDataInstance tile = tileData[x, y, layer];
						if (tile != null && tile.IsAtLocation(x, y) && tile.ID == tileID)
							return tile;
					}
				}
			}
			return null;
		}

		public IEnumerable<TileDataInstance> FindTilesByID(string tileID) {
			for (int layer = 0; layer < LayerCount; layer++) {
				for (int x = 0; x < Width; x++) {
					for (int y = 0; y < Height; y++) {
						TileDataInstance tile = tileData[x, y, layer];
						if (tile != null && tile.IsAtLocation(x, y) && tile.ID == tileID)
							yield return tile;
					}
				}
			}
		}

		public IEnumerable<TileDataInstance> GetTiles() {
			for (int layer = 0; layer < LayerCount; layer++) {
				for (int x = 0; x < Width; x++) {
					for (int y = 0; y < Height; y++) {
						TileDataInstance tile = tileData[x, y, layer];
						if (tile != null && tile.IsAtLocation(x, y))
							yield return tile;
					}
				}
			}
		}

		public bool HasUnopenedTreasure() {
			foreach (TileDataInstance tile in GetTiles()) {
				if (tile.Type == typeof(TileChest) && !tile.Properties.GetBoolean("looted", false))
					return true;
				if (tile.Type == typeof(TileReward) && !tile.Properties.GetBoolean("looted", false))
					return true;
			}
			return false;
		}


		//-----------------------------------------------------------------------------
		// Tile Management
		//-----------------------------------------------------------------------------
		
		public void PlaceTile(TileDataInstance tile, int x, int y, int layer) {
			PlaceTile(tile, new Point2I(x, y), layer);
		}

		public void PlaceTile(TileDataInstance tile, Point2I location, int layer) {
			Point2I size = (tile != null ? tile.Size : Point2I.One);
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

		public void RemoveTile(TileDataInstance tile) {
			if (tile.Room == this) {
				Point2I size = tile.Size;
				for (int x = 0; x < size.X; x++) {
					for (int y = 0; y < size.Y; y++) {
						Point2I loc = new Point2I(tile.Location.X + x, tile.Location.Y + y);
						if (loc.X < Width && loc.Y < Height && tileData[loc.X, loc.Y, tile.Layer] == tile)
							tileData[loc.X, loc.Y, tile.Layer] = null;
					}
				}
			}
		}

		public void RemoveTile(Point2I location, int layer) {
			TileDataInstance tile = tileData[location.X, location.Y, layer];
			if (tile != null)
				RemoveTile(tile);
		}

		public void RemoveTile(int x, int y, int layer) {
			TileDataInstance tile = tileData[x, y, layer];
			if (tile != null)
				RemoveTile(tile);
		}

		public void Remove(BaseTileDataInstance tile) {
			if (tile is TileDataInstance)
				RemoveTile((TileDataInstance) tile);
			else if (tile is ActionTileDataInstance)
				RemoveActionTile((ActionTileDataInstance) tile);
		}

		public TileDataInstance CreateTile(TileData data, Point2I location, int layer) {
			return CreateTile(data, location.X, location.Y, layer);
		}

		public TileDataInstance CreateTile(TileData data, int x, int y, int layer) {
			TileDataInstance dataInstance = null;
			if (data != null) {
				dataInstance = new TileDataInstance(data, x, y, layer);
				dataInstance.Room = this;
			}
			PlaceTile(dataInstance, new Point2I(x, y), layer);
			return dataInstance;
		}

		public ActionTileDataInstance CreateActionTile(ActionTileData data, int x, int y) {
			return CreateActionTile(data, new Point2I(x, y));
		}

		public ActionTileDataInstance CreateActionTile(ActionTileData data, Point2I position) {
			ActionTileDataInstance dataInstance = new ActionTileDataInstance(data, position);
			AddActionTile(dataInstance);
			return dataInstance;
		}

		public void UpdateTileSize(TileDataInstance tile, Point2I oldSize) {
			if (ContainsTile(tile)) {
				Point2I newSize = tile.Size;
				tile.Size = oldSize;
				RemoveTile(tile);
				tile.Size = newSize;
				PlaceTile(tile, tile.Location, tile.Layer);
			}
		}
		
		public void AddActionTile(ActionTileDataInstance actionTile) {
			actionData.Add(actionTile);
			actionTile.Room = this;
		}
		
		public void RemoveActionTile(ActionTileDataInstance actionTile) {
			actionData.Remove(actionTile);
		}

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
		
		public void OnRoomLeave() {
			// Reset tile states.
			foreach (TileDataInstance tile in GetTiles()) {
				if (tile.ResetCondition == TileResetCondition.LeaveRoom)
					tile.ResetState();
			}

			// TODO: Reset action tile states.
		}

		public void RespawnMonsters() {
			// Reset tile states.
			foreach (TileDataInstance tile in GetTiles()) {
				if (tile.ResetCondition == TileResetCondition.LeaveArea)
					tile.ResetState();
			}

			// Reset action tile states.
			foreach (ActionTileDataInstance tile in actionData) {
				if (tile.Type == typeof(MonsterAction)) {
					tile.Properties.Set("dead", false);
				}
			}
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public TileDataInstance[,,] TileData {
			get { return tileData; }
			set { tileData = value; }
		}

		public List<ActionTileDataInstance> ActionData {
			get { return actionData; }
			set { actionData = value; }
		}

		public Level Level {
			get { return level; }
			set { level = value; }
		}

		public Point2I Location {
			get { return location; }
			set { location = value; }
		}

		public Point2I Size {
			get { return new Point2I(tileData.GetLength(0), tileData.GetLength(1)); }
		}

		public int Width {
			get { return tileData.GetLength(0); }
		}

		public int Height {
			get { return tileData.GetLength(1); }
		}

		public int LayerCount {
			get { return tileData.GetLength(2); }
		}

		public int BottomLayer {
			get { return 0; }
		}

		public int TopLayer {
			get { return (level.RoomLayerCount - 1); }
		}

		public Zone Zone {
			get {
				Zone zone = properties.GetResource<Zone>("zone", null);
				if (zone != null)
					return zone;
				return level.Zone;
			}
			set {
				if (value != null)
					properties.Set("zone", value.ID);
				else
					properties.Set("zone", "");
			}
		}


		public string ID {
			get { return properties.GetString("id"); }
			set { properties.Set("id", value); }
		}

		public Dungeon Dungeon {
			get { return level.Dungeon; }
		}

		public Properties Properties {
			get { return properties; }
			set {
				properties = value;
				properties.PropertyObject = this;
			}
		}
		
		public EventCollection Events {
			get { return events; }
		}
		
		public bool IsDiscovered {
			get { return properties.GetBoolean("discovered", false); }
			set { properties.Set("discovered", value); }
		}
		
		public bool IsHiddenFromMap {
			get { return properties.GetBoolean("hidden_from_map", false); }
			set { properties.Set("hidden_from_map", value); }
		}
		
		public bool IsBossRoom {
			get { return properties.GetBoolean("boss_room", false); }
			set { properties.Set("boss_room", value); }
		}

		public bool DeathOutOfBounds {
			get { return properties.GetBoolean("death_out_of_bounds", false); }
			set { properties.Set("death_out_of_bounds", value); }
		}
	}
}
