
namespace ZeldaOracle.Game.Entities.Projectiles.MonsterProjectiles {
	
	public class BoneProjectile : Arrow {

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public BoneProjectile() {
			projectileType				= ProjectileType.Physical;
			crashAnimation				= GameData.ANIM_PROJECTILE_MONSTER_BONE;
			damage						= GameSettings.MONSTER_PROJECTILE_BONE_DAMAGE;
			syncAnimationWithAngle		= false;
			syncAnimationWithDirection	= false;
		}

		
		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void Initialize() {
			base.Initialize();
			Graphics.PlayAnimation(GameData.ANIM_PROJECTILE_MONSTER_BONE);
		}
	}
}
