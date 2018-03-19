using System.Collections.Generic;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Input;
using ZeldaOracle.Game.Control;
using ZeldaOracle.Game.Main;

namespace ZeldaOracle.Game.Items {
	/// <summary>The manager and container for all available items in the game.</summary>
	public class Inventory {

		/// <summary>The game control for the current game session.</summary>
		private GameControl gameControl;
		/// <summary>The collection of all items in the game.</summary>
		private Dictionary<string, Item> items;
		/// <summary>The collection of all ammos in the game.</summary>
		private Dictionary<string, Ammo> ammos;
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

		/// <summary>Constructs the in-editor inventory manager.</summary>
		public Inventory() {
			gameControl		= null;
			items			= new Dictionary<string, Item>();
			ammos			= new Dictionary<string, Ammo>();
			equippedWeapons	= new ItemWeapon[NumEquipSlots];
			piecesOfHeart	= 0;
		}

		/// <summary>Constructs the in-game inventory manager.</summary>
		public Inventory(GameControl gameControl) : this() {
			this.gameControl = gameControl;
		}


		//-----------------------------------------------------------------------------
		// Resources
		//-----------------------------------------------------------------------------

		/// <summary>Initializes all items and ammo by loading them from resources.</summary>
		public void Initialize() {
			foreach (var pair in Resources.GetDictionary<AmmoData>()) {
				Ammo ammo = Ammo.CreateAmmo(pair.Value);
				AddAmmo(ammo, false);
			}
			foreach (var pair in Resources.GetDictionary<ItemData>()) {
				Item item = Item.CreateItem(pair.Value);
				AddItem(item, false);
			}
		}


		//-----------------------------------------------------------------------------
		// Item Mutators
		//-----------------------------------------------------------------------------

		/// <summary>Equips a non-usable item.</summary>
		public void EquipItem(Item item) {
			ItemEquipment equippableItem = item as ItemEquipment;
			if (equippableItem != null)
				equippableItem.Equip();
		}

		/// <summary>Equips a non-usable item.</summary>
		public void EquipItem(string id) {
			ItemEquipment equippableItem = GetItem(id) as ItemEquipment;
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
			items.Add(item.ID, item);

			item.Initialize(this);
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
			}
		}

		/// <summary>Sets the level of the item with the specified ID.</summary>
		public void SetLevel(string id, int level) {
			Item item = GetItem(id);
			if (item != null)
				item.Level = level;
		}

		/// <summary>Sets the level of the item with the specified ID to the max level.</summary>
		public void SetMaxLevel(string id) {
			Item item = GetItem(id);
			if (item != null)
				item.Level = item.MaxLevel;
		}


		//-----------------------------------------------------------------------------
		// Weapon Accessors
		//-----------------------------------------------------------------------------

		/// <summary>Get the control for the given equip slot</summary>
		public InputControl GetSlotButton(int slot) {
			return (slot == 0 ? Controls.A : Controls.B);
		}

		/// <summary>Returns true if the weapon with the given ID is currently equipped
		/// in slot A or B.</summary>
		public bool IsWeaponEquipped(string id) {
			return ((equippedWeapons[0] != null && equippedWeapons[0].ID == id) ||
					(equippedWeapons[1] != null && equippedWeapons[1].ID == id));
		}

		/// <summary>Returns true if the given weapon is currently equipped in slot A
		/// or B.</summary>
		public bool IsWeaponEquipped(ItemWeapon weapon) {
			return (equippedWeapons[0] == weapon || equippedWeapons[1] == weapon);
		}

		/// <summary>Return true if the weapon with the given ID is currently equipped
		/// and its control button (A or B) is down.</summary>
		public bool IsWeaponButtonDown(string id) {
			return ((equippedWeapons[SLOT_A] != null  &&
					 equippedWeapons[SLOT_A].ID == id && Controls.A.IsDown()) ||
					(equippedWeapons[SLOT_B] != null  &&
					 equippedWeapons[SLOT_B].ID == id && Controls.B.IsDown()));
		}

		/// <summary>Return true if the given weapon is currently equipped and its
		/// control button (A or B) is down.</summary>
		public bool IsWeaponButtonDown(ItemWeapon weapon) {
			return ((equippedWeapons[SLOT_A] == weapon && Controls.A.IsDown()) ||
					(equippedWeapons[SLOT_B] == weapon && Controls.B.IsDown()));
		}

