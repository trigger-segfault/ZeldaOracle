using System.Collections.Generic;
using ZeldaOracle.Common.Input;
using ZeldaOracle.Game.Control;
using ZeldaOracle.Game.Main;

namespace ZeldaOracle.Game.Items {

	public class Inventory {

		/// <summary>The game control for the current game session.</summary>
		private GameControl gameControl;
		/// <summary>The list of all items in the game.</summary>
		private List<Item> items;
		/// <summary>The list of all ammos in the game.</summary>
		private List<Ammo> ammo;
		/// <summary>The player's equip slots.</summary>
		private ItemWeapon[] equippedWeapons;
		/// <summary>Number of pieces of heart between 0 and 3.</summary>
		private int piecesOfHeart;


		//-----------------------------------------------------------------------------
		// Constants
		//-----------------------------------------------------------------------------

		/// <summary>The number of equip slots for player weapons.</summary>
		public const int NumEquipSlots = 2;

		public const int SLOT_A = 0;
		public const int SLOT_B = 1;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public Inventory(GameControl gameControl) {
			this.gameControl = gameControl;

			items			= new List<Item>();
			ammo			= new List<Ammo>();
			equippedWeapons	= new ItemWeapon[NumEquipSlots];
			piecesOfHeart	= 0;
		}


		//-----------------------------------------------------------------------------
		// Item Management
		//-----------------------------------------------------------------------------

		/// <summary>Equips a non-usable item.</summary>
		public void EquipEquipment(Item item) {
			ItemEquipment equippableItem = item as ItemEquipment;
			if (equippableItem != null)
				equippableItem.Equip();
		}
		
		/// <summary>Adds multiple items to the list of all items in the game.
		/// </summary>
		public void AddItems(bool obtain, params Item[] items) {
			for (int i = 0; i < items.Length; i++)
				AddItem(items[i], obtain);
		}

		/// <summary>Adds the item to the list of all items in the game.</summary>
		public Item AddItem(Item item, bool obtain) {
			foreach (Item item2 in items) {
				if (item2.ID == item.ID) {
					if (obtain)
						ObtainItem(item2);
					return item2;
				}
			}
			items.Add(item);
			item.OnAdded(this);
			if (obtain)
				ObtainItem(item);
			return item;
		}

		/// <summary>Obtain the item with the given ID, adding it to the player's
		/// inventory.</summary>
		public void ObtainItem(string id) {
			ObtainItem(GetItem(id));
		}
		
		/// <summary>Obtain an item, adding it to the player's inventory.</summary>
		public void ObtainItem(Item item) {
			if (!item.IsObtained) {
				// Place the item into its appropriate inventory menu
				if (item is ItemWeapon)
					gameControl.MenuWeapons.AddToInventory((ItemWeapon) item);
				else if (item is ItemSecondary)
					gameControl.MenuSecondaryItems.AddItem((ItemSecondary) item);
				else if (item is ItemEssence)
					gameControl.MenuEssences.AddItem((ItemEssence) item);
				
				item.IsObtained = true;
				item.OnObtained();
			}
		}

		/// <summary>Unobtain an item, removing it from the player's inventory.
		/// </summary>
		public void UnobtainItem(Item item) {
			if (item.IsObtained) {
				// Remove the item from its inventory menu
				if (item is ItemWeapon)
					gameControl.MenuWeapons.RemoveFromInventory((ItemWeapon) item);
				else if (item is ItemSecondary)
					gameControl.MenuSecondaryItems.RemoveItem((ItemSecondary) item);
				else if (item is ItemEssence)
					gameControl.MenuEssences.RemoveItem((ItemEssence) item);
				
				item.IsObtained = false;
				item.OnUnobtained();
			}
		}


		//-----------------------------------------------------------------------------
		// Item Queries
		//-----------------------------------------------------------------------------

		/// <summary>Get the control for the given equip slot</summary>
		public InputControl GetSlotButton(int slot) {
			return (slot == 0 ? Controls.A : Controls.B);
		}

		/// <summary>Returns true if the given weapon is currently equipped in slot A
		/// or B.</summary>
		public bool IsWeaponEquipped(ItemWeapon weapon) {
			return (equippedWeapons[0] == weapon || equippedWeapons[1] == weapon);
		}

