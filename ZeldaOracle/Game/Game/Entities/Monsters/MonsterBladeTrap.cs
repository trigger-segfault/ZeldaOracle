
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
			Interactions.SetReaction(InteractionType.Sword,			SenderReactions.Bump, Reactions.ClingEffect);
			Interactions.SetReaction(InteractionType.SwordSpin,		SenderReactions.Bump, Reactions.ClingEffect);
			Interactions.SetReaction(InteractionType.BiggoronSword,	Reactions.ClingEffect);
			Interactions.SetReaction(InteractionType.Shovel,			Reactions.ClingEffect);
			// Interactions (Projectiles)
			Interactions.SetReaction(InteractionType.Arrow,			SenderReactions.Intercept);
			Interactions.SetReaction(InteractionType.SwordBeam,		SenderReactions.Intercept);
			Interactions.SetReaction(InteractionType.Boomerang,		SenderReactions.Intercept, Reactions.ClingEffect);
			Interactions.SetReaction(InteractionType.SwitchHook,		SenderReactions.Intercept, Reactions.ClingEffect);

			
			Interactions.SetReaction(InteractionType.Sword,			Reactions.ParryWithClingEffect);
			Interactions.SetReaction(InteractionType.SwordSpin,		Reactions.ParryWithClingEffect);
			Interactions.SetReaction(InteractionType.BiggoronSword,	Reactions.ClingEffect);
			Interactions.SetReaction(InteractionType.Shovel,			Reactions.ClingEffect);
		}

		public override void Initialize() {
			base.Initialize();
			Graphics.PlayAnimation(GameData.ANIM_MONSTER_BLADE_TRAP);
		}
	}
}