		/// <summary>Gets the weapon with the specified ID.</summary>
		public ItemWeapon GetWeapon(string id) {
			return GetItem(id) as ItemWeapon;
		}


		//-----------------------------------------------------------------------------
		// Item Accessors
		//-----------------------------------------------------------------------------

		/// <summary>Returns an enumerable list of all items in the game.</summary>
		public IEnumerable<Item> GetItems() {
			foreach (var pair in items) {
				yield return pair.Value;
			}
		}

		/// <summary>Gets the item with the specified ID.</summary>
		public Item GetItem(string id) {
			Item item;
			items.TryGetValue(id, out item);
			return item;
		}

		/// <summary>Checks if the item exists.</summary>
		public bool ContainsItem(string id) {
			return items.ContainsKey(id);
		}

		/// <summary>Checks if the item has been obtained.</summary>
		public bool IsItemObtained(string id) {
			Item item = GetItem(id);
			if (item != null)
				return item.IsObtained;
			return false;
		}

		/// <summary>Checks if the item has been obtained and is not lost.</summary>
		public bool IsItemAvailable(string id) {
			Item item = GetItem(id);
			if (item != null)
				return item.IsObtained && !item.IsLost;
			return false;
		}


		//-----------------------------------------------------------------------------
		// Ammo Mutators
		//-----------------------------------------------------------------------------

		/// <summary>Adds multiple ammo types to the list.</summary>
		public void AddAmmos(bool obtain, params Ammo[] ammos) {
			for (int i = 0; i < ammos.Length; i++)
				AddAmmo(ammos[i], obtain);
		}

		/// <summary>Adds the ammo type to the list.</summary>
		public Ammo AddAmmo(Ammo ammo, bool obtain) {
			ammos.Add(ammo.ID, ammo);
			if (obtain)
				ObtainAmmo(ammo);
			return ammo;
		}

		/// <summary>Fills all the ammo in the player's inventory.</summary>
		public void FillAllAmmo() {
			foreach (Ammo ammo in ammos.Values) {
				ammo.Amount = ammo.MaxAmount;
				// Prevent rupee spamming sound
				if (ammo.ID == "rupees")
					gameControl.HUD.DynamicRupees = ammo.Amount;
			}
		}

		/// <summary>Empties all the ammo from the player's inventory.</summary>
		public void EmptyAllAmmo() {
			foreach (Ammo ammo in ammos.Values) {
				ammo.Amount = 0;
				// Prevent rupee spamming sound
				if (ammo.ID == "rupees")
					gameControl.HUD.DynamicRupees = ammo.Amount;
			}
		}

		/// <summary>Obtain the ammo with the given ID, adding it to the player's
		/// inventory.</summary>
		public void ObtainAmmo(string id) {
			ObtainAmmo(GetAmmo(id));
		}

		/// <summary>Obtain an ammo, adding it to the player's inventory.</summary>
		public void ObtainAmmo(Ammo ammo) {
			if (!ammo.IsObtained) {
				ammo.IsObtained = true;
			}
		}


		//-----------------------------------------------------------------------------
		// Ammo Accessors
		//-----------------------------------------------------------------------------
		
		/// <summary>Gets the ammo class with the specified ID.</summary>
		public Ammo GetAmmo(string id) {
			Ammo ammo;
			ammos.TryGetValue(id, out ammo);
			return ammo;
		}

		/// <summary>Gets the collection of ammos in the game.</summary>
		public IEnumerable<Ammo> GetAmmos() {
			foreach (var pair in ammos) {
				yield return pair.Value;
			}
		}

		/// <summary>Checks if the ammo exists.</summary>
		public bool ContainsAmmo(string id) {
			return ammos.ContainsKey(id);
		}

		/// <summary>Checks if the ammo has been obtained.</summary>
		public bool IsAmmoObtained(string id) {
			Ammo ammo = GetAmmo(id);
			if (ammo != null)
				return ammo.IsObtained;
			return false;
		}

		/// <summary>Checks if the ammo has been obtained and is not lost.</summary>
		public bool IsAmmoAvailable(string id) {
			Ammo ammo = GetAmmo(id);
			if (ammo != null)
				return ammo.IsObtained && !ammo.IsLost;
			return false;
		}

		/// <summary>Checks if the ammos container is available and thus it can be
		/// picked up. Also returns true if the ammo does not need a container.</summary>
		public bool IsAmmoContainerAvailable(string id) {
			Ammo ammo = GetAmmo(id);
			if (ammo != null)
				return ammo.IsContainerAvailable;
			return false;
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
