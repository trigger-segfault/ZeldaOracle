using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Entities.Projectiles;

namespace ZeldaOracle.Game.Tiles {

	public class TileColorJumpPad : Tile, ZeldaAPI.ColorJumpPad {

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
				PuzzleColor color = (PuzzleColor) Properties.Get("color", (int) PuzzleColor.Red);
				if (color == PuzzleColor.Red)
					color = PuzzleColor.Yellow;
				else if (color == PuzzleColor.Yellow)
					color = PuzzleColor.Blue;
				else if (color == PuzzleColor.Blue)
 					color = PuzzleColor.Red;

				// Set the color property.
				if (Properties.Get("remember_state", false))
					Properties.SetBase("color", (int) color);
				else
					Properties.Set("color", (int) color);

				// Set the sprite.
				if (color == PuzzleColor.Red)
					CustomSprite = GameData.SPR_TILE_COLOR_JUMP_PAD_RED;
				else if (color == PuzzleColor.Yellow)
					CustomSprite = GameData.SPR_TILE_COLOR_JUMP_PAD_YELLOW;
				else if (color == PuzzleColor.Blue)
					CustomSprite = GameData.SPR_TILE_COLOR_JUMP_PAD_BLUE;
				
				AudioSystem.PlaySound(GameData.SOUND_GET_ITEM);

				GameControl.FireEvent(this, "event_color_change", this, ((ZeldaAPI.ColorJumpPad) this).Color);
			}
		}

		public override void OnInitialize() {
			// Set the sprite.
			PuzzleColor color = (PuzzleColor) Properties.Get("color", (int) PuzzleColor.Red);
			if (color == PuzzleColor.Red)
				CustomSprite = GameData.SPR_TILE_COLOR_JUMP_PAD_RED;
			else if (color == PuzzleColor.Yellow)
				CustomSprite = GameData.SPR_TILE_COLOR_JUMP_PAD_YELLOW;
			else if (color == PuzzleColor.Blue)
				CustomSprite = GameData.SPR_TILE_COLOR_JUMP_PAD_BLUE;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public PuzzleColor Color {
			get { return (PuzzleColor) Properties.Get("color", (int) PuzzleColor.Red); }
			set { Properties.Set("color", (int) value); }
		}
		

		//-----------------------------------------------------------------------------
		// API Implementations
		//-----------------------------------------------------------------------------

		ZeldaAPI.Color ZeldaAPI.ColorJumpPad.Color {
			get { return (ZeldaAPI.Color) Color; }
			set { Color = (PuzzleColor) value; }
		}
	}
}
