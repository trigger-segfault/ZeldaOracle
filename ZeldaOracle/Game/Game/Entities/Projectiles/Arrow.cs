using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Entities.Collisions;
using ZeldaOracle.Game.Entities.Players;

namespace ZeldaOracle.Game.Entities.Projectiles {
	
	public class Arrow : Projectile {
		
		protected int damage;
		protected bool silent;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public Arrow(bool silent = false) {
			// Graphics
			Graphics.DepthLayer = DepthLayer.ProjectileArrow;
			crashAnimation	= GameData.ANIM_PROJECTILE_PLAYER_ARROW_CRASH;
			bounceOnCrash	= true;

			// Physics
			Physics.CollisionBox = new Rectangle2F(-1, -1, 2, 1);
			Physics.Enable(
				PhysicsFlags.CollideWorld |
				PhysicsFlags.LedgePassable |
				PhysicsFlags.HalfSolidPassable |
				PhysicsFlags.DestroyedOutsideRoom);

			// Interactions
			Interactions.InteractionBox = new Rectangle2F(-1, -1, 2, 1);

			// Projectile
			syncAnimationWithAngle = true;
			projectileType = ProjectileType.Physical;

			// Arrow
			damage = GameSettings.PROJECTILE_ARROW_DAMAGE;
			this.silent = silent;
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void Initialize() {
			base.Initialize();
			Graphics.PlayAnimation(GameData.ANIM_PROJECTILE_PLAYER_ARROW);
		}
		
		public override bool Intercept() {
			Crash(true);
			return true;
		}

		public override void OnCollideSolid(Collision collision) {
			Crash(collision.IsResolved);
		}

		public override void OnCollidePlayer(Player player) {
			if (owner != player) {
				player.Hurt(damage, Center);
				Destroy();
			}
		}

		protected override void OnCrash() {
			base.OnCrash();
			if (!silent)
				AudioSystem.PlaySound(GameData.SOUND_EFFECT_CLING);
		}
	}
}
