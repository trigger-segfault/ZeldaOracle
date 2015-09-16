using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeldaOracle.Game.Items {
	public class Inventory {

		private List<Item>		items;
		private List<Ammo>		ammo;
		private PlayerItem[]	equippedPlayerItems;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public Inventory() {
			items	= new List<Item>();
			ammo	= new List<Ammo>();
			equippedPlayerItems = new PlayerItem[2];
		}


		//-----------------------------------------------------------------------------
		// Items
		//-----------------------------------------------------------------------------

		// Equip a player item into the given slot (slot 0 (A) or 1 (B))
		public void EquipPlayerItem(PlayerItem item, int slot) {
			if (item.IsTwoHanded) {
				equippedPlayerItems[0] = item;
				equippedPlayerItems[1] = item;
			}
			else {
				equippedPlayerItems[slot] = item;
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
