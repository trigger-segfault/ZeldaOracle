using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Control;
using ZeldaOracle.Game.Entities.Collisions;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Worlds;

namespace ZeldaOracle.Game.Entities {
	
	[Flags]
	public enum PhysicsFlags {
		None					= 0,
		Solid					= 0x1,		// The entity is solid (other entities can collide with this one).
		HasGravity				= 0x2,		// The entity is affected by gravity.
		CollideWorld			= 0x4,		// Collide with solid tiles.
		CollideEntities			= 0x8,		// Collide with solid entities.
		CollideRoomEdge			= 0x10,		// Colide with the edges of the room.
		ReboundSolid			= 0x20,		// Rebound off of solids.
		ReboundRoomEdge			= 0x40,		// Rebound off of room edges.
		Bounces					= 0x80,		// The entity bounces when it hits the ground.
		DestroyedOutsideRoom	= 0x100,	// The entity is destroyed when it is outside of the room.
		DestroyedInHoles		= 0x200,	// The entity gets destroyed in holes.
		LedgePassable			= 0x400,	// The entity can pass over ledges.
		HalfSolidPassable		= 0x800,	// The entity can pass over half-solids (railings).
		AutoDodge				= 0x1000,	// Will move out of the way when colliding with the edges of objects.
		PassableToOthers		= 0x2000,	// Other entities are unable to check collisions with the entity.
		MoveWithConveyors		= 0x4000,	// Moves with conveyor tiles.
		MoveWithPlatforms		= 0x8000,	// Moves with moving platform tiles.
		Crushable				= 0x10000,
		EdgeClipping			= 0x20000,	// Allow clipping into the edges of tiles.
	}

	public enum CollisionBoxType {
		Hard	= 0,
		Soft	= 1,
		Custom	= 2,
	}


	public delegate void EntityCollisionHandler(Entity entity);

	public class EntityCollisionHandlerInstance {
		private Type entityType;
		private EntityCollisionHandler collisionHandler;
		private CollisionBoxType collisionBoxType;

		public EntityCollisionHandlerInstance(Type entityType, EntityCollisionHandler handler, CollisionBoxType collisionBoxType) {
			this.entityType = entityType;
			this.collisionHandler = handler;
			this.collisionBoxType = collisionBoxType;
		}
		
		public Type EntityType {
			get { return entityType; }
			set { entityType = value; }
		}
		
		public EntityCollisionHandler CollisionHandler {
			get { return collisionHandler; }
			set { collisionHandler = value; }
		}

		public CollisionBoxType CollisionBoxType {
			get { return collisionBoxType; }
			set { collisionBoxType = value; }
		}
	}


	public class PhysicsComponent {

		public delegate bool TileCollisionCondition(Tile tile);

		// General.
		private Entity					entity;				// The entity this component belongs to.
		private bool					isEnabled;			// Are physics enabled for the entity?
		private PhysicsFlags			flags;
		private float					gravity;			// Gravity in pixels per frame^2
		private float					maxFallSpeed;
		private Vector2F				velocity;			// XY-Velocity in pixels per frame.
		private float					zVelocity;			// Z-Velocity in pixels per frame.
		
		private int						crushMaxGapSize;
		private int						edgeClipAmount;

		// Collision.
		private Rectangle2F				collisionBox;		// The "hard" collision box, used to collide with solid entities/tiles.
		private Rectangle2F				softCollisionBox;	// The "soft" collision box, used to collide with items, monsters, room edges, etc.
		private int						autoDodgeDistance;	// The maximum distance allowed to dodge collisions.
		private Action					customCollisionFunction;
		private TileCollisionCondition	customTileCollisionCondition;
		private CollisionBoxType		roomEdgeCollisionBoxType;
		//private List<EntityCollisionHandlerInstance> entityCollisionHandlers;

		// Internal physics state.
		private Vector2F			previousVelocity;	// XY-Velocity before physics update is called.
		private float				previousZVelocity;	// Z-Velocity before physics update is called.
		private Vector2F			reboundVelocity;
		private bool				isColliding;
		private CollisionInfo[]		collisionInfo;
		private bool				hasLanded;
		private Tile				topTile;			// The top-most tile the entity is located over.
		private int					ledgeAltitude;		// How many ledges the entity has passed over.
		private Point2I				ledgeTileLocation;	// The tile location of the ledge we are currently passing over, or (-1, -1) if not passing over ledge.

		// Events:
		//private event Action		eventBounce;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		// By default, physics are disabled.
		public PhysicsComponent(Entity entity) {
			this.isEnabled			= false;
			this.flags				= PhysicsFlags.None;
			this.entity				= entity;
			this.velocity			= Vector2F.Zero;
			this.zVelocity			= 0.0f;
			this.previousVelocity	= Vector2F.Zero;
			this.previousZVelocity	= 0.0f;
			this.gravity			= GameSettings.DEFAULT_GRAVITY;
			this.maxFallSpeed		= GameSettings.DEFAULT_MAX_FALL_SPEED;
			this.collisionBox		= new Rectangle2F(-1, -1, 2, 2);
			this.softCollisionBox	= new Rectangle2F(-1, -1, 2, 2);
			this.topTile			= null;
			this.isColliding		= false;
			this.autoDodgeDistance	= 6;
			this.customCollisionFunction	= null;
			//this.entityCollisionHandlers	= new List<EntityCollisionHandlerInstance>();
			this.hasLanded			= false;
			this.reboundVelocity	= Vector2F.Zero;
			this.ledgeAltitude		= 0;
			this.ledgeTileLocation	= new Point2I(-1, -1);
			this.roomEdgeCollisionBoxType = CollisionBoxType.Hard;

			this.crushMaxGapSize	= 0;
			this.edgeClipAmount		= 1;

			this.collisionInfo = new CollisionInfo[Directions.Count];
			for (int i = 0; i < Directions.Count; i++)
				collisionInfo[i].Clear();

			this.CollisionInfoNew = new CollisionInfoNew[4];
			for (int i = 0; i < 4; i++)
				CollisionInfoNew[i] = new CollisionInfoNew();
		}
		

