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
using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.Entities.Projectiles;

namespace ZeldaOracle.Game.Tiles {

	public class TileColorJumpPad : Tile, IColoredTile, ZeldaAPI.ColorJumpPad {

		private static List<TileColorJumpPad> markedTiles = new List<TileColorJumpPad>();
		private static Point2I playerJumpStartTileLocation;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public TileColorJumpPad() {

		}


		//-----------------------------------------------------------------------------
		// Color Methods
		//-----------------------------------------------------------------------------

		private void CycleColor() {
			// Cycle the color (red -> yellow -> blue)
			PuzzleColor color = (PuzzleColor) Properties.Get("color", (int) PuzzleColor.Red);
			if (color == PuzzleColor.Red)
				color = PuzzleColor.Yellow;
			else if (color == PuzzleColor.Yellow)
				color = PuzzleColor.Blue;
			else if (color == PuzzleColor.Blue)
 				color = PuzzleColor.Red;

			// Set the color property.
			Properties.Set("color", (int) color);

			// Set the sprite.
			if (color == PuzzleColor.Red)
				Graphics.PlayAnimation(GameData.SPR_TILE_COLOR_JUMP_PAD_RED);
			else if (color == PuzzleColor.Yellow)
				Graphics.PlayAnimation(GameData.SPR_TILE_COLOR_JUMP_PAD_YELLOW);
			else if (color == PuzzleColor.Blue)
				Graphics.PlayAnimation(GameData.SPR_TILE_COLOR_JUMP_PAD_BLUE);
				
			AudioSystem.PlaySound(GameData.SOUND_GET_ITEM);

			GameControl.FireEvent(this, "color_change", this, ((ZeldaAPI.ColorJumpPad) this).Color);
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnInitialize() {
			// Set the sprite.
			PuzzleColor color = (PuzzleColor) Properties.Get("color", (int) PuzzleColor.Red);
			if (color == PuzzleColor.Red)
				Graphics.PlayAnimation(GameData.SPR_TILE_COLOR_JUMP_PAD_RED);
			else if (color == PuzzleColor.Yellow)
				Graphics.PlayAnimation(GameData.SPR_TILE_COLOR_JUMP_PAD_YELLOW);
			else if (color == PuzzleColor.Blue)
				Graphics.PlayAnimation(GameData.SPR_TILE_COLOR_JUMP_PAD_BLUE);
			
			// NOTE: The player automaticlly nullifies his events upon leaving the room.
			RoomControl.Player.EventJump += OnPlayerJump;
			RoomControl.Player.EventLand += OnPlayerLand;
		}

		private void OnPlayerJump(Player player) {
			// Remember the players location upon jumping.
			playerJumpStartTileLocation = RoomControl.GetTileLocation(player.Position);
		}
		
		private void OnPlayerLand(Player player) {
			Point2I playerTileLocation = RoomControl.GetTileLocation(player.Position);

			// Toggle all tiles that were jumped over (except the one landed on).
			if (playerJumpStartTileLocation != playerTileLocation) {
				foreach (TileColorJumpPad tile in markedTiles) {
					if (tile.Location != playerJumpStartTileLocation)
						tile.CycleColor();
				}
				markedTiles.Clear();
			}
		}

		public override void Update() {
			base.Update();

			// Mark jump pad tiles that are jumped over.
			Player player = RoomControl.Player;
			if (player.IsInAir && RoomControl.GetTileLocation(player.Position) == Location && !markedTiles.Contains(this)) {
				markedTiles.Add(this);
			}
		}


		//-----------------------------------------------------------------------------
		// Static Methods
		//-----------------------------------------------------------------------------

		/// <summary>Draws the tile data to display in the editor.</summary>
		public new static void DrawTileData(Graphics2D g, TileDataDrawArgs args) {
			PuzzleColor tileColor = args.Properties.GetEnum<PuzzleColor>("color", PuzzleColor.Red);
			ISprite sprite = null;
			if (tileColor == PuzzleColor.Red)
				sprite = GameData.SPR_TILE_COLOR_JUMP_PAD_RED;
			else if (tileColor == PuzzleColor.Yellow)
				sprite = GameData.SPR_TILE_COLOR_JUMP_PAD_YELLOW;
			else if (tileColor == PuzzleColor.Blue)
				sprite = GameData.SPR_TILE_COLOR_JUMP_PAD_BLUE;
			if (sprite != null) {
				g.DrawISprite(
					sprite,
					args.SpriteDrawSettings,
					args.Position,
					args.Color);
			}
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public PuzzleColor Color {
			get { return Properties.GetEnum<PuzzleColor>("color", PuzzleColor.Red); }
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
