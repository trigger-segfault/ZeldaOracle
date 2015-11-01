using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Entities.Effects;
using ZeldaOracle.Game.Entities.Monsters;
using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.Tiles;

namespace ZeldaOracle.Game.Entities.Projectiles {
	public class MonsterArrowProjectile : Projectile, IInterceptable {
		

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public MonsterArrowProjectile() {
			// General.
			syncAnimationWithDirection = true;

			// Physics.
			Physics.CollisionBox		= new Rectangle2F(-1, -1, 2, 1);
			Physics.SoftCollisionBox	= new Rectangle2F(-1, -1, 2, 1);
			EnablePhysics(
				PhysicsFlags.CollideWorld |
				PhysicsFlags.LedgePassable |
				PhysicsFlags.HalfSolidPassable |
				PhysicsFlags.DestroyedOutsideRoom);

			// Graphics.
			Graphics.DepthLayer = DepthLayer.ProjectileArrow;
			crashAnimation	= GameData.ANIM_PROJECTILE_MONSTER_ARROW_CRASH;
			bounceOnCrash	= true;
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void Initialize() {
			base.Initialize();
			Graphics.PlayAnimation(GameData.ANIM_PROJECTILE_MONSTER_ARROW);
			CheckInitialCollision();
		}
		
		public void Intercept() {
			Crash(false);
		}

		public override void OnCollideTile(Tile tile, bool isInitialCollision) {
			Crash(isInitialCollision);
		}

		public override void OnCollidePlayer(Player player) {
			player.Hurt(1, position);
			Destroy();
		}
	}
}
