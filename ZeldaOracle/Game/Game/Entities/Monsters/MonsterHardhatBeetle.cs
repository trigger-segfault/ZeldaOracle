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
			color						= MonsterColor.Orange;

			// Movement
			moveSpeed = 0.375f;
			
			// Weapon interactions
			SetReaction(InteractionType.Sword,			Reactions.Bump);
			SetReaction(InteractionType.SwordSpin,		Reactions.Bump);
			SetReaction(InteractionType.BiggoronSword,	Reactions.Bump);
			// Projectile interactions
			SetReaction(InteractionType.Arrow,			SenderReactions.Intercept, Reactions.Bump);
			SetReaction(InteractionType.SwordBeam,		SenderReactions.Intercept, Reactions.Bump);
			SetReaction(InteractionType.Boomerang,		SenderReactions.Intercept, Reactions.Bump);
			SetReaction(InteractionType.SwitchHook,		SenderReactions.Intercept, Reactions.Bump);
			SetReaction(InteractionType.ScentSeed,		SenderReactions.Intercept, Reactions.Bump);
			SetReaction(InteractionType.EmberSeed,		SenderReactions.Intercept, Reactions.None);
			SetReaction(InteractionType.Fire,			SenderReactions.Intercept, Reactions.None);
			SetReaction(InteractionType.RodFire,		SenderReactions.Intercept, Reactions.None);
			SetReaction(InteractionType.BombExplosion,	Reactions.Bump);
			SetReaction(InteractionType.Block,			Reactions.Bump);
			SetReaction(InteractionType.ThrownObject,	Reactions.Bump);
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
