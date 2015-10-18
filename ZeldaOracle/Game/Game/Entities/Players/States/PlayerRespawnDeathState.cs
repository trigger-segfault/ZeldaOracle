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
			player.IsStateControlled = true;
			//player.Movement.MoveCondition = PlayerMoveCondition.NoControl;
			player.Movement.StopMotion();
			player.Physics.CollideWithWorld = false;
		}
		
		public override void OnEnd(PlayerState newState) {
			player.IsStateControlled = false;
			player.Physics.CollideWithWorld = true;
			//player.Movement.MoveCondition = PlayerMoveCondition.FreeMovement;
		}

		public override void Update() {
			base.Update();

			if (respawning) {
				if (player.RoomControl.ViewControl.IsCenteredOnPosition(player.Center)) {
					player.Graphics.IsVisible = true;
					player.Hurt(2);
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
