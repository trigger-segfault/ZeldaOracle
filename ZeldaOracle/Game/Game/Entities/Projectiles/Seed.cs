using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Entities.Effects;
using ZeldaOracle.Game.Tiles;

namespace ZeldaOracle.Game.Entities.Projectiles {
	
	public enum SeedType {
		Ember	= 0,
		Scent	= 1,
		Pegasus	= 2,
		Gale	= 3,
		Mystery	= 4
	}

	public class Seed : Entity {

		SeedType type;
		

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public Seed(SeedType type) {
			this.type = type;

			// Flags for seed's dropped from satchel
			EnablePhysics(
				PhysicsFlags.HasGravity |
				PhysicsFlags.DestroyedOutsideRoom |
				PhysicsFlags.DestroyedInHoles);

			/*
			// Flags for seed Projectiles
			EnablePhysics(
				PhysicsFlags.DestroyedOutsideRoom |
				PhysicsFlags.CollideWorld |
				PhysicsFlags.ReboundSolid |
				PhysicsFlags.HalfSolidPassable |
				PhysicsFlags.LedgePassable);
			*/

			Physics.CollisionBox		= new Rectangle2F(-1, -1, 2, 2);
			Physics.SoftCollisionBox	= new Rectangle2F(-1, -1, 2, 2);
			graphics.DrawOffset			= new Point2I(-6, -4);
			centerOffset				= new Point2I(0, -2);
		}


		//-----------------------------------------------------------------------------
		// Internal methods
		//-----------------------------------------------------------------------------


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnLand() {
			// Create seed effect: fire, pod, gale, mystery

			if (type == SeedType.Ember) {
				RoomControl.SpawnEntity(new Fire(), Center);
			}
			else if (type == SeedType.Scent) {

			}
			else if (type == SeedType.Mystery) {

			}
			else if (type == SeedType.Gale) {

			}
			Destroy();
		}

		public override void Initialize() {
			base.Initialize();

			Graphics.PlaySprite(GameData.SPR_ITEM_SEEDS[(int) type]);
		}

		public override void Update() {
			base.Update();
		}
	}
}
