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
using ZeldaOracle.Common.Util;

namespace ZeldaOracle.Game.Tiles.ActionTiles {

	public class ActionTile : IEventObject {
		
		private RoomControl				roomControl;
		private	ActionTileDataInstance	actionData;
		protected Vector2F				position;
		protected Point2I				size;
		protected Properties			properties;

		protected Rectangle2I			collisionBox;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public ActionTile() {
			roomControl		= null;
			actionData		= null;
			position		= Vector2F.Zero;
			size			= new Point2I(GameSettings.TILE_SIZE);
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

		protected virtual void Initialize() {}

		public virtual void OnTouch() {}

		public virtual void OnRemoveFromRoom() {}
		
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

		// Instantiate an action tile from the given action-data.
		public static ActionTile CreateAction(ActionTileDataInstance data) {
			ActionTile tile;

			// Construct the action tile
			if (data.Type == null)
				tile = new ActionTile();
			else
				tile = ReflectionHelper.Construct<ActionTile>(data.Type);
			
			tile.position	= data.Position;
			tile.actionData	= data;
			tile.size		= data.PixelSize;
			//tile.properties.SetAll(data.BaseProperties);
			//tile.properties.SetAll(data.Properties);
			//tile.properties.BaseProperties	= data.Properties;
			tile.properties	= data.ModifiedProperties;
			
			return tile;
		}

		/// <summary>Draws the action tile data to display in the editor.</summary>
		public static void DrawTileData(Graphics2D g, ActionDataDrawArgs args) {
			ISprite sprite = args.Action.Sprite;
			if (sprite is Animation) {
				int substripIndex = args.Properties.GetInteger("substrip_index", 0);
				sprite = ((Animation) sprite).GetSubstrip(substripIndex);
			}
			if (sprite != null) {
				SpriteSettings settings = new SpriteSettings(args.Zone.StyleDefinitions, args.Time);
				g.DrawSprite(
					sprite,
					settings,
					args.Position,
					args.Color);
			}
			else {
				Rectangle2I r = new Rectangle2I(args.Position, args.Action.TileSize * GameSettings.TILE_SIZE);
				g.FillRectangle(r, Color.Blue);
			}
		}

		/// <summary>Draws the action tile data to display in the editor.</summary>
		public static void DrawTileDataColors(Graphics2D g, ActionDataDrawArgs args, ColorDefinitions colorDefinitions) {
			ISprite sprite = args.Action.Sprite;
			if (sprite is Animation) {
				int substripIndex = args.Properties.GetInteger("substrip_index", 0);
				sprite = ((Animation) sprite).GetSubstrip(substripIndex);
			}
			if (sprite != null) {
				SpriteSettings settings = new SpriteSettings(args.Zone.StyleDefinitions,
					colorDefinitions, args.Time);
				g.DrawSprite(
					sprite,
					settings,
					args.Position,
					args.Color);
			}
			else {
				Rectangle2I r = new Rectangle2I(args.Position, args.Action.TileSize * GameSettings.TILE_SIZE);
				g.FillRectangle(r, Color.Blue);
			}
		}

		/// <summary>Draws the action tile data to display in the editor.</summary>
		public static void DrawTileDataWithOffset(Graphics2D g, ActionDataDrawArgs args, Point2I offset) {
			ISprite sprite = args.Action.Sprite;
			if (sprite is Animation) {
				int substripIndex = args.Properties.GetInteger("substrip_index", 0);
				sprite = ((Animation) sprite).GetSubstrip(substripIndex);
			}
			if (sprite != null) {
				g.DrawSprite(
					sprite,
					new SpriteSettings(args.Zone.StyleDefinitions, args.Time),
					args.Position,
					args.Color);
			}
			else {
				Rectangle2I r = new Rectangle2I(args.Position, args.Action.TileSize * GameSettings.TILE_SIZE);
				g.FillRectangle(r, Color.Blue);
			}
		}

		/// <summary>Draws the action tile data to display in the editor with the specified sprite index.</summary>
		public static void DrawTileDataIndex(Graphics2D g, ActionDataDrawArgs args, int substripIndex = -1, ColorDefinitions colorDefinitions = null) {
			ISprite sprite = args.Action.Sprite;
			if (sprite is Animation) {
				if (substripIndex == -1)
					substripIndex = args.Properties.GetInteger("substrip_index", 0);
				sprite = ((Animation) sprite).GetSubstrip(substripIndex);
			}
			if (sprite != null) {
				SpriteSettings settings = new SpriteSettings(args.Zone.StyleDefinitions, args.Time);
				if (colorDefinitions != null)
					settings.Colors = colorDefinitions;
				g.DrawSprite(
					sprite,
					settings,
					args.Position,
					args.Color);
			}
			else {
				Rectangle2I r = new Rectangle2I(args.Position, args.Action.TileSize * GameSettings.TILE_SIZE);
				g.FillRectangle(r, Color.Blue);
			}
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public Rectangle2F PositionedCollisionBox {
			get { return new Rectangle2F(collisionBox.Point + position, collisionBox.Size); }
		}
		
		// Get the room control this action belongs to.
		public RoomControl RoomControl {
			get { return roomControl; }
			set { roomControl = value; }
		}

		/// <summary>Gets the area control this tile belongs to.</summary>
		public AreaControl AreaControl {
			get { return roomControl.AreaControl; }
		}

		/// <summary>Gets the game control running the game.</summary>
		public GameControl GameControl {
			get { return roomControl.GameControl; }
		}

		/// <summary>Gets or sets the instance associated with this action.</summary>
		public ActionTileDataInstance ActionData {
			get { return actionData; }
			set { actionData = value; }
		}
		
		/// <summary>Gets or sets the position of the action.</summary>
		public Vector2F Position {
			get { return position; }
			set { position = value; }
		}

		/// <summary>Gets or sets the size of the action in pixels.</summary>
		public Point2I PixelSize {
			get { return size; }
			set { size = value; }
		}

		/// <summary>Gets or sets the center of the action.</summary>
		public Vector2F Center {
			get { return position + (Vector2F) size / 2f; }
		}
		
		// Get the properties for this action.
		public Properties Properties {
			get { return properties; }
		}

		public EventCollection Events {
			get { return actionData.Events; }
		}

		/// <summary>Gets the type of entity this action spawns.</summary>
		public Type EntityType {
			get { return actionData.EntityType; }
		}
	}
}
