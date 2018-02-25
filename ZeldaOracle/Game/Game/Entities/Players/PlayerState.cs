using System.Collections.Generic;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Game.Control;
using ZeldaOracle.Game.Entities.Players.States;

namespace ZeldaOracle.Game.Entities.Players {
	
	public class PlayerState {

		/// <summary>True if this state is currently active and being updated
		/// </summary>
		private bool isActive;

		/// <summary>Reference to the player</summary>
		protected Player player;

		/// <summary>Reference to the state machine controlling this state</summary>
		protected PlayerStateMachine stateMachine;

		/// <summary>Player state parameters applied when this state is active
		/// </summary>
		private PlayerStateParameters stateParameters;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public PlayerState() {
			isActive				= false;
			stateParameters			= new PlayerStateParameters();
			stateMachine			= null;
		}


		//-----------------------------------------------------------------------------
		// Virtual methods
		//-----------------------------------------------------------------------------
		
		public virtual void OnBegin(PlayerState previousState) {}
		
		public virtual void OnEnd(PlayerState newState) {}
		
		public virtual void OnEnterRoom() {}

		public virtual void OnLeaveRoom() {}

		public virtual void OnEnterMinecart() {}

		public virtual void OnExitMinecart() {}

		public virtual void OnHurt(DamageInfo damage) {}

		public virtual void OnInterruptWeapons() {}

		public virtual void Update() {}
		
		public virtual void DrawUnder(RoomGraphics g) {}
		
		public virtual void DrawOver(RoomGraphics g) {}
		
		public virtual bool CanTransitionFromState(PlayerState state) {
			return true;
		}

		public virtual bool CanTransitionToState(PlayerState state) {
			return true;
		}


		//-----------------------------------------------------------------------------
		// Begin and End
		//-----------------------------------------------------------------------------

		public void Begin(Player player, PlayerState previousState) {
			this.isActive = true;
			this.player = player;
			OnBegin(previousState);
		}

		public void End(PlayerState newState = null) {
			this.isActive = false;
			OnEnd(newState);
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------
		
		/// <summary>Reference to the player</summary>
		public Player Player {
			get { return player; }
			set { player = value; }
		}

		/// <summary>Reference to the room control</summary>
		public RoomControl RoomControl {
			get { return player.RoomControl; }
		}
		
		/// <summary>True if this state is currently active and being updated
		/// </summary>
		public bool IsActive {
			get { return isActive; }
			set { isActive = value; }
		}

		/// <summary>Player state parameters applied when this state is active
		/// </summary>
		public PlayerStateParameters StateParameters {
			get { return stateParameters; }
			set { stateParameters = value; }
		}

		/// <summary>Player state animations applied when this state is active
		/// </summary>
		public PlayerStateAnimations PlayerAnimations {
			get { return stateParameters.PlayerAnimations; }
		}

		/// <summary>Reference to the state machine controlling this state</summary>
		public PlayerStateMachine StateMachine {
			get { return stateMachine; }
			set { stateMachine = value; }
		}
	}
}
