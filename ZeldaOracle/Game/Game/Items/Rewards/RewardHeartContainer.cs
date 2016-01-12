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
			this.id				= "heart_container";
			this.animation		= new Animation(GameData.SPR_REWARD_HEART_CONTAINER);
			this.message		= "You got a <red>Heart Container<red>!";
			this.hasDuration	= false;
			this.holdType		= RewardHoldTypes.TwoHands;
			this.isCollectibleWithItems	= false;
		}

		//-----------------------------------------------------------------------------
		// Virtual methods
		//-----------------------------------------------------------------------------

		public override void OnCollect(GameControl gameControl) {
			gameControl.Inventory.PiecesOfHeart++;
			if (gameControl.Inventory.PiecesOfHeart == 4) {
				gameControl.Inventory.PiecesOfHeart = 0;
				gameControl.Player.MaxHealth += 4;
				gameControl.Player.Health += 4;
			}
		}
	}
}
