using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Game.Items.Rewards;

namespace ZeldaOracle.Game.Items.KeyItems {
	public class ItemEssenceSeed : ItemSecondary {


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ItemEssenceSeed() : base("item_essence_seed") {
			SetName("Maku Seed");
			SetDescription("Evil-cleansing sacred seed.");
			SetSprite(GameData.SPR_ITEM_ICON_ESSENCE_SEED);
			HoldType = RewardHoldTypes.TwoHands;

			slot = new Point2I(3, 0);
		}

	}
}
