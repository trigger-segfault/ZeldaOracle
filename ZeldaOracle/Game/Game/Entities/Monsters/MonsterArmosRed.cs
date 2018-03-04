using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Audio;

namespace ZeldaOracle.Game.Entities.Monsters {
	public class MonsterArmosRed : BasicMonster {
		public MonsterArmosRed() {
			// General.
			MaxHealth       = 2;
			ContactDamage   = 4;
			Color           = MonsterColor.DarkRed;
			animationMove   = GameData.ANIM_MONSTER_ARMOS;

			// Movement.
			moveSpeed                   = 0.5f;
			facePlayerOdds				= 0;
			changeDirectionsOnCollide   = true;
			syncAnimationWithDirection  = false;
			stopTime.Set(0, 4);
			moveTime.Set(60, 120);

			// Monster & unit settings
			isGaleable				= false;
			isBurnable				= false;
			isKnockbackable			= false;

			// Weapon Reactions
			Reactions[InteractionType.Sword]
				.SetBegin(MonsterReactions.ParryWithClingEffect);
			Reactions[InteractionType.SwordStrafe]
				.SetBegin(MonsterReactions.ParryWithClingEffect);
			Reactions[InteractionType.SwordSpin]
				.SetBegin(MonsterReactions.ParryWithClingEffect);
			Reactions[InteractionType.BiggoronSword]
				.Set(MonsterReactions.Damage3);
			// Seed Reactions
			Reactions[InteractionType.GaleSeed]
				.Set(SenderReactions.Intercept);
			// Projectile Reactions
			Reactions[InteractionType.Arrow]
				.Set(SenderReactions.Destroy)
				.Add(MonsterReactions.ClingEffect);
			Reactions[InteractionType.SwordBeam]
				.Set(SenderReactions.Destroy)
				.Add(MonsterReactions.ClingEffect);
			Reactions[InteractionType.Boomerang]
				.Set(SenderReactions.Intercept)
				.Add(MonsterReactions.Stun)
				.Add(MonsterReactions.DamageByLevel(0, 1));
			Reactions[InteractionType.SwitchHook]
				.Set(MonsterReactions.ParryWithClingEffect);
			// Environment Reactions
			Reactions[InteractionType.Fire].Clear();
			Reactions[InteractionType.Gale].Clear();
			Reactions[InteractionType.BombExplosion]
				.Set(MonsterReactions.Damage2);
		}
	}
}
