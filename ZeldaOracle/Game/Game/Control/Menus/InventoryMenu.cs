using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Translation;
using ZeldaOracle.Game.GameStates.Transitions;
using ZeldaOracle.Game.Main;

namespace ZeldaOracle.Game.Control.Menus {
	public class InventoryMenu : Menu {

		protected InventoryMenu previousMenu;
		protected InventoryMenu nextMenu;

		private int textPosition;
		private int textTimer;
		private int textStart;
		private LetterString description;

		protected bool inSubMenu;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public InventoryMenu(GameManager gameManager) :
			base(gameManager)
		{
			this.previousMenu	= null;
			this.nextMenu		= null;
			this.drawHUD		= true;
			this.description	= new LetterString();
			this.textPosition	= 0;
			this.textTimer		= 0;
			this.textStart		= 0;
			this.inSubMenu		= false;
		}


		//-----------------------------------------------------------------------------
		// Virtual methods
		//-----------------------------------------------------------------------------
		
		// Called when the inventory menu is opened from the gameplay.
		public virtual void OnOpen() {}
		
		// Called when the entire menu collection is closed.
		public virtual void OnClose() {}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnBegin() {
			base.OnBegin();

			ResetDescription();
		}

		public override void Update() {
			base.Update();
			if (!inSubMenu) {
				if (Controls.Start.IsPressed()) {
					GameControl.CloseMenu(this);
				}
				if (Controls.Select.IsPressed()) {
					AudioSystem.PlaySound(GameData.SOUND_MENU_NEXT);
					gameManager.PopGameState();
					gameManager.PushGameState(new MenuTransitionPush(
						this, nextMenu, Direction.Right));
				}
			}

			UpdateDescription();

			if (!inSubMenu) {
				for (int i = 0; i < 4; i++) {
					if (Controls.Arrows[i].IsPressed()) {
						ResetDescription();
						break;
					}
				}
			}
		}

		protected override void DrawMenu(Graphics2D g) {
			base.DrawMenu(g);
			if (IsActive)
				DrawDescription(g);
		}


		//-----------------------------------------------------------------------------
		// Description
		//-----------------------------------------------------------------------------

		public void ResetDescription() {
			ISlotItem item = currentSlotGroup.CurrentSlot.SlotItem;
			description = FormatCodes.FormatString(item != null ? item.Name : "", GameControl.Vars);
			textPosition = 0;
			textTimer = 0;
			textStart = 0;
			if (!description.IsEmpty) {
				if (description.Length % 2 == 1)
					description.Add(' ');

				if (description.Length == 16) {
					description.Add(' ');
					textStart = 0;
				}
				else {
					for (int i = (8 - description.Length / 2); i > 0; i--)
						description.Add(' ');
					textStart = (16 - description.Length) * 8;
				}

				description.AddRange(FormatCodes.FormatString(item.Description, GameControl.Vars));
			}
		}

		// Called every step to update the item description's scroll position.
		public void UpdateDescription() {
			if (!description.IsEmpty) {
				if (textTimer > 0) {
					// Update the pause timer.
					textTimer--;
					if (textTimer == 0)
						textPosition++;
				}
				else if (textPosition == 0) {
					// Pause the text when it lands on the item name.
					textTimer = 32;
				}
				else {
					textPosition++;
					if (textPosition / 8 > description.Length + textStart / 8) {
						// Wrap the text when it reaches the end.
						textPosition = -128 + textStart;
					}
				}
			}

		}

		// Draws the scrolling item description at the bottom of the screen.
		public void DrawDescription(Graphics2D g) {
			int position = textPosition - textStart;
			int textIndex = position / 8;
			if (position < 0) {
				// Round down always.
				textIndex = (position - 7) / 8;
				position = ((position - 7) / 8) * 8;
			}
			else {
				position = (position / 8) * 8;
			}

			int startIndex = GMath.Max(0, textIndex);
			int endIndex = GMath.Clamp(textIndex + 16, 0, description.Length);
			LetterString text = description.Substring(startIndex, endIndex - startIndex);

			if (position < 0)
				g.DrawLetterString(GameData.FONT_LARGE, text, new Point2I(16 - (position / 8) * 8, 108), TileColors.MenuDark);
			else
				g.DrawLetterString(GameData.FONT_LARGE, text, new Point2I(16, 108), TileColors.MenuDark);
		}


		//-----------------------------------------------------------------------------
		// Slots
		//-----------------------------------------------------------------------------

		public Slot GetSlotWithItem(ISlotItem item) {
			for (int i = 0; i < slotGroups.Count; i++) {
				for (int j = 0; j < slotGroups[i].NumSlots; j++) {
					if (slotGroups[i].GetSlotAt(j).SlotItem == item) {
						return slotGroups[i].GetSlotAt(j);
					}
				}
			}
			return null;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public InventoryMenu PreviousMenu {
			get { return previousMenu; }
			set { previousMenu = value; }
		}

		public InventoryMenu NextMenu {
			get { return nextMenu; }
			set { nextMenu = value; }
		}
	}
}
