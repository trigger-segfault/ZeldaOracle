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

		public PlayerBusyState(int duration) : base() {
			this.duration = duration;
		}
		

		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnBegin() {
			base.OnBegin();
			timer = duration;
			if (player.IsOnGround)
				player.Physics.Velocity = Vector2F.Zero;
		}
		
		public override void OnEnd() {
			base.OnEnd();
		}

		public override void Update() {
			base.Update();

			if (player.IsOnGround)
				player.Physics.Velocity = Vector2F.Zero;

			timer--;
			if (timer <= 0) {
				if (player.IsOnGround)
					player.BeginState(player.NormalState);
				else
					player.BeginState(player.JumpState);
			}
		}
	}
}
