using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Game.Entities.Players {
	public class PlayerBusyState : PlayerState {
		private int timer;
		private int duration;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public PlayerBusyState(int duration) {
			this.duration = duration;
		}
		

		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnBegin() {
			base.OnBegin();
			timer = duration;
			player.Movement.AllowMovementControl = false;
		}
		
		public override void OnEnd() {
			player.Movement.AllowMovementControl = true;
			base.OnEnd();
		}

		public override void Update() {
			base.Update();

			timer--;
			if (timer <= 0) {
				player.BeginNormalState();
			}
		}
	}
}
