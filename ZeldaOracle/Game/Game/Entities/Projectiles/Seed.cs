using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Entities.Collisions;
using ZeldaOracle.Game.Entities.Effects;
using ZeldaOracle.Game.Entities.Monsters;
using ZeldaOracle.Game.Tiles;

namespace ZeldaOracle.Game.Entities.Projectiles {

	// Seeds dropped from the satchel.
	public class Seed : SeedEntity, IInterceptable {

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
			Graphics.DepthLayer	= DepthLayer.InAirSeed;
			Graphics.DrawOffset	= new Point2I(-4, -6);
			centerOffset		= new Point2I(0, -2);
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------
		
		public override void OnLand() {
			if (IsDestroyed)
				return;

			// Collide with monsters.
			CollisionIterator iterator = new CollisionIterator(this, typeof(Monster), CollisionBoxType.Soft);
			for (iterator.Begin(); iterator.IsGood(); iterator.Next()) {
				Monster monster = iterator.CollisionInfo.Entity as Monster;
				monster.OnSeedHit(this);
				if (IsDestroyed)
					return;
			}

			// Notify the tile of the seed hitting it.
			Point2I location = RoomControl.GetTileLocation(position);
			Tile tile = RoomControl.GetTopTile(location);
			if (tile != null) {
				tile.OnSeedHit(type, this);
				if (IsDestroyed)
					return;
			}

			// Spawn the seed effect.
			DestroyWithSatchelEffect();
		}

		public override void Initialize() {
			base.Initialize();
			Graphics.PlaySprite(GameData.SPR_ITEM_SEEDS[(int) type]);
		}
	}
}
