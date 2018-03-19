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
		
		/// <summary>Construct a seed satchel ammo.</summary>
		public AmmoSatchelSeeds() {
		}


		//-----------------------------------------------------------------------------
		// Virtual
		//-----------------------------------------------------------------------------

		/// <summary>Draws the item inside the inventory.</summary>
		public override void DrawSlot(Graphics2D g, Point2I position) {
			g.DrawSprite(Sprite, position + new Point2I(4, 0));
			g.DrawString(GameData.FONT_SMALL, Amount.ToString("00"),
				position + new Point2I(0, 12), TileColors.MenuWhite);
		}
	}
}
