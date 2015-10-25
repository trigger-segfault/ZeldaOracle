using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Entities.Projectiles;

namespace ZeldaOracle.Game.Tiles {

	public class TileColorJumpPad : Tile {

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public TileColorJumpPad() {

		}

		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------
		
		public override void OnLand(Point2I startTile) {
			if (startTile != Location) {
				int spriteIndex = Properties.GetInteger("sprite_index");
				spriteIndex++;
				if (spriteIndex == TileData.SpriteList.Length)
					spriteIndex = 0;
				Properties.SetBase("sprite_index", spriteIndex);
				// TODO: Play color change sound
			}
		}

		public override void OnInitialize() {
			
		}
	}
}
