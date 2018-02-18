using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Game.Entities.Players.States {
	public class PlayerRespawnDeathState : PlayerState {

		private bool respawning;
		private bool waitForAnimation;

		// Crush: 44 frames of squished. 40 frames of flicker
		// (blank, normal, blank, squish, blank, squish, blank, normal, (repeat))


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public PlayerRespawnDeathState() {
			respawning = false;
			waitForAnimation = true;
		}
		

		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnBegin(PlayerState previousState) {
			respawning = false;
			
			StateParameters.EnableAutomaticRoomTransitions	= true;
			StateParameters.EnableStrafing					= true;
			StateParameters.DisableSolidCollisions			= true;
			StateParameters.DisableInteractionCollisions	= true;
			StateParameters.DisableGravity					= true;
			StateParameters.DisablePlayerControl			= true;
			StateParameters.DisablePlatformMovement			= true;

			player.InterruptWeapons();
			player.Movement.StopMotion();
			player.Physics.ZVelocity = 0.0f;
			player.KnockbackVelocity = Vector2F.Zero;
		}
		
		public override void OnEnd(PlayerState newState) {
		}

		public override void Update() {
			base.Update();

			if (respawning) {
				// Wait for the view to pan to the player
				if (player.RoomControl.ViewControl.IsCenteredOnPosition(player.Center)) {
					player.Graphics.IsVisible = true;
					player.Hurt(new DamageInfo(2));
					End();
				}
			}
			else if (!waitForAnimation || player.Graphics.IsAnimationDone) {
				player.Graphics.IsVisible = false;
				respawning = true;
				player.Respawn();
			}
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public bool WaitForAnimation {
			get { return waitForAnimation; }
			set { waitForAnimation = value; }
		}
	}
}
