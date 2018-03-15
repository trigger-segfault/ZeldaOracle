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

		public ItemEssence5() : base("essence_5") {
			SetName("Sacred Soil");
			SetDescription("Its warmth is known by all that rests in the earth.");
			SetSprite(GameData.SPR_ITEM_ICON_ESSENCE_5);

			slot = 4;
		}

	}
}
