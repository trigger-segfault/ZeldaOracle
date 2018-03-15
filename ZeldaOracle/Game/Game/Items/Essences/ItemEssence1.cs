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

		public ItemEssence1() : base("essence_1") {
			SetName("Eternal Spirit");
			SetDescription("It speaks across time to the heart!");
			SetSprite(GameData.SPR_ITEM_ICON_ESSENCE_1);

			slot = 0;
		}

	}
}
