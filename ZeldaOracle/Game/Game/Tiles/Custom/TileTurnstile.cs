using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Entities.Effects;
using ZeldaOracle.Game.Entities.Projectiles;
using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.Worlds;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.GameStates.RoomStates;
using ZeldaOracle.Game.Entities.Collisions;
using ZeldaOracle.Common.Graphics.Sprites;

namespace ZeldaOracle.Game.Tiles {

	public class TileTurnstile : Tile {

		private WindingOrder windingOrder;
		private AnimationPlayer arrowsAnimationPlayer;
		private AnimationPlayer turnstileAnimationPlayer;
		private int turnDirection;
		private int timer;


		//-----------------------------------------------------------------------------
		// Constants
		//-----------------------------------------------------------------------------

		private const int ACCEPT_TURN_DELAY = 8;
		private const int ACCEPT_TURN_MIN_DISTANCE_TO_CENTER = 4;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public TileTurnstile() {
			arrowsAnimationPlayer = new AnimationPlayer();
			turnstileAnimationPlayer = new AnimationPlayer();

			Graphics.SyncPlaybackWithRoomTicks = false;
		}


		//-----------------------------------------------------------------------------
		// Turnstile Methods
		//-----------------------------------------------------------------------------
		
		public void Rotate() {
			AudioSystem.PlaySound(GameData.SOUND_TURNSTILE);

			if (windingOrder == WindingOrder.Clockwise)
				turnstileAnimationPlayer.Play(GameData.ANIM_TURNSTILE_ROTATE_CLOCKWISE);
			else
				turnstileAnimationPlayer.Play(GameData.ANIM_TURNSTILE_ROTATE_COUNTERCLOCKWISE);
		}

		public bool IsDoneTurning() {
			return turnstileAnimationPlayer.IsDone;
		}

