using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeldaOracle.Game.Entities.Monsters {
	public class MonsterShroudedStalfos : MonsterMoblin {

		public MonsterShroudedStalfos() {
			MaxHealth = 2;
			animationMove = GameData.ANIM_MONSTER_SHROUDED_STALFOS;
		}
	}
}
