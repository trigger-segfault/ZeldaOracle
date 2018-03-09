using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Game.Control;
using ZeldaOracle.Game.Worlds;

namespace ZeldaOracle.Game.Items.Rewards {
	public class RewardDungeonItem : Reward {


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public RewardDungeonItem(string id, ISprite sprite, RewardHoldTypes holdType, string message) {
			InitSprite(sprite);

			this.id				= id;
			this.message		= message;
			this.holdType		= holdType;

			this.hasDuration	= false;
			this.isCollectibleWithItems	= (id != "small_key");
			this.onlyShowMessageInChest = (id == "small_key");

			if (id == "small_key" || id == "boss_key")
				this.soundBounce = GameData.SOUND_KEY_BOUNCE;
		}


		//-----------------------------------------------------------------------------
		// Virtual methods
		//-----------------------------------------------------------------------------

		public override void OnCollect(GameControl gameControl) {
			// TEMP: hard coded dungeon reward possibilities.
			Area area = gameControl.RoomControl.Area;
			
			if (id == "small_key") {
				area.SmallKeyCount++;
				AudioSystem.PlaySound(GameData.SOUND_GET_ITEM);
			}
			else if (id == "boss_key") {
				area.HasBossKey = true;
				AudioSystem.PlaySound(GameData.SOUND_GET_ITEM);
			}
			else if (id == "map") {
				area.HasMap = true;
			}
			else if (id == "compass") {
				area.HasCompass = true;
			}
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

	}
}
