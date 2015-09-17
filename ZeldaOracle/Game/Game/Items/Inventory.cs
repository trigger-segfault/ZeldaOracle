using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Game.Control;

namespace ZeldaOracle.Game.Items {
	public class Inventory {

		// The game control for the current game session.
		private GameControl gameControl;
		// The list of items in the game.
		private List<Item> items;
		// The list of ammos in the game.
		private List<Ammo> ammo;
		// The player's equip slots.
		private PlayerItem[] equippedItems;

		//-----------------------------------------------------------------------------
		// Constants
		//-----------------------------------------------------------------------------

		// The number of slots to equip player items.
		public const int NumEquipSlots = 2;

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public Inventory(GameControl gameControl) {
			this.gameControl	= gameControl;
			this.items			= new List<Item>();
			this.ammo			= new List<Ammo>();
			this.equippedItems	= new PlayerItem[NumEquipSlots];
		}


		//-----------------------------------------------------------------------------
		// Items
		//-----------------------------------------------------------------------------

		// Equip a player item into the given slot (slot 0 (A) or 1 (B))
		public void EquipPlayerItem(PlayerItem item, int slot) {
			if (item.IsTwoHanded) {
				equippedItems[0] = item;
				equippedItems[1] = item;
			}
			else {
				equippedItems[slot] = item;
			}
		}

		// Adds the item to the list
		public void AddItem(Item item) {
			this.items.Add(item);
		}

		// Gets the item at the specified index
		public Item GetItem(int index) {
			return items[index];
		}

		// Gets the item with the specified id
		public Item GetItem(string id) {
			foreach (Item item in items) {
				if (item.ID == id)
					return item;
			}
			return null;
		}

		// Checks if the item exists
		public bool ItemExists(string id) {
			foreach (Item item in items) {
				if (item.ID == id)
					return true;
			}
			return false;
		}

		// Checks if the item has been obtained
		public bool IsItemObtained(string id) {
			foreach (Item item in items) {
				if (item.ID == id)
					return item.IsObtained;
			}
			return false;
		}

		// Checks if the item has been obtained and is not stolen
		public bool IsItemAvailable(string id) {
			foreach (Item item in items) {
				if (item.ID == id)
					return item.IsObtained && !item.IsStolen;
			}
			return false;
		}


		//-----------------------------------------------------------------------------
		// Ammo
		//-----------------------------------------------------------------------------

		// Adds the ammo type to the list
		public void AddAmmo(Ammo ammo) {
			this.ammo.Add(ammo);
		}

		// Gets the ammo class with the specified id
		public Ammo GetAmmo(string id) {
			foreach (Ammo ammo in this.ammo) {
				if (ammo.ID == id)
					return ammo;
			}
			return null;
		}

		// Checks if the ammo exists
		public bool AmmoExists(string id) {
			foreach (Ammo ammo in this.ammo) {
				if (ammo.ID == id)
					return true;
			}
			return false;
		}

		// Checks if the ammo has been obtained
		public bool IsAmmoObtained(string id) {
			foreach (Ammo ammo in this.ammo) {
				if (ammo.ID == id)
					return ammo.IsObtained;
			}
			return false;
		}

		// Checks if the ammo has been obtained and is not stolen
		public bool IsAmmoAvailable(string id) {
			foreach (Ammo ammo in this.ammo) {
				if (ammo.ID == id)
					return ammo.IsObtained && !ammo.IsStolen;
			}
			return false;
		}
	}
}
