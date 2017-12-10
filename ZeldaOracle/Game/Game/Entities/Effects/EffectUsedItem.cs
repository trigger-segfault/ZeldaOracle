using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Game.Tiles;

namespace ZeldaOracle.Game.Entities.Effects {
	public class EffectUsedItem : Effect {


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public EffectUsedItem(ISprite sprite) :
			base()
		{
			// Create an animation of the sprite rising with two key-frames.
			Rectangle2I bounds = sprite.Bounds;
			Animation animation = new Animation(LoopMode.Reset);
			animation.AddFrame(0, 8, sprite, new Point2I(-bounds.Width / 2, -12));
			animation.AddFrame(8, 20, sprite, new Point2I(-bounds.Width / 2, -20));
			
			// Play the animation.
			destroyTimer = -1;
			destroyOnAnimationComplete = true;
			Graphics.PlayAnimation(animation);
			Graphics.DepthLayer = DepthLayer.EffectCling; // TODO: proper depth layer for EffectUsedItem
		}
	}
}
