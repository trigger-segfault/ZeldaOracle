using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Input;
using ZeldaOracle.Game.Control;
using ZeldaOracle.Game.Main;

namespace ZeldaOracle.Game.Items {
	public class Inventory {

		// The game control for the current game session.
		private GameControl gameControl;
		// The list of items in the game.
		private List<Item> items;
		// The list of ammos in the game.
		private List<Ammo> ammo;
		// The player's equip slots.
		private ItemWeapon[] equippedWeapons;

		private int piecesOfHeart;


		//-----------------------------------------------------------------------------
		// Constants
		//-----------------------------------------------------------------------------

		// The number of slots to equip player items.
		public const int NumEquipSlots = 2;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public Inventory(GameControl gameControl) {
			this.gameControl		= gameControl;
			this.items				= new List<Item>();
			this.ammo				= new List<Ammo>();
			this.equippedWeapons	= new ItemWeapon[NumEquipSlots];

			this.piecesOfHeart		= 0;
		}


		//-----------------------------------------------------------------------------
		// Items
		//-----------------------------------------------------------------------------

		public InputControl GetSlotButton(int slot) {
			return (slot == 0 ? Controls.A : Controls.B);
		}

		// Equip a weapon into the given slot (slot 0 (A) or 1 (B)).
		public void EquipWeapon(Item item, int slot) {
			ItemWeapon weapon = item as ItemWeapon;
			if (weapon != null && weapon.HasFlag(ItemFlags.TwoHanded)) {
				// Unequip the current items.
				if (equippedWeapons[0] != null)
					equippedWeapons[0].Unequip();
				if (equippedWeapons[1] != null)
					equippedWeapons[1].Unequip();

				equippedWeapons[0] = weapon;
				equippedWeapons[1] = weapon;
			}
			else {
				// Unequip the current item.
				if (equippedWeapons[slot] != null) {
					if (equippedWeapons[slot].HasFlag(ItemFlags.TwoHanded))
						equippedWeapons[1 - slot] = null;
					equippedWeapons[slot].Unequip();
				}

				equippedWeapons[slot] = weapon;
			}

			// Equip the new item.
			if (weapon != null) {
				weapon.CurrentEquipSlot = slot;
				weapon.Equip();
			}
		}

		// Equips a non-usable item.
		public void EquipEquipment(Item item) {
			ItemEquipment equippableItem = item as ItemEquipment;
		}
		

		// Adds multiple item to the list
		public void AddItems(bool obtain, params Item[] items) {
			for (int i = 0; i < items.Length; i++)
				AddItem(items[i], obtain);
		}

		// Adds the item to the list
		public Item AddItem(Item item, bool obtain) {
			foreach (Item item2 in items) {
				if (item2.ID == item.ID) {
					if (obtain)
						ObtainItem(item2);
					return item2;
				}
			}
			this.items.Add(item);
			item.OnAdded(this);
			if (obtain)
				ObtainItem(item);
			return item;
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

		public void ObtainItem(string id) {
			ObtainItem(GetItem(id));
		}

		public void ObtainItem(Item item) {
			if (!item.IsObtained) {
				item.IsObtained = true;
				item.OnObtained();
				if (item is ItemWeapon) {
					if (equippedWeapons[0] == null)
						EquipWeapon(item, 0);
					else if (equippedWeapons[1] == null)
						EquipWeapon(item, 1);
					else
						gameControl.MenuWeapons.NextAvailableSlot.SlotItem = item;
				}
				else if (item is ItemSecondary) {
					ItemSecondary secondary = item as ItemSecondary;
					gameControl.MenuSecondaryItems.GetSecondarySlotAt(secondary.SecondarySlot).SlotItem = item;
				}
				else if (item is ItemEssence) {
					ItemEssence essence = item as ItemEssence;
					gameControl.MenuEssences.GetEssenceSlotAt(essence.EssenceSlot).SlotItem = item;
				}
			}
		}

		public void UnobtainItem(Item item) {
			if (item.IsObtained) {
				item.IsObtained = false;
				item.OnUnobtained();
				if (item is ItemWeapon) {
					if (equippedWeapons[0] == item)
						EquipWeapon(null, 0);
					else if (equippedWeapons[1] == item)
						EquipWeapon(null, 1);
					else
						gameControl.MenuWeapons.NextAvailableSlot.SlotItem = item;
				}
			}
		}


		//-----------------------------------------------------------------------------
		// Ammo
		//-----------------------------------------------------------------------------

		// Adds the ammo type to the list.
		public Ammo AddAmmo(Ammo ammo, bool obtain) {
			foreach (Ammo ammo2 in this.ammo) {
				if (ammo2.ID == ammo.ID) {
					if (obtain)
						ObtainAmmo(ammo2);
					return ammo2;
				}
			}
			this.ammo.Add(ammo);
			if (obtain)
				ObtainAmmo(ammo);
			return ammo;
		}

		// Gets the ammo class with the specified id.
		public Ammo GetAmmo(string id) {
			foreach (Ammo ammo in this.ammo) {
				if (ammo.ID == id)
					return ammo;
			}
			return null;
		}

		// Checks if the ammo exists.
		public bool AmmoExists(string id) {
			foreach (Ammo ammo in this.ammo) {
				if (ammo.ID == id)
					return true;
			}
			return false;
		}

		// Checks if the ammo has been obtained.
		public bool IsAmmoObtained(string id) {
			foreach (Ammo ammo in this.ammo) {
				if (ammo.ID == id)
					return ammo.IsObtained;
			}
			return false;
		}

		// Checks if the ammo has been obtained and is not stolen.
		public bool IsAmmoAvailable(string id) {
			foreach (Ammo ammo in this.ammo) {
				if (ammo.ID == id)
					return ammo.IsObtained && !ammo.IsStolen;
			}
			return false;
		}

		// Fills all the ammo in the player's inventory.
		public void FillAllAmmo() {
			foreach (Ammo ammo in this.ammo) {
				ammo.Amount = ammo.MaxAmount;
				// Prevent rupee spamming sound.
				if (ammo.ID == "rupees")
					gameControl.HUD.DynamicRupees = ammo.Amount;
			}
		}

		// Empties all the ammo from the player's inventory.
		public void EmptyAllAmmo() {
			foreach (Ammo ammo in this.ammo) {
				ammo.Amount = 0;
				// Prevent rupee spamming sound.
				if (ammo.ID == "rupees")
					gameControl.HUD.DynamicRupees = ammo.Amount;
			}
		}

		public void ObtainAmmo(string id) {
			ObtainAmmo(GetAmmo(id));
		}

		public void ObtainAmmo(Ammo ammo) {
			if (!ammo.IsObtained) {
				ammo.IsObtained = true;
			}
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		// Gets the equipped weapons list.
		public ItemWeapon[] EquippedWeapons {
			get { return equippedWeapons; }
		}

		// Gets if a two handed weapon is equipped.
		public bool IsTwoHandedEquipped {
			get { return (equippedWeapons[0] != null ? equippedWeapons[0].HasFlag(ItemFlags.TwoHanded) : false); }
		}

		public int PiecesOfHeart {
			get { return piecesOfHeart; }
			set { piecesOfHeart = value; }
		}
	}
}
