using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Input;
using ZeldaOracle.Game.Main;
using ZeldaOracle.Game.Entities.Effects;
using ZeldaOracle.Game.Entities.Projectiles;
using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.Items;
using ZeldaOracle.Game.Items.Weapons;
using ZeldaOracle.Game.Control;
using ZeldaOracle.Game.Tiles;

namespace ZeldaOracle.Game.Entities.Players {
	public class PlayerJumpState : PlayerMovableState {

		private int jumpStartTime;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public PlayerJumpState() {
			jumpStartTime = -1;
			
			// Movement settings.
			allowMovementControl	= false;
			moveSpeed				= 1.0f;
			moveSpeedScale			= 1.0f;
			isSlippery				= true;
			acceleration			= 0.1f;
			deceleration			= 0.00f;
			minSpeed				= 0.05f;
			autoAccelerate			= false;
			directionSnapCount		= 8;
		}
		

		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnBegin() {
			base.OnBegin();
			
			if (player.IsOnGround) {
				// Jump!
				player.Physics.ZVelocity = GameSettings.PLAYER_JUMP_SPEED;
				player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_JUMP);
				jumpStartTime = player.RoomControl.GameManager.ElapsedTicks;
			}
			else {
				player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_DEFAULT);
				//if (jumpStartTime >= 0) {
				//	player.Graphics.AnimationPlayer.PlaybackTime = 
				//		player.RoomControl.GameManager.ElapsedTicks - jumpStartTime;
				//}
			}

			player.SyncAnimationWithDirection = false;
		}
		
		public override void OnEnd() {
			player.SyncAnimationWithDirection = true;
			base.OnEnd();
		}

		public override void Update() {
			// Only allow movement control on the descent.
			allowMovementControl = (player.Physics.ZVelocity < 0.1f);

			base.Update();

			if (player.IsOnGround) {
				player.BeginState(player.NormalState);
			}
			else {
				// Update items
				Player.UpdateEquippedItems();
			}
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------
		
		// This is the threshhold of movement speed scale to be considered stroking.
		public bool IsStroking {
			get { return (moveSpeedScale > 1.3f); }
		}

	}
}
