﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Entities.Effects;
using ZeldaOracle.Game.Entities.Monsters;
using ZeldaOracle.Game.Tiles;

namespace ZeldaOracle.Game.Entities.Projectiles.Seeds {
	
	// Seeds shot from the seed-shooter or slingshot.
	public class SeedProjectile : SeedEntity {

		private int reboundCounter;
		private bool reboundOffWalls;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public SeedProjectile(SeedType type, bool reboundOffWalls) :
			base(type)
		{
			// Physics.
			Physics.CollisionBox		= new Rectangle2F(-1, -5, 2, 1);
			Physics.SoftCollisionBox	= new Rectangle2F(-1, -5, 2, 1);
			EnablePhysics(
				PhysicsFlags.DestroyedOutsideRoom |
				PhysicsFlags.CollideWorld |
				PhysicsFlags.HalfSolidPassable |
				PhysicsFlags.LedgePassable);

			this.reboundOffWalls = reboundOffWalls;
			if (reboundOffWalls)
				Physics.Flags |= PhysicsFlags.ReboundSolid;

			// Graphics.
			Graphics.DepthLayer	= DepthLayer.InAirSeed;
			Graphics.DrawOffset	= new Point2I(-4, -6);
			centerOffset		= new Point2I(0, -2);
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void Initialize() {
			base.Initialize();
			reboundCounter = 0;
			Graphics.PlaySprite(GameData.SPR_ITEM_SEEDS[(int) type]);
			CheckInitialCollision();
		}

		public override void OnCollideTile(Tile tile, bool isInitialCollision) {
			// Count the number of rebounds.
			if (reboundOffWalls && !isInitialCollision) {
				reboundCounter++;
				if (!(reboundCounter >= 3 || (tile != null && tile.Flags.HasFlag(TileFlags.AbsorbSeeds)))) {
					return;
				}
			}

			// Move 3 pixels into the block from where it collided.
			if (!isInitialCollision)
				position += Physics.PreviousVelocity.Normalized * 3.0f;

			DestroyWithEffect();
		}

		public override void OnCollideMonster(Monster monster) {
			monster.OnSeedHit(this);
		}
	}
}