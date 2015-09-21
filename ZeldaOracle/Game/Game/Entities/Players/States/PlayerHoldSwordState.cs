using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Main;

namespace ZeldaOracle.Game.Entities.Players.States {
	public class PlayerHoldSwordState : PlayerState {

		private PlayerState nextState;
		private Animation weaponAnimation;
		private int equipSlot;
		private int chargeTimer;
		private int direction;

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
			if (player.GameControl.Inventory.GetSlotButton(equipSlot).IsDown()) {
				chargeTimer = 0;
				player.Movement.AllowMovementControl = true;
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

			if (player.Movement.IsMoving && !Player.Graphics.IsAnimationPlaying)
				Player.Graphics.PlayAnimation();
			if (!player.Movement.IsMoving && Player.Graphics.IsAnimationPlaying)
				Player.Graphics.StopAnimation();

			chargeTimer++;
			if (chargeTimer == ChargeTime) {
				player.toolAnimation.Animation = GameData.ANIM_SWORD_CHARGED;
				// Play charge sound.
			}

			if (player.GameControl.Inventory.GetSlotButton(equipSlot).IsUp()) {
				if (chargeTimer >= ChargeTime)
					player.BeginState(player.SpinSwordState);
				else
					player.BeginNormalState();
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
