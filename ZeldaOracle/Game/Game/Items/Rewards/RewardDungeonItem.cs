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

		public RewardDungeonItem(string id, ISprite sprite, RewardHoldTypes holdType,
			string message) : base(id)
		{
			Sprite			= sprite;
			Message			= message;
			HoldType		= holdType;
			HasDuration		= false;
			ShowMessageOnPickup			= (id != "small_key");
			InteractWithWeapons	= (id != "small_key");

			if (id == "small_key" || id == "boss_key")
				BounceSound = GameData.SOUND_KEY_BOUNCE;
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		/// <summary>Called when the player collects the reward.</summary>
		public override void OnCollect() {
			// TEMP: hard coded dungeon reward possibilities.
			Area area = RoomControl.Area;
			
			if (ID == "small_key") {
				area.SmallKeyCount++;
				AudioSystem.PlaySound(GameData.SOUND_GET_ITEM);
			}
			else if (ID == "boss_key") {
				area.HasBossKey = true;
				AudioSystem.PlaySound(GameData.SOUND_GET_ITEM);
			}
			else if (ID == "map") {
				area.HasMap = true;
			}
			else if (ID == "compass") {
				area.HasCompass = true;
			}
		}
	}
}
