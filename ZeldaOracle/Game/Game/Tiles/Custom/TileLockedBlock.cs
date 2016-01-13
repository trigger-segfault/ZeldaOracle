using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Entities.Effects;
using ZeldaOracle.Game.Entities.Projectiles;
using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.Worlds;

namespace ZeldaOracle.Game.Tiles {

	public class TileLockedBlock : Tile {
		
		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public TileLockedBlock() {
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override bool OnPush(int direction, float movementSpeed) {
			Dungeon dungeon = RoomControl.Dungeon;

			// Check if we have a small key to use.
			if (dungeon.NumSmallKeys > 0) {
				dungeon.NumSmallKeys--;

				// Spawn the key and poof effects.
				RoomControl.SpawnEntity(new EffectUsedItem(GameData.SPR_REWARD_SMALL_KEY), Center);
				RoomControl.SpawnEntity(new Effect(GameData.ANIM_EFFECT_BLOCK_POOF, DepthLayer.EffectSomariaBlockPoof), Center);

				// Destroy the tile forever.
				BaseProperties.Set("enabled", false);
				RoomControl.RemoveTile(this);

				return true;
			}
			else {
				GameControl.DisplayMessage("You need a <red>key<red> for this block!");
			}

			return false;
		}
	}
}
