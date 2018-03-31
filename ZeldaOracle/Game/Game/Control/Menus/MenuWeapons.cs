using System;
using System.Collections.Generic;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Items;
using ZeldaOracle.Game.Main;

namespace ZeldaOracle.Game.Control.Menus {

	public class MenuWeapons : InventoryMenu {

		private int ammoSlot;
		private Point2I ammoMenuSize;
		private SlotGroup ammoSlotGroup;
		private ItemWeapon[] oldEquippedWeapons;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public MenuWeapons(GameManager gameManager)
			: base(gameManager)
		{
			background = GameData.SPR_BACKGROUND_MENU_WEAPONS;

			ammoSlot			= 0;
			ammoMenuSize		= new Point2I(16, 8);
			ammoSlotGroup		= null;
			oldEquippedWeapons	= new ItemWeapon[2];

			SlotGroup group = new SlotGroup();
			currentSlotGroup = group;
			slotGroups.Add(group);
			Slot[,] slots = new Slot[4, 4];
			Point2I gridSize = new Point2I(4, 4);

			// Create the slot grid
			for (int y = 0; y < gridSize.Y; y++) {
				for (int x = 0; x < gridSize.X; x++) {
					slots[x, y] = group.AddSlot(
						new Point2I(24 + 32 * x, 8 + 24 * y), 24);
				}
			}

			// Setep the slot connections
			for (int y = 0; y < gridSize.Y; y++) {
				for (int x = 0; x < gridSize.X; x++) {
					// Horizontal connections will wrap to the next/previous rows
					if (x == 0)
						slots[x, y].SetConnection(Direction.Left,
							slots[gridSize.X - 1, (y + gridSize.Y - 1) % gridSize.Y]);
					else
						slots[x, y].SetConnection(Direction.Left, slots[x - 1, y]);
					if (x == gridSize.X - 1)
						slots[x, y].SetConnection(Direction.Right,
							slots[0, (y + 1) % gridSize.Y]);
					else
						slots[x, y].SetConnection(Direction.Right, slots[x + 1, y]);

					// Vertical connections wrap around
					slots[x, y].SetConnection(Direction.Up,
						slots[x, (y + gridSize.Y - 1) % gridSize.Y]);
					slots[x, y].SetConnection(Direction.Down,
						slots[x, (y + 1) % gridSize.Y]);
				}
			}
		}

		
		//-----------------------------------------------------------------------------
		// Item Management
		//-----------------------------------------------------------------------------

		public bool AddToInventory(ItemWeapon weapon) {
			if (weapon.IsTwoHanded &&
				GameControl.Inventory.EquippedWeapons[Inventory.SLOT_A] == null &&
				GameControl.Inventory.EquippedWeapons[Inventory.SLOT_A] == null)
			{
				GameControl.Inventory.EquippedWeapons[Inventory.SLOT_A] = weapon;
				GameControl.Inventory.EquippedWeapons[Inventory.SLOT_B] = weapon;
				weapon.Equip(Inventory.SLOT_A);
				return true;
			}
			else if (!weapon.IsTwoHanded &&
				GameControl.Inventory.EquippedWeapons[Inventory.SLOT_A] == null)
			{
				GameControl.Inventory.EquippedWeapons[Inventory.SLOT_A] = weapon;
				weapon.Equip(Inventory.SLOT_A);
				return true;
			}
			else if (!weapon.IsTwoHanded &&
				GameControl.Inventory.EquippedWeapons[Inventory.SLOT_B] == null)
			{
				GameControl.Inventory.EquippedWeapons[Inventory.SLOT_B] = weapon;
				weapon.Equip(Inventory.SLOT_B);
				return true;
			}
			else {
				// Find an empty slot in the inventory grid
				Slot emptySlot = NextAvailableSlot;
				if (emptySlot != null) {
					emptySlot.SlotItem = weapon;
					return true;
				}
				else {
					// No room for the item
					return false;
				}
			}
		}

