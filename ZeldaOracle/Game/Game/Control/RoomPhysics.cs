using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Entities.Collisions;
using ZeldaOracle.Game.Tiles;

namespace ZeldaOracle.Game.Control {

	// UNUSED Class. I may use it at some point.


	public class RoomPhysics {

		private RoomControl roomControl;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public RoomPhysics(RoomControl roomControl) {
			this.roomControl = roomControl;
		}


		//-----------------------------------------------------------------------------
		// Physics Update
		//-----------------------------------------------------------------------------

		public void ProcessPhysics() {

			// Create list of collisions.
			for (int i = 0; i < roomControl.Entities.Count; i++) {
				Entity entity = roomControl.Entities[i];
				if (entity.Physics != null && entity.Physics.IsEnabled)
					ProcessEntity(entity);
			}
		}
		
		public void ProcessEntity(Entity entity) {
			// Reset collision states
			// Update Z dynamics
			// Collide with tiles & entities
			// Collide with room edge.
			// Integrate velocity.
			// Check ledges
			// Top tile - conveyor effect
			// Destroy outside of room
			// Hazard tiles
			// OnLand()

			// Initialize the collision state for this frame.
			entity.Physics.PushedDirection		= -1;
			entity.Physics.ClippedDirection		= -1;
			entity.Physics.ClippedDirections	= DirectionMask.None;
			entity.Physics.AnyCollisions		= false;
			for (int i = 0; i < 4; i++) {
				CollisionInfoNew collision = entity.Physics.CollisionInfoNew[i];
				if (!collision.IsResolved) {
					if (CanResolveCollision(collision)) {
						entity.Position -= Directions.ToVector(collision.PenetrationDirection) *
							Math.Max(0.0f, collision.PenetrationDistance - collision.MaxAllowedPenetrationDistance);
					}
						collision.IsResolved = true;
				}
				if (collision.IsResolved) {
					collision.Reset();
					collision.PenetrationDirection = i;
				}
			}

			// Detect collisions.
			DetectClipCollisions(entity);
			
			// Apply positional correction.
			for (int i = 0; i < 2; i++) {
				CollisionInfoNew collision1 = entity.Physics.CollisionInfoNew[i];
				CollisionInfoNew collision2 = entity.Physics.CollisionInfoNew[i + 2];

				// Only one collision can be resolved per axis.
				CollisionInfoNew collision = GetPriorityCollision(collision1, collision2);
				if (collision != null) {
					entity.Position -= Directions.ToVector(collision.PenetrationDirection) *
						Math.Max(0.0f, collision.PenetrationDistance - collision.MaxAllowedPenetrationDistance);
					collision.IsResolved = true;
				}
			}

			// Clip velocity from collisions.
			for (int i = 0; i < 4; i++) {
				CollisionInfoNew collision = entity.Physics.CollisionInfoNew[i];
				if (collision.IsColliding) {
					ClipEntityVelocity(entity, collision.PenetrationDirection);
					if (collision.CollidedObject is Tile)
						entity.Physics.CollisionInfo[i].SetTileCollision((Tile) collision.CollidedObject, i);
				}
			}
			
			// Detect and resolve collisions with movement.
			CheckCollisions(entity);
			

			// Integrate velocity.
			entity.Position += entity.Physics.Velocity;
			

			//UpdateEntityZPosition(entity);
			//UpdateEntityTopTile(entity);
		}

		private void ClipEntityVelocity(Entity entity, int direction) {
			if (direction == Directions.Right && entity.Physics.VelocityX > 0.0f)
				entity.Physics.VelocityX = 0.0f;
			if (direction == Directions.Down && entity.Physics.VelocityY > 0.0f)
				entity.Physics.VelocityY = 0.0f;
			if (direction == Directions.Left && entity.Physics.VelocityX < 0.0f)
				entity.Physics.VelocityX = 0.0f;
			if (direction == Directions.Up && entity.Physics.VelocityY < 0.0f)
				entity.Physics.VelocityY = 0.0f;
		}

