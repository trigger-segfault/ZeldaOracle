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
			Interactions.SetReaction(InteractionType.Sword,			MonsterReactions.Bump);
			Interactions.SetReaction(InteractionType.SwordSpin,		MonsterReactions.Bump);
			Interactions.SetReaction(InteractionType.BiggoronSword,	MonsterReactions.Bump);
			// Projectile interactions
			Interactions.SetReaction(InteractionType.Arrow,			SenderReactions.Intercept, MonsterReactions.Bump);
			Interactions.SetReaction(InteractionType.SwordBeam,		SenderReactions.Intercept, MonsterReactions.Bump);
			Interactions.SetReaction(InteractionType.Boomerang,		SenderReactions.Intercept, MonsterReactions.Bump);
			Interactions.SetReaction(InteractionType.SwitchHook,	SenderReactions.Intercept, MonsterReactions.Bump);
			Interactions.SetReaction(InteractionType.ScentSeed,		SenderReactions.Intercept, MonsterReactions.Bump);
			Interactions.SetReaction(InteractionType.EmberSeed,		SenderReactions.Intercept);
			Interactions.SetReaction(InteractionType.Fire,			SenderReactions.Intercept);
			Interactions.SetReaction(InteractionType.RodFire,		SenderReactions.Intercept);
			Interactions.SetReaction(InteractionType.BombExplosion,	MonsterReactions.Bump);
			Interactions.SetReaction(InteractionType.Block,			MonsterReactions.Bump);
			Interactions.SetReaction(InteractionType.ThrownObject,	MonsterReactions.Bump);
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
