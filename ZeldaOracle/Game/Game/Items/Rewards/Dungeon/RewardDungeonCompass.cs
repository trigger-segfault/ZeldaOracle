using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Game.Worlds;

namespace ZeldaOracle.Game.Items.Rewards.Dungeon {
	public class RewardDungeonCompass : Reward {

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public RewardDungeonCompass() {
		}


		//-----------------------------------------------------------------------------
		// Static Methods
		//-----------------------------------------------------------------------------

		/// <summary>Initializes the properties for the reward type.</summary>
		public static void InitializeRewardData(RewardData data) {
			data.HoldInChest		= true;
			data.HoldType			= RewardHoldTypes.TwoHands;
			data.HasDuration		= false;
			data.ShowPickupMessage	= true;
			data.WeaponInteract		= false;
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		/// <summary>Called when the player collects the reward.</summary>
		public override void OnCollect() {
			Area area = RoomControl.Area;

			area.HasCompass = true;
		}
	}
}
