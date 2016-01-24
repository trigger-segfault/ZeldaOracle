using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;

namespace ZeldaOracle.Game.Items.KeyItems {
	public class ItemEssenceSeed : ItemSecondary {


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ItemEssenceSeed() {
			this.id = "item_essence_seed";
			this.name = new string[] { "Maku Seed" };
			this.description = new string[] { "Evil-cleansing sacred seed." };
			this.slot = new Point2I(3, 0);
			this.sprite = new Sprite[] { GameData.SPR_ITEM_ICON_ESSENCE_SEED };
		}

	}
}
