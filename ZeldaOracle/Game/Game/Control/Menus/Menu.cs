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
using ZeldaOracle.Game.Main;

namespace ZeldaOracle.Game.Control.Menus {
	public class Menu : GameState {

		// The background image used in the menu.
		//protected Image backgroundSprite;
		protected ISprite background;

		protected SlotGroup currentSlotGroup;
		protected List<SlotGroup> slotGroups;

		protected bool drawHUD;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public Menu(GameManager gameManager) {
			this.gameManager = gameManager;
			this.background = null;
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

		public override void AssignPalettes() {
			GameData.PaletteShader.TilePalette = GameData.PAL_MENU_DEFAULT;
			GameData.PaletteShader.EntityPalette = GameData.PAL_ENTITIES_DEFAULT;
		}

		public sealed override void Draw(Graphics2D g) {
			if (drawHUD) {
				GameControl.HUD.Draw(g, true);
				g.PushTranslation(0, GameSettings.HUD_HEIGHT);
			}

			DrawMenu(g);

			if (drawHUD) {
				g.PopTranslation();
			}
		}

		protected virtual void DrawMenu(Graphics2D g) {
			g.DrawSprite(background, Point2I.Zero);
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
			g.DrawSprite(GameData.SPR_HUD_BRACKET_LEFT, slot.Position + new Point2I(-8, 0));
			g.DrawSprite(GameData.SPR_HUD_BRACKET_RIGHT, slot.Position + new Point2I(slot.Width, 0));
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
					AudioSystem.PlaySound(GameData.SOUND_MENU_CURSOR_MOVE);
				}
				else if (connection is SlotGroup) {
					currentSlotGroup = (connection as SlotGroup);
					AudioSystem.PlaySound(GameData.SOUND_MENU_CURSOR_MOVE);
				}
			}
		}

		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public SlotGroup CurrentSlotGroup {
			get { return currentSlotGroup; }
		}
	}
}
