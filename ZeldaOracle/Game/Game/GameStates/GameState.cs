using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeldaOracle.Game.GameStates {
	
	public abstract class GameState {
		
		private bool isActive;

		
		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public GameState() {
			isActive = false;
		}

		//-----------------------------------------------------------------------------
		// Virtual methods
		//-----------------------------------------------------------------------------

		public virtual void OnBegin() {}

		public virtual void OnEnd()   {}

		public virtual void Update(float timeDelta) {}

		public virtual void Draw(float timeDelta) {}


		//-----------------------------------------------------------------------------
		// Accessors
		//-----------------------------------------------------------------------------

		public void Begin() {
			if (!isActive) {
				isActive = true;
				OnBegin();
			}
		}

		public void End() {
			if (isActive) {
				isActive = false;
				OnEnd();
			}
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public bool IsActive {
			get { return isActive; }
		}
	}
}
