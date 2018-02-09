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
			StateParameters.EnableAutomaticRoomTransitions	= true;
			StateParameters.EnableStrafing					= true;
			StateParameters.DisableSolidCollisions			= true;
			StateParameters.DisableInteractionCollisions	= true;
			StateParameters.DisablePlayerControl			= true;

			// Play the jump animation
			if (player.WeaponState == null)
				player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_JUMP);
			else
				player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_DEFAULT);

			// Face the jump direction
			if (!player.StateParameters.EnableStrafing)
				player.Direction = direction;

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

			if (timer == 0) {
				player.Physics.Velocity = Vector2F.Zero;
				player.LandOnSurface();
				End();
			}
		}
	}
}