		public void ToggleDirection() {
			if (windingOrder == WindingOrder.Clockwise) {
				windingOrder = WindingOrder.CounterClockwise;
				arrowsAnimationPlayer.Play(GameData.ANIM_TURNSTILE_ARROWS_COUNTERCLOCKWISE);
				turnstileAnimationPlayer.Play(GameData.SPR_TURNSTILE_BARS_COUNTERCLOCKWISE);
				//turnstileAnimationPlayer.SubStripIndex = 1;
				Properties.Set("clockwise", false);
			}
			else {
				windingOrder = WindingOrder.Clockwise;
				arrowsAnimationPlayer.Play(GameData.ANIM_TURNSTILE_ARROWS_CLOCKWISE);
				turnstileAnimationPlayer.Play(GameData.SPR_TURNSTILE_BARS_CLOCKWISE);
				//turnstileAnimationPlayer.SubStripIndex = 0;
				Properties.Set("clockwise", true);
			}
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnInitialize() {
			base.OnInitialize();

			// Create the turnstile collision model.
			// It loooks like this: (each character is a quarter-tile)
			// X    X
			//  X  X
			//   XX
			//   XX
			//  X  X
			// X    X

			// Now in collision_models.conscript
			/*CollisionModel = new CollisionModel();
			CollisionModel.AddBox( 0,  0, 8, 8);
			CollisionModel.AddBox( 8,  8, 8, 8);
			CollisionModel.AddBox(40,  0, 8, 8);
			CollisionModel.AddBox(32,  8, 8, 8);
			CollisionModel.AddBox( 0, 40, 8, 8);
			CollisionModel.AddBox( 8, 32, 8, 8);
			CollisionModel.AddBox(40, 40, 8, 8);
			CollisionModel.AddBox(32, 32, 8, 8);
			CollisionModel.AddBox(16, 16, 16, 16);*/ // center.

			SolidType		= TileSolidType.Solid;
			IsSolid			= true;
			turnDirection	= -1;

			// Setup based on the winding order.
			windingOrder = (Properties.GetBoolean("clockwise", false) ?
					WindingOrder.Clockwise : WindingOrder.CounterClockwise);

			if (windingOrder == WindingOrder.Clockwise) {
				arrowsAnimationPlayer.Play(GameData.ANIM_TURNSTILE_ARROWS_CLOCKWISE);
				turnstileAnimationPlayer.Play(GameData.SPR_TURNSTILE_BARS_CLOCKWISE);
				//turnstileAnimationPlayer.SubStripIndex = 0;
			}
			else {
				arrowsAnimationPlayer.Play(GameData.ANIM_TURNSTILE_ARROWS_COUNTERCLOCKWISE);
				turnstileAnimationPlayer.Play(GameData.SPR_TURNSTILE_BARS_COUNTERCLOCKWISE);
				//turnstileAnimationPlayer.SubStripIndex = 1;
			}

			//turnstileAnimationPlayer.Play(GameData.ANIM_TURNSTILE_ROTATE_CLOCKWISE);
			//turnstileAnimationPlayer.SkipToEnd();
		}

		public override void Update() {
			base.Update();
			
			Player player = RoomControl.Player;
			
			int d = ACCEPT_TURN_MIN_DISTANCE_TO_CENTER;
			Rectangle2F[] sideRects = new Rectangle2F[] {
				new Rectangle2F(32, 16, d, 16),
				new Rectangle2F(16, 16 - d, 16, d),
				new Rectangle2F(16 - d, 16, d, 16),
				new Rectangle2F(16, 32, 16, d),
			};

			if (player.IsOnGround) {
				int direction = -1;

				for (int dir = 0; dir < Directions.Count; dir++) {
					Rectangle2F sideRect = Rectangle2F.Translate(sideRects[dir], Position);

					if (player.Physics.PositionedCollisionBox.Intersects(sideRect)) {
						direction = dir;
						break;
					}
				}

				if (turnDirection != direction) {
					timer = 0;
					turnDirection = direction;
				}

				if (turnDirection >= 0) {
					timer++;
					if (timer >= ACCEPT_TURN_DELAY) {
						AudioSystem.PlaySound(GameData.SOUND_CHEST_OPEN);
						GameControl.PushRoomState(new RoomStateTurnstile(this, direction));
					}
				}
			}
		}

		public override void UpdateGraphics() {
			base.UpdateGraphics();

			arrowsAnimationPlayer.Update();
			turnstileAnimationPlayer.Update();
		}

		public override void Draw(RoomGraphics g) {
			base.Draw(g);

			g.DrawAnimationPlayer(arrowsAnimationPlayer, Position, Graphics.DepthLayer);
			g.DrawAnimationPlayer(turnstileAnimationPlayer, Position, Graphics.DepthLayer);
		}


		//-----------------------------------------------------------------------------
		// Static Methods
		//-----------------------------------------------------------------------------

		/// <summary>Draws the tile data to display in the editor.</summary>
		public new static void DrawTileData(Graphics2D g, TileDataDrawArgs args) {
			Tile.DrawTileData(g, args);
			int substripIndex = args.Properties.GetInteger("substrip_index");
			bool clockwise = args.Properties.GetBoolean("clockwise", false);
			Animation arrowAnimation, turnstileAnimation;
			if (clockwise) {
				arrowAnimation = GameData.ANIM_TURNSTILE_ARROWS_CLOCKWISE;
				turnstileAnimation = GameData.ANIM_TURNSTILE_ROTATE_CLOCKWISE;
			}
			else {
				arrowAnimation = GameData.ANIM_TURNSTILE_ARROWS_COUNTERCLOCKWISE;
				turnstileAnimation = GameData.ANIM_TURNSTILE_ROTATE_COUNTERCLOCKWISE;
			}
			g.DrawSprite(
				arrowAnimation,//.GetSubstrip(clockwise ? 0 : 1),
				args.SpriteDrawSettings,
				args.Position,
				args.Color);
			g.DrawSprite(
				turnstileAnimation,//.GetSubstrip(clockwise ? 0 : 1),
				new SpriteDrawSettings(args.Zone.StyleDefinitions, 16f),
				args.Position,
				args.Color);
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public WindingOrder WindingOrder {
			get { return windingOrder; }
		}
	}
}
