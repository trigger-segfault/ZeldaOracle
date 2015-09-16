using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeldaOracle.Game.Entities.Players
{
	public class PlayerState {

		//public delegate void UpdateMethod();
		//private event UpdateMethod updateMethod;
		
		protected Player player;
		private bool isActive;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public PlayerState() {
			isActive = false;
		}


		//-----------------------------------------------------------------------------
		// Methods
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
		
		public virtual void OnBegin() {}
		
		public virtual void OnEnd() {}

		public virtual void Update() {}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public Player Player {
			get { return player; }
			set { player = value; }
		}
	}
}
