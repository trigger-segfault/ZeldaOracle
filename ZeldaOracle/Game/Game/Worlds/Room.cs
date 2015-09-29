using ZeldaOracle.Common.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Tiles.EventTiles;

namespace ZeldaOracle.Game.Worlds {
	public class Room {
		
		private Level							level;		// The level this room is in.
		private Point2I							location;	// Location within the level.
		private Point2I							size;		// Size of the tile grid.
		private int								layerCount;	// Number of tile layers.
		private TileDataInstance[,,]			tileData;	// 3D grid of tile data (x, y, layer)
		private List<EventTileDataInstance>		eventData;
		private Zone							zone;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public Room(Level level, int x, int y) {
			this.level		= level;
			this.location	= new Point2I(x, y);
			this.size		= level.RoomSize;
			this.layerCount	= GameSettings.DEFAULT_TILE_LAYER_COUNT; // Default tile layers.
			this.tileData	= new TileDataInstance[size.X, size.Y, layerCount];
			this.eventData	= new List<EventTileDataInstance>();
			this.zone		= null;
		}
		

		//-----------------------------------------------------------------------------
		// Accessors
		//-----------------------------------------------------------------------------

		public TileDataInstance GetTile(int x, int y, int layer) {
			return tileData[x, y, layer];
		}


		//-----------------------------------------------------------------------------
		// Mutators
		//-----------------------------------------------------------------------------
		
		public void SetTile(TileDataInstance tile, int x, int y, int layer) {
			tileData[x, y, layer] = tile;
			tile.Location	= new Point2I(x, y);
			tile.Layer		= layer;
			tile.Room		= this;
		}

		public TileDataInstance CreateTile(TileData data, int x, int y, int layer) {
			TileDataInstance dataInstance = new TileDataInstance(data, x, y, layer);
			dataInstance.Room = this;
			tileData[x, y, layer] = dataInstance;
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
			get { return size; }
		}

		public int Width {
			get { return size.X; }
		}

		public int Height {
			get { return size.Y; }
		}

		public int LayerCount {
			get { return layerCount; }
		}

		public Zone Zone {
			get { return zone; }
			set { zone = value; }
		}
	}
}
