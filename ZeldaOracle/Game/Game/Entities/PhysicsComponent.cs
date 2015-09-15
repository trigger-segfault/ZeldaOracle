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

		[Flags]
		private enum StateFlags {
			None				= 0,
			Colliding			= 0x1,		// The entity is colliding.
			CollidingRight		= 0x2,		// Colliding with something to the right.
			CollidingUp			= 0x4,		// Colliding with something up.
			CollidingLeft		= 0x8,		// Colliding with something to the left.
			CollidingDown		= 0x10,		// Colliding with something down.
			LedgePassing		= 0x20,		// The entity is passing over a ledge.
			HalfSolidPassing	= 0x40,		// The entity is passing over a half-solid tile.
		}

		private bool			isEnabled;		// Are physics enabled for the entity?
		private PhysicsFlags	flags;
		private Entity			entity;			// The entity this component belongs to.
		private Vector2F		velocity;		// XY-Velocity in pixels per frame.
		private float			zVelocity;		// Z-Velocity in pixels per frame.
		private float			gravity;		// Gravity in pixels per frame^2
		private Rectangle2F		collisionBox;	// The collision box used to collide with solid objects
		private Rectangle2F		softCollisionBox;	// The collision box used to collide with items, monsters, room edges, etc.
		private StateFlags		stateFlags;		// Flags for the physics state of the entity (is it colliding?)
		private CollisionInfo[] collisionInfo;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		// By default, physics are disabled.
		public PhysicsComponent(Entity entity) {
			this.isEnabled		= false;
			this.flags			= PhysicsFlags.None;
			this.stateFlags		= StateFlags.None;
			this.entity			= entity;
			this.velocity		= Vector2F.Zero;
			this.zVelocity		= 0.0f;
			this.gravity		= GameSettings.DEFAULT_GRAVITY;
			this.collisionBox	= new Rectangle2F(-4, -10, 8, 9);		// TEMP: this is the player collision box.
			this.softCollisionBox = new Rectangle2F(-6, -14, 12, 14);	// TEMP: this is the player collision box.

			this.collisionInfo	= new CollisionInfo[Directions.Count];
			for (int i = 0; i < Directions.Count; i++)
				collisionInfo[i] .Clear();

		}
		
		
		//-----------------------------------------------------------------------------
		// Accessors
		//-----------------------------------------------------------------------------
		
		public bool HasFlags(PhysicsFlags flags) {
			return ((this.flags & flags) == flags);
		}

		// Returns true if the entity is colliding in the given direction.
		public bool IsCollidingDirection(int direction) {
			return stateFlags.HasFlag((StateFlags) ((int) StateFlags.CollidingRight << direction));
		}

		
		//-----------------------------------------------------------------------------
		// Mutators
		//-----------------------------------------------------------------------------
		
		public void SetFlags(PhysicsFlags flagsToSet, bool enabled) {
			if (enabled)
				flags |= flagsToSet;
			else
				flags &= ~flagsToSet;
		}


		//-----------------------------------------------------------------------------
		// Simulation
		//-----------------------------------------------------------------------------

		public void Update(float ticks) {

			// Handle Z position.
			if (entity.ZPosition > 0.0f || zVelocity != 0.0f) {
				entity.ZPosition += zVelocity * ticks;

				// Apply gravity.
				if (HasFlags(PhysicsFlags.HasGravity))
					zVelocity -= gravity * ticks;

				if (entity.ZPosition <= 0.0f)
				{
					entity.ZPosition = 0.0f;
					zVelocity = 0.0f;
				}
			}
			else
				zVelocity = 0.0f;

			// Check world collisions.
			if (HasFlags(PhysicsFlags.CollideWorld))
				CheckCollisions(ticks);
	
			// Apply velocity.
			entity.Position += velocity * ticks;
			
			// Collide with room edges.
			if (HasFlags(PhysicsFlags.CollideRoomEdge))
				CheckRoomEdgeCollisions(softCollisionBox); // TODO: this should default to hard collision-box.

			// Check if outside room.
			if (HasFlags(PhysicsFlags.DestroyedOutsideRoom) &&
				!entity.RoomControl.RoomBounds.Contains(entity.Position))
			{
				entity.Destroy();
			}
		}

		
		//-----------------------------------------------------------------------------
		// Collisions
		//-----------------------------------------------------------------------------

		// This should be called after applying velocity.
		public void CheckRoomEdgeCollisions(Rectangle2F collisionBox) {
			Rectangle2F roomBounds = entity.RoomControl.RoomBounds;
			Rectangle2F myBox = Rectangle2F.Translate(collisionBox, entity.Position);

			if (myBox.Left < roomBounds.Left) {
				stateFlags |= StateFlags.Colliding | StateFlags.CollidingLeft;
				entity.X = roomBounds.Left - collisionBox.Left;
				velocity.X = 0;
			}
			else if (myBox.Right > roomBounds.Right) {
				stateFlags |= StateFlags.Colliding | StateFlags.CollidingRight;
				entity.X = roomBounds.Right - collisionBox.Right;
				velocity.X = 0;
			}
			if (myBox.Top < roomBounds.Top) {
				stateFlags |= StateFlags.Colliding | StateFlags.CollidingUp;
				entity.Y = roomBounds.Top - collisionBox.Top;
				velocity.Y = 0;
			}
			else if (myBox.Bottom > roomBounds.Bottom) {
				stateFlags |= StateFlags.Colliding | StateFlags.CollidingDown;
				entity.Y = roomBounds.Bottom - collisionBox.Bottom;
				velocity.Y = 0;
			}
		}

		public void CheckCollisions(float ticks) {
			Room room = entity.RoomControl.Room;

			// Remove collision state flags.
			stateFlags &= ~(StateFlags.Colliding | StateFlags.CollidingLeft |
				StateFlags.CollidingRight | StateFlags.CollidingUp | StateFlags.CollidingDown);
			for (int i = 0; i < Directions.Count; i++)
				collisionInfo[i].Clear();

			// Find the rectangular area of nearby tiles to collide with.
			Rectangle2F myBox = collisionBox;
			myBox.Point += entity.Position;
			myBox.Inflate(2, 2);
	
			Rectangle2F myBox2 = collisionBox;
			myBox2.Point += entity.Position + (velocity * ticks);
			myBox2.Inflate(2, 2);
			myBox = Rectangle2F.Union(myBox, myBox2);
	
			int x1 = (int) (myBox.Left   / (float) GameSettings.TILE_SIZE);
			int y1 = (int) (myBox.Top    / (float) GameSettings.TILE_SIZE);
			int x2 = (int) (myBox.Right  / (float) GameSettings.TILE_SIZE) + 1;
			int y2 = (int) (myBox.Bottom / (float) GameSettings.TILE_SIZE) + 1;

			Rectangle2I area;
			area.Point	= (Point2I) (myBox.TopLeft / (float) GameSettings.TILE_SIZE);
			area.Size	= ((Point2I) (myBox.BottomRight / (float) GameSettings.TILE_SIZE)) + Point2I.One - area.Point;
			area = Rectangle2I.Intersect(area, new Rectangle2I(Point2I.Zero, room.Size));

			// Collide with nearby solid tiles HORIZONTALLY and then VERTICALLY.
			for (int axis = 0; axis < 2; ++axis) {
				for (int x = area.Left; x < area.Right; ++x) {
					for (int y = area.Top; y < area.Bottom; ++y) {
						for (int i = 0; i < room.LayerCount; ++i) {
							Tile t = entity.RoomControl.GetTile(x, y, i);

							if (t != null && t.Flags.HasFlag(TileFlags.Solid) && t.CollisionModel != null) {
								ResolveCollision(ticks, axis, t, new Vector2F(x, y) * GameSettings.TILE_SIZE, t.CollisionModel);
							}
						}
					}
				}
			}
		}
		

		private bool ResolveCollision(float ticks, int axis, Tile tile, Vector2F modelPos, CollisionModel model) {
			bool collide = false;
			for (int i = 0; i < model.Boxes.Count; ++i) {
				if (ResolveCollision(ticks, axis, tile, Rectangle2F.Translate((Rectangle2F) model.Boxes[i], modelPos)))
					collide = true;
			}
			return collide;
		}

		private bool ResolveCollision(float ticks, int axis, Tile tile, Rectangle2F block) {
			bool collide = false;

			if (axis == 0) { // AXIS_0
				Rectangle2F myBox = Rectangle2F.Translate(collisionBox, entity.X + (velocity.X * ticks), entity.Y);
				if (myBox.Intersects(block)) {
					collide = true;
					stateFlags |= StateFlags.Colliding;
					velocity.X = 0.0f;

					if (myBox.Center.X < block.Center.X) {
						entity.X = block.Left - collisionBox.Right;
						stateFlags |= StateFlags.CollidingRight;
						collisionInfo[Directions.Right].SetTileCollision(tile);
					}
					else {
						entity.X = block.Right - collisionBox.Left;
						stateFlags |= StateFlags.CollidingLeft;
						collisionInfo[Directions.Left].SetTileCollision(tile);
					}
				}
			}
			else if (axis == 1) { // AXIS_Y
				Rectangle2F myBox = Rectangle2F.Translate(collisionBox, entity.Position + (velocity * ticks));
				if (myBox.Intersects(block)) {
					collide = true;
					stateFlags |= StateFlags.Colliding;
					velocity.Y = 0.0f;

					if (myBox.Center.Y < block.Center.Y) {
						entity.Y = block.Top - collisionBox.Bottom;
						stateFlags |= StateFlags.CollidingDown;
						collisionInfo[Directions.Down].SetTileCollision(tile);
					}
					else {
						entity.Y = block.Bottom - collisionBox.Top;
						stateFlags |= StateFlags.CollidingUp;
						collisionInfo[Directions.Up].SetTileCollision(tile);
					}
				}
			}

			return collide;
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

		public Rectangle2F CollisionBox {
			get { return collisionBox; }
			set { collisionBox = value; }
		}
		
		public CollisionInfo[] CollisionInfo {
			get { return collisionInfo; }
		}
		
		public bool IsColliding {
			get { return stateFlags.HasFlag(StateFlags.Colliding); }
		}

		public PhysicsFlags Flags {
			get { return flags; }
			set { flags = value; }
		}

		// Flags:
		
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
