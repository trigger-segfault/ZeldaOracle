using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Entities.Collisions;
using ZeldaOracle.Game.Entities.Effects;

namespace ZeldaOracle.Game.Entities.Projectiles.PlayerProjectiles {

	public class SwordBeam : Projectile {
		
		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public SwordBeam() {
			// Physics
			Physics.CollisionBox = new Rectangle2F(-1, -4, 2, 1);
			Physics.Enable(
				PhysicsFlags.CollideWorld |
				PhysicsFlags.LedgePassable |
				PhysicsFlags.HalfSolidPassable |
				PhysicsFlags.DestroyedOutsideRoom);
			
			// Interactions
			Interactions.Enable();
			Interactions.InteractionType = InteractionType.SwordBeam;
			Interactions.InteractionBox = new Rectangle2F(-1, -1, 2, 1);

			// Projectile
			syncAnimationWithDirection = true;
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void Initialize() {
			base.Initialize();

			if (Direction.IsHorizontal) {
				Physics.CollisionBox		= new Rectangle2F(-1, -5, 2, 1);
				Interactions.InteractionBox	= new Rectangle2F(-1, -5, 2, 1);
			}
			else {
				Physics.CollisionBox		= new Rectangle2F(-1, -1, 2, 2);
				Interactions.InteractionBox	= new Rectangle2F(-1, -1, 2, 2);
			}

			Graphics.PlayAnimation(GameData.ANIM_PROJECTILE_SWORD_BEAM);
		}
		
		public override bool Intercept() {
			// Spawn a silent cling effect
			Effect effect = new EffectCling(true);
			RoomControl.SpawnEntity(effect, position, zPosition);
			DestroyAndTransform(effect);
			return true;
		}

		public override void OnCollideSolid(Collision collision) {
			Intercept();
		}
	}
}
