using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Control;

namespace ZeldaOracle.Game.Items.Rewards.Health {
	public class RewardHeartContainer : Reward {

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public RewardHeartContainer() {
		}
		

		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void OnDisplayMessage() {
			IncrementHeartContainers();
			GameControl.DisplayMessage(Message);
		}

		public override void OnCollectNoMessage() {
			IncrementHeartContainers();
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
		// Internal Methods
		//-----------------------------------------------------------------------------

		private void IncrementHeartContainers() {
			Player.MaxHealth += 4;
			Player.Health = Player.MaxHealth;
		}
	}
}
