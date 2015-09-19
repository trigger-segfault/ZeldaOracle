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

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public MenuWeapons(GameManager gameManager)
			: base(gameManager) {
			this.backgroundSprite = Resources.GetImage("UI/menu_weapons_a");

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

			// Equip weapons.
			if (Controls.A.IsPressed() || Controls.B.IsPressed()) {
				int slot = (Controls.A.IsPressed() ? 0 : 1);
				AudioSystem.PlaySound("UI/menu_select");
				UsableItem selectedItem = currentSlotGroup.CurrentSlot.SlotItem as UsableItem;
				if (GameControl.Inventory.EquippedUsableItems[slot] != null) {
					UsableItem placeholder = GameControl.Inventory.EquippedUsableItems[slot];
					GameControl.Inventory.EquipUsableItem(selectedItem, slot);
					currentSlotGroup.CurrentSlot.SlotItem = placeholder;
				}
				else {
					GameControl.Inventory.EquipUsableItem(selectedItem, slot);
					currentSlotGroup.CurrentSlot.SlotItem = null;
				}
				ResetDescription();
			}
		}

		public override void Draw(Graphics2D g) {
			base.Draw(g);
		}

		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------


		public Slot NextAvailableSlot {
			get {
				for (int i = 0; i < currentSlotGroup.NumSlots; i++) {
					if (currentSlotGroup.GetSlotAt(i).SlotItem == null)
						return currentSlotGroup.GetSlotAt(i);
				}
				return null;
			}
		}
	}
}
