using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;

namespace ZeldaOracle.Game.Items.KeyItems {
	public class ItemMagicPotion : ItemSecondary {


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ItemMagicPotion() {
			this.id = "item_magic_potion";
			this.name = new string[] { "Magic Potion" };
			this.description = new string[] { "Fill your hearts!" };
			this.slot = new Point2I(1, 0);
			this.sprite = new Sprite[] { GameData.SPR_ITEM_ICON_MAGIC_POTION };
		}

	}
}
