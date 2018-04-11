using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Entities.Projectiles;
using ZeldaOracle.Game.Worlds;

namespace ZeldaOracle.Game.Tiles {

	public class TileColorBarrier : Tile, IColoredTile {

		private bool isRaised;
		private PuzzleColor color;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public TileColorBarrier() {
			Graphics.IsAnimatedWhenPaused = true;
			Graphics.SyncPlaybackWithRoomTicks = false;
		}


		//-----------------------------------------------------------------------------
		// Color Barrier Methods
		//-----------------------------------------------------------------------------

		public void Raise() {
			if (!isRaised) {
				isRaised	= true;
				IsSolid		= true;
				/*if (color == PuzzleColor.Blue)
					Graphics.PlayAnimation(GameData.ANIM_TILE_COLOR_BARRIER_BLUE_RAISE);
				else
					Graphics.PlayAnimation(GameData.ANIM_TILE_COLOR_BARRIER_RED_RAISE);*/
				Graphics.PlayAnimation(SpriteList[2]);

				// Break any blocks on top of this tile.
				foreach (Tile tile in RoomControl.TileManager.GetTilesAtLocation(Location)) {
					if (tile.Layer > Layer)
						tile.Break(false);
				}
			}
		}
		
		public void Lower() {
			if (isRaised) {
				isRaised	= false;
				IsSolid		= false;
				Graphics.PlayAnimation(SpriteList[3]);
				/*if (color == PuzzleColor.Blue)
					Graphics.PlayAnimation(GameData.ANIM_TILE_COLOR_BARRIER_BLUE_LOWER);
				else
					Graphics.PlayAnimation(GameData.ANIM_TILE_COLOR_BARRIER_RED_LOWER);*/
			}
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnInitialize() {
			this.color		= Color;
			this.isRaised	= (color == PuzzleColor.Blue);

			Area dungeon = RoomControl.Area;
			if (dungeon != null)
				this.isRaised = (dungeon.ColorSwitchColor == color);

			this.IsSolid = isRaised;

			// Set the sprite
			Graphics.PlayAnimation(SpriteList[isRaised ? 2 : 3]);
			/*if (isRaised) {
				if (color == PuzzleColor.Blue)
					Graphics.PlayAnimation(GameData.ANIM_TILE_COLOR_BARRIER_BLUE_RAISE);
				else
					Graphics.PlayAnimation(GameData.ANIM_TILE_COLOR_BARRIER_RED_RAISE);
			}
			else {
				if (color == PuzzleColor.Blue)
					Graphics.PlayAnimation(GameData.ANIM_TILE_COLOR_BARRIER_BLUE_LOWER);
				else
					Graphics.PlayAnimation(GameData.ANIM_TILE_COLOR_BARRIER_RED_LOWER);
			}*/

			Graphics.AnimationPlayer.SkipToEnd();
		}


		//-----------------------------------------------------------------------------
		// Static Methods
		//-----------------------------------------------------------------------------

		/// <summary>Draws the tile data to display in the editor.</summary>
		public new static void DrawTileData(Graphics2D g, TileDataDrawArgs args) {
			PuzzleColor color = args.Properties.GetEnum("color", PuzzleColor.Red);
			bool isRaised = (color == PuzzleColor.Blue);
			Area area = args.Room?.Area ?? args.Level?.Area;
			if (area != null)
				isRaised = (color == area.ColorSwitchColor);
			Tile.DrawTileDataIndex(g, args, isRaised ? 0 : 1);
			/*ISprite sprite = null;
			if (color == PuzzleColor.Red)
				sprite = GameData.SPR_TILE_COLOR_JUMP_PAD_RED;
			else if (color == PuzzleColor.Yellow)
				sprite = GameData.SPR_TILE_COLOR_JUMP_PAD_YELLOW;
			else if (color == PuzzleColor.Blue)
				sprite = GameData.SPR_TILE_COLOR_JUMP_PAD_BLUE;
			if (sprite != null) {
				g.DrawSprite(
					sprite,
					args.SpriteSettings,
					args.Position,
					args.Color);
			}*/
		}

		/// <summary>Initializes the properties and events for the tile type.</summary>
		public static void InitializeTileData(TileData data) {
			data.Properties.SetEnumInt("color", PuzzleColor.Red)
				.SetDocumentation("Color", "enum", typeof(PuzzleColor), "Color", "The color of the barrier (red or blue).");
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public PuzzleColor Color {
			get { return Properties.GetEnum("color", PuzzleColor.Red); }
		}

		public bool IsRaised {
			get { return isRaised; }
		}
	}
}
