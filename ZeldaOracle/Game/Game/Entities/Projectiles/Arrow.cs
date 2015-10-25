using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Entities.Effects;
using ZeldaOracle.Game.Entities.Monsters;
using ZeldaOracle.Game.Tiles;

namespace ZeldaOracle.Game.Entities.Projectiles {
	public class Arrow : Projectile {
		

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
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void Initialize() {
			base.Initialize();
			Graphics.PlayAnimation(GameData.ANIM_PROJECTILE_PLAYER_ARROW);
			CheckInitialCollision();
		}

		public override void OnCollideTile(Tile tile, bool isInitialCollision) {
			// Create crash effect.
			Effect effect = new Effect();
			effect.CreateDestroyTimer(32);	
			effect.Physics.Velocity		= Angles.ToVector(Angles.Reverse(Angle)) * 0.25f;
			effect.Physics.ZVelocity	= 1.0f;
			effect.Physics.Gravity		= 0.07f;
			effect.EnablePhysics(PhysicsFlags.HasGravity);
			effect.Graphics.IsShadowVisible = false;
			effect.Graphics.PlayAnimation(GameData.ANIM_PROJECTILE_PLAYER_ARROW_CRASH);
			RoomControl.SpawnEntity(effect, position);

			Destroy();
		}

		public override void OnCollideMonster(Monster monster) {
			monster.TriggerInteraction(monster.HandlerArrow, this);
		}
	}
}
