using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeldaOracle.Game.Entities.Monsters.States {

	public class MonsterState {

		private bool isActive;
		protected Monster monster;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public MonsterState() {
			isActive = false;
		}

		
		//-----------------------------------------------------------------------------
		// Virtual methods
		//-----------------------------------------------------------------------------
		
		public virtual void OnBegin(MonsterState previousState) {}
		
		public virtual void OnEnd(MonsterState newState) {}
		
		public virtual void Update() {}
		
		public virtual void DrawUnder(RoomGraphics g) {}
		
		public virtual void DrawOver(RoomGraphics g) {}

		
		//-----------------------------------------------------------------------------
		// Begin/end
		//-----------------------------------------------------------------------------

		public void Begin(Monster monster, MonsterState previousState) {
			this.monster = monster;
			this.isActive = true;
			OnBegin(previousState);
		}

		public void End(MonsterState newState) {
			this.isActive = false;
			OnEnd(newState);
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public Monster Monster {
			get { return monster; }
			set { monster = value; }
		}

		public bool IsActive {
			get { return isActive; }
			set { isActive = value; }
		}
	}
}
