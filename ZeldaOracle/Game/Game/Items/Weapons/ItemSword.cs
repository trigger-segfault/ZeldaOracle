using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;

namespace ZeldaOracle.Game.Items.Weapons {
	public class ItemSword : ItemWeapon {
		

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ItemSword()
			: base() {
			this.id				= "item_sword";
			this.name			= new string[] { "Wooden Sword", "Noble Sword", "Master Sword" };
			this.description	= new string[] { "A hero's blade.", "A sacred blade.", "The blade of legends." };
			this.maxLevel		= Item.Level3;
			this.sprite			= new Sprite[] {
				new Sprite(GameData.SHEET_ITEMS_SMALL, new Point2I(0, 0)),
				new Sprite(GameData.SHEET_ITEMS_SMALL, new Point2I(1, 0)),
				new Sprite(GameData.SHEET_ITEMS_SMALL, new Point2I(2, 0))
			};
			this.spriteLight	= new Sprite[] {
				new Sprite(GameData.SHEET_ITEMS_SMALL_LIGHT, new Point2I(0, 0)),
				new Sprite(GameData.SHEET_ITEMS_SMALL_LIGHT, new Point2I(1, 0)),
				new Sprite(GameData.SHEET_ITEMS_SMALL_LIGHT, new Point2I(2, 0))
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
