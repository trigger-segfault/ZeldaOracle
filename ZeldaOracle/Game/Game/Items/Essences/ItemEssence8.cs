using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;

namespace ZeldaOracle.Game.Items.Essences {
	public class ItemEssence8 : ItemEssence {

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ItemEssence8() : base() {
			this.id = "item_essence_8";
			this.name = new string[] { "Falling Star" };
			this.description = new string[] { "Its eternal light acts as guide to the other essences." };
			this.slot = 7;
			this.sprite = new Sprite[] { GameData.SPR_ITEM_ICON_ESSENCE_8 };
		}

	}
}
