using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Graphics;

namespace ZeldaOracle.Game.Entities.Effects {
	
	public class Effect : Entity {

		protected int	destroyTimer;
		protected bool	destroyOnAnimationComplete;
		protected int	fadeDelay;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------
		
		public Effect() {
			destroyTimer = -1;
			fadeDelay = -1;
			destroyOnAnimationComplete = false;

			Graphics.IsShadowVisible		= false;
			Graphics.IsGrassEffectVisible	= false;
			Graphics.IsRipplesEffectVisible	= false;
		}
		
		// Create an effect that plays an animation and then dissapears
		protected Effect(Animation animation) :
			this(animation, DepthLayer.None)
		{
		}
		
		// Create an effect that plays an animation and then dissapears
		public Effect(Animation animation, DepthLayer depthLayer) :
			this()
		{
			destroyTimer = -1;
			destroyOnAnimationComplete = true;

			Graphics.PlayAnimation(animation);
			Graphics.DepthLayer = depthLayer;
		}
		
		// Create an effect that plays an animation and then dissapears
		public Effect(Effect copy) :
			this()
		{
			destroyTimer = copy.destroyTimer;
			destroyOnAnimationComplete = copy.destroyOnAnimationComplete;
			fadeDelay = copy.fadeDelay;
			Graphics.DepthLayer = copy.Graphics.DepthLayer;

			if (copy.Graphics.Animation != null)
				Graphics.PlayAnimation(copy.Graphics.Animation);
		}

		public Effect Clone() {
			return new Effect(this);
		}


		//-----------------------------------------------------------------------------
		// Muators
		//-----------------------------------------------------------------------------

		// Setup a timer so that the effect destroys itself after the given number of ticks.
		public void CreateDestroyTimer(int ticks, int fadeDelay = -1, int flickerAlternateDelay = 2) {
			this.destroyTimer = ticks;
			this.fadeDelay = fadeDelay;
			Graphics.FlickerAlternateDelay = flickerAlternateDelay;
		}
		

		//-----------------------------------------------------------------------------
		// Virtual Methods
		//-----------------------------------------------------------------------------

		public virtual void OnDestroyTimerDone() {
			Destroy();
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------
		
		public override void Initialize() {
			base.Initialize();
		}

		public override void Update() {
			base.Update();

			if (destroyOnAnimationComplete && Graphics.IsAnimationDone)
				OnDestroyTimerDone();

			// Update the destroy timer.
			if (destroyTimer >= 0) {
				destroyTimer--;
				if (fadeDelay > 0 && destroyTimer == fadeDelay)
					graphics.IsFlickering = true;
				if (destroyTimer <= 0)
					OnDestroyTimerDone();
			}
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

	}
}
