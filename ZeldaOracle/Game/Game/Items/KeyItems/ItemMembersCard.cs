using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Game.Items.Rewards;

namespace ZeldaOracle.Game.Items.KeyItems {
	public class ItemMembersCard : ItemSecondary {


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ItemMembersCard() : base("item_members_card") {
			SetName("Member's Card");
			SetDescription("Opens the door!");
			SetSprite(GameData.SPR_ITEM_ICON_MEMBERS_CARD);
			HoldType = RewardHoldTypes.TwoHands;

			slot = new Point2I(1, 1);
		}

	}
}
