using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.GameStates;
using ZeldaOracle.Game.Main;

namespace ZeldaOracle.Game.Control.Menus {
	public class Menu : GameState {

		// The background image used in the menu.
		protected Image backgroundSprite;

		protected SlotGroup currentSlotGroup;
		protected List<SlotGroup> slotGroups;

		protected bool drawHUD;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public Menu(GameManager gameManager) {
			this.gameManager = gameManager;
			this.backgroundSprite = null;
			this.currentSlotGroup = null;
			this.slotGroups = new List<SlotGroup>();
			this.drawHUD = false;
		}

		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnBegin() {

		}
		
		public override void Update() {
			if (drawHUD) {
				GameControl.HUD.Update();
			}
			UpdateSlotTraversal();
		}
		
		public override void Draw(Graphics2D g) {
			if (drawHUD) {
				GameControl.HUD.Draw(g, true);
				g.Translate(0, 16);
			}
			g.DrawImage(backgroundSprite, Point2I.Zero);
			if (currentSlotGroup != null)
				DrawSlotCursor(g, currentSlotGroup.CurrentSlot);
			for (int i = 0; i < slotGroups.Count; i++) {
				slotGroups[i].Draw(g);
			}
		}


		//-----------------------------------------------------------------------------
		// Slots
		//-----------------------------------------------------------------------------

		public virtual void DrawSlotCursor(Graphics2D g, Slot slot) {
			Sprite tR = new Sprite(GameData.SHEET_MENU_SMALL_LIGHT, new Point2I(9, 0));
			Sprite bR = new Sprite(GameData.SHEET_MENU_SMALL_LIGHT, new Point2I(9, 1));
			Sprite tL = new Sprite(GameData.SHEET_MENU_SMALL_LIGHT, new Point2I(10, 0));
			Sprite bL = new Sprite(GameData.SHEET_MENU_SMALL_LIGHT, new Point2I(10, 1));

			g.DrawSprite(tR, slot.Position + new Point2I(-8, 0));
			g.DrawSprite(bR, slot.Position + new Point2I(-8, 8));

			g.DrawSprite(tL, slot.Position + new Point2I(slot.Width, 0));
			g.DrawSprite(bL, slot.Position + new Point2I(slot.Width, 8));
		}

		public void UpdateSlotTraversal() {
			if (currentSlotGroup != null) {
				for (int i = 0; i < 4; i++) {
					if (Controls.Arrows[i].IsPressed())
						NextSlot(i);
				}
			}
		}

		public void NextSlot(int direction) {
			ISlotConnection connection = currentSlotGroup.CurrentSlot.GetConnectionAt(direction);

			if (connection != null) {
				if (connection is Slot) {
					Slot slot = connection as Slot;
					slot.Select();
					if (slot.Disabled) {
						NextSlot(direction);
						return;
					}
					AudioSystem.PlaySound("UI/menu_cursor_move");
				}
				else if (connection is SlotGroup) {
					currentSlotGroup = (connection as SlotGroup);
					AudioSystem.PlaySound("UI/menu_cursor_move");
				}
			}
		}
	}
}
