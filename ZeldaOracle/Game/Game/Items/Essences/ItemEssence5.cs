using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Graphics.Sprites;

namespace ZeldaOracle.Game.Items.Essences {
	public class ItemEssence5 : ItemEssence {

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ItemEssence5() : base() {
			this.id = "item_essence_5";
			this.name = new string[] { "Sacred Soil" };
			this.description = new string[] { "Its warmth is known by all that rests in the earth." };
			this.slot = 4;
			this.sprite = new ISprite[] { GameData.SPR_ITEM_ICON_ESSENCE_5 };
		}

	}
}
