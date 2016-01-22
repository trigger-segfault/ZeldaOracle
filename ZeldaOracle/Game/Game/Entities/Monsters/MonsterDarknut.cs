using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Game.Entities.Projectiles;

namespace ZeldaOracle.Game.Entities.Monsters {
	
	public class MonsterDarknut : BasicMonster {
		public MonsterDarknut() {
			// General.
			MaxHealth		= 2;
			ContactDamage	= 4;
			color			= MonsterColor.Red;
			animationMove	= GameData.ANIM_MONSTER_DARKNUT;
			
			// Movement.
			moveSpeed					= 0.5f;
			changeDirectionsOnCollide	= true;
			syncAnimationWithDirection	= true;
			stopTime.Set(0, 15);
			moveTime.Set(50, 100);

			// Projectiles.
			projectileType		= typeof(MonsterArrow);
			shootType			= ShootType.OnStop;
			aimType				= AimType.FacePlayer;
			shootSpeed			= 2.0f;
			projectileShootOdds	= 5; // 1 in 5 chance to shoot between movements.
			shootPauseDuration	= 5;
		}

		public override void Initialize() {
			base.Initialize();

			if (color == MonsterColor.Red) {
				MaxHealth = 3;
			}
			else if (color == MonsterColor.Blue) {
				MaxHealth = 5;
			}
		}
	}
}
