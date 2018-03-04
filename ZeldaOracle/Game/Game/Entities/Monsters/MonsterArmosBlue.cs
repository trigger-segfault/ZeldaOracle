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
			Interactions.SetReaction(InteractionType.Sword,			SenderReactions.Intercept, MonsterReactions.DamageByLevel(1, 2, 3));
			Interactions.SetReaction(InteractionType.SwordSpin,		MonsterReactions.Damage2);
			Interactions.SetReaction(InteractionType.BiggoronSword,	MonsterReactions.Damage3);
			// Projectile interactions
			Interactions.SetReaction(InteractionType.Arrow,			SenderReactions.Destroy, MonsterReactions.Damage);
			Interactions.SetReaction(InteractionType.SwordBeam,		SenderReactions.Destroy, MonsterReactions.SilentDamage);
			Reactions[InteractionType.SwitchHook]
				.Set(MonsterReactions.ParryWithClingEffect);
		}
	}
}
