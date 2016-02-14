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

namespace ZeldaOracle.Game.Entities.Projectiles.Seeds {
	
	// Seeds shot from the seed-shooter or slingshot.
	public class SeedProjectile : SeedEntity {

		private int reboundCounter;
		private bool reboundOffWalls;
		private Point2I tileLocation;


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
		// Seed Bouncing
		//-----------------------------------------------------------------------------
		
		// Try to bounce off of a seed bouncer tile, returning true if the bounce was successful.
		private bool AttemptBounce(TileSeedBouncer seedBouncer) {
			// Determine the angle the we are moving in.
			angle = Angles.NearestFromVector(physics.Velocity);

			// Determine the angle to bounce off at.
			int newAngle = -1;
			int bouncerAngle = seedBouncer.Angle;
			int bouncerAngleReverse	= Angles.Reverse(seedBouncer.Angle);
			int plus1  = Angles.Add(angle, 1, WindingOrder.Clockwise);
			int plus2  = Angles.Add(angle, 2, WindingOrder.Clockwise);
			int minus1 = Angles.Add(angle, 1, WindingOrder.CounterClockwise);
			if (plus2 == bouncerAngle || plus2 == bouncerAngleReverse)
				newAngle = Angles.Reverse(angle);
			else if (plus1 == bouncerAngle || plus1 == bouncerAngleReverse)
				newAngle = Angles.Add(angle, 2, WindingOrder.Clockwise);
			else if (minus1 == bouncerAngle || minus1 == bouncerAngleReverse)
				newAngle = Angles.Add(angle, 2, WindingOrder.CounterClockwise);
			
			// Start moving in the new angle.
			if (newAngle >= 0) {
				angle = newAngle;
				physics.Velocity = Angles.ToVector(angle) * physics.Velocity.Length;
				return true;
			}
			return false;
		}

		private void CrashOnTile(Tile tile, bool isInitialCollision) {
			// Move 3 pixels into the block from where it collided.
			if (!isInitialCollision)
				position += Physics.PreviousVelocity.Normalized * 3.0f;
			DestroyWithEffect();
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void Initialize() {
			base.Initialize();
			
			if (RoomControl.IsSideScrolling) {
				Physics.CollisionBox = new Rectangle2F(-1, 0, 2, 1);
			}
			else {
				Physics.CollisionBox = new Rectangle2F(-1, -5, 2, 1);
			}
			Physics.SoftCollisionBox = Physics.CollisionBox;

			reboundCounter = 0;
			tileLocation = new Point2I(-1, -1);
			Graphics.PlaySprite(GameData.SPR_ITEM_SEEDS[(int) type]);
			CheckInitialCollision();
		}

		public override void OnCollideTile(Tile tile, bool isInitialCollision) {
			// Keep track of number of rebounds.
			if (reboundOffWalls && !isInitialCollision && !tile.Flags.HasFlag(TileFlags.AbsorbSeeds)) {
				reboundCounter++;
				if (reboundCounter >= GameSettings.SEED_PROJECTILE_REBOUND_COUNT) {
					CrashOnTile(tile, false);
					return;
				}
				else if (!(tile is TileSeedBouncer))
					tileLocation = new Point2I(-1, -1);
			}

			// Bounce off of seed bouncers.
			if (tile is TileSeedBouncer) {
				if (AttemptBounce((TileSeedBouncer) tile))
					return;
			}
			
			// Crash into the tile.
			if (!reboundOffWalls || isInitialCollision || tile.Flags.HasFlag(TileFlags.AbsorbSeeds))
				CrashOnTile(tile, isInitialCollision);
		}

		public override void OnCollideMonster(Monster monster) {
			monster.OnSeedHit(this);
		}

		public override void Update() {
			Point2I loc = (Point2I) (Physics.PositionedCollisionBox.Center / GameSettings.TILE_SIZE);

			// Check if tile location has changed.
			if (loc != tileLocation) {
				Rectangle2F rect = new Rectangle2F(loc * GameSettings.TILE_SIZE,
					new Vector2F(GameSettings.TILE_SIZE, GameSettings.TILE_SIZE));

				// Only change tile locations if the collision box is fully contained inside the tile.
				if (rect.Contains(physics.PositionedCollisionBox)) {
					tileLocation = loc;
					
					// Check for seed bouncers in the new location.
					foreach (Tile tile in RoomControl.TileManager.GetTilesAtLocation(tileLocation)) {
						TileSeedBouncer seedBouncer = tile as TileSeedBouncer;
						if (seedBouncer != null) {
							OnCollideTile(seedBouncer, false);
							break;
						}
					}
				}
			}

			base.Update();
		}
	}
}
