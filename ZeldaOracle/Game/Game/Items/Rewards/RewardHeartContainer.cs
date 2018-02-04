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

		public RewardHeartContainer() {
			InitSprite(GameData.SPR_REWARD_HEART_CONTAINER);

			this.id				= "heart_container";
			this.message		= "You got a <red>Heart Container<red>!";
			this.hasDuration	= false;
			this.holdType		= RewardHoldTypes.TwoHands;
			this.isCollectibleWithItems	= false;
		}
		

		//-----------------------------------------------------------------------------
		// Virtual methods
		//-----------------------------------------------------------------------------

		public override void OnDisplayMessage(GameControl gameControl) {
			IncrementHeartContainers(gameControl);
			gameControl.DisplayMessage(message);
		}

		public override void OnCollectNoMessage(GameControl gameControl) {
			IncrementHeartContainers(gameControl);
		}


		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------

		private void IncrementHeartContainers(GameControl gameControl) {
			gameControl.Player.MaxHealth += 4;
			gameControl.Player.Health = gameControl.Player.MaxHealth;
		}
	}
}
