using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Entities.Collisions;
using ZeldaOracle.Game.Entities.Players;

namespace ZeldaOracle.Game.Entities.Projectiles.MonsterProjectiles {
	
	public class MagicProjectile : Projectile {
		
		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public MagicProjectile() {
			// Graphics
			Graphics.DepthLayer = DepthLayer.ProjectileArrow;
			
			// Physics
			Physics.CollisionBox = new Rectangle2F(-1, -1, 2, 1);
			Physics.Enable(
				PhysicsFlags.CollideWorld |
				PhysicsFlags.LedgePassable |
				PhysicsFlags.HalfSolidPassable |
				PhysicsFlags.DestroyedOutsideRoom);

			// Interactions
			Interactions.Enable();
			Interactions.InteractionBox = new Rectangle2F(-1, -1, 2, 1);

			// Projectile
			syncAnimationWithDirection = true;
			projectileType = ProjectileType.Magic;
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void Initialize() {
			base.Initialize();
			Graphics.PlayAnimation(GameData.ANIM_PROJECTILE_MONSTER_MAGIC);
		}
		
		public override void Intercept() {
			Crash();
		}

		public override void OnCollideSolid(Collision collision) {
			Crash();
		}

		public override void OnCollidePlayer(Player player) {
			player.Hurt(3, position);
			Destroy();
		}
	}
}
