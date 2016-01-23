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

namespace ZeldaOracle.Game.Tiles {

	public class TileTurnstile : Tile {

		private WindingOrder windingOrder;
		private AnimationPlayer arrowsAnimationPlayer;
		private AnimationPlayer turnstileAnimationPlayer;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public TileTurnstile() {
			arrowsAnimationPlayer = new AnimationPlayer();
			turnstileAnimationPlayer = new AnimationPlayer();
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
				turnstileAnimationPlayer.SubStripIndex = 1;
				Properties.SetBase("clockwise", false);
			}
			else {
				windingOrder = WindingOrder.Clockwise;
				arrowsAnimationPlayer.Play(GameData.ANIM_TURNSTILE_ARROWS_CLOCKWISE);
				turnstileAnimationPlayer.SubStripIndex = 0;
				Properties.SetBase("clockwise", true);
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

			CollisionModel = new CollisionModel();
			CollisionModel.AddBox( 0,  0, 8, 8);
			CollisionModel.AddBox( 8,  8, 8, 8);
			CollisionModel.AddBox(40,  0, 8, 8);
			CollisionModel.AddBox(32,  8, 8, 8);
			CollisionModel.AddBox( 0, 40, 8, 8);
			CollisionModel.AddBox( 8, 32, 8, 8);
			CollisionModel.AddBox(40, 40, 8, 8);
			CollisionModel.AddBox(32, 32, 8, 8);
			CollisionModel.AddBox(16, 16, 16, 16); // center.

			SolidType	= TileSolidType.Solid;
			IsSolid		= true;

			// Setup based on the winding order.
			windingOrder = (Properties.GetBoolean("clockwise", false) ?
					WindingOrder.Clockwise : WindingOrder.CounterClockwise);

			if (windingOrder == WindingOrder.Clockwise) {
				arrowsAnimationPlayer.Play(GameData.ANIM_TURNSTILE_ARROWS_CLOCKWISE);
				turnstileAnimationPlayer.SubStripIndex = 0;
			}
			else {
				arrowsAnimationPlayer.Play(GameData.ANIM_TURNSTILE_ARROWS_COUNTERCLOCKWISE);
				turnstileAnimationPlayer.SubStripIndex = 1;
			}

			turnstileAnimationPlayer.Play(GameData.ANIM_TURNSTILE_ROTATE_CLOCKWISE);
			turnstileAnimationPlayer.PlaybackTime = turnstileAnimationPlayer.Animation.Duration;
		}

		public override void Update() {
			base.Update();
			
			Player player = RoomControl.Player;
			
			Rectangle2F[] sideRects = new Rectangle2F[] {
				new Rectangle2F(32, 16, 3, 16),
				new Rectangle2F(16, 16 - 3, 16, 3),
				new Rectangle2F(16 - 3, 16, 3, 16),
				new Rectangle2F(16, 32, 16, 3),
			};

			if (player.IsOnGround) {
				int direction = -1;

				CollisionInfo[] collisionInfo = player.Physics.CollisionInfo;
				for (int dir = 0; dir < Directions.Count; dir++) {
					Rectangle2F sideRect = Rectangle2F.Translate(sideRects[dir], Position);

					if (player.Physics.PositionedCollisionBox.Intersects(sideRect)) {
						direction = dir;
						break;
					}
				}

				if (direction >= 0) {
					// TODO: Interrupt player state (similar to falling in hole)
					// Carried objects are dropped.
					// Sword, seed shooter is put away.
					// Switch hook is put away.
					// Busy state is cancelled.
					AudioSystem.PlaySound(GameData.SOUND_CHEST_OPEN);
					GameControl.PushRoomState(new RoomStateTurnstile(this, direction));
				}
			}
		}

		public override void UpdateGraphics() {
			base.UpdateGraphics();

			arrowsAnimationPlayer.Update();
			turnstileAnimationPlayer.Update();
		}

		public override void Draw(Common.Graphics.Graphics2D g) {
			base.Draw(g);

			g.DrawAnimation(arrowsAnimationPlayer, Zone.ImageVariantID, Position);
			g.DrawAnimation(turnstileAnimationPlayer, Zone.ImageVariantID, Position);
		}

		
		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public WindingOrder WindingOrder {
			get { return windingOrder; }
		}
	}
}
