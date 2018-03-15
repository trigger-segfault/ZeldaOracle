using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Game.Worlds;

namespace ZeldaOracle.Game.Items.Rewards {
	public class RewardDungeonCompass : Reward {

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public RewardDungeonCompass(string id) : base(id) {
			HoldType        = RewardHoldTypes.TwoHands;
			HasDuration     = false;
			ShowMessageOnPickup = true;
			InteractWithWeapons = false;
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
