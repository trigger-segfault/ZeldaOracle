using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.API;
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
				if (color == PuzzleColor.Blue)
					Graphics.PlayAnimation(GameData.ANIM_TILE_COLOR_BARRIER_BLUE_RAISE);
				else
					Graphics.PlayAnimation(GameData.ANIM_TILE_COLOR_BARRIER_RED_RAISE);

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
				if (color == PuzzleColor.Blue)
					Graphics.PlayAnimation(GameData.ANIM_TILE_COLOR_BARRIER_BLUE_LOWER);
				else
					Graphics.PlayAnimation(GameData.ANIM_TILE_COLOR_BARRIER_RED_LOWER);
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
			
			// Set the sprite.
			if (isRaised) {
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
			}

			Graphics.AnimationPlayer.SkipToEnd();
		}


		//-----------------------------------------------------------------------------
		// Static Methods
		//-----------------------------------------------------------------------------

		/// <summary>Initializes the properties and events for the tile type.</summary>
		public static void InitializeTileData(TileData data) {
			data.Properties.SetEnumInt("color", PuzzleColor.Red)
				.SetDocumentation("Color", "enum", typeof(PuzzleColor), "Color", "The color of the barrier (red or blue).");
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public PuzzleColor Color {
			get { return Properties.GetEnum<PuzzleColor>("color", PuzzleColor.Red); }
		}

		public bool IsRaised {
			get { return isRaised; }
		}
	}
}