		//-----------------------------------------------------------------------------
		// Custom Collision Setup
		//-----------------------------------------------------------------------------

		public Rectangle2F GetCollisionBox(CollisionBoxType type) {
			if (type == CollisionBoxType.Hard)
				return collisionBox;
			return softCollisionBox;
		}

		/*
		public void AddCollisionHandler(Type entityType, CollisionBoxType collisionBoxType, EntityCollisionHandler handler) {
			entityCollisionHandlers.Add(new EntityCollisionHandlerInstance(
					entityType, handler, collisionBoxType));
		}
		
		public void HandleEntityCollisions(Type entityType, CollisionBoxType collisionBoxType, EntityCollisionHandler handler) {
			for (int i = 0; i < entity.RoomControl.EntityCount; i++) {
				Entity e = entity.RoomControl.Entities[i];
				if (e.GetType().IsAssignableFrom(entityType) && IsMeetingEntity(e, collisionBoxType)) {
					handler(e);
					if (entity.IsDestroyed)
						return;
				}
			}
		}
		*/
		
		//-----------------------------------------------------------------------------
		// Flags
		//-----------------------------------------------------------------------------
		
		public bool HasFlags(PhysicsFlags flags) {
			return ((this.flags & flags) == flags);
		}

		public void SetFlags(PhysicsFlags flagsToSet, bool enabled) {
			if (enabled)
				flags |= flagsToSet;
			else
				flags &= ~flagsToSet;
		}


		//-----------------------------------------------------------------------------
		// Update methods
		//-----------------------------------------------------------------------------

		public void Initialize() {
			previousVelocity  = velocity;
			previousZVelocity = zVelocity;
		}

		public void Update() {
			previousVelocity = velocity;
			previousZVelocity = zVelocity;

			// Remove frame collision state flags.
			hasLanded		= false;
			isColliding		= false;
			reboundVelocity	= Vector2F.Zero;
			for (int i = 0; i < Directions.Count; i++)
				collisionInfo[i].Clear();

			// Perform custom collision handling tests.
			/*if (entityCollisionHandlers.Count > 0) {
				PerformCustomCollisionHandling();
				if (entity.IsDestroyed)
					return;
			}*/

			// Update Z dynamics.
			UpdateZVelocity();

			// 1. Collide with solid tiles and entities.
			//if (HasFlags(PhysicsFlags.CollideWorld) || HasFlags(PhysicsFlags.CollideEntities))
				//CheckCollisions();

			// 2. Collide with room edges.
			if (HasFlags(PhysicsFlags.CollideRoomEdge) || HasFlags(PhysicsFlags.ReboundRoomEdge))
				CheckRoomEdgeCollisions(GetCollisionBox(roomEdgeCollisionBoxType));

			// 3. Custom collision function.
			if (customCollisionFunction != null) {
				customCollisionFunction.Invoke();
				if (entity.IsDestroyed)
					return;
			}

			// Apply velocity.
			//entity.Position += velocity;
			//velocity += reboundVelocity;

			// Check ledges.
			if (HasFlags(PhysicsFlags.LedgePassable))
				CheckLedges();

			// Keep track of the highest-layer tile the entity is positioned over.
			topTile = entity.RoomControl.TileManager
				.GetSurfaceTileAtPosition(entity.Position, MovesWithPlatforms);
			if (topTile != null) {
				// TODO: Integrate the surface tile's velocity into our
				// velocity rather than just moving position.
				if (MovesWithPlatforms)
					entity.Position += topTile.Velocity;
				if (MovesWithConveyors && IsOnGround)
					entity.Position += topTile.ConveyorVelocity;
			}

			// Check if destroyed outside room.
			if (HasFlags(PhysicsFlags.DestroyedOutsideRoom) &&
				!entity.RoomControl.RoomBounds.Contains(entity.Position))
			{
				entity.Destroy();
				return;
			}
			
			// Check if in hazard tiles.
			if (IsInHole)
				entity.OnFallInHole();
			else if (IsInWater)
				entity.OnFallInWater();
			else if (IsInLava)
				entity.OnFallInLava();
			if (entity.IsDestroyed)
				return;
			
			if (hasLanded)
				entity.OnLand();
		}

