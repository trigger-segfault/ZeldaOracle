
namespace ZeldaOracle.Game.Entities.Projectiles.MonsterProjectiles {

	public class SpearProjectile : Arrow {
		
		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public SpearProjectile() {
			// Physics
			Physics.CollideWithWorld = false;

			// Projectile
			projectileType				= ProjectileType.Beam;
			crashAnimation				= null;
			syncAnimationWithAngle		= false;
			syncAnimationWithDirection	= true;
		}
		
		
		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void Initialize() {
			base.Initialize();
			Graphics.PlayAnimation(GameData.ANIM_PROJECTILE_MONSTER_SPEAR);
		}
	}
}