		public void RemoveFromInventory(ItemWeapon weapon) {
			// Clear all slots containing the weapon
			if (GameControl.Inventory.EquippedWeapons[Inventory.SLOT_A] == weapon)
				GameControl.Inventory.EquippedWeapons[Inventory.SLOT_A] = null;
			if (GameControl.Inventory.EquippedWeapons[Inventory.SLOT_B] == weapon)
				GameControl.Inventory.EquippedWeapons[Inventory.SLOT_B] = null;
			SlotGroup slots = slotGroups[0];
			for (int i = 0; i < slots.NumSlots; i++) {
				if (slots[i].SlotItem == weapon)
					slots[i].SlotItem = null;
			}
			weapon.Unequip();
		}


		//-----------------------------------------------------------------------------
		// Weapon Equipping
		//-----------------------------------------------------------------------------
		
		/// <summary>Equip the given weapon for the desired equip slot (A or B). This
		/// will first find the item in the inventory, then swap it with the item
		/// currently equipped in the slot.</summary>
		public bool EquipWeapon(ItemWeapon equippedWeapon, int equipSlot) {
			// Do nothing if this weapon type is already equipped in the correct slot
			if (GameControl.Inventory.EquippedWeapons[equipSlot] == equippedWeapon)
				return true;

			// If the weapon is equipped but in the wrong slot, then simply swap the
			// slots of the two equipped weapons
			if (GameControl.Inventory.EquippedWeapons[1 - equipSlot] ==
				equippedWeapon)
			{
				SwapEquippedWeaponSlots();
				return true;
			}

			// Find the inventory slot which contains this weapon
			Slot inventorySlot = null;
			SlotGroup slotGroup = slotGroups[0];
			for (int i = 0; i < slotGroup.NumSlots; i++) {
				if (slotGroup[i].SlotItem == equippedWeapon)
					inventorySlot = slotGroup[i];
			}
			if (inventorySlot == null)
				return false;

			// Equip the weapon from the slot
			return EquipWeaponFromSlot(inventorySlot, equipSlot);
		}

		/// <summary>Equip the weapon in an inventory slot for the desired equip slot
		/// (A or B). This will swap it with the item currently equipped in the slot.
		/// </summary>
		private bool EquipWeaponFromSlot(Slot swapSlot, int equipSlot) {
			int otherEquipSlot = 1 - equipSlot;
			ItemWeapon weaponToEquip = swapSlot.SlotItem as ItemWeapon;
			ItemWeapon[] equippedWeapons = GameControl.Inventory.EquippedWeapons;
			
			// Get a list of currently equipped weapons that we'll need to unequip
			List<ItemWeapon> weaponsToUnequip = new List<ItemWeapon>();
			if (equippedWeapons[equipSlot] != null)
				weaponsToUnequip.Add(equippedWeapons[equipSlot]);
			if (weaponToEquip != null && weaponToEquip.IsTwoHanded &&
				equippedWeapons[otherEquipSlot] != null &&
				!equippedWeapons[otherEquipSlot].IsTwoHanded)
				weaponsToUnequip.Add(equippedWeapons[otherEquipSlot]);

			// If we need to unequip two weapons, then find an empty slot to put the
			// second weapon in. If there are none, then cancel the equip action.
			// Note that if the inventory is completely full, then two-handed weapons
			// can never be equipped.
			if (weaponsToUnequip.Count == 2) {
				Slot emptySlot = NextAvailableSlot;
				if (emptySlot != null) {
					emptySlot.SlotItem = weaponsToUnequip[1];
				equippedWeapons[otherEquipSlot] = null;
				}
				else {
					swapSlot.SlotItem = weaponToEquip;
					return false;
				}
			}

			// Unequip the first weapon
			if (weaponsToUnequip.Count >= 1) {
				swapSlot.SlotItem = weaponsToUnequip[0];
				if (weaponsToUnequip[0].IsTwoHanded)
					equippedWeapons[otherEquipSlot] = null;
			}
			else
				swapSlot.SlotItem = null;

			// Equip the weapon from the slot
			equippedWeapons[equipSlot] = weaponToEquip;
			if (weaponToEquip != null && weaponToEquip.IsTwoHanded)
				equippedWeapons[otherEquipSlot] = weaponToEquip;
			
			// Invoke the ItemEquipment callbacks
			for (int i = 0; i < weaponsToUnequip.Count; i++)
				weaponsToUnequip[i].Unequip();
			weaponToEquip?.Equip(equipSlot);
			return true;
		}

