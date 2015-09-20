using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;

namespace ZeldaOracle.Game.Items {
	public class ItemBombs : ItemWeapon {

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ItemBombs() : base() {
			this.id = "item_bombs";
			this.name = new string[] { "Bombs", "Bombs", "Bombs" };
			this.description = new string[] { "Very explosive.", "Very explosive.", "Very explosive." };
			this.maxLevel = Item.Level3;
			this.currentAmmo = 0;
			this.sprite = new Sprite[] {
				new Sprite(GameData.SHEET_ITEMS_SMALL, new Point2I(13, 0)),
				new Sprite(GameData.SHEET_ITEMS_SMALL, new Point2I(13, 0)),
				new Sprite(GameData.SHEET_ITEMS_SMALL, new Point2I(13, 0))
			};
			this.spriteLight = new Sprite[] {
				new Sprite(GameData.SHEET_ITEMS_SMALL_LIGHT, new Point2I(13, 0)),
				new Sprite(GameData.SHEET_ITEMS_SMALL_LIGHT, new Point2I(13, 0)),
				new Sprite(GameData.SHEET_ITEMS_SMALL_LIGHT, new Point2I(13, 0))
			};
		}


		//-----------------------------------------------------------------------------
		// Virtual
		//-----------------------------------------------------------------------------

		// Called when the item is switched to.
		public override void OnStart() { }

		// Called when the item is put away.
		public override void OnEnd() { }

		// Immediately interrupt this item (ex: if link falls in a hole).
		public override void Interrupt() { }

		// Draws under link's sprite.
		public override void DrawUnder() { }

		// Draws over link's sprite.
		public override void DrawOver() { }

		// Called when the item is added to the inventory list
		public override void OnAdded(Inventory inventory) {
			base.OnAdded(inventory);
			int[] maxAmounts = { 10, 20, 30 };

			this.currentAmmo = 0;
			this.ammo = new Ammo[] {
				inventory.AddAmmo(
					new Ammo(
						"ammo_bombs", "Bombs", "Very explosive.",
						new Sprite(GameData.SHEET_ITEMS_SMALL, new Point2I(13, 0)),
						new Sprite(GameData.SHEET_ITEMS_SMALL_LIGHT, new Point2I(13, 0)),
						maxAmounts[level], maxAmounts[level]
					),
					false
				)
			};
		}

		// Called when the item's level is changed.
		public override void OnLevelUp() {
			int[] maxAmounts = { 10, 20, 30 };
			inventory.GetAmmo("ammo_bombs").MaxAmount = maxAmounts[level];
			inventory.GetAmmo("ammo_bombs").Amount = maxAmounts[level];
		}

		// Called when the item has been obtained.
		public override void OnObtained() {
			inventory.ObtainAmmo(inventory.GetAmmo("ammo_bombs"));
		}

		// Called when the item has been unobtained.
		public override void OnUnobtained() {

		}

		// Called when the item has been stolen.
		public override void OnStolen() {

		}

		// Called when the stolen item has been returned.
		public override void OnReturned() {

		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		// Draws the item inside the inventory.
		public override void DrawSlot(Graphics2D g, Point2I position, bool light) {
			DrawSprite(g, position, light);
			DrawAmmo(g, position, light);
		}
	}
}
