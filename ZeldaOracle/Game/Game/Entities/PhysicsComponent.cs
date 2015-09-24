using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Collision;
using ZeldaOracle.Game.Worlds;
using ZeldaOracle.Game.Tiles;

namespace ZeldaOracle.Game.Entities {
	
	[Flags]
	public enum PhysicsFlags {
		None					= 0,
		Solid					= 0x1,		// The entity is solid (other entities can collide with this one).
		HasGravity				= 0x2,		// The entity is affected by gravity.
		CollideWorld			= 0x4,		// Collide with solid tiles.
		CollideRoomEdge			= 0x8,		// Colide with the edges of the room.
		ReboundSolid			= 0x10,		// Rebound off of solids.
		ReboundRoomEdge			= 0x20,		// Rebound off of room edges.
		Bounces					= 0x40,		// The entity bounces when it hits the ground.
		DestroyedOutsideRoom	= 0x80,		// The entity is destroyed when it is outside of the room.
		DestroyedInHoles		= 0x100,	// The entity gets destroyed in holes.
		LedgePassable			= 0x200,	// The entity can pass over ledges.
		HalfSolidPassable		= 0x400,	// The entity can pass over half-solids (railings).
		AutoDodge				= 0x800,	// Will move out of the way when colliding with the edges of objects.
	}

	public class PhysicsComponent {

		private Entity			entity;				// The entity this component belongs to.
		private bool			isEnabled;			// Are physics enabled for the entity?
		private PhysicsFlags	flags;
		private Vector2F		velocity;			// XY-Velocity in pixels per frame.
		private float			zVelocity;			// Z-Velocity in pixels per frame.
		private float			gravity;			// Gravity in pixels per frame^2
		private float			maxFallSpeed;
		private Rectangle2F		collisionBox;		// The "hard" collision box, used to collide with solid entities/tiles.
		private Rectangle2F		softCollisionBox;	// The "soft" collision box, used to collide with items, monsters, room edges, etc.
		private int				autoDodgeDistance; // The maximum distance allowed to dodge collisions.

		private bool			isColliding;
		private CollisionInfo[] collisionInfo;
		private bool			hasLanded;
		private TileFlags		topTileFlags;		// The flags for the top-most tile the entity is located over.
		private TileFlags		allTileFlags;		// The group of flags for all the tiles the entity is located over.


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
			this.gravity			= GameSettings.DEFAULT_GRAVITY;
			this.maxFallSpeed		= GameSettings.DEFAULT_MAX_FALL_SPEED;
			this.collisionBox		= new Rectangle2F(-4, -10, 8, 9);		// TEMPORARY: this is the player collision box.
			this.softCollisionBox	= new Rectangle2F(-6, -14, 12, 14);	// TEMPORARY: this is the player collision box.
			this.topTileFlags		= TileFlags.None;
			this.allTileFlags		= TileFlags.None;
			this.isColliding		= false;
			this.autoDodgeDistance	= 6;

			this.hasLanded			= false;

			this.collisionInfo = new CollisionInfo[Directions.Count];
			for (int i = 0; i < Directions.Count; i++)
				collisionInfo[i].Clear();

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
			// Remove collision state flags.
			hasLanded = false;
			isColliding = false;
			for (int i = 0; i < Directions.Count; i++)
				collisionInfo[i].Clear();

			// Handle Z position.
			UpdateZVelocity();
			if (entity.IsDestroyed)
				return;

			// Check world collisions.
			if (HasFlags(PhysicsFlags.CollideWorld))
				CheckCollisions();
	
			// Apply velocity.
			entity.Position += velocity;

			CheckGroundTiles();
			
			// Collide with room edges.
			if (HasFlags(PhysicsFlags.CollideRoomEdge))
				CheckRoomEdgeCollisions(softCollisionBox); // TODO: this should default to hard collision-box.

