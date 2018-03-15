using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Graphics.Sprites;

namespace ZeldaOracle.Game.Items.Essences {
	public class ItemEssence3 : ItemEssence {

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ItemEssence3() : base("essence_3") {
			SetName("Echoing Howl");
			SetDescription("It echoes far across plains to speak to insolent hearts.");
			SetSprite(GameData.SPR_ITEM_ICON_ESSENCE_3);

			slot = 2;
		}

	}
}
