using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeldaOracle.Game.Entities.Projectiles.MonsterProjectiles {
	
	public class MonsterArrow : Arrow {
		public MonsterArrow() : this(false) { }

		public MonsterArrow(bool silent) : base(silent) {
			projectileType				= ProjectileType.Physical;
			crashAnimation				= GameData.ANIM_PROJECTILE_MONSTER_ARROW_CRASH;
			syncAnimationWithAngle		= false;
			syncAnimationWithDirection	= true;
		}
		
		public override void Initialize() {
			base.Initialize();
			Graphics.PlayAnimation(GameData.ANIM_PROJECTILE_MONSTER_ARROW);
		}
	}
}
