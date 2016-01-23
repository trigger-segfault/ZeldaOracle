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
	public class MagicRodFire : Projectile, IInterceptable {
		

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public MagicRodFire() {
			// Physics.
			Physics.CollisionBox		= new Rectangle2F(-1, -1, 2, 1);
			Physics.SoftCollisionBox	= new Rectangle2F(-1, -1, 2, 1);
			EnablePhysics(
				PhysicsFlags.CollideWorld |
				PhysicsFlags.LedgePassable |
				PhysicsFlags.HalfSolidPassable |
				PhysicsFlags.DestroyedOutsideRoom);

			// Graphics.
			Graphics.DepthLayer	= DepthLayer.ProjectileRodFire;
		}


		//-----------------------------------------------------------------------------
		// Intercept Methods
		//-----------------------------------------------------------------------------

		public void Intercept() {
			DestroyWithFire();
		}

		public void InterceptAndAbsorb() {
			// Spawn fire.
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
			CheckInitialCollision();
		}

		public override void OnCollideTile(Tile tile, bool isInitialCollision) {
			// Move 3 pixels into the block from where it collided.
			if (!isInitialCollision)
				position += Physics.PreviousVelocity.Normalized * 3.0f;

			Intercept();
		}

		public override void OnCollideMonster(Monster monster) {
			monster.TriggerInteraction(InteractionType.RodFire, this);
		}
	}
}
