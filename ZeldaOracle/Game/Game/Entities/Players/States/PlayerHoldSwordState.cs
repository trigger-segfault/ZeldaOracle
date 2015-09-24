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
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnBegin() {
			base.OnBegin();

			isStabbing = false;

			if (player.GameControl.Inventory.GetSlotButton(equipSlot).IsDown()) {
				chargeTimer = 0;
				player.Movement.MoveCondition = PlayerMoveCondition.FreeMovement;
				player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_DEFAULT);
				player.toolAnimation.Animation = weaponAnimation;
				player.toolAnimation.SubStripIndex = player.Direction;
				player.toolAnimation.Play();
				direction = Player.Direction;
			}
			else {
				player.BeginNormalState();
			}
		}
		
		public override void OnEnd() {
			player.toolAnimation.Animation = null;
			base.OnEnd();
		}

		public override void Update() {
			base.Update();

			player.Direction = direction;

			if (isStabbing) {
				if (player.Graphics.IsAnimationDone) {
					isStabbing = false;
					chargeTimer = 0;
					player.Movement.MoveCondition = PlayerMoveCondition.FreeMovement;
					player.toolAnimation.Animation = weaponAnimation;
					player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_DEFAULT);
					if (player.GameControl.Inventory.GetSlotButton(equipSlot).IsUp())
						player.BeginNormalState();
				}
			}
			else {
				// Handle move/stand animations.
				//if (player.Movement.IsMoving && !Player.Graphics.IsAnimationPlaying)
				//	Player.Graphics.PlayAnimation();
				//if (!player.Movement.IsMoving && Player.Graphics.IsAnimationPlaying)
				//	Player.Graphics.StopAnimation();

				// Charge up the sword.
				chargeTimer++;
				 if (chargeTimer == ChargeTime) {
					player.toolAnimation.Animation = GameData.ANIM_SWORD_CHARGED;
					// Play charge sound.
				}
			
				// Check for tiles to stab.
				CollisionInfo collisionInfo = player.Physics.CollisionInfo[player.Direction];
				Tile tile = player.Physics.GetMeetingSolidTile(player.Position, player.Direction);
				if (tile != null && player.Movement.IsMoving && collisionInfo.Type == CollisionType.Tile) {
					isStabbing = true;
					chargeTimer = 0;
					player.Movement.MoveCondition = PlayerMoveCondition.NoControl; // Allows sideways movement
					player.toolAnimation.Play(GameData.ANIM_SWORD_STAB);
					player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_STAB);
					if (player.IsOnGround)
						tile.OnSwordHit();
				}

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
