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
	public class ItemFeather : UsableItem {


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ItemFeather(int level = 0) : base() {
			this.id				= "item_feather";
			this.name			= new string[] { "Roc's Feather", "Roc's Cape" };
			this.description	= new string[] { "A nice lift.", "A wing-riding cape." };
			this.level			= level;
			this.maxLevel		= 2;
			this.sprite			= new Sprite(GameData.SHEET_ITEMS_SMALL, 2, 1);
			this.spriteLight	= new Sprite(GameData.SHEET_ITEMS_SMALL_LIGHT, 2, 1);
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		// Jump/deploy cape.
		public override void OnButtonPress() {
			Player.Jump();
		}

		// Draws the item inside the inventory.
		public override void DrawInInventory(Graphics2D g, Point2I position, bool light) {
			DrawSprite(g, position, light);
			DrawSprite(g, position, light);
			DrawLevel(g, position, light);
		}

	}
}
