using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Game.Entities.Projectiles.PlayerProjectiles;
using ZeldaOracle.Common.Util;

namespace ZeldaOracle.Game.Entities.Players.States {

	public class PlayerCarryState : PlayerState {

		enum SubState {
			Pickup,
			Carry,
		};
		
		private Entity carriedEntity;
		private int pickupTimer;
		private int throwDuration; // How many ticks the player waits when throwing.
		private int pickupFrame1Duration;
		private int pickupFrame2Duration;
		private GenericStateMachine<SubState> subStateMachine;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------
		
		public PlayerCarryState() {
			// Constant State Parameters
			StateParameters.ProhibitJumping				= true;
			StateParameters.ProhibitLedgeJumping		= true;
			StateParameters.ProhibitWeaponUse			= true;
			StateParameters.ProhibitWarping				= true;

			// Sub-State Machine
			subStateMachine = new GenericStateMachine<SubState>();
			subStateMachine.AddState(SubState.Pickup)
				.OnBegin(OnBeginPickupState)
				.OnEnd(OnEndPickupState)
				.OnUpdate(OnUpdatePickupState);
			subStateMachine.AddState(SubState.Carry)
				.OnBegin(OnBeginCarryState)
				.OnUpdate(OnUpdateCarryState);
		}


		//-----------------------------------------------------------------------------
		// Carried Object Setup
		//-----------------------------------------------------------------------------

		public void SetCarriedObject(Tile tile) {
			carriedEntity			= new CarriedTile(tile);
			throwDuration			= 2;
			pickupFrame1Duration	= 6;
			pickupFrame2Duration	= 4;
		}

		public void SetCarriedObject(Entity entity) {
			carriedEntity			= entity;
			throwDuration			= 8;
			pickupFrame1Duration	= 4;
			pickupFrame2Duration	= 4;
		}

		//-----------------------------------------------------------------------------
		// Dropping & Throwing
		//-----------------------------------------------------------------------------

		public void DropObject(bool enterBusyState = true, bool playSound = true) {
			PlayerAnimations.Default = null;

			if (carriedEntity != null && carriedEntity.IsAlive) {
				DropObjectBase(enterBusyState, playSound);
				carriedEntity.OnDrop();
			}
		}

		public void ThrowObject(bool enterBusyState = true, bool playSound = true) {
			if (carriedEntity != null && carriedEntity.IsAlive) {
				DropObjectBase(enterBusyState, playSound);
				carriedEntity.Physics.ZVelocity = 1.0f;
				carriedEntity.Physics.Velocity = player.MoveDirection.ToVector(1.5f);
				carriedEntity.OnThrow();
			}
		}

		public void ReleaseObject(bool enterBusyState = true, bool playSound = true) {
			// Prevent throwing during pickup phase
			if (player.Movement.IsMoving &&
				subStateMachine.CurrentState == SubState.Carry)
				ThrowObject(enterBusyState, playSound);
			else
				DropObject(enterBusyState, playSound);
		}

		private void DropObjectBase(bool enterBusyState = true,
			bool playSound = true)
		{
			PlayerAnimations.Default = null;
			
			subStateMachine.EndCurrentState();
			
			// Detach the carried entity from the player
			player.DetachEntity(carriedEntity);
			carriedEntity.Position = player.Position;
			carriedEntity.ZPosition = 16;

			if (enterBusyState) {
				player.Graphics.PlayAnimation(player.Animations.Throw);
				StateMachine.BeginState(new PlayerBusyState(
					throwDuration, player.StateParameters.PlayerAnimations.Throw));
			}
			else {
				End();
			}

			if (playSound)
				AudioSystem.PlaySound(GameData.SOUND_PLAYER_THROW);
		}
		

		//-----------------------------------------------------------------------------
		// Sub-State Callbacks
		//-----------------------------------------------------------------------------

		private void OnBeginPickupState() {
			StateParameters.ProhibitMovementControl	= true;
			pickupTimer = 0;

			player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_PULL);

			AudioSystem.PlaySound(GameData.SOUND_PLAYER_PICKUP);
			
			// Pickup the carried entity
			player.AttachEntity(carriedEntity);
			carriedEntity.AttachmentZOffset = 0;
			carriedEntity.AttachmentOffset = player.Direction.ToVector(8);
			//if (carryEntity is CarriedTile)
			//	carryEntity.AttachmentZOffset -= 2;
			carriedEntity.OnPickup();
		}

		private void OnEndPickupState() {
			StateParameters.ProhibitMovementControl	= false;
		}

		private void OnUpdatePickupState() {
			pickupTimer++;
				
			// Handle the 2 unique frames of picking - up the object
			if (pickupTimer < pickupFrame1Duration) {
				carriedEntity.AttachmentZOffset = 0;
				carriedEntity.AttachmentOffset = player.Direction.ToVector(8);
				player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_PULL);
			}
			else if (pickupTimer < pickupFrame1Duration + pickupFrame2Duration) {
				carriedEntity.AttachmentZOffset = 8;
				carriedEntity.AttachmentOffset = player.Direction.ToVector(2);
				player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_GRAB);
			}
			else {
				subStateMachine.BeginState(SubState.Carry);
			}
		}

		private void OnBeginCarryState() {
			carriedEntity.AttachmentZOffset = 13;
			carriedEntity.AttachmentOffset = Vector2F.Zero;
			PlayerAnimations.Default = player.Animations.Carry;
		}

		private void OnUpdateCarryState() {
			PlayerAnimations.Default = player.Animations.Carry;
			carriedEntity.AttachmentZOffset = 13;
			carriedEntity.AttachmentOffset = Vector2F.Zero;
				
			// Adjust z-offset when facing horizontal
			if (player.Direction.IsHorizontal) {
				carriedEntity.AttachmentZOffset += 1;

				// Adjust z-offset for head bobbing
				float playbackTime = player.Graphics.AnimationPlayer.PlaybackTime + 1;
				if (playbackTime >= 2 && playbackTime < 8)
					carriedEntity.AttachmentZOffset -= 1;
			}
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------
		
		public override void OnBegin(PlayerState previousState) {			
			subStateMachine.InitializeOnState(SubState.Pickup);
		}
		
		public override void OnEnd(PlayerState newState) {
			if (subStateMachine.CurrentState != null)
				DropObject(false, false);
		}

		public override void OnHurt(DamageInfo damage) {
			base.OnHurt(damage);
			ReleaseObject(true, true);
		}

		public override void Update() {
			// Check if the carried entity was destroyed somehow
			if (!carriedEntity.IsAliveAndInRoom) {
				End();
				return;
			}

			subStateMachine.Update();
		}
	}
}
