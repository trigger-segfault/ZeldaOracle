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
			SetReaction(InteractionType.Sword,			SenderReactions.Intercept, Reactions.ParryWithClingEffect);
			SetReaction(InteractionType.SwordSpin,		Reactions.ParryWithClingEffect);
			SetReaction(InteractionType.BiggoronSword,	Reactions.Damage3);
			// Seed interactions
			SetReaction(InteractionType.GaleSeed,		SenderReactions.Intercept);
			// Projectile interactions
			SetReaction(InteractionType.Arrow,			SenderReactions.Destroy, Reactions.ClingEffect);
			SetReaction(InteractionType.SwordBeam,		SenderReactions.Destroy, Reactions.ClingEffect);
			SetReaction(InteractionType.Boomerang,		SenderReactions.Intercept, Reactions.Stun, Reactions.DamageByLevel(0, 1));
			SetReaction(InteractionType.SwitchHook,		SenderReactions.Intercept, Reactions.ClingEffect);
			// Environment interactions
			SetReaction(InteractionType.Fire,			Reactions.None);
			SetReaction(InteractionType.Gale,			Reactions.None);
			SetReaction(InteractionType.BombExplosion,	Reactions.Damage2);
		}
	}
}
