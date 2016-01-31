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

		public void ClearCollisionState(Entity entity) {
			entity.Physics.IsColliding = false;
			for (int i = 0; i < Directions.Count; i++) {
				entity.Physics.CollisionInfo[i].Clear();
				CollisionInfoNew collision = entity.Physics.CollisionInfoNew[i];
				collision.Reset();
				collision.PenetrationDirection = i;
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
				// 3. Resolve collisions.
				ApplyPositionalCorrection(entity);
				// 4. Detect any new unresolved collisions.
				DetectClipCollisions(entity);
				// 5. Clip velocity for all detected collisions.
				ClipVelocity(entity);
				// 6. Check if the entity is being crushed.
				if (entity.Physics.IsCrushable)
					CheckCrush(entity);
				// 7. Detect and resolve collisions with movement.
				ResolveMovementCollisions(entity);
			}

			// Collide with room edges.
			if (entity.Physics.CollideWithRoomEdge || entity.Physics.ReboundRoomEdge)
				entity.Physics.CheckRoomEdgeCollisions(entity.Physics.GetCollisionBox(entity.Physics.RoomEdgeCollisionBoxType));

			// Integrate velocity.
			entity.Position += entity.Physics.Velocity;

			//UpdateEntityZPosition(entity);
			//UpdateEntityTopTile(entity);
		}

		// Resolve collisions that were not resolved from the previous frame.
		private void ResolvePreviousCollisions(Entity entity) {
			for (int i = 0; i < Directions.Count; i++) {
				CollisionInfoNew collision = entity.Physics.CollisionInfoNew[i];

				entity.Physics.ClippingDirections[i] = collision.IsAllowedClipping;

				if (!collision.IsResolved && CanResolveCollision(entity, collision)) {
					float resolveDistance = Math.Max(0.0f, collision.PenetrationDistance - collision.MaxAllowedPenetrationDistance);
					entity.Position -= Directions.ToVector(collision.PenetrationDirection) * resolveDistance;
				}
				collision.Reset();
				collision.PenetrationDirection = i;
				//DetectClipCollision(entity, (Tile) collision.CollidedObject, collision.CollisionBox);
			}
		}
		
		// Resolve tile collisions.
		private void ApplyPositionalCorrection(Entity entity) {
			for (int i = 0; i < 2; i++) {
				CollisionInfoNew collision1 = entity.Physics.CollisionInfoNew[i];
				CollisionInfoNew collision2 = entity.Physics.CollisionInfoNew[i + 2];

				// Only one collision can be resolved per axis.
				CollisionInfoNew collision = GetPriorityCollision(entity, collision1, collision2);
				if (collision != null) {
					float resolveDistance = Math.Max(0.0f, collision.PenetrationDistance - collision.MaxAllowedPenetrationDistance);
					Vector2F newPosition = entity.Position - Directions.ToVector(collision.PenetrationDirection) * resolveDistance;
					entity.Position = newPosition;
					collision.IsResolved = true;
					CollisionInfoNew otherCollision = entity.Physics.CollisionInfoNew[(collision.PenetrationDirection + 2) % 4];
					//if (otherCollision.IsColliding)
					//	DetectClipCollision(entity, otherCollision.CollidedObject, otherCollision.CollisionBox);
						otherCollision.PenetrationDistance += resolveDistance;
				}
			}
		}
		
		// Restrict an entity's velocity based on its collisions.
		private void ClipVelocity(Entity entity) {
			// Check if the entity is in a deadlock (has collisions but cannot resolve any of them).
			/*entity.Physics.IsDeadlocked = false;
			for (int i = 0; i < Directions.Count; i++) {
				CollisionInfoNew collision = entity.Physics.CollisionInfoNew[i];
				if (collision.IsColliding) {
					if (collision.IsResolvable) {
						entity.Physics.IsDeadlocked = false;
						break;
					}
					else
						entity.Physics.IsDeadlocked = true;
				}
			}
			if (entity.Physics.IsDeadlocked)
				entity.Physics.Velocity = Vector2F.Zero;*/

			// Clip velocities for collisions.
			for (int i = 0; i < Directions.Count; i++) {
				CollisionInfoNew collision = entity.Physics.CollisionInfoNew[i];
				if (collision.IsColliding) {
					if (!collision.IsAllowedClipping)
						ClipEntityVelocity(entity, collision.PenetrationDirection);
					if (collision.CollidedObject is Tile) {
						entity.Physics.CollisionInfo[i].SetCollision(collision.CollidedObject, i);
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
					if (collision1.IsResolvable)
						penetration = collision1.PenetrationDistance;
					else if (collision2.IsResolvable)
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
		private bool CanResolveCollision(Entity entity, CollisionInfoNew collision) {
			if (!collision.IsColliding || collision.PenetrationDistance <= collision.MaxAllowedPenetrationDistance)
				return false;
			
			// Static collisions cannot resolve into colliding with other static objects.
			Tile tile = collision.CollidedObject as Tile;
			if (tile != null && !tile.IsInMotion) {
				float resolveDistance = Math.Max(0.0f, collision.PenetrationDistance - collision.MaxAllowedPenetrationDistance);
				Vector2F newPosition = entity.Position - Directions.ToVector(collision.PenetrationDirection) * resolveDistance;
				if (IsCollidingAt(entity, newPosition, true)) {
					return false;
				}
			}

			return true;
		}

		// Return the collision that takes holds resolution priority.
		private CollisionInfoNew GetPriorityCollision(Entity entity, CollisionInfoNew collision1, CollisionInfoNew collision2) {
			if (!collision1.IsResolvable && !collision2.IsResolvable)
				return null;
			else if (collision1.IsResolvable && !collision2.IsResolvable)
				return collision1;
			else if (collision2.IsResolvable && !collision1.IsResolvable)
				return collision2;

			Tile tile1 = collision1.CollidedObject as Tile;
			Tile tile2 = collision2.CollidedObject as Tile;

			if (tile1 != null && tile2 != null) {
				if (tile1.IsInMotion && !tile2.IsInMotion)
					return collision1;
				else if (tile2.IsInMotion && !tile1.IsInMotion)
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

		private bool IsCollidingAt(Entity entity, Vector2F position, bool onlyStatic) {
			Rectangle2F entityBox = entity.Physics.CollisionBox;
			entityBox.Point += position;
			if (entity.Physics.CollideWithRoomEdge && !((Rectangle2F) roomControl.RoomBounds).Contains(entityBox))
				return true;
			foreach (CollisionCheck check in GetCollisions(entity, entityBox)) {
				if (onlyStatic && (check.CollisionObject is Tile) && ((Tile) check.CollisionObject).IsInMotion)
					continue;
				if (check.CollisionBox.Intersects(entityBox))
					return true;
			}
			return false;
		}

		private struct CollisionCheck {
			public object CollisionObject { get; set; }
			public Rectangle2F CollisionBox { get; set; }
		}

		private IEnumerable<CollisionCheck> GetCollisions(Entity entity, Rectangle2F area) {
			// Detect collisions with entities.
			if (entity.Physics.CollideWithEntities) {
				foreach (Entity other in RoomControl.Entities) {
					if (other != entity && other.Physics.IsEnabled && other.Physics.IsSolid) {
						yield return new CollisionCheck() {
							CollisionBox = other.Physics.PositionedCollisionBox,
							CollisionObject = other
						};
					}
				}
			}
			// Detect collisions with tiles.
			if (entity.Physics.CollideWithWorld) {
				foreach (Tile tile in RoomControl.TileManager.GetTilesTouching(area)) {
					if (tile.IsSolid && tile.CollisionModel != null && tile.CollisionStyle == CollisionStyle.Rectangular) {
						foreach (Rectangle2I box in tile.CollisionModel.Boxes) {
							Rectangle2F tileBox = box;
							tileBox.Point += tile.Position;
							yield return new CollisionCheck() {
								CollisionBox = tileBox,
								CollisionObject = tile
							};
						}
					}
				}
			}
		}

		private void DetectClipCollisions(Entity entity, int clipAxis = -1) {
			Rectangle2F tileCheckArea = entity.Physics.PositionedCollisionBox;
			foreach (CollisionCheck check in GetCollisions(entity, tileCheckArea))
				DetectClipCollision(entity, check.CollisionObject, check.CollisionBox, clipAxis);
			foreach (CollisionInfoNew collision in entity.Physics.CollisionInfoNew)
				collision.IsResolvable = CanResolveCollision(entity, collision);
		}

		private void DetectClipCollisions(Entity entity, Entity other) {
			if (other.Physics.IsEnabled && other.Physics.IsSolid) {
				Rectangle2F tileBox = other.Physics.PositionedCollisionBox;
				DetectClipCollision(entity, other, tileBox);
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

		private int GetCollisionPenetrationDirection(Entity entity, object other, Rectangle2F solidBox, int clipAxis = -1) {
			Rectangle2F entityBox = entity.Physics.PositionedCollisionBox;
			
			// Direction for moving tiles.
			Tile tile = other as Tile;
			if (tile != null && tile.IsMoving) {
				if ((solidBox.Center - entityBox.Center).Dot(Directions.ToVector(tile.MoveDirection)) < 0.0f)
					return Directions.Reverse(tile.MoveDirection);
			}

			// Nearest direction along the specified clip axis.
			if (clipAxis >= 0) {
				if (entityBox.Center[clipAxis] < solidBox.Center[clipAxis])
					return (clipAxis == Axes.X ? Directions.Right : Directions.Down);
				else
					return (clipAxis == Axes.X ? Directions.Left : Directions.Up);
			}

			// Get the nearest direction from the center of the collision.
			Vector2F intersectionCenter = Rectangle2F.Intersect(entityBox, solidBox).Center;
			int clipDirection = Directions.NearestFromVector(solidBox.Center - intersectionCenter);
			
			// Use a backup if the collision can't be resolved for this direction.
			CollisionInfoNew testCollision = CreateCollisionInfo(entity, other, solidBox, clipDirection);
			if (!CanResolveCollision(entity, testCollision)) {
				int a = 1 - Directions.ToAxis(clipDirection);
				if (entityBox.Center[a] < solidBox.Center[a])
					return (a == Axes.X ? Directions.Right : Directions.Down);
				else
					return (a == Axes.X ? Directions.Left : Directions.Up);
			}
			
			return clipDirection;
		}
		
		private void DetectClipCollision(Entity entity, object other, Rectangle2F solidBox, int clipAxis = -1) {
			Rectangle2F entityBox = entity.Physics.PositionedCollisionBox;
			if (!entityBox.Intersects(solidBox))
				return;
			
			// Determine clip direction.
			int clipDirection = GetCollisionPenetrationDirection(entity, other, solidBox, clipAxis);
			
			// Perform collision resolution.
			if (clipDirection >= 0) {
				Vector2F prevPos = entity.Position;
				Vector2F nextPos = entity.Position;

				/*if (entity.Physics.ClippingDirections[(clipDirection + 1) % 4] ||
					entity.Physics.ClippingDirections[(clipDirection + 3) % 4])
					return;*/
				/*if (entity.Physics.CollisionInfoNew[(clipDirection + 1) % 4].IsAllowedClipping ||
					entity.Physics.CollisionInfoNew[(clipDirection + 3) % 4].IsAllowedClipping)
					return;*/

				// Determine position to snap to.
				Rectangle2F collisionBox = entity.Physics.CollisionBox;
				if (clipDirection == Directions.Left)
					nextPos.X = solidBox.Right - collisionBox.Left;
				else if (clipDirection == Directions.Right)
					nextPos.X = solidBox.Left - collisionBox.Right;
				else if (clipDirection == Directions.Up)
					nextPos.Y = solidBox.Bottom - collisionBox.Top;
				else if (clipDirection == Directions.Down)
					nextPos.Y = solidBox.Top - collisionBox.Bottom;

				// Set the collision info.
				int penetrationAxis = Directions.ToAxis(clipDirection);
				float penetrationDistance = Math.Abs(nextPos[penetrationAxis] - prevPos[penetrationAxis]);
				CollisionInfoNew collisionInfo = entity.Physics.CollisionInfoNew[clipDirection];
				if (!collisionInfo.IsColliding || (penetrationDistance > collisionInfo.PenetrationDistance)) {
					collisionInfo.PenetrationDistance	= penetrationDistance;
					collisionInfo.IsColliding			= true;
					collisionInfo.CollidedObject		= other;
					collisionInfo.CollisionBox			= solidBox;
					if (entity.Physics.AllowEdgeClipping)
						collisionInfo.MaxAllowedPenetrationDistance = entity.Physics.EdgeClipAmount;
					entity.Physics.ClippingDirections[clipDirection] = collisionInfo.IsAllowedClipping;
				}
			}
		}

		private CollisionInfoNew CreateCollisionInfo(Entity entity, object other,
				Rectangle2F solidBox, int clipDirection)
		{
			Rectangle2F collisionBox = entity.Physics.CollisionBox;
			Vector2F prevPos = entity.Position;
			Vector2F nextPos = entity.Position;
			if (clipDirection == Directions.Left)
				nextPos.X = solidBox.Right - collisionBox.Left;
			else if (clipDirection == Directions.Right)
				nextPos.X = solidBox.Left - collisionBox.Right;
			else if (clipDirection == Directions.Up)
				nextPos.Y = solidBox.Bottom - collisionBox.Top;
			else if (clipDirection == Directions.Down)
				nextPos.Y = solidBox.Top - collisionBox.Bottom;

			// Set the collision info.
			int penetrationAxis = Directions.ToAxis(clipDirection);
			float penetrationDistance = Math.Abs(nextPos[penetrationAxis] - prevPos[penetrationAxis]);
			CollisionInfoNew collisionInfo = new CollisionInfoNew();
			collisionInfo.PenetrationDistance	= penetrationDistance;
			collisionInfo.PenetrationDirection	= clipDirection;
			collisionInfo.IsColliding			= true;
			collisionInfo.CollidedObject		= other;
			collisionInfo.CollisionBox			= solidBox;
			if (entity.Physics.AllowEdgeClipping)
				collisionInfo.MaxAllowedPenetrationDistance = entity.Physics.EdgeClipAmount;
			return collisionInfo;
		}

		private void ResolveMovementCollisions(Entity entity) {
			Rectangle2F tileCheckArea = Rectangle2F.Union(
				Rectangle2F.Translate(entity.Physics.CollisionBox, entity.Position),
				Rectangle2F.Translate(entity.Physics.CollisionBox, entity.Position + entity.Physics.Velocity));
			
			for (int axis = 0; axis < 2; axis++) {
				foreach (CollisionCheck check in GetCollisions(entity, tileCheckArea))
					CheckCollision(entity, axis, check.CollisionObject, check.CollisionBox);
			}
			/*
			// Collide with hard tiles.
			for (int axis = 0; axis < 2; axis++) {
				foreach (Tile tile in RoomControl.TileManager.GetTilesTouching(tileCheckArea)) {
					CheckCollision(entity, axis, tile);
				}
			}*/
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
		private bool IsSafeClipping(Entity entity, int axis, Rectangle2F entityCollisionBox, object other, Rectangle2F tileCollisionBox) {
			if (entity.Physics.AllowEdgeClipping) {
				float penetration = Math.Min(
					tileCollisionBox.BottomRight[axis] - entityCollisionBox.TopLeft[axis],
					entityCollisionBox.BottomRight[axis] - tileCollisionBox.TopLeft[axis]);
				return (penetration <= entity.Physics.EdgeClipAmount);
			}
			return false;
		}

		private void CheckCollision(Entity entity, int axis, object other, Rectangle2F tileCollisionBox) {
			// TODO: Collision dodging.
			
			int collideDirection = -1;
			Rectangle2F entityCollisionBox;
			
			// FIXME: Can collide with edges of tiles when clipping.
			// FIXME: Glitchy snapping when pushing down on moving tile near sign above start room.

			/*if ((entity.Physics.CollisionInfoNew[(axis + 1) % 4].IsColliding && !entity.Physics.CollisionInfoNew[(axis + 1) % 4].IsResolved) ||
				(entity.Physics.CollisionInfoNew[(axis + 3) % 4].IsColliding && !entity.Physics.CollisionInfoNew[(axis + 3) % 4].IsResolved))
				return;*/

			if (axis == Axes.X) {
				entityCollisionBox = Rectangle2F.Translate(
					entity.Physics.CollisionBox, entity.X + entity.Physics.Velocity.X, entity.Y);
			}
			else {
				entityCollisionBox = Rectangle2F.Translate(
					entity.Physics.CollisionBox, entity.Position + entity.Physics.Velocity);
			}
			
			if (!entityCollisionBox.Intersects(tileCollisionBox))
				return;
			if (IsSafeClipping(entity, 1 - axis, entityCollisionBox, other, tileCollisionBox)) {
				//entity.Physics.ClippingDirections[axis] = true;
				//entity.Physics.ClippingDirections[axis + 2] = true;
				Vector2F translation = entityCollisionBox.Point - entity.Physics.PositionedCollisionBox.Point;
				entity.Position += translation;
				DetectClipCollision(entity, other, tileCollisionBox, 1 - axis);
				entity.Position -= translation;
				return;
			}

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
					entity.Physics.CollisionInfo[collideDirection].SetCollision(other, collideDirection);
					
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
