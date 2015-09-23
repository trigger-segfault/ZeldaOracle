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

		public RewardRupee(string id, int amount, Sprite sprite, Rectangle2I collisionBox) {
			this.id				= id;
			this.duration		= 513;
			this.fadeDuration	= 400;
			this.animation		= new Animation(sprite);
			this.collisionBox	= collisionBox;
			this.amount			= amount;
			this.isCollectibleWithItems	= true;
		}
		public RewardRupee(string id, int amount, Animation animation, Rectangle2I collisionBox) {
			this.id				= id;
			this.duration		= 513;
			this.fadeDuration	= 400;
			this.animation		= animation;
			this.collisionBox	= collisionBox;
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
