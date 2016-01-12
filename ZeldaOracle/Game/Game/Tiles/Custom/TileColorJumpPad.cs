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
				// Cycle the color (red -> yellow -> blue)
				PuzzleColor c = Color;
				if (c == PuzzleColor.Red)
					Color = PuzzleColor.Yellow;
				else if (c == PuzzleColor.Yellow)
					Color = PuzzleColor.Blue;
				else
 					Color = PuzzleColor.Red;

				int spriteIndex = Properties.GetInteger("sprite_index");
				spriteIndex++;
				if (spriteIndex == TileData.SpriteList.Length)
					spriteIndex = 0;
				Properties.SetBase("sprite_index", spriteIndex);
				// TODO: Play color change sound
			}
		}

		public override void OnInitialize() {
			//Color = PuzzleColor.Red;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public PuzzleColor Color {
			get { return (PuzzleColor) SpriteIndex; }
			set { Properties.Set("sprite_index", (int) value); }
		}

	}
}
