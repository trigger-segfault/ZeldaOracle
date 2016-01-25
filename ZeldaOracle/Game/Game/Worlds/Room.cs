using ZeldaOracle.Common.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Tiles.EventTiles;
using ZeldaOracle.Game.Tiles.Custom;
using ZeldaOracle.Common.Content;

namespace ZeldaOracle.Game.Worlds {
	public class Room : IPropertyObject, IEventObject {
		
		private Level							level;		// The level this room is in.
		private Point2I							location;	// Location within the level.
		private TileDataInstance[,,]			tileData;	// 3D grid of tile data (x, y, layer)
		private List<EventTileDataInstance>		eventData;
		//private Zone							zone;
		private Properties						properties;
		private ObjectEventCollection			events;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public Room(Level level, int x, int y, Zone zone = null) {
			this.level		= level;
			this.location	= new Point2I(x, y);
			this.tileData	= new TileDataInstance[level.RoomSize.X, level.RoomSize.Y, level.RoomLayerCount];
			this.eventData	= new List<EventTileDataInstance>();
			//this.zone		= zone;
			this.events		= new ObjectEventCollection();
			this.properties	= new Properties();
			this.properties.PropertyObject = this;
			this.properties.BaseProperties = new Properties();

			properties.BaseProperties.Set("id", "")
				.SetDocumentation("ID", "", "", "", "The id used to refer to this room.", true, false);
			properties.BaseProperties.Set("music", "")
				.SetDocumentation("Music", "song", "", "", "The music to play in this room. Select none to choose the default music.", true, false);
			properties.BaseProperties.Set("zone", "")
				.SetDocumentation("Zone", "zone", "", "", "The zone type for this room.", true, false);
			
			properties.BaseProperties.Set("discovered", false);
			properties.BaseProperties.Set("hidden_from_map", false);
			properties.BaseProperties.Set("boss_room", false);

			events.AddEvent("event_room_start", "Room Start", "Occurs when the room begins.");
			properties.BaseProperties.Set("event_room_start", "")
				.SetDocumentation("Room Start", "script", "", "Events", "Occurs when the room begins.");
			
			events.AddEvent("event_all_monsters_dead", "All Monsters Dead", "Occurs when all monsters are dead.");
			properties.BaseProperties.Set("event_all_monsters_dead", "")
				.SetDocumentation("All Monsters Dead", "script", "", "Events", "Occurs when all monsters are dead.");

			/*if (zone != null)
				this.properties.Set("zone", zone.ID);*/
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

		public EventTileDataInstance FindEventTileByID(string id) {
			for (int i = 0; i < eventData.Count; i++) {
				if (eventData[i].Id == id)
					return eventData[i];
			}
			return null;
		}

		public IEnumerable<TileDataInstance> GetTiles(string id) {
			for (int i = 0; i < LayerCount; i++) {
				for (int x = 0; x < Width; x++) {
					for (int y = 0; y < Height; y++) {
						TileDataInstance tile = tileData[x, y, i];
						if (tile != null && tile.Id == id)
							yield return tile;
					}
				}
			}
		}

		public IEnumerable<TileDataInstance> GetTiles() {
			for (int i = 0; i < LayerCount; i++) {
				for (int x = 0; x < Width; x++) {
					for (int y = 0; y < Height; y++) {
						TileDataInstance tile = tileData[x, y, i];
						if (tile != null)
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
			Point2I size = tile.Size;
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
						if (loc.X < Width && loc.Y < Height)
							tileData[loc.X, loc.Y, tile.Layer] = null;
					}
				}
			}
		}

		public void RemoveTile(int x, int y, int layer) {
			TileDataInstance tile = tileData[x, y, layer];
			if (tile != null)
				RemoveTile(tile);
		}

		public void Remove(BaseTileDataInstance tile) {
			if (tile is TileDataInstance)
				RemoveTile((TileDataInstance) tile);
			else if (tile is EventTileDataInstance)
				RemoveEventTile((EventTileDataInstance) tile);
		}

		public TileDataInstance CreateTile(TileData data, Point2I location, int layer) {
			return CreateTile(data, location.X, location.Y, layer);
		}

		public TileDataInstance CreateTile(TileData data, int x, int y, int layer) {
			TileDataInstance dataInstance = new TileDataInstance(data, x, y, layer);
			dataInstance.Room = this;
			PlaceTile(dataInstance, new Point2I(x, y), layer);
			return dataInstance;
		}

		public EventTileDataInstance CreateEventTile(EventTileData data, int x, int y) {
			return CreateEventTile(data, new Point2I(x, y));
		}

		public EventTileDataInstance CreateEventTile(EventTileData data, Point2I position) {
			EventTileDataInstance dataInstance = new EventTileDataInstance(data, position);
			AddEventTile(dataInstance);
			return dataInstance;
		}
		
		public void AddEventTile(EventTileDataInstance eventTile) {
			eventData.Add(eventTile);
			eventTile.Room = this;
		}
		
		public void RemoveEventTile(EventTileDataInstance eventTile) {
			eventData.Remove(eventTile);
		}


		//-----------------------------------------------------------------------------
		// Special In-Game Methods
		//-----------------------------------------------------------------------------

		public void RespawnMonsters() {
			foreach (EventTileDataInstance tile in eventData) {
				if (tile.Type == typeof(MonsterEvent)) {
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

		public List<EventTileDataInstance> EventData {
			get { return eventData; }
			set { eventData = value; }
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
			get { return level.RoomSize; }
		}

		public int Width {
			get { return level.RoomWidth; }
		}

		public int Height {
			get { return level.RoomHeight; }
		}

		public int LayerCount {
			get { return level.RoomLayerCount; }
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
			set {/*
				if (value != null)
					properties.Set("zone", value.ID);
				else
					properties.Set("zone", "");*/
			}
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
		
		public ObjectEventCollection Events {
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
	}
}
