using System;
using System.Collections.Generic;
using System.Linq;
using ZeldaOracle.Common.Geometry;
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
		DisableSurfaceContact	= 0x40000,	// Do not check for surface interactions such as water, holes, conveyors

		CheckRadialCollisions	= 0x80000,
		ResolveClippingCollisions	= 0x200000,

		// This was added for player ladder climbing in a side-scrolling room,
		// because when climbing a ladder, it is like the player is on the ground
		// even though he is not.
		OnGroundOverride		= 0x100000,	// If this is set, IsOnGround will always be true.
	}

	public enum LedgePassState {
		None,
		PassingDown,
		PassingUp,
	}

	public class PhysicsComponent : EntityComponent {

		public delegate bool TileCollisionCondition(Tile tile);

		// General
		private PhysicsFlags			flags;
		private float					gravity;			// Gravity in pixels per frame^2
		private float					maxFallSpeed;
		private Vector2F				velocity;			// XY-Velocity in pixels per frame.
		private float					zVelocity;			// Z-Velocity in pixels per frame.
		private Vector2F				surfacePosition;	// Used for draw position rounding to prevent jittering.
		private Vector2F				surfaceVelocity;	// Used for draw position rounding to prevent jittering.
		
		// Collision settings
		private Rectangle2F				collisionBox;		// The "hard" collision box, used to collide with solid entities/tiles.
		private TileCollisionCondition	customTileIsSolidCondition;
		private TileCollisionCondition	customTileIsNotSolidCondition;
		private Rectangle2F?			roomEdgeCollisionBox;
		private int						autoDodgeDistance;	// The maximum distance allowed to dodge collisions.
		private float					autoDodgeSpeed;		// The speed to move at when dodging collisions.
		private int						edgeClipAmount;
		private int						crushMaxGapSize;
		private Vector2F				topTilePointOffset;

		// Internal physics state
		private Vector2F			previousVelocity;	// XY-Velocity before physics update is called.
		private float				previousZVelocity;	// Z-Velocity before physics update is called.
		private Vector2F			reboundVelocity;
		private bool				isColliding;
		private bool				hasLanded;
		private Tile				topTile;			// The top-most tile the entity is located over.
		private int					ledgeAltitude;		// How many ledges the entity has passed over.
		private Point2I				ledgeTileLocation;	// The tile location of the ledge we are currently passing over, or (-1, -1) if not passing over ledge.
		private LedgePassState		ledgePassState;
		private Tile				ledgePassTile;
		private List<Collision>		potentialCollisions;
		private List<Collision>		previousPotentialCollisions;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		// By default, physics are disabled.
		public PhysicsComponent(Entity entity) :
			base(entity)
		{
			flags				= PhysicsFlags.None;
			velocity			= Vector2F.Zero;
			zVelocity			= 0.0f;
			previousVelocity	= Vector2F.Zero;
			previousZVelocity	= 0.0f;
			gravity				= GameSettings.DEFAULT_GRAVITY;
			maxFallSpeed		= GameSettings.DEFAULT_MAX_FALL_SPEED;
			collisionBox		= new Rectangle2F(-1, -1, 2, 2);
			topTile				= null;
			topTilePointOffset	= Vector2F.Zero;
			isColliding			= false;
			autoDodgeDistance	= 6;
			autoDodgeSpeed		= 1.0f;
			hasLanded			= false;
			reboundVelocity		= Vector2F.Zero;
			ledgeAltitude		= 0;
			ledgeTileLocation	= new Point2I(-1, -1);
			ledgePassState		= LedgePassState.None;
			ledgePassTile		= null;
			roomEdgeCollisionBox		= null;
			customTileIsSolidCondition	= null;
			potentialCollisions			= new List<Collision>();
			previousPotentialCollisions	= new List<Collision>();

			crushMaxGapSize	= 0;
			edgeClipAmount	= 1;
		}
		
		//-----------------------------------------------------------------------------
		// Enable / Disable
		//-----------------------------------------------------------------------------

		/// <summary>Enable the physics component with the given physics flags.
		/// </summary>
		public void Enable(PhysicsFlags flags) {
			Enable();
			this.flags |= flags;
		}

		
		//-----------------------------------------------------------------------------
		// Flags
		//-----------------------------------------------------------------------------
		
		/// <summary>Returns true if the given physics flags are set.</summary>
		public bool HasFlags(PhysicsFlags flags) {
			return ((this.flags & flags) == flags);
		}

		/// <summary>Set or clear the given physics flags.</summary>
		public void SetFlags(PhysicsFlags flagsToSet, bool enabled) {
			if (enabled)
				flags |= flagsToSet;
			else
				flags &= ~flagsToSet;
		}

		
		//-----------------------------------------------------------------------------
		// Tile Queries
		//-----------------------------------------------------------------------------
		
		/// <summary>Iterate the tiles the would be touching this entity's collision
		/// box if it were placed at the given position.</summary>
		public IEnumerable<Tile> GetTilesMeeting(Vector2F position) {
			return GetTilesMeeting(position, collisionBox);
		}
		
		/// <summary>Iterate the tiles the would be touching this entity's collision
		/// box if it were placed at the given position.</summary>
		public IEnumerable<Tile> GetTilesMeeting(
			Vector2F position, Rectangle2F collisionBox)
		{
			// Create rectangular area of nearby tiles to collide with
			Rectangle2F myBox = collisionBox;
			myBox.Point += position;
			myBox.Inflate(2, 2);
			Rectangle2I area = entity.RoomControl.GetTileAreaFromRect(myBox, 1);

			// Check intersection with the tiles in that area
			foreach (Tile t in entity.RoomControl.GetTopTilesInArea(area)) {
				if (t.CollisionModel != null &&
					CollisionModel.Intersecting(t.CollisionModel, t.Position,
						collisionBox, position))
				{
					yield return t;
				}
			}
		}
		
		/// <summary>Iterate solid tiles which this entity would be colliding with if
		/// it were placed at the given position and had the given collision box.
		/// </summary>
		public IEnumerable<Tile> GetSolidTilesMeeting(
			Vector2F position, Rectangle2F collisonBox)
		{
			foreach (Tile tile in GetTilesMeeting(position, collisonBox)) {
				if (CanCollideWithTile(tile))
					yield return tile;
			}
		}


		//-----------------------------------------------------------------------------
		// Collision Queries
		//-----------------------------------------------------------------------------
		
		/// <summary>Return true if the entity would collide with a solid object using
		/// the given collision box if it were placed at the given position.</summary>
		public bool IsPlaceMeetingSolid(Vector2F position) {
			return IsPlaceMeetingSolid(position, collisionBox);
		}
		
		/// <summary>Return true if the entity would collide with a solid object using
		/// the given collision box if it were placed at the given position.</summary>
		public bool IsPlaceMeetingSolid(Vector2F position, Rectangle2F collisionBox) {
			Room room = entity.RoomControl.Room;
			
			// Find the rectangular area of nearby tiles to collide with.
			Rectangle2F myBox = collisionBox;
			myBox.Point += position;
			myBox.Inflate(2, 2);
	
			int x1 = (int) (myBox.Left   / GameSettings.TILE_SIZE);
			int y1 = (int) (myBox.Top    / GameSettings.TILE_SIZE);
			int x2 = (int) (myBox.Right  / GameSettings.TILE_SIZE) + 1;
			int y2 = (int) (myBox.Bottom / GameSettings.TILE_SIZE) + 1;

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

		/// <summary>Return true if the entity would collide with a tile if it were at
		/// the given position.</summary>
		public bool IsPlaceMeetingSolidTile(Vector2F position, Tile tile) {
			if (CanCollideWithTile(tile)) {
				return CollisionModel.Intersecting(
					tile.CollisionModel, tile.Position, collisionBox, position);
			}
			return false;
		}

		/// <summary>Return true if the entity would collide with a tile if it were at
		/// the given position.</summary>
		public bool IsPlaceMeetingSolidEntity(Vector2F position, Entity entity,
			float maxZDistance = 10)
		{
			if (CanCollideWithEntity(entity) &
				Rectangle2F.Translate(collisionBox, position)
					.Intersects(entity.Physics.PositionedCollisionBox))
			{
				return true;
			}
			return false;
		}


		//-----------------------------------------------------------------------------
		// Collision Queries
		//-----------------------------------------------------------------------------

		/// <summary>Returns true if the entity is currently colliding in the given
		/// direction.</summary>
		public bool IsCollidingInDirection(Direction direction) {
			return  Collisions.Any(c => c.Direction == direction);
		}

		/// <summary>Iterate the collisions which are penetrating on the given axis.
		/// </summary>
		public IEnumerable<Collision> GetCollisionsOnAxis(int axis) {
			return Collisions.Where(c => c.Axis == axis);
		}
		
		/// <summary>Iterate the collisions which are penetrating in the given
		/// direction.</summary>
		public IEnumerable<Collision> GetCollisionsInDirection(Direction direction) {
			return Collisions.Where(c => c.Direction == direction);
		}
		
		/// <summary>Iterate the collisions from the previous update which were
		/// penetrating in the given direction.</summary>
		public IEnumerable<Collision> GetPreviousCollisionsInDirection(
			Direction direction)
		{
			return PreviousCollisions.Where(c => c.Direction == direction);
		}

		/// <summary></summary>
		public Collision GetCenteredCollisionInDirection(Direction direction) {
			Collision best = null;
			int lateralAxis = direction.PerpendicularAxis;
			
			foreach (Collision collision in Collisions) {
				if (collision.Direction == direction) {
					if (entity.Center[lateralAxis] <= collision.SolidBox.Max[lateralAxis] &&
						entity.Center[lateralAxis] >= collision.SolidBox.Min[lateralAxis])
					{
						if (best == null)
							best = collision;
					}
				}
			}
			return best;
		}

		/// <summary></summary>
		public Collision GetCenteredPotentialCollisionInDirection(
			Direction direction, float minPenetration = 0.0f)
		{
			Collision best = null;
			int lateralAxis = direction.PerpendicularAxis;
			
			foreach (Collision collision in PotentialCollisions) {
				if (collision.Direction == direction) {
					if (collision.Penetration >= minPenetration - GameSettings.EPSILON &&
						entity.Center[lateralAxis] <= collision.SolidBox.Max[lateralAxis] &&
						entity.Center[lateralAxis] >= collision.SolidBox.Min[lateralAxis])
					{
						if (best == null)
							best = collision;
					}
				}
			}
			return best;
		}

		/// <summary>Return the center-most collision of all collisions which are
		/// penetrating in the given direction.</summary>
		public Collision GetCollisionInDirection(Direction direction) {
			return GetCollisionInDirection(direction, Collisions);
		}
		
		/// <summary>Return the center-most collision of all collisions from the
		/// previous update which are penetrating in the given direction.</summary>
		public Collision GetPreviousCollisionInDirection(Direction direction) {
			return GetCollisionInDirection(direction, PreviousCollisions);
		}
		
		/// <summary>Return the center-most collision of all collisions which are
		/// penetrating in the given direction.</summary>
		private Collision GetCollisionInDirection(Direction direction,
			IEnumerable<Collision> collisions)
		{
			Collision best = null;
			bool bestIsCentered = false;
			int lateralAxis = direction.PerpendicularAxis;

			foreach (Collision collision in collisions) {
				if (collision.Direction == direction) {
					bool isCentered =
						(entity.Center[lateralAxis] <= collision.SolidBox.Max[lateralAxis] &&
						entity.Center[lateralAxis] >= collision.SolidBox.Min[lateralAxis]);

					if (best == null || (isCentered && !bestIsCentered)) {
						best = collision;
						bestIsCentered = isCentered;
					}
				}
			}
			return best;
		}

		/// <summary>Collide with the inside edges of a rectangle.
		/// NOTE: At the moment, this is only used when player is doomed to fall in a
		/// hole.</summary>
		public void PerformInsideEdgeCollisions(
			Rectangle2F collisionBox, Rectangle2F rect)
		{
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

		/// <summary>Returns true if the entity is able to collide with the given tile.
		/// </summary>
		public bool CanCollideWithTile(Tile checkTile) {
			if (checkTile.CollisionModel == null)
				return false;
			if (customTileIsSolidCondition != null && 
				customTileIsSolidCondition(checkTile))
				return true;
			if (!checkTile.IsSolid ||
				(checkTile.IsHalfSolid && PassOverHalfSolids))
				return false;
			if (checkTile.IsAnyLedge && PassOverLedges &&
				((ledgePassState != LedgePassState.None &&
					ledgePassTile == checkTile) ||
				IsMovingDownLedge(checkTile) ||
				IsMovingUpLedge(checkTile) && ledgeAltitude > 0))
			{
				return false;
			}
			if (customTileIsNotSolidCondition != null &&
				!customTileIsNotSolidCondition(checkTile))
				return false;
			return true;
		}
		
		/// <summary>Returns true if the entity is able to collide with the given
		/// entity.</summary>
		public bool CanCollideWithEntity(Entity other) {
			return (other != entity && other.Physics.IsEnabled &&
				other.Physics.IsSolid);
		}
		
		/// <summary>Returns true if the entity is moving down the ledge.</summary>
		public bool IsMovingDownLedge(Tile ledgeTile) {
			return velocity.Dot(ledgeTile.LedgeDirection.ToVector()) > 0.0f;
		}

		/// <summary>Returns true if the entity is moving up the ledge.</summary>
		public bool IsMovingUpLedge(Tile ledgeTile) {
			return velocity.Dot(ledgeTile.LedgeDirection.ToVector()) < 0.0f;
		}

		public bool CanDodgeCollision(Tile tile, Direction direction) {
			if (!CanCollideWithTile(tile))
				return false;
			for (int i = 0; i < tile.CollisionModel.Boxes.Count; i++) {
				if (CanDodgeCollision(Rectangle2F.Translate(
					tile.CollisionModel.Boxes[i], tile.Position), direction))
				{
					return true;
				}
			}
			return false;
		}

		public bool CanDodgeCollision(Rectangle2F block, Direction direction) {
			// Only dodge when moving horizontally or vertically.
			if (GMath.Abs(velocity.X) > GameSettings.EPSILON &&
				GMath.Abs(velocity.Y) > GameSettings.EPSILON)
				return false;

			float dodgeDist		= autoDodgeDistance;
			Rectangle2F	objBox	= PositionedCollisionBox;
			Vector2F pos		= entity.Position;
			Vector2F dirVect	= direction.ToVector();

			for (int side = 0; side < 2; side++) {
				Direction moveDir = (direction + (side == 0 ? 1 : 3)) % 4;
				float distance = GMath.Abs(
					objBox.GetEdge(moveDir.Reverse()) - block.GetEdge(moveDir));

				if (distance <= dodgeDist) {
					Vector2F checkPos = pos + (dirVect * 1.0f) +
						moveDir.ToVector(distance);
					Vector2F gotoPos = GMath.Round(pos) + moveDir.ToVector(1.0f);

					if (!IsPlaceMeetingSolid(checkPos, collisionBox) &&
						!IsPlaceMeetingSolid(gotoPos, collisionBox))
					{
						return true;
					}
				}
			}
			return false;
		}
		
		public bool IsSafeClippingInDirection(Rectangle2F solidBox, Direction direction) {
			return false; // TODO: IsSafeClippingInDirection
			//return (ClipCollisionInfo[direction].IsAllowedClipping &&
				//RoomPhysics.AreEdgesAligned(solidBox,
				//ClipCollisionInfo[direction].CollisionBox, direction));
		}

		/*private bool IsSafeClippingTileInDirection(Tile tile, int direction) { // TODO!!!!
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
		}*/


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

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
					return !IsCollidingInDirection(Direction.Down);
				return (entity.ZPosition > 0.0f || zVelocity > 0.0f);
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
		
		public Rectangle2F RoomEdgeCollisionBox {
			get { return (roomEdgeCollisionBox ?? collisionBox); }
			set { roomEdgeCollisionBox = value; }
		}
		
		public int EdgeClipAmount {
			get { return edgeClipAmount; }
			set { edgeClipAmount = value; }
		}

		public int CrushMaxGapSize {
			get { return crushMaxGapSize; }
			set { crushMaxGapSize = value; }
		}

		
		//-----------------------------------------------------------------------------
		// Collision Properties
		//-----------------------------------------------------------------------------

		/// <summary>The "Hard" collision box which is used to collide with solid
		/// objects.</summary>
		public Rectangle2F CollisionBox {
			get { return collisionBox; }
			set { collisionBox = value; }
		}
		
		/// <summary>The "Hard" collision box translated to the entity's current
		/// position.</summary>
		public Rectangle2F PositionedCollisionBox {
			get { return Rectangle2F.Translate(collisionBox, entity.Position); }
		}

		public TileCollisionCondition CustomTileIsSolidCondition {
			get { return customTileIsSolidCondition; }
			set { customTileIsSolidCondition = value; }
		}

		public TileCollisionCondition CustomTileIsNotSolidCondition {
			get { return customTileIsNotSolidCondition; }
			set { customTileIsNotSolidCondition = value; }
		}
		
		/// <summary>True if the entity is currently colliding.</summary>
		public bool IsColliding {
			get { return isColliding; }
			set { isColliding = value; }
		}

		/// <summary>The surface tile underneath the entity.</summary>
		public Tile TopTile {
			get { return topTile; }
			set { topTile = value; }
		}

		public Vector2F TopTilePointOffset {
			get { return topTilePointOffset; }
			set { topTilePointOffset = value; }
		}


		//-----------------------------------------------------------------------------
		// Surface Properties
		//-----------------------------------------------------------------------------

		public bool IsInWater {
			get {
				return ((IsOnGround || entity.RoomControl.IsSideScrolling) &&
					!DisableSurfaceContact) &&
					topTile != null && topTile.IsWater;
			}
		}

		public bool IsOnSideScrollingIce {
			get {
				return (StandingCollision != null &&
					StandingCollision.IsTile && StandingCollision.Tile.IsIce);
			}
		}

		public bool IsInGrass {
			get {
				return IsOnGround && topTile != null &&
					topTile.EnvironmentType == TileEnvironmentType.Grass;
			}
		}

		public bool IsInPuddle {
			get {
				return IsOnGround && topTile != null &&
					topTile.EnvironmentType == TileEnvironmentType.Puddle;
			}
		}

		public bool IsInHole {
			get {
				return IsOnGround && !DisableSurfaceContact && topTile != null &&
					topTile.IsHole;
			}
		}

		public bool IsInOcean {
			get {
				return IsOnGround && !DisableSurfaceContact && topTile != null &&
					topTile.EnvironmentType == TileEnvironmentType.Ocean;
			}
		}

		public bool IsInLava {
			get {
				return ((IsOnGround || entity.RoomControl.IsSideScrolling)
					&& !DisableSurfaceContact) &&
					topTile != null && topTile.IsLava;
			}
		}

		public bool IsOnIce {
			get {
				return IsOnGround && !DisableSurfaceContact && topTile != null &&
					topTile.EnvironmentType == TileEnvironmentType.Ice;
			}
		}

		public bool IsOnStairs {
			get {
				return IsOnGround && !DisableSurfaceContact &&  topTile != null &&
					topTile.EnvironmentType == TileEnvironmentType.Stairs;
			}
		}

		public bool IsOnLadder {
			get {
				return IsOnGround && !DisableSurfaceContact && topTile != null &&
					topTile.EnvironmentType == TileEnvironmentType.Ladder;
			}
		}

		public bool IsOverHalfSolid {
			get { return (topTile != null && topTile.IsHalfSolid); }
		}

		public bool IsOverLedge {
			get { return (topTile != null && topTile.IsLedge); }
		}


		//-----------------------------------------------------------------------------
		// Physics Flags Properties
		//-----------------------------------------------------------------------------

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
		
		public bool DisableSurfaceContact {
			get { return HasFlags(PhysicsFlags.DisableSurfaceContact); }
			set { SetFlags(PhysicsFlags.DisableSurfaceContact, value); }
		}
		
		public bool CheckRadialCollisions {
			get { return HasFlags(PhysicsFlags.CheckRadialCollisions); }
			set { SetFlags(PhysicsFlags.CheckRadialCollisions, value); }
		}
		
		public bool OnGroundOverride {
			get { return HasFlags(PhysicsFlags.OnGroundOverride); }
			set { SetFlags(PhysicsFlags.OnGroundOverride, value); }
		}
		
		public bool ResolveClippingCollisions {
			get { return HasFlags(PhysicsFlags.ResolveClippingCollisions); }
			set { SetFlags(PhysicsFlags.ResolveClippingCollisions, value); }
		}
		

		//-----------------------------------------------------------------------------
		// Misc Properties
		//-----------------------------------------------------------------------------

		public Vector2F ReboundVelocity {
			get { return reboundVelocity; }
			set { reboundVelocity = value; }
		}

		public int LedgeAltitude {
			get { return ledgeAltitude; }
			set { ledgeAltitude = value; }
		}

		public LedgePassState LedgePassState {
			get { return ledgePassState; }
			set { ledgePassState = value; }
		}

		public Tile LedgePassTile {
			get { return ledgePassTile; }
			set { ledgePassTile = value; }
		}

		public Point2I LedgeTileLocation {
			get { return ledgeTileLocation; }
			set { ledgeTileLocation = value; }
		}

		public bool HasLanded {
			get { return hasLanded; }
			set { hasLanded = value; }
		}
		
		public List<Collision> PotentialCollisions {
			get { return potentialCollisions; }
			set { potentialCollisions = value; }
		}
		
		public List<Collision> PreviousPotentialCollisions {
			get { return previousPotentialCollisions; }
			set { previousPotentialCollisions = value; }
		}
		
		public IEnumerable<Collision> PreviousCollisions {
			get { return previousPotentialCollisions.Where(
					c => c.IsColliding || c.IsResolved); }
		}
				
		public IEnumerable<Collision> Collisions {
			get {
				return potentialCollisions.Where(
					c => c.IsColliding || c.IsResolved);
			}
		}

		public Collision PreviousStandingCollision { get; set; } = null;

		public Collision StandingCollision { get; set; } = null;
		
		/// <summary>Only used by RoomPhysics to detect landing in side scrolling mode.
		/// </summary>
		public bool IsOnGroundPrevious { get; set; }  = true;

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
