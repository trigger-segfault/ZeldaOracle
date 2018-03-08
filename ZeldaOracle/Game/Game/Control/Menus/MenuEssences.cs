using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Items;
using ZeldaOracle.Game.Main;

namespace ZeldaOracle.Game.Control.Menus {

	public class MenuEssences : InventoryMenu {

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public MenuEssences(GameManager gameManager)
			: base(gameManager)
		{
			//this.backgroundSprite = Resources.GetImage("UI/menu_essences_a");
			this.background = GameData.SPR_BACKGROUND_MENU_ESSENCES;

			SlotGroup group1 = new SlotGroup();
			SlotGroup group2 = new SlotGroup();
			currentSlotGroup = group1;
			this.slotGroups.Add(group1);
			this.slotGroups.Add(group2);

			Point2I[] essencePoints = new Point2I[]{
				new Point2I(32, 16),
				new Point2I(56, 16),
				new Point2I(72, 32),
				new Point2I(72, 56),
				new Point2I(56, 72),
				new Point2I(32, 72),
				new Point2I(16, 56),
				new Point2I(16, 32)
			};

			for (int i = 0; i < 8; i++) {
				group1.AddSlot(essencePoints[i], 16);
			}

			for (int i = 0; i < 8; i++) {
				group1.GetSlotAt(i).SetConnection(Direction.Up, group1.GetSlotAt((i + 7) % 8));
				group1.GetSlotAt(i).SetConnection(Direction.Down, group1.GetSlotAt((i + 1) % 8));
				group1.GetSlotAt(i).SetConnection(Direction.Left, group2);
				group1.GetSlotAt(i).SetConnection(Direction.Right, group2);
			}

			//group2.AddSlot(new Point2I(112, 8), 32);
			//group2.AddSlot(new Point2I(112, 56), 32);


			group2.AddSlot(new Point2I(112, 32), 32);
			group2.GetSlotAt(0).SlotItem = new CustomSlotItem("Pieces of Heart", "4 more makes a Heart Container.", null);

			group2.AddSlot(new Point2I(112, 80), 32);
			group2.GetSlotAt(1).SlotItem = new CustomSlotItem("Save Screen", "Go to the Save Screen.", GameData.SPR_HUD_SAVE_BUTTON);

			for (int i = 0; i < 2; i++) {
				group2.GetSlotAt(i).SetConnection(Direction.Up, group2.GetSlotAt((i + 1) % 2));
				group2.GetSlotAt(i).SetConnection(Direction.Down, group2.GetSlotAt((i + 1) % 2));
				group2.GetSlotAt(i).SetConnection(Direction.Left, group1);
				group2.GetSlotAt(i).SetConnection(Direction.Right, group1);
			}
		}
		

		//-----------------------------------------------------------------------------
		// Item Management
		//-----------------------------------------------------------------------------

		public void AddItem(ItemEssence essence) {
			GetEssenceSlotAt(essence.EssenceSlot).SlotItem = essence;
		}

		public void RemoveItem(ItemEssence essence) {
			GetEssenceSlotAt(essence.EssenceSlot).SlotItem = null;
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnBegin() {
			base.OnBegin();

			(slotGroups[1].GetSlotAt(0).SlotItem as CustomSlotItem).Description =
				(4 - GameControl.Inventory.PiecesOfHeart).ToString() + " more makes a Heart Container.";
		}

		public override void Update() {
			base.Update();

			// Equip equipment.
			if (Controls.A.IsPressed()) {
				if (currentSlotGroup.NumSlots == 3 && currentSlotGroup.CurrentSlotIndex == 2) {
					AudioSystem.PlaySound(GameData.SOUND_MENU_SELECT);
				}
			}
		}

		protected override void DrawMenu(Graphics2D g) {
			base.DrawMenu(g);
			DrawHeartPieces(g);
		}


		//-----------------------------------------------------------------------------
		// Drawing
		//-----------------------------------------------------------------------------

		public void DrawHeartPieces(Graphics2D g) {

			for (int i = 0; i < 4; i++) {
				if (i < GameControl.Inventory.PiecesOfHeart)
					g.DrawSprite(GameData.SPR_HUD_HEART_PIECES_FULL[i], new Point2I(112, 8));
				else
					g.DrawSprite(GameData.SPR_HUD_HEART_PIECES_EMPTY[i], new Point2I(112, 8));
			}

			g.DrawString(GameData.FONT_SMALL, GameControl.Inventory.PiecesOfHeart.ToString() + "/4", new Point2I(120, 40), EntityColors.Black);
		}

		//-----------------------------------------------------------------------------
		// Slots
		//-----------------------------------------------------------------------------

		public Slot GetEssenceSlotAt(int index) {
			return slotGroups[0].GetSlotAt(index);
		}
	}
}
