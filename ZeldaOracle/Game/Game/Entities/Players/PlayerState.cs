using System.Collections.Generic;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Control;
using ZeldaOracle.Game.Entities.Players.States;

namespace ZeldaOracle.Game.Entities.Players {
	
	public class PlayerStateParameters {
		
		// Physics
		public bool DisableGravity { get; set; } = false;
		public bool EnableGroundOverride { get; set; } = false;
		public bool DisableSolidCollisions { get; set; } = false;

		// Unit
		public bool DisableInteractionCollisions { get; set; } = false;

		// Player
		public bool ProhibitJumping { get; set; } = false;
		public bool ProhibitLedgeJumping { get; set; } = false;
		public bool ProhibitWarping { get; set; } = false;
		public bool ProhibitMovementControlOnGround { get; set; } = false;
		public bool ProhibitMovementControlInAir { get; set; } = false;
		public bool ProhibitPushing { get; set; } = false;
		public bool ProhibitWeaponUse { get; set; } = false;
		public bool ProhibitEnteringMinecart { get; set; } = false;
		public bool ProhibitRoomTransitions { get; set; } = false;

		public bool EnableStrafing { get; set; } = false;
		//public bool AlwaysFaceRight { get; set; } = false;
		public bool AlwaysFaceUp { get; set; } = false;
		public bool AlwaysFaceLeftOrRight { get; set; } = false;
		//public bool AlwaysFaceDown { get; set; } = false;
		public bool EnableAutomaticRoomTransitions { get; set; } = false;
		public bool DisableMovement { get; set; } = false;
		public bool DisableAutomaticStateTransitions { get; set; } = false;
		public bool DisableUpdateMethod { get; set; } = false;
		public bool DisablePlatformMovement { get; set; } = false;
		public float MovementSpeedScale { get; set; } = 1.0f;
		

		public bool ProhibitMovementControl {
			set {
				ProhibitMovementControlOnGround = value;
				ProhibitMovementControlInAir = value;
			}
		}
		public bool DisablePlayerControl {
			set {
				DisableMovement = true;
				DisableAutomaticStateTransitions = true;
				ProhibitWeaponUse = true;
				ProhibitMovementControlOnGround = value;
				ProhibitMovementControlInAir = value;
			}
		}

		public static PlayerStateParameters operator |(PlayerStateParameters a, PlayerStateParameters b) {
			return new PlayerStateParameters() {
				DisableGravity = a.DisableGravity || b.DisableGravity,
				EnableGroundOverride = a.EnableGroundOverride || b.EnableGroundOverride,
				DisableSolidCollisions = a.DisableSolidCollisions || b.DisableSolidCollisions,

				DisableInteractionCollisions = a.DisableInteractionCollisions || b.DisableInteractionCollisions,

				ProhibitJumping = a.ProhibitJumping || b.ProhibitJumping,
				ProhibitLedgeJumping = a.ProhibitLedgeJumping || b.ProhibitLedgeJumping,
				ProhibitWarping = a.ProhibitWarping || b.ProhibitWarping,
				ProhibitMovementControlOnGround = a.ProhibitMovementControlOnGround || b.ProhibitMovementControlOnGround,
				ProhibitMovementControlInAir = a.ProhibitMovementControlInAir || b.ProhibitMovementControlInAir,
				ProhibitPushing = a.ProhibitPushing || b.ProhibitPushing,
				ProhibitWeaponUse = a.ProhibitWeaponUse || b.ProhibitWeaponUse,
				ProhibitEnteringMinecart = a.ProhibitEnteringMinecart || b.ProhibitEnteringMinecart,
				ProhibitRoomTransitions = a.ProhibitRoomTransitions || b.ProhibitRoomTransitions,

				EnableStrafing = a.EnableStrafing || b.EnableStrafing,
				//AlwaysFaceRight = a.AlwaysFaceRight || b.AlwaysFaceRight,
				AlwaysFaceUp = a.AlwaysFaceUp || b.AlwaysFaceUp,
				//AlwaysFaceLeft = a.AlwaysFaceLeft || b.AlwaysFaceLeft,
				//AlwaysFaceDown = a.AlwaysFaceDown || b.AlwaysFaceDown,
				DisablePlatformMovement = a.DisablePlatformMovement || b.DisablePlatformMovement,
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
			isActive = false;
			stateParameters = new PlayerStateParameters();
			stateMachine = null;
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
		// Begin/end
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

		/// <summary>Reference to the state machine controlling this state</summary>
		public PlayerStateMachine StateMachine {
			get { return stateMachine; }
			set { stateMachine = value; }
		}
	}
}
