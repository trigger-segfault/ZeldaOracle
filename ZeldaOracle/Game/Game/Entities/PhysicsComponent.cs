using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Collision;
using ZeldaOracle.Game.Worlds;
using ZeldaOracle.Game.Tiles;

namespace ZeldaOracle.Game.Entities {

	public class PhysicsComponent {

		private Entity		entity;			// The entity this component belongs to.
		private Vector2F	velocity;		// XY-Velocity in pixels per frame.
		private float		zVelocity;		// Z-Velocity in pixels per frame.
		private float		gravity;		// Gravity in pixels per frame^2
		private Rectangle2F collisionBox;	// The collision box used to collide with things


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public PhysicsComponent(Entity entity) {
			this.entity			= entity;
			this.velocity		= Vector2F.Zero;
			this.zVelocity		= 0.0f;
			this.collisionBox	= new Rectangle2F(-4, -10, 8, 9); // TEMP: player collision box.
			this.gravity		= GameSettings.DEFAULT_GRAVITY;
		}
		
		
		//-----------------------------------------------------------------------------
		// Simulation
		//-----------------------------------------------------------------------------

		public void Update(float ticks) {

			// Handle Z position.
			if (entity.ZPosition > 0.0f || zVelocity != 0.0f) {
				entity.ZPosition += zVelocity * ticks;

				// Apply gravity.
				if (entity.Flags.HasFlag(EntityFlags.HasGravity))
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
			if (entity.Flags.HasFlag(EntityFlags.CollideWorld))
				CheckCollisions(ticks);
	
			// Apply velocity.
			entity.Position += velocity * ticks;

			// Check if outside room.
			Rectangle2F bounds = new Rectangle2F(Vector2F.Zero, entity.RoomControl.Room.Size * GameSettings.TILE_SIZE);
			if (!bounds.Contains(entity.Position) && entity.Flags.HasFlag(EntityFlags.DestroyedOutsideRoom))
				entity.Destroy();
		}

		
		//-----------------------------------------------------------------------------
		// Collisions
		//-----------------------------------------------------------------------------

		public void CheckCollisions(float ticks) {
			Room room = entity.RoomControl.Room;
			Vector2F pos = entity.Position;

			Rectangle2F myBox = collisionBox;
			myBox.Point += entity.Position;
			myBox.Inflate(2, 2);
	
			Rectangle2F myBox2 = collisionBox;
			myBox2.Point += entity.Position + (velocity * ticks);
			myBox2.Inflate(2, 2);
			myBox = Rectangle2F.Union(myBox, myBox2);

			//Util::removeFlags(&m_stateFlags, PHYS_STATE_COLLIDING | PHYS_STATE_COLLIDING_LEFT |
			//	PHYS_STATE_COLLIDING_RIGHT | PHYS_STATE_COLLIDING_UP | PHYS_STATE_COLLIDING_DOWN);
	
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
								ResolveCollision(ticks, axis, new Vector2F(x, y) * GameSettings.TILE_SIZE, t.CollisionModel);
							}
						}
					}
				}
			}
		}
		

		private bool ResolveCollision(float ticks, int axis, Vector2F modelPos, CollisionModel model)
		{
			bool collide = false;
			for (int i = 0; i < model.Boxes.Count; ++i) {
				if (ResolveCollision(ticks, axis, Rectangle2F.Translate((Rectangle2F) model.Boxes[i], modelPos)))
					collide = true;
			}
			return collide;
		}

		private bool ResolveCollision(float ticks, int axis, Rectangle2F block)
		{
			Vector2F pos = entity.Position;
			bool collide = false;

			if (axis == 0) { // AXIS_0
				Rectangle2F myBox = Rectangle2F.Translate(collisionBox, pos.X + (velocity.X * ticks), pos.Y);
				if (myBox.Intersects(block)) {
					collide = true;
					//stateFlags |= PHYS_STATE_COLLIDING;
					velocity.X = 0.0f;

					if (myBox.Center.X < block.Center.X) {
						pos.X = block.Left - collisionBox.Right;
						//stateFlags |= PHYS_STATE_COLLIDING_RIGHT;
					}
					else {
						pos.X = block.Right - collisionBox.Left;
						//stateFlags |= PHYS_STATE_COLLIDING_LEFT;
					}
			
					entity.Position = pos;
				}
			}
			else if (axis == 1) { // AXIS_Y
				Rectangle2F myBox = Rectangle2F.Translate(collisionBox, pos + (velocity * ticks));
				if (myBox.Intersects(block)) {
					collide = true;
					//stateFlags |= PHYS_STATE_COLLIDING;
					velocity.Y = 0.0f;

					if (myBox.Center.Y < block.Center.Y) {
						pos.Y = block.Top - collisionBox.Bottom;
						//stateFlags |= PHYS_STATE_COLLIDING_DOWN;
					}
					else {
						pos.Y = block.Bottom - collisionBox.Top;
						//stateFlags |= PHYS_STATE_COLLIDING_UP;
					}
					
					entity.Position = pos;
				}
			}

			return collide;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------
		
		public float Gravity {
			get { return gravity; }
			set { gravity = value; }
		}

		public Entity Entity {
			get { return entity; }
			set { entity = value; }
		}

		public Vector2F Velocity {
			get { return velocity; }
			set { velocity = value; }
		}

		public float ZVelocity {
			get { return zVelocity; }
			set { zVelocity = value; }
		}
	}
}