		// Update ledge passing, handling changes in altitude.
		private void CheckLedges() {
			Point2I prevLocation = entity.RoomControl.GetTileLocation(entity.PreviousPosition + collisionBox.Center);
			Point2I location = entity.RoomControl.GetTileLocation(entity.Position + collisionBox.Center);

			// When moving over a new tile, check its ledge state.
			if (location != prevLocation) {
				ledgeTileLocation = new Point2I(-1, -1);

				if (entity.RoomControl.IsTileInBounds(location)) {
					Tile tile = entity.RoomControl.GetTopTile(location);

					if (tile != null && tile.IsLedge) {
						ledgeTileLocation = location;
						// Adjust ledge altitude.
						if (IsGoingUpLedge(tile))
							ledgeAltitude--;
						else if (IsGoingDownLedge(tile))
							ledgeAltitude++;
					}
				}
			}
		}

		// Update the z-velocity, position, and gravity of the entity.
		private void UpdateZVelocity() {
			if (entity.ZPosition > 0.0f || zVelocity != 0.0f) {
				// Apply gravity.
				if (HasFlags(PhysicsFlags.HasGravity)) {
					zVelocity -= gravity;
					if (zVelocity < -maxFallSpeed && maxFallSpeed >= 0)
						zVelocity = -maxFallSpeed;
				}

				// Apply z-velocity.
				entity.ZPosition += zVelocity;

				// Check if landed on the ground.
				if (entity.ZPosition <= 0.0f) {
					hasLanded = true;
					entity.ZPosition = 0.0f;

					if (HasFlags(PhysicsFlags.Bounces)) {
						Bounce();
					}
					else {
						entity.ZPosition = 0.0f;
						zVelocity = 0.0f;
					}
				}
			}
			else
				zVelocity = 0.0f;
		}
		
		private void Bounce() {
			if (IsInHole) {
				entity.OnFallInHole();
			}
			else if (IsInWater) {
				entity.OnFallInWater();
			}
			else if (IsInLava) {
				entity.OnFallInLava();
			}
			if (entity.IsDestroyed)
				return;

			if (zVelocity < -1.0f) {
				// Bounce back into the air.
				hasLanded = false;
				entity.ZPosition = 0.1f;
				zVelocity = -zVelocity * 0.5f;
			}
			else {
				// Stay on the ground.
				entity.ZPosition = 0.0f;
				zVelocity = 0;
				velocity = Vector2F.Zero;
			}

			if (velocity.Length > 0.25)
				velocity *= 0.5f;
			else
				velocity = Vector2F.Zero;

			
			entity.OnBounce();
		}
		/*
		private void PerformCustomCollisionHandling() {
			for (int i = 0; i < entity.RoomControl.EntityCount; i++) {
				Entity e = entity.RoomControl.Entities[i];
				for (int j = 0; j < entityCollisionHandlers.Count; j++) {
					EntityCollisionHandlerInstance handler = entityCollisionHandlers[j];
					if (e.GetType().IsAssignableFrom(handler.EntityType) &&
						IsMeetingEntity(e, handler.CollisionBoxType))
					{
						entityCollisionHandlers[j].CollisionHandler(e);
						if (entity.IsDestroyed)
							return;
					}
				}
			}
		}*/
		
		
		//-----------------------------------------------------------------------------
		// Collision Iteration
		//-----------------------------------------------------------------------------
		
		public IEnumerable<Tile> GetTilesMeeting(CollisionBoxType collisionBoxType) {
			return GetTilesMeeting(entity.Position, collisionBoxType);
		}
		
		// Return a list of tiles colliding with this entity (solidity is ignored).
		public IEnumerable<Tile> GetTilesMeeting(Vector2F position, CollisionBoxType collisionBoxType) {
			// Find the rectangular area of nearby tiles to collide with.
			Rectangle2F myBox = GetCollisionBox(collisionBoxType);
			myBox.Point += position;
			myBox.Inflate(2, 2);
			Rectangle2I area = entity.RoomControl.GetTileAreaFromRect(myBox, 1);
			
			// Iterate the tiles in the area.
			foreach (Tile t in entity.RoomControl.GetTilesInArea(area)) {
				if (t.CollisionModel != null &&
					CollisionModel.Intersecting(t.CollisionModel, t.Position, collisionBox, position))
				{
					yield return t;
				}
			}
		}

		// Return a list of entities colliding with this entity.
		public IEnumerable<T> GetEntitiesMeeting<T>(CollisionBoxType collisionBoxType, int maxZDistance = 10) where T : Entity {
			CollisionTestSettings settings = new CollisionTestSettings(typeof(T), collisionBoxType, maxZDistance);
			for (int i = 0; i < entity.RoomControl.Entities.Count; i++) {
				T other = entity.RoomControl.Entities[i] as T;
				if (other != null && CollisionTest.PerformCollisionTest(entity, other, settings).IsColliding)
					yield return other;
			}
		}


		//-----------------------------------------------------------------------------
		// Collision polls
		//-----------------------------------------------------------------------------

		// Is it possible for the entity to collide with the given tile?
		public bool CanCollideWithTile(Tile tile) {
			if (tile == null || tile.CollisionModel == null || !tile.IsSolid)
				return false;
			if (tile.IsHalfSolid && flags.HasFlag(PhysicsFlags.HalfSolidPassable))
				return false;
			if (customTileCollisionCondition != null)
				return customTileCollisionCondition(tile);
			return true;
		}

		// Is it possible for the entity to collide with the given tile?
		public bool CanCollideWithEntity(Entity other) {
			if (other == null)// || !other.Physics.IsEnabled)
				return false;
			return true;
		}
		
