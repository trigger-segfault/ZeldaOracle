using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;

namespace ZeldaOracle.Game.Items.Essences {
	public class ItemEssence7 : ItemEssence {

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ItemEssence7() : base() {
			this.id = "item_essence_7";
			this.name = new string[] { "Rolling Sea" };
			this.description = new string[] { "The song of the sea forms a wave that carries heros into adventure." };
			this.slot = 6;
			this.sprite = new Sprite[] { new Sprite(GameData.SHEET_ITEMS_LARGE, new Point2I(2, 8)) };
			this.spriteLight = new Sprite[] { new Sprite(GameData.SHEET_ITEMS_LARGE_LIGHT, new Point2I(2, 8)) };
		}

	}
}
