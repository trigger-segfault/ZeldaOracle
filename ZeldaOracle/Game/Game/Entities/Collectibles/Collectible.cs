using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Entities.Projectiles.PlayerProjectiles;
using ZeldaOracle.Game.Items.Rewards;

namespace ZeldaOracle.Game.Entities {
	public class Collectible : Entity {

		protected int timer;
		protected int collectibleDelay;
		protected int aliveDuration;
		protected int fadeDelay;
		protected bool hasDuration;
		private event Action collected;
		protected bool isCollectibleWithWeapons;


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
			Physics.MovesWithConveyors	= true;

			// Interactions
			Interactions.Enable();
			Interactions.InteractionBox = new Rectangle2I(-2, -2, 4, 4);
			Interactions.SetReaction(InteractionType.PlayerContact,
				delegate(Entity sender, EventArgs args)
			{
				// TODO: Z-Distance of 9 here?
				if (IsCollectible)
					Collect();
			});
			Interactions.SetReaction(InteractionType.Sword,
				delegate(Entity sender, EventArgs args)
			{
				if (IsCollectible && IsCollectibleWithWeapons)
					Collect();
			});
			Interactions.SetReaction(InteractionType.Boomerang,
				delegate(Entity sender, EventArgs args)
			{
				if (IsCollectible && IsCollectibleWithWeapons) {
					PlayerBoomerang boomerang = (PlayerBoomerang) sender;
					boomerang.GrabCollectible(this);
				}
			});
			Interactions.SetReaction(InteractionType.SwitchHook,
				delegate(Entity sender, EventArgs args)
			{
				if (IsCollectible && IsCollectibleWithWeapons) {
					SwitchHookProjectile hook = (SwitchHookProjectile) sender;
					hook.GrabCollectible(this);
				}
			});
			
			// Collectible
			hasDuration				= true;
			isCollectibleWithWeapons	= true;
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
			collected?.Invoke();
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void Initialize() {
			base.Initialize();
			timer = 0;
		}

		public override void Update() {
			// Update timeout timer
			timer++;
			if (hasDuration) {
				if (timer == aliveDuration) {
					Destroy();
				}
				else if (timer == fadeDelay)
					graphics.IsFlickering = true;
			}

			base.Update();
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public bool IsCollectible {
			get { return (timer >= collectibleDelay); }
		}

		public bool IsCollectibleWithWeapons {
			get { return isCollectibleWithWeapons; }
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
