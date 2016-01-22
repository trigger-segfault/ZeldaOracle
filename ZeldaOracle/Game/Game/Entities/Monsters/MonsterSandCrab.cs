using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Game.Entities.Monsters {
	public class MonsterSandCrab : BasicMonster {
		public MonsterSandCrab() {
			MaxHealth		= 4;
			ContactDamage	= 2;
			color			= MonsterColor.Orange;
			animationMove	= GameData.ANIM_MONSTER_SAND_CRAB;
			moveSpeed		= 0.375f;
			syncAnimationWithDirection = false;
			stopTime.Set(0, 0);
			moveTime.Set(40, 80);
		}

		public override void UpdateAI() {
			if (isMoving)
				speed = (Directions.IsHorizontal(direction) ? 1.25f : 0.25f);
			base.UpdateAI();
		}
	}
}
