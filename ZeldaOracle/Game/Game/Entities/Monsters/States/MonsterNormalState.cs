using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeldaOracle.Game.Entities.Monsters.States {
	
	public class MonsterNormalState : MonsterState {


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public MonsterNormalState() {

		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void OnBegin(MonsterState previousState) {
			
		}

		public override void OnEnd(MonsterState newState) {
			
		}

		public override void Update() {
			Monster.UpdateAI();
		}
	}
}
