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
			graphics.DrawOffset			= new Point2I(-4, -6);
			centerOffset				= new Point2I(0, -2);
		}


		//-----------------------------------------------------------------------------
		// Internal methods
		//-----------------------------------------------------------------------------


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnLand() {
			// Notify the tile of the seed hitting it.
			Point2I location = RoomControl.GetTileLocation(position);
			Tile tile = RoomControl.GetTopTile(location);
			if (tile != null) {
				tile.OnSeedHit(this);
				if (IsDestroyed)
					return;
			}

			// Create the seed's effect.
			if (type == SeedType.Ember) {
				RoomControl.SpawnEntity(new Fire(), Center);
			}
			else if (type == SeedType.Scent) {
				RoomControl.SpawnEntity(new ScentPod(), Center);
			}
			else if (type == SeedType.Mystery) {
				RoomControl.SpawnEntity(new Effect(GameData.ANIM_EFFECT_SEED_MYSTERY), Center - new Point2I(8, 8));
			}
			else if (type == SeedType.Gale) {
				RoomControl.SpawnEntity(new Effect(GameData.ANIM_EFFECT_SEED_GALE), Center - new Point2I(8, 8));
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


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public SeedType Type {
			get { return type; }
		}
	}
}
