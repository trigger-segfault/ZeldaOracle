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
			this.spriteLight	= new Sprite[] { new Sprite(GameData.SHEET_ITEMS_SMALL_LIGHT, new Point2I(12, 1)) };
			this.flags			= ItemFlags.None;
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		// Draws the item inside the inventory.
		public override void DrawSlot(Graphics2D g, Point2I position, bool light) {
			DrawSprite(g, position, light);
			// Draw song
		}
	}
}
