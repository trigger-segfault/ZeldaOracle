using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Main;
using ZeldaOracle.Game.Tiles;

namespace ZeldaOracle.Game.Entities.Players.States {
	public class PlayerSpinSwordState : PlayerState {

		private Animation weaponAnimation;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public PlayerSpinSwordState() {
			this.weaponAnimation = null;
		}
		
		
		//-----------------------------------------------------------------------------
		// Virtual methods
		//-----------------------------------------------------------------------------

		private void CutTilesAtPosition(Vector2F position) {
			Point2I location = player.RoomControl.GetTileLocation(position);

			if (player.RoomControl.IsTileInBounds(location)) {
				Tile tile = player.RoomControl.GetTopTile(location);
				if (tile != null)
					tile.OnSwordHit();
			}
		}

		public virtual void OnHitAngle(int angle, bool isLast = false) {
			if (player.IsOnGround) {
				CutTilesAtPosition(player.Center + (Angles.ToVector(angle) * 16));
				if (isLast)
					CutTilesAtPosition(player.Center);
			}
		}
		

		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnBegin(PlayerState previousState) {
			player.Movement.MoveCondition = PlayerMoveCondition.OnlyInAir;
			player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_SPIN);
			player.toolAnimation.Animation = weaponAnimation;
			player.toolAnimation.SubStripIndex = player.Direction;
			player.toolAnimation.Play();
		}
		
		public override void OnEnd(PlayerState newState) {
			player.toolAnimation.Animation = null;
			player.Movement.MoveCondition = PlayerMoveCondition.FreeMovement;
		}

		public override void Update() {
			base.Update();

			// Check for angle changes.
			int delay = 3;
			int t = (int) player.toolAnimation.PlaybackTime;
			int angle = Directions.ToAngle(player.Direction);
			for (int i = 0; i < 8; i++) {
				if (t == delay) {
					OnHitAngle((angle + 8 - i - 1) % Angles.AngleCount, i == 7);
					break;
				}
				if (i % 2 == 0)
					delay += 2;
				else
					delay += 3;
			}
			
			// End the spin.
			if (player.Graphics.IsAnimationDone)
				player.BeginNormalState();
		}

		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public Animation WeaponAnimation {
			get { return weaponAnimation; }
			set { weaponAnimation = value; }
		}
	}
}
