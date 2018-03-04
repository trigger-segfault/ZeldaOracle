
namespace ZeldaOracle.Game.Entities.Monsters {

	public class MonsterBladeTrap : Monster {
		
		public MonsterBladeTrap() {
			// General.
			ContactDamage	= 2;
			isDamageable	= false;
			isBurnable		= false;
			isStunnable		= false;
			isGaleable		= false;
			IsKnockbackable	= false;
				
			// Weapon Reactions
			Reactions[InteractionType.Sword]
				.SetBegin(MonsterReactions.ParryWithClingEffect);
			Reactions[InteractionType.SwordStrafe]
				.SetBegin(MonsterReactions.ParryWithClingEffect);
			Reactions[InteractionType.SwordSpin]
				.SetBegin(MonsterReactions.ParryWithClingEffect);
			Reactions[InteractionType.BiggoronSword]
				.SetBegin(MonsterReactions.ClingEffect);
			Reactions[InteractionType.Shield]
				.SetBegin(MonsterReactions.ParryWithClingEffect);
			Reactions[InteractionType.Shovel]
				.SetBegin(MonsterReactions.ClingEffect);
			// Projectile Reactions
			Reactions[InteractionType.Arrow]
				.Set(SenderReactions.Intercept);
			Reactions[InteractionType.SwordBeam]
				.Set(SenderReactions.Intercept);
			Reactions[InteractionType.Boomerang]
				.Set(MonsterReactions.ParryWithClingEffect);
			Reactions[InteractionType.SwitchHook]
				.Set(MonsterReactions.ParryWithClingEffect);
		}

		public override void Initialize() {
			base.Initialize();
			Graphics.PlayAnimation(GameData.ANIM_MONSTER_BLADE_TRAP);
		}
	}
}
