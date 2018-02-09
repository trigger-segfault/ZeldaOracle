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
		
		// True if the player allowed to hold the sword after stabbing.
		// This is false when the player stabs a monster.
		private bool continueHolding;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public PlayerSwordStabState() {
			weapon = null;
			continueHolding = true;

			StateParameters.ProhibitMovementControl = true;
		}
		

		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnBegin(PlayerState previousState) {
			// TODO: allows sideways movement for stabbing when jumping
			player.EquipTool(player.ToolVisual);
			player.ToolVisual.PlayAnimation(GameData.ANIM_SWORD_STAB);

			player.Graphics.PlayAnimation(
				player.StateParameters.PlayerAnimations.Stab);

			player.ToolVisual.AnimationPlayer.SubStripIndex = player.Direction;
		}
		
		public override void OnEnd(PlayerState newState) {
			player.UnequipTool(player.ToolVisual);
		}

		public override void Update() {
			base.Update();

			if (player.Graphics.IsAnimationDone) {
				if (weapon.IsEquipped && continueHolding && weapon.IsButtonDown()) {
					// Continue holding sword
					StateMachine.BeginState(player.HoldSwordState);
				}
				else {
					// Put sword away
					player.UnequipTool(player.ToolVisual);
					End();
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

		public bool ContinueHoldingSword {
			get { return continueHolding; }
			set { continueHolding = value; }
		}
	}
}