		private bool CanResolveCollision(CollisionInfoNew collision) {
			return (collision.IsColliding && collision.PenetrationDistance > collision.MaxAllowedPenetrationDistance);
		}

		private CollisionInfoNew GetPriorityCollision(CollisionInfoNew collision1, CollisionInfoNew collision2) {
			bool canResolve1 = CanResolveCollision(collision1);
			bool canResolve2 = CanResolveCollision(collision2);

			if (!canResolve1 && !canResolve2)
				return null;
			else if (canResolve1 && !canResolve2)
				return collision1;
			else if (canResolve2 && !canResolve1)
				return collision2;

			Tile tile1 = collision1.CollidedObject as Tile;
			Tile tile2 = collision2.CollidedObject as Tile;

			if (tile1 != null && tile2 != null) {
				if (tile1.IsMoving && !tile2.IsMoving)
					return collision1;
				else if (tile2.IsMoving && !tile1.IsMoving)
					return collision2;
				else if (tile1.Layer > tile2.Layer)
					return collision1;
				else if (tile2.Layer > tile1.Layer)
					return collision2;
				else if (tile1.Position.Y < tile2.Position.Y)
					return collision1;
				else if (tile2.Position.Y < tile1.Position.Y)
					return collision2;
				else if (tile1.Position.X < tile2.Position.X)
					return collision1;
				else if (tile2.Position.X < tile1.Position.X)
					return collision2;
			}

			return collision1;
		}

		private void DetectClipCollisions(Entity entity) {
			Rectangle2F tileCheckArea = entity.Physics.PositionedCollisionBox;
			foreach (Tile tile in RoomControl.TileManager.GetTilesTouching(tileCheckArea)) {
				DetectClipCollisions(entity, tile);
			}
		}

		private void DetectClipCollisions(Entity entity, Tile tile) {
			if (tile.IsSolid && tile.CollisionModel != null && tile.CollisionStyle == CollisionStyle.Rectangular) {
				foreach (Rectangle2I box in tile.CollisionModel.Boxes) {
					Rectangle2F tileBox = box;
					tileBox.Point += tile.Position;
					DetectClipCollision(entity, tile, tileBox);
				}
			}
		}
		
		private void DetectClipCollision(Entity entity, Tile tile, Rectangle2F tileCollisionBox) {
			Rectangle2F entityCollisionBox = entity.Physics.PositionedCollisionBox;
			if (!entityCollisionBox.Intersects(tileCollisionBox))
				return;
			
			// Determine clip direction.
			int clipDirection = -1;
			/*if (tile.IsMoving) {
				Rectangle2F tilePrevCollisionBox = tileCollisionBox;
				tilePrevCollisionBox.Point += tile.PreviousPosition - tile.Position;
				if (entityCollisionBox.Intersects(tilePrevCollisionBox)) {
					clipDirection = Directions.Reverse(tile.MoveDirection);
				}
			}
			else*/ {
				Vector2F intersectionCenter = Rectangle2F.Intersect(entityCollisionBox, tileCollisionBox).Center;
				clipDirection = Directions.NearestFromVector(tileCollisionBox.Center - intersectionCenter);
			}

			// Perform collision resolution.
			if (clipDirection >= 0) {
				Vector2F prevPos = entity.Position;
				Vector2F nextPos = entity.Position;

				// Determine position to snap to.
				Rectangle2F collisionBox = entity.Physics.CollisionBox;
				if (clipDirection == Directions.Left)
					nextPos.X = tileCollisionBox.Right - collisionBox.Left;
				else if (clipDirection == Directions.Right)
					nextPos.X = tileCollisionBox.Left - collisionBox.Right;
				else if (clipDirection == Directions.Up)
					nextPos.Y = tileCollisionBox.Bottom - collisionBox.Top;
				else if (clipDirection == Directions.Down)
					nextPos.Y = tileCollisionBox.Top - collisionBox.Bottom;

				// Set the collision info.
				int penetrationAxis = Directions.ToAxis(clipDirection);
				float penetrationDistance = Math.Abs(nextPos[penetrationAxis] - prevPos[penetrationAxis]);
				CollisionInfoNew collisionInfo = entity.Physics.CollisionInfoNew[clipDirection];
				if (!collisionInfo.IsColliding || (penetrationDistance > collisionInfo.PenetrationDistance)) {
					collisionInfo.PenetrationDistance	= penetrationDistance;
					collisionInfo.IsColliding			= true;
					collisionInfo.CollidedObject		= tile;
					collisionInfo.CollisionBox			= tileCollisionBox;
					collisionInfo.MaxAllowedPenetrationDistance = 2.0f;
				}
			}
		}

