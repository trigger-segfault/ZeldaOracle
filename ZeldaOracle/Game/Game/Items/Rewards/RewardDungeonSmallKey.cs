using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Game.Worlds;

namespace ZeldaOracle.Game.Items.Rewards {
	public class RewardDungeonSmallKey : Reward {

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public RewardDungeonSmallKey(string id) : base(id) {
			HoldType		= RewardHoldTypes.OneHand;
			HasDuration		= false;
			ShowMessageOnPickup	= false;
			InteractWithWeapons = true;
			BounceSound		= GameData.SOUND_KEY_BOUNCE;
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		/// <summary>Called when the player collects the reward.</summary>
		public override void OnCollect() {
			Area area = RoomControl.Area;
			
			area.SmallKeyCount++;
			AudioSystem.PlaySound(GameData.SOUND_GET_ITEM);
		}
	}
}
