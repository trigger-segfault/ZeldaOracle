using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Main;
using ZeldaOracle.Game.Control;
using ZeldaOracle.Game.Entities.Effects;
using ZeldaOracle.Game.Entities.Projectiles;
using ZeldaOracle.Game.Worlds;

namespace ZeldaOracle.Game.Tiles.EventTiles {
	public class EventTile {
		
		private RoomControl				roomControl;
		private	EventTileDataInstance	eventData;
		protected Vector2F				position;
		protected Point2I				size;
		protected Properties			properties;

		protected Rectangle2I			collisionBox;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public EventTile() {
			roomControl		= null;
			eventData		= null;
			position		= Vector2F.Zero;
			size			= Point2I.One;
			properties		= new Properties();
			collisionBox	= new Rectangle2I(0, 0, 16, 16);
		}
		

		//-----------------------------------------------------------------------------
		// Initialization
		//-----------------------------------------------------------------------------
		
		public void Initialize(RoomControl control) {
			this.roomControl = control;
			Initialize();
		}
		

		//-----------------------------------------------------------------------------
		// Accessors
		//-----------------------------------------------------------------------------
		
		public bool IsTouchingPlayer() {
			return PositionedCollisionBox.Contains(roomControl.Player.Position);
			//return (roomControl.Player.Physics.PositionedCollisionBox.Intersects(PositionedCollisionBox));
		}


		//-----------------------------------------------------------------------------
		// Virtual methods
		//-----------------------------------------------------------------------------

		public virtual void OnTouch() {}

		protected virtual void Initialize() {}
		
		// Called when the room is only to update graphics.
		public virtual void UpdateGraphics() {}

		public virtual void Update() {
			if (IsTouchingPlayer()) {
				OnTouch();
			}
		}
		
		public virtual void Draw(Graphics2D g) {
			//g.DrawSprite(GameData.SPR_TILE_ARMOS_STATUE, position);
		}
		

		//-----------------------------------------------------------------------------
		// Static methods
		//-----------------------------------------------------------------------------

		// Instantiate an event tile from the given event-data.
		public static EventTile CreateEvent(EventTileDataInstance data) {
			EventTile tile;
			
			// Construct the tile.
			if (data.Type == null)
				tile = new EventTile();
			else
				tile = (EventTile) data.Type.GetConstructor(Type.EmptyTypes).Invoke(null);
			
			tile.position	= data.Position;
			tile.eventData	= data;
			tile.size		= data.Size;
			tile.properties.SetAll(data.BaseProperties);
			tile.properties.SetAll(data.Properties);

			return tile;
		}

		
		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public Rectangle2F PositionedCollisionBox {
			get { return new Rectangle2F(collisionBox.Point + position, collisionBox.Size); }
		}
		
		// Get the room control this event belongs to.
		public RoomControl RoomControl {
			get { return roomControl; }
			set { roomControl = value; }
		}


		public EventTileDataInstance EventData {
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
