using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Game.Entities.Players.States {
	public class PlayerRespawnDeathState : PlayerState {

		private bool respawning;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public PlayerRespawnDeathState() {
			respawning = false;
		}
		

		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnBegin(PlayerState previousState) {
			respawning = false;
			player.IsStateControlled		= true;
			player.IsPassable				= true;
			player.Physics.CollideWithWorld	= false;
			player.Movement.StopMotion();
		}
		
		public override void OnEnd(PlayerState newState) {
			player.IsStateControlled		= false;
			player.IsPassable				= false;
			player.Physics.CollideWithWorld = true;
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
			else if (player.Graphics.IsAnimationDone) {
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

	}
}
