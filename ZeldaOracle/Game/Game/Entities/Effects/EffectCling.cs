using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeldaOracle.Game.Entities.Effects {

	public class EffectCling : Effect {

		public EffectCling(bool light = false) :
			base(light ? GameData.ANIM_EFFECT_CLING_LIGHT : GameData.ANIM_EFFECT_CLING, DepthLayer.EffectCling)
		{
			UpdateOnRoomPaused = true;
		}
	}
}
