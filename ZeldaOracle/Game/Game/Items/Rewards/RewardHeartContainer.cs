using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Control;

namespace ZeldaOracle.Game.Items.Rewards {
	public class RewardHeartContainer : Reward {

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public RewardHeartContainer() : base("heart_container") {
			Sprite			= GameData.SPR_REWARD_HEART_CONTAINER;
			Message			= "You got a <red>Heart Container<red>!";
			HoldType		= RewardHoldTypes.TwoHands;
			HasDuration		= false;
			ShowMessageOnPickup			= true;
			InteractWithWeapons	= false;
		}
		
		public RewardHeartContainer(string id) : base(id) {
			Sprite			= GameData.SPR_REWARD_HEART_CONTAINER;
			Message			= "You got a <red>Heart Container<red>!";
			HoldType		= RewardHoldTypes.TwoHands;
			HasDuration		= false;
			ShowMessageOnPickup			= true;
			InteractWithWeapons	= false;
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
		// Internal Methods
		//-----------------------------------------------------------------------------

		private void IncrementHeartContainers() {
			Player.MaxHealth += 4;
			Player.Health = Player.MaxHealth;
		}
	}
}
