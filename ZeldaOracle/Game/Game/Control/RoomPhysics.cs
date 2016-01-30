using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Entities.Collisions;
using ZeldaOracle.Game.Entities.Players;
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

			// Collision Order:
			//   1. Soft Tiles
			//   2. Soft Entities
			//   3. Hard Tiles
			//   4. Hard Entities

			// Clear the collision state.
			entity.Physics.IsColliding = false;
			for (int i = 0; i < Directions.Count; i++) {
				entity.Physics.CollisionInfo[i].Clear();
			}

			// Perform world collisions.
			if (entity.Physics.CollideWithWorld) {
				// 1. Resolve unresolved collisions from the previous frame.
				ResolvePreviousCollisions(entity);
				// 2. Detect collisions.
				DetectClipCollisions(entity);
				// 3. Apply positional correction.
				ApplyPositionalCorrection(entity);
				// 4. Clip velocity from collisions.
				ClipVelocity(entity);
				// 5. Check if the entity is being crushed.
				if (entity.Physics.IsCrushable)
					CheckCrush(entity);
				// 6. Detect and resolve collisions with movement.
				ResolveMovementCollisions(entity);
			}

			// Integrate velocity.
			entity.Position += entity.Physics.Velocity;

			//UpdateEntityZPosition(entity);
			//UpdateEntityTopTile(entity);
		}

		// Resolve collisions that were not resolved from the previous frame.
		private void ResolvePreviousCollisions(Entity entity) {
			for (int i = 0; i < Directions.Count; i++) {
				CollisionInfoNew collision = entity.Physics.CollisionInfoNew[i];
				if (!collision.IsResolved && CanResolveCollision(collision)) {
					entity.Position -= Directions.ToVector(collision.PenetrationDirection) *
						Math.Max(0.0f, collision.PenetrationDistance - collision.MaxAllowedPenetrationDistance);
				}
				collision.Reset();
				collision.PenetrationDirection = i;
			}
		}
		
		// Resolve tile collisions.
		private void ApplyPositionalCorrection(Entity entity) {
			for (int i = 0; i < 2; i++) {
				CollisionInfoNew collision1 = entity.Physics.CollisionInfoNew[i];
				CollisionInfoNew collision2 = entity.Physics.CollisionInfoNew[i + 2];

				// Only one collision can be resolved per axis.
				CollisionInfoNew collision = GetPriorityCollision(collision1, collision2);
				if (collision != null) {
					float resolveDistance = Math.Max(0.0f, collision.PenetrationDistance - collision.MaxAllowedPenetrationDistance);
					entity.Position -= Directions.ToVector(collision.PenetrationDirection) * resolveDistance;
					collision.IsResolved = true;
					CollisionInfoNew otherCollision = entity.Physics.CollisionInfoNew[(collision.PenetrationDirection + 2) % 4];
					if (otherCollision.IsColliding)
						DetectClipCollision(entity, (Tile) otherCollision.CollidedObject, otherCollision.CollisionBox);
						//otherCollision.PenetrationDistance += resolveDistance;
				}
			}
		}
		
		// Restrict an entity's velocity based on its collisions.
		private void ClipVelocity(Entity entity) {
			for (int i = 0; i < Directions.Count; i++) {
				CollisionInfoNew collision = entity.Physics.CollisionInfoNew[i];
				if (collision.IsColliding) {
					if (!collision.IsAllowedClipping)
						ClipEntityVelocity(entity, collision.PenetrationDirection);
					if (collision.CollidedObject is Tile) {
						entity.Physics.CollisionInfo[i].SetTileCollision((Tile) collision.CollidedObject, i);
						entity.Physics.IsColliding = true;
					}
				}
			}
		}
		
		// Check if the entity is being crushed.
		private void CheckCrush(Entity entity) {
			// Check crush for each axis.
			for (int axis = 0; axis < Axes.Count; axis++) {
				CollisionInfoNew collision1 = entity.Physics.CollisionInfoNew[axis];
				CollisionInfoNew collision2 = entity.Physics.CollisionInfoNew[axis + 2];

				// Make sure the entity is colliding between two objects along this axis.
				float penetration = 0.0f;
				if (collision1.IsColliding && collision2.IsColliding) {
					if (CanResolveCollision(collision1))
						penetration = collision1.PenetrationDistance;
					else if (CanResolveCollision(collision2))
						penetration = collision2.PenetrationDistance;

					if (penetration > 0.0f) {
						// Check if the size of the gap the entity is in is small 
						float gap = entity.Physics.CollisionBox.Size[axis] - penetration;
						if (gap <= entity.Physics.CrushMaxGapSize)
							entity.OnCrush(axis == Axes.X);
					}
				}
			}
		}

		// Restrict the entity's velocity in the given direction.
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

		// Returns true if the given collision needs resolution.
		private bool CanResolveCollision(CollisionInfoNew collision) {
			return (collision.IsColliding && collision.PenetrationDistance > collision.MaxAllowedPenetrationDistance);
		}

		// Return the collision that takes holds resolution priority.
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
					if (entity.Physics.AllowEdgeClipping)
						collisionInfo.MaxAllowedPenetrationDistance = entity.Physics.EdgeClipAmount;
				}
			}
		}

		private void ResolveMovementCollisions(Entity entity) {
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
		
		// Returns true if the entity allowably clipping the given collision.
		private bool IsSafeClipping(Entity entity, int axis, Rectangle2F entityCollisionBox, Tile tile, Rectangle2F tileCollisionBox) {
			if (entity.Physics.AllowEdgeClipping) {
				float penetration = Math.Min(
					tileCollisionBox.BottomRight[axis] - entityCollisionBox.TopLeft[axis],
					entityCollisionBox.BottomRight[axis] - tileCollisionBox.TopLeft[axis]);
				return (penetration <= entity.Physics.EdgeClipAmount);
			}
			return false;
		}

		private void CheckCollision(Entity entity, int axis, Tile tile, Rectangle2F tileCollisionBox) {
			// TODO: Collision dodging.
			
			int collideDirection = -1;
			Rectangle2F entityCollisionBox;
			
			if (entity.Physics.CollisionInfoNew[(axis + 1) % 4].IsColliding &&
				entity.Physics.CollisionInfoNew[(axis + 3) % 4].IsColliding)
				return;

			if (axis == Axes.X) {
				entityCollisionBox = Rectangle2F.Translate(
					entity.Physics.CollisionBox, entity.X + entity.Physics.Velocity.X, entity.Y);
			}
			else {
				entityCollisionBox = Rectangle2F.Translate(
					entity.Physics.CollisionBox, entity.Position + entity.Physics.Velocity);
			}
			
			if (!entityCollisionBox.Intersects(tileCollisionBox) ||
				IsSafeClipping(entity, 1 - axis, entityCollisionBox, tile, tileCollisionBox))
				return;

			// Determine collision direction.
			if (axis == Axes.X) {
				if (entityCollisionBox.Center.X < tileCollisionBox.Center.X) {
					if (entity.Physics.VelocityX > 0.0f && !entity.Physics.CollisionInfoNew[Directions.Left].IsColliding)
						collideDirection = Directions.Right;
				}
				else {
					if (entity.Physics.VelocityX < 0.0f && !entity.Physics.CollisionInfoNew[Directions.Right].IsColliding)
						collideDirection = Directions.Left;
				}
			}
			else {
				if (entityCollisionBox.Center.Y < tileCollisionBox.Center.Y) {
					if (entity.Physics.VelocityY > 0.0f && !entity.Physics.CollisionInfoNew[Directions.Up].IsColliding)
						collideDirection = Directions.Down;
				}
				else {
					if (entity.Physics.VelocityY < 0.0f && !entity.Physics.CollisionInfoNew[Directions.Down].IsColliding)
						collideDirection = Directions.Up;
				}
			}

			// Resolve collision.
			if (collideDirection >= 0) {
				
				Vector2F prevPos = entity.Position;
				Vector2F nextPos = entity.Position;
				Rectangle2F collisionBox = entity.Physics.CollisionBox;
				if (collideDirection == Directions.Left)
					nextPos.X = tileCollisionBox.Right - collisionBox.Left;
				else if (collideDirection == Directions.Right)
					nextPos.X = tileCollisionBox.Left - collisionBox.Right;
				else if (collideDirection == Directions.Up)
					nextPos.Y = tileCollisionBox.Bottom - collisionBox.Top;
				else if (collideDirection == Directions.Down)
					nextPos.Y = tileCollisionBox.Top - collisionBox.Bottom;
				
				if (!entity.Physics.CollisionInfoNew[collideDirection].IsColliding) {
					// Set the collision info.
					int penetrationAxis = Directions.ToAxis(collideDirection);
					float penetrationDistance = (nextPos - prevPos).Dot(Directions.ToVector(collideDirection));
					float maxAllowedPenetration = 0.0f;
					if (entity.Physics.Velocity[axis] == 0.0f && entity.Physics.AllowEdgeClipping)
						maxAllowedPenetration = entity.Physics.EdgeClipAmount;
					
					entity.Physics.IsColliding = true;
					entity.Physics.CollisionInfo[collideDirection].SetTileCollision(tile, collideDirection);
					
					entity.Position = nextPos;
					if (penetrationDistance <= maxAllowedPenetration)
						entity.Position += Directions.ToVector(collideDirection) * penetrationDistance;
				}

				Vector2F velocity = entity.Physics.Velocity;
				velocity[axis] = 0.0f;
				entity.Physics.Velocity = velocity;
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
