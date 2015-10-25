using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Tiles;

namespace ZeldaOracle.Game.Entities.Effects {
	public class ScentPod : Effect {

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public ScentPod() {
			graphics.DrawOffset = new Point2I(-8, -12);
			centerOffset		= new Point2I(0, -4);

			CreateDestroyTimer(
				GameSettings.SCENT_POD_DURATION,
				GameSettings.SCENT_POD_FADE_DELAY);
		}
		

		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------
		
		public override void Initialize() {
			base.Initialize();
			graphics.PlayAnimation(GameData.ANIM_ITEM_SCENT_POD);
		}
	}
}
