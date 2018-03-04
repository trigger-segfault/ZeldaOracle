using System;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Entities.Collisions;
using ZeldaOracle.Game.Entities.Players;

namespace ZeldaOracle.Game.Entities.Projectiles.MonsterProjectiles {
	
	public class BeamProjectile : Projectile {

		private int damage;
		private bool flickers;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public BeamProjectile() {
			// Graphics
			Graphics.DepthLayer = DepthLayer.ProjectileBeam;
			
			// Physics
			Physics.CollisionBox = new Rectangle2F(-1, -1, 2, 1);
			Physics.Enable(
				PhysicsFlags.CollideWorld |
				PhysicsFlags.LedgePassable |
				PhysicsFlags.HalfSolidPassable |
				PhysicsFlags.DestroyedOutsideRoom);

			// Projectile
			syncAnimationWithAngle	= true;
			projectileType			= ProjectileType.Beam;
			crashAnimation			= null;
			syncAnimationWithAngle	= true;

			// Beam Projectile
			damage		= 2;
			flickers	= false;
		}


		//-----------------------------------------------------------------------------
		// Reactions
		//-----------------------------------------------------------------------------

		public void OnCollidePlayer(Entity sender, EventArgs args) {
			((Player) sender).Hurt(damage, position);
			Destroy();
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void Initialize() {
			base.Initialize();

			if (flickers) {
				Graphics.IsFlickering = true;
				Graphics.FlickerAlternateDelay = 1;

				// Only flickering beams will damage the player
				Interactions.Enable();
				Interactions.InteractionBox = new Rectangle2F(-2, -2, 4, 4);
				Reactions[InteractionType.PlayerContact].Set(OnCollidePlayer);
			}
			else {
				Reactions[InteractionType.PlayerContact].Clear();
			}

			Graphics.PlayAnimation(GameData.ANIM_PROJECTILE_MONSTER_BEAM);
		}

		public override bool Intercept() {
			Crash();
			return true;
		}

		public override void OnCollideSolid(Collision collision) {
			Crash();
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
