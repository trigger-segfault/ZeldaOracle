using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Control;

namespace ZeldaOracle.Game.Items.Rewards {
	public class RewardItem : Reward {

		protected string itemID;
		protected int level;

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public RewardItem(string id, string itemID, int level, RewardHoldTypes holdType, string message, Sprite sprite) {
			this.id				= id;
			this.animation		= new Animation(sprite);
			this.message		= message;
			this.hasDuration	= true;
			this.holdType		= holdType;
			this.isCollectibleWithItems	= true;

			this.itemID			= itemID;
			this.level			= level;
		}
		public RewardItem(string id, string itemID, int level, RewardHoldTypes holdType, string message, Animation animation) {
			this.id				= id;
			this.animation		= animation;
			this.message		= message;
			this.hasDuration	= true;
			this.holdType		= holdType;
			this.isCollectibleWithItems	= true;

			this.itemID			= itemID;
			this.level			= level;
		}

		//-----------------------------------------------------------------------------
		// Virtual methods
		//-----------------------------------------------------------------------------

		public override void OnCollect(GameControl gameControl) {
			//AudioSystem.PlaySound("Pickups/get_ammo");

			gameControl.Inventory.ObtainItem(gameControl.Inventory.GetItem(itemID));
			gameControl.Inventory.GetItem(itemID).Level = level;
		}

		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

	}
}
