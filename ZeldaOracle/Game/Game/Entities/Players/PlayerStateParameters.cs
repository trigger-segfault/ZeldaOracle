using ZeldaOracle.Common.Graphics.Sprites;

namespace ZeldaOracle.Game.Entities.Players {

	public enum PlayerAnimationTypes {
		Swing,
		SwingNoLunge,
		SwingBig,
		Spin,
		Stab,
		Aim,
		Throw,
		Default,
		Carry,

		Count,
	}

	public class PlayerStateAnimations {

		private Animation[] animations;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public PlayerStateAnimations() {
			animations = new Animation[(int) PlayerAnimationTypes.Count];
			for (int i = 0; i < animations.Length; i++)
				animations[i] = null;
		}


		//-----------------------------------------------------------------------------
		// Indexing
		//-----------------------------------------------------------------------------

		public Animation GetAnimation(PlayerAnimationTypes type) {
			return animations[(int) type];
		}

		public void SetAnimation(PlayerAnimationTypes type, Animation value) {
			animations[(int) type] = value;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public Animation this[PlayerAnimationTypes type] {
			get { return animations[(int) type]; }
			set { animations[(int) type] = value; }
		}

		public Animation[] Animations {
			get { return animations; }
		}

		public Animation Default {
			get { return GetAnimation(PlayerAnimationTypes.Default); }
			set { SetAnimation(PlayerAnimationTypes.Default, value); }
		}

		public Animation Swing {
			get { return GetAnimation(PlayerAnimationTypes.Swing); }
			set { SetAnimation(PlayerAnimationTypes.Swing, value); }
		}

		public Animation SwingNoLunge {
			get { return GetAnimation(PlayerAnimationTypes.SwingNoLunge); }
			set { SetAnimation(PlayerAnimationTypes.SwingNoLunge, value); }
		}

		public Animation SwingBig {
			get { return GetAnimation(PlayerAnimationTypes.SwingBig); }
			set { SetAnimation(PlayerAnimationTypes.SwingBig, value); }
		}

		public Animation Spin {
			get { return GetAnimation(PlayerAnimationTypes.Spin); }
			set { SetAnimation(PlayerAnimationTypes.Spin, value); }
		}

		public Animation Stab {
			get { return GetAnimation(PlayerAnimationTypes.Stab); }
			set { SetAnimation(PlayerAnimationTypes.Stab, value); }
		}

		public Animation Aim {
			get { return GetAnimation(PlayerAnimationTypes.Aim); }
			set { SetAnimation(PlayerAnimationTypes.Aim, value); }
		}

		public Animation Throw {
			get { return GetAnimation(PlayerAnimationTypes.Throw); }
			set { SetAnimation(PlayerAnimationTypes.Throw, value); }
		}

		public Animation Carry {
			get { return GetAnimation(PlayerAnimationTypes.Carry); }
			set { SetAnimation(PlayerAnimationTypes.Carry, value); }
		}
	}
	
	public enum PlayerStateParameterTypes {
		DisableGravity,
		EnableGroundOverride,
		DisableSolidCollisions,
	
		DisableInteractionCollisions,

		ProhibitJumping,
		ProhibitLedgeJumping,
		ProhibitWarping,
		ProhibitMovementControlOnGround,
		ProhibitMovementControlInAir,
		ProhibitPushing,
		ProhibitWeaponUse,
		ProhibitEnteringMinecart,
		ProhibitRoomTransitions,
		ProhibitReleasingSword,

		EnableStrafing,
		AlwaysFaceRight,
		AlwaysFaceUp,
		AlwaysFaceLeftOrRight,
		AlwaysFaceDown,
		EnableAutomaticRoomTransitions,
		DisableMovement,
		DisableAutomaticStateTransitions,
		DisableUpdateMethod,
		DisablePlatformMovement,

		MovementSpeedScale,

		Count,
	}

	public class PlayerStateParameters {

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------
		
		public PlayerStateParameters() {
		}
		

		//-----------------------------------------------------------------------------
		// Operators
		//-----------------------------------------------------------------------------

		public static PlayerStateParameters operator |(PlayerStateParameters a, PlayerStateParameters b) {
			PlayerStateParameters result = new PlayerStateParameters() {
				// Physics
				DisableGravity = a.DisableGravity || b.DisableGravity,
				EnableGroundOverride = a.EnableGroundOverride || b.EnableGroundOverride,
				DisableSolidCollisions = a.DisableSolidCollisions || b.DisableSolidCollisions,

				// Unit
				DisableInteractionCollisions = a.DisableInteractionCollisions || b.DisableInteractionCollisions,

				// Player
				ProhibitJumping = a.ProhibitJumping || b.ProhibitJumping,
				ProhibitLedgeJumping = a.ProhibitLedgeJumping || b.ProhibitLedgeJumping,
				ProhibitWarping = a.ProhibitWarping || b.ProhibitWarping,
				ProhibitMovementControlOnGround = a.ProhibitMovementControlOnGround || b.ProhibitMovementControlOnGround,
				ProhibitMovementControlInAir = a.ProhibitMovementControlInAir || b.ProhibitMovementControlInAir,
				ProhibitPushing = a.ProhibitPushing || b.ProhibitPushing,
				ProhibitWeaponUse = a.ProhibitWeaponUse || b.ProhibitWeaponUse,
				ProhibitEnteringMinecart = a.ProhibitEnteringMinecart || b.ProhibitEnteringMinecart,
				ProhibitRoomTransitions = a.ProhibitRoomTransitions || b.ProhibitRoomTransitions,
				ProhibitReleasingSword = a.ProhibitReleasingSword || b.ProhibitReleasingSword,
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

			// Prefer the right side's animations if they are non-null.
			for (int i = 0; i < (int) PlayerAnimationTypes.Count; i++) {
				PlayerAnimationTypes type = (PlayerAnimationTypes) i;
				result.PlayerAnimations[type] = b.PlayerAnimations[type] ??
					a.PlayerAnimations[type];
			}

			return result;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------
		
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
		public bool ProhibitReleasingSword { get; set; } = false;
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
		
		public PlayerStateAnimations PlayerAnimations { set; get; } = new PlayerStateAnimations();
		

		//-----------------------------------------------------------------------------
		// Compound Parameter Properties
		//----------------------------------------------------------------------------

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
				ProhibitJumping = value;
			}
		}
	}
}
