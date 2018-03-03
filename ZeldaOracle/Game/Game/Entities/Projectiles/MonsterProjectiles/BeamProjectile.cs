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

			// Interactions
			Interactions.InteractionBox = new Rectangle2F(-2, -2, 4, 4);

			// Projectile
			syncAnimationWithAngle	= true;
			projectileType			= ProjectileType.Beam;
			crashAnimation			= null;
			syncAnimationWithAngle	= true;

			// Beam Projectile
			damage					= 2;
			flickers				= false;
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void Initialize() {
			base.Initialize();

			Graphics.PlayAnimation(GameData.ANIM_PROJECTILE_MONSTER_BEAM);

			if (flickers) {
				Graphics.FlickerAlternateDelay = 1;
				Graphics.IsFlickering = true;
			}
		}

		public override void Intercept() {
			Crash();
		}

		public override void OnCollideSolid(Collision collision) {
			// Disable collisions with the source tile
			if (!collision.IsTile || collision.Tile != TileOwner)
				Crash();
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
