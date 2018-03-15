using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Graphics.Sprites;

namespace ZeldaOracle.Game.Items.Essences {
	public class ItemEssence8 : ItemEssence {

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ItemEssence8() : base("essence_8") {
			SetName("Falling Star");
			SetDescription("Its eternal light acts as guide to the other essences.");
			SetSprite(GameData.SPR_ITEM_ICON_ESSENCE_8);

			slot = 7;
		}

	}
}