			// Check if outside room.
			if (HasFlags(PhysicsFlags.DestroyedOutsideRoom) &&
				!entity.RoomControl.RoomBounds.Contains(entity.Origin))
			{
				entity.Destroy();
				return;
			}
			
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
			
			if (hasLanded) {
				entity.OnLand();
			}
		}

		// Check the flags of the tiles the entity is located on top of (if it is on the ground).
		private void CheckGroundTiles() {
			topTileFlags = TileFlags.None;
			allTileFlags = TileFlags.None;

			Point2I location = entity.RoomControl.GetTileLocation(entity.Origin);
			if (entity.RoomControl.IsTileInBounds(location)) {
				for (int i = entity.RoomControl.Room.LayerCount - 1; i >= 0; i--) {
					Tile tile = entity.RoomControl.GetTile(location, i);

					if (tile != null) {
						topTileFlags |= tile.Flags;
						allTileFlags |= tile.Flags;
						break;
					}
				}
			}
		}

		// Update the z-velocity, position, and gravity of the entity.
		private void UpdateZVelocity() {
			if (entity.ZPosition > 0.0f || zVelocity != 0.0f) {
				entity.ZPosition += zVelocity;

				// Apply gravity.
				if (HasFlags(PhysicsFlags.HasGravity)) {
					zVelocity -= gravity;
					if (zVelocity < -maxFallSpeed && maxFallSpeed >= 0)
						zVelocity = -maxFallSpeed;
				}

				// Land on the ground.
				if (entity.ZPosition <= 0.0f) {
					hasLanded = true;
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
				entity.ZPosition = 0.1f;
				zVelocity = -zVelocity * 0.5f;
			}
			else {
				zVelocity = 0;
				velocity = Vector2F.Zero;
			}
			//Sounds.play(soundBounce);

			if (velocity.Length > 0.25)
				velocity *= 0.5f;
			else
				velocity = Vector2F.Zero;
		}

		
		//-----------------------------------------------------------------------------
		// Collision polls
		//-----------------------------------------------------------------------------

		// Is it possible for the entity to collide with the given tile?
		public bool CanCollideWithTile(Tile tile) {
			if (tile == null || tile.CollisionModel == null || !tile.Flags.HasFlag(TileFlags.Solid))
				return false;
			if (tile.Flags.HasFlag(TileFlags.HalfSolid) && flags.HasFlag(PhysicsFlags.HalfSolidPassable))
				return false;
			return true;
		}

		// Is it possible for the entity to collide with the given tile?
		public bool CanCollideWithEntity(Entity other) {
			if (other == null || !other.Physics.IsEnabled)
				return false;
			return true;
		}

		// Returns true if the entity is colliding in the given direction.
		public bool IsCollidingDirection(int direction) {
			return collisionInfo[direction].IsColliding;
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

			for (int x = area.Left; x < area.Right; ++x) {
				for (int y = area.Top; y < area.Bottom; ++y) {
					for (int i = 0; i < room.LayerCount; ++i) {
						Tile t = entity.RoomControl.GetTile(x, y, i);
						if (CanCollideWithTile(t)) {
							if (CollisionModel.Intersecting(t.CollisionModel, t.Position, collisionBox, position))
								return true;
						}
					}
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
			Vector2F checkPos = position + Directions.ToPoint(direction);
			Point2I location = entity.RoomControl.GetTileLocation(entity.Center) + 
				Directions.ToPoint(direction);
			if (!entity.RoomControl.IsTileInBounds(location))
				return null;

			for (int i = 0; i < entity.RoomControl.Room.LayerCount; i++) {
				Tile tile = entity.RoomControl.GetTile(location, i);

				if (CanCollideWithTile(tile) && CollisionModel.Intersecting(
					tile.CollisionModel, tile.Position, collisionBox, checkPos) &&
					!CanDodgeCollision(tile, direction))
				{
					return tile;
				}
			}
			return null;
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


		//-----------------------------------------------------------------------------
		// Collisions
		//-----------------------------------------------------------------------------

		// This should be called after applying velocity.
		public void CheckRoomEdgeCollisions(Rectangle2F collisionBox) {
			Rectangle2F roomBounds = entity.RoomControl.RoomBounds;
			Rectangle2F myBox = Rectangle2F.Translate(collisionBox, entity.Position);

			if (myBox.Left < roomBounds.Left) {
				isColliding = true;
				entity.X = roomBounds.Left - collisionBox.Left;
				velocity.X = 0;
				collisionInfo[Directions.Left].SetRoomEdgeCollision(Directions.Left);
			}
			else if (myBox.Right > roomBounds.Right) {
				isColliding = true;
				entity.X = roomBounds.Right - collisionBox.Right;
				velocity.X = 0;
				collisionInfo[Directions.Right].SetRoomEdgeCollision(Directions.Right);
			}
			if (myBox.Top < roomBounds.Top) {
				isColliding = true;
				entity.Y = roomBounds.Top - collisionBox.Top;
				velocity.Y = 0;
				collisionInfo[Directions.Up].SetRoomEdgeCollision(Directions.Up);
			}
			else if (myBox.Bottom > roomBounds.Bottom) {
				isColliding = true;
				entity.Y = roomBounds.Bottom - collisionBox.Bottom;
				velocity.Y = 0;
				collisionInfo[Directions.Down].SetRoomEdgeCollision(Directions.Down);
			}
		}

		// Check collisions with tiles.
		public void CheckCollisions() {
			// Find the rectangular area of nearby tiles to collide with.
			Rectangle2F myBox = PositionedCollisionBox;
			Rectangle2F myBox2 = Rectangle2F.Translate(myBox, velocity);
			myBox = Rectangle2F.Union(myBox, myBox2);
			myBox.Inflate(2, 2);

			// Collide with nearby solid tiles HORIZONTALLY and then VERTICALLY.
			Rectangle2I area = entity.RoomControl.GetTileAreaFromRect(myBox, 1);
			Room room = entity.RoomControl.Room;
			for (int axis = 0; axis < 2; ++axis) {
				for (int x = area.Left; x < area.Right; ++x) {
					for (int y = area.Top; y < area.Bottom; ++y) {
						for (int i = 0; i < room.LayerCount; ++i) {
							Tile t = entity.RoomControl.GetTile(x, y, i);
							if (CanCollideWithTile(t))
								ResolveCollision(axis, t, t.Position, t.CollisionModel);
						}
					}
				}
			}
		}
		
		private bool ResolveCollision(int axis, Tile tile, Vector2F modelPos, CollisionModel model) {
			bool collide = false;
			for (int i = 0; i < model.Boxes.Count; ++i) {
				Rectangle2F box = Rectangle2F.Translate((Rectangle2F) model.Boxes[i], modelPos);
				if (ResolveCollision(axis, tile, box))
					collide = true;
			}
			return collide;
		}

		private bool ResolveCollision(int axis, Tile tile, Rectangle2F block) {
			if (axis == 0) { // X-Axis
				Rectangle2F myBox = Rectangle2F.Translate(collisionBox, entity.X + velocity.X, entity.Y);
				if (myBox.Intersects(block)) {
					isColliding	= true;
					velocity.X	= 0.0f;

					if (myBox.Center.X < block.Center.X) {
						entity.X = block.Left - collisionBox.Right;
						if (!HasFlags(PhysicsFlags.AutoDodge) || !PerformCollisionDodge(block, Directions.Right))
							collisionInfo[Directions.Right].SetTileCollision(tile, Directions.Right);
					}
					else {
						entity.X = block.Right - collisionBox.Left;
						if (!HasFlags(PhysicsFlags.AutoDodge) || !PerformCollisionDodge(block, Directions.Left))
							collisionInfo[Directions.Left].SetTileCollision(tile, Directions.Left);
					}
					return true;
				}
			}
			else if (axis == 1) { // Y-Axis
				Rectangle2F myBox = Rectangle2F.Translate(collisionBox, entity.Position + velocity);
				if (myBox.Intersects(block)) {
					isColliding	= true;
					velocity.Y	= 0.0f;

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
					return true;
				}
			}
			return false;
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
			set { isEnabled = value; }
		}

		public Vector2F Velocity {
			get { return velocity; }
			set { velocity = value; }
		}

		public float ZVelocity {
			get { return zVelocity; }
			set { zVelocity = value; }
		}
		
		public float Gravity {
			get { return gravity; }
			set { gravity = value; }
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


		// Collision info.

		public Rectangle2F PositionedCollisionBox {
			get { return Rectangle2F.Translate(collisionBox, entity.Position); }
		}
		
		public Rectangle2F PositionedSoftCollisionBox {
			get { return Rectangle2F.Translate(softCollisionBox, entity.Position); }
		}

		public Rectangle2F CollisionBox {
			get { return collisionBox; }
			set { collisionBox = value; }
		}

		public Rectangle2F SoftCollisionBox {
			get { return softCollisionBox; }
			set { softCollisionBox = value; }
		}
		
		public CollisionInfo[] CollisionInfo {
			get { return collisionInfo; }
		}
		
		public bool IsColliding {
			get { return isColliding; }
		}


		// Tile flags.
		
		public TileFlags GroundTileFlags {
			get { return topTileFlags; }
		}
		
		public TileFlags AllTileFlags {
			get { return allTileFlags; }
		}

		public bool IsInGrass {
			get { return IsOnGround && topTileFlags.HasFlag(TileFlags.Grass); }
		}

		public bool IsInPuddle {
			get { return IsOnGround && topTileFlags.HasFlag(TileFlags.Puddle); }
		}

		public bool IsInHole {
			get { return IsOnGround && topTileFlags.HasFlag(TileFlags.Hole); }
		}

		public bool IsInWater {
			get { return IsOnGround && topTileFlags.HasFlag(TileFlags.Water); }
		}
		
		public bool IsInLava {
			get { return IsOnGround && topTileFlags.HasFlag(TileFlags.Lava); }
		}

		public bool IsOnIce {
			get { return IsOnGround && topTileFlags.HasFlag(TileFlags.Ice); }
		}

		public bool IsOnStairs {
			get { return IsOnGround && topTileFlags.HasFlag(TileFlags.Stairs); }
		}

		public bool IsOnLadder {
			get { return IsOnGround && topTileFlags.HasFlag(TileFlags.Ladder); }
		}

		public bool IsOverHalfSolid {
			get { return topTileFlags.HasFlag(TileFlags.HalfSolid); }
		}

		public bool IsOverLedge {
			get { return topTileFlags.HasFlag(TileFlags.Ledge); }
		}


		// Flags:

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
		
		public bool DestroyedOutsideRoom {
			get { return HasFlags(PhysicsFlags.DestroyedOutsideRoom); }
			set { SetFlags(PhysicsFlags.DestroyedOutsideRoom, value); }
		}
		
		public bool DestroyedInHoles {
			get { return HasFlags(PhysicsFlags.DestroyedInHoles); }
			set { SetFlags(PhysicsFlags.DestroyedInHoles, value); }
		}
		
		public bool LedgePassable {
			get { return HasFlags(PhysicsFlags.LedgePassable); }
			set { SetFlags(PhysicsFlags.LedgePassable, value); }
		}
		
		public bool HalfSolidPassable {
			get { return HasFlags(PhysicsFlags.HalfSolidPassable); }
			set { SetFlags(PhysicsFlags.HalfSolidPassable, value); }
		}
		
		public bool AutoDodges {
			get { return HasFlags(PhysicsFlags.AutoDodge); }
			set { SetFlags(PhysicsFlags.AutoDodge, value); }
		}
	}
}
