using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Tiles;

namespace ZeldaOracle.Game.Entities.Effects {
	public class EffectUsedItem : Effect {

		private Vector2F holeCenterPosition;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public EffectUsedItem(Sprite sprite) :
			base()
		{
			// Create an animation of the sprite rising with two key-frames.
			Sprite spr1 = new Sprite(sprite);
			spr1.DrawOffset += new Point2I(-sprite.SourceRect.Width / 2, -12);
			Sprite spr2 = new Sprite(sprite);
			spr2.DrawOffset += new Point2I(-sprite.SourceRect.Width / 2, -20);
			
			Animation animation = new Animation(LoopMode.Reset);
			animation.AddFrame(0, 8, spr1);
			animation.AddFrame(8, 20, spr2);
			
			// Play the animation.
			destroyTimer = -1;
			destroyOnAnimationComplete = true;
			Graphics.PlayAnimation(animation);
			Graphics.DepthLayer = DepthLayer.EffectCling; // TODO: proper depth layer for EffectUsedItem
		}
	}
}
