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

		public ItemEssence6() : base("essence_6") {
			SetName("Lonely Peak");
			SetDescription("A proud spirit that remains stalwart in trying times.");
			SetSprite(GameData.SPR_ITEM_ICON_ESSENCE_6);

			slot = 5;
		}

	}
}
