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
		Flying					= 0x40000,	// Flying entities are never considered to be on the ground (even if their Z position is zero)


		CheckRadialCollisions	= 0x80000,

		// This was added for player ladder climbing in a side-scrolling room,
		// because when climbing a ladder, it is like the player is on the ground
		// even though he is not.
		OnGroundOverride		= 0x100000,	// If this is set, IsOnGround will always be true.
	}

	public enum CollisionBoxType {
		Hard	= 0,
		Soft	= 1,
		Custom	= 2,
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
		private Vector2F				surfacePosition;	// Used for draw position rounding to prevent jittering.
		private Vector2F				surfaceVelocity;	// Used for draw position rounding to prevent jittering.
		
		// Collision settings.
		private Rectangle2F				collisionBox;		// The "hard" collision box, used to collide with solid entities/tiles.
		private Rectangle2F				softCollisionBox;	// The "soft" collision box, used to collide with items, monsters, room edges, etc.
		private TileCollisionCondition	customTileIsSolidCondition;
		private TileCollisionCondition	customTileIsNotSolidCondition;
		private CollisionBoxType		roomEdgeCollisionBoxType;
		private int						autoDodgeDistance;	// The maximum distance allowed to dodge collisions.
		private float					autoDodgeSpeed;		// The speed to move at when dodging collisions.
		private int						edgeClipAmount;
		private int						crushMaxGapSize;
		private Vector2F				topTilePointOffset;

		// Internal physics state.
		private Vector2F			previousVelocity;	// XY-Velocity before physics update is called.
		private float				previousZVelocity;	// Z-Velocity before physics update is called.
		private Vector2F			reboundVelocity;
		private bool				isColliding;
		private CollisionInfo[]		collisionInfo;
		private CollisionInfo[]		previousCollisionInfo;
		private bool				hasLanded;
		private Tile				topTile;			// The top-most tile the entity is located over.
		private int					ledgeAltitude;		// How many ledges the entity has passed over.
		private Point2I				ledgeTileLocation;	// The tile location of the ledge we are currently passing over, or (-1, -1) if not passing over ledge.


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
			this.topTilePointOffset	= Vector2F.Zero;
			this.isColliding		= false;
			this.autoDodgeDistance	= 6;
			this.autoDodgeSpeed		= 1.0f;
			this.hasLanded			= false;
			this.reboundVelocity	= Vector2F.Zero;
			this.ledgeAltitude		= 0;
			this.ledgeTileLocation	= new Point2I(-1, -1);
			this.roomEdgeCollisionBoxType = CollisionBoxType.Hard;
			this.customTileIsSolidCondition = null;

			this.crushMaxGapSize	= 0;
			this.edgeClipAmount		= 1;

			this.collisionInfo = new CollisionInfo[Directions.Count];
			this.previousCollisionInfo = new CollisionInfo[Directions.Count];
			for (int i = 0; i < Directions.Count; i++) {
				collisionInfo[i].Clear();
				previousCollisionInfo[i].Clear();
			}

			this.MovementCollisions = new bool[4];
			this.ClipCollisionInfo = new CollisionInfoNew[4];
			for (int i = 0; i < 4; i++)
				ClipCollisionInfo[i] = new CollisionInfoNew();
		}
		

		//-----------------------------------------------------------------------------
		// Custom Collision Setup
		//-----------------------------------------------------------------------------

		public virtual Rectangle2F GetCollisionBox(CollisionBoxType type) {
			if (type == CollisionBoxType.Hard)
				return collisionBox;
			return softCollisionBox;
		}

		
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

		public void Update() {
			// Nothing left here....
			// All moved to RoomPhysics.
		}

		
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
			foreach (Tile t in entity.RoomControl.GetTopTilesInArea(area)) {
				if (t.CollisionModel != null &&
					CollisionModel.Intersecting(t.CollisionModel, t.Position,
					GetCollisionBox(collisionBoxType), position))
				{
					yield return t;
				}
			}
		}

		// Return a list of solid tiles colliding with this entity.
		public IEnumerable<Tile> GetSolidTilesMeeting(CollisionBoxType collisionBoxType) {
			foreach (Tile tile in GetTilesMeeting(entity.Position, collisionBoxType)) {
				if (CanCollideWithTile(tile))
					yield return tile;
			}
		}

		// Return a list of solid tiles colliding with this entity.
		public IEnumerable<Tile> GetSolidTilesMeeting(Vector2F position, CollisionBoxType collisionBoxType) {
			foreach (Tile tile in GetTilesMeeting(position, collisionBoxType)) {
				if (CanCollideWithTile(tile))
					yield return tile;
			}
		}

		// Return a list of solid entities colliding with this entity.
		public IEnumerable<T> GetSolidEntitiesMeeting<T>(CollisionBoxType collisionBoxType, int maxZDistance = 10) where T : Entity {
			CollisionTestSettings settings = new CollisionTestSettings(typeof(T), collisionBoxType, maxZDistance);
			for (int i = 0; i < entity.RoomControl.Entities.Count; i++) {
				T other = entity.RoomControl.Entities[i] as T;
				if (other != null && other.Physics.IsSolid && CollisionTest.PerformCollisionTest(entity, other, settings).IsColliding)
					yield return other;
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

		// Return a list of entities colliding with this entity.
		public IEnumerable<T> GetEntitiesMeeting<T>(Rectangle2I myCollisionBox, CollisionBoxType otherCollisionBoxType, int maxZDistance = 10) where T : Entity {
			CollisionTestSettings settings = new CollisionTestSettings(typeof(T), myCollisionBox, otherCollisionBoxType, maxZDistance);
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
			if (tile.IsAnyLedge && PassOverLedges) {
				if (ledgeAltitude > 0 ||
					ledgeTileLocation == tile.Location ||
					IsMovingDownLedge(tile))
					return false;
			}
			if (customTileIsNotSolidCondition != null)
				return customTileIsNotSolidCondition(tile);
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
		public bool IsPlaceMeetingSolid(Vector2F position) {
			return IsPlaceMeetingSolid(position, collisionBox);
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
		
		
		/// <summary>Return true if the entity would collide with the room edge if it
		/// were placed at the given position.</summary>
		public bool IsPlaceMeetingRoomEdge(Vector2F position) {
			Room room = entity.RoomControl.Room;
			Rectangle2F bounds = entity.RoomControl.RoomBounds;
			return !bounds.Contains(collisionBox + position);
		}

		// Return true if the entity would collide with a tile if it were at the given position.
		public bool IsPlaceMeetingTile(Vector2F position, Tile tile) {
			if (CanCollideWithTile(tile)) {
				return CollisionModel.Intersecting(tile.CollisionModel, tile.Position, collisionBox, position);
			}
			return false;
		}

		// Return true if the entity would collide with a tile if it were at the given position.
		public bool IsPlaceMeetingEntity(Vector2F position, Entity entity, CollisionBoxType collisionBoxType, float maxZDistance = 10) {
			if (CanCollideWithEntity(entity)) {
				return CollisionTest.PerformCollisionTest(entity, entity,
					new CollisionTestSettings(null, collisionBoxType,
					collisionBoxType, maxZDistance)).IsColliding;
			}
			return false;
		}
				
		// Return the closest solid tile positioned directly in front of the entity (if there is one).
		public Tile GetFacingSolidTile(int direction, float distance = 1.0f) { // TODO: return closest tile.
			Rectangle2F entityBox = PositionedCollisionBox;
			entityBox.ExtendEdge(direction, distance);
			Vector2F directionVector = Directions.ToVector(direction);

			Tile closestTile = null;
			int alignAxis = 1 - Directions.ToAxis(direction);
			float closestDistance = -1.0f;

			foreach (Tile tile in entity.RoomControl.TileManager.GetTilesTouching(entityBox)) {
				if (CanCollideWithTile(tile)) {
					foreach (Rectangle2F box in tile.CollisionModel.Boxes) {
						Rectangle2F solidBox = box;
						solidBox.Point += tile.Position;

						if (entityBox.Intersects(solidBox) &&
							!IsSafeClippingInDirection(solidBox, (direction + 1) % 4) &&
							!IsSafeClippingInDirection(solidBox, (direction + 2) % 4) &&
							!IsSafeClippingInDirection(solidBox, (direction + 3) % 4) &&
							!CanDodgeCollision(solidBox, direction) && 
							(solidBox.Center - entityBox.Center).Dot(directionVector) > 0.0f)
						{
							float distToEdge = Math.Max(
								entityBox.TopLeft[alignAxis] - solidBox.BottomRight[alignAxis],
								solidBox.TopLeft[alignAxis] - entityBox.BottomRight[alignAxis]);
							if (closestTile == null || distToEdge < closestDistance) {
								closestTile = tile;
								closestDistance = distToEdge;
								break;
							}
						}
					}
				}
			}
			return closestTile;
		}
				
		/// <summary>Check if the given tile is directly in front of the entity.
		/// </summary>
		public bool IsFacingSolidTile(Tile tile, int direction,
			float distance = 1.0f)
		{
			Rectangle2F entityBox = PositionedCollisionBox;
			entityBox.ExtendEdge(direction, distance);
			Vector2F directionVector = Directions.ToVector(direction);
			int alignAxis = 1 - Directions.ToAxis(direction);

			foreach (Rectangle2I box in tile.CollisionModel.Boxes) {
				Rectangle2F solidBox = box;
				solidBox.Point += tile.Position;

				if (entityBox.Intersects(solidBox) &&
					!IsSafeClippingInDirection(solidBox, (direction + 1) % 4) &&
					!IsSafeClippingInDirection(solidBox, (direction + 2) % 4) &&
					!IsSafeClippingInDirection(solidBox, (direction + 3) % 4) &&
					!CanDodgeCollision(solidBox, direction) && 
					(solidBox.Center - entityBox.Center).Dot(directionVector) > 0.0f)
				{
					return true;
				}
			}
			return false;
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

		
		//-----------------------------------------------------------------------------
		// Internal Collision Tests
		//-----------------------------------------------------------------------------
		
		// Returns true if the entity moving down the ledge.
		public bool IsMovingDownLedge(Tile ledgeTile) {
			return velocity.Dot(Directions.ToVector(ledgeTile.LedgeDirection)) > 0.0f;
		}

		// Returns true if the entity moving up the ledge.
		public bool IsMovingUpLedge(Tile ledgeTile) {
			return velocity.Dot(Directions.ToVector(ledgeTile.LedgeDirection)) < 0.0f;
		}

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
			// Only dodge when moving horizontally or vertically.
			if (GMath.Abs(velocity.X) > GameSettings.EPSILON &&
				GMath.Abs(velocity.Y) > GameSettings.EPSILON)
				return false;

			float		dodgeDist	= autoDodgeDistance;
			Rectangle2F	objBox		= PositionedCollisionBox;
			Vector2F	pos			= entity.Position;
			Vector2F	dirVect		= Directions.ToVector(direction);

			for (int side = 0; side < 2; side++) {
				int moveDir		= (direction + (side == 0 ? 1 : 3)) % 4;
				float distance	= Math.Abs(objBox.GetEdge((moveDir + 2) % 4) - block.GetEdge(moveDir));

				if (distance <= dodgeDist) {
					Vector2F checkPos = pos + (dirVect * 1.0f) +
						(Directions.ToVector(moveDir) * distance);
					Vector2F gotoPos = GMath.Round(pos) + Directions.ToVector(moveDir);

					if (!IsPlaceMeetingSolid(checkPos, collisionBox) &&
						!IsPlaceMeetingSolid(gotoPos, collisionBox))
					{
						return true;
					}
				}
			}
			return false;
		}
		
		public bool IsSafeClippingInDirection(Rectangle2F solidBox, int direction) {
			return (ClipCollisionInfo[direction].IsAllowedClipping &&
				RoomPhysics.AreEdgesAligned(solidBox,
				ClipCollisionInfo[direction].CollisionBox, direction));
		}

		private bool IsSafeClippingTileInDirection(Tile tile, int direction) { // TODO!!!!
			if (!ClipCollisionInfo[direction].IsAllowedClipping)
				return false;
			if (ClipCollisionInfo[direction].CollidedObject == tile)
				return true;

			Rectangle2F entityBox = PositionedCollisionBox;

			foreach (Rectangle2F box in tile.CollisionModel.Boxes) {
				Rectangle2F solidBox = Rectangle2F.Translate(box, tile.Position);
				
				if (RoomPhysics.AreEdgesAligned(solidBox, ClipCollisionInfo[direction].CollisionBox, direction))
				{
					return true;
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
						ClipCollisionInfo[i].Reset();
						ClipCollisionInfo[i].PenetrationDirection = i;
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
			set { previousVelocity = value; }
		}

		public float PreviousZVelocity {
			get { return previousZVelocity; }
			set { previousZVelocity = value; }
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
			get {
				if (flags.HasFlag(PhysicsFlags.OnGroundOverride))
					return false;
				if (entity.RoomControl.IsSideScrolling)
					return !collisionInfo[Directions.Down].IsColliding;
				return (entity.ZPosition > 0.0f || zVelocity > 0.0f || IsFlying);
			}
		}

		public bool IsOnGround {
			get { return !IsInAir; }
		}
		
		public int AutoDodgeDistance {
			get { return autoDodgeDistance; }
			set { autoDodgeDistance = value; }
		}
		
		public float AutoDodgeSpeed {
			get { return autoDodgeSpeed; }
			set { autoDodgeSpeed = value; }
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

		public TileCollisionCondition CustomTileIsSolidCondition {
			get { return customTileIsSolidCondition; }
			set { customTileIsSolidCondition = value; }
		}

		public TileCollisionCondition CustomTileIsNotSolidCondition {
			get { return customTileIsNotSolidCondition; }
			set { customTileIsNotSolidCondition = value; }
		}
		
		public CollisionInfo[] CollisionInfo {
			get { return collisionInfo; }
		}
		
		public CollisionInfo[] PreviousCollisionInfo {
			get { return previousCollisionInfo; }
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

		public Vector2F TopTilePointOffset {
			get { return topTilePointOffset; }
			set { topTilePointOffset = value; }
		}

		// Tile Flags.

		public bool IsInGrass {
			get { return IsOnGround && topTile != null && topTile.EnvironmentType == TileEnvironmentType.Grass; }
		}

		public bool IsInPuddle {
			get { return IsOnGround && topTile != null && topTile.EnvironmentType == TileEnvironmentType.Puddle; }
		}

		public bool IsInHole {
			get { return IsOnGround && topTile != null && topTile.IsHole; }
		}

		public bool IsInWater {
			get {
				return (IsOnGround || entity.RoomControl.IsSideScrolling) &&
					topTile != null && topTile.IsWater;
			}
		}

		public bool IsInOcean {
			get { return IsOnGround && topTile != null && topTile.EnvironmentType == TileEnvironmentType.Ocean; }
		}

		public bool IsInLava {
			get { return IsOnGround && topTile != null && topTile.IsLava; }
		}

		public bool IsOnIce {
			get { return IsOnGround && topTile != null && topTile.EnvironmentType == TileEnvironmentType.Ice; }
		}

		public bool IsOnSideScrollingIce {
			get {
				return IsOnGround && collisionInfo[Directions.Down].Tile != null &&
					collisionInfo[Directions.Down].Tile.EnvironmentType == TileEnvironmentType.Ice;
			}
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
		
		public bool IsFlying {
			get { return HasFlags(PhysicsFlags.Flying); }
			set { SetFlags(PhysicsFlags.Flying, value); }
		}
		
		public bool CheckRadialCollisions {
			get { return HasFlags(PhysicsFlags.CheckRadialCollisions); }
			set { SetFlags(PhysicsFlags.CheckRadialCollisions, value); }
		}
		
		public bool OnGroundOverride {
			get { return HasFlags(PhysicsFlags.OnGroundOverride); }
			set { SetFlags(PhysicsFlags.OnGroundOverride, value); }
		}
		

		public Vector2F ReboundVelocity {
			get { return reboundVelocity; }
			set { reboundVelocity = value; }
		}

		public int LedgeAltitude {
			get { return ledgeAltitude; }
			set { ledgeAltitude = value; }
		}

		public Point2I LedgeTileLocation {
			get { return ledgeTileLocation; }
			set { ledgeTileLocation = value; }
		}

		public bool HasLanded {
			get { return hasLanded; }
			set { hasLanded = value; }
		}
		
		public bool[] MovementCollisions { get; set; }
		public CollisionInfoNew[] ClipCollisionInfo { get; set; }
		public bool IsDeadlocked { get; set; }

		private Vector2F netVelocity;

		public Vector2F NetVelocity {
			get { return  netVelocity; }
			set { netVelocity = value; }
		}
		
		public float NetVelocityX {
			get { return netVelocity.X; }
			set { netVelocity.X = value; }
		}

		public float NetVelocityY {
			get { return netVelocity.Y; }
			set { netVelocity.Y = value; }
		}

		public Vector2F SurfacePosition {
			get { return surfacePosition; }
			set { surfacePosition = value; }
		}

		public Vector2F SurfaceVelocity {
			get { return surfaceVelocity; }
			set { surfaceVelocity = value; }
		}
	}
}
