using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;

namespace ZeldaOracle.Game.Items.KeyItems {
	public class ItemFlippers : ItemSecondary {


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ItemFlippers() {
			this.id = "item_flippers";
			this.name = new string[] { "Zora's Flippers", "Mermaid Suit" };
			this.description = new string[] { "Hit the beach.", "The skin of the mythical beast." };
			this.maxLevel = Item.Level2;
			this.slot = new Point2I(0, 0);
			this.sprite = new Sprite[] {
				new Sprite(GameData.SHEET_ITEMS_LARGE, new Point2I(6, 1)),
				new Sprite(GameData.SHEET_ITEMS_LARGE, new Point2I(7, 1))
			};
			this.spriteLight = new Sprite[] {
				new Sprite(GameData.SHEET_ITEMS_LARGE_LIGHT, new Point2I(6, 1)),
				new Sprite(GameData.SHEET_ITEMS_LARGE_LIGHT, new Point2I(7, 1))
			};
		}

	}
}
