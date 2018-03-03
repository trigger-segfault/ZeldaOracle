
using ZeldaOracle.Game.Entities.Players;

namespace ZeldaOracle.Game.Entities.Projectiles.MonsterProjectiles {

	public class FireballProjectile : Projectile {
		
		private int damage;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public FireballProjectile() {
			// Graphics
			Graphics.DepthLayer = DepthLayer.ProjectileBeam;

			// Projectile
			projectileType	= ProjectileType.Magic;
			crashAnimation	= null;
			damage			= 2;
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------
		
		public override void Initialize() {
			base.Initialize();
			Graphics.PlayAnimation(GameData.ANIM_PROJECTILE_MONSTER_FIREBALL);
		}

		public override void OnCollidePlayer(Player player) {
			player.Hurt(damage, position);
			Destroy();
		}
	}
}
