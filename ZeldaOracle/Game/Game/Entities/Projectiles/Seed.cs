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

	// Seeds dropped from the satchel.
	public class Seed : SeedEntity {

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public Seed(SeedType type) :
			base(type)
		{
			// Physics.
			Physics.CollisionBox		= new Rectangle2F(-1, -1, 2, 2);
			Physics.SoftCollisionBox	= new Rectangle2F(-1, -1, 2, 2);
			EnablePhysics(
				PhysicsFlags.HasGravity |
				PhysicsFlags.DestroyedOutsideRoom |
				PhysicsFlags.DestroyedInHoles);

			// Graphics.
			graphics.DrawOffset	= new Point2I(-4, -6);
			centerOffset		= new Point2I(0, -2);
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnLand() {
			if (IsDestroyed)
				return;

			// Collide with monsters.
			for (int i = 0; i < RoomControl.Entities.Count; i++) {
				Monster monster = RoomControl.Entities[i] as Monster;
				if (monster != null && physics.IsSoftMeetingEntity(monster)) {
					monster.TriggerInteraction(monster.HandlerSeeds[(int) type], this);
					if (IsDestroyed)
						return;
				}
			}

			// Notify the tile of the seed hitting it.
			Point2I location = RoomControl.GetTileLocation(position);
			Tile tile = RoomControl.GetTopTile(location);
			if (tile != null) {
				tile.OnSeedHit(type, this);
				if (IsDestroyed)
					return;
			}

			DestroyWithEffect(type, Center);
		}

		public override void Initialize() {
			base.Initialize();
			Graphics.PlaySprite(GameData.SPR_ITEM_SEEDS[(int) type]);
		}
	}
}
