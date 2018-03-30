using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Game.Control;
using ZeldaOracle.Game.Entities.Projectiles.PlayerProjectiles;

namespace ZeldaOracle.Game.Tiles.ActionTiles {

	public class MagnetBallAction : ActionTile {

		private MagnetBall magnetBall;

		
		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public MagnetBallAction() {
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		protected override void Initialize() {
			// Spawn the magnet ball entity
			magnetBall = new MagnetBall();
			magnetBall.Position = properties.GetPoint("position", (Point2I) Position);
			magnetBall.Properties = properties;
			RoomControl.SpawnEntity(magnetBall);
		}

		public override void OnRemoveFromRoom() {
			if (magnetBall.IsDestroyed) {
				properties.Set("position", (Point2I) Position);
			}
			else {
				// Remember the ball's position
				Point2I ballPosition = GMath.RoundI(magnetBall.Position);

				// TODO: this needs to reset when leaving the dungeon

				// Clamp the position so that the ball is at least one tile aways from the
				// room edge
				Rectangle2I area = RoomControl.RoomBounds;
				area.Inflate(-24, -24);
				ballPosition = GMath.Clamp(ballPosition, area);

				properties.Set("position", ballPosition);
			}
		}


		//-----------------------------------------------------------------------------
		// Static Methods
		//-----------------------------------------------------------------------------

		/// <summary>Draws the action tile data to display in the editor.</summary>
		public new static void DrawTileData(Graphics2D g, ActionDataDrawArgs args) {
			Polarity polarity = args.Properties.GetEnum("polarity", Polarity.North);
			ISprite sprite = null;
			if (polarity == Polarity.North)
				sprite = GameData.SPR_MAGNET_BALL_NORTH;
			else
				sprite = GameData.SPR_MAGNET_BALL_SOUTH;
			g.DrawSprite(sprite, args.SpriteSettings, args.Position, args.Color);
		}
		
		/// <summary>Initializes the properties and events for the action type.</summary>
		public static void InitializeTileData(ActionTileData data) {
			data.Properties.SetEnumStr("polarity", Polarity.North)
				.SetDocumentation("Polarity", "enum", typeof(Polarity), "Magnet Ball", "The magnetic polarity (north or south) for interaction with the magnetic gloves.");
			data.EntityType = typeof(MagnetBall);
			data.ResetCondition = TileResetCondition.LeaveArea;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public Polarity Polarity {
			get { return properties.GetEnum("polarity", Polarity.North); }
			set { properties.SetEnum("polarity", value); }
		}
	}
}
