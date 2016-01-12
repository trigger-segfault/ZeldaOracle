using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Control;

namespace ZeldaOracle.Game.Items.Rewards {
	public class RewardRecoveryHeart : Reward {

		protected int amount;

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public RewardRecoveryHeart(string id, int amount, string message, Sprite sprite) {
			InitSprite(sprite);

			this.id				= id;
			this.message		= message;
			this.hasDuration	= true;
			this.holdType		= RewardHoldTypes.Raise;
			this.isCollectibleWithItems	= true;
			this.onlyShowMessageInChest = true;

			this.amount			= amount;
		}

		//-----------------------------------------------------------------------------
		// Virtual methods
		//-----------------------------------------------------------------------------

		public override void OnCollect(GameControl gameControl) {
			if (gameControl.HUD.DynamicHealth >= gameControl.Player.MaxHealth)
				AudioSystem.PlaySound("Pickups/get_heart");

			gameControl.Player.Health += amount * 4;
		}

		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

	}
}
