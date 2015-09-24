using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Main;
using ZeldaOracle.Game.Tiles;

namespace ZeldaOracle.Game.Entities.Players.States {
	public class PlayerHoldSwordState : PlayerState {

		private PlayerState nextState;
		private Animation weaponAnimation;
		private int equipSlot;
		private int chargeTimer;
		private int direction;
		private bool isStabbing;


		//-----------------------------------------------------------------------------
		// Constants
		//-----------------------------------------------------------------------------

		private const int ChargeTime = 40;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public PlayerHoldSwordState() {
			this.weaponAnimation	= null;
			this.nextState			= null;
			this.equipSlot			= 0;
			this.chargeTimer		= 0;
			this.direction			= Directions.Right;
		}
		
		
		//-----------------------------------------------------------------------------
		// Internal methods
		//-----------------------------------------------------------------------------

		private void StabTile(Tile tile) {
			isStabbing	= true;
			chargeTimer	= 0;
			player.Movement.MoveCondition = PlayerMoveCondition.NoControl; // TODO: allows sideways movement for stabbing.
			player.toolAnimation.Play(GameData.ANIM_SWORD_STAB);
			player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_STAB);
			if (player.IsOnGround)
				tile.OnSwordHit();
		}
		
		private void EndStab() {
			isStabbing	= false;
			chargeTimer	= 0;
			player.Movement.MoveCondition	= PlayerMoveCondition.FreeMovement;
			player.toolAnimation.Animation	= weaponAnimation;
			player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_DEFAULT);
			if (player.GameControl.Inventory.GetSlotButton(equipSlot).IsUp())
				player.BeginNormalState();
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnBegin(PlayerState previousState) {
			isStabbing = false;
			
			// The player can hold his sword while ledge jumping.

			if (!(previousState is PlayerLedgeJumpState))
				chargeTimer = 0;

			if (player.GameControl.Inventory.GetSlotButton(equipSlot).IsDown()) {
				player.Movement.IsStrafing		= true;
				player.Movement.MoveCondition	= PlayerMoveCondition.FreeMovement;
				player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_DEFAULT);
				player.toolAnimation.SubStripIndex = player.Direction;
				player.toolAnimation.Play();
				direction = Player.Direction;
				if (!(previousState is PlayerLedgeJumpState))
					player.toolAnimation.Animation = weaponAnimation;
			}
			else {
				if (chargeTimer >= ChargeTime)
					player.BeginState(player.SpinSwordState);
				else
					player.BeginNormalState();
			}
		}
		
		public override void OnEnd(PlayerState newState) {
			player.Movement.IsStrafing		= false;
			player.Movement.MoveCondition	= PlayerMoveCondition.FreeMovement;
			
			// The player can hold his sword while ledge jumping.
			if (!(newState is PlayerLedgeJumpState))
				player.toolAnimation.Animation = null;
		}

		public override void Update() {
			base.Update();

			player.Direction = direction;

			if (isStabbing) {
				if (player.Graphics.IsAnimationDone)
					EndStab();
			}
			else {
				// Charge up the sword.
				chargeTimer++;
				 if (chargeTimer == ChargeTime) {
					player.toolAnimation.Animation = GameData.ANIM_SWORD_CHARGED;
					// TODO: play charge sound.
				}
			
				// Check for tiles to stab.
				CollisionInfo collisionInfo = player.Physics.CollisionInfo[player.Direction];
				Tile tile = player.Physics.GetMeetingSolidTile(player.Position, player.Direction);
				if (tile != null && player.Movement.IsMoving && collisionInfo.Type == CollisionType.Tile)
					StabTile(tile);

				// Release the sword button (spin if charged).
				else if (player.GameControl.Inventory.GetSlotButton(equipSlot).IsUp()) {
					if (chargeTimer >= ChargeTime)
						player.BeginState(player.SpinSwordState);
					else
						player.BeginNormalState();
				}
			}
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public PlayerState NextState {
			get { return nextState; }
			set { nextState = value; }
		}

		public Animation WeaponAnimation {
			get { return weaponAnimation; }
			set { weaponAnimation = value; }
		}

		public int EquipSlot {
			get { return equipSlot; }
			set { equipSlot = value; }
		}
	}
}
