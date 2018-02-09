using ZeldaOracle.Common.Graphics.Sprites;

namespace ZeldaOracle.Game.Entities.Players {
	
	public class PlayerStateParameters {

		public class PlayerStateAnimations {
			public Animation Swing { get; set; } = null;
			public Animation SwingNoLunge { get; set; } = null;
			public Animation Spin { get; set; } = null;
			public Animation Stab { get; set; } = null;
			public Animation Aim { get; set; } = null;
			public Animation Throw { get; set; } = null;
			public Animation Default { get; set; } = null;
			public Animation Carry { get; set; } = null;
		}

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------
		
		public PlayerStateParameters() {
		}

		public static PlayerStateParameters operator |(PlayerStateParameters a, PlayerStateParameters b) {
			PlayerStateParameters result = new PlayerStateParameters() {
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

			result.PlayerAnimations.Swing			= a.PlayerAnimations.Swing;
			result.PlayerAnimations.SwingNoLunge	= a.PlayerAnimations.SwingNoLunge;
			result.PlayerAnimations.Spin			= a.PlayerAnimations.Spin;
			result.PlayerAnimations.Stab			= a.PlayerAnimations.Stab;
			result.PlayerAnimations.Throw			= a.PlayerAnimations.Throw;
			result.PlayerAnimations.Aim				= a.PlayerAnimations.Aim;
			result.PlayerAnimations.Default			= a.PlayerAnimations.Default;
			result.PlayerAnimations.Carry			= a.PlayerAnimations.Carry;

			if (b.PlayerAnimations.Swing != null)
				result.PlayerAnimations.Swing = b.PlayerAnimations.Swing;
			if (b.PlayerAnimations.SwingNoLunge != null)
				result.PlayerAnimations.SwingNoLunge = b.PlayerAnimations.SwingNoLunge;
			if (b.PlayerAnimations.Spin != null)
				result.PlayerAnimations.Spin = b.PlayerAnimations.Spin;
			if (b.PlayerAnimations.Stab != null)
				result.PlayerAnimations.Stab = b.PlayerAnimations.Stab;
			if (b.PlayerAnimations.Throw != null)
				result.PlayerAnimations.Throw = b.PlayerAnimations.Throw;
			if (b.PlayerAnimations.Aim != null)
				result.PlayerAnimations.Aim  = b.PlayerAnimations.Aim;
			if (b.PlayerAnimations.Default != null)
				result.PlayerAnimations.Default = b.PlayerAnimations.Default;
			if (b.PlayerAnimations.Carry != null)
				result.PlayerAnimations.Carry = b.PlayerAnimations.Carry;

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
