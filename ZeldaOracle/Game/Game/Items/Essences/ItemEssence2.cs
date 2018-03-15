using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Graphics.Sprites;

namespace ZeldaOracle.Game.Items.Essences {
	public class ItemEssence2 : ItemEssence {

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ItemEssence2() : base("essence_2") {
			SetName("Ancient Wood");
			SetDescription("It speaks only truth to closed ears.");
			SetSprite(GameData.SPR_ITEM_ICON_ESSENCE_2);

			slot = 1;
		}

	}
}
