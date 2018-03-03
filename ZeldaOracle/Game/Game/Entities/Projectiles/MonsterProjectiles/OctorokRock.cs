
namespace ZeldaOracle.Game.Entities.Projectiles.MonsterProjectiles {
	
	public class OctorokRock : Arrow {

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public OctorokRock() {
			projectileType				= ProjectileType.Physical;
			crashAnimation				= GameData.ANIM_PROJECTILE_MONSTER_ROCK;
			syncAnimationWithAngle		= false;
			syncAnimationWithDirection	= true;
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void Initialize() {
			base.Initialize();
			Graphics.PlayAnimation(GameData.ANIM_PROJECTILE_MONSTER_ROCK);
		}
	}
}
