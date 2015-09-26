using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;

namespace ZeldaOracle.Game.Items.Essences {
	public class ItemEssence2 : ItemEssence {

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ItemEssence2() : base() {
			this.id = "item_essence_2";
			this.name = new string[] { "Ancient Wood" };
			this.description = new string[] { "It speaks only truth to closed ears." };
			this.slot = 1;
			this.sprite = new Sprite[] { new Sprite(GameData.SHEET_ITEMS_LARGE, new Point2I(9, 7)) };
		}

	}
}