		// Return true if the entity would collide with a solid object using the
		// given collision box if it were placed at the given position.
		public bool IsPlaceMeetingSolid(Vector2F position, Rectangle2F collisionBox) {
			Room room = entity.RoomControl.Room;
			
			// Find the rectangular area of nearby tiles to collide with.
			Rectangle2F myBox = collisionBox;
			myBox.Point += position;
			myBox.Inflate(2, 2);
	
			int x1 = (int) (myBox.Left   / (float) GameSettings.TILE_SIZE);
			int y1 = (int) (myBox.Top    / (float) GameSettings.TILE_SIZE);
			int x2 = (int) (myBox.Right  / (float) GameSettings.TILE_SIZE) + 1;
			int y2 = (int) (myBox.Bottom / (float) GameSettings.TILE_SIZE) + 1;

			Rectangle2I area;
			area.Point	= (Point2I) (myBox.TopLeft / (float) GameSettings.TILE_SIZE);
			area.Size	= ((Point2I) (myBox.BottomRight / (float) GameSettings.TILE_SIZE)) + Point2I.One - area.Point;
			area.Inflate(1, 1);
			area = Rectangle2I.Intersect(area, new Rectangle2I(Point2I.Zero, room.Size));

			myBox.Inflate(-2, -2);
			
			foreach (Tile t in entity.RoomControl.GetTilesInArea(area)) {
				if (CanCollideWithTile(t)) {
					if (CollisionModel.Intersecting(t.CollisionModel, t.Position, collisionBox, position))
						return true;
				}
			}

			return false;
		}

		// Return true if the entity would collide with a tile if it were at the given position.
		public bool IsPlaceMeetingTile(Vector2F position, Tile tile) {
			if (CanCollideWithTile(tile)) {
				return CollisionModel.Intersecting(tile.CollisionModel, tile.Position, collisionBox, position);
			}
			return false;
		}
		
		// Return the solid tile that the entity is facing towards if it were at the given position.
		public Tile GetMeetingSolidTile(Vector2F position, int direction) {
			Vector2F checkPos = position + (Directions.ToVector(direction) * 1.0f);
			return GetTilesMeeting(checkPos, CollisionBoxType.Hard)
						.FirstOrDefault(tile =>
							CanCollideWithTile(tile) &&
							!CanDodgeCollision(tile, direction));
		}
		
		public bool IsMeetingEntity(Entity other, CollisionBoxType collisionBoxType, int maxZDistance = 10) {
			if (collisionBoxType == CollisionBoxType.Hard)
				return IsHardMeetingEntity(other);
			return IsSoftMeetingEntity(other, maxZDistance);
		}

		public bool IsSoftMeetingEntity(Entity other, int maxZDistance = 10) {
			if (CanCollideWithEntity(other) && GMath.Abs(entity.ZPosition - other.ZPosition) < maxZDistance)
				return PositionedSoftCollisionBox.Intersects(other.Physics.PositionedSoftCollisionBox);
			return false;
		}

		public bool IsHardMeetingEntity(Entity other) {
			if (CanCollideWithEntity(other))
				return PositionedCollisionBox.Intersects(other.Physics.PositionedCollisionBox);
			return false;
		}

		public bool IsSoftMeetingEntity(Entity other, Rectangle2F collisionBox, int maxZDistance = 10) {
			collisionBox.Point += entity.Position;
			if (CanCollideWithEntity(other) && GMath.Abs(entity.ZPosition - other.ZPosition) < maxZDistance)
				return collisionBox.Intersects(other.Physics.PositionedSoftCollisionBox);
			return false;
		}

		public bool IsCollidingWith(Entity other, CollisionBoxType collisionBoxType, int maxZDistance = 10) {
			return IsCollidingWith(other, collisionBoxType, collisionBoxType, maxZDistance);
		}

		public bool IsCollidingWith(Entity other, CollisionBoxType myBoxType, CollisionBoxType otherBoxType, int maxZDistance = 10) {
			return CollisionTest.PerformCollisionTest(entity, other,
				new CollisionTestSettings(null, myBoxType, otherBoxType, maxZDistance)).IsColliding;
		}

		private bool IsGoingDownLedge(Tile ledgeTile) {
			return velocity.Dot(Directions.ToVector(ledgeTile.LedgeDirection)) > 0.0f;
		}

		private bool IsGoingUpLedge(Tile ledgeTile) {
			return velocity.Dot(Directions.ToVector(ledgeTile.LedgeDirection)) < 0.0f;
		}


		//-----------------------------------------------------------------------------
		// Collisions
		//-----------------------------------------------------------------------------

		// Collide with the inside edges of a rectangle.
		// NOTE: At the moment, this is only used when player is doomed to fall in a hole.
		public void PerformInsideEdgeCollisions(Rectangle2F collisionBox, Rectangle2F rect) {
			Rectangle2F myBox = Rectangle2F.Translate(collisionBox, entity.Position);

			if (myBox.Left < rect.Left) {
				isColliding	= true;
				entity.X	= rect.Left - collisionBox.Left;
				velocity.X	= 0;
			}
			else if (myBox.Right > rect.Right) {
				isColliding	= true;
				entity.X	= rect.Right - collisionBox.Right;
				velocity.X	= 0;
			}
			if (myBox.Top < rect.Top) {
				isColliding	= true;
				entity.Y	= rect.Top - collisionBox.Top;
				velocity.Y	= 0;
			}
			else if (myBox.Bottom > rect.Bottom) {
				isColliding	= true;
				entity.Y	= rect.Bottom - collisionBox.Bottom;
				velocity.Y	= 0;
			}
		}

