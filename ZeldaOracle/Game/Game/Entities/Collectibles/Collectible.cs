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
			// Graphics
			Graphics.IsShadowVisible		= true;
			Graphics.IsGrassEffectVisible	= true;
			Graphics.IsRipplesEffectVisible	= true;
			Graphics.DepthLayer				= DepthLayer.Collectibles;
			Graphics.DepthLayerInAir		= DepthLayer.InAirCollectibles;

			// Physics
			Physics.CollisionBox		= new Rectangle2I(-2, -2, 4, 4);
			Physics.SoftCollisionBox	= new Rectangle2I(-2, -2, 4, 4);
			Physics.MovesWithConveyors	= true;

			// Interactions
			Interactions.Enable();
			Interactions.SetReaction(InteractionType.Sword,
				delegate(Entity sender, EventArgs args)
			{
				if (IsCollectible && IsCollectibleWithItems)
					Collect();
			});
			
			// Collectible
			hasDuration				= true;
			isCollectibleWithItems	= true;
			collected				= null;
			aliveDuration			= GameSettings.COLLECTIBLE_ALIVE_DURATION;
			fadeDelay				= GameSettings.COLLECTIBLE_FADE_DELAY;
			collectibleDelay		= GameSettings.COLLECTIBLE_PICKUPABLE_DELAY;
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
			physics.SoftCollisionBox = Interactions.InteractionBox;

			// Update timeout timer
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

			base.Update();
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
