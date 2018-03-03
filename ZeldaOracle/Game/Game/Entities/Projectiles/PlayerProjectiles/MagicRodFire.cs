using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Entities.Collisions;
using ZeldaOracle.Game.Entities.Effects;

namespace ZeldaOracle.Game.Entities.Projectiles.PlayerProjectiles {

	public class MagicRodFire : Projectile {
		
		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public MagicRodFire() {
			// Graphics
			Graphics.DepthLayer	= DepthLayer.ProjectileRodFire;

			// Physics
			Physics.CollisionBox = new Rectangle2F(-1, -1, 2, 1);
			Physics.Enable(
				PhysicsFlags.CollideWorld |
				PhysicsFlags.LedgePassable |
				PhysicsFlags.HalfSolidPassable |
				PhysicsFlags.DestroyedOutsideRoom);

			// Interactions
			Interactions.InteractionBox = new Rectangle2F(-1, -1, 2, 1);
			Interactions.InteractionType = InteractionType.RodFire;
		}


		//-----------------------------------------------------------------------------
		// Intercept Methods
		//-----------------------------------------------------------------------------

		public void InterceptAndAbsorb() {
			// Spawn fire
			Fire fire = DestroyWithFire();
			fire.IsAbsorbed = true;
		}

		private Fire DestroyWithFire() {
			Fire fire = new Fire();
			RoomControl.SpawnEntity(fire, position);
			AudioSystem.PlaySound(GameData.SOUND_FIRE);
			DestroyAndTransform(fire);
			return fire;
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void Initialize() {
			base.Initialize();
			Graphics.PlayAnimation(GameData.ANIM_PROJECTILE_MAGIC_ROD_FIRE);
		}

		public override void Intercept() {
			DestroyWithFire();
		}

		public override void OnCollideSolid(Collision collision) {
			// Move 3 pixels into the block from where it collided
			if (collision.IsResolved)
				position += Physics.PreviousVelocity.Normalized * 3.0f;
			Intercept();
		}
	}
}
