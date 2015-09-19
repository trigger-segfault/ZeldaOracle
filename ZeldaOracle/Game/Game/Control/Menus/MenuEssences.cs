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

	public class MenuEssences : PlayerMenu {

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public MenuEssences(GameManager gameManager)
			: base(gameManager) {
			this.backgroundSprite = Resources.GetImage("UI/menu_essences_a");

			SlotGroup group1 = new SlotGroup();
			SlotGroup group2 = new SlotGroup();
			currentSlotGroup = group1;
			this.slotGroups.Add(group1);
			this.slotGroups.Add(group2);

			Point2I[] essencePoints = new Point2I[]{
				new Point2I(32, 16),
				new Point2I(64, 16),
				new Point2I(72, 32),
				new Point2I(72, 56),
				new Point2I(64, 72),
				new Point2I(32, 72),
				new Point2I(16, 56),
				new Point2I(16, 32)
			};

			for (int i = 0; i < 8; i++) {
				group1.AddSlot(essencePoints[i], 16);
			}

			for (int i = 0; i < 8; i++) {
				group1.GetSlotAt(i).SetConnection(Directions.Up, group1.GetSlotAt((i + 7) % 8));
				group1.GetSlotAt(i).SetConnection(Directions.Down, group1.GetSlotAt((i + 1) % 8));
				group1.GetSlotAt(i).SetConnection(Directions.Left, group2);
				group1.GetSlotAt(i).SetConnection(Directions.Right, group2);
			}

			group2.AddSlot(new Point2I(112, 8), 32);
			group2.AddSlot(new Point2I(112, 56), 32);
			group2.AddSlot(new Point2I(112, 80), 32);

			for (int i = 0; i < 3; i++) {
				group2.GetSlotAt(i).SetConnection(Directions.Up, group2.GetSlotAt((i + 2) % 3));
				group2.GetSlotAt(i).SetConnection(Directions.Down, group2.GetSlotAt((i + 1) % 3));
				group2.GetSlotAt(i).SetConnection(Directions.Left, group1);
				group2.GetSlotAt(i).SetConnection(Directions.Right, group1);
			}
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void Update() {
			base.Update();

			// Equip equipment.
			if (Controls.A.IsPressed()) {
				if (currentSlotGroup.NumSlots == 3 && currentSlotGroup.CurrentSlotIndex == 2) {
					AudioSystem.PlaySound("UI/menu_select");
				}
			}
		}

		public override void Draw(Graphics2D g) {
			base.Draw(g);
		}
	}
}
