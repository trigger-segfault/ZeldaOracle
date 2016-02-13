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
			player.IsStateControlled			= true;
			player.IsPassable					= true;
			player.Physics.CollideWithWorld		= false;
			player.Physics.MovesWithPlatforms	= false;
			player.Physics.HasGravity			= false;
			player.Movement.StopMotion();
		}
		
		public override void OnEnd(PlayerState newState) {
			player.IsStateControlled			= false;
			player.IsPassable					= false;
			player.Physics.CollideWithWorld		= true;
			player.Physics.MovesWithPlatforms	= true;
			player.Physics.HasGravity			= true;
		}

		public override void Update() {
			base.Update();

			if (respawning) {
				if (player.RoomControl.ViewControl.IsCenteredOnPosition(player.Center)) {
					player.Graphics.IsVisible = true;
					player.Hurt(new DamageInfo(2));
					player.BeginNormalState();
				}
			}
			else if (!waitForAnimation || player.Graphics.IsAnimationDone) {
				player.Graphics.IsVisible = false;
				respawning = true;
				player.Respawn();
			}
		}

		public override bool RequestStateChange(PlayerState newState) {
			return false;
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
