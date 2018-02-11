using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeldaOracle.Game.Entities.Monsters.States {
	public class MonsterSpawnState : MonsterState {

		private int timer;
		private bool isPassableBefore;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public MonsterSpawnState(int duration) {
			this.timer = duration;
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void OnBegin(MonsterState previousState) {
			isPassableBefore = Monster.IsPassable;
			Monster.IsPassable = true;
		}

		public override void OnEnd(MonsterState newState) {
			Monster.IsPassable = isPassableBefore;
		}

		public override void Update() {
			if (timer == 0) {
				monster.BeginNormalState();
			}
			else {
				Monster.IsPassable = true;
				Monster.UpdateAI();
				Monster.IsPassable = true;
			}

			timer--;
		}
	}
}
