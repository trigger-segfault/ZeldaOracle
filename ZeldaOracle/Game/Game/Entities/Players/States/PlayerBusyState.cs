using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Game.Entities.Players.States {

	public class PlayerBusyState : PlayerState {

		private int timer;
		private int duration;
		private Dictionary<int, Action<PlayerState>> actions;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public PlayerBusyState() {
			this.timer		= 0;
			this.duration	= 0;
			this.actions	= new Dictionary<int,Action<PlayerState>>();
		}
		

		//-----------------------------------------------------------------------------
		// Customization
		//-----------------------------------------------------------------------------

		public void AddDelayedAction(int time, Action<PlayerState> action) {
			actions[time] = action;
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnBegin(PlayerState previousState) {
			player.Movement.MoveCondition = PlayerMoveCondition.OnlyInAir;
			timer = 0;
			
			if (actions.ContainsKey(0))
				actions[0].Invoke(this);
		}
		
		public override void OnEnd(PlayerState newState) {
			player.Movement.MoveCondition = PlayerMoveCondition.FreeMovement;
			timer = 0;
			duration = 0;
			actions.Clear();
		}

		public override void Update() {
			base.Update();

			timer++;

			if (actions.ContainsKey(timer))
				actions[timer].Invoke(this);

			if (timer >= duration) {
				player.BeginNormalState();
			}
		}

		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public int Duration {
			get { return duration; }
			set { duration = value; }
		}
	}
}
