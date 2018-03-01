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
			Interactions.SetReaction(InteractionType.Sword,			SenderReactions.Intercept, Reactions.ParryWithClingEffect);
			Interactions.SetReaction(InteractionType.SwordSpin,		Reactions.ParryWithClingEffect);
			Interactions.SetReaction(InteractionType.BiggoronSword,	Reactions.Damage3);
			// Seed interactions
			Interactions.SetReaction(InteractionType.GaleSeed,		SenderReactions.Intercept);
			// Projectile interactions
			Interactions.SetReaction(InteractionType.Arrow,			SenderReactions.Destroy, Reactions.ClingEffect);
			Interactions.SetReaction(InteractionType.SwordBeam,		SenderReactions.Destroy, Reactions.ClingEffect);
			Interactions.SetReaction(InteractionType.Boomerang,		SenderReactions.Intercept, Reactions.Stun, Reactions.DamageByLevel(0, 1));
			Interactions.SetReaction(InteractionType.SwitchHook,		SenderReactions.Intercept, Reactions.ClingEffect);
			// Environment interactions
			Interactions.SetReaction(InteractionType.Fire,			Reactions.None);
			Interactions.SetReaction(InteractionType.Gale,			Reactions.None);
			Interactions.SetReaction(InteractionType.BombExplosion,	Reactions.Damage2);
		}
	}
}
