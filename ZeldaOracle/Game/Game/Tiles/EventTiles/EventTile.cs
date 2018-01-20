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
using ZeldaOracle.Common.Graphics.Sprites;

namespace ZeldaOracle.Game.Tiles.EventTiles {

	public class EventTile : IEventObject {
		
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
			tile.properties.BaseProperties	= data.Properties;
			
			return tile;
		}
		
		/// <summary>Draws the event tile data to display in the editor.</summary>
		public static void DrawTileData(Graphics2D g, EventTileDataDrawArgs args) {
			ISprite sprite = args.EventTile.Sprite;
			if (sprite is Animation) {
				int substripIndex = args.Properties.GetInteger("substrip_index", 0);
				sprite = ((Animation) sprite).GetSubstrip(substripIndex);
			}
			if (sprite != null) {
				int imageVariantID = args.Properties.GetInteger("image_variant");
				if (imageVariantID < 0)
					imageVariantID = args.Zone.ImageVariantID;
				g.DrawSprite(
					sprite,
					new SpriteDrawSettings(args.Zone.StyleDefinitions, imageVariantID, args.Time),
					args.Position,
					args.Color);
			}
			else {
				Rectangle2I r = new Rectangle2I(args.Position, args.EventTile.Size * GameSettings.TILE_SIZE);
				g.FillRectangle(r, Color.Blue);
			}
		}

		/// <summary>Draws the event tile data to display in the editor.</summary>
		public static void DrawTileDataWithOffset(Graphics2D g, EventTileDataDrawArgs args, Point2I offset) {
			ISprite sprite = args.EventTile.Sprite;
			if (sprite is Animation) {
				int substripIndex = args.Properties.GetInteger("substrip_index", 0);
				sprite = ((Animation) sprite).GetSubstrip(substripIndex);
			}
			if (sprite != null) {
				int imageVariantID = args.Properties.GetInteger("image_variant");
				if (imageVariantID < 0)
					imageVariantID = args.Zone.ImageVariantID;
				g.DrawSprite(
					sprite,
					new SpriteDrawSettings(args.Zone.StyleDefinitions, imageVariantID, args.Time),
					args.Position,
					args.Color);
			}
			else {
				Rectangle2I r = new Rectangle2I(args.Position, args.EventTile.Size * GameSettings.TILE_SIZE);
				g.FillRectangle(r, Color.Blue);
			}
		}

		/// <summary>Draws the event tile data to display in the editor with the specified sprite index.</summary>
		public static void DrawTileDataIndex(Graphics2D g, EventTileDataDrawArgs args, int substripIndex = -1) {
			ISprite sprite = args.EventTile.Sprite;
			if (sprite is Animation) {
				if (substripIndex == -1)
					substripIndex = args.Properties.GetInteger("substrip_index", 0);
				sprite = ((Animation) sprite).GetSubstrip(substripIndex);
			}
			if (sprite != null) {
				int imageVariantID = args.Properties.GetInteger("image_variant");
				if (imageVariantID < 0)
					imageVariantID = args.Zone.ImageVariantID;
				g.DrawSprite(
					sprite,
					new SpriteDrawSettings(args.Zone.StyleDefinitions, imageVariantID, args.Time),
					args.Position,
					args.Color);
			}
			else {
				Rectangle2I r = new Rectangle2I(args.Position, args.EventTile.Size * GameSettings.TILE_SIZE);
				g.FillRectangle(r, Color.Blue);
			}
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

		public EventCollection Events {
			get { return eventData.Events; }
		}
	}
}
