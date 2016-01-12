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

		public RewardAmmo(string id, string ammoID, int amount, string message, SpriteAnimation animation) {
			InitAnimation(animation);

			this.id				= id;
			this.message		= message;
			this.hasDuration	= true;
			this.holdType		= RewardHoldTypes.Raise;
			this.isCollectibleWithItems	= true;
			this.onlyShowMessageInChest = true;

			this.ammoID			= ammoID;
			this.amount			= amount;
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void OnCollect(GameControl gameControl) {
			//AudioSystem.PlaySound("Pickups/get_ammo");

			gameControl.Inventory.GetAmmo(ammoID).Amount += amount;
		}
		
		public override bool IsAvailable(GameControl gameControl) {
			return gameControl.Inventory.IsAmmoAvailable(ammoID);
		}

		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

	}
}
