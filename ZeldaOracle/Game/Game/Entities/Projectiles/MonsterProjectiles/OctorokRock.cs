using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeldaOracle.Game.Entities.Projectiles.MonsterProjectiles {
	
	public class OctorokRock : Arrow {
		public OctorokRock() {
			projectileType				= ProjectileType.Physical;
			crashAnimation				= GameData.ANIM_PROJECTILE_MONSTER_ROCK;
			syncAnimationWithAngle		= false;
			syncAnimationWithDirection	= true;
		}
		
		public override void Initialize() {
			base.Initialize();
			Graphics.PlayAnimation(GameData.ANIM_PROJECTILE_MONSTER_ROCK);
		}
	}
}
