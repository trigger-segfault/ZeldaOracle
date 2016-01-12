using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Items;
using ZeldaOracle.Game.Main;
using ZeldaOracle.Game.Tiles;

namespace ZeldaOracle.Game.Entities.Players.States {
	public class PlayerSwordStabState : PlayerState {

		private ItemWeapon weapon;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public PlayerSwordStabState() {
			this.weapon = null;
		}
		

		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnBegin(PlayerState previousState) {
			player.Movement.MoveCondition = PlayerMoveCondition.NoControl; // TODO: allows sideways movement for stabbing when jumping.
			player.EquipTool(player.ToolVisual);
			player.ToolVisual.PlayAnimation(GameData.ANIM_SWORD_STAB);
			player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_STAB);
			player.ToolVisual.AnimationPlayer.SubStripIndex = player.Direction;
		}
		
		public override void OnEnd(PlayerState newState) {
			player.Movement.MoveCondition = PlayerMoveCondition.FreeMovement;
			player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_DEFAULT);
			player.UnequipTool(player.ToolVisual);
		}

		public override void Update() {
			base.Update();

			if (player.Graphics.IsAnimationDone) {
				if (weapon.IsEquipped && weapon.IsButtonDown()) {
					// Continue holding sword.
					player.BeginState(Player.HoldSwordState);
				}
				else {
					// Put sword away.
					player.UnequipTool(player.ToolVisual);
					player.BeginNormalState();
				}
			}
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public ItemWeapon Weapon {
			get { return weapon; }
			set { weapon = value; }
		}
	}
}
