using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.Entities.Projectiles.MonsterProjectiles;

namespace ZeldaOracle.Game.Entities.Monsters {
	
	public class MonsterGopongaFlower : Monster {
		
		private enum GopongaFlowerState {
			Closed,
			Open,
		}
		
		private GopongaFlowerState flowerState;
		private FireballProjectile fireball;
		private bool isShooting;
		private int timer;

		
		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public MonsterGopongaFlower() {
			// General
			MaxHealth		= 1;
			ContactDamage	= 2;
			Color			= MonsterColor.Red;
			isGaleable		= false;
			isKnockbackable	= false;

			// Physics
			Physics.HasGravity			= false;
			Physics.CollideWithWorld	= false;
			Physics.IsDestroyedInHoles	= false;

			// Weapon interactions
			SetReaction(InteractionType.Sword,			Reactions.Kill);
			SetReaction(InteractionType.BiggoronSword,	Reactions.Kill);
			SetReaction(InteractionType.SwordSpin,		Reactions.Kill);
			SetReaction(InteractionType.Shield,			SenderReactions.Bump);
			SetReaction(InteractionType.Shovel,			SenderReactions.Bump);
			// Seed interactions
			SetReaction(InteractionType.ScentSeed,		SenderReactions.Intercept);
			SetReaction(InteractionType.PegasusSeed,	SenderReactions.Intercept);
			// Projectile interactions
			SetReaction(InteractionType.Boomerang,		SenderReactions.Intercept, Reactions.Kill);
			SetReaction(InteractionType.Arrow,			SenderReactions.Intercept, Reactions.Kill);
			SetReaction(InteractionType.SwordBeam,		SenderReactions.Intercept, Reactions.Kill);
			SetReaction(InteractionType.SwitchHook,		SenderReactions.Intercept, Reactions.Kill);
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------
		
		public override void Initialize() {
			base.Initialize();

			// Begin in the Closed state
			isShooting	= false;
			timer		= 0;
			fireball	= null;
			flowerState	= GopongaFlowerState.Closed;
			Graphics.PlayAnimation(GameData.ANIM_MONSTER_GOPONGA_FLOWER_CLOSED);
		}

		public override void OnDestroy() {
			base.OnDestroy();
			
			// If we spawned a fireball and have not shot it, then destroy it
			if (fireball != null)
				fireball.Destroy();
		}

		public override void UpdateAI() {
			Player player = RoomControl.Player;
			
			if (flowerState == GopongaFlowerState.Closed) {
				timer++;

				// Enter the Open state
				if (timer >= GameSettings.MONSTER_GOPONGA_FLOWER_CLOSED_DURATION) {
					timer = 0;
					flowerState = GopongaFlowerState.Open;
					Graphics.PlayAnimation(
						GameData.ANIM_MONSTER_GOPONGA_FLOWER_OPEN);

					if (GRandom.NextInt(
						GameSettings.MONSTER_GOPONGA_FLOWER_SHOOT_ODDS) == 0)
					{
						isShooting = true;
					}
				}
			}
			else if (flowerState == GopongaFlowerState.Open) {
				timer++;

				if (isShooting) {
					// Spawn the fireball
					if (timer == GameSettings.MONSTER_GOPONGA_FLOWER_SHOOT_DELAY)
					{
						fireball = new FireballProjectile();
						ShootProjectile(fireball, Vector2F.Zero);
					}

					// Shoot the fireball
					if (timer == GameSettings.MONSTER_GOPONGA_FLOWER_SHOOT_DELAY +
						GameSettings.MONSTER_GOPONGA_FLOWER_SHOOT_HOLD_DURATION)
					{
						Vector2F vectorToPlayer = RoomControl.Player.Center - Center;
						fireball.Physics.Velocity = vectorToPlayer.Normalized *
							GameSettings.MONSTER_GOPONGA_FLOWER_SHOOT_SPEED;
						fireball = null;
					}
				}

				// Enter the closed state
				if (timer >= GameSettings.MONSTER_GOPONGA_FLOWER_OPEN_DURATION) {
					timer = 0;
					flowerState = GopongaFlowerState.Closed;
					Graphics.PlayAnimation(
						GameData.ANIM_MONSTER_GOPONGA_FLOWER_CLOSED);
				}
			}
		}
	}
}
