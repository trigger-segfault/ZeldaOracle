using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Graphics.Sprites;

namespace ZeldaOracle.Game.Items.Essences {
	public class ItemEssence4 : ItemEssence {

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ItemEssence4() : base("item_essence_4") {
			SetName("Burning Flame");
			SetDescription("It reignights a hero's passion deep in wavering hearts.");
			SetSprite(GameData.SPR_ITEM_ICON_ESSENCE_4);

			slot = 3;
		}

	}
}
