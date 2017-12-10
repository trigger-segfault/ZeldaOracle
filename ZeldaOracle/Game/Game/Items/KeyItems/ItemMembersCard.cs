using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Graphics.Sprites;

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
			this.sprite = new ISprite[] { GameData.SPR_ITEM_ICON_MEMBERS_CARD };
		}

	}
}
