using System;
using System.Collections.Generic;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Entities.Players.States;

namespace ZeldaOracle.Game.Entities.Players {
	
	public class PlayerStateMachine {

		/// <summary> The previous player state </summary>
		private PlayerState previousState;

		/// <summary> The current Player State </summary>
		private PlayerState state;

		/// <summary> Reference to the player </summary>
		private Player player;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public PlayerStateMachine(Player player) {
			this.player = player;
			this.state = null;
			this.previousState = null;
		}

		
		//-----------------------------------------------------------------------------
		// States
		//-----------------------------------------------------------------------------

		/// <summary>Transition to the given player state if it is not already active
		/// </summary>
		public void BeginState(PlayerState newState) {
			if (newState != state || !state.IsActive)
				ForceBeginState(newState);
		}

		/// <summary>Transition to the given player state</summary>
		public void ForceBeginState(PlayerState newState) {
			//Console.WriteLine("Begin State: {0}", newState.GetType().Name);

			// End the current state
			previousState = state;
			if (state != null && state.IsActive)
				state.End(newState);
			
			// Begin the new state
			state = newState;
			if (state != null) {
				state.StateMachine = this;
				state.Begin(player, previousState);
				if (!state.IsActive) {
					previousState = state;
					state = null;
				}
			}
		}

		/// <summary> Update the active state </summary>
		public void Update() {
			if (state != null && state.IsActive) {
				state.Update();
				if (!state.IsActive)
					state = null;
			}
			else
				state = null;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------
		
		/// <summary> True if there is an active state that is being updated </summary>
		public bool IsActive {
			get { return (state != null && state.IsActive); }
		}
		
		/// <summary> The currently active player state </summary>
		public PlayerState CurrentState {
			get {
				if (state == null || !state.IsActive)
					return null;
				return state;
			}
			set { state = value; }
		}

		/// <summary> Reference to the player </summary>
		public Player Player {
			get { return player; }
		}

		/// <summary> The currently active player state parameters </summary>
		public PlayerStateParameters StateParameters {
			get {
				return (IsActive ? state.StateParameters :
					new PlayerStateParameters());
			}
		}
	}
}
