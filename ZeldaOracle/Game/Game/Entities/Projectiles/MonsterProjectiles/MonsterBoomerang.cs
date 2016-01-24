using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Game.Entities.Players;

namespace ZeldaOracle.Game.Entities.Projectiles.MonsterProjectiles {
	
	public class MonsterBoomerang : Boomerang {
		
		private int damage;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public MonsterBoomerang() {
			damage = GameSettings.MONSTER_BOOMERANG_DAMAGE;
		}
		

		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void Initialize() {
			base.Initialize();
			Graphics.PlayAnimation(GameData.ANIM_PROJECTILE_MONSTER_BOOMERANG);
		}

		public override void OnCollidePlayer(Player player) {
			player.Hurt(damage, Center);
		}
	}
}
