using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Entities.Projectiles;

namespace ZeldaOracle.Game.Tiles {

	public class TileColorTile : Tile, IColoredTile, ZeldaAPI.ColorTile {

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public TileColorTile() {

		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnInitialize() {
			// Set the sprite.
			/*PuzzleColor color = Color;
			if (color == PuzzleColor.Red)
				Graphics.PlayAnimation(GameData.SPR_TILE_COLOR_TILE_RED);
			else if (color == PuzzleColor.Yellow)
				Graphics.PlayAnimation(GameData.SPR_TILE_COLOR_TILE_YELLOW);
			else if (color == PuzzleColor.Blue)
				Graphics.PlayAnimation(GameData.SPR_TILE_COLOR_TILE_BLUE);*/
			Graphics.PlayAnimation(SpriteList[(int) Color]);
		}


		//-----------------------------------------------------------------------------
		// Static Methods
		//-----------------------------------------------------------------------------

		/// <summary>Draws the tile data to display in the editor.</summary>
		public new static void DrawTileData(Graphics2D g, TileDataDrawArgs args) {
			PuzzleColor tileColor = args.Properties.GetEnum("color", PuzzleColor.Red);
			/*ISprite sprite = null;
			if (tileColor == PuzzleColor.Red)
				sprite = GameData.SPR_TILE_COLOR_TILE_RED;
			else if (tileColor == PuzzleColor.Yellow)
				sprite = GameData.SPR_TILE_COLOR_TILE_YELLOW;
			else if (tileColor == PuzzleColor.Blue)
				sprite = GameData.SPR_TILE_COLOR_TILE_BLUE;
			if (sprite != null) {
				g.DrawSprite(
					sprite,
					args.SpriteSettings,
					args.Position,
					args.Color);
			}*/
			Tile.DrawTileDataIndex(g, args, (int) tileColor);
		}

		/// <summary>Initializes the properties and events for the tile type.</summary>
		public static void InitializeTileData(TileData data) {
			data.Properties.SetEnumInt("color", PuzzleColor.Red)
				.SetDocumentation("Color", "enum", typeof(PuzzleColor), "Color", "The color of the tile.");
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public PuzzleColor Color {
			get { return Properties.GetEnum("color", PuzzleColor.Red); }
			set {
				//bool changed = (value != Color);
				Properties.SetEnum("color", value);
				//if (changed)
					//GameControl.FireEvent(this, "color_change", this, ((ZeldaAPI.ColorTile) this).Color);
			}
		}
		
		
		//-----------------------------------------------------------------------------
		// API Implementations
		//-----------------------------------------------------------------------------

		/*ZeldaAPI.Color ZeldaAPI.ColorTile.Color {
			get { return (ZeldaAPI.Color) Color; }
			set { Color = (PuzzleColor) value; }
		}*/
	}
}
