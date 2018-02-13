using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Tiles;

namespace ZeldaOracle.Game.Entities.Effects {
	public class EffectFallingObject : Effect {

		private Vector2F holeCenterPosition;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public EffectFallingObject() :
			base(GameData.ANIM_EFFECT_FALLING_OBJECT)
		{
			Graphics.DepthLayer = DepthLayer.EffectFallingObject;
		}
		
		public EffectFallingObject(DepthLayer depthLayer) :
			base(GameData.ANIM_EFFECT_FALLING_OBJECT)
		{
			Graphics.DepthLayer = depthLayer;
		}
		

		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------
		
		public override void Initialize() {
			base.Initialize();

			// Find the center position of the hole tile.
			Point2I location = RoomControl.GetTileLocation(position);
			holeCenterPosition = (Vector2F) location + Vector2F.Half;
			holeCenterPosition *= GameSettings.TILE_SIZE;
		}

		public override void Update() {
			base.Update();

			// Move towards the center of the hole.
			if (GMath.Abs(position.X - holeCenterPosition.X) > 0.5f)
				position.X += 0.5f * GMath.Sign(holeCenterPosition.X - position.X);
			if (GMath.Abs(position.Y - holeCenterPosition.Y) > 0.5f)
				position.Y += 0.5f * GMath.Sign(holeCenterPosition.Y - position.Y);
		}
	}
}
