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
			animationPlayer = new AnimationPlayer();
		}


		//-----------------------------------------------------------------------------
		// Color Barrier Methods
		//-----------------------------------------------------------------------------

		public void Raise() {
			if (!isRaised) {
				isRaised	= true;
				IsSolid		= true;
				if (color == PuzzleColor.Blue)
					animationPlayer.Play(GameData.ANIM_TILE_COLOR_BARRIER_BLUE_RAISE);
				else
					animationPlayer.Play(GameData.ANIM_TILE_COLOR_BARRIER_RED_RAISE);

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
					animationPlayer.Play(GameData.ANIM_TILE_COLOR_BARRIER_BLUE_LOWER);
				else
					animationPlayer.Play(GameData.ANIM_TILE_COLOR_BARRIER_RED_LOWER);
			}
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnInitialize() {
			this.color		= Color;
			this.isRaised	= (color == PuzzleColor.Blue);

			Dungeon dungeon = RoomControl.Dungeon;
			if (dungeon != null)
				this.isRaised = (dungeon.ColorSwitchColor == color);

			this.IsSolid = isRaised;
			
			// Set the sprite.
			if (isRaised) {
				if (color == PuzzleColor.Blue)
					animationPlayer.Play(GameData.ANIM_TILE_COLOR_BARRIER_BLUE_RAISE);
				else
					animationPlayer.Play(GameData.ANIM_TILE_COLOR_BARRIER_RED_RAISE);
			}
			else {
				if (color == PuzzleColor.Blue)
					animationPlayer.Play(GameData.ANIM_TILE_COLOR_BARRIER_BLUE_LOWER);
				else
					animationPlayer.Play(GameData.ANIM_TILE_COLOR_BARRIER_RED_LOWER);
			}

			animationPlayer.PlaybackTime = animationPlayer.Animation.Duration;
		}

		public override void UpdateGraphics() {
			animationPlayer.Update();
			base.UpdateGraphics();
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public PuzzleColor Color {
			get { return (PuzzleColor) Properties.Get("color", (int) PuzzleColor.Red); }
		}

		public bool IsRaised {
			get { return isRaised; }
		}
	}
}
