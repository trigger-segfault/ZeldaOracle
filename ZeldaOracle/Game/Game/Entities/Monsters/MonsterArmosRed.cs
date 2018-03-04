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

			// Weapon interactions
			Reactions[InteractionType.Sword]
				.SetBegin(MonsterReactions.ParryWithClingEffect);
			Reactions[InteractionType.SwordStrafe]
				.SetBegin(MonsterReactions.ParryWithClingEffect);
			Reactions[InteractionType.SwordSpin]
				.SetBegin(MonsterReactions.ParryWithClingEffect);
			Reactions[InteractionType.BiggoronSword].Set(MonsterReactions.Damage3);
			// Seed interactions
			Interactions.SetReaction(InteractionType.GaleSeed,		SenderReactions.Intercept);
			// Projectile interactions
			Interactions.SetReaction(InteractionType.Arrow,			SenderReactions.Destroy, MonsterReactions.ClingEffect);
			Interactions.SetReaction(InteractionType.SwordBeam,		SenderReactions.Destroy, MonsterReactions.ClingEffect);
			Interactions.SetReaction(InteractionType.Boomerang,		SenderReactions.Intercept, MonsterReactions.Stun, MonsterReactions.DamageByLevel(0, 1));
			Interactions.SetReaction(InteractionType.SwitchHook,	SenderReactions.Intercept, MonsterReactions.ClingEffect);
			// Environment interactions
			Reactions[InteractionType.Fire].Clear();
			Reactions[InteractionType.Gale].Clear();
			Interactions.SetReaction(InteractionType.BombExplosion,	MonsterReactions.Damage2);
		}
	}
}
