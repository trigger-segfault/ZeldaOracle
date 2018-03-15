using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Game.Items.Rewards;

namespace ZeldaOracle.Game.Items.KeyItems {
	public class ItemMagicPotion : ItemSecondary {


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ItemMagicPotion() : base("item_magic_potion") {
			SetName("Magic Potion");
			SetDescription("Fill your hearts!");
			SetSprite(GameData.SPR_ITEM_ICON_MAGIC_POTION);
			HoldType = RewardHoldTypes.TwoHands;

			slot = new Point2I(1, 0);
		}

	}
}
