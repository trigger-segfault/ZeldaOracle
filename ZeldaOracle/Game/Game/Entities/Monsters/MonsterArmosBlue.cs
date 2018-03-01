using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeldaOracle.Game.Entities.Monsters {
	public class MonsterArmosBlue : MonsterArmosRed {
		public MonsterArmosBlue() {
			// General.
			MaxHealth		= 3;
			Color			= MonsterColor.DarkBlue;

			// Movement.
			moveSpeed					= 0.75f;
			stopTime.Set(0, 4);
			moveTime.Set(60, 120);

			// Weapon interactions
			Interactions.SetReaction(InteractionType.Sword,			SenderReactions.Intercept, Reactions.DamageByLevel(1, 2, 3));
			Interactions.SetReaction(InteractionType.SwordSpin,		Reactions.Damage2);
			Interactions.SetReaction(InteractionType.BiggoronSword,	Reactions.Damage3);
			// Projectile interactions
			Interactions.SetReaction(InteractionType.Arrow,			SenderReactions.Destroy, Reactions.Damage);
			Interactions.SetReaction(InteractionType.SwordBeam,		SenderReactions.Destroy, Reactions.SilentDamage);
			Interactions.SetReaction(InteractionType.SwitchHook,		SenderReactions.Intercept, Reactions.ClingEffect);
		}
	}
}
