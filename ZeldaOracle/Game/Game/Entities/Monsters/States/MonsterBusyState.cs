using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Game.Entities.Monsters.States {
	
	public class MonsterBusyState : MonsterState {

		private int timer;
		private int duration;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------
		
		public MonsterBusyState(int duration) {
			this.duration = duration;
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void OnBegin(MonsterState previousState) {
			timer = 0;

			monster.Physics.Velocity = Vector2F.Zero;
			monster.Graphics.PauseAnimation();
		}

		public override void OnEnd(MonsterState newState) {
			monster.Graphics.ResumeAnimation();
		}

		public override void Update() {
			timer++;

			if (timer > duration) {
				monster.BeginNormalState();
			}
		}
	}
}
