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
		protected int collectibleDelay;
		protected int aliveDuration;
		protected int fadeDelay;
		protected bool hasDuration;
		private event Action collected;
		protected bool isCollectibleWithItems;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public Collectible() {
			this.timer			= 0;
			this.hasDuration	= true;

			// Physics.
			Physics.CollisionBox		= new Rectangle2I(-2, -2, 4, 4);
			Physics.SoftCollisionBox	= new Rectangle2I(-2, -2, 4, 4);
			Physics.MovesWithConveyors	= true;

			// Graphics.
			Graphics.DepthLayer			= DepthLayer.Collectibles;
			Graphics.DepthLayerInAir	= DepthLayer.InAirCollectibles;

			aliveDuration	= GameSettings.COLLECTIBLE_ALIVE_DURATION;
			fadeDelay		= GameSettings.COLLECTIBLE_FADE_DELAY;
			collectibleDelay	= GameSettings.COLLECTIBLE_PICKUPABLE_DELAY;

			isCollectibleWithItems = true;

			collected = null;
		}


		//-----------------------------------------------------------------------------
		// Collection
		//-----------------------------------------------------------------------------

		public virtual void Collect() {
			Destroy();
			if (collected != null)
				collected();
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
			if (physics.IsSoftMeetingEntity(GameControl.Player, 9) && IsCollectible)
				Collect();
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public bool IsCollectible {
			get { return (timer >= collectibleDelay); }
		}

		public bool IsCollectibleWithItems {
			get { return isCollectibleWithItems; }
		}

		public int CollectibleDelay {
			get { return collectibleDelay; }
			set { collectibleDelay = value; }
		}

		public bool HasDuration {
			get { return hasDuration; }
			set { hasDuration = value; }
		}

		public event Action Collected {
			add { collected += value; }
			remove { collected -= value; }
		}
	}
}
