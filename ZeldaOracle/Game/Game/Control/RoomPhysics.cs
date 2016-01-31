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

		
		//-----------------------------------------------------------------------------
		// Clip Collision Resolution
		//-----------------------------------------------------------------------------

		// Restrict an entity's velocity based on its clip collisions.
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
					entity.Physics.IsColliding = true;
					entity.Physics.CollisionInfo[i].SetCollision(collision.CollidedObject, i);
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


		//-----------------------------------------------------------------------------
		// Clip Collision Detection
		//-----------------------------------------------------------------------------
		
		// Detect clip collisions for an entity, optionally restricting clip direction to a given axis.
		private void DetectClipCollisions(Entity entity, int clipAxis = -1) {
			Rectangle2F tileCheckArea = entity.Physics.PositionedCollisionBox;
			foreach (CollisionCheck check in GetCollisions(entity, tileCheckArea))
				DetectClipCollision(entity, check.CollisionObject, check.CollisionBox, clipAxis);
			foreach (CollisionInfoNew collision in entity.Physics.CollisionInfoNew)
				collision.IsResolvable = CanResolveCollision(entity, collision);
		}
		
		// Determine the clipping direction for a collision.
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
				int newClipDirection;
				int a = 1 - Directions.ToAxis(clipDirection);
				if (entityBox.Center[a] < solidBox.Center[a])
					newClipDirection = (a == Axes.X ? Directions.Right : Directions.Down);
				else
					newClipDirection = (a == Axes.X ? Directions.Left : Directions.Up);
				testCollision = CreateCollisionInfo(entity, other, solidBox, newClipDirection);
				if (CanResolveCollision(entity, testCollision))
					clipDirection = newClipDirection;
			}
			
			return clipDirection;
		}
		
		// Detect a clip collision between the entity and a solid object, optionally restricting clip direction to a given axis.
		private void DetectClipCollision(Entity entity, object other, Rectangle2F solidBox, int clipAxis = -1) {
			Rectangle2F entityBox = entity.Physics.PositionedCollisionBox;
			if (!entityBox.Intersects(solidBox))
				return;
			
			// Determine clip direction.
			int clipDirection = GetCollisionPenetrationDirection(entity, other, solidBox, clipAxis);
			
			// Perform collision resolution.
			if (clipDirection >= 0) {
				/*if (entity.Physics.ClippingDirections[(clipDirection + 1) % 4] ||
					entity.Physics.ClippingDirections[(clipDirection + 3) % 4])
					return;*/
				/*if (entity.Physics.CollisionInfoNew[(clipDirection + 1) % 4].IsAllowedClipping ||
					entity.Physics.CollisionInfoNew[(clipDirection + 3) % 4].IsAllowedClipping)
					return;*/

				// Set the collision info.
				float penetrationDistance = GetClipPenetration(entityBox, solidBox, clipDirection);
				CollisionInfoNew collisionInfo = entity.Physics.CollisionInfoNew[clipDirection];

				CollisionInfoNew newCollisionInfo = new CollisionInfoNew();
				newCollisionInfo.PenetrationDistance	= penetrationDistance;
				newCollisionInfo.PenetrationDirection	= clipDirection;
				newCollisionInfo.IsColliding			= true;
				newCollisionInfo.CollidedObject			= other;
				newCollisionInfo.CollisionBox			= solidBox;
				if (entity.Physics.AllowEdgeClipping)
					newCollisionInfo.MaxAllowedPenetrationDistance = entity.Physics.EdgeClipAmount;

				if (!collisionInfo.IsColliding || (newCollisionInfo.PenetrationDistance > collisionInfo.PenetrationDistance))
					//|| (CanResolveCollision(entity, newCollisionInfo) && !CanResolveCollision(entity, collisionInfo)))
				{
					/*
					collisionInfo.PenetrationDistance	= penetrationDistance;
					collisionInfo.IsColliding			= true;
					collisionInfo.CollidedObject		= other;
					collisionInfo.CollisionBox			= solidBox;
					if (entity.Physics.AllowEdgeClipping)
						collisionInfo.MaxAllowedPenetrationDistance = entity.Physics.EdgeClipAmount;
					*/
					entity.Physics.CollisionInfoNew[clipDirection] = newCollisionInfo;
					entity.Physics.ClippingDirections[clipDirection] = collisionInfo.IsAllowedClipping;
				}
			}
		}

		// Create a clip collision info between an entity and solid object with a specified clip direction.
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
		
		
		//-----------------------------------------------------------------------------
		// Clip Collision Resolution
		//-----------------------------------------------------------------------------

		// Resolve clip collisions that were not resolved from the previous frame.
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
		
		// Resolve clip collisions.
		private void ApplyPositionalCorrection(Entity entity) {
			CollisionInfoNew[] collisions = new CollisionInfoNew[2];
			collisions[Axes.X] = GetPriorityCollision(entity,
				entity.Physics.CollisionInfoNew[Directions.Right],
				entity.Physics.CollisionInfoNew[Directions.Left]);
			collisions[Axes.Y] = GetPriorityCollision(entity,
				entity.Physics.CollisionInfoNew[Directions.Up],
				entity.Physics.CollisionInfoNew[Directions.Down]);


			for (int i = 0; i < 2; i++) {
				CollisionInfoNew collision = collisions[i];
				if (collision != null && collision.IsResolvable) {
					// Resolve the collision.
					float resolveDistance = Math.Max(0.0f, collision.PenetrationDistance - collision.MaxAllowedPenetrationDistance);
					Vector2F newPosition = entity.Position - Directions.ToVector(collision.PenetrationDirection) * resolveDistance;
					entity.Position = newPosition;
					collision.IsResolved = true;

					// Add to the penetration distance of the opposite collision.
					CollisionInfoNew otherCollision = entity.Physics.CollisionInfoNew[Directions.Reverse(collision.PenetrationDirection)];
					otherCollision.PenetrationDistance += resolveDistance;
				}
			}
			
			// Check if the collisions can be resolved together, not separately.
			if (collisions[0] != null && collisions[1] != null &&
				!collisions[0].IsResolvable && !collisions[1].IsResolvable &&
				CanResolveCollisionPair(entity, collisions[0], collisions[1]))
			{
				for (int i = 0; i < 2; i++) {
					CollisionInfoNew collision = collisions[i];
					// Resolve the collision.
					float resolveDistance = Math.Max(0.0f, collision.PenetrationDistance - collision.MaxAllowedPenetrationDistance);
					Vector2F newPosition = entity.Position - Directions.ToVector(collision.PenetrationDirection) * resolveDistance;
					entity.Position = newPosition;
					collision.IsResolved = true;

					// Add to the penetration distance of the opposite collision.
					CollisionInfoNew otherCollision = entity.Physics.CollisionInfoNew[Directions.Reverse(collision.PenetrationDirection)];
					otherCollision.PenetrationDistance += resolveDistance;
				}
			}

			/*
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
			*/
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

		// Returns true if the given clip collision needs resolution.
		private bool CanResolveCollisionPair(Entity entity, CollisionInfoNew collision1, CollisionInfoNew collision2) {
			
			// Static collisions cannot resolve into colliding with other static objects.
			Tile tile1 = collision1.CollidedObject as Tile;
			Tile tile2 = collision2.CollidedObject as Tile;
			if ((tile1 != null && !tile1.IsInMotion) || (tile2 != null && !tile2.IsInMotion)) {
				Vector2F newPosition = entity.Position;
				float resolveDistance;

				resolveDistance = Math.Max(0.0f, collision1.PenetrationDistance - collision1.MaxAllowedPenetrationDistance);
				newPosition -= Directions.ToVector(collision1.PenetrationDirection) * resolveDistance;
				resolveDistance = Math.Max(0.0f, collision2.PenetrationDistance - collision2.MaxAllowedPenetrationDistance);
				newPosition -= Directions.ToVector(collision2.PenetrationDirection) * resolveDistance;

				if (IsCollidingAt(entity, newPosition, true)) {
					return false;
				}
			}

			return true;
		}

		// Returns the clip collision that has resolution priority (or null if neither are colliding).
		private CollisionInfoNew GetPriorityCollision(Entity entity, CollisionInfoNew collision1, CollisionInfoNew collision2) {
			if (!collision1.IsColliding && !collision2.IsColliding)
				return null;
			else if (collision1.IsColliding && !collision2.IsColliding)
				return collision1;
			else if (collision2.IsColliding && !collision1.IsColliding)
				return collision2;

			/*if (!collision1.IsResolvable && !collision2.IsResolvable)
				return null;
			else */if (collision1.IsResolvable && !collision2.IsResolvable)
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

		// Returns true if the entity would be clipping if it were placed at the given position.
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

		// Returns an enumerable list of clip collision checks.
		private IEnumerable<CollisionCheck> GetCollisions(Entity entity, Rectangle2F area) {
			// Find nearby solid entities.
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
			// Find nearby solid tiles tiles.
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
		
				
		//-----------------------------------------------------------------------------
		// Other Collision Checks
		//-----------------------------------------------------------------------------
				
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
		

		//-----------------------------------------------------------------------------
		// Movement Collisions
		//-----------------------------------------------------------------------------
		
		// Returns true if the entity allowably clipping the given collision.
		private bool IsSafeClipping(Entity entity, int axis, Rectangle2F entityCollisionBox, object other, Rectangle2F solidBox) {
			if (entity.Physics.AllowEdgeClipping) {
				float penetration = Math.Min(
					solidBox.BottomRight[axis] - entityCollisionBox.TopLeft[axis],
					entityCollisionBox.BottomRight[axis] - solidBox.TopLeft[axis]);
				return (penetration <= entity.Physics.EdgeClipAmount);
			}
			return false;
		}

		// Resolve any movement collisions caused by an entity's velocity.
		private void ResolveMovementCollisions(Entity entity) {
			Rectangle2F checkArea = Rectangle2F.Union(
				Rectangle2F.Translate(entity.Physics.CollisionBox, entity.Position),
				Rectangle2F.Translate(entity.Physics.CollisionBox, entity.Position + entity.Physics.Velocity));
			
			for (int axis = 0; axis < 2; axis++) {
				foreach (CollisionCheck check in GetCollisions(entity, checkArea))
					CheckCollision(entity, axis, check.CollisionObject, check.CollisionBox);
			}
		}
		
		// Resolve any movement collision between the entity and a solid object
		private void CheckCollision(Entity entity, int axis, object other, Rectangle2F solidBox) {
			
			// TODO: Collision dodging.
			// FIXME: Can collide with edges of tiles when clipping.
			// FIXME: Glitchy snapping when pushing down on moving tile near sign above start room.

			/*if ((entity.Physics.CollisionInfoNew[(axis + 1) % 4].IsColliding && !entity.Physics.CollisionInfoNew[(axis + 1) % 4].IsResolved) ||
				(entity.Physics.CollisionInfoNew[(axis + 3) % 4].IsColliding && !entity.Physics.CollisionInfoNew[(axis + 3) % 4].IsResolved))
				return;*/
			
			Rectangle2F entityBox;
			if (axis == Axes.X) {
				entityBox = Rectangle2F.Translate(
					entity.Physics.CollisionBox, entity.X + entity.Physics.Velocity.X, entity.Y);
			}
			else {
				entityBox = Rectangle2F.Translate(
					entity.Physics.CollisionBox, entity.Position + entity.Physics.Velocity);
			}
			
			if (!entityBox.Intersects(solidBox))
				return;
			if (IsSafeClipping(entity, 1 - axis, entityBox, other, solidBox)) {
				//entity.Physics.ClippingDirections[axis] = true;
				//entity.Physics.ClippingDirections[axis + 2] = true;
				Vector2F translation = entityBox.Point - entity.Physics.PositionedCollisionBox.Point;
				entity.Position += translation;
				DetectClipCollision(entity, other, solidBox, 1 - axis);
				entity.Position -= translation;
				return;
			}

			// Determine clipping direction.
			int clipDirection = -1;
			if (axis == Axes.X) {
				if (entityBox.Center.X < solidBox.Center.X) {
					if (entity.Physics.VelocityX > 0.0f && !entity.Physics.CollisionInfoNew[Directions.Left].IsColliding)
						clipDirection = Directions.Right;
				}
				else {
					if (entity.Physics.VelocityX < 0.0f && !entity.Physics.CollisionInfoNew[Directions.Right].IsColliding)
						clipDirection = Directions.Left;
				}
			}
			else {
				if (entityBox.Center.Y < solidBox.Center.Y) {
					if (entity.Physics.VelocityY > 0.0f && !entity.Physics.CollisionInfoNew[Directions.Up].IsColliding)
						clipDirection = Directions.Down;
				}
				else {
					if (entity.Physics.VelocityY < 0.0f && !entity.Physics.CollisionInfoNew[Directions.Down].IsColliding)
						clipDirection = Directions.Up;
				}
			}

			/*if (entity.Physics.Velocity.Dot(Directions.ToVector(clipDirection)) <= 0.0f)
				clipDirection = -1;*/
			
			// Ignore the collision if the entity is clipped into a solid object that shares
			// a clip edge with this object and the entity is also moving parallel with that edge.
			if (clipDirection >= 0) {
				for (int i = 0; i < 2; i++) {
					int dir = Axes.GetOpposite(axis) + (i * 2);
					CollisionInfoNew checkCollision = entity.Physics.CollisionInfoNew[dir];
					if (checkCollision.IsColliding && Math.Abs(checkCollision.CollisionBox.GetEdge(Directions.Reverse(dir)) -
						solidBox.GetEdge(Directions.Reverse(dir))) < 0.1f)
					{
						clipDirection = -1;
						break;
					}
				}
			}

			// Resolve collision.
			if (clipDirection >= 0) {
				// Resolve the collision.
				//if (!entity.Physics.CollisionInfoNew[clipDirection].IsColliding) {
				if (!entity.Physics.IsColliding) {
					// Determine the penetration.
					float penetrationDistance = GetClipPenetration(entityBox, solidBox, clipDirection);
					float maxAllowedPenetration = 0.0f;
					if (entity.Physics.Velocity[axis] == 0.0f && entity.Physics.AllowEdgeClipping)
						maxAllowedPenetration = entity.Physics.EdgeClipAmount;
					
					// Snap the entity's position to the edge of the solid object.
					Vector2F newPos = entity.Position;
					newPos[axis] += entity.Physics.Velocity[axis];
					newPos -= Directions.ToVector(clipDirection) * penetrationDistance;
					if (penetrationDistance <= maxAllowedPenetration)
						newPos += Directions.ToVector(clipDirection) * penetrationDistance;
					entity.Position = newPos;
					
					entity.Physics.IsColliding = true;
					entity.Physics.CollisionInfo[clipDirection].SetCollision(other, clipDirection);
				}

				// Zero the entity's velocity for this axis.
				Vector2F velocity = entity.Physics.Velocity;
				velocity[axis] = 0.0f;
				entity.Physics.Velocity = velocity;
			}
		}

		public float GetClipPenetration(Rectangle2F entityBox, Rectangle2F solidBox, int clipDirection) {
			if (clipDirection == Directions.Right)
				return entityBox.Right - solidBox.Left;
			else if (clipDirection == Directions.Up)
				return solidBox.Bottom - entityBox.Top;
			else if (clipDirection == Directions.Left)
				return solidBox.Right - entityBox.Left;
			else if (clipDirection == Directions.Down)
				return entityBox.Bottom - solidBox.Top;
			return 0.0f;
		}
		
		
		//-----------------------------------------------------------------------------
		// Z-Dynamics & Misc.
		//-----------------------------------------------------------------------------
		
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
	}
}
