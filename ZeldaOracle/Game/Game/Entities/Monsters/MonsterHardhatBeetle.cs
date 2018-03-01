using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Game.Entities.Monsters {
	
	public class MonsterHardhatBeetle : Monster {

		private float moveSpeed;
		

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public MonsterHardhatBeetle() {
			// General
			ContactDamage			= 1;
			isDamageable			= false;
			isBurnable				= false;
			bumpKnockbackDuration	= 11;

			// Graphics
			syncAnimationWithDirection	= false;
			Color						= MonsterColor.Orange;

			// Movement
			moveSpeed = 0.375f;
			
			// Weapon interactions
			Interactions.SetReaction(InteractionType.Sword,			Reactions.Bump);
			Interactions.SetReaction(InteractionType.SwordSpin,		Reactions.Bump);
			Interactions.SetReaction(InteractionType.BiggoronSword,	Reactions.Bump);
			// Projectile interactions
			Interactions.SetReaction(InteractionType.Arrow,			SenderReactions.Intercept, Reactions.Bump);
			Interactions.SetReaction(InteractionType.SwordBeam,		SenderReactions.Intercept, Reactions.Bump);
			Interactions.SetReaction(InteractionType.Boomerang,		SenderReactions.Intercept, Reactions.Bump);
			Interactions.SetReaction(InteractionType.SwitchHook,		SenderReactions.Intercept, Reactions.Bump);
			Interactions.SetReaction(InteractionType.ScentSeed,		SenderReactions.Intercept, Reactions.Bump);
			Interactions.SetReaction(InteractionType.EmberSeed,		SenderReactions.Intercept, Reactions.None);
			Interactions.SetReaction(InteractionType.Fire,			SenderReactions.Intercept, Reactions.None);
			Interactions.SetReaction(InteractionType.RodFire,		SenderReactions.Intercept, Reactions.None);
			Interactions.SetReaction(InteractionType.BombExplosion,	Reactions.Bump);
			Interactions.SetReaction(InteractionType.Block,			Reactions.Bump);
			Interactions.SetReaction(InteractionType.ThrownObject,	Reactions.Bump);
		}

		
		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void Initialize() {
			base.Initialize();
			Graphics.PlayAnimation(GameData.ANIM_MONSTER_HARDHAT_BEETLE);
		}

		public override void Knockback(int duration, float speed, Vector2F sourcePosition) {
			base.Knockback(duration, speed, sourcePosition);
			Graphics.PauseAnimation();
		}

		public override void OnKnockbackEnd() {
			base.OnKnockbackEnd();
			Graphics.ResumeAnimation();
		}

		public override void UpdateAI() {
			// Chase player
			if (!IsBeingKnockedBack) {
				Vector2F vectorToPlayer = RoomControl.Player.Center - Center;
				physics.Velocity = vectorToPlayer.Normalized * moveSpeed;
			}
		}
	}
}
