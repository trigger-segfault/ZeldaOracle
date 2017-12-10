using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Graphics.Sprites;

namespace ZeldaOracle.Game.Items.Essences {
	public class ItemEssence6 : ItemEssence {

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ItemEssence6() : base() {
			this.id = "item_essence_6";
			this.name = new string[] { "Lonely Peak" };
			this.description = new string[] { "A proud spirit that remains stalwart in trying times." };
			this.slot = 5;
			this.sprite = new ISprite[] { GameData.SPR_ITEM_ICON_ESSENCE_6 };
		}

	}
}
