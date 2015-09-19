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

	public class MenuKeyItems : PlayerMenu {

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public MenuKeyItems(GameManager gameManager)
			: base(gameManager) {
			this.backgroundSprite = Resources.GetImage("UI/menu_key_items_a");

			SlotGroup group = new SlotGroup();
			currentSlotGroup = group;
			this.slotGroups.Add(group);
			Slot[,] slots = new Slot[5, 4];
			Slot ringBagSlot = null;


			Point2I gridSize = new Point2I(5, 4);

			for (int y = 0; y < gridSize.Y; y++) {
				for (int x = 0; x < gridSize.X; x++) {
					if (y == gridSize.Y - 1) {
						if (x == 0)
							ringBagSlot = group.AddSlot(new Point2I(12, 80), 16);
						slots[x, y] = group.AddSlot(new Point2I(32 + 24 * x, 80), 16);
					}
					else {
						slots[x, y] = group.AddSlot(new Point2I(24 + 24 * x, 8 + 24 * y), (x == (gridSize.X - 1) ? 24 : 16));
						group.GetSlotAt(group.NumSlots - 1);
					}
				}
			}
			for (int y = 0; y < gridSize.Y; y++) {
				for (int x = 0; x < gridSize.X; x++) {
					if (x == 0 && y == gridSize.Y - 1)
						slots[x, y].SetConnection(Directions.Left, ringBagSlot);
					else if (x == 0)
						slots[x, y].SetConnection(Directions.Left, slots[gridSize.X - 1, (y + gridSize.Y - 1) % gridSize.Y]);
					else
						slots[x, y].SetConnection(Directions.Left, slots[x - 1, y]);

					if (x == gridSize.X - 1 && y == gridSize.Y - 2)
						slots[x, y].SetConnection(Directions.Right, ringBagSlot);
					else if (x == gridSize.X - 1)
						slots[x, y].SetConnection(Directions.Right, slots[0, (y + 1) % gridSize.Y]);
					else
						slots[x, y].SetConnection(Directions.Right, slots[x + 1, y]);

					slots[x, y].SetConnection(Directions.Up, slots[x, (y + gridSize.Y - 1) % gridSize.Y]);
					slots[x, y].SetConnection(Directions.Down, slots[x, (y + 1) % gridSize.Y]);
				}
			}

			ringBagSlot.SetConnection(Directions.Left, slots[gridSize.X - 1, gridSize.Y - 2]);
			ringBagSlot.SetConnection(Directions.Right, slots[0, gridSize.Y - 1]);
			ringBagSlot.SetConnection(Directions.Up, slots[0, gridSize.Y - 2]);
			ringBagSlot.SetConnection(Directions.Down, slots[0, 0]);
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void Update() {
			base.Update();

			// Equip equipment.
			if (Controls.A.IsPressed()) {
				if (currentSlotGroup.CurrentSlotIndex >= currentSlotGroup.NumSlots - 6) {
					AudioSystem.PlaySound("UI/menu_select");
					EquippableItem selectedItem = currentSlotGroup.CurrentSlot.SlotItem as EquippableItem;
					GameControl.Inventory.EquipEquippableItem(selectedItem);
				}
			}
		}

		public override void Draw(Graphics2D g) {
			base.Draw(g);
		}
	}
}