		/// <summary>Return true if the given weapon is currently equipped and its
		/// control button (A or B) is down.</summary>
		public bool IsWeaponButtonDown(ItemWeapon weapon) {
			return ((equippedWeapons[SLOT_A] == weapon && Controls.A.IsDown()) ||
				(equippedWeapons[SLOT_B] == weapon && Controls.B.IsDown()));
		}

		/// <summary>Returns an enumerable list of all items in the game.</summary>
		public IEnumerable<Item> GetItems() {
			foreach (Item item in items) {
				yield return item;
			}
		}

		/// <summary>Gets the item at the specified index.</summary>
		public Item GetItemByIndex(int index) {
			return items[index];
		}

		/// <summary>Gets the item with the specified ID.</summary>
		public Item GetItem(string id) {
			foreach (Item item in items) {
				if (item.ID == id)
					return item;
			}
			return null;
		}

		/// <summary>Gets the weapon with the specified ID.</summary>
		public ItemWeapon GetWeapon(string id) {
			foreach (Item item in items) {
				if (item.ID == id && item is ItemWeapon)
					return (ItemWeapon) item;
			}
			return null;
		}

		/// <summary>Checks if the item exists.</summary>
		public bool ItemExists(string id) {
			foreach (Item item in items) {
				if (item.ID == id)
					return true;
			}
			return false;
		}

		/// <summary>Checks if the item has been obtained.</summary>
		public bool IsItemObtained(string id) {
			foreach (Item item in items) {
				if (item.ID == id)
					return item.IsObtained;
			}
			return false;
		}

		/// <summary>Checks if the item has been obtained and is not stolen.</summary>
		public bool IsItemAvailable(string id) {
			foreach (Item item in items) {
				if (item.ID == id)
					return item.IsObtained && !item.IsStolen;
			}
			return false;
		}


		//-----------------------------------------------------------------------------
		// Ammo Management
		//-----------------------------------------------------------------------------
		
		/// <summary>Adds multiple ammo types to the list.</summary>
		public void AddAmmos(bool obtain, params Ammo[] ammos) {
			for (int i = 0; i < ammos.Length; i++)
				AddAmmo(ammos[i], obtain);
		}

		/// <summary>Adds the ammo type to the list.</summary>
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

		/// <summary>Gets the ammo class with the specified ID.</summary>
		public Ammo GetAmmo(string id) {
			foreach (Ammo ammo in this.ammo) {
				if (ammo.ID == id)
					return ammo;
			}
			return null;
		}

		/// <summary>Checks if the ammo exists.</summary>
		public bool AmmoExists(string id) {
			foreach (Ammo ammo in this.ammo) {
				if (ammo.ID == id)
					return true;
			}
			return false;
		}

		/// <summary>Checks if the ammo has been obtained.</summary>
		public bool IsAmmoObtained(string id) {
			foreach (Ammo ammo in this.ammo) {
				if (ammo.ID == id)
					return ammo.IsObtained;
			}
			return false;
		}

		/// <summary>Checks if the ammo has been obtained and is not stolen.</summary>
		public bool IsAmmoAvailable(string id) {
			foreach (Ammo ammo in this.ammo) {
				if (ammo.ID == id)
					return (ammo.IsObtained && !ammo.IsStolen);
			}
			return false;
		}

		/// <summary>Fills all the ammo in the player's inventory.</summary>
		public void FillAllAmmo() {
			foreach (Ammo ammo in this.ammo) {
				ammo.Amount = ammo.MaxAmount;
				// Prevent rupee spamming sound
				if (ammo.ID == "rupees")
					gameControl.HUD.DynamicRupees = ammo.Amount;
			}
		}

		/// <summary>Empties all the ammo from the player's inventory.</summary>
		public void EmptyAllAmmo() {
			foreach (Ammo ammo in this.ammo) {
				ammo.Amount = 0;
				// Prevent rupee spamming sound
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

		/// <summary>Gets the equipped weapons list.</summary>
		public ItemWeapon[] EquippedWeapons {
			get { return equippedWeapons; }
		}

		/// <summary>Gets if a two handed weapon is equipped.</summary>
		public bool IsTwoHandedEquipped {
			get {
				return (equippedWeapons[0] != null && equippedWeapons[0].IsTwoHanded);
			}
		}

		/// <summary>Number of pieces of heart between 0 and 3.</summary>
		public int PiecesOfHeart {
			get { return piecesOfHeart; }
			set { piecesOfHeart = value; }
		}

		/// <summary>Reference to the current game control.</summary>
		public GameControl GameControl {
			get { return gameControl; }
		}
	}
}
