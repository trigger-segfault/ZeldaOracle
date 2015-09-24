using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Control;

namespace ZeldaOracle.Game.Items.Rewards {
	public class RewardRupee : Reward {

		protected int amount;

		//-----------------------------------------------------------------------------
		// Constants
		//-----------------------------------------------------------------------------

		public static readonly Rectangle2I SmallCollisionBox = new Rectangle2I();

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public RewardRupee(string id, int amount, Sprite sprite) {
			this.id				= id;
			this.duration		= 513;
			this.fadeTime		= 400;
			this.pickupableTime	= 12;
			this.animation		= new Animation(sprite);
			this.collisionBox	= new Rectangle2I(-6, -10, 9, 9);
			this.amount			= amount;
			this.isCollectibleWithItems	= true;
		}
		public RewardRupee(string id, int amount, Animation animation) {
			this.id				= id;
			this.duration		= 513;
			this.fadeTime		= 400;
			this.pickupableTime	= 12;
			this.animation		= animation;
			this.collisionBox	= new Rectangle2I(-6, -10, 9, 9);
			this.amount			= amount;
			this.isCollectibleWithItems	= true;
		}

		//-----------------------------------------------------------------------------
		// Virtual methods
		//-----------------------------------------------------------------------------

		public override void OnCollect(GameControl gameControl) {
			if (gameControl.HUD.DynamicRupees >= gameControl.Inventory.GetAmmo("rupees").MaxAmount)
				AudioSystem.PlaySound("Pickups/get_rupee");

			gameControl.Inventory.GetAmmo("rupees").Amount += amount;
		}

		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

	}
}
