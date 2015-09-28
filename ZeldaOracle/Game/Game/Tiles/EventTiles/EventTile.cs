using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Collision;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Properties;
using ZeldaOracle.Game.Main;
using ZeldaOracle.Game.Control;
using ZeldaOracle.Game.Entities.Effects;
using ZeldaOracle.Game.Entities.Projectiles;
using ZeldaOracle.Game.Worlds;

namespace ZeldaOracle.Game.Tiles.EventTiles {
	public class EventTile {
		
		private RoomControl		roomControl;
		private	EventTileData	eventData;
		protected Vector2F		position;
		protected Point2I		size;
		protected Properties	properties;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public EventTile() {
			roomControl		= null;
			eventData		= null;
			position		= Vector2F.Zero;
			size			= Point2I.One;
			properties		= new Properties();
		}
		

		//-----------------------------------------------------------------------------
		// Initialization
		//-----------------------------------------------------------------------------
		
		public void Initialize(RoomControl control) {
			this.roomControl = control;
			Initialize();
		}
		

		//-----------------------------------------------------------------------------
		// Virtual methods
		//-----------------------------------------------------------------------------

		protected virtual void Initialize() {}
		
		// Called when the room is only to update graphics.
		public virtual void UpdateGraphics() {}

		public virtual void Update() {}
		
		public virtual void Draw() {}
		

		//-----------------------------------------------------------------------------
		// Static methods
		//-----------------------------------------------------------------------------

		// Instantiate an event tile from the given event-data.
		public static EventTile CreateEvent(EventTileData data) {
			EventTile tile;
			
			// Construct the tile.
			if (data.Type == null)
				tile = new EventTile();
			else
				tile = (EventTile) data.Type.GetConstructor(Type.EmptyTypes).Invoke(null);
			
			tile.eventData	= data;
			tile.size		= data.Size;
			tile.properties.Merge(data.Properties, true);
			tile.properties.Merge(data.ModifiedProperties, true);

			return tile;
		}

		
		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		
		// Get the room control this event belongs to.
		public RoomControl RoomControl {
			get { return roomControl; }
			set { roomControl = value; }
		}


		public EventTileData EventData {
			get { return eventData; }
			set { eventData = value; }
		}
		
		// Get the position of this event.
		public Vector2F Position {
			get { return position; }
			set { position = value; }
		}
		
		public Point2I Size {
			get { return size; }
			set { size = value; }
		}
		
		// Get the properties for this event.
		public Properties Properties {
			get { return properties; }
		}
	}
}
