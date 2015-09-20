using System;
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

		public ItemShield()
			: base() {
			this.id				= "item_shield";
			this.name			= new string[] { "Wooden Shield", "Iron Shield", "Mirror Shield" };
			this.description	= new string[] { "A small shield.", "A large shield.", "A reflective shield." };
			this.maxLevel		= Item.Level3;
			this.sprite			= new Sprite[] {
				new Sprite(GameData.SHEET_ITEMS_SMALL, new Point2I(3, 0)),
				new Sprite(GameData.SHEET_ITEMS_SMALL, new Point2I(4, 0)),
				new Sprite(GameData.SHEET_ITEMS_SMALL, new Point2I(5, 0))
			};
			this.spriteLight	= new Sprite[] {
				new Sprite(GameData.SHEET_ITEMS_SMALL_LIGHT, new Point2I(3, 0)),
				new Sprite(GameData.SHEET_ITEMS_SMALL_LIGHT, new Point2I(4, 0)),
				new Sprite(GameData.SHEET_ITEMS_SMALL_LIGHT, new Point2I(5, 0))
			};
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		// Draws the item inside the inventory.
		public override void DrawSlot(Graphics2D g, Point2I position, bool light) {
			DrawSprite(g, position, light);
			DrawLevel(g, position, light);
		}
	}
}
