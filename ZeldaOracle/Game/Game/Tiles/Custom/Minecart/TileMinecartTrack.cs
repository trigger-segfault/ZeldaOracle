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

	public class TileMinecartTrack : Tile, ZeldaAPI.MinecartTrack {
		
		private int trackIndex;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public TileMinecartTrack() {

		}
		

		//-----------------------------------------------------------------------------
		// Track methods
		//-----------------------------------------------------------------------------

		public IEnumerable<int> GetDirections() {
			if (trackIndex == 0 || trackIndex == 2 || trackIndex == 5)
				yield return Directions.Right;
			if (trackIndex == 1 || trackIndex == 2 || trackIndex == 3)
				yield return Directions.Up;
			if (trackIndex == 0 || trackIndex == 3 || trackIndex == 4)
				yield return Directions.Left;
			if (trackIndex == 1 || trackIndex == 4 || trackIndex == 5)
				yield return Directions.Down;
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnInitialize() {
			trackIndex = Properties.GetInteger("track_index", 0);
			Graphics.PlaySprite(SpriteList[trackIndex]);

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
			int switchedIndex = Properties.GetInteger("switched_track_index", trackIndex);

			if (trackIndex != switchedIndex) {
				Properties.Set("switched_track_index", trackIndex);
				Properties.Set("track_index", switchedIndex);
				trackIndex = switchedIndex;
				Graphics.PlaySprite(SpriteList[trackIndex]);
			}
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public bool IsHorizontal {
			get { return (trackIndex == 0); }
		}
		
		public bool IsVertical {
			get { return (trackIndex == 1); }
		}

		public bool SpawnsMinecart {
			get { return Properties.GetBoolean("minecart", false); }
			set { Properties.Set("minecart", value); }
		}
	}
}
