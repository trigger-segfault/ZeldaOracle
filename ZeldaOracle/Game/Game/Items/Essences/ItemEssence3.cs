using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;

namespace ZeldaOracle.Game.Items.Essences {
	public class ItemEssence3 : ItemEssence {

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ItemEssence3() : base() {
			this.id = "item_essence_3";
			this.name = new string[] { "Echoing Howl" };
			this.description = new string[] { "It echoes far across plains to speak to insolent hearts." };
			this.slot = 2;
			this.sprite = new Sprite[] { new Sprite(GameData.SHEET_ITEMS_LARGE, new Point2I(10, 7)) };
		}

	}
}
