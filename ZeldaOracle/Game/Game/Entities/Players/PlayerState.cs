using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Entities.Players.States;

namespace ZeldaOracle.Game.Entities.Players {
	
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

		/// <summary>
		/// This function returns whether or not the player is allowed to
		/// automatically change a new state. This is called when the player 
		/// is not in his natural state, and then requests to change back to
		/// his natural state.<para/>
		/// The default implementation makes this State allow changing to any
		/// state that is not the Normal state, unless this state is a natural
		/// state, then it allows changing to any other state.<para/>
		/// 
		/// For example, when the player is swinging his sword, he should not
		/// transition back to the Normal state until he is done. But if the
		/// player falls in water, then he should transition to the Swim state,
		/// interrupting his swing.
		/// </summary>
		/// 
		/// <param name="newState">The desired state to change to</param>
		/// 
		/// <returns>True if the state change is allowed else false</returns>
		public virtual bool RequestStateChange(PlayerState newState) {
			if (!isNaturalState &&
				(newState is PlayerNormalState ||
				newState is PlayerUnderwaterState)) // "normal" state for underwater rooms
			{
				return false;
			}
			return true;
		}

		public virtual void OnBegin(PlayerState previousState) {}
		
		public virtual void OnEnd(PlayerState newState) {}
		
		public virtual void OnEnterRoom() {}

		public virtual void OnLeaveRoom() {}

		public virtual void OnEnterMinecart() {}

		public virtual void OnExitMinecart() {}

		public virtual void OnHurt(DamageInfo damage) {}

		public virtual void Update() {}
		
		public virtual void DrawUnder(RoomGraphics g) {}
		
		public virtual void DrawOver(RoomGraphics g) {}


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
