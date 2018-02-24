using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Input;
using ZeldaOracle.Game.Main;
using ZeldaOracle.Game.Entities.Effects;
using ZeldaOracle.Game.Entities.Projectiles;
using ZeldaOracle.Game.Items;
using ZeldaOracle.Game.Items.Weapons;
using ZeldaOracle.Game.Control;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Game.Entities.Projectiles.PlayerProjectiles;

namespace ZeldaOracle.Game.Entities.Players.States {
	public class PlayerCarryState : PlayerState {
		
		private bool isPickingUp; // Is the pickup animation playing?
		private int pickupTimer;
		private Point2I objectDrawOffset;
		private int objectZOffset;
		private bool isObjectDropped;

		private Entity carryEntity;
		private int throwDuration; // How many ticks the player waits when throwing.
		private int pickupFrame1Duration;
		private int pickupFrame2Duration;
		


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------
		
		public PlayerCarryState() {
		}


		//-----------------------------------------------------------------------------
		// Internal methods.
		//-----------------------------------------------------------------------------

		public void SetCarryObject(Tile tile) {
			this.carryEntity            = new CarriedTile(tile);
			this.throwDuration          = 2;
			this.pickupFrame1Duration   = 6;
			this.pickupFrame2Duration   = 4;
		}

		public void SetCarryObject(Entity entity) {
			this.carryEntity            = entity;
			this.throwDuration          = 8;
			this.pickupFrame1Duration   = 4;
			this.pickupFrame2Duration   = 4;
		}

		public void DropObject(bool enterBusyState = true, bool playSound = true) {
			PlayerAnimations.Default = null;

			if (carryEntity != null && carryEntity.IsAlive) {
				DropObjectBase(enterBusyState, playSound);
				carryEntity.OnDrop();
			}
		}

		private void DropObjectBase(bool enterBusyState = true, bool playSound = true) {
			PlayerAnimations.Default = null;
			
			isObjectDropped = true;
			player.RoomControl.SpawnEntity(carryEntity, player.Position, 16);
			if (enterBusyState) {
				player.Graphics.PlayAnimation(player.Animations.Throw);
				StateMachine.BeginState(new PlayerBusyState(
					throwDuration, player.StateParameters.PlayerAnimations.Throw));
			}
			else if (isPickingUp) {
				StateParameters.ProhibitMovementControl = false;
				isPickingUp = false;
			}
			if (playSound)
				AudioSystem.PlaySound(GameData.SOUND_PLAYER_THROW);
		}

		public void ThrowObject(bool enterBusyState = true, bool playSound = true) {
			if (carryEntity != null && carryEntity.IsAlive) {
				carryEntity.Physics.ZVelocity = 1.0f;
				carryEntity.Physics.Velocity = Directions.ToVector(player.MoveDirection) * 1.5f;
				DropObjectBase(enterBusyState, playSound);
				carryEntity.OnThrow();
			}
		}

		public void ReleaseObject(bool enterBusyState = true, bool playSound = true) {
			// Prevent throwing during pickup phase
			if (player.Movement.IsMoving && !isPickingUp)
				ThrowObject(enterBusyState, playSound);
			else
				DropObject(enterBusyState, playSound);
		}

		public void DestroyObject() {
			isObjectDropped = true;
			carryEntity.Destroy();
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------
		
		public override void OnBegin(PlayerState previousState) {
			carryEntity.Initialize(player.RoomControl);

			isObjectDropped = false;
			
			objectZOffset = 0;
			//if (carryEntity is CarriedTile)
			//	objectDrawOffset.Y -= 2;
			objectDrawOffset = Directions.ToPoint(player.Direction) * 8;
			pickupTimer = 0;
			isPickingUp = true;

			StateParameters.ProhibitMovementControl		= true;
			StateParameters.ProhibitJumping				= true;
			StateParameters.ProhibitLedgeJumping		= true;
			StateParameters.ProhibitWeaponUse			= true;
			StateParameters.ProhibitWarping				= true;

			player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_PULL);
			AudioSystem.PlaySound(GameData.SOUND_PLAYER_PICKUP);
			carryEntity.OnPickup();
		}
		
		public override void OnEnd(PlayerState newState) {
			if (!isObjectDropped)
				DropObject(false, false);
		}

		public override void OnHurt(DamageInfo damage) {
			base.OnHurt(damage);
			ReleaseObject(false, false);
			End();
		}

		public override void Update() {
			base.Update();

			if (isPickingUp) {
				pickupTimer++;
				
				// Handle the 2 frames of picking up.
				if (pickupTimer < pickupFrame1Duration) {
					objectZOffset = 0;
					objectDrawOffset = Directions.ToPoint(player.Direction) * 8;
					player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_PULL);
				}
				else if (pickupTimer < pickupFrame1Duration + pickupFrame2Duration) {
					objectZOffset = 8;
					objectDrawOffset = Directions.ToPoint(player.Direction) * 2;
					player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_GRAB);
				}
				else {
					objectDrawOffset	= Point2I.Zero;
					objectZOffset		= 13;
					isPickingUp = false;
					StateParameters.ProhibitMovementControl	= false;
					PlayerAnimations.Default = player.Animations.Carry;
					carryEntity.OnCarry();
				}
			}
			else {
				PlayerAnimations.Default = player.Animations.Carry;

				// Update the carried object.
				objectDrawOffset		= Point2I.Zero;
				objectZOffset			= 13;
				carryEntity.RoomControl	= player.RoomControl;
				carryEntity.Position	= player.Position;
				carryEntity.ZPosition	= player.ZPosition + 16;

			}
			carryEntity.Physics.SurfacePosition = player.Physics.SurfacePosition;

			carryEntity.UpdateCarrying(isPickingUp);
			if (carryEntity.IsDestroyed) {
				End();
				return;
			}
		}
		
		public override void DrawOver(RoomGraphics g) {
			carryEntity.Position	= player.Position + objectDrawOffset;
			carryEntity.ZPosition	= player.ZPosition + objectZOffset;
			if (!isPickingUp && Directions.IsHorizontal(player.Direction))
				carryEntity.ZPosition += 1;

			// Handle head bobbing when the player is moving horizontally.
			float playbackTime = player.Graphics.AnimationPlayer.PlaybackTime;
			if (!isPickingUp && Directions.IsHorizontal(player.Direction)
				&& playbackTime >= 2 && playbackTime < 8) // TODO: magic number
			{
				carryEntity.ZPosition -= 1;
			}

			// Draw the object.
			carryEntity.Physics.SurfacePosition = player.Physics.SurfacePosition;
			carryEntity.Position += carryEntity.CarriedDrawOffset;
			carryEntity.DrawCarrying(g, isPickingUp);
			carryEntity.Position -= carryEntity.CarriedDrawOffset;
		}
	}
}