		// Check collisions with room edges.
		private void CheckRoomEdgeCollisions(Rectangle2F collisionBox) {
			Rectangle2F roomBounds = entity.RoomControl.RoomBounds;
			Rectangle2F myBox = Rectangle2F.Translate(collisionBox, entity.Position + velocity);

			if (myBox.Left < roomBounds.Left) {
				isColliding	= true;
				entity.X	= roomBounds.Left - collisionBox.Left;
				if (flags.HasFlag(PhysicsFlags.ReboundRoomEdge) && reboundVelocity.X == 0.0f)
					reboundVelocity.X = -velocity.X;
				velocity.X	= 0;
				collisionInfo[Directions.Left].SetRoomEdgeCollision(Directions.Left);
			}
			else if (myBox.Right > roomBounds.Right) {
				isColliding	= true;
				entity.X	= roomBounds.Right - collisionBox.Right;
				if (flags.HasFlag(PhysicsFlags.ReboundRoomEdge) && reboundVelocity.X == 0.0f)
					reboundVelocity.X = -velocity.X;
				velocity.X	= 0;
				collisionInfo[Directions.Right].SetRoomEdgeCollision(Directions.Right);
			}
			if (myBox.Top < roomBounds.Top) {
				isColliding	= true;
				entity.Y	= roomBounds.Top - collisionBox.Top;
				if (flags.HasFlag(PhysicsFlags.ReboundRoomEdge) && reboundVelocity.Y == 0.0f)
					reboundVelocity.Y = -velocity.Y;
				velocity.Y	= 0;
				collisionInfo[Directions.Up].SetRoomEdgeCollision(Directions.Up);
			}
			else if (myBox.Bottom > roomBounds.Bottom) {
				isColliding	= true;
				entity.Y	= roomBounds.Bottom - collisionBox.Bottom;
				if (flags.HasFlag(PhysicsFlags.ReboundRoomEdge) && reboundVelocity.Y == 0.0f)
					reboundVelocity.Y = -velocity.Y;
				velocity.Y	= 0;
				collisionInfo[Directions.Down].SetRoomEdgeCollision(Directions.Down);
			}
		}

		// Check collisions with solid tiles and entities.
		private void CheckCollisions() {
			// Find the rectangular area of nearby tiles to collide with.
			Rectangle2F myBox = PositionedCollisionBox;
			Rectangle2F myBoxNext = Rectangle2F.Translate(myBox, velocity);
			myBox = Rectangle2F.Union(myBox, myBoxNext);
			myBox.Inflate(2, 2);

			// Collide with nearby solid tiles HORIZONTALLY and then VERTICALLY.
			Rectangle2I area = entity.RoomControl.GetTileAreaFromRect(myBox, 1);
			Room room = entity.RoomControl.Room;


			// Handle circular tile collisions.
			if (HasFlags(PhysicsFlags.CollideWorld)) {
				foreach (Tile t in entity.RoomControl.GetTilesInArea(area)) {
					if (CanCollideWithTile(t) && t.CollisionStyle == CollisionStyle.Circular)
						ResolveCircularCollision(t, t.Position, t.CollisionModel);
				}
			}

			for (int axis = 0; axis < 2; ++axis) {
				// Collide with solid tiles.
				if (HasFlags(PhysicsFlags.CollideWorld)) {
					foreach (Tile t in entity.RoomControl.GetTilesInArea(area)) {
						if (CanCollideWithTile(t) && t.CollisionStyle == CollisionStyle.Rectangular)
							ResolveCollision(axis, t, t.Position, t.CollisionModel);
					}
				}

				// Collide with solid entities.
				if (flags.HasFlag(PhysicsFlags.CollideEntities)) {
					for (int i = 0; i < entity.RoomControl.EntityCount; i++) {
						Entity e = entity.RoomControl.Entities[i];
						if (e.Physics.IsEnabled && e.Physics.IsSolid) {
							if (CanCollideWithEntity(e))
								ResolveCollision(axis, e);
						}
					}
				}
			}
		}
		
		private bool ResolveCollision(int axis, Tile tile, Vector2F modelPos, CollisionModel model) {
			bool collide = false;
			for (int i = 0; i < model.Boxes.Count; ++i) {
				Rectangle2F box = Rectangle2F.Translate((Rectangle2F) model.Boxes[i], modelPos);
				if (ResolveCollision(axis, tile, box)) {
					collide = true;
				}
			}
			return collide;
		}

