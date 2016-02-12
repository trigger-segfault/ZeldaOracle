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

		private Entity carryObject;
		private int throwDuration; // How many ticks the player waits when throwing.
		private int pickupFrame1Duration;
		private int pickupFrame2Duration;
		


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------
		
		public PlayerCarryState() {
		}

		public PlayerCarryState(Entity carryObject) {
			this.carryObject			= carryObject;
			this.throwDuration			= 8;
			this.pickupFrame1Duration	= 4;
			this.pickupFrame2Duration	= 4;
		}

		public PlayerCarryState(Tile carryTile) {
			this.carryObject			= new CarriedTile(carryTile);
			this.throwDuration			= 2;
			this.pickupFrame1Duration	= 6;
			this.pickupFrame2Duration	= 4;
			this.carryObject.Graphics.ImageVariant = carryTile.Zone.ImageVariantID;
		}

		
		//-----------------------------------------------------------------------------
		// Internal methods.
		//-----------------------------------------------------------------------------
		
		public void SetCarryObject(object carryObject) {
			if (carryObject is Tile) {
				Tile carryTile = (Tile) carryObject;
				this.carryObject			= new CarriedTile(carryTile);
				this.throwDuration			= 2;
				this.pickupFrame1Duration	= 6;
				this.pickupFrame2Duration	= 4;
				this.carryObject.Graphics.ImageVariant = carryTile.Zone.ImageVariantID;
			}
			else if (carryObject is Entity) {
				this.carryObject			= (Entity) carryObject;
				this.throwDuration			= 8;
				this.pickupFrame1Duration	= 4;
				this.pickupFrame2Duration	= 4;
			}
		}

		public void DropObject(bool enterBusyState = true, bool playSound = true) {
			if (carryObject != null && carryObject.IsAlive) {
				isObjectDropped = true;
				player.RoomControl.SpawnEntity(carryObject, player.Position, 16);
				if (enterBusyState) {
					player.BeginBusyState(throwDuration);
					player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_THROW);
				}
				if (playSound)
					AudioSystem.PlaySound(GameData.SOUND_PLAYER_THROW);
			}
		}

		public void ThrowObject(bool enterBusyState = true, bool playSound = true) {
			if (carryObject != null && carryObject.IsAlive) {
				carryObject.Physics.ZVelocity = 1.0f;
				carryObject.Physics.Velocity = Directions.ToVector(Player.MoveDirection) * 1.5f;
				DropObject(enterBusyState, playSound);
			}
		}

		public void DestroyObject() {
			isObjectDropped = true;
			carryObject.Destroy();
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------
		
		public override void OnBegin(PlayerState previousState) {
			carryObject.Initialize(player.RoomControl);

			isObjectDropped = false;
			
			objectZOffset = 0;
			if (carryObject is CarriedTile)
				objectDrawOffset.Y -= 2;
			objectDrawOffset = Directions.ToPoint(player.Direction) * 8;
			pickupTimer = 0;
			isPickingUp = true;
			player.Movement.CanJump			= false;
			player.Movement.CanLedgeJump	= false;
			player.Movement.CanUseWarpPoint	= false;
			player.Movement.MoveCondition	= PlayerMoveCondition.NoControl;
			Player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_PULL);
			AudioSystem.PlaySound(GameData.SOUND_PLAYER_PICKUP);
		}
		
		public override void OnEnd(PlayerState newState) {
			player.Movement.CanJump			= true;
			player.Movement.CanLedgeJump	= true;
			player.Movement.CanUseWarpPoint	= true;
			
			if (!isObjectDropped) {
				DropObject(false, false);
			}
		}

		public override void OnEnterMinecart() {
			if (Player.Graphics.Animation == GameData.ANIM_PLAYER_CARRY)
				Player.Graphics.Animation = GameData.ANIM_PLAYER_MINECART_CARRY;
		}

		public override void OnExitMinecart() {
			if (Player.Graphics.Animation == GameData.ANIM_PLAYER_MINECART_CARRY)
				Player.Graphics.Animation = GameData.ANIM_PLAYER_CARRY;
		}

		public override void OnHurt(DamageInfo damage) {
			base.OnHurt(damage);
			if (player.Movement.IsMoving)
				ThrowObject(false, false);
			else
 				DropObject(false, false);
			player.BeginNormalState();
		}

		public override void Update() {
			base.Update();

			if (isPickingUp) {
				pickupTimer++;
				
				// Handle the 2 frames of picking up.
				if (pickupTimer < pickupFrame1Duration) {
					objectZOffset = 0;
					if (carryObject is CarriedTile)
						objectDrawOffset.Y -= 2;
					objectDrawOffset = Directions.ToPoint(player.Direction) * 8;
					Player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_PULL);
				}
				else if (pickupTimer < pickupFrame1Duration + pickupFrame2Duration) {
					objectZOffset = 8;
					if (carryObject is CarriedTile)
						objectDrawOffset.Y -= 2;
					objectDrawOffset = Directions.ToPoint(player.Direction) * 2;
					Player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_GRAB);
				}
				else {
					objectDrawOffset	= Point2I.Zero;
					objectZOffset		= 13;
					isPickingUp = false;
					player.Movement.MoveCondition = PlayerMoveCondition.FreeMovement;
					if (player.IsInMinecart)
						Player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_MINECART_CARRY);
					else
						Player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_CARRY);
				}
			}
			else {
				// Update the carried object.
				objectDrawOffset		= Point2I.Zero;
				objectZOffset			= 13;
				carryObject.RoomControl	= player.RoomControl;
				carryObject.Position	= player.Position;
				carryObject.ZPosition	= player.ZPosition + 16;
				carryObject.UpdateCarrying();
				if (carryObject.IsDestroyed) {
					player.BeginNormalState();
					return;
				}

				// Check for button press to throw/drop.
				if (!player.IsStateControlled && (Controls.A.IsPressed() || Controls.B.IsPressed())) {
					if (Controls.A.IsPressed())
						Controls.A.Reset();
					if (Controls.B.IsPressed())
						Controls.B.Reset();
					if (player.Movement.IsMoving)
						ThrowObject();
					else
						DropObject();
				}
			}
		}
		
		public override void DrawOver(RoomGraphics g) {
			carryObject.Position	= player.Position + objectDrawOffset;
			carryObject.ZPosition	= player.ZPosition + objectZOffset;
			if (!isPickingUp && Directions.IsHorizontal(player.Direction))
				carryObject.ZPosition += 1;

			// Handle head bobbing when the player is moving horizontally.
			float playbackTime = player.Graphics.AnimationPlayer.PlaybackTime;
			if (!isPickingUp && Directions.IsHorizontal(player.Direction)
				&& playbackTime >= 2 && playbackTime < 8) // TODO: magic number
			{
				carryObject.ZPosition -= 1;
			}

			// Draw the object.
			carryObject.Graphics.Draw(g, DepthLayer.ProjectileCarriedTile);
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

	}
}
