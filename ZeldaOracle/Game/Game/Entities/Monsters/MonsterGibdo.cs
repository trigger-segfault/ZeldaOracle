using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeldaOracle.Game.Entities.Monsters {
	
	public class MonsterGibdo : BasicMonster {
		public MonsterGibdo() {
			MaxHealth		= 4;
			ContactDamage	= 2;
			color			= MonsterColor.Red;
			animationMove	= GameData.ANIM_MONSTER_GIBDO;
			IsKnockbackable	= false;
			moveSpeed		= 0.375f;
			syncAnimationWithDirection = false;
			stopTime.Set(0, 0);
			moveTime.Set(50, 90);
		}
	}
}
