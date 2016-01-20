using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Entities.Effects;
using ZeldaOracle.Game.Entities.Monsters;
using ZeldaOracle.Game.Tiles;

namespace ZeldaOracle.Game.Entities.Projectiles {
	
	public class MonsterArrow : Arrow {
		public MonsterArrow() {
			crashAnimation				= GameData.ANIM_PROJECTILE_MONSTER_ARROW_CRASH;
			syncAnimationWithAngle		= false;
			syncAnimationWithDirection	= true;
		}
		
		public override void Initialize() {
			base.Initialize();
			Graphics.PlayAnimation(GameData.ANIM_PROJECTILE_MONSTER_ARROW);
		}
	}
	
	public class OctorokRock : Arrow {
		public OctorokRock() {
			crashAnimation				= GameData.ANIM_PROJECTILE_MONSTER_ROCK;
			syncAnimationWithAngle		= false;
			syncAnimationWithDirection	= true;
		}
		
		public override void Initialize() {
			base.Initialize();
			Graphics.PlayAnimation(GameData.ANIM_PROJECTILE_MONSTER_ROCK);
		}
	}
	
	public class SpearProjectile : Arrow {
		public SpearProjectile() {
			crashAnimation				= null;
			syncAnimationWithAngle		= false;
			syncAnimationWithDirection	= true;
			Physics.CollideWithWorld	= false;
		}
		
		public override void Initialize() {
			base.Initialize();
			Graphics.PlayAnimation(GameData.ANIM_PROJECTILE_MONSTER_SPEAR);
		}
	}

	public class Arrow : Projectile, IInterceptable {
		
		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public Arrow() {
			// General.
			syncAnimationWithAngle = true;

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
			crashAnimation	= GameData.ANIM_PROJECTILE_PLAYER_ARROW_CRASH;
			bounceOnCrash	= true;
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void Initialize() {
			base.Initialize();
			Graphics.PlayAnimation(GameData.ANIM_PROJECTILE_PLAYER_ARROW);
			CheckInitialCollision();
		}
		
		public void Intercept() {
			Crash(false);
		}

		public override void OnCollideTile(Tile tile, bool isInitialCollision) {
			Crash(isInitialCollision);
		}

		public override void OnCollideMonster(Monster monster) {
			monster.TriggerInteraction(InteractionType.Arrow, this);
		}

		protected override void OnCrash() {
			base.OnCrash();
			AudioSystem.PlaySound(GameData.SOUND_EFFECT_CLING);
		}
	}
}
