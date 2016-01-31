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
				collision.Entity = entity;
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

			// Clear the collision state.
			entity.Physics.IsColliding = false;
			for (int i = 0; i < Directions.Count; i++) {
				entity.Physics.CollisionInfo[i].Clear();
				entity.Physics.MovementCollisions[i] = false;
			}

			// Perform world collisions.
			if (entity.Physics.CollideWithWorld) {
				// 1. Resolve unresolved collisions from the previous frame.
				ResolvePreviousClipCollisions(entity);
				// 2. Detect collisions.
				DetectClipCollisions(entity);
				// 3. Resolve collisions.
				ResolveClipCollisions(entity);
				// 4. Detect any new unresolved collisions.
				DetectClipCollisions(entity);
				// 5. Clip velocity for all detected collisions.
				ClipVelocity(entity);
				// 6. Check if the entity is being crushed.
				CheckCrush(entity);
				// 7. Detect and resolve collisions with movement.
				ResolveMovementCollisions(entity);

				// Set the entity's collision info.
				for (int i = 0; i < Directions.Count; i++) {
					CollisionInfoNew clipCollision = entity.Physics.CollisionInfoNew[i];

					if (clipCollision.IsCollidingAndNotAllowedClipping) {
						if (!entity.Physics.CollisionInfo[i].IsColliding)
							entity.Physics.CollisionInfo[i].SetCollision(clipCollision.CollidedObject, i);
					}
					else if (!entity.Physics.MovementCollisions[i])
						entity.Physics.CollisionInfo[i].Clear();

					if (entity.Physics.CollisionInfo[i].IsColliding)
						entity.Physics.IsColliding = true;
				}
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
		// Clip Collision Detection
		//-----------------------------------------------------------------------------
		
		private struct CollisionCheck {
			public object SolidObject { get; set; }
			public Rectangle2F SolidBox { get; set; }
		}

		// Returns an enumerable list of all possible collisions.
		private IEnumerable<CollisionCheck> GetCollisions(Entity entity, Rectangle2F area) {
			// Find nearby solid entities.
			if (entity.Physics.CollideWithEntities) {
				foreach (Entity other in RoomControl.Entities) {
					if (other != entity && other.Physics.IsEnabled && other.Physics.IsSolid) {
						yield return new CollisionCheck() {
							SolidBox = other.Physics.PositionedCollisionBox,
							SolidObject = other
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
								SolidBox = tileBox,
								SolidObject = tile
							};
						}
					}
				}
			}
		}
		
		// Detect clip collisions for an entity, optionally restricting clip direction to a given axis.
		private void DetectClipCollisions(Entity entity) {
			Rectangle2F checkArea = entity.Physics.PositionedCollisionBox;
			foreach (CollisionCheck check in GetCollisions(entity, checkArea))
				DetectClipCollision(entity, check.SolidObject, check.SolidBox);
			
			// Determine which collisions can be resolved later.
			for (int i = 0; i < Directions.Count; i++) {
				CollisionInfoNew collision = entity.Physics.CollisionInfoNew[i];
				if (collision.IsColliding) {
					collision.IsResolvable = CanResolveCollision(entity, collision);
					if (!collision.IsAllowedClipping)
						entity.Physics.IsColliding = true;
				}
			}
		}
		
		// Detect a clip collision between the entity and a solid object.
		private void DetectClipCollision(Entity entity, object other, Rectangle2F solidBox) {
			// Check if there actually is a collision.
			Rectangle2F entityBox = entity.Physics.PositionedCollisionBox;
			if (entityBox.Intersects(solidBox)) {
				// Determine clip direction.
				int clipDirection = GetCollisionClipDirection(entity, other, solidBox);
			
				// Set or replace the collision info for this clip direction.
				if (clipDirection >= 0) {
					CollisionInfoNew oldCollisionInfo = entity.Physics.CollisionInfoNew[clipDirection];
					CollisionInfoNew newCollisionInfo = CreateCollisionInfo(entity, other, solidBox, clipDirection);
					if (!oldCollisionInfo.IsColliding || (newCollisionInfo.PenetrationDistance > oldCollisionInfo.PenetrationDistance))
						entity.Physics.CollisionInfoNew[clipDirection] = newCollisionInfo;
				}
			}
		}

		// Determine the clipping direction for a collision.
		private int GetCollisionClipDirection(Entity entity, object other, Rectangle2F solidBox) {
			Rectangle2F entityBox = entity.Physics.PositionedCollisionBox;
			
			// For moving tiles, use the move direction to determine clip direction.
			Tile tile = other as Tile;
			if (tile != null && tile.IsMoving) {
				if ((solidBox.Center - entityBox.Center).Dot(Directions.ToVector(tile.MoveDirection)) < 0.0f)
					return Directions.Reverse(tile.MoveDirection);
			}

			// Get the nearest direction from the collision intersection to the center of the solid box.
			Vector2F intersectionCenter = Rectangle2F.Intersect(entityBox, solidBox).Center;
			int clipDirection = Directions.NearestFromVector(solidBox.Center - intersectionCenter);
			
			// If the collision can't be resolved, then try to use a direction on the opposite axis.
			CollisionInfoNew testCollision = CreateCollisionInfo(entity, other, solidBox, clipDirection);
			if (!testCollision.IsAllowedClipping && !CanResolveCollision(entity, testCollision)) {
				int newClipDirection;
				int a = 1 - Directions.ToAxis(clipDirection);
				if (entityBox.Center[a] < solidBox.Center[a])
					newClipDirection = (a == Axes.X ? Directions.Right : Directions.Down);
				else
					newClipDirection = (a == Axes.X ? Directions.Left : Directions.Up);
				testCollision = CreateCollisionInfo(entity, other, solidBox, newClipDirection);
				if (testCollision.IsAllowedClipping || CanResolveCollision(entity, testCollision))
					clipDirection = newClipDirection;
			}
			
			return clipDirection;
		}
		
		// Create a clip collision info between an entity and solid object with a specified clip direction.
		private CollisionInfoNew CreateCollisionInfo(Entity entity, object other, Rectangle2F solidBox, int clipDirection) {
			return new CollisionInfoNew() {
				IsColliding						= true,
				Entity							= entity,
				CollidedObject					= other,
				CollisionBox					= solidBox,
				PenetrationDirection			= clipDirection,
				PenetrationDistance				= GetClipPenetration(entity.Physics.PositionedCollisionBox, solidBox, clipDirection),
				MaxAllowedPenetrationDistance	= GetAllowedEdgeClipAmount(entity, other)
			};
		}
		
		// Returns the allowable edge clip amount between an entity and solid object.
		private float GetAllowedEdgeClipAmount(Entity entity, object solidObject) {
			if (!entity.Physics.AllowEdgeClipping)
				return 0.0f;
			if ((solidObject is Tile) && ((Tile) solidObject).IsInMotion)
				return 0.0f;
			return entity.Physics.EdgeClipAmount;
		}

		
		//-----------------------------------------------------------------------------
		// Clip Collision Resolution
		//-----------------------------------------------------------------------------

		// Resolve clip collisions that were not resolved from the previous frame.
		private void ResolvePreviousClipCollisions(Entity entity) {
			for (int i = 0; i < Directions.Count; i++) {
				CollisionInfoNew collision = entity.Physics.CollisionInfoNew[i];

				if (!collision.IsResolved && CanResolveCollision(entity, collision)) {
					float resolveDistance = Math.Max(0.0f, collision.PenetrationDistance - collision.MaxAllowedPenetrationDistance);
					entity.Position -= Directions.ToVector(collision.PenetrationDirection) * resolveDistance;
				}
				collision.Reset();
				collision.Entity = entity;
				collision.PenetrationDirection = i;
			}
		}
		
		// Resolve clip collisions.
		private void ResolveClipCollisions(Entity entity) {
			// Determine which collisions have priority.
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
					entity.Position += GetPositionalCorrection(collision);
					collision.IsResolved = true;

					// Add to the penetration distance of the opposite collision.
					CollisionInfoNew otherCollision = entity.Physics.CollisionInfoNew[Directions.Reverse(collision.PenetrationDirection)];
					float resolveDistance = Math.Max(0.0f, collision.PenetrationDistance - collision.MaxAllowedPenetrationDistance);
					otherCollision.PenetrationDistance += resolveDistance;
				}
			}
			
			// Check if the collisions can be only resolved together and NOT separately.
			if (collisions[0] != null && collisions[1] != null &&
				!collisions[0].IsResolvable && !collisions[1].IsResolvable &&
				CanResolveCollisionPair(entity, collisions[0], collisions[1]))
			{
				for (int i = 0; i < 2; i++) {
					CollisionInfoNew collision = collisions[i];
					// Resolve the collision.
					entity.Position += GetPositionalCorrection(collision);
					collision.IsResolved = true;

					// Add to the penetration distance of the opposite collision.
					CollisionInfoNew otherCollision = entity.Physics.CollisionInfoNew[Directions.Reverse(collision.PenetrationDirection)];
					float resolveDistance = Math.Max(0.0f, collision.PenetrationDistance - collision.MaxAllowedPenetrationDistance);
					otherCollision.PenetrationDistance += resolveDistance;
				}
			}
		}
		
		// Returns the clip collision that has resolution priority (or null if neither are colliding).
		private CollisionInfoNew GetPriorityCollision(Entity entity, CollisionInfoNew collision1, CollisionInfoNew collision2) {
			if (!collision1.IsCollidingAndNotAllowedClipping && !collision2.IsCollidingAndNotAllowedClipping)
				return null;
			else if (collision1.IsCollidingAndNotAllowedClipping && !collision2.IsCollidingAndNotAllowedClipping)
				return collision1;
			else if (collision2.IsCollidingAndNotAllowedClipping && !collision1.IsCollidingAndNotAllowedClipping)
				return collision2;

			if (collision1.IsResolvable && !collision2.IsResolvable)
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
		
		// Get the positional correction needed to resolve a collision.
		private Vector2F GetPositionalCorrection(CollisionInfoNew collision) {
			float resolveDistance = Math.Max(0.0f, collision.PenetrationDistance - collision.MaxAllowedPenetrationDistance);
			return -Directions.ToVector(collision.PenetrationDirection) * resolveDistance;
		}
		
		// Returns true if the given collision needs resolution.
		private bool CanResolveCollision(Entity entity, CollisionInfoNew collision) {
			if (!collision.IsColliding || collision.IsAllowedClipping)
				return false;
			
			// Static collisions cannot resolve into colliding with other static objects.
			Tile tile = collision.CollidedObject as Tile;
			if (tile != null && !tile.IsInMotion) {
				Vector2F newPosition = entity.Position + GetPositionalCorrection(collision);
				if (IsCollidingAt(entity, newPosition, true))
					return false;
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
				newPosition += GetPositionalCorrection(collision1);
				newPosition += GetPositionalCorrection(collision2);
				if (IsCollidingAt(entity, newPosition, true))
					return false;
			}
			return true;
		}

		// Returns true if the entity would be clipping if it were placed at the given position.
		private bool IsCollidingAt(Entity entity, Vector2F position, bool onlyStatic) {
			Rectangle2F entityBox = entity.Physics.CollisionBox;
			entityBox.Point += position;
			if (entity.Physics.CollideWithRoomEdge && !((Rectangle2F) roomControl.RoomBounds).Contains(entityBox))
				return true;
			foreach (CollisionCheck check in GetCollisions(entity, entityBox)) {
				if (onlyStatic && (check.SolidObject is Tile) && ((Tile) check.SolidObject).IsInMotion)
					continue;
				float allowedClipAmount = GetAllowedEdgeClipAmount(entity, check.SolidObject);
				Rectangle2F insetSolidBox = check.SolidBox.Inflated(-allowedClipAmount, -allowedClipAmount);
				if (entityBox.Intersects(insetSolidBox))
					return true;
			}
			return false;
		}

				
		//-----------------------------------------------------------------------------
		// Post-Clip Collision Stage
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
				if (collision.IsCollidingAndNotAllowedClipping)
					ClipEntityVelocity(entity, collision.PenetrationDirection);
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

		// Check if the entity is being crushed.
		private void CheckCrush(Entity entity) {
			if (!entity.Physics.IsCrushable)
				return;

			// Check for crushing along each axis.
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

					// Check if the gap between the solid objects is small enough to crush.
					if (penetration > 0.0f) {
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
		
		// Resolve any movement collisions caused by an entity's velocity.
		private void ResolveMovementCollisions(Entity entity) {
			Rectangle2F checkArea = Rectangle2F.Union(
				Rectangle2F.Translate(entity.Physics.CollisionBox, entity.Position),
				Rectangle2F.Translate(entity.Physics.CollisionBox, entity.Position + entity.Physics.Velocity));
			
			for (int axis = 0; axis < 2; axis++) {
				foreach (CollisionCheck check in GetCollisions(entity, checkArea))
					ResolveMovementCollision(entity, axis, check.SolidObject, check.SolidBox);
			}
		}
		
		// Resolve any movement collision between the entity and a solid object
		private void ResolveMovementCollision(Entity entity, int axis, object other, Rectangle2F solidBox) {
			
			// TODO: Collision dodging.

			// Setup the entity's translated collision box based on which axis we're checking.
			Rectangle2F entityBox;
			if (axis == Axes.X) {
				entityBox = Rectangle2F.Translate(
					entity.Physics.CollisionBox, entity.X + entity.Physics.Velocity.X, entity.Y);
			}
			else {
				entityBox = Rectangle2F.Translate(
					entity.Physics.CollisionBox, entity.Position + entity.Physics.Velocity);
			}
			
			// Check if there actually is a collision.
			if (!entityBox.Intersects(solidBox))
				return;

			// Determine clipping direction.
			int clipDirection = -1;
			if (axis == Axes.X) {
				if (entityBox.Center.X < solidBox.Center.X)
					clipDirection = Directions.Right;
				else
					clipDirection = Directions.Left;
			}
			else {
				if (entityBox.Center.Y < solidBox.Center.Y)
					clipDirection = Directions.Down;
				else
					clipDirection = Directions.Up;
			}

			// Ignore collision if the entity is already colliding in the opposite
			// direction or if the entity is not moving in the clip direction.
			if (entity.Physics.CollisionInfoNew[Directions.Reverse(clipDirection)].IsCollidingAndNotAllowedClipping ||
				entity.Physics.Velocity.Dot(Directions.ToVector(clipDirection)) <= 0.0f)
				return;
			
			// Ignore collisions that are within the allowed clipping range.
			if (IsSafeClipping(entity, 1 - axis, entityBox, other, solidBox))
				return;
			
			// Ignore the collision if the entity is clipped into a solid object that shares
			// a clip edge with this object and the entity is also moving parallel with that edge.
			for (int i = 0; i < 2; i++) {
				int dir = Axes.GetOpposite(axis) + (i * 2);
				CollisionInfoNew checkCollision = entity.Physics.CollisionInfoNew[dir];
				if (checkCollision.IsColliding && AreEdgesAligned(
					checkCollision.CollisionBox, solidBox, Directions.Reverse(dir)))
				{
					return;
				}
			}
			
			// Resolve the collision.
			if (!entity.Physics.IsColliding && !entity.Physics.CollisionInfoNew[clipDirection].IsColliding) {
				// Determine the penetration.
				float penetrationDistance = GetClipPenetration(entityBox, solidBox, clipDirection);
				float maxAllowedPenetration = 0.0f;
				if (entity.Physics.Velocity[axis] == 0.0f)
					maxAllowedPenetration = GetAllowedEdgeClipAmount(entity, other);
					
				// Snap the entity's position to the edge of the solid object.
				Vector2F newPos = entity.Position;
				newPos[axis] += entity.Physics.Velocity[axis];
				newPos -= Directions.ToVector(clipDirection) * penetrationDistance;
				if (penetrationDistance <= maxAllowedPenetration)
					newPos += Directions.ToVector(clipDirection) * penetrationDistance;
				entity.Position = newPos;

				entity.Physics.CollisionInfo[clipDirection].SetCollision(other, clipDirection);
			}
			
			// Zero the entity's velocity for this axis.
			Vector2F velocity = entity.Physics.Velocity;
			velocity[axis] = 0.0f;
			entity.Physics.Velocity = velocity;
			
			entity.Physics.MovementCollisions[clipDirection] = true;
		}
		
		// Returns true if the entity allowably clipping the given collision.
		private bool IsSafeClipping(Entity entity, int axis, Rectangle2F entityBox, object other, Rectangle2F solidBox) {
			if (entity.Physics.AllowEdgeClipping) {
				float allowedEdgeClipAmount = GetAllowedEdgeClipAmount(entity, other);
				float penetration = Math.Min(
					solidBox.BottomRight[axis] - entityBox.TopLeft[axis],
					entityBox.BottomRight[axis] - solidBox.TopLeft[axis]);
				return (penetration <= allowedEdgeClipAmount);
			}
			return false;
		}

		// Calculate a collision's penetration distance in the given direction.
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
		
		// Returns true if two rectangles share an edge in the given direction.
		private bool AreEdgesAligned(Rectangle2F box1, Rectangle2F box2, int edgeDirection) {
			return Math.Abs(box1.GetEdge(edgeDirection) - box2.GetEdge(edgeDirection)) < 0.1f;
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