		/// <summary>Swap equip slots of the two equipped weapons.</summary>
		private void SwapEquippedWeaponSlots() {
			ItemWeapon placeHolder = GameControl.Inventory.EquippedWeapons[0];
			GameControl.Inventory.EquippedWeapons[0] = 
				GameControl.Inventory.EquippedWeapons[1];
			GameControl.Inventory.EquippedWeapons[1] = placeHolder;
			GameControl.Inventory.EquippedWeapons[0].CurrentEquipSlot = 0;
			GameControl.Inventory.EquippedWeapons[1].CurrentEquipSlot = 1;
		}

		/// <summary>Equip the weapon in the currently selected inventory slot.
		/// </summary>
		private bool EquipWeaponFromCursor(int equipSlot) {
			bool success = EquipWeaponFromSlot(
				slotGroups[0].CurrentSlot, equipSlot);
			ResetDescription();
			return success;
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------
		
		public override void OnOpen() {
			// Remember the equipped items from before opening the menu.
			for (int i = 0; i < Inventory.NumEquipSlots; i++)
				oldEquippedWeapons[i] = GameControl.Inventory.EquippedWeapons[i];
		}

		public override void OnClose() {
			// Unequp old weapons.
			for (int i = 0; i < Inventory.NumEquipSlots; i++) {
				ItemWeapon weapon = oldEquippedWeapons[i];
				if (weapon != null && !GameControl.Inventory.IsWeaponEquipped(weapon)) {
					weapon.Unequip();
					if (weapon.IsTwoHanded)
						break;
				}
			}

			// Equip new weapons.
			for (int i = 0; i < Inventory.NumEquipSlots; i++) {
				ItemWeapon weapon = GameControl.Inventory.EquippedWeapons[i];
				if (weapon != null) {
					weapon.Equip(i);
					if (weapon.IsTwoHanded)
						break;
				}
			}
		}

		public override void Update() {
			base.Update();

			if (!inSubMenu) {
				// Equip weapons.
				if (Controls.A.IsPressed() || Controls.B.IsPressed()) {
					int slot = (Controls.A.IsPressed() ? 0 : 1);
					ItemWeapon selectedItem = slotGroups[0].CurrentSlot.SlotItem as ItemWeapon;

					if (selectedItem != null && selectedItem.AmmoCount > 1) {
						ammoMenuSize		= new Point2I(16, 8);
						ammoSlot			= slot;
						inSubMenu			= true;
						currentSlotGroup	= null;
						ammoSlotGroup		= new SlotGroup();
						List<Ammo> ammo = new List<Ammo>();

						int currentAmmoIndex = 0;

						for (int i = 0; i < selectedItem.AmmoCount; i++) {
							if (selectedItem.GetAmmoAt(i).IsObtained) {
								if (selectedItem.AmmoIndex == i)
									currentAmmoIndex = ammo.Count;
								ammo.Add(selectedItem.GetAmmoAt(i));
							}
						}

						int start = (160 - (ammo.Count * 24 + 8)) / 2 + 8;
						int y = (DrawAmmoMenuAtTop ? 20 : 60);
						for (int i = 0; i < ammo.Count; i++) {
							ammoSlotGroup.AddSlot(new Point2I(start + 24 * i, y), 16);
							ammoSlotGroup.GetSlotAt(i).SlotItem = ammo[i];
						}
						for (int i = 0; i < ammo.Count; i++) {
							ammoSlotGroup.GetSlotAt(i).SetConnection(Direction.Left, ammoSlotGroup.GetSlotAt((i + ammo.Count - 1) % ammo.Count));
							ammoSlotGroup.GetSlotAt(i).SetConnection(Direction.Right, ammoSlotGroup.GetSlotAt((i + 1) % ammo.Count));
						}
						ammoSlotGroup.SetCurrentSlot(ammoSlotGroup.GetSlotAt(currentAmmoIndex));
					}
					else {
						AudioSystem.PlaySound(GameData.SOUND_MENU_SELECT);
						EquipWeaponFromCursor(slot);
						ResetDescription();
					}
				}
			}
			else {
				Point2I maxSize = new Point2I(ammoSlotGroup.NumSlots * 24 + 8, 32);

				if (ammoMenuSize.X < maxSize.X) {
					ammoMenuSize.X += 8;
				}
				else if (ammoMenuSize.Y < maxSize.Y) {
					ammoMenuSize.Y += 4;
					if (ammoMenuSize.Y == maxSize.Y) {
						currentSlotGroup = ammoSlotGroup;
						ResetDescription();
					}
				}
				else if (Controls.A.IsPressed() || Controls.B.IsPressed() || Controls.Start.IsPressed()) {
					Ammo selectedAmmo = ammoSlotGroup.CurrentSlot.SlotItem as Ammo;
					ItemWeapon weapon = slotGroups[0].CurrentSlot.SlotItem as ItemWeapon;
					for (int i = 0; i < weapon.AmmoCount; i++) {
						if (weapon.GetAmmoAt(i) == selectedAmmo)
							weapon.AmmoIndex = i;
					}
					AudioSystem.PlaySound(GameData.SOUND_MENU_SELECT);
					EquipWeaponFromCursor(ammoSlot);
					ResetDescription();
					inSubMenu = false;
					ammoSlotGroup = null;
					currentSlotGroup = slotGroups[0];
				}
			}
		}

		protected override void DrawMenu(Graphics2D g) {
			base.DrawMenu(g);
			if (inSubMenu) {
				Point2I maxSize = new Point2I(ammoSlotGroup.NumSlots * 24 + 8, 32);
				Point2I menuPos = new Point2I((160 - (ammoSlotGroup.NumSlots * 24 + 8)) / 2 + (maxSize.X - ammoMenuSize.X) / 2, DrawAmmoMenuAtTop ? 16 : 56);
				//ISprite ammoMenuSprite = GameData.SHEET_MENU_SMALL_LIGHT.GetSprite(1, 4);
				g.DrawSprite(GameData.SPR_HUD_AMMO_SELECT_BACKGROUND, new Rectangle2I(menuPos, ammoMenuSize));
				if (IsAmmoMenuFullyOpen) {
					ammoSlotGroup.Draw(g);
				}
				if (currentSlotGroup != null) {
					DrawSlotCursor(g, currentSlotGroup.CurrentSlot);
				}
			}
		}

		public override void DrawSlotCursor(Graphics2D g, Slot slot) {
			if (!inSubMenu) {
				base.DrawSlotCursor(g, slot);
			}
			else if (IsAmmoMenuFullyOpen) {
				//ISprite arrowSprite = GameData.SHEET_MENU_SMALL_LIGHT.GetSprite(5, 5);
				g.DrawSprite(GameData.SPR_HUD_AMMO_SELECT_ARROW, slot.Position + new Point2I(4, 20));
			}
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		private Slot NextAvailableSlot {
			get {
				for (int i = 0; i < slotGroups[0].NumSlots; i++) {
					if (slotGroups[0].GetSlotAt(i).SlotItem == null)
						return slotGroups[0].GetSlotAt(i);
				}
				return null;
			}
		}

		private bool IsAmmoMenuFullyOpen {
			get {
				return (ammoMenuSize.X == (ammoSlotGroup.NumSlots * 24 + 8) &&
					ammoMenuSize.Y == 32);
			}
		}

		private bool DrawAmmoMenuAtTop {
			get { return (slotGroups[0].CurrentSlotIndex >= 8); }
		}
	}
}
