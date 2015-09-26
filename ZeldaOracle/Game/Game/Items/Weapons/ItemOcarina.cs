using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;

namespace ZeldaOracle.Game.Items.Weapons {
	public class ItemOcarina : ItemWeapon {
		

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ItemOcarina() {
			this.id				= "item_ocarina";
			this.name			= new string[] { "Ocarina" };
			this.description	= new string[] { "Plays a beautiful sound." };
			this.sprite			= new Sprite[] { new Sprite(GameData.SHEET_ITEMS_SMALL, new Point2I(12, 1)) };
			this.flags			= ItemFlags.None;
		}

	}
}
