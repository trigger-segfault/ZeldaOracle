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
	public class SwordBeam : Projectile {
		

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public SwordBeam() {
			// General.
			syncAnimationWithDirection = true;

			// Physics.
			EventCollision += Crash;
			Physics.CollisionBox		= new Rectangle2F(-1, -4, 2, 1);
			Physics.SoftCollisionBox	= new Rectangle2F(-1, -1, 2, 1);
			EnablePhysics(PhysicsFlags.CollideWorld | PhysicsFlags.LedgePassable |
					PhysicsFlags.HalfSolidPassable | PhysicsFlags.DestroyedOutsideRoom);
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void Initialize() {
			base.Initialize();

			if (Directions.IsHorizontal(Direction)) {
				Physics.CollisionBox		= new Rectangle2F(-1, -5, 2, 1);
				Physics.SoftCollisionBox	= new Rectangle2F(-1, -5, 2, 1);
			}
			else {
				Physics.CollisionBox		= new Rectangle2F(-1, -1, 2, 2);
				Physics.SoftCollisionBox	= new Rectangle2F(-1, -1, 2, 2);
			}

			Graphics.PlayAnimation(GameData.ANIM_PROJECTILE_SWORD_BEAM);
			CheckInitialCollision();
		}

		public override void OnCollideMonster(Monster monster) {
			monster.TriggerInteraction(monster.HandlerSwordBeam, this);
		}

		private void Crash() {
			// Create cling effect.
			Effect effect = new Effect(GameData.ANIM_EFFECT_CLING_LIGHT);
			RoomControl.SpawnEntity(effect, position, zPosition);
			Destroy();
		}
	}
}
