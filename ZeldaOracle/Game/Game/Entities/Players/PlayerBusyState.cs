using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeldaOracle.Game.Entities.Players {
	public class PlayerBusyState : PlayerState {
		private int timer;
		private int duration;

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public PlayerBusyState(int duration) : base() {
			this.duration = duration;
		}
		

		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnBegin() {
			timer = duration;
		}
		
		public override void OnEnd() {
			
		}

		public override void Update() {
			timer--;
			if (timer <= 0)
				player.BeginState(player.NormalState);
		}
	}
}
