using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Entities.Effects;
using ZeldaOracle.Game.Entities.Monsters;
using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.Tiles;

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
			Physics.CollisionBox		= new Rectangle2F(-1, -1, 2, 1);
			Physics.SoftCollisionBox	= new Rectangle2F(-1, -1, 2, 1);
			Physics.Enable(
				PhysicsFlags.CollideWorld |
				PhysicsFlags.LedgePassable |
				PhysicsFlags.HalfSolidPassable |
				PhysicsFlags.DestroyedOutsideRoom);

			// Projectile
			syncAnimationWithAngle	= true;
			projectileType			= ProjectileType.Physical;

			// Arrow
			damage		= GameSettings.PROJECTILE_ARROW_DAMAGE;
			this.silent	= silent;
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void Initialize() {
			base.Initialize();
			Graphics.PlayAnimation(GameData.ANIM_PROJECTILE_PLAYER_ARROW);
			CheckInitialCollision();
		}
		
		public override void Intercept() {
			Crash(false);
		}

		public override void OnCollideTile(Tile tile, bool isInitialCollision) {
			Crash(isInitialCollision);
		}

		public override void OnCollideMonster(Monster monster) {
			monster.Interactions.Trigger(InteractionType.Arrow, this);
		}

		public override void OnCollidePlayer(Player player) {
			player.Hurt(damage, Center);
			Destroy();
		}

		protected override void OnCrash() {
			base.OnCrash();
			if (!silent)
				AudioSystem.PlaySound(GameData.SOUND_EFFECT_CLING);
		}
	}
}
