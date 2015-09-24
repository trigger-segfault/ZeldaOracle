using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Input;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Entities.Projectiles;
using ZeldaOracle.Game.Entities.Effects;
using ZeldaOracle.Game.Entities.Players;

namespace ZeldaOracle.Game.Items.Weapons {
	public class ItemBoomerang : ItemWeapon {


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ItemBoomerang() {
			this.id				= "item_boomerang";
			this.name			= new string[] { "Boomerang", "Magic Boomerang" };
			this.description	= new string[] { "Always comes back to you.", "A remote-control weapon." };
			this.level			= 0;
			this.maxLevel		= Item.Level2;
			this.flags			= ItemFlags.UsableInMinecart | ItemFlags.UsableWhileJumping | ItemFlags.UsableWithSword;

			sprite = new Sprite[] {
				new Sprite(GameData.SHEET_ITEMS_SMALL, new Point2I(4, 1)),
				new Sprite(GameData.SHEET_ITEMS_SMALL, new Point2I(5, 1))
			};
			spriteLight	= new Sprite[] {
				new Sprite(GameData.SHEET_ITEMS_SMALL_LIGHT, new Point2I(4, 1)),
				new Sprite(GameData.SHEET_ITEMS_SMALL_LIGHT, new Point2I(5, 1))
			};
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnButtonPress() {
			
		}

		// Draws the item inside the inventory.
		public override void DrawSlot(Graphics2D g, Point2I position, bool light) {
			DrawSprite(g, position, light);
			DrawLevel(g, position, light);
		}

	}
}
