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

namespace ZeldaOracle.Game.Entities.Players.States {
	public class PlayerCarryState : PlayerState {
		
		private bool isPickingUp; // Is the pickup animation playing?
		private int pickupTimer;
		private Point2I objectDrawOffset;

		private Entity carryObject;
		private int throwDuration; // How many ticks the player waits when throwing.
		private int pickupFrame1Duration;
		private int pickupFrame2Duration;
		


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------
		
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
		
		public void DropObject(bool enterBusyState = true) {
			player.RoomControl.SpawnEntity(carryObject, player.Origin, 16);
			if (enterBusyState) {
				player.BeginBusyState(throwDuration);
				player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_THROW);
			}
		}

		public void ThrowObject(bool enterBusyState = true) {
			DropObject(enterBusyState);
			carryObject.Physics.ZVelocity = 1.0f;
			carryObject.Physics.Velocity = Directions.ToVector(Player.MoveDirection) * 1.5f;
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------
		
		public override void OnBegin(PlayerState previousState) {
			carryObject.Initialize(player.RoomControl);

			objectDrawOffset = new Point2I(0, -2);
			objectDrawOffset += Directions.ToPoint(player.Direction) * 8;
			pickupTimer = 0;
			isPickingUp = true;
			player.Movement.CanJump			= false;
			player.Movement.CanLedgeJump	= false;
			player.Movement.MoveCondition	= PlayerMoveCondition.NoControl;
			Player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_PULL);
		}
		
		public override void OnEnd(PlayerState newState) {
			player.Movement.CanJump			= true;
			player.Movement.CanLedgeJump	= true;
			
			if (newState is PlayerSwimState || newState is PlayerLadderState) {
				DropObject(false);
			}
		}

		public override void Update() {
			base.Update();

			if (isPickingUp) {
				pickupTimer++;
				
				// Handle the 2 frames of picking up.
				if (pickupTimer < pickupFrame1Duration) {
					objectDrawOffset = new Point2I(0, -2);
					objectDrawOffset += Directions.ToPoint(player.Direction) * 8;
					Player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_PULL);
				}
				else if (pickupTimer < pickupFrame1Duration + pickupFrame2Duration) {
					objectDrawOffset = new Point2I(0, -10);
					objectDrawOffset += Directions.ToPoint(player.Direction) * 2;
					Player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_GRAB);
				}
				else {
					objectDrawOffset = new Point2I(0, -15);
					isPickingUp = false;
					player.Movement.MoveCondition = PlayerMoveCondition.FreeMovement;
					Player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_CARRY);
				}
			}
			else {
				// Update the carried object.
				carryObject.RoomControl	= player.RoomControl;
				carryObject.Position	= player.Origin;
				carryObject.ZPosition	= player.ZPosition + 16;
				carryObject.UpdateCarrying();
				if (carryObject.IsDestroyed) {
					player.BeginNormalState();
					return;
				}

				// Check for button press to throw/drop.
				if (Controls.A.IsPressed() || Controls.B.IsPressed()) {
					if (player.Movement.IsMoving)
						ThrowObject();
					else
						DropObject();
				}
			}
		}
		
		public override void DrawOver(Graphics2D g) {
			Vector2F pos = player.Position + objectDrawOffset + carryObject.Graphics.DrawOffset;
			pos.Y -= player.ZPosition;

			// Handle head bobbing when the player is moving horizontally.
			if (!isPickingUp &&
				(player.Direction == Directions.Left || player.Direction == Directions.Right)
					&& player.Graphics.AnimationPlayer.PlaybackTime >= 6)
			{
				pos.Y += 1;
			}

			// Draw the object.
			if (carryObject.Graphics.AnimationPlayer.SubStrip != null) {
				g.DrawAnimation(carryObject.Graphics.AnimationPlayer.SubStrip, carryObject.Graphics.ImageVariant,
					carryObject.Graphics.AnimationPlayer.PlaybackTime, pos, 0.0f);
			}
			else if (carryObject.Graphics.Sprite != null)
				g.DrawSprite(carryObject.Graphics.Sprite, carryObject.Graphics.ImageVariant, pos, 0.0f);
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

	}
}
