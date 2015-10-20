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
	
	// Seeds shot from the seed-shooter or slingshot.
	public class SeedProjectile : SeedEntity {

		private int reboundCounter;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public SeedProjectile(SeedType type) :
			base(type)
		{
			// Physics.
			Physics.CollisionBox		= new Rectangle2F(-1, -5, 2, 1);
			Physics.SoftCollisionBox	= new Rectangle2F(-1, -5, 2, 1);
			EnablePhysics(
				PhysicsFlags.DestroyedOutsideRoom |
				PhysicsFlags.CollideWorld |
				PhysicsFlags.ReboundSolid |
				PhysicsFlags.HalfSolidPassable |
				PhysicsFlags.LedgePassable);

			// Graphics.
			graphics.DrawOffset	= new Point2I(-4, -6);
			centerOffset		= new Point2I(0, -2);
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void Initialize() {
			base.Initialize();
			Graphics.PlaySprite(GameData.SPR_ITEM_SEEDS[(int) type]);
			reboundCounter = 0;
		}

		public override void OnCollideTile(Tile tile) {
			reboundCounter++;
			if (!(reboundCounter >= 3 || (tile != null && tile.Flags.HasFlag(TileFlags.AbsorbSeeds)))) {
				return;
			}

			// Move 3 pixels into the block from where it collided.
			position += Physics.PreviousVelocity.Normalized * 3.0f;

			// Notify the tile of the seed hitting it.
			if (tile != null) {
				tile.OnSeedHit(type, this);
				if (IsDestroyed)
					return;
			}
			
			// Create the seed's effect.
			if (type == SeedType.Ember)
				RoomControl.SpawnEntity(new Fire(), Center - new Vector2F(0, 1), zPosition);
			else
				CreateVisualEffect(type, Center);

			Destroy();
		}

		public override void OnCollideMonster(Monster monster) {
			monster.TriggerInteraction(monster.HandlerSeeds[(int) type], this);
		}
	}
}
