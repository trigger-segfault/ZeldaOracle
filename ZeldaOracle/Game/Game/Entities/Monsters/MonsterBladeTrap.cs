
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
			SetReaction(InteractionType.Sword,			SenderReactions.Bump, Reactions.ClingEffect);
			SetReaction(InteractionType.SwordSpin,		SenderReactions.Bump, Reactions.ClingEffect);
			SetReaction(InteractionType.BiggoronSword,	Reactions.ClingEffect);
			SetReaction(InteractionType.Shovel,			Reactions.ClingEffect);
			// Interactions (Projectiles)
			SetReaction(InteractionType.Arrow,			SenderReactions.Intercept);
			SetReaction(InteractionType.SwordBeam,		SenderReactions.Intercept);
			SetReaction(InteractionType.Boomerang,		SenderReactions.Intercept, Reactions.ClingEffect);
			SetReaction(InteractionType.SwitchHook,		SenderReactions.Intercept, Reactions.ClingEffect);

			
			SetReaction(InteractionType.Sword,			Reactions.ParryWithClingEffect);
			SetReaction(InteractionType.SwordSpin,		Reactions.ParryWithClingEffect);
			SetReaction(InteractionType.BiggoronSword,	Reactions.ClingEffect);
			SetReaction(InteractionType.Shovel,			Reactions.ClingEffect);
		}

		public override void Initialize() {
			base.Initialize();
			Graphics.PlayAnimation(GameData.ANIM_MONSTER_BLADE_TRAP);
		}
	}
}
