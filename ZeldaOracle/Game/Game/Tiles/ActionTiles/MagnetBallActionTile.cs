using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Game.Control;
using ZeldaOracle.Game.Entities.Projectiles.PlayerProjectiles;

namespace ZeldaOracle.Game.Tiles.ActionTiles {

	public class MagnetBallActionTile : ActionTile {

		private MagnetBall magnetBall;

		
		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public MagnetBallActionTile() {
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
			// Remember the ball position
			Point2I ballPosition = (Point2I) GMath.Round(magnetBall.Position);

			// Clamp the position so that the ball is at least one tile aways from the
			// room edge
			Rectangle2I area = RoomControl.RoomBounds;
			area.Inflate(-24, -24);
			ballPosition = GMath.Clamp(ballPosition, area);

			properties.SetBase("position", ballPosition);
		}


		//-----------------------------------------------------------------------------
		// Static Methods
		//-----------------------------------------------------------------------------

		/// <summary>Draws the action tile data to display in the editor.</summary>
		public new static void DrawTileData(Graphics2D g, ActionTileDataDrawArgs args) {
			Polarity polarity = args.Properties.GetEnum("polarity", Polarity.North);
			ISprite sprite = null;
			if (polarity == Polarity.North)
				sprite = GameData.SPR_MAGNET_BALL_NORTH;
			else
				sprite = GameData.SPR_MAGNET_BALL_SOUTH;
			g.DrawSprite(sprite, args.SpriteDrawSettings, args.Position, args.Color);
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public Polarity Polarity {
			get { return properties.GetEnum<Polarity>("polarity", Polarity.North); }
			set { properties.Set("polarity", (int) value); }
		}
	}
}
