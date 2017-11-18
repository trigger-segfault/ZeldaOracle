using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Entities.Projectiles;

namespace ZeldaOracle.Game.Tiles {
	/*
	 * Track Index (and sprite index).
	 * 0 - horizontal
	 * 1 - vertical
	 * 2 - right/up
	 * 3 - up/left
	 * 4 - left/down
	 * 5 - down/right
	*/
	
	/**<summary>The orientations a minecart track can be in.</summary>*/
	public enum MinecartTrackOrientation {
		/**<summary>Track goes from left to right.</summary>*/
		Horizontal = 0,
		/**<summary>Track goes from top to bottom.</summary>*/
		Vertical,
		/**<summary>Track goes from top to right.</summary>*/
		UpRight,
		/**<summary>Track goes from top to left.</summary>*/
		UpLeft,
		/**<summary>Track goes from bottom to left.</summary>*/
		DownLeft,
		/**<summary>Track goes from bottom to right.</summary>*/
		DownRight
	}

	public static class MinecartTrackOrientationExtensions {
		public static bool HasDirection(this MinecartTrackOrientation orientation, int dir) {
			switch (dir) {
			case Directions.Right:
				return orientation == MinecartTrackOrientation.Horizontal ||
						orientation == MinecartTrackOrientation.UpRight ||
						orientation == MinecartTrackOrientation.DownRight;
			case Directions.Up:
				return orientation == MinecartTrackOrientation.Vertical ||
						orientation == MinecartTrackOrientation.UpRight ||
						orientation == MinecartTrackOrientation.UpLeft;
			case Directions.Left:
				return orientation == MinecartTrackOrientation.Horizontal ||
						orientation == MinecartTrackOrientation.UpLeft ||
						orientation == MinecartTrackOrientation.DownLeft;
			case Directions.Down:
				return orientation == MinecartTrackOrientation.Vertical ||
						orientation == MinecartTrackOrientation.DownRight ||
						orientation == MinecartTrackOrientation.DownLeft;
			}
			return false;
		}
	}

	public class TileMinecartTrack : Tile, ZeldaAPI.MinecartTrack {
		
		private MinecartTrackOrientation trackOrientation;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public TileMinecartTrack() {

		}
		

		//-----------------------------------------------------------------------------
		// Track methods
		//-----------------------------------------------------------------------------

		public IEnumerable<int> GetDirections() {
			for (int dir = 0; dir < 4; dir++) {
				if (trackOrientation.HasDirection(dir))
					yield return dir;
			}
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnInitialize() {
			trackOrientation = (MinecartTrackOrientation)Properties.GetInteger("track_orientation", 0);
			Graphics.PlaySpriteAnimation(SpriteList[(int)trackOrientation]);

			// Spawn a minecart entity.
			if (SpawnsMinecart) {
				Minecart minecart = new Minecart(this);
				RoomControl.SpawnEntity(minecart, Position);
			}
		}
		

		//-----------------------------------------------------------------------------
		// API Methods
		//-----------------------------------------------------------------------------

		public void SwitchTrackDirection() {
			MinecartTrackOrientation switchedOrientation = (MinecartTrackOrientation)Properties.GetInteger("switched_track_orientation", (int)trackOrientation);

			if (trackOrientation != switchedOrientation) {
				Properties.Set("switched_track_orientation", (int)trackOrientation);
				Properties.Set("track_orientation", (int)switchedOrientation);
				trackOrientation = switchedOrientation;
				Graphics.PlaySpriteAnimation(SpriteList[(int)trackOrientation]);
			}
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public bool IsHorizontal {
			get { return (trackOrientation == MinecartTrackOrientation.Horizontal); }
		}
		
		public bool IsVertical {
			get { return (trackOrientation == MinecartTrackOrientation.Vertical); }
		}

		public bool SpawnsMinecart {
			get { return Properties.GetBoolean("minecart", false); }
			set { Properties.Set("minecart", value); }
		}

		//-----------------------------------------------------------------------------
		// API Properties
		//-----------------------------------------------------------------------------

	}
}
