using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Tiles;

namespace ZeldaOracle.Game.Entities.Effects {
	public class EffectGale : Effect {
		
		// FROM SHOOTER TO WALL:
		//   12 before fade.
		//   18 of fade
		// ON MONSTER:
		//   31 before fade.
		//   18 of fade.
		// ON NOTHING (Dropped from satchel)
		//   0 before fade.
		//   256 of fade.

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public EffectGale() :
			base(GameData.ANIM_EFFECT_SEED_GALE)
		{
			CreateDestroyTimer(256, 255, 1);
		}
		

		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void Update() {
			base.Update();
			// TODO: collide with player to send him to the warp screen.
		}
	}
}
