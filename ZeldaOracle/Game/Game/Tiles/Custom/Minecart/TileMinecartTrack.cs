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
	 * Sprite Index:
	 * 0 - horizontal
	 * 1 - vertical
	 * 2 - right/up
	 * 3 - up/left
	 * 4 - left/down
	 * 5 - down/right
	*/

	public class TileMinecartTrack : Tile, ZeldaAPI.MinecartTrack {

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public TileMinecartTrack() {

		}
		

		//-----------------------------------------------------------------------------
		// Track methods
		//-----------------------------------------------------------------------------

		public IEnumerable<int> GetDirections() {
			int spriteIndex = SpriteIndex;
			if (spriteIndex == 0 || spriteIndex == 2 || spriteIndex == 5)
				yield return Directions.Right;
			if (spriteIndex == 1 || spriteIndex == 2 || spriteIndex == 3)
				yield return Directions.Up;
			if (spriteIndex == 0 || spriteIndex == 3 || spriteIndex == 4)
				yield return Directions.Left;
			if (spriteIndex == 1 || spriteIndex == 4 || spriteIndex == 5)
				yield return Directions.Down;
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnInitialize() {
			if (Properties.GetBoolean("minecart", false)) {
				// Spawn a minecart entity.
				Minecart minecart = new Minecart(this);
				RoomControl.SpawnEntity(minecart, Position);
			}
		}
		

		//-----------------------------------------------------------------------------
		// API Methods
		//-----------------------------------------------------------------------------

		public void SwitchTrackDirection() {

		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public bool IsHorizontal {
			get { return (SpriteIndex == 0); }
		}
		
		public bool IsVertical {
			get { return (SpriteIndex == 1); }
		}
	}
}
