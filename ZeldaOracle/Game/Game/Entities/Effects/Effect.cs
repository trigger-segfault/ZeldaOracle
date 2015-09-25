using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Graphics;

namespace ZeldaOracle.Game.Entities.Effects {
	
	public class Effect : Entity {
		private int		destroyTimer;
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
		public Effect(Animation animation) : this() {
			destroyTimer = -1;
			destroyOnAnimationComplete = true;

			Graphics.PlayAnimation(animation);
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
		// Overridden methods
		//-----------------------------------------------------------------------------
		
		public override void Initialize() {
			base.Initialize();
		}

		public override void Update() {
			base.Update();

			if (destroyOnAnimationComplete && Graphics.IsAnimationDone)
				Destroy();

			// Update the destroy timer.
			if (destroyTimer >= 0) {
				destroyTimer--;
				if (fadeDelay > 0 && destroyTimer == fadeDelay)
					graphics.IsFlickering = true;
				if (destroyTimer <= 0)
					Destroy();
			}
		}

		public override void Draw(Graphics2D g) {
			base.Draw(g);
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

	}
}
