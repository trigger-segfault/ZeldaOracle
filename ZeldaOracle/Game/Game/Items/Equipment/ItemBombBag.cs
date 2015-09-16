using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeldaOracle.Game.Items.Equipment {
	public class ItemBombBag : Item {

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ItemBombBag()
			: base() {
			this.id = "item_bomb_bag";
			this.name = new string[] { "Bomb Bag", "Big Bomb Bag", "Biggest Bomb Bag" };
			this.description = new string[] {
				"Allows you to carry 10 bombs.",
				"Allows you to carry 20 bombs!",
				"Allows you to carry 30 bombs!"
			};
			this.maxLevel = 2;
		}


		//-----------------------------------------------------------------------------
		// Virtual
		//-----------------------------------------------------------------------------

		// Called when the item is added to the inventory list
		public override void OnAdded(Inventory inventory) {
			base.OnAdded(inventory);

			if (!inventory.AmmoExists("ammo_bombs"))
				inventory.AddAmmo(new Ammo("ammo_bombs", "Bombs", 10, 10));
		}
		// Called when the item's level is changed.
		public override void OnLevelUp() {
			int[] maxAmounts = {10, 20, 30};
			inventory.GetAmmo("ammo_bombs").MaxAmount = maxAmounts[level];
			inventory.GetAmmo("ammo_bombs").Amount = maxAmounts[level];
		}
		// Called when the item has been obtained.
		public override void OnObtained() {
			inventory.GetAmmo("ammo_bombs").IsObtained = true;
			inventory.GetItem("item_bombs").IsObtained = true;
		}
		// Called when the item has been unobtained.
		public override void OnUnobtained() {
			inventory.GetAmmo("ammo_bombs").IsObtained = false;
			inventory.GetItem("item_bombs").IsObtained = false;
		}
		// Called when the item has been stolen.
		public override void OnStolen() {
			inventory.GetAmmo("ammo_bombs").IsStolen = true;
			inventory.GetItem("item_bombs").IsStolen = true;
		}
		// Called when the stolen item has been returned.
		public override void OnReturned() {
			inventory.GetAmmo("ammo_bombs").IsStolen = false;
			inventory.GetItem("item_bombs").IsStolen = false;
		}

	}
}
