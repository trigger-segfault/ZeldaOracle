using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Game.Worlds;

namespace ZeldaOracle.Game.Items.Rewards.Dungeon {
	public class RewardDungeonSmallKey : Reward {

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------
		
		public RewardDungeonSmallKey() {
		}


		//-----------------------------------------------------------------------------
		// Static Methods
		//-----------------------------------------------------------------------------

		/// <summary>Initializes the properties for the reward type.</summary>
		public static void InitializeRewardData(RewardData data) {
			data.HoldInChest		= false;
			data.HoldType			= RewardHoldTypes.OneHand;
			data.HasDuration		= false;
			data.ShowPickupMessage	= false;
			data.WeaponInteract		= true;
			data.BounceSound		= GameData.SOUND_KEY_BOUNCE;
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