		private bool ResolveCollision(int axis, Tile tile, Rectangle2F block) {
			// Check if tile is a ledge we won't collide with.
			if (tile.IsLedge && flags.HasFlag(PhysicsFlags.LedgePassable)) {
				if (ledgeAltitude > 0 || tile.Location == ledgeTileLocation || IsGoingDownLedge(tile))
					return false;
			}

			if (axis == Axes.X) {
				Rectangle2F myBox = Rectangle2F.Translate(collisionBox, entity.X + velocity.X, entity.Y);
				if (myBox.Intersects(block)) {
					isColliding	= true;
					if (flags.HasFlag(PhysicsFlags.ReboundSolid) && reboundVelocity.X == 0.0f)
						reboundVelocity.X = -velocity.X;

					if (myBox.Center.X < block.Center.X) {
						// TODO: David refactor this hackish 'if statement' to prevent the player from jittering when getting pushed vertically.
						// See below if statement as well.
						if (tile.MoveDirection % 2 != 1) {
							entity.X = block.Left - collisionBox.Right;
							if (!HasFlags(PhysicsFlags.AutoDodge) || !PerformCollisionDodge(block, Directions.Right))
								collisionInfo[Directions.Right].SetTileCollision(tile, Directions.Right);
						}
					}
					else {
						if (tile.MoveDirection % 2 != 1) {
							entity.X = block.Right - collisionBox.Left;
							if (!HasFlags(PhysicsFlags.AutoDodge) || !PerformCollisionDodge(block, Directions.Left))
								collisionInfo[Directions.Left].SetTileCollision(tile, Directions.Left);
						}
					}

					velocity.X = 0.0f;
					return true;
				}
			}
			else if (axis == Axes.Y) {
				Rectangle2F myBox = Rectangle2F.Translate(collisionBox, entity.Position + velocity);
				if (myBox.Intersects(block)) {
					isColliding	= true;
					if (flags.HasFlag(PhysicsFlags.ReboundSolid) && reboundVelocity.Y == 0.0f)
						reboundVelocity.Y = -velocity.Y;

					if (myBox.Center.Y < block.Center.Y) {
						entity.Y = block.Top - collisionBox.Bottom;
						if (!HasFlags(PhysicsFlags.AutoDodge) || !PerformCollisionDodge(block, Directions.Down))
							collisionInfo[Directions.Down].SetTileCollision(tile, Directions.Down);
					}
					else {
						entity.Y = block.Bottom - collisionBox.Top;
						if (!HasFlags(PhysicsFlags.AutoDodge) || !PerformCollisionDodge(block, Directions.Up))
							collisionInfo[Directions.Up].SetTileCollision(tile, Directions.Up);
					}

					velocity.Y = 0.0f;
					return true;
				}
			}
			return false;
		}
		
		private bool ResolveCollision(int axis, Entity other) {
			Rectangle2F block = other.Physics.PositionedCollisionBox;

			if (axis == Axes.X) {
				Rectangle2F myBox = Rectangle2F.Translate(collisionBox, entity.X + velocity.X, entity.Y);
				if (myBox.Intersects(block)) {
					isColliding	= true;
					velocity.X	= 0.0f;

					if (myBox.Center.X < block.Center.X) {
						entity.X = block.Left - collisionBox.Right;
						collisionInfo[Directions.Right].SetEntityCollision(other, Directions.Right);
					}
					else {
						entity.X = block.Right - collisionBox.Left;
						collisionInfo[Directions.Left].SetEntityCollision(other, Directions.Left);
					}
					return true;
				}
			}
			else if (axis == Axes.Y) {
				Rectangle2F myBox = Rectangle2F.Translate(collisionBox, entity.Position + velocity);
				if (myBox.Intersects(block)) {
					isColliding	= true;
					velocity.Y	= 0.0f;

					if (myBox.Center.Y < block.Center.Y) {
						entity.Y = block.Top - collisionBox.Bottom;
						collisionInfo[Directions.Down].SetEntityCollision(other, Directions.Down);
					}
					else {
						entity.Y = block.Bottom - collisionBox.Top;
						collisionInfo[Directions.Up].SetEntityCollision(other, Directions.Up);
					}
					return true;
				}
			}
			return false;
		}
		
		private bool ResolveCircularCollision(Tile tile, Vector2F modelPos, CollisionModel model) {
			bool colliding = false;
			for (int i = 0; i < model.BoxCount; i++) {
				Rectangle2F box = model.Boxes[i];
				box.Point += modelPos;
				if (PositionedCollisionBox.Intersects(box)) {
					entity.Position += 1.0f * (entity.Position - box.Center).Normalized;
					colliding = true;
				}
			}
			return colliding;
		}


		//-----------------------------------------------------------------------------
		// Collision Dodging
		//-----------------------------------------------------------------------------

		public bool CanDodgeCollision(Tile tile, int direction) {
			if (!CanCollideWithTile(tile))
				return false;
			for (int i = 0; i < tile.CollisionModel.Boxes.Count; i++) {
				if (CanDodgeCollision(Rectangle2F.Translate(tile.CollisionModel.Boxes[i], tile.Position), direction)) {
					return true;
				}
			}
			return false;
		}

		public bool CanDodgeCollision(Rectangle2F block, int direction) {
			if (Math.Abs(velocity.X) > 0.001f && Math.Abs(velocity.Y) > 0.001f)
				return false; // Only dodge when moving horizontally or vertically.

			float		dodgeDist	= autoDodgeDistance;
			Rectangle2F	objBox		= Rectangle2F.Translate(collisionBox, entity.Position);
			Vector2F	pos			= entity.Position;
			Vector2F	dirVect		= Directions.ToVector(direction);

			for (int side = 0; side < 2; side++) {
				int moveDir		= (direction + (side == 0 ? 1 : 3)) % 4;
				float distance	= Math.Abs(objBox.GetEdge((moveDir + 2) % 4) - block.GetEdge(moveDir));

				if (distance <= dodgeDist) {
					Vector2F checkPos	= pos + dirVect + (Directions.ToVector(moveDir) * distance);
					Vector2F gotoPos	= GMath.Round(pos) + Directions.ToVector(moveDir);

					if (!IsPlaceMeetingSolid(checkPos, collisionBox) &&
						!IsPlaceMeetingSolid(gotoPos, collisionBox))
					{
						return true;
					}
				}
			}
			return false;
		}

