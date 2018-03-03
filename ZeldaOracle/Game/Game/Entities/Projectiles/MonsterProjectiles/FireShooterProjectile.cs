using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Entities.Collisions;
using ZeldaOracle.Game.Entities.Effects;
using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.Entities.Projectiles.PlayerProjectiles;
using ZeldaOracle.Game.Tiles;

namespace ZeldaOracle.Game.Entities.Projectiles.MonsterProjectiles {

	public class FireShooterProjectile : Projectile {
		
		private int phase;
		private int damage;
		private int timer;

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public FireShooterProjectile() {
			projectileType  = ProjectileType.Magic;
			crashAnimation  = null;
			syncAnimationWithDirection = true;
			damage          = 6;
			Graphics.DepthLayer = DepthLayer.ProjectileBeam;

			// Physics
			Physics.CollisionBox		= new Rectangle2F(-1, -1, 2, 2);
			Physics.SoftCollisionBox    = new Rectangle2F(-2, -2, 4, 4);
			Physics.Enable(
				PhysicsFlags.CollideWorld |
				PhysicsFlags.LedgePassable |
				PhysicsFlags.HalfSolidPassable |
				PhysicsFlags.DestroyedOutsideRoom);

			// TODO: Confirm accuracy of collision boxes.
			// NOTE: Speed is not an identical match as the projectile
			// speeds up and slows down differently for all 3 phases.
			// Speed is also designed to slow down once it reached the
			// tile it was going to collide with in dungeon 5.
			// AKA: It's speed was only designed for one use scenario.

			phase = 0;
			timer = 0;
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void Initialize() {
			base.Initialize();
			
			Graphics.PlayAnimation(GameData.ANIM_PROJECTILE_TILE_FIRE_SHOOTER_SMALL);
		}

		public override void Update() {
			base.Update();
			
			if (phase < 2 && timer >= GameSettings.PROJECTILE_FIRE_SHOOTER_PHASE_DURATIONS[phase]) {
				phase++;
				timer = 0;
				if (phase == 1) {
					Physics.CollisionBox        = new Rectangle2F(-2, -2, 4, 4);
					Physics.SoftCollisionBox    = new Rectangle2F(-4, -4, 8, 8);
					Physics.Velocity *= 1.5f;
					Graphics.PlayAnimation(GameData.ANIM_PROJECTILE_TILE_FIRE_SHOOTER_MEDIUM);
				}
				else {
					Physics.CollisionBox        = new Rectangle2F(-4, -4, 8, 8);
					Physics.SoftCollisionBox    = new Rectangle2F(-6, -6, 12, 12);
					Physics.Velocity *= 0.5f;
					Graphics.PlayAnimation(GameData.ANIM_PROJECTILE_TILE_FIRE_SHOOTER_LARGE);
				}
			}
			timer++;
		}

		public override void OnCollidePlayer(Player player) {
			player.Hurt(damage, position);
		}

		public override void OnCollideSolid(Collision collision) {
			// Spawn a poof effect upon colliding with a Magnet Ball
			if (collision.IsEntity && collision.Entity is MagnetBall) {
				Effect effect = new Effect(GameData.ANIM_EFFECT_BLOCK_POOF,
					DepthLayer.EffectBlockPoof);
				RoomControl.SpawnEntity(effect, Position);
				AudioSystem.PlaySound(GameData.SOUND_APPEAR_VANISH);
			}

			Destroy();
		}
	}
}
