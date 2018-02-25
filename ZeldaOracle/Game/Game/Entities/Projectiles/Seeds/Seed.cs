﻿using System;
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

namespace ZeldaOracle.Game.Entities.Projectiles.Seeds {

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
			foreach (Monster monster in Physics.GetEntitiesMeeting<Monster>(CollisionBoxType.Soft)) {
				monster.OnSeedHit(this);
				if (IsDestroyed)
					return;
			}

			// Notify the tile of the seed hitting it.
			Point2I location = RoomControl.GetTileLocation(position);
			Tile tile = RoomControl.GetTopTile(location);
			if (tile != null) {
				tile.OnHitByProjectile(this);
				if (IsDestroyed)
					return;
			}

			// Spawn the seed effect.
			Entity effect = DestroyWithSatchelEffect();

			if (type == SeedType.Scent) {
				if (RoomControl.IsSideScrolling)
					effect.Position += new Vector2F(0, 3);
			}
			else if (type == SeedType.Ember) {
				if (RoomControl.IsSideScrolling)
					effect.Position += new Vector2F(0, 1);
				effect.Physics.HasGravity = false;
			}
			else if (type == SeedType.Mystery) {
				if (RoomControl.IsSideScrolling)
					effect.Position += new Vector2F(0, -1);
			}
		}

		public override void Initialize() {
			base.Initialize();
			
			if (RoomControl.IsSideScrolling) {
				Physics.CollisionBox		= new Rectangle2F(-1, -1, 2, 3);
				Physics.CollideWithWorld	= true;
			}
			else {
				Physics.CollisionBox = new Rectangle2F(-1, -1, 2, 2);
			}
			Physics.SoftCollisionBox = Physics.CollisionBox;

			Graphics.PlayAnimation(GameData.SPR_ITEM_SEEDS[(int) type]);
		}
	}
}
