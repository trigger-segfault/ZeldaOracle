using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;

namespace ZeldaOracle.Game.Items.KeyItems {
	public class ItemMembersCard : ItemSecondary {


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ItemMembersCard() {
			this.id = "item_members_card";
			this.name = new string[] { "Member's Card" };
			this.description = new string[] { "Opens the door!" };
			this.slot = new Point2I(1, 1);
			this.sprite = new Sprite[] { new Sprite(GameData.SHEET_ITEMS_LARGE, new Point2I(7, 7)) };
			this.spriteLight = new Sprite[] { new Sprite(GameData.SHEET_ITEMS_LARGE_LIGHT, new Point2I(7, 7)) };
		}

	}
}
