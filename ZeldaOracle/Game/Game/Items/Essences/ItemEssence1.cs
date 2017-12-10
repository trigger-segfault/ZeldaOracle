using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Graphics.Sprites;

namespace ZeldaOracle.Game.Items.Essences {
	public class ItemEssence1 : ItemEssence {

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ItemEssence1() : base() {
			this.id = "item_essence_1";
			this.name = new string[] { "Eternal Spirit" };
			this.description = new string[] { "It speaks across time to the heart!" };
			this.slot = 0;
			this.sprite = new ISprite[] { GameData.SPR_ITEM_ICON_ESSENCE_1 };
		}

	}
}
