using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.Tiles;

namespace ZeldaOracle.Game.Entities.Projectiles.MonsterProjectiles {
	
	public class BeamProjectile : Projectile {

		private int damage;
		private bool flickers;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public BeamProjectile() {
			projectileType			= ProjectileType.Beam;
			crashAnimation			= null;
			syncAnimationWithAngle	= true;
			
			// General.
			syncAnimationWithAngle	= true;
			damage					= 2;
			flickers				= false;

			// Physics.
			Physics.CollisionBox		= new Rectangle2F(-1, -1, 2, 1);
			Physics.SoftCollisionBox	= new Rectangle2F(-2, -2, 4, 4);
			Physics.Enable(
				PhysicsFlags.CollideWorld |
				PhysicsFlags.LedgePassable |
				PhysicsFlags.HalfSolidPassable |
				PhysicsFlags.DestroyedOutsideRoom);

			// Graphics.
			Graphics.DepthLayer = DepthLayer.ProjectileBeam;
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void Initialize() {
			base.Initialize();
			Graphics.PlayAnimation(GameData.ANIM_PROJECTILE_MONSTER_BEAM);
			if (flickers)
			{
				Graphics.FlickerAlternateDelay = 1;
				Graphics.IsFlickering = true;
			}
			CheckInitialCollision();
		}

		public override void Intercept() {
			Crash(false);
		}

		public override void OnCollideTile(Tile tile, bool isInitialCollision) {
			// Disable collisions with the source tile.
			if (tile != TileOwner)
				Crash(isInitialCollision);
		}

		public override void OnCollidePlayer(Player player) {
			// Only the flickering beams can damage the player
			if (flickers)
				player.Hurt(damage, position);
			Destroy();
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public bool Flickers {
			get { return flickers; }
			set { flickers = value; }
		}
	}
}