		private void CheckCollisions(Entity entity) {
			// 1. Soft Tiles
			// 2. Soft Entities
			// 3. Hard Tiles
			// 4. Hard Entities

			Rectangle2F tileCheckArea = Rectangle2F.Union(
				Rectangle2F.Translate(entity.Physics.CollisionBox, entity.Position),
				Rectangle2F.Translate(entity.Physics.CollisionBox, entity.Position + entity.Physics.Velocity));

			// Collide with hard tiles.
			for (int axis = 0; axis < 2; axis++) {
				foreach (Tile tile in RoomControl.TileManager.GetTilesTouching(tileCheckArea)) {
					CheckCollision(entity, axis, tile);
				}
			}
		}

		private void CheckCollision(Entity entity, int axis, Tile tile) {
			if (tile.IsSolid && tile.CollisionModel != null && tile.CollisionStyle == CollisionStyle.Rectangular) {
				foreach (Rectangle2I box in tile.CollisionModel.Boxes) {
					Rectangle2F tileBox = box;
					tileBox.Point += tile.Position;
					CheckCollision(entity, axis, tile, tileBox);
				}
			}
		}
		
		private void CheckCollision(Entity entity, int axis, Tile tile, Rectangle2F tileCollisionBox) {
			Rectangle2F collisionBox = entity.Physics.CollisionBox;
			
			// TODO: Collision dodging.

			if (axis == Axes.X) {
				if (entity.Physics.CollisionInfoNew[Directions.Up].IsColliding ||
					entity.Physics.CollisionInfoNew[Directions.Down].IsColliding)
					return;
				bool isColliding = false;
				Rectangle2F entityCollisionBox = Rectangle2F.Translate(
					entity.Physics.CollisionBox, entity.X + entity.Physics.Velocity.X, entity.Y);
				
				if (entityCollisionBox.Intersects(tileCollisionBox)) {
					if (entityCollisionBox.Center.X < tileCollisionBox.Center.X) {
						if (entity.Physics.VelocityX > 0.0f && !entity.Physics.CollisionInfoNew[Directions.Left].IsColliding) {
							entity.X = tileCollisionBox.Left - collisionBox.Right;
							entity.Physics.CollisionInfo[Directions.Right].SetTileCollision(tile, Directions.Right);
							isColliding = true;
						}
					}
					else {
						if (entity.Physics.VelocityX < 0.0f && !entity.Physics.CollisionInfoNew[Directions.Right].IsColliding) {
							entity.X = tileCollisionBox.Right - collisionBox.Left;
							entity.Physics.CollisionInfo[Directions.Left].SetTileCollision(tile, Directions.Left);
							isColliding = true;
						}
					}
					if (isColliding)
						entity.Physics.VelocityX = 0.0f;
				}
			}
			else {
				if (entity.Physics.CollisionInfoNew[Directions.Left].IsColliding ||
					entity.Physics.CollisionInfoNew[Directions.Right].IsColliding)
					return;
				bool isColliding = false;
				Rectangle2F entityCollisionBox = Rectangle2F.Translate(
					entity.Physics.CollisionBox, entity.Position + entity.Physics.Velocity);
				
				if (entityCollisionBox.Intersects(tileCollisionBox)) {
					if (entityCollisionBox.Center.Y < tileCollisionBox.Center.Y) {
						if (entity.Physics.VelocityY > 0.0f && !entity.Physics.CollisionInfoNew[Directions.Up].IsColliding) {
							entity.Y = tileCollisionBox.Top - collisionBox.Bottom;
							entity.Physics.CollisionInfo[Directions.Down].SetTileCollision(tile, Directions.Down);
							isColliding = true;
						}
					}
					else {
						if (entity.Physics.VelocityY < 0.0f && !entity.Physics.CollisionInfoNew[Directions.Down].IsColliding) {
							entity.Y = tileCollisionBox.Bottom - collisionBox.Top;
							entity.Physics.CollisionInfo[Directions.Up].SetTileCollision(tile, Directions.Up);
							isColliding = true;
						}
					}
					if (isColliding)
						entity.Physics.VelocityY = 0.0f;
				}
			}
		}

