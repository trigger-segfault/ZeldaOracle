using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Control;
using ZeldaOracle.Game.Entities.Collisions;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Worlds;

namespace ZeldaOracle.Game.Entities.Players.States {
	public class PlayerLeapLedgeJumpState : PlayerLedgeJumpState {
		
		private int			timer;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public PlayerLeapLedgeJumpState() {

		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnBegin(PlayerState previousState) {
			// TODO: player.passable = true;
			player.IsStateControlled        = true;
			player.Movement.IsStrafing      = true;
			player.Physics.CollideWithWorld = false;
			player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_JUMP);

			// The player can hold his sword while ledge jumping.
			isHoldingSword = (previousState == player.HoldSwordState);

			if (isHoldingSword) {
				isHoldingSword = true;
				player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_DEFAULT);
			}
			else {
				player.Direction = direction;
			}

			timer = GameSettings.PLAYER_LEAP_LEDGE_JUMP_DURATION + 1;

			float jumpSpeed = GameSettings.PLAYER_LEAP_LEDGE_JUMP_SPEED;
			float speed = GameSettings.PLAYER_LEAP_LEDGE_JUMP_DISTANCE /
							GameSettings.PLAYER_LEAP_LEDGE_JUMP_DURATION;

			velocity = Directions.ToVector(direction) * speed;
			player.Physics.ZVelocity = jumpSpeed;

			player.Physics.Velocity = velocity;
			player.Position += velocity;
			AudioSystem.PlaySound(GameData.SOUND_PLAYER_JUMP);
		}

		public override void OnEnd(PlayerState newState) {
			player.IsStateControlled        = false;
			player.Physics.CollideWithWorld = true;
			player.Movement.IsStrafing      = false;
		}

		public override void OnEnterRoom() {
			
		}

		public override void Update() {
			// TODO: If update ever gets any base content. Create a way to
			// call base.base.Update since this extends LedgeJumpState for the sword properties
			//base.Update();
			
			timer--;

			// Update velocity while checking we've reached the landing spot.
			player.Physics.Velocity = velocity;

			// If done, return to the normal player state.
			if (timer == 0) {
				player.Physics.Velocity = Vector2F.Zero;
				
				player.LandOnSurface();
				
				if (isHoldingSword)
					player.BeginState(player.HoldSwordState);
				else
					player.BeginNormalState();
			}
		}
	}
}
