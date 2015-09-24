using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Entities.Players.States;

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
		
		public virtual bool RequestStateChange(PlayerState newState) {
			if (!isNaturalState && newState is PlayerNormalState)
				return false;
			return true;
		}

		public virtual void OnBegin(PlayerState previousState) {}
		
		public virtual void OnEnd(PlayerState newState) {}
		
		public virtual void OnEnterRoom() {}

		public virtual void OnLeaveRoom() {}

		public virtual void Update() {}
		
		public virtual void DrawOver(Graphics2D g) {}


		//-----------------------------------------------------------------------------
		// Begin/end
		//-----------------------------------------------------------------------------

		public void Begin(Player player, PlayerState previousState) {
			this.player = player;
			this.isActive = true;
			OnBegin(previousState);
		}

		public void End(PlayerState newState) {
			this.isActive = false;
			OnEnd(newState);
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
