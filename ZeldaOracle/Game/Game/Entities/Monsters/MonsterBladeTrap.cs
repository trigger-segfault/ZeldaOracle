
using ZeldaOracle.Common.Geometry;

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
			
			// Interactions (Tools)
			Interactions.SetReaction(InteractionType.Sword,			SenderReactions.Bump, MonsterReactions.ClingEffect);
			Interactions.SetReaction(InteractionType.SwordSpin,		SenderReactions.Bump, MonsterReactions.ClingEffect);
			Interactions.SetReaction(InteractionType.BiggoronSword,	MonsterReactions.ClingEffect);
			Interactions.SetReaction(InteractionType.Shovel,			MonsterReactions.ClingEffect);
			// Interactions (Projectiles)
			Interactions.SetReaction(InteractionType.Arrow,			SenderReactions.Intercept);
			Interactions.SetReaction(InteractionType.SwordBeam,		SenderReactions.Intercept);
			Interactions.SetReaction(InteractionType.Boomerang,		SenderReactions.Intercept, MonsterReactions.ClingEffect);
			Interactions.SetReaction(InteractionType.SwitchHook,		SenderReactions.Intercept, MonsterReactions.ClingEffect);

			
			Interactions.SetReaction(InteractionType.Sword,			MonsterReactions.ParryWithClingEffect);
			Interactions.SetReaction(InteractionType.SwordSpin,		MonsterReactions.ParryWithClingEffect);
			Interactions.SetReaction(InteractionType.BiggoronSword,	MonsterReactions.ClingEffect);
			Interactions.SetReaction(InteractionType.Shovel,			MonsterReactions.ClingEffect);
		}

		public override void Initialize() {
			base.Initialize();
			Graphics.PlayAnimation(GameData.ANIM_MONSTER_BLADE_TRAP);
		}
	}
}
