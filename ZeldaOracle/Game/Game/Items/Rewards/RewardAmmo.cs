using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Control;

namespace ZeldaOracle.Game.Items.Rewards {
	public class RewardAmmo : Reward {

		protected string ammoID;
		protected int amount;

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public RewardAmmo(string id, string ammoID, int amount, string message, Sprite sprite) {
			this.id				= id;
			this.animation		= new Animation(sprite);
			this.message		= message;
			this.hasDuration	= true;
			this.holdType		= RewardHoldTypes.Raise;
			this.isCollectibleWithItems	= true;

			this.ammoID			= ammoID;
			this.amount			= amount;
		}
		public RewardAmmo(string id, string ammoID, int amount, string message, Animation animation) {
			this.id				= id;
			this.animation		= animation;
			this.message		= message;
			this.hasDuration	= true;
			this.holdType		= RewardHoldTypes.Raise;
			this.isCollectibleWithItems	= true;

			this.ammoID			= ammoID;
			this.amount			= amount;
		}

		//-----------------------------------------------------------------------------
		// Virtual methods
		//-----------------------------------------------------------------------------

		public override void OnCollect(GameControl gameControl) {
			//AudioSystem.PlaySound("Pickups/get_ammo");

			gameControl.Inventory.GetAmmo(ammoID).Amount += amount;
		}

		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

	}
}
