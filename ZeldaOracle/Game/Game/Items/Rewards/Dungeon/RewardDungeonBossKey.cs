using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Game.Worlds;

namespace ZeldaOracle.Game.Items.Rewards.Dungeon {
	public class RewardDungeonBossKey : Reward {

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public RewardDungeonBossKey() {
		}


		//-----------------------------------------------------------------------------
		// Static Methods
		//-----------------------------------------------------------------------------

		/// <summary>Initializes the properties for the reward type.</summary>
		public static void InitializeRewardData(RewardData data) {
			data.HoldInChest		= true;
			data.HoldType			= RewardHoldTypes.OneHand;
			data.HasDuration		= false;
			data.ShowPickupMessage	= true;
			data.WeaponInteract		= false;
			data.BounceSound		= GameData.SOUND_KEY_BOUNCE;
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		/// <summary>Called when the player collects the reward.</summary>
		public override void OnCollect() {
			Area area = RoomControl.Area;

			area.HasBossKey = true;
			AudioSystem.PlaySound(GameData.SOUND_GET_ITEM);
		}
	}
}