		private bool PerformCollisionDodge(Rectangle2F block, int direction) {
			if (Math.Abs(velocity.X) > 0.001f && Math.Abs(velocity.Y) > 0.001f)
				return false; // Only dodge when moving horizontally or vertically.

			float		dodgeDist	= autoDodgeDistance;
			Rectangle2F	objBox		= Rectangle2F.Translate(collisionBox, entity.Position);
			Vector2F	pos			= entity.Position;
			Vector2F	dirVect		= Directions.ToVector(direction);

			for (int side = 0; side < 2; side++) {
				int moveDir		= (direction + (side == 0 ? 1 : 3)) % 4;
				float distance	= Math.Abs(objBox.GetEdge((moveDir + 2) % 4) - block.GetEdge(moveDir));

				if (distance <= dodgeDist) {
					Vector2F checkPos	= pos + dirVect + (Directions.ToVector(moveDir) * distance);
					Vector2F gotoPos	= GMath.Round(pos) + Directions.ToVector(moveDir);

					if (!IsPlaceMeetingSolid(checkPos, collisionBox) &&
						!IsPlaceMeetingSolid(gotoPos, collisionBox))
					{
						entity.Position = gotoPos;
						return true;
					}
				}
			}
			return false;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public Entity Entity {
			get { return entity; }
			set { entity = value; }
		}

		public bool IsEnabled {
			get { return isEnabled; }
			set {
				isEnabled = value;
				if (!isEnabled) {
					// Clear the collision state.
					for (int i = 0; i < Directions.Count; i++) {
						CollisionInfoNew[i].Reset();
						CollisionInfoNew[i].PenetrationDirection = i;
						collisionInfo[i].Clear();
					}
				}
			}
		}

		public Vector2F Velocity {
			get { return velocity; }
			set { velocity = value; }
		}

		public float VelocityX {
			get { return velocity.X; }
			set { velocity.X = value; }
		}

		public float VelocityY {
			get { return velocity.Y; }
			set { velocity.Y = value; }
		}

		public float ZVelocity {
			get { return zVelocity; }
			set { zVelocity = value; }
		}

		public Vector2F PreviousVelocity {
			get { return previousVelocity; }
		}

		public float PreviousZVelocity {
			get { return previousZVelocity; }
		}
		
		public float Gravity {
			get { return gravity; }
			set { gravity = value; }
		}

		public float MaxFallSpeed {
			get { return maxFallSpeed; }
			set { maxFallSpeed = value; }
		}

		public bool IsInAir {
			get { return (entity.ZPosition > 0.0f || zVelocity > 0.0f); }
		}

		public bool IsOnGround {
			get { return !IsInAir; }
		}
		
		public int AutoDodgeDistance {
			get { return autoDodgeDistance; }
			set { autoDodgeDistance = value; }
		}
		
		public CollisionBoxType RoomEdgeCollisionBoxType {
			get { return roomEdgeCollisionBoxType; }
			set { roomEdgeCollisionBoxType = value; }
		}
		
		public int EdgeClipAmount {
			get { return edgeClipAmount; }
			set { edgeClipAmount = value; }
		}

		public int CrushMaxGapSize {
			get { return crushMaxGapSize; }
			set { crushMaxGapSize = value; }
		}

		// Collision info.

		public Rectangle2F CollisionBox {
			get { return collisionBox; }
			set { collisionBox = value; }
		}

		public Rectangle2F SoftCollisionBox {
			get { return softCollisionBox; }
			set { softCollisionBox = value; }
		}

		public Rectangle2F PositionedCollisionBox {
			get { return Rectangle2F.Translate(collisionBox, entity.Position); }
		}
		
		public Rectangle2F PositionedSoftCollisionBox {
			get { return Rectangle2F.Translate(softCollisionBox, entity.Position); }
		}

		public Action CustomCollisionFunction {
			get { return customCollisionFunction; }
			set { customCollisionFunction = value; }
		}

		public TileCollisionCondition CustomTileCollisionCondition {
			get { return customTileCollisionCondition; }
			set { customTileCollisionCondition = value; }
		}
		
		public CollisionInfo[] CollisionInfo {
			get { return collisionInfo; }
		}

		public IEnumerable<CollisionInfo> GetCollisions() {
			for (int i = 0; i < collisionInfo.Length; i++) {
				if (collisionInfo[i].IsColliding)
					yield return collisionInfo[i];
			}
		}
		
		public bool IsColliding {
			get { return isColliding; }
			set { isColliding = value; }
		}


		public Tile TopTile {
			get { return topTile; }
			set { topTile = value; }
		}

		// Tile Flags.

		public bool IsInGrass {
			get { return IsOnGround && topTile != null && topTile.EnvironmentType == TileEnvironmentType.Grass; }
		}

		public bool IsInPuddle {
			get { return IsOnGround && topTile != null && topTile.EnvironmentType == TileEnvironmentType.Puddle; }
		}

		public bool IsInHole {
			get { return IsOnGround &&
				topTile != null && topTile.EnvironmentType == TileEnvironmentType.Hole ||
				topTile != null && topTile.EnvironmentType == TileEnvironmentType.Whirlpool; }
		}

		public bool IsInWater {
			get { return IsOnGround && (
				topTile != null && topTile.EnvironmentType == TileEnvironmentType.Water ||
				topTile != null && topTile.EnvironmentType == TileEnvironmentType.Ocean ||
				topTile != null && topTile.EnvironmentType == TileEnvironmentType.DeepWater); }
		}

		public bool IsInOcean {
			get { return IsOnGround && topTile != null && topTile.EnvironmentType == TileEnvironmentType.Ocean; }
		}

		public bool IsInLava {
			get { return IsOnGround && topTile != null && topTile.EnvironmentType == TileEnvironmentType.Lava; }
		}

		public bool IsOnIce {
			get { return IsOnGround && topTile != null && topTile.EnvironmentType == TileEnvironmentType.Ice; }
		}

		public bool IsOnStairs {
			get { return IsOnGround && topTile != null && topTile.EnvironmentType == TileEnvironmentType.Stairs; }
		}

		public bool IsOnLadder {
			get { return IsOnGround && topTile != null && topTile.EnvironmentType == TileEnvironmentType.Ladder; }
		}

		public bool IsOverHalfSolid {
			get { return (topTile != null && topTile.IsHalfSolid); }
		}

		public bool IsOverLedge {
			get { return (topTile != null && topTile.IsLedge); }
		}


		// Physics Flags:

		public PhysicsFlags Flags {
			get { return flags; }
			set { flags = value; }
		}
		
		public bool IsSolid {
			get { return HasFlags(PhysicsFlags.Solid); }
			set { SetFlags(PhysicsFlags.Solid, value); }
		}
		
		public bool HasGravity {
			get { return HasFlags(PhysicsFlags.HasGravity); }
			set { SetFlags(PhysicsFlags.HasGravity, value); }
		}

		public bool CollideWithWorld {
			get { return HasFlags(PhysicsFlags.CollideWorld); }
			set { SetFlags(PhysicsFlags.CollideWorld, value); }
		}
		
		public bool CollideWithEntities {
			get { return HasFlags(PhysicsFlags.CollideEntities); }
			set { SetFlags(PhysicsFlags.CollideEntities, value); }
		}

		public bool CollideWithRoomEdge {
			get { return HasFlags(PhysicsFlags.CollideRoomEdge); }
			set { SetFlags(PhysicsFlags.CollideRoomEdge, value); }
		}

		public bool ReboundSolid {
			get { return HasFlags(PhysicsFlags.ReboundSolid); }
			set { SetFlags(PhysicsFlags.ReboundSolid, value); }
		}
		
		public bool ReboundRoomEdge {
			get { return HasFlags(PhysicsFlags.ReboundRoomEdge); }
			set { SetFlags(PhysicsFlags.ReboundRoomEdge, value); }
		}

		public bool Bounces {
			get { return HasFlags(PhysicsFlags.Bounces); }
			set { SetFlags(PhysicsFlags.Bounces, value); }
		}
		
		public bool IsDestroyedOutsideRoom {
			get { return HasFlags(PhysicsFlags.DestroyedOutsideRoom); }
			set { SetFlags(PhysicsFlags.DestroyedOutsideRoom, value); }
		}
		
		public bool IsDestroyedInHoles {
			get { return HasFlags(PhysicsFlags.DestroyedInHoles); }
			set { SetFlags(PhysicsFlags.DestroyedInHoles, value); }
		}
		
		public bool PassOverLedges {
			get { return HasFlags(PhysicsFlags.LedgePassable); }
			set { SetFlags(PhysicsFlags.LedgePassable, value); }
		}
		
		public bool PassOverHalfSolids {
			get { return HasFlags(PhysicsFlags.HalfSolidPassable); }
			set { SetFlags(PhysicsFlags.HalfSolidPassable, value); }
		}
		
		public bool AutoDodges {
			get { return HasFlags(PhysicsFlags.AutoDodge); }
			set { SetFlags(PhysicsFlags.AutoDodge, value); }
		}

		public bool IsPassableToOthers {
			get { return HasFlags(PhysicsFlags.PassableToOthers); }
			set { SetFlags(PhysicsFlags.PassableToOthers, value); }
		}

		public bool MovesWithConveyors {
			get { return HasFlags(PhysicsFlags.MoveWithConveyors); }
			set { SetFlags(PhysicsFlags.MoveWithConveyors, value); }
		}

		public bool MovesWithPlatforms {
			get { return HasFlags(PhysicsFlags.MoveWithPlatforms); }
			set { SetFlags(PhysicsFlags.MoveWithPlatforms, value); }
		}
		
		public bool AllowEdgeClipping {
			get { return HasFlags(PhysicsFlags.EdgeClipping); }
			set { SetFlags(PhysicsFlags.EdgeClipping, value); }
		}
		
		public bool IsCrushable {
			get { return HasFlags(PhysicsFlags.Crushable); }
			set { SetFlags(PhysicsFlags.Crushable, value); }
		}
		



		public CollisionInfoNew[] CollisionInfoNew { get; set; }
	}
}
