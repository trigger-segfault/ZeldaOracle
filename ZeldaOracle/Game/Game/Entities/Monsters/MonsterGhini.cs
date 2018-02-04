using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Game.Entities.Projectiles;

namespace ZeldaOracle.Game.Entities.Monsters {
	
	public class MonsterGhini : BasicMonster {
		public MonsterGhini() {
			// General.
			MaxHealth		= 5;
			ContactDamage	= 2;
			Color			= MonsterColor.Red;
			animationMove	= GameData.ANIM_MONSTER_GHINI;
			movesInAir		= true;

			// Graphics.
			isAnimationHorizontal		= true;
			syncAnimationWithDirection	= true;
			
			// Physics.
			Physics.HasGravity			= false;
			Physics.CollideWithWorld	= false;

			// Movement.
			moveSpeed					= 0.5f;
			changeDirectionsOnCollide	= true;
			stopTime.Set(0, 0);
			moveTime.Set(50, 90);
		}

		public override void Initialize() {
			base.Initialize();
			zPosition = 1;
		}

		public override void Update() {
			base.Update();
		}
	}
}
