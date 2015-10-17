using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;

namespace ZeldaOracle.Game.Items.Weapons {
	public class ItemShield : ItemWeapon {

		private Animation	shieldAnimation;
		private Animation	shieldBlockAnimation;
		private bool		isShieldBlocking;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ItemShield() {
			this.id				= "item_shield";
			this.name			= new string[] { "Wooden Shield", "Iron Shield", "Mirror Shield" };
			this.description	= new string[] { "A small shield.", "A large shield.", "A reflective shield." };
			this.maxLevel		= Item.Level3;
			this.flags			= ItemFlags.UsableWhileJumping | ItemFlags.UsableWithSword;

			this.sprite			= new Sprite[] {
				new Sprite(GameData.SHEET_ITEMS_SMALL, new Point2I(3, 0)),
				new Sprite(GameData.SHEET_ITEMS_SMALL, new Point2I(4, 0)),
				new Sprite(GameData.SHEET_ITEMS_SMALL, new Point2I(5, 0))
			};

			shieldAnimation = GameData.ANIM_PLAYER_SHIELD;
			shieldBlockAnimation = GameData.ANIM_PLAYER_SHIELD_BLOCK;
		}
		


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		// Called when the item is switched to.
		public override void OnEquip() {
			isShieldBlocking = false;
			Player.MoveAnimation = shieldAnimation;
		}

		// Called when the item is put away.
		public override void OnUnequip() {
			isShieldBlocking = false;
			Player.MoveAnimation = GameData.ANIM_PLAYER_DEFAULT;
		}

		// Immediately interrupt this item (ex: if the player falls in a hole).
		public override void Interrupt() {
			// Stop shield blocking.
			isShieldBlocking = false;
			Player.MoveAnimation = shieldAnimation;
		}

		// Update the item.
		public override void Update() {
			if (IsShieldBlocking) {
				// Stop shield blocking.
				if (!IsButtonDown()) {
					isShieldBlocking = false;
					Player.MoveAnimation = shieldAnimation;
				}
			}
			else {
				// Start shield blocking.
				if (IsButtonDown()) {
					isShieldBlocking = true;
					Player.MoveAnimation = shieldBlockAnimation;
					if (Player.CurrentState == Player.NormalState && Player.IsOnGround)
						AudioSystem.PlaySound("Items/shield");
				}
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
