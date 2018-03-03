using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Entities.Collisions;
using ZeldaOracle.Game.Entities.Monsters;
using ZeldaOracle.Game.Tiles;

namespace ZeldaOracle.Game.Entities.Projectiles.Seeds {
	
	/// <summary>Seeds shot from the seed-shooter or slingshot.</summary>
	public class SeedProjectile : SeedEntity {

		/// <summary>True if the projectile should rebound off of walls and seed
		/// bouncer tiles, crashing after a certain number of rebounds.</summary>
		private bool reboundOffWalls;
		/// <summary>Counts the number of rebounds.</summary>
		private int reboundCounter;
		/// <summary>The current tile location of the projectile, used to detect when
		/// it moves into a different tile, in order to check for a new seed bouncer
		/// tile.</summary>
		private Point2I tileLocation;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------
		
		/// <summary>Create a seed projectile of the given seed type. Seeds shot from
		/// the seed shooter should specify reboundOffWalls as true, while seeds shot
		/// from the slingshot should specify it as false.</summary>
		public SeedProjectile(SeedType type, bool reboundOffWalls) :
			base(type)
		{
			// Graphics
			Graphics.DepthLayer	= DepthLayer.InAirSeed;
			Graphics.DrawOffset	= new Point2I(-4, -6);
			centerOffset		= new Point2I(0, -2);

			// Physics
			Physics.CollisionBox = new Rectangle2F(-1, -5, 2, 1);
			Physics.Enable(
				PhysicsFlags.DestroyedOutsideRoom |
				PhysicsFlags.CollideWorld |
				PhysicsFlags.HalfSolidPassable |
				PhysicsFlags.LedgePassable |
				PhysicsFlags.DisableSurfaceContact);
			if (reboundOffWalls)
				Physics.ReboundSolid = true;

			// Interactions
			Interactions.Enable();
			Interactions.InteractionBox = new Rectangle2F(-4, -9, 8, 10);
			
			// Seed Projectile
			this.reboundOffWalls = reboundOffWalls;
		}
		

		//-----------------------------------------------------------------------------
		// Seed Bouncing
		//-----------------------------------------------------------------------------

		/// <summary>Collide with a seed bouncer tile, and either bounce, crash, or
		/// pass through it.</summary>
		private void CollideWithSeedBouncer(TileSeedBouncer seedBouncerTile) {
			// Increment the rebound counter
			if (reboundOffWalls) {
				if (IncrementRebound(1))
					CrashOnCollision(true);
			}

			// Try to bounce off the tile
			AttemptBounce(seedBouncerTile);

			// TODO: A 90 degree bounce will increment the rebound counter twice
		}
		
		/// <summary>Try to bounce off of a seed bouncer tile, returning true if the
		/// bounce was successful.</summary>
		private bool AttemptBounce(TileSeedBouncer seedBouncerTile) {
			// Determine the angle the we are moving in
			angle = Angle.FromVector(physics.Velocity);

			// Determine the angle to bounce off at
			Angle newAngle = Angle.Invalid;
			Angle bouncerAngle = seedBouncerTile.Angle;
			Angle plus1 = angle.Rotate(1, WindingOrder.Clockwise);
			Angle minus1 = angle.Rotate(1, WindingOrder.CounterClockwise);
			Angle perpendicular = angle.Rotate(2, WindingOrder.Clockwise);
			if (perpendicular == bouncerAngle ||
				perpendicular == bouncerAngle.Reverse())
				newAngle = angle.Reverse();
			else if (plus1 == bouncerAngle || plus1 == bouncerAngle.Reverse())
				newAngle = angle.Rotate(2, WindingOrder.Clockwise);
			else if (minus1 == bouncerAngle || minus1 == bouncerAngle.Reverse())
				newAngle = angle.Rotate(2, WindingOrder.CounterClockwise);
			
			// Start moving in the new angle
			if (newAngle.IsValid) {
				angle = newAngle;
				physics.Velocity = angle.ToVector(physics.Velocity.Length);
				return true;
			}
			return false;
		}


		/// <summary>Increment the rebound counter by the given amount. Returns true if
		/// the projectile has rebounded enough times and should now crash.</summary>
		private bool IncrementRebound(int count) {
			if (reboundOffWalls) {
				reboundCounter += count;
				if (reboundCounter >= GameSettings.SEED_PROJECTILE_REBOUND_COUNT)
					return true;
			}
			return false;
		}

		/// <summary>Crash into a collision. If penetrateIntoWall is true, then the
		/// projectile will first move 3 pixels deeper into the wall.</summary>
		private void CrashOnCollision(bool penetrateIntoWall) {
			// Move 3 pixels into the block from where it collided
			if (penetrateIntoWall)
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
				Interactions.InteractionBox = Physics.CollisionBox;
			}
			else {
				Physics.CollisionBox = new Rectangle2F(-1, -5, 2, 1);
				Interactions.InteractionBox = Physics.CollisionBox;
			}

			reboundCounter = 0;
			tileLocation = new Point2I(-1, -1);

			// Use the proper interaction and sprite for the current seed type
			Interactions.InteractionType =
				InteractionComponent.GetSeedInteractionType(SeedType);
			Graphics.PlayAnimation(GameData.SPR_ITEM_SEEDS[(int) SeedType]);
		}

		public override void OnCollideSolid(Collision collision) {
			bool absorbSeeds = (collision.IsTile &&
				collision.Tile.Flags.HasFlag(TileFlags.AbsorbSeeds));

			// Keep track of number of rebounds
			if (reboundOffWalls && collision.IsResolved && !absorbSeeds) {
				if (IncrementRebound(1)) {
					CrashOnCollision(collision.IsResolved);
					return;
				}

				// Reset the tile-location in case we rebounded off of a solid tile
				// right next to a seed bouncer. This way, the seed will be able to
				// rebound again off of that same seed bouncer even though it never
				// changed tile locations.
				if (!collision.IsTile || !(collision.Tile is TileSeedBouncer))
					tileLocation = new Point2I(-1, -1);
			}
			
			// Crash into the tile
			if (!reboundOffWalls || !collision.IsResolved || absorbSeeds)
				CrashOnCollision(collision.IsResolved);
		}

		public override void Update() {
			Point2I newTileLocation = (Point2I)
				(Physics.PositionedCollisionBox.Center / GameSettings.TILE_SIZE);

			// Check if tile location has changed, to then check for rebounding 
			// off a seed bouncer tile.
			if (newTileLocation != tileLocation) {
				Rectangle2F tileBounds = new Rectangle2F(
					newTileLocation * GameSettings.TILE_SIZE,
					new Vector2F(GameSettings.TILE_SIZE));

				// Only change tile locations if the collision box is fully contained
				// inside the tile
				if (tileBounds.Contains(physics.PositionedCollisionBox)) {
					tileLocation = newTileLocation;
					
					// Check for seed bouncers in the new location
					foreach (Tile tile in
						RoomControl.TileManager.GetTilesAtLocation(tileLocation))
					{
						TileSeedBouncer seedBouncer = tile as TileSeedBouncer;
						if (seedBouncer != null) {
							CollideWithSeedBouncer(seedBouncer);
							break;
						}
					}
				}
			}

			base.Update();
		}
	}
}
