using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Graphics;

namespace ZeldaOracle.Game.Entities.Effects {
	
	public class Effect : Entity {
		private int destroyTimer;
		private bool destroyOnAnimationComplete;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------
		
		public Effect() {
			destroyTimer = -1;
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
		public void CreateDestroyTimer(int ticks) {
			destroyTimer = ticks;
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
