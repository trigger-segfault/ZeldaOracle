using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;

namespace ZeldaOracle.Game.Items.Ammos {
	public class AmmoSatchelSeeds : Ammo {
		

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public AmmoSatchelSeeds(string id, string name, string description, Sprite sprite, Sprite spriteLight, int amount, int maxAmount)
			: base(id, name, description, sprite, spriteLight, amount, maxAmount)
		{
		}

		//-----------------------------------------------------------------------------
		// Virtual
		//-----------------------------------------------------------------------------

		// Draws the item inside the inventory.
		public override void DrawSlot(Graphics2D g, Point2I position, bool light) {
			g.DrawSprite(light ? spriteLight : sprite, position + new Point2I(4, 0));
			g.DrawString(GameData.FONT_SMALL, Amount.ToString("00"), position + new Point2I(0, 12), new Color(248, 248, 248));
		}
	}
}
