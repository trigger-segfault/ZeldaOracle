using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Input;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Entities.Effects;
using ZeldaOracle.Game.Entities.Projectiles;
using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.Worlds;

namespace ZeldaOracle.Game.Tiles {

	public class TileBossKeyDoor : TileDoor {


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public TileBossKeyDoor() {
			animationOpen	= GameData.ANIM_TILE_BOSS_DOOR_OPEN;
			animationClose	= GameData.ANIM_TILE_BOSS_DOOR_CLOSE;
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override bool OnPush(int direction, float movementSpeed) {
			Dungeon dungeon = RoomControl.Dungeon;

			if (dungeon.HasBossKey) {
				Open();
				
				// Spawn the key effect.
				EffectUsedItem effect = new EffectUsedItem(GameData.SPR_REWARD_BOSS_KEY);
				RoomControl.SpawnEntity(effect, Center);
				
				// Disable this tile forever.
				BaseProperties.Set("enabled", false);
				
				// Unlock doors connected to this one in the adjacent room.
				TileDataInstance connectedDoor = GetConnectedDoor();
				if (connectedDoor != null) {
					connectedDoor.Properties.Set("enabled", false);
				}
				return true;
			}
			else {
				GameControl.DisplayMessage("This keyhole is different!");
			}

			return false;
		}
	}
}
