using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Graphics.Sprites;

namespace ZeldaOracle.Game.Items.Essences {
	public class ItemEssence7 : ItemEssence {

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ItemEssence7() : base("essence_7") {
			SetName("Rolling Sea");
			SetDescription("The song of the sea forms a wave that carries heros into adventure.");
			SetSprite(GameData.SPR_ITEM_ICON_ESSENCE_7);

			slot = 6;
		}

	}
}
