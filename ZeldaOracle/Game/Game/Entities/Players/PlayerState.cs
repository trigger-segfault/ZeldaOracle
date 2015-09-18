using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeldaOracle.Game.Entities.Players
{
	public class PlayerState {

		//public delegate void UpdateMethod();
		//private event UpdateMethod updateMethod;
		
		private bool		isActive;
		protected Player	player;
		protected bool		isNaturalState; // Is this a state that is caused by the environment?


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public PlayerState() {
			isActive = false;
			isNaturalState = false;
		}


		//-----------------------------------------------------------------------------
		// Virtual methods
		//-----------------------------------------------------------------------------
		
		public virtual void OnBegin() {}
		
		public virtual void OnEnd() {}
		
		public virtual void OnEnterRoom() {}

		public virtual void OnLeaveRoom() {}

		public virtual void Update() {}


		//-----------------------------------------------------------------------------
		// Begin/end
		//-----------------------------------------------------------------------------

		public void Begin(Player player) {
			this.player = player;
			this.isActive = true;
			OnBegin();
		}

		public void End() {
			this.isActive = false;
			OnEnd();
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public Player Player {
			get { return player; }
			set { player = value; }
		}

		public bool IsActive {
			get { return isActive; }
			set { isActive = value; }
		}

		public bool IsNaturalState {
			get { return isNaturalState; }
			set { isNaturalState = value; }
		}
	}
}
