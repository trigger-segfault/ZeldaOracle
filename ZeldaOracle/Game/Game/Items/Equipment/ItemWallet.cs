using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;

namespace ZeldaOracle.Game.Items.Equipment {
	public class ItemWallet : Item {

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ItemWallet(int level = 0) {
			this.id = "item_wallet";
			this.name = new string[] { "Child's Wallet", "Adult's Wallet", "Giant's Wallet" };
			this.description = new string[] {
				"Allows you to carry a measly 99 rupees.",
				"Allows you to carry 300 rupees!",
				"Allows you to carry a whopping 999 rupees!"
			};
			this.level = GMath.Clamp(level, Item.Level1, Item.Level3);
			this.maxLevel = Item.Level3;
		}


		//-----------------------------------------------------------------------------
		// Virtual
		//-----------------------------------------------------------------------------

		// Called when the item is added to the inventory list
		public override void OnAdded(Inventory inventory) {
			base.OnAdded(inventory);
			int[] maxAmounts = { 99, 300, 999 };

			inventory.AddAmmo(
				new Ammo(
					"rupees", "Rupees", "A currency.",
					GameData.SHEET_ITEMS_SMALL.GetSprite(5, 3),
					0, maxAmounts[level]
				),
				false
			);
		}

		// Called when the item's level is changed.
		public override void OnLevelUp() {
			int[] maxAmounts = {99, 300, 999};
			inventory.GetAmmo("rupees").MaxAmount = maxAmounts[level];
		}

		// Called when the item has been obtained.
		public override void OnObtained() {
			inventory.ObtainAmmo(inventory.GetAmmo("rupees"));
		}

		// Called when the item has been unobtained.
		public override void OnUnobtained() {
			inventory.GetAmmo("rupees").IsObtained = false;
		}

		// Called when the item has been stolen.
		public override void OnStolen() {
			inventory.GetAmmo("rupees").IsStolen = true;
		}

		// Called when the stolen item has been returned.
		public override void OnReturned() {
			inventory.GetAmmo("rupees").IsStolen = false;
		}

	}
}
