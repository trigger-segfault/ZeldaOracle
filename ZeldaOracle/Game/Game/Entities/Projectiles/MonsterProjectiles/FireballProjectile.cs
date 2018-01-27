
using ZeldaOracle.Game.Entities.Players;

namespace ZeldaOracle.Game.Entities.Projectiles.MagicProjectiles {

	public class FireballProjectile : Projectile {
		
		private int damage;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public FireballProjectile() {
			projectileType	= ProjectileType.Magic;
			crashAnimation	= null;
			damage			= 2;
			Graphics.DepthLayer = DepthLayer.ProjectileBeam;
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
