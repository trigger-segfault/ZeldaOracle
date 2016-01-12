using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Control;
using ZeldaOracle.Game.Worlds;

namespace ZeldaOracle.Game.Items.Rewards {
	public class RewardDungeonItem : Reward {


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public RewardDungeonItem(string id, SpriteAnimation animation, RewardHoldTypes holdType, string message) {
			InitAnimation(animation);

			this.id				= id;
			this.message		= message;
			this.holdType		= holdType;

			this.hasDuration	= false;
			this.isCollectibleWithItems	= (id != "small_key");
			this.onlyShowMessageInChest = (id == "small_key");
		}


		//-----------------------------------------------------------------------------
		// Virtual methods
		//-----------------------------------------------------------------------------

		public override void OnCollect(GameControl gameControl) {
			// TEMP: hard coded dungeon reward possibilities.
			Dungeon dungeon = gameControl.RoomControl.Dungeon;

			if (dungeon != null) {
				if (id == "small_key")
					dungeon.NumSmallKeys++;
				else if (id == "boss_key")
					dungeon.HasBossKey = true;
				else if (id == "map")
					dungeon.HasMap = true;
				else if (id == "compass")
					dungeon.HasCompass = true;
			}
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

	}
}
