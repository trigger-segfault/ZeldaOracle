using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Items.Rewards;

namespace ZeldaOracle.Game.Entities {
	public class Collectible : Entity {

		protected int timer;
		protected int pickupableDelay;
		protected int aliveDuration;
		protected int fadeDelay;
		protected bool hasDuration;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public Collectible() {
			this.timer			= 0;
			this.hasDuration	= true;

			// Physics.
			Physics.CollisionBox		= new Rectangle2I(-2, -2, 4, 4);
			Physics.SoftCollisionBox	= new Rectangle2I(-2, -2, 4, 4);

			aliveDuration	= GameSettings.COLLECTIBLE_ALIVE_DURATION;
			fadeDelay		= GameSettings.COLLECTIBLE_FADE_DELAY;
			pickupableDelay	= GameSettings.COLLECTIBLE_PICKUPABLE_DELAY;
		}


		//-----------------------------------------------------------------------------
		// Collection
		//-----------------------------------------------------------------------------

		public virtual void Collect() {
			Destroy();
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void Initialize() {
			base.Initialize();
			
			timer = 0;
		}

		public override void Update() {
			base.Update();

			// Update timeout timer.
			timer++;
			if (hasDuration) {
				if (timer == aliveDuration) {
					Destroy();
				}
				else if (timer == fadeDelay)
					graphics.IsFlickering = true;
			}

			// Check if colliding with the player.
			if (physics.IsSoftMeetingEntity(GameControl.Player, 9) && IsPickupable)
				Collect();
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public bool IsPickupable {
			get { return (timer >= pickupableDelay); }
		}

		public int PickupableDelay {
			get { return pickupableDelay; }
			set { pickupableDelay = value; }
		}
	}
}
