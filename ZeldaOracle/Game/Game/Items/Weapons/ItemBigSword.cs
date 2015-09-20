using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;

namespace ZeldaOracle.Game.Items.Weapons {
	public class ItemBigSword : ItemWeapon {
		
		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ItemBigSword()
			: base() {
			this.id				= "item_biggoron_sword";
			this.name			= new string[] { "Biggoron's Sword" };
			this.description	= new string[] { "A powerful, two-handed sword." };
			this.sprite			= new Sprite[] { new Sprite(GameData.SHEET_ITEMS_LARGE, new Point2I(8, 0)) };
			this.spriteLight	= new Sprite[] { new Sprite(GameData.SHEET_ITEMS_LARGE_LIGHT, new Point2I(8, 0)) };
			this.flags |= ItemFlags.TwoHanded;
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------


		// Draws the item inside the inventory.
		public override void DrawSlot(Graphics2D g, Point2I position, bool light) {
			if (inventory.EquippedWeapons[0] == this) {
				SpriteSheet sheetItemsLarge = (light ? GameData.SHEET_ITEMS_LARGE_LIGHT : GameData.SHEET_ITEMS_LARGE);
				Sprite largeSprite = new Sprite(sheetItemsLarge, new Point2I(9, 0));
				largeSprite.NextPart = new Sprite(sheetItemsLarge, new Point2I(10, 0), new Point2I(16, 0));
				g.DrawSprite(largeSprite, position);
			}
			else {
				DrawSprite(g, position, light);
			}
		}
	}
}
