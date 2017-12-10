using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Graphics.Sprites;

namespace ZeldaOracle.Game.Items.Ammos {
	public class AmmoSatchelSeeds : Ammo {
		

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public AmmoSatchelSeeds(string id, string name, string description, ISprite sprite, int amount, int maxAmount)
			: base(id, name, description, sprite, amount, maxAmount)
		{
		}

		//-----------------------------------------------------------------------------
		// Virtual
		//-----------------------------------------------------------------------------

		// Draws the item inside the inventory.
		public override void DrawSlot(Graphics2D g, Point2I position, int lightOrDark) {
			g.DrawSprite(sprite, position + new Point2I(4, 0));
			g.DrawString(GameData.FONT_SMALL, Amount.ToString("00"), position + new Point2I(0, 12), new Color(248, 248, 248));
		}
	}
}
