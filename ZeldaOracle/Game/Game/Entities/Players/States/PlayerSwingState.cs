using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Main;

namespace ZeldaOracle.Game.Entities.Players.States {
	public class PlayerSwingState : PlayerState {

		private PlayerState nextState;
		private Animation weaponAnimation;
		private int equipSlot;
		
		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public PlayerSwingState() {
			this.weaponAnimation	= null;
			this.nextState			= null;
			this.equipSlot			= 0;
		}
		

		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnBegin() {
			base.OnBegin();
			player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_SWING);
			player.Movement.AllowMovementControl = false;
			player.toolAnimation.Animation = weaponAnimation;
			player.toolAnimation.SubStripIndex = player.Direction;
			player.toolAnimation.Play();
		}
		
		public override void OnEnd() {
			player.Movement.AllowMovementControl = true;
			player.toolAnimation.Animation = null;
			base.OnEnd();
		}

		public override void Update() {
			base.Update();
			if (player.GameControl.Inventory.GetSlotButton(equipSlot).IsPressed()) {
				for (int i = 0; i < 4; i++) {
					if (Controls.Arrows[i].IsDown()) {
						player.Direction = i;
						player.Angle = Directions.ToAngle(i);
					}
				}
				player.toolAnimation.SubStripIndex = player.Direction;
				player.Graphics.PlayAnimation();
				player.toolAnimation.Play();
			}
			if (player.Graphics.IsAnimationDone) {
				player.BeginState(nextState);
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
