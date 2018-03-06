using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Game.GameStates;
using ZeldaOracle.Game.GameStates.Transitions;
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
			//this.backgroundSprite	= Resources.GetImage("UI/menu_weapons_a");
			this.background = GameData.SPR_BACKGROUND_MENU_WEAPONS;

			this.ammoSlot			= 0;
			this.ammoMenuSize		= new Point2I(16, 8);
			this.ammoSlotGroup		= null;
			this.oldEquippedWeapons		= new ItemWeapon[2];

			SlotGroup group = new SlotGroup();
			currentSlotGroup = group;
			this.slotGroups.Add(group);
			Slot[,] slots = new Slot[4, 4];

			Point2I gridSize = new Point2I(4, 4);

			for (int y = 0; y < gridSize.Y; y++) {
				for (int x = 0; x < gridSize.X; x++) {
					slots[x, y] = group.AddSlot(new Point2I(24 + 32 * x, 8 + 24 * y), 24);
				}
			}
			for (int y = 0; y < gridSize.Y; y++) {
				for (int x = 0; x < gridSize.X; x++) {
					if (x == 0)
						slots[x, y].SetConnection(Direction.Left, slots[gridSize.X - 1, (y + gridSize.Y - 1) % gridSize.Y]);
					else
						slots[x, y].SetConnection(Direction.Left, slots[x - 1, y]);

					if (x == gridSize.X - 1)
						slots[x, y].SetConnection(Direction.Right, slots[0, (y + 1) % gridSize.Y]);
					else
						slots[x, y].SetConnection(Direction.Right, slots[x + 1, y]);

					slots[x, y].SetConnection(Direction.Up, slots[x, (y + gridSize.Y - 1) % gridSize.Y]);
					slots[x, y].SetConnection(Direction.Down, slots[x, (y + 1) % gridSize.Y]);
				}
			}
		}


		//-----------------------------------------------------------------------------
		// Weapon Equipping
		//-----------------------------------------------------------------------------

		/// <summary>Equip the given weapon for the desired equip slot (A or B). This
		/// will first find the item in the inventory, then swap it with the item
		/// currently equipped in the slot.</summary>
		public void EquipWeapon(ItemWeapon equippedWeapon, int equipSlot) {
			int otherEquipSlot = 1 - equipSlot;

			// Do nothing if this weapon type is already equipped in the correct slot
			if (GameControl.Inventory.EquippedWeapons[equipSlot] == equippedWeapon)
				return;

			// If the weapon is equipped but in the wrong slot, then swap the slots of
			// the two equipped weapons
			if (GameControl.Inventory.EquippedWeapons[otherEquipSlot] ==
				equippedWeapon)
			{
				ItemWeapon placeholder =
					GameControl.Inventory.EquippedWeapons[equipSlot];
				GameControl.Inventory.EquippedWeapons[equipSlot] = 
					GameControl.Inventory.EquippedWeapons[otherEquipSlot];
				GameControl.Inventory.EquippedWeapons[otherEquipSlot] = placeholder;
			}

			// Find the inventory slot which contains the weapon of this type
			Slot swapSlot = null;
			SlotGroup slotGroup = slotGroups[0];
			for (int i = 0; i < slotGroup.NumSlots; i++) {
				if (slotGroup[i].SlotItem == equippedWeapon)
					swapSlot = slotGroup[i];
			}
			if (swapSlot == null)
				return;

			// Swap the weapon between the equip slot and the inventory slot
			//ItemWeapon equippedWeapon = (ItemWeapon) swapSlot.SlotItem;
			ItemWeapon unequippedWeapon = GameControl.Inventory.EquippedWeapons[equipSlot];
			swapSlot.SlotItem = unequippedWeapon;
			GameControl.Inventory.EquippedWeapons[equipSlot] = equippedWeapon;
			unequippedWeapon.Unequip();

			// Handle unequipping two handed weapons by nullifying the other slot
			if (unequippedWeapon.IsTwoHanded)
				GameControl.Inventory.EquippedWeapons[otherEquipSlot] = null;

			// Handle equipping two handed weapons by replacing the other equip slot
			if (equippedWeapon.IsTwoHanded) {
				if (GameControl.Inventory.EquippedWeapons[1 - equipSlot] != null)
					NextAvailableSlot.SlotItem =
						GameControl.Inventory.EquippedWeapons[otherEquipSlot];
				GameControl.Inventory.EquippedWeapons[1 - equipSlot] = equippedWeapon;
			}

			unequippedWeapon.Equip(equipSlot);
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

					if (selectedItem != null && selectedItem.NumAmmos > 1) {
						ammoMenuSize		= new Point2I(16, 8);
						ammoSlot			= slot;
						inSubMenu			= true;
						currentSlotGroup	= null;
						ammoSlotGroup		= new SlotGroup();
						List<Ammo> ammo = new List<Ammo>();

						int currentAmmoIndex = 0;

						for (int i = 0; i < selectedItem.NumAmmos; i++) {
							if (selectedItem.GetAmmoAt(i).IsObtained) {
								if (selectedItem.CurrentAmmo == i)
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
						EquipWeaponFromCursor(slot);
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
					for (int i = 0; i < weapon.NumAmmos; i++) {
						if (weapon.GetAmmoAt(i) == selectedAmmo)
							weapon.CurrentAmmo = i;
					}
					EquipWeaponFromCursor(ammoSlot);
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

		private void EquipWeaponFromCursor(int slot) {
			ItemWeapon weapon = slotGroups[0].CurrentSlot.SlotItem as ItemWeapon;
			AudioSystem.PlaySound(GameData.SOUND_MENU_SELECT);
			
			if (GameControl.Inventory.EquippedWeapons[slot] != null) {
				if (GameControl.Inventory.EquippedWeapons[slot].IsTwoHanded)
					GameControl.Inventory.EquippedWeapons[1 - slot] = null;

				ItemWeapon placeholder = GameControl.Inventory.EquippedWeapons[slot];
				GameControl.Inventory.EquippedWeapons[slot] = weapon;
				slotGroups[0].CurrentSlot.SlotItem = placeholder;
				if (placeholder != null)
					placeholder.Unequip();
				if (weapon != null)
					weapon.Equip(slot);
			}
			else {
				GameControl.Inventory.EquippedWeapons[slot] = weapon;
				slotGroups[0].CurrentSlot.SlotItem = null;
				if (weapon != null)
					weapon.Equip(slot);
			}

			if (weapon != null && weapon.IsTwoHanded) {
				if (GameControl.Inventory.EquippedWeapons[1 - slot] != null)
					NextAvailableSlot.SlotItem = GameControl.Inventory.EquippedWeapons[1 - slot];
				GameControl.Inventory.EquippedWeapons[1 - slot] = weapon;
			}
			
			ResetDescription();
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public Slot NextAvailableSlot {
			get {
				for (int i = 0; i < slotGroups[0].NumSlots; i++) {
					if (slotGroups[0].GetSlotAt(i).SlotItem == null)
						return slotGroups[0].GetSlotAt(i);
				}
				return null;
			}
		}

		private bool IsAmmoMenuFullyOpen {
			get { return (ammoMenuSize.X == (ammoSlotGroup.NumSlots * 24 + 8) && ammoMenuSize.Y == 32); }
		}

		private bool DrawAmmoMenuAtTop {
			get { return (slotGroups[0].CurrentSlotIndex >= 8); }
		}
	}
}
