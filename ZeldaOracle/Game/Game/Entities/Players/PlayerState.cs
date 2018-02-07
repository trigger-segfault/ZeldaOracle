using System.Collections.Generic;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Entities.Players.States;

namespace ZeldaOracle.Game.Entities.Players {
	
	public class PlayerStateParameters {

		public bool ProhibitJumping { get; set; } = false;
		public bool ProhibitLedgeJumping { get; set; } = false;
		public bool ProhibitWarping { get; set; } = false;
		public bool ProhibitMovementControlOnGround { get; set; } = false;
		public bool ProhibitMovementControlInAir { get; set; } = false;
		public bool ProhibitPushing { get; set; } = false;
		public bool ProhibitWeaponUse { get; set; } = false;
		public bool EnableStrafing { get; set; } = false;
		//public bool AlwaysFaceRight { get; set; } = false;
		public bool AlwaysFaceUp { get; set; } = false;
		public bool AlwaysFaceLeftOrRight { get; set; } = false;
		//public bool AlwaysFaceDown { get; set; } = false;
		public bool EnableAutomaticRoomTransitions { get; set; } = false;
		public bool DisableMovement { get; set; } = false;
		public bool DisableAutomaticStateTransitions { get; set; } = false;
		public bool DisableUpdateMethod { get; set; } = false;
		public float MovementSpeedScale { get; set; } = 1.0f;
		
		public bool ProhibitMovementControl {
			set {
				ProhibitMovementControlOnGround = value;
				ProhibitMovementControlInAir = value;
			}
		}

		public static PlayerStateParameters operator |(PlayerStateParameters a, PlayerStateParameters b) {
			return new PlayerStateParameters() {
				ProhibitJumping = a.ProhibitJumping || b.ProhibitJumping,
				ProhibitLedgeJumping = a.ProhibitLedgeJumping || b.ProhibitLedgeJumping,
				ProhibitWarping = a.ProhibitWarping || b.ProhibitWarping,
				ProhibitMovementControlOnGround = a.ProhibitMovementControlOnGround || b.ProhibitMovementControlOnGround,
				ProhibitMovementControlInAir = a.ProhibitMovementControlInAir || b.ProhibitMovementControlInAir,
				ProhibitPushing = a.ProhibitPushing || b.ProhibitPushing,
				ProhibitWeaponUse = a.ProhibitWeaponUse || b.ProhibitWeaponUse,
				EnableStrafing = a.EnableStrafing || b.EnableStrafing,
				//AlwaysFaceRight = a.AlwaysFaceRight || b.AlwaysFaceRight,
				AlwaysFaceUp = a.AlwaysFaceUp || b.AlwaysFaceUp,
				//AlwaysFaceLeft = a.AlwaysFaceLeft || b.AlwaysFaceLeft,
				//AlwaysFaceDown = a.AlwaysFaceDown || b.AlwaysFaceDown,
				AlwaysFaceLeftOrRight = a.AlwaysFaceLeftOrRight || b.AlwaysFaceLeftOrRight,
				EnableAutomaticRoomTransitions = a.EnableAutomaticRoomTransitions || b.EnableAutomaticRoomTransitions,
				DisableMovement = a.DisableMovement || b.DisableMovement,
				DisableAutomaticStateTransitions = a.DisableAutomaticStateTransitions || b.DisableAutomaticStateTransitions,
				DisableUpdateMethod = a.DisableUpdateMethod || b.DisableUpdateMethod,
				MovementSpeedScale = a.MovementSpeedScale * b.MovementSpeedScale,
			};
		}
	}

	public class PlayerState {

		/// <summary> True if this state is currently active and being updated
		/// </summary>
		private bool isActive;

		/// <summary> Reference to the player </summary>
		protected Player player;

		/// <summary> Reference to the state machine controlling this state </summary>
		protected PlayerStateMachine stateMachine;

		/// <summary> Is this a state that is caused by the environment? </summary>
		private bool isNaturalState;

		/// <summary> Player state parameters applied when this state is active
		/// </summary>
		private PlayerStateParameters stateParameters;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public PlayerState() {
			isActive = false;
			isNaturalState = false;
			stateParameters = new PlayerStateParameters();
			stateMachine = null;
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
				newState is PlayerUnderwaterState || // "normal" state for underwater rooms
				newState is PlayerSidescrollSwimState))
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
			this.stateParameters = new PlayerStateParameters();
			OnBegin(previousState);
		}

		public void End(PlayerState newState = null) {
			this.isActive = false;
			OnEnd(newState);
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------
		
		/// <summary> Reference to the player </summary>
		public Player Player {
			get { return player; }
			set { player = value; }
		}
		
		/// <summary> True if this state is currently active and being updated
		/// </summary>
		public bool IsActive {
			get { return isActive; }
			set { isActive = value; }
		}

		/// <summary> Is this a state that is caused by the environment? </summary>
		public bool IsNaturalState {
			get { return isNaturalState; }
			set { isNaturalState = value; }
		}
		
		/// <summary> Player state parameters applied when this state is active
		/// </summary>
		public PlayerStateParameters StateParameters {
			get { return stateParameters; }
			set { stateParameters = value; }
		}

		/// <summary> Reference to the state machine controlling this state </summary>
		public PlayerStateMachine StateMachine {
			get { return stateMachine; }
			set { stateMachine = value; }
		}
	}
}
