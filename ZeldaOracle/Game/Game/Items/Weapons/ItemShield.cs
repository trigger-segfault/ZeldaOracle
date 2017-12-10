using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Graphics.Sprites;

namespace ZeldaOracle.Game.Items.Weapons {
	public class ItemShield : ItemWeapon {

		private Animation   shieldAnimation;
		private Animation   shieldBlockAnimation;
		private bool		isShieldBlocking;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ItemShield() {
			this.id				= "item_shield";
			this.name			= new string[] { "Wooden Shield", "Iron Shield", "Mirror Shield" };
			this.description	= new string[] { "A small shield.", "A large shield.", "A reflective shield." };
			this.maxLevel		= Item.Level3;
			this.flags			= ItemFlags.UsableWhileJumping | ItemFlags.UsableWithSword | ItemFlags.UsableWhileInHole;

			this.sprite = new ISprite[] {
				GameData.SPR_ITEM_ICON_SHIELD_1,
				GameData.SPR_ITEM_ICON_SHIELD_2,
				GameData.SPR_ITEM_ICON_SHIELD_3
			};

			shieldAnimation = GameData.ANIM_PLAYER_SHIELD;
			shieldBlockAnimation = GameData.ANIM_PLAYER_SHIELD_BLOCK;
		}


		//-----------------------------------------------------------------------------
		// Internal methods
		//-----------------------------------------------------------------------------

		private void StartBlocking() {
			isShieldBlocking		= true;
			Player.Movement.CanPush	= false;
			Player.MoveAnimation	= shieldBlockAnimation;
			if (Player.CurrentState == Player.NormalState && Player.IsOnGround)
				AudioSystem.PlaySound(GameData.SOUND_SHIELD);
		}
		
		private void StopBlocking() {
			isShieldBlocking		= false;
			Player.Movement.CanPush	= true;
			Player.MoveAnimation	= shieldAnimation;
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnLevelUp() {
			if (level == Item.Level1) {
				shieldAnimation			= GameData.ANIM_PLAYER_SHIELD;
				shieldBlockAnimation	= GameData.ANIM_PLAYER_SHIELD_BLOCK;
			}
			else {
				shieldAnimation			= GameData.ANIM_PLAYER_SHIELD_LARGE;
				shieldBlockAnimation	= GameData.ANIM_PLAYER_SHIELD_LARGE_BLOCK;
			}

			if (isEquipped) {
				if (isShieldBlocking)
					Player.MoveAnimation = shieldBlockAnimation;
				else
					Player.MoveAnimation = shieldAnimation;
			}
		}

		// Called when the item is switched to.
		public override void OnEquip() {
			isShieldBlocking = false;
			Player.MoveAnimation = shieldAnimation;
		}

		// Called when the item is put away.
		public override void OnUnequip() {
			StopBlocking();
			Player.MoveAnimation = GameData.ANIM_PLAYER_DEFAULT;
		}

		// Immediately interrupt this item (ex: if the player falls in a hole).
		public override void Interrupt() {
			StopBlocking();
			Player.MoveAnimation = shieldAnimation;
		}

		// Update the item.
		public override void Update() {
			if (IsShieldBlocking) {
				// Stop shield blocking.
				if (!IsButtonDown())
					StopBlocking();
			}
			else {
				// Start shield blocking.
				if (IsButtonDown())
					StartBlocking();
			}
		}
		


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public bool IsShieldBlocking {
			get { return isShieldBlocking; }
		}
	}
}
