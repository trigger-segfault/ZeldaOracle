using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeldaOracle.Game.Entities.Projectiles.MonsterProjectiles {

	public class SpearProjectile : Arrow {
		public SpearProjectile() {
			projectileType				= ProjectileType.Beam;
			crashAnimation				= null;
			syncAnimationWithAngle		= false;
			syncAnimationWithDirection	= true;
			Physics.CollideWithWorld	= false;
		}
		
		public override void Initialize() {
			base.Initialize();
			Graphics.PlayAnimation(GameData.ANIM_PROJECTILE_MONSTER_SPEAR);
		}
	}
}
