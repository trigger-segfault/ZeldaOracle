﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;

namespace ZeldaOracle.Game.Items.Weapons {
	public class ItemShield : ItemWeapon {
		

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ItemShield() {
			this.id				= "item_shield";
			this.name			= new string[] { "Wooden Shield", "Iron Shield", "Mirror Shield" };
			this.description	= new string[] { "A small shield.", "A large shield.", "A reflective shield." };
			this.maxLevel		= Item.Level3;
			this.flags			= ItemFlags.UsableWhileJumping;

			this.sprite			= new Sprite[] {
				new Sprite(GameData.SHEET_ITEMS_SMALL, new Point2I(3, 0)),
				new Sprite(GameData.SHEET_ITEMS_SMALL, new Point2I(4, 0)),
				new Sprite(GameData.SHEET_ITEMS_SMALL, new Point2I(5, 0))
			};
		}

	}
}
