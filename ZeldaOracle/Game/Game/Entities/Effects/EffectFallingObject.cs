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
		

		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------
		
		public override void Initialize() {
			base.Initialize();

			// Find the center position of the hole tile.
			Point2I location = RoomControl.GetTileLocation(position);
			holeCenterPosition = (Vector2F) location + new Vector2F(0.5f, 0.5f);
			holeCenterPosition *= GameSettings.TILE_SIZE;
		}

		public override void Update() {
			base.Update();

			// Move towards the center of the hole.
			if (Math.Abs(position.X - holeCenterPosition.X) > 0.5f)
				position.X += 0.5f * Math.Sign(holeCenterPosition.X - position.X);
			if (Math.Abs(position.Y - holeCenterPosition.Y) > 0.5f)
				position.Y += 0.5f * Math.Sign(holeCenterPosition.Y - position.Y);
		}
	}
}