		public void UpdateEntityTopTile(Entity entity) {
			Tile topTile = null;

			foreach (Tile tile in roomControl.GetTiles()) {
				Rectangle2F tileRect = new Rectangle2F(tile.Position, tile.Size * GameSettings.TILE_SIZE);
				if (!tile.IsSolid && tileRect.Contains(entity.Position) &&
					(topTile == null || tile.Layer > topTile.Layer))
				{
					topTile = tile;
				}
			}

			if (topTile != null) {
				// TODO: Integrate the surface tile's velocity into our
				// velocity rather than just moving position.
				entity.Position += topTile.Velocity;
				if (entity.Physics.MovesWithConveyors && entity.Physics.IsOnGround)
					entity.Position += topTile.ConveyorVelocity;
			}

			entity.Physics.TopTile = topTile;
		}

		public void UpdateEntityZPosition(Entity entity) {
			if (entity.ZPosition > 0.0f || entity.Physics.ZVelocity != 0.0f) {
				// Apply gravity.
				if (entity.Physics.HasFlags(PhysicsFlags.HasGravity)) {
					entity.Physics.ZVelocity -= entity.Physics.Gravity;
					if (entity.Physics.ZVelocity < -entity.Physics.MaxFallSpeed && entity.Physics.MaxFallSpeed >= 0)
						entity.Physics.ZVelocity = -entity.Physics.MaxFallSpeed;
				}

				// Apply z-velocity.
				entity.ZPosition += entity.Physics.ZVelocity;

				// Check if landed on the ground.
				if (entity.ZPosition <= 0.0f) {
					//hasLanded = true;
					entity.ZPosition = 0.0f;

					if (entity.Physics.HasFlags(PhysicsFlags.Bounces)) {
						BounceEntity(entity);
					}
					else {
						entity.ZPosition = 0.0f;
						entity.Physics.ZVelocity = 0.0f;
					}
				}
			}
			else
				entity.Physics.ZVelocity = 0.0f;
		}

		public void BounceEntity(Entity entity) {
			if (entity.Physics.ZVelocity < -1.0f) {
				// Bounce back into the air.
				//hasLanded = false;
				entity.ZPosition = 0.1f;
				entity.Physics.ZVelocity = -entity.Physics.ZVelocity * 0.5f;
			}
			else {
				// Stay on the ground.
				entity.ZPosition = 0.0f;
				entity.Physics.ZVelocity = 0;
				entity.Physics.Velocity = Vector2F.Zero;
			}

			if (entity.Physics.Velocity.Length > 0.25)
				entity.Physics.Velocity *= 0.5f;
			else
				entity.Physics.Velocity = Vector2F.Zero;
		}

		
		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public RoomControl RoomControl {
			get { return roomControl; }
		}
		
		public GameControl GameControl {
			get { return roomControl.GameControl; }
		}
	}
}
