using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.GameStates;
using ZeldaOracle.Game.GameStates.Transitions;
using ZeldaOracle.Game.Items;
using ZeldaOracle.Game.Main;

namespace ZeldaOracle.Game.Control.Menus {

	public class MenuWeapons : InventoryMenu {

		private int ammoSlot;
		private Point2I ammoMenuSize;
		private SlotGroup ammoSlotGroup;

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public MenuWeapons(GameManager gameManager)
			: base(gameManager) {
			this.backgroundSprite	= Resources.GetImage("UI/menu_weapons_a");

			this.ammoSlot			= 0;
			this.ammoMenuSize		= new Point2I(16, 8);
			this.ammoSlotGroup		= null;

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
						slots[x, y].SetConnection(Directions.Left, slots[gridSize.X - 1, (y + gridSize.Y - 1) % gridSize.Y]);
					else
						slots[x, y].SetConnection(Directions.Left, slots[x - 1, y]);

					if (x == gridSize.X - 1)
						slots[x, y].SetConnection(Directions.Right, slots[0, (y + 1) % gridSize.Y]);
					else
						slots[x, y].SetConnection(Directions.Right, slots[x + 1, y]);

					slots[x, y].SetConnection(Directions.Up, slots[x, (y + gridSize.Y - 1) % gridSize.Y]);
					slots[x, y].SetConnection(Directions.Down, slots[x, (y + 1) % gridSize.Y]);
				}
			}
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void Update() {
			base.Update();

			if (!inSubMenu) {
				// Equip weapons.
				if (Controls.A.IsPressed() || Controls.B.IsPressed()) {
					int slot = (Controls.A.IsPressed() ? 0 : 1);
					ItemWeapon selectedItem = slotGroups[0].CurrentSlot.SlotItem as ItemWeapon;
					if (selectedItem != null && selectedItem.NumAmmos > 1) {
						ammoMenuSize = new Point2I(16, 8);
						ammoSlot = slot;
						inSubMenu = true;
						currentSlotGroup = null;
						ammoSlotGroup = new SlotGroup();
						List<Ammo> ammo = new List<Ammo>();
						for (int i = 0; i < selectedItem.NumAmmos; i++) {
							if (selectedItem.GetAmmoAt(i).IsObtained)
								ammo.Add(selectedItem.GetAmmoAt(i));
						}

						int start = (160 - (ammo.Count * 24 + 8)) / 2 + 8;
						int y = (DrawAmmoMenuAtTop ? 20 : 60);
						for (int i = 0; i < ammo.Count; i++) {
							ammoSlotGroup.AddSlot(new Point2I(start + 24 * i, y), 16);
							ammoSlotGroup.GetSlotAt(i).SlotItem = ammo[i];
						}
						for (int i = 0; i < ammo.Count; i++) {
							ammoSlotGroup.GetSlotAt(i).SetConnection(Directions.Left, ammoSlotGroup.GetSlotAt((i + ammo.Count - 1) % ammo.Count));
							ammoSlotGroup.GetSlotAt(i).SetConnection(Directions.Right, ammoSlotGroup.GetSlotAt((i + 1) % ammo.Count));
						}
						ammoSlotGroup.SetCurrentSlot(ammoSlotGroup.GetSlotAt(selectedItem.CurrentAmmo));
					}
					else {
						EquipWeapon(slot);
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
					EquipWeapon(ammoSlot);
					inSubMenu = false;
					ammoSlotGroup = null;
					currentSlotGroup = slotGroups[0];
				}
			}
		}

		public override void Draw(Graphics2D g) {
			base.Draw(g);
			if (inSubMenu) {
				Point2I maxSize = new Point2I(ammoSlotGroup.NumSlots * 24 + 8, 32);
				Point2I menuPos = new Point2I((160 - (ammoSlotGroup.NumSlots * 24 + 8)) / 2 + (maxSize.X - ammoMenuSize.X) / 2, DrawAmmoMenuAtTop ? 16 : 56);
				Sprite ammoMenuSprite = new Sprite(GameData.SHEET_MENU_SMALL_LIGHT, new Point2I(1, 4));
				g.DrawSprite(ammoMenuSprite, new Rectangle2I(menuPos, ammoMenuSize));
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
				Sprite arrowSprite = new Sprite(GameData.SHEET_MENU_SMALL_LIGHT, new Point2I(5, 5));
				g.DrawSprite(arrowSprite, slot.Position + new Point2I(4, 20));
			}
		}

		private void EquipWeapon(int slot) {
			ItemWeapon weapon = slotGroups[0].CurrentSlot.SlotItem as ItemWeapon;
			AudioSystem.PlaySound("UI/menu_select");
			if (GameControl.Inventory.EquippedWeapons[slot] != null) {
				if (weapon != null && weapon.HasFlag(ItemFlags.TwoHanded)) {
					NextAvailableSlot.SlotItem = GameControl.Inventory.EquippedWeapons[1 - slot];
				}
				ItemWeapon placeholder = GameControl.Inventory.EquippedWeapons[slot];
				GameControl.Inventory.EquipWeapon(weapon, slot);
				slotGroups[0].CurrentSlot.SlotItem = placeholder;
			}
			else {
				GameControl.Inventory.EquipWeapon(weapon, slot);
				slotGroups[0].CurrentSlot.SlotItem = null;
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
