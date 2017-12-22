using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaAPI;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Entities.Projectiles;

namespace ZeldaOracle.Game.Tiles {

	public class TileColorLantern : Tile, IColoredTile, ZeldaAPI.ColorLantern {

		private Animation flameAnimation;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public TileColorLantern() {
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnInitialize() {
			PuzzleColor color = Color;

			if (color == PuzzleColor.Red)
				flameAnimation = GameData.ANIM_EFFECT_COLOR_FLAME_RED;
			else if (color == PuzzleColor.Blue)
				flameAnimation = GameData.ANIM_EFFECT_COLOR_FLAME_BLUE;
			else if (color == PuzzleColor.Yellow)
				flameAnimation = GameData.ANIM_EFFECT_COLOR_FLAME_YELLOW;
			else
				flameAnimation = null;
		}

		public override void Draw(RoomGraphics g) {
			base.Draw(g);

			if (flameAnimation != null) {
				g.DrawISprite(flameAnimation, new SpriteDrawSettings((float) GameControl.RoomTicks),
					Center - new Vector2F(0, 9), Graphics.DepthLayer);
			}
		}


		//-----------------------------------------------------------------------------
		// Static Methods
		//-----------------------------------------------------------------------------

		/// <summary>Draws the tile data to display in the editor.</summary>
		public new static void DrawTileData(Graphics2D g, TileDataDrawArgs args) {
			Tile.DrawTileData(g, args);
			PuzzleColor color = args.Properties.GetEnum<PuzzleColor>("color", PuzzleColor.None);
			ISprite flameAnimation = null;
			if (color == PuzzleColor.Red)
				flameAnimation = GameData.ANIM_EFFECT_COLOR_FLAME_RED;
			else if (color == PuzzleColor.Blue)
				flameAnimation = GameData.ANIM_EFFECT_COLOR_FLAME_BLUE;
			else if (color == PuzzleColor.Yellow)
				flameAnimation = GameData.ANIM_EFFECT_COLOR_FLAME_YELLOW;
			if (flameAnimation != null) {
				g.DrawISprite(
					flameAnimation,
					args.SpriteDrawSettings,
					args.Position + new Point2I(8, -1),
					args.Color);
			}
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public PuzzleColor Color {
			get { return Properties.GetEnum<PuzzleColor>("color", PuzzleColor.None); }
			set {
				PuzzleColor prevColor = Color;
				Properties.Set("color", (int) value);

				// Fire the color change event.
				if (prevColor != value) {
					GameControl.FireEvent(this, "color_change", this, (ZeldaAPI.Color) value);
				}

				if (value == PuzzleColor.Red)
					flameAnimation = GameData.ANIM_EFFECT_COLOR_FLAME_RED;
				else if (value == PuzzleColor.Blue)
					flameAnimation = GameData.ANIM_EFFECT_COLOR_FLAME_BLUE;
				else if (value == PuzzleColor.Yellow)
					flameAnimation = GameData.ANIM_EFFECT_COLOR_FLAME_YELLOW;
				else
					flameAnimation = null;
				
				if (prevColor == PuzzleColor.None && value != PuzzleColor.None)
					AudioSystem.PlaySound(GameData.SOUND_FIRE);
			}
		}
		

		//-----------------------------------------------------------------------------
		// Scripting API
		//-----------------------------------------------------------------------------

		ZeldaAPI.Color ZeldaAPI.ColorLantern.Color {
			get { return (ZeldaAPI.Color) Color; }
			set { Color = (PuzzleColor) value; }
		}
	}
}
