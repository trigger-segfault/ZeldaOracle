using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Entities.Collisions;
using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.Main;
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

		/// <summary>Process all entity physics for the room</summary>
		public void ProcessPhysics() {
			// Process physics for all entities (except the player)
			for (int i = 0; i < roomControl.Entities.Count; i++) {
				Entity entity = roomControl.Entities[i];
				if (entity != RoomControl.Player && entity.Physics != null &&
					entity.Physics.IsEnabled)
				{
					ProcessEntityPhysics(entity);
				}
			}
			
			// Process physics for the player last
			if (roomControl.Player.Physics != null &&
				roomControl.Player.Physics.IsEnabled)
			{
				ProcessEntityPhysics(roomControl.Player);
			}
		}
		
		/// <summary>Process physics for a single entity</summary>
		public void ProcessEntityPhysics(Entity entity) {
			bool startedOnGround = entity.IsOnGround;

			// Update Z dynamics
			UpdateEntityZPosition(entity);
			
			// Initialize the collision state for this frame
			InitPhysicsState(entity);

			// Check the surface tile beneath the entity
			CheckSurfaceTile(entity);
			// Resolve collisions with solid objects
			CheckSolidCollisions(entity);
			// Resolve collisions with the room edge
			//CheckRoomEdgeCollisions(entity);
			// Integrate velocity.
			//entity.Position += entity.Physics.Velocity;
			
			// Restore the entity's velocity before collision checks.
			Vector2F newVelocity = entity.Physics.PreviousVelocity;
			for (int axis = 0; axis < 2; axis++) {
				if (Math.Sign(newVelocity[axis]) > 0 && entity.Physics.Velocity[axis] <= newVelocity[axis])
					newVelocity[axis] = entity.Physics.Velocity[axis];
				else if (Math.Sign(newVelocity[axis]) < 0 && entity.Physics.Velocity[axis] >= newVelocity[axis])
					newVelocity[axis] = entity.Physics.Velocity[axis];
			}
			entity.Physics.Velocity = newVelocity;
			
			// Check for landing and falling in side-scrolling mode.
			if (IsSideScrolling) {
				if (!startedOnGround && entity.IsOnGround && entity.Physics.HasGravity)
					LandEntity(entity);
				if (startedOnGround && !entity.IsOnGround && entity.Physics.HasGravity)
					entity.OnBeginFalling();
			}

			// Change velocity if rebounded.
			CheckRebound(entity);
			
			//entity.Physics.IsColliding = false;
			//for (int i = 0; i < Directions.Count; i++) {
			//	if (entity.Physics.CollisionInfo[i].IsColliding)
			//		entity.Physics.IsColliding = true;
			//}

			// Update ledge passing.
			CheckLedges(entity);
			// Check if destroyed outside room.
			CheckOutsideRoomBounds(entity);
		}

		// Initialize an entity's physics state for the frame.
		public void InitPhysicsState(Entity entity) {
			entity.Physics.PreviousVelocity		= entity.Physics.Velocity;
			entity.Physics.PreviousZVelocity	= entity.Physics.ZVelocity;
			entity.Physics.IsColliding			= false;
			entity.Physics.AllClipCollisionInfo.Clear();
			entity.Physics.AllMovementCollisionInfo.Clear();
			entity.Physics.PotentialCollisions.Clear();

			for (int i = 0; i < Directions.Count; i++) {
				entity.Physics.PreviousCollisionInfo[i] = entity.Physics.CollisionInfo[i];
				entity.Physics.CollisionInfo[i].Clear();
				entity.Physics.MovementCollisions[i] = false;
			}
		}

		
		//-----------------------------------------------------------------------------
		// Solid Collisions
		//-----------------------------------------------------------------------------

		// Check collisions with solid objects (entities and/or tiles).
		private void CheckSolidCollisions(Entity entity) {
			if (entity.Physics.CollideWithWorld || entity.Physics.CollideWithEntities) {
				
				// Handle circular tile collisions
				if (entity.Physics.CollideWithWorld && entity.Physics.CheckRadialCollisions) {
					Rectangle2F checkArea = Rectangle2F.Union(
						Rectangle2F.Translate(entity.Physics.CollisionBox, entity.Position),
						Rectangle2F.Translate(entity.Physics.CollisionBox, entity.Position + entity.Physics.Velocity));

					foreach (Tile tile in roomControl.TileManager.GetTilesTouching(checkArea)) {
						if (CanCollideWithTile(entity, tile) && tile.CollisionStyle == CollisionStyle.Circular)
							ResolveCircularCollision(entity, tile, tile.Position, tile.CollisionModel);
					}
				}
			}

			if (entity.Physics.CollideWithWorld)
				DetectPotentialCollisions(entity);
			ResolveCollisions(entity);
			CheckCrush(entity);

				/*
				// Resolve unresolved collisions from the previous frame
				ResolvePreviousClipCollisions(entity);
				// Detect clip collisions
				DetectClipCollisions(entity);
				// Resolve clip collisions
				ResolveClipCollisions(entity);
				// Detect any new unresolved clip collisions
				//DetectClipCollisions(entity);
				// Clip velocity for all detected collisions
				ClipVelocity(entity);
				// Check if the entity is being crushed
				CheckCrush(entity);
				// Detect and resolve collisions caused by movement
				ResolveMovementCollisions(entity);
				*/
				
				// Check the player's side-scrolling ladder collisions
				//if (entity is Player)
				//	CheckPlayerLadderClimbing((Player) entity);

				// Set the entity's collision info
				/*for (int i = 0; i < Directions.Count; i++) {
					CollisionInfoNew clipCollision = entity.Physics.ClipCollisionInfo[i];

					if (clipCollision.IsCollidingAndNotAllowedClipping) {
						if (!entity.Physics.CollisionInfo[i].IsColliding)
							entity.Physics.CollisionInfo[i].SetCollision(clipCollision.CollidedObject, i);
					}
					else if (!entity.Physics.MovementCollisions[i])
						entity.Physics.CollisionInfo[i].Clear();

					if (entity.Physics.CollisionInfo[i].IsColliding)
						entity.Physics.IsColliding = true;
				}
			}*/
		}
		
		// Check if the entity should rebound off of its collisions.
		private void CheckRebound(Entity entity) {
			if (!entity.Physics.ReboundSolid && !entity.Physics.ReboundRoomEdge)
				return;

			Vector2F newEntityVelocity = entity.Physics.Velocity;

			for (int i = 0; i < Directions.Count; i++) {
				CollisionInfo collision = entity.Physics.CollisionInfo[i];

				if (collision.IsColliding &&
					((entity.Physics.ReboundSolid && (collision.Type == CollisionType.Tile || collision.Type == CollisionType.Entity)) ||
					(entity.Physics.ReboundRoomEdge && collision.Type == CollisionType.RoomEdge)) &&
					entity.Physics.PreviousVelocity.Dot(Directions.ToVector(i)) > 0.0f)
				{
					int axis = Directions.ToAxis(i);
					newEntityVelocity[axis] = -entity.Physics.PreviousVelocity[axis];
				}
			}
			entity.Physics.Velocity = newEntityVelocity;
		}
		
		
		//-----------------------------------------------------------------------------
		// Circular Collisions
		//-----------------------------------------------------------------------------
		
		private void ResolveCircularCollision(Entity entity, Tile tile, Vector2F modelPos, CollisionModel model) {
			for (int i = 0; i < model.BoxCount; i++) {
				Rectangle2F box = model.Boxes[i];
				box.Point += modelPos;
				ResolveCircularCollision(entity, tile, box);
			}
		}
		
		private void ResolveCircularCollision(Entity entity, object solidObject, Rectangle2F solidBox) {
			Rectangle2F collisionBox = entity.Physics.CollisionBox;
			Rectangle2F entityBoxPrev = entity.Physics.CollisionBox;
			entityBoxPrev.Point += entity.Position;

			// If already colliding with the object, then push away from its center.
			if (entityBoxPrev.Intersects(solidBox)) {
				Vector2F correction = (entity.Position - solidBox.Center).Normalized;
				correction = Vector2F.SnapDirectionByCount(correction, 16);
				entity.Position += 1.0f * correction;
				return;
			}

			// Predict collisions for each axis.
			for (int axis = 0; axis < 2; axis++) {
				Rectangle2F entityBox = entity.Physics.CollisionBox;
				entityBox.Point += entity.Position;
				if (axis == 0)
					entityBox.X += entity.Physics.VelocityX;
				else
					entityBox.Point += entity.Physics.Velocity;

				if (entityBox.Intersects(solidBox)) {
					// Snap the position.
					Vector2F newPos = entity.Position;
					if (entityBox.Center[axis] < solidBox.Center[axis])
						newPos[axis] = solidBox.TopLeft[axis] - collisionBox.BottomRight[axis];
					else
						newPos[axis] = solidBox.BottomRight[axis] - collisionBox.TopLeft[axis];
					entity.Position = newPos;
					
					Vector2F newVelocity = entity.Physics.Velocity;
					if (Math.Abs(entity.Physics.Velocity[1 - axis]) < 0.1f) {
						// Slightly push away from the center of the solid object.
						Vector2F distance = entity.Physics.PositionedCollisionBox.Center - solidBox.Center;
						//Vector2F correction = (entity.Position - solidBox.Center).Normalized;
						//correction = Vector2F.SnapDirectionByCount(correction, 16);
						if (GMath.Abs(distance[1 - axis]) > 1.0f) {
							//distance = GMath.Sqr(GMath.Abs(distance)) * GMath.Sign(distance);
							distance = (distance * distance) * GMath.Sign(distance);
							//newVelocity[1 - axis] += distance[1 - axis] * 0.015f;
							newPos = entity.Position;
							newPos[1 - axis] += distance[1 - axis] * 0.015f;
							if (!IsCollidingAt(entity, newPos, false))
								entity.Position = newPos;
						}
					}
					newVelocity[axis] = 0.0f;
					entity.Physics.Velocity = newVelocity;
				}
			}
		}



		//-----------------------------------------------------------------------------
		// Clip Collision Detection
		//-----------------------------------------------------------------------------

		// Returns an enumerable list of all possible collisions.
		private IEnumerable<CollisionCheck> GetCollisions(Entity entity,
			Rectangle2F area)
		{
			// Find nearby solid entities
			if (entity.Physics.CollideWithEntities) {
				foreach (Entity other in RoomControl.Entities) {
					if (other != entity && CanCollideWithEntity(entity, other)) {
						yield return CollisionCheck.CreateEntityCollision(other);
					}
				}
			}
			// Find nearby solid tiles
			if (entity.Physics.CollideWithWorld) {
				foreach (Tile tile in RoomControl.TileManager.GetTilesTouching(area)) {
					if (CanCollideWithTile(entity, tile) &&
						tile.CollisionStyle == CollisionStyle.Rectangular)
					{
						for (int i = 0; i < tile.CollisionModel.Boxes.Count; i++) {
							Rectangle2I box = tile.CollisionModel.Boxes[i];
							Rectangle2F tileBox = box;
							tileBox.Point += tile.Position;
							yield return CollisionCheck.CreateTileCollision(tile, i);
						}
					}
				}
			}
			// Find room edges
			if (entity.Physics.CollideWithRoomEdge) {
				yield return CollisionCheck.CreateRoomEdgeCollision(entity.RoomControl);
			}
		}
		
		// Returns true if the entity is able to collide with a tile.
		private bool CanCollideWithTile(Entity entity, Tile checkTile) {
			if (checkTile.CollisionModel == null)
				return false;
			if (entity.Physics.CustomTileIsSolidCondition != null && 
				entity.Physics.CustomTileIsSolidCondition(checkTile))
				return true;
			if (!checkTile.IsSolid ||
				(checkTile.IsHalfSolid && entity.Physics.PassOverHalfSolids))
				return false;
			if (checkTile.IsAnyLedge && entity.Physics.PassOverLedges) {
				if (entity.Physics.LedgeAltitude > 0 ||
					entity.Physics.LedgeTileLocation == checkTile.Location ||
					IsMovingDownLedge(entity, checkTile))
					return false;
			}
			if (entity.Physics.CustomTileIsNotSolidCondition != null)
				return entity.Physics.CustomTileIsNotSolidCondition(checkTile);
			return true;
		}
		
		/// <summary>Returns true if the entity is able to collide with another entity.
		/// </summary>
		private bool CanCollideWithEntity(Entity entity, Entity checkEntity) {
			return (checkEntity != entity && checkEntity.Physics.IsEnabled &&
				checkEntity.Physics.IsSolid);
		}

		/// <summary>Detect clip collisions for an entity, optionally restricting clip
		/// direction to a given axis.</summary>
		private void DetectClipCollisions(Entity entity) {
			Rectangle2F checkArea = entity.Physics.PositionedCollisionBox;
			foreach (CollisionCheck check in GetCollisions(entity, checkArea))
				DetectClipCollision(entity, check);
			
			// Determine which collisions can be resolved later
			for (int i = 0; i < Directions.Count; i++) {
				CollisionInfoNew collision = entity.Physics.ClipCollisionInfo[i];
				if (collision.IsColliding) {
					collision.IsResolvable = CanResolveCollision(entity, collision);
					if (!collision.IsAllowedClipping)
						entity.Physics.IsColliding = true;
				}
			}
		}







		

		//-----------------------------------------------------------------------------
		// Collision Detection
		//-----------------------------------------------------------------------------
		
		/// <summary>Find an entity's nearby collisions.</summary>
		private void DetectPotentialCollisions(Entity entity) {
			// Create a list of nearby potential collisions
			Rectangle2F checkArea = entity.Physics.PositionedCollisionBox;
			checkArea.Inflate(entity.Physics.PositionedCollisionBox.Size);
			foreach (CollisionCheck check in GetCollisions(entity, checkArea)) {
				Collision collision = InitPotentialCollision(entity, check);
				entity.Physics.PotentialCollisions.Add(collision);
			}

			// Connect adjacent static collisions
			ConnectStaticCollisions(entity);

			// Calculate the penetration direction and distance for each collision
			for (int i = 0; i < entity.Physics.PotentialCollisions.Count; i++) {
				if (!DetectPotentialCollision(entity, entity.Physics.PotentialCollisions[i]))
						entity.Physics.PotentialCollisions.RemoveAt(i--);
			}
		}
		
		/// <summary>Initialize a collision information.</summary>
		private Collision InitPotentialCollision(Entity entity,
			CollisionCheck source)
		{
			// Construct the collision object
			Collision collision = new Collision(entity, source);
			
			// Set the collision box based on the collision type
			if (source.Type == CollisionType.RoomEdge) {
				collision.CollisionBox = entity.Physics.SoftCollisionBox;
				collision.SolidBox = entity.RoomControl.RoomBounds;
			}
			else if (source.Type == CollisionType.Entity) {
				collision.CollisionBox = entity.Physics.CollisionBox;
				collision.SolidBox = source.Entity.Physics.PositionedCollisionBox;
			}
			else if (source.Type == CollisionType.Tile) {
				collision.CollisionBox = entity.Physics.CollisionBox;
				collision.SolidBox = Rectangle2F.Translate(
					source.Tile.CollisionModel.Boxes[source.CollisionBoxIndex],
					source.Tile.Position);
			}

			// Determine the allowed edge clipping amount
			collision.AllowedPenetration = GetAllowedPenetration(entity, source);

			// Determine if this collision is dynamic rather than static
			collision.IsDynamic = (source.IsEntity ||
				(source.IsTile && source.Tile.IsInMotion));

			return collision;
		}

		/// <summary>Connect adjacent static collisions.</summary>
		private void ConnectStaticCollisions(Entity entity) {
			for (int i = 0; i < entity.Physics.PotentialCollisions.Count; i++) {
				Collision a = entity.Physics.PotentialCollisions[i];
				if (a.IsDynamic)
					continue;
				for (int j = i + 1; j < entity.Physics.PotentialCollisions.Count; j++) {
					Collision b = entity.Physics.PotentialCollisions[j];
					if (!b.IsDynamic)
						ConnectCollisionPair(a, b);
				}
			}
		}

		/// <summary>Attempt to connect the edges of two static collisions.</summary>
		private void ConnectCollisionPair(Collision a, Collision b) {
			for (int direction = 0; direction < Directions.Count; direction++) {
				int lateralAxis = Axes.GetOpposite(Directions.ToAxis(direction));
				int oppositeDirection = Directions.Reverse(direction);

				// Make sure their edges are touching
				if (GMath.Abs(a.GetEdge(direction) -
					b.GetEdge(oppositeDirection)) < GameSettings.EPSILON)
				{
					// Connect B with A if A contains B
					if (a.SolidBox.BottomRight[lateralAxis] >=
							b.SolidBox.BottomRight[lateralAxis] -
							GameSettings.EPSILON &&
						a.SolidBox.TopLeft[lateralAxis] <=
							b.SolidBox.TopLeft[lateralAxis] + GameSettings.EPSILON)
					{
						if (b.Type != CollisionType.RoomEdge)
							b.Connections[direction] = true;
					}
					// Connect A with B if B contains A
					if (b.SolidBox.BottomRight[lateralAxis] >=
							a.SolidBox.BottomRight[lateralAxis] -
							GameSettings.EPSILON &&
						b.SolidBox.TopLeft[lateralAxis] <= 
							a.SolidBox.TopLeft[lateralAxis] + GameSettings.EPSILON)
					{
						if (a.Type != CollisionType.RoomEdge)
							a.Connections[oppositeDirection] = true;
					}
				}
			}
		}

		/// <summary>Calculate a collision's penetration direction and distance.
		/// </summary>
		private bool DetectPotentialCollision(Entity entity, Collision collision) {
			Rectangle2F solidBox = collision.SolidBox;
			Rectangle2F entityBox = Rectangle2F.Translate(
				collision.CollisionBox, entity.Position);
			
			// Calculate the penetration on each axis
			Vector2F penetrationBox;
			penetrationBox.X = GMath.Min(solidBox.Right - entityBox.Left,
				entityBox.Right - solidBox.Left);
			penetrationBox.Y = GMath.Min(solidBox.Bottom - entityBox.Top,
				entityBox.Bottom - solidBox.Top);
			
			// Check which possible directions we can collide in
			int[] axisDirections = new int[2] { -1, -1 };
			bool surrounded = (collision.Connections[0] &&
				collision.Connections[1] &&
				collision.Connections[2] &&
				collision.Connections[3]);
			for (int i = 0; i < 4; i++) {
				if ((!collision.Connections[(i + 2) % 4] || surrounded) &&
					collision.Source.IsInsideCollision ==
						((solidBox.Center - entityBox.Center).Dot(
						Directions.ToVector(i)) <= 0.0f))
				{
					collision.AllowableDirections[i] = true;
					axisDirections[Directions.ToAxis(i)] = i;
				}
			}
			
			// Set the collision direction on the axis of least penetration,
			int collisionAxis = -1;
			if (axisDirections[Axes.X] >= 0 && axisDirections[Axes.Y] >= 0)
				collisionAxis = (penetrationBox.X < penetrationBox.Y ? Axes.X : Axes.Y);
			else if (axisDirections[Axes.X] >= 0)
				collisionAxis = Axes.X;
			else if (axisDirections[Axes.Y] >= 0)
				collisionAxis = Axes.Y;
			else
				return false; // This should never happen
			collision.Direction = axisDirections[collisionAxis];

			// Calculate the penetration and allowable penetration
			collision.CalcPenetration();
			collision.CalcIsColliding();
			return true;
		}
				

		//-----------------------------------------------------------------------------
		// Collision Resolution
		//-----------------------------------------------------------------------------

		/// <summary>Resolve all collisions for an entity for the current frame, while
		/// integrating its velocity.</summary>
		/// <param name="entity">The entity who needs collision resolution.</param>
		private void ResolveCollisions(Entity entity) {
			// Remember the starting velocity
			Vector2F velocity = entity.Physics.Velocity;

			// Integrate velocity, one axis at a time, while resolving collisions
			for (int axis = 0; axis < 2; axis++) {
				// Integrate velocity in this axis
				Vector2F entityNewPosition = entity.Position;
				entityNewPosition[axis] += entity.Physics.Velocity[axis];
				entity.Position = entityNewPosition;

				// Adjust and recalculate collision penetrations
				AdjustCollisionPenetrations(entity, entity.Physics.Velocity[axis],
					axis == Axes.X ? Directions.Right : Directions.Down);

				// Resolve the collisions on this axis
				Collision collision = GetPriorityCollision(entity, axis);
				if (collision != null && collision.IsResolvable)
					ResolveCollision(entity, velocity, collision);
			}
		}

		/// <summary>Resolve a collision by adjusting the entity's position.</summary>
		/// <param name="entity">The entity who is colliding.</param>
		/// <param name="velocity">The entity's original velocity for the current
		/// frame.</param>
		/// <param name="collision">The collision to resolve.</param>
		private void ResolveCollision(Entity entity, Vector2F velocity,
			Collision collision)
		{
			int oppositeDirection = Directions.Reverse(collision.Direction);

			// Determine the maximum allowed penetration distance.
			// Do not allow any additional penetration due to velocity.
			float velocityPenetration = entity.Physics.Velocity.Dot(
				Directions.ToVector(collision.Direction));
			float originalPenetration = collision.Penetration - velocityPenetration;
			collision.AllowedPenetration = GMath.Clamp(collision.AllowedPenetration,
				0.0f, originalPenetration);

			// Now calculate the positional correction distance that would resolve
			// this collision
			float positionalCorrection = collision.Penetration -
				collision.AllowedPenetration;
			if (positionalCorrection <= 0.0f) {
				// Since this isn't techincally a 'collision' it shouldn't be
				// considered 'resolved'
				collision.IsResolved = false;
				return;
			}
			
			// Determine the adjusted positional correction, after considering
			// collisions that would be created by resolving this collision
			// Make sure we do not resolve into another, higher priority collision
			foreach (Collision oppositeCollision in
				entity.Physics.GetCollisionsInDirection(oppositeDirection))
			{
				if (oppositeCollision.Penetration <= 0.0f &&
					GetPriorityCollision(collision, oppositeCollision) ==
						oppositeCollision)
				{
					if (-oppositeCollision.Penetration < positionalCorrection) {
						positionalCorrection = -oppositeCollision.Penetration;
						oppositeCollision.IsResolved = true;
					}
				}
			}

			// Apply positional correction
			collision.IsResolved = true;
			entity.Position -= Directions.ToVector(collision.Direction) *
				positionalCorrection;
			entity.Physics.IsColliding = true;

			// Adjust and recalculate penetration for all collisions
			AdjustCollisionPenetrations(entity, positionalCorrection,
				Directions.Reverse(collision.Direction));
			
			// Clip velocity in the collision direction
			if (entity.Physics.Velocity.Dot(
				Directions.ToVector(collision.Direction)) > 0.0f)
			{
				if (Directions.IsHorizontal(collision.Direction))
					entity.Physics.VelocityX = 0.0f;
				else
					entity.Physics.VelocityY = 0.0f;
			}
			
			// Perform collision dodging
			bool canDodge = true;
			if (entity is Player) {
				// TODO: this won't work for when player is knocked back
				Player player = (Player) entity;
				canDodge = (player.Movement.IsMoving &&
					player.Movement.MoveDirection == collision.Direction);
			}
			if (entity.Physics.AutoDodges && canDodge) {
				if (roomControl.IsSideScrolling && collision.Axis == Axes.X && entity.IsOnGround)
				{
					// Step up onto blocks if it is a small enough distance
					//PerformSideScrollCollisionSnap(entity, clipDirection, other, solidBox);
				}
				else {
					// Auto dodging
					PerformCollisionDodge(entity, collision);
				}
			}
		}

		/// <summary>Attempt to dodge a collision. Returns true if the collision was
		/// dodged.</summary>
		private bool PerformCollisionDodge(Entity entity, Collision collision) {
			// Only dodge if moving perpendicular to the edge
			int axis = collision.Axis;
			int lateralAxis = Axes.GetOpposite(axis);
			if (GMath.Abs(entity.Physics.Velocity[lateralAxis]) > GameSettings.EPSILON)
				return false;

			// Can't dodge moving tiles
			if (collision.Tile != null && collision.Tile.IsMoving && 
				axis != Directions.ToAxis(collision.Tile.MoveDirection))
				return false;

			Rectangle2F entityBox = Rectangle2F.Translate(
				collision.CollisionBox, entity.Position);

			// Check dodging for both edges of the solid object
			for (int side = 0; side < 2; side++) {
				int dodgeDirection = lateralAxis + (side * 2);
				Vector2F dodgeVector = Directions.ToVector(dodgeDirection);
				float distanceToEdge = Math.Abs(entityBox.GetEdge(
					Directions.Reverse(dodgeDirection)) -
					collision.SolidBox.GetEdge(dodgeDirection));
				
				// Check if the distance to the edge is within dodge range
				if (distanceToEdge <= entity.Physics.AutoDodgeDistance) {
					float moveAmount = Math.Min(entity.Physics.AutoDodgeSpeed,
						distanceToEdge);
					Vector2F nextPosition = GMath.Round(entity.Position) +
						(dodgeVector * moveAmount);
					Vector2F goalPosition = entity.Position +
						Directions.ToVector(collision.Direction) * 4 +
						(dodgeVector * distanceToEdge);

					// Make sure the entity is not colliding when placed at the solid
					// object's edge
					if (!IsCollidingAtNew(entity, nextPosition, dodgeDirection) &&
						!IsCollidingAtNew(entity, goalPosition, collision.Direction)) // TODO: safe clipping
					{
						entity.Position += dodgeVector * moveAmount;
						collision.IsAutoDodged = true;

						// Adjust and recalculate penetration for all collisions
						AdjustCollisionPenetrations(entity, moveAmount, dodgeDirection);
						return true; // Dodged successfully
					}
					else {
						return false; // Dodge is obstructed
					}
				}
			}

			return false; // Not close enough to collision edge
		}

		private void AdjustCollisionPenetrations(Entity entity, float amount,
			int direction)
		{
			int axis = Directions.ToAxis(direction);
			foreach (Collision collision in entity.Physics.PotentialCollisions)
			{
				if (collision.Axis == axis) {
					// The entity moved perpendicular to this collision face, so
					// adjust the penetration distance
					if (collision.Direction == direction)
						collision.Penetration += amount;
					else
						collision.Penetration -= amount;
				}
				else {
					// Entity moved parallel to this collision face, so recalculate
					// the lateral penetration
					collision.CalcLateralPenetration();
				}

				// Recalculate whether this is colliding
				collision.CalcIsColliding();
			}
		}

		private bool IsCollidingAtNew(Entity entity, Vector2F position, int direction) {
			foreach (Collision collision in entity.Physics.PotentialCollisions) {
				if (direction >= 0 && collision.Direction != direction)
					continue;
				if (collision.IsCollidingAt(position))
					return true;
			}
			return false;
		}

		/// <summary>Get the highest-priority collision to resolve for the given axis.
		/// </summary>
		private Collision GetPriorityCollision(Entity entity, int axis) {
			Collision best = null;
			foreach (Collision collision in entity.Physics.GetCollisionsOnAxis(axis)) {
				if (collision.IsSafeColliding) {
					if (best == null ||
						GetPriorityCollision(best, collision) == collision)
					{
						best = collision;
					}
				}
			}
			return best;
		}

		/// <summary>Return which of two collisions takes resolution priority.
		/// </summary>
		private Collision GetPriorityCollision(Collision a, Collision b) {
			// Prefer room edges over anything else
			if (a.Source.Type == CollisionType.RoomEdge)
				return a;
			else if (b.Source.Type == CollisionType.RoomEdge)
				return b;

			// Prefer dynamic collisions over static collisions
			if (a.IsDynamic && !b.IsDynamic)
				return a;
			if (b.IsDynamic && !a.IsDynamic)
				return b;

			Tile tile1 = a.Tile;
			Tile tile2 = b.Tile;
			Entity entity1 = a.Entity;
			Entity entity2 = b.Entity;

			// Prefer entities over tiles
			if (entity1 != null && tile2 != null)
				return a;
			if (entity2 != null && tile1 != null)
				return b;

			// Compare two entities using their ID
			if (entity1 != null && entity2 != null) {
				if (entity1.EntityIndex < entity2.EntityIndex)
					return a;
				if (entity2.EntityIndex < entity1.EntityIndex)
					return b;
			}

			// Compare two tiles using their position/layer
			if (tile1 != null && tile2 != null) {
				if (tile1.Position.Y > tile2.Position.Y)
					return a;
				else if (tile2.Position.Y > tile1.Position.Y)
					return b;
				else if (tile1.Position.X > tile2.Position.X)
					return a;
				else if (tile2.Position.X > tile1.Position.X)
					return b;
				else if (tile1.Layer > tile2.Layer)
					return a;
				else if (tile2.Layer > tile1.Layer)
					return b;
			}

			return a;
		}

















		
		/// <summary>Detect a clip collision between the entity and a solid object.</summary>
		private void DetectClipCollision(Entity entity, CollisionCheck source) {
			object other = source.SolidObject;
			Rectangle2F solidBox = source.SolidBox;

			// Check if there actually is a collision
			Rectangle2F entityBox = entity.Physics.PositionedCollisionBox;
			if (entityBox.Intersects(solidBox)) {
				// Determine the direction the entity is clipping into the solid object
				int clipDirection = GetCollisionClipDirection(entity, other, solidBox);
				int oppositeAxis = 1 - Directions.ToAxis(clipDirection);

				// Replace the collision info for this clip direction, but not if already
				// colliding and clipping in this direction with greater penetration.
				if (clipDirection >= 0) {
					CollisionInfoNew oldCollisionInfo =
						entity.Physics.ClipCollisionInfo[clipDirection];

					CollisionInfoNew newCollisionInfo =
						CreateCollisionInfo(entity, other, solidBox, clipDirection);
					newCollisionInfo.Source = source;
					entity.Physics.AllClipCollisionInfo.Add(newCollisionInfo);

					if (!oldCollisionInfo.IsColliding ||
						(newCollisionInfo.PenetrationDistance >
							oldCollisionInfo.PenetrationDistance ||
							Rectangle2F.Intersect(entityBox, newCollisionInfo.CollisionBox).Size[oppositeAxis] > 
							Rectangle2F.Intersect(entityBox, oldCollisionInfo.CollisionBox).Size[oppositeAxis]))
						entity.Physics.ClipCollisionInfo[clipDirection] = newCollisionInfo;
				}
			}
		}

		/// <summary>Determine the clipping direction for a collision.</summary>
		private int GetCollisionClipDirection(Entity entity, object other, Rectangle2F solidBox) {
			Rectangle2F entityBox = entity.Physics.PositionedCollisionBox;

			// For moving tiles which are moving toward the entity, use the tile's
			// move direction to determine clip direction.
			Tile tile = other as Tile;
			if (tile != null && tile.IsMoving) {
				if ((solidBox.Center - entityBox.Center).Dot(
					Directions.ToVector(tile.MoveDirection)) < 0.0f)
					return Directions.Reverse(tile.MoveDirection);
			}

			// Get the nearest direction from the collision intersection to the center
			// of the solid box.
			Vector2F intersectionCenter =
				Rectangle2F.Intersect(entityBox, solidBox).Center;
			int clipDirection =
				Directions.NearestFromVector(solidBox.Center - intersectionCenter);

			// If the collision can't be resolved, then try to use a direction on the
			// opposite axis.
			CollisionInfoNew testCollision = 
				CreateCollisionInfo(entity, other, solidBox, clipDirection);
			if (!testCollision.IsAllowedClipping &&
				!CanResolveCollision(entity, testCollision))
			{
				int newClipDirection;
				int oppositeAxis = 1 - Directions.ToAxis(clipDirection);
				if (entityBox.Center[oppositeAxis] < solidBox.Center[oppositeAxis])
					newClipDirection = (oppositeAxis == Axes.X ? Directions.Right : Directions.Down);
				else
					newClipDirection = (oppositeAxis == Axes.X ? Directions.Left : Directions.Up);
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
		
		private float GetAllowedPenetration(Entity entity, CollisionCheck collision) {
			if (!entity.Physics.AllowEdgeClipping)
				return 0.0f;
			if (collision.IsTile && collision.Tile.IsInMotion)
				return 0.0f;
			if (collision.IsRoomEdge)
				return 0.0f;
			return entity.Physics.EdgeClipAmount;
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

		/// <summary>Resolve clip collisions that were not resolved from the previous
		/// frame.</summary>
		private void ResolvePreviousClipCollisions(Entity entity) {
			for (int i = 0; i < Directions.Count; i++) {
				CollisionInfoNew collision = entity.Physics.ClipCollisionInfo[i];
				/*
				if (!collision.IsResolved && CanResolveCollision(entity, collision)) {
					float resolveDistance = Math.Max(0.0f, collision.PenetrationDistance - collision.MaxAllowedPenetrationDistance);
					entity.Position -= Directions.ToVector(collision.PenetrationDirection) * resolveDistance;
				}*/
				if (!collision.IsResolved) {
					collision.PenetrationDistance =
						GetClipPenetration(entity.Physics.PositionedCollisionBox,
						collision.CollisionBox, collision.PenetrationDirection);
				}
				else {
					collision.Reset();
					collision.Entity = entity;
					collision.PenetrationDirection = i;
				}
			}
		}
		
		/// <summary>Resolve the clip collisions for each axis.</summary>
		private void ResolveClipCollisions(Entity entity) {
			// For each axis, determine which collisions take priority
			CollisionInfoNew[] collisions = new CollisionInfoNew[2];
			collisions[Axes.X] = GetPriorityCollision(entity,
				entity.Physics.ClipCollisionInfo[Directions.Right],
				entity.Physics.ClipCollisionInfo[Directions.Left]);
			collisions[Axes.Y] = GetPriorityCollision(entity,
				entity.Physics.ClipCollisionInfo[Directions.Up],
				entity.Physics.ClipCollisionInfo[Directions.Down]);

			// Try to resolve each collision individually
			for (int i = 0; i < 2; i++) {
				CollisionInfoNew collision = collisions[i];
				if (collision != null && collision.IsResolvable)
					ResolveClipCollision(entity, collisions[i]);
			}

			// Check if the collisions can be only resolved together and NOT separately
			// For example, if an entity is penetrating the inside corner of a wall,
			// then it is colliding in two directions and can only stop colliding with
			// either if it is moved diagonally.
			if (collisions[0] != null && collisions[1] != null &&
				!collisions[0].IsResolvable && !collisions[1].IsResolvable &&
				CanResolveCollisionPair(entity, collisions[0], collisions[1]))
			{
				for (int i = 0; i < 2; i++)
					ResolveClipCollision(entity, collisions[i]);
			}
		}
		
		/// <summary>Resolve a single clip collisions.</summary>
		private void ResolveClipCollision(Entity entity, CollisionInfoNew collision) {
			// Resolve the collision
			entity.Position += GetPositionalCorrection(collision);
			collision.IsResolved = true;

			// Add to the penetration distance of the opposite collision
			CollisionInfoNew otherCollision = entity.Physics.ClipCollisionInfo[
				Directions.Reverse(collision.PenetrationDirection)];
			if (otherCollision.IsColliding) {
				float resolveDistance = Math.Max(0.0f, collision.PenetrationDistance -
					collision.MaxAllowedPenetrationDistance);
				otherCollision.PenetrationDistance += resolveDistance;
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

			// Resolvable collisions over non resolvable collisions
			if (collision1.IsResolvable && !collision2.IsResolvable)
				return collision1;
			else if (collision2.IsResolvable && !collision1.IsResolvable)
				return collision2;

			// Dynamic collisions over static collisions
			if (IsSolidObjectDynamic(collision1.CollidedObject) && !IsSolidObjectDynamic(collision2.CollidedObject))
				return collision1;
			if (IsSolidObjectDynamic(collision2.CollidedObject) && !IsSolidObjectDynamic(collision1.CollidedObject))
				return collision2;

			Tile tile1 = collision1.CollidedObject as Tile;
			Tile tile2 = collision2.CollidedObject as Tile;
			Entity entity1 = collision1.CollidedObject as Entity;
			Entity entity2 = collision2.CollidedObject as Entity;

			// Entities over tiles.
			if (entity1 != null && tile2 != null)
				return collision1;
			if (entity2 != null && tile1 != null)
				return collision2;

			// Compare two entities.
			if (entity1 != null && entity2 != null) {
				if (entity1.EntityIndex < entity2.EntityIndex)
					return collision1;
				if (entity2.EntityIndex < entity1.EntityIndex)
					return collision2;
			}

			// Compare two tiles.
			if (tile1 != null && tile2 != null) {
				if (tile1.Position.Y > tile2.Position.Y)
					return collision1;
				else if (tile2.Position.Y > tile1.Position.Y)
					return collision2;
				else if (tile1.Position.X > tile2.Position.X)
					return collision1;
				else if (tile2.Position.X > tile1.Position.X)
					return collision2;
				else if (tile1.Layer > tile2.Layer)
					return collision1;
				else if (tile2.Layer > tile1.Layer)
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
			
			// Don't resolve collisions if they are within the allowable edge clip amount.
			if (IsSafeClipping(entity, 1 - Directions.ToAxis(collision.PenetrationDirection), entity.Physics.PositionedCollisionBox, collision.CollidedObject, collision.CollisionBox))
				return false;

			// Static collisions cannot resolve into colliding with other static objects.
			Tile tile = collision.CollidedObject as Tile;
			if (tile != null && !tile.IsInMotion) {
				Vector2F newPosition = entity.Position + GetPositionalCorrection(collision);
				if (IsCollidingAt(entity, newPosition, true))
					return false;
			}

			if ((collision.CollidedObject is Tile) && !entity.Physics.CollideWithWorld)
				return false;
			if ((collision.CollidedObject is Entity) && !entity.Physics.CollideWithEntities)
				return false;

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
		private bool IsCollidingAt(Entity entity, Vector2F position, bool onlyStatic, int clipDirection = -1, float clipDistance = 0.0f) {
			Rectangle2F entityBox = entity.Physics.CollisionBox;
			entityBox.Point += position;
			if (entity.Physics.CollideWithRoomEdge && !((Rectangle2F) roomControl.RoomBounds).Contains(entityBox))
				return true;
			foreach (CollisionCheck check in GetCollisions(entity, entityBox)) {
				if (onlyStatic && IsSolidObjectDynamic(check.SolidObject))
					continue;
				float allowedClipAmount = GetAllowedEdgeClipAmount(entity, check.SolidObject);
				Rectangle2F insetSolidBox = check.SolidBox.Inflated(-allowedClipAmount, -allowedClipAmount);
				if (clipDirection >= 0)
					insetSolidBox.ExtendEdge(Directions.Reverse(clipDirection), allowedClipAmount - clipDistance);
				if (entityBox.Intersects(insetSolidBox))
					return true;
			}
			return false;
		}

		// Returns whether a solid object is considered dynamic (moving tiles or entities).
		private bool IsSolidObjectDynamic(object solidObject) {
			if (solidObject is Tile)
				return ((Tile) solidObject).IsInMotion;
			return (solidObject is Entity);
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
				CollisionInfoNew collision = entity.Physics.ClipCollisionInfo[i];
				if (collision.IsCollidingAndNotAllowedClipping)
					ClipEntityVelocity(entity, collision.PenetrationDirection);
			}
		}
		
		/// <summary>Restrict the entity's velocity in the given direction.</summary>
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

		/// <summary>Check if the entity is being crushed between two collisions.
		/// </summary>
		private void CheckCrush(Entity entity) {
			if (!entity.Physics.IsCrushable)
				return;
			
			// Iterate collisions which are able to crush the entity
			foreach (Collision rock in entity.Physics.ActualCollisions) {
				if (rock.IsTile && rock.Tile.IsMoving &&
					rock.Tile.MoveDirection == Directions.Reverse(rock.Direction))
				{
					int crushDirection = rock.Tile.MoveDirection;
					int crushAxis = Directions.ToAxis(crushDirection);

					// Iterate collisions which are caused by the crushing collision
					foreach (Collision hardPlace in entity.Physics.ActualCollisions) {
						if (hardPlace.Direction == crushDirection) {
							float gap = entity.Physics.CollisionBox.Size[crushAxis] -
								hardPlace.Penetration;
							if (gap <= entity.Physics.CrushMaxGapSize)
								entity.OnCrush(rock, hardPlace);
						}
					}
				}
			}
		}
		

		//-----------------------------------------------------------------------------
		// Movement Collisions
		//-----------------------------------------------------------------------------
		
		/// <summary>Resolve any movement collisions caused by an entity's velocity.
		/// </summary>
		private void ResolveMovementCollisions(Entity entity) {
			
			Rectangle2F checkArea = Rectangle2F.Union(
				Rectangle2F.Translate(entity.Physics.CollisionBox, entity.Position),
				Rectangle2F.Translate(entity.Physics.CollisionBox, entity.Position + entity.Physics.Velocity));

			for (int axis = 0; axis < 2; axis++) {
				
				// Setup the entity's translated collision box based on which axis we're checking.
				Rectangle2F entityBox;
				if (axis == Axes.X) {
					entityBox = Rectangle2F.Translate(entity.Physics.CollisionBox,
						entity.X + entity.Physics.Velocity.X, entity.Y);
				}
				else {
					entityBox = Rectangle2F.Translate(entity.Physics.CollisionBox,
						entity.Position + entity.Physics.Velocity);
				}
				
				// Find all collisions
				foreach (CollisionCheck check in GetCollisions(entity, checkArea)) {
					if (entityBox.Intersects(check.SolidBox)) {
						// Determine clipping direction
						int clipDirection = -1;
						if (axis == Axes.X) {
							if (entityBox.Center.X < check.SolidBox.Center.X)
								clipDirection = Directions.Right;
							else
								clipDirection = Directions.Left;
						}
						else {
							if (entityBox.Center.Y < check.SolidBox.Center.Y)
								clipDirection = Directions.Down;
							else
								clipDirection = Directions.Up;
						}

						CollisionInfo info = new CollisionInfo();
						info.SetCollision(check.SolidObject, clipDirection);
						info.IsMovementCollision = true;
						info.SolidBox = check.SolidBox;
						
						// Determine the penetration
						float penetrationDistance = GetClipPenetration(entityBox, info.SolidBox, clipDirection);
						float maxAllowedPenetration = 0.0f;
						if (entity.Physics.Velocity[axis] == 0.0f)
							maxAllowedPenetration = GetAllowedEdgeClipAmount(entity, check.SolidObject);
						info.Penetration = penetrationDistance;

						// Ignore collisions that are within the allowed clipping range
						if (IsSafeClipping(entity, 1 - axis, entityBox, info.SolidObject, info.SolidBox))
							continue;
						entity.Physics.AllMovementCollisionInfo.Add(info);
					}
				}
				
				for (int i = 0; i < entity.Physics.AllMovementCollisionInfo.Count; i++) {
					CollisionInfo info = entity.Physics.AllMovementCollisionInfo[i];
					if (Directions.ToAxis(info.Direction) == axis) {
						if (!ResolveMovementCollision(entity, info))
							entity.Physics.AllMovementCollisionInfo.RemoveAt(i--);
					}
				}
			}

			//for (int i = 0; i < entity.Physics.AllMovementCollisionInfo.Count; i++) {
			//	CollisionInfo info = entity.Physics.AllMovementCollisionInfo[i];
			//	if (ResolveMovementCollision(entity, Directions.ToAxis(info.Direction),
			//		info.SolidObject, info.SolidBox))
			//		info.IsResolved = true;
			//}
						
			//Rectangle2F checkArea = Rectangle2F.Union(
				//Rectangle2F.Translate(entity.Physics.CollisionBox, entity.Position),
				//Rectangle2F.Translate(entity.Physics.CollisionBox, entity.Position + entity.Physics.Velocity));

			/*for (int axis = 0; axis < 2; axis++) {
				foreach (CollisionCheck check in GetCollisions(entity, checkArea)) {
					ResolveMovementCollision(entity, axis,
						check.SolidObject, check.SolidBox);
				}
			}*/
		}
		
		/// <summary>Resolve any movement collision between the entity and a solid
		/// object.</summary>
		private bool ResolveMovementCollision(Entity entity, CollisionInfo info) {
			int axis = info.Axis;
			object other = info.SolidObject;
			Rectangle2F solidBox = info.SolidBox;
			int clipDirection = info.Direction;

			// Setup the entity's translated collision box based on which axis we're checking.
			Rectangle2F entityBox;
			if (axis == Axes.X) {
				entityBox = Rectangle2F.Translate(entity.Physics.CollisionBox,
					entity.X + entity.Physics.Velocity.X, entity.Y);
			}
			else {
				entityBox = Rectangle2F.Translate(entity.Physics.CollisionBox,
					entity.Position + entity.Physics.Velocity);
			}


			// Check if this collision was resolved due to resolving another collision
			if (!entityBox.Intersects(solidBox)) {
				info.IsResolved = true;
				return true;
			}

			// Determine the unresolved penetration
			float penetrationDistance = GetClipPenetration(entityBox, solidBox, clipDirection);
			float maxAllowedPenetration = 0.0f;
			if (entity.Physics.Velocity[axis] == 0.0f)
				maxAllowedPenetration = GetAllowedEdgeClipAmount(entity, other);

			// Ignore collisions that are within the allowed clipping range
			if (IsSafeClipping(entity, 1 - axis, entityBox, other, solidBox) || 
				penetrationDistance <= maxAllowedPenetration)
			{
				info.IsResolved = true;
				return true;
			}

			// Ignore collision if the entity is already colliding in the opposite
			// direction or if the entity is not moving in the clip direction.
			if (entity.Physics.ClipCollisionInfo[Directions.Reverse(clipDirection)].IsCollidingAndNotAllowedClipping ||
				entity.Physics.Velocity.Dot(Directions.ToVector(clipDirection)) <= 0.0f)
				return true;
			
			// Ignore the collision if the entity is clipped into a solid object that shares
			// a clip edge with this object and the entity is also moving parallel with that edge.
			for (int i = 0; i < 2; i++) {
				int dir = Axes.GetOpposite(axis) + (i * 2);
				CollisionInfoNew checkCollision = entity.Physics.ClipCollisionInfo[dir];
				if (checkCollision.IsColliding && AreEdgesAligned(
					checkCollision.CollisionBox, solidBox, Directions.Reverse(dir)))
				{
					return true;
				}
			}

			// Resolve the collision
			if (!entity.Physics.IsColliding &&
				!entity.Physics.ClipCollisionInfo[clipDirection].IsColliding)
			{
				// Snap the entity's position to the edge of the solid object
				Vector2F resolvedEntityPosition = entity.Position;
				resolvedEntityPosition[axis] += entity.Physics.Velocity[axis];
				resolvedEntityPosition -= Directions.ToVector(clipDirection) * penetrationDistance;
				if (penetrationDistance <= maxAllowedPenetration)
					resolvedEntityPosition += Directions.ToVector(clipDirection) * penetrationDistance;
				entity.Position = resolvedEntityPosition;
				entity.Physics.CollisionInfo[clipDirection].SetCollision(other, clipDirection);
			}
			
			// Zero the entity's velocity for this axis
			Vector2F velocity = entity.Physics.Velocity;
			velocity[axis] = 0.0f;
			entity.Physics.Velocity = velocity;
			
			entity.Physics.MovementCollisions[clipDirection] = true;
			if (!entity.Physics.CollisionInfo[clipDirection].IsColliding)
				entity.Physics.CollisionInfo[clipDirection].SetCollision(other, clipDirection);
			
			// Perform collision dodging
			bool canDodge = true;
			if (entity is Player) {
				// TODO: this won't work for when player is knocked back
				canDodge = ((Player)entity).Movement.AllowMovementControl && Controls.Arrows[clipDirection].IsDown();
			}
			if (entity.Physics.AutoDodges && canDodge) { 
				if (roomControl.IsSideScrolling && axis == Axes.X &&
					(!(entity is Player) || !((Player) entity).IsOnSideScrollLadder || ((other is Tile) && ((Tile) other).IsInMotion)) &&
					(entity.IsOnGround || entity.Physics.PreviousCollisionInfo[Directions.Down].IsColliding))
				{
					// Step up onto blocks if it is a small enough distance.
					PerformSideScrollCollisionSnap(entity, clipDirection, other, solidBox);
				}
				else {
					// Auto dodging
					if (PerformCollisionDodge(entity, clipDirection, other, solidBox))
						info.IsAutoDodged = true;
				}
			}

			info.IsResolved = true;
			return true;
		}

		// Attempt to dodge a collision.
		private bool PerformCollisionDodge(Entity entity, int direction, object solidObject, Rectangle2F solidBox) {
			// Only dodge if moving perpendicular to the edge.
			int axis = Directions.ToAxis(direction);
			if (Math.Abs(entity.Physics.Velocity[1 - axis]) > GameSettings.EPSILON)
				return false;

			// Can't dodge moving tiles.
			if ((solidObject is Tile) && ((Tile) solidObject).IsMoving)
				return false;

			Rectangle2F entityBox = entity.Physics.PositionedCollisionBox;
			float penetrationDistance = GetClipPenetration(
				entity.Physics.PositionedCollisionBox, solidBox, direction);

			// Check dodging for both edges of the solid object.
			for (int side = 0; side < 2; side++) {
				int moveDirection = (direction + (side == 0 ? 1 : 3)) % 4;
				float distanceToEdge = Math.Abs(entityBox.GetEdge(
					Directions.Reverse(moveDirection)) - solidBox.GetEdge(moveDirection));
				
				// Check if the distance to the edge is within dodge range.
				if (distanceToEdge <= entity.Physics.AutoDodgeDistance) {
					float moveAmount = Math.Min(entity.Physics.AutoDodgeSpeed, distanceToEdge);

					Vector2F nextPosition = GMath.Round(entity.Position) +
						(Directions.ToVector(moveDirection) * moveAmount);
					Vector2F goalPosition = entity.Position + Directions.ToVector(direction) +
						(Directions.ToVector(moveDirection) * distanceToEdge);
					
					// Make sure the entity is not colliding when placed at the solid object's edge.
					if (!IsCollidingAt(entity, nextPosition, false, direction, penetrationDistance) &&
						!IsCollidingAt(entity, goalPosition, false, direction, penetrationDistance))
					{
						entity.Position += Directions.ToVector(moveDirection) * moveAmount;
						entity.Physics.CollisionInfo[direction].IsAutoDodged = true;
						//entity.Physics.MovementCollisions[direction] = false; // TODO: Figure out complications for removing this.
						return true;
					}
				}
			}

			return false;
		}
		
		// Attempt to snap a collision.
		private bool PerformSideScrollCollisionSnap(Entity entity, int direction, object solidObject, Rectangle2F solidBox) {
			Rectangle2F entityBox = entity.Physics.PositionedCollisionBox;
			float penetrationDistance = GetClipPenetration(
				entity.Physics.PositionedCollisionBox, solidBox, direction);
			float distanceToEdge = entityBox.Bottom - solidBox.Top;
			//Vector2F goalPosition = entity.Position + Directions.ToVector(direction) +
				//(Directions.ToVector(Directions.Up) * distanceToEdge);
			
			Vector2F goalPosition = entity.Position +
				Directions.ToVector(direction) -
				new Vector2F(0.0f, distanceToEdge);

			// Check if the distance to the edge is within dodge range.
			// Make sure the entity is not colliding when placed at the solid object's edge.
			if (distanceToEdge <= entity.Physics.AutoDodgeDistance &&
				!IsCollidingAt(entity, goalPosition, false, direction, penetrationDistance))
			{
				entity.Position = goalPosition;
				entity.Physics.MovementCollisions[Directions.Down] = true;
				entity.Physics.CollisionInfo[Directions.Down].SetCollision(solidObject, Directions.Down);
				return true;
			}
			return false;
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
		public static bool AreEdgesAligned(Rectangle2F box1, Rectangle2F box2, int edgeDirection) {
			return Math.Abs(box1.GetEdge(edgeDirection) - box2.GetEdge(edgeDirection)) < 0.1f;
		}
		

		//-----------------------------------------------------------------------------
		// Side Scrolling Ladder Collisions
		//-----------------------------------------------------------------------------

		// Check the player's side-scrolling ladder collisions.
		private void CheckPlayerLadderClimbing(Player player) {
			if (!roomControl.IsSideScrolling)
				return;
			
			// Get the highest ladder the player is touching
			Rectangle2F climbBoxPrev = Rectangle2F.Translate(
				player.Movement.ClimbCollisionBox, player.PreviousPosition);
			Tile ladder = GetHighestLadder(climbBoxPrev, player.PreviousPosition);
			
			// Check for beginning climbing
			if (ladder != null && !player.IsOnSideScrollLadder) {

				Rectangle2F ladderBox = ladder.Bounds;
				Rectangle2F collisionBox = player.Physics.PositionedCollisionBox;
				Rectangle2F climbBoxNext = Rectangle2F.Translate(
					player.Movement.ClimbCollisionBox,
					player.Position + player.Physics.Velocity);
				CollisionInfo standingCollisionPrev =
					player.Physics.PreviousCollisionInfo[Directions.Down];
				CollisionInfo standingCollision =
					player.Physics.CollisionInfo[Directions.Down];
				
				// Check if this ladder is a ladder-top
				Tile checkAboveTile = player.RoomControl.TileManager
					.GetSurfaceTile(ladder.Location - new Point2I(0, 1));
				bool isTopLadder = (checkAboveTile == null ||
					!checkAboveTile.IsLadder);

				// Check for beginning climbing by moving up while falling
				if ((player.Physics.VelocityY >= 0.0f || !player.Physics.HasGravity) &&
					!player.Physics.IsInWater &&
					player.Movement.IsMovingInDirection(Directions.Up))
				{
					player.BeginEnvironmentState(player.SideScrollLadderState);
					player.Physics.VelocityY = 0.0f;
				}

				// Check for beginning climbing by stepping off of a solid object and
				// onto the ladder. Make sure the player is not on a flat surface
				// aligned with the ladder-top.  <--FIXME: safe clipping makes this false
				else if (climbBoxNext.Intersects(ladderBox) &&
					standingCollisionPrev.IsColliding &&
					standingCollisionPrev.Tile != ladder &&
					!standingCollision.IsColliding &&
					player.Physics.VelocityY >= 0.0f &&
					(!isTopLadder || collisionBox.Bottom > ladderBox.Top ||
						standingCollisionPrev.Tile == null ||
						standingCollisionPrev.Tile.Bounds.Top != ladderBox.Top))
				{
					player.BeginEnvironmentState(player.SideScrollLadderState);
					return;
				}
			}

			// Collide with ladder tops
			Rectangle2F ladderCheckArea = Rectangle2F.Union(
				Rectangle2F.Translate(player.Physics.CollisionBox,
					player.Position),
				Rectangle2F.Translate(player.Physics.CollisionBox,
					player.Position + player.Physics.Velocity));
			foreach (Tile tile in
				RoomControl.TileManager.GetTilesTouching(ladderCheckArea))
			{
				if (tile.IsLadder)
					CheckLadderCollision(player, tile);
			}
		}
		
		/// <summary>Return the top-most ladder the player is colliding with when
		/// placed at the given position</summary>
		private Tile GetHighestLadder(Rectangle2F collisionBox, Vector2F position) {
			Tile highestLadder = null;
			foreach (Tile tile in
				RoomControl.TileManager.GetTilesTouching(collisionBox))
			{
				if (tile.IsLadder && (highestLadder == null ||
					tile.Bounds.Top < highestLadder.Bounds.Top))
					highestLadder = tile;
			}
			return highestLadder;
		}

		/// <summary>Check collisions with ladder-tops</summary>
		private void CheckLadderCollision(Player player, Tile ladder) {
			Rectangle2F ladderBox		= ladder.Bounds;
			Rectangle2F entityBoxPrev	= player.Physics.PositionedCollisionBox;
			Rectangle2F entityBox		= Rectangle2F.Translate(entityBoxPrev, player.Physics.Velocity);
			Rectangle2F climbBox		= player.Movement.ClimbCollisionBox;
			Rectangle2F climbBoxPrev	= climbBox;
			climbBox.Point		+= player.Position + player.Physics.Velocity;
			climbBoxPrev.Point	+= player.Position;
			
			// Check if this tile is a top-most ladder
			Tile checkAboveTile = player.RoomControl.TileManager
				.GetSurfaceTile(ladder.Location - new Point2I(0, 1));
			bool isTopLadder = (checkAboveTile == null || !checkAboveTile.IsLadder);
			
			// The player can collide with the top of a ladder if he...
			//  - is not climbing
			//  - is touching the ladder
			//  - was previously above the ladder
			//  - is not standing on something else with clipping
			if (isTopLadder && !player.IsOnSideScrollLadder &&
				entityBox.Intersects(ladderBox) &&
				entityBoxPrev.Bottom <= ladderBox.Top &&
				!player.Physics.ClipCollisionInfo[Directions.Down].IsColliding)
			{
				// If holding the [Down] button, then begin climbing the ladder instead
				if (player.Movement.IsMovingInDirection(Directions.Down)) {
					player.BeginEnvironmentState(player.SideScrollLadderState);
				}
				else {
					// Collide with the top of the ladder
					player.Physics.VelocityY = 0.0f;
					player.Y = ladderBox.Top - player.Physics.CollisionBox.Bottom;
					player.Physics.MovementCollisions[Directions.Down] = true;
					if (!player.Physics.CollisionInfo[Directions.Down].IsColliding)
						player.Physics.CollisionInfo[Directions.Down].SetCollision(ladder, Directions.Down);
				}
			}
		}
		
		
		//-----------------------------------------------------------------------------
		// Room Boundaries
		//-----------------------------------------------------------------------------

		// Check collisions with the room's edges.
		private void CheckRoomEdgeCollisions(Entity entity) {
			if (!entity.Physics.CollideWithRoomEdge && !entity.Physics.ReboundRoomEdge)
				return;
			
			Rectangle2F collisionBox = entity.Physics.CollisionBox;
			Rectangle2F entityBox = entity.Physics.CollisionBox;
			entityBox.Point += entity.Position + entity.Physics.Velocity;
			Rectangle2F roomBounds = roomControl.RoomBounds;

			/*
			for (int i = 0; i < Directions.Count; i++) {
				int axis = Directions.ToAxis(i);
				float penetration = GetClipPenetration(roomBounds, entityBox, i);
				float distanceOutside = entityBox.Size[axis] - penetration;

				if (distanceOutside > 0.0f) {
					entity.Position -= Directions.ToVector(i) * distanceOutside;

					entity.Physics.IsColliding = true;
					entity.Physics.CollisionInfo[i].SetRoomEdgeCollision(i);
				}
			}*/

			if (entityBox.Left < roomBounds.Left) {
				entity.Physics.IsColliding = true;
				entity.X = roomBounds.Left - collisionBox.Left;
				if (entity.Physics.ReboundRoomEdge && entity.Physics.ReboundVelocity.X == 0.0f)
					entity.Physics.ReboundVelocity = new Vector2F(-entity.Physics.VelocityX, entity.Physics.ReboundVelocity.Y);
				entity.Physics.VelocityX = 0;
				entity.Physics.CollisionInfo[Directions.Left].SetRoomEdgeCollision(Directions.Left);
			}
			else if (entityBox.Right > roomBounds.Right) {
				entity.Physics.IsColliding = true;
				entity.X = roomBounds.Right - collisionBox.Right;
				if (entity.Physics.ReboundRoomEdge && entity.Physics.ReboundVelocity.X == 0.0f)
					entity.Physics.ReboundVelocity = new Vector2F(-entity.Physics.VelocityX, entity.Physics.ReboundVelocity.Y);
				entity.Physics.VelocityX = 0;
				entity.Physics.CollisionInfo[Directions.Right].SetRoomEdgeCollision(Directions.Right);
			}
			if (entityBox.Top < roomBounds.Top) {
				entity.Physics.IsColliding = true;
				entity.Y = roomBounds.Top - collisionBox.Top;
				if (entity.Physics.ReboundRoomEdge && entity.Physics.ReboundVelocity.Y == 0.0f)
					entity.Physics.ReboundVelocity = new Vector2F(entity.Physics.ReboundVelocity.X, -entity.Physics.VelocityY);
				entity.Physics.VelocityY = 0;
				entity.Physics.CollisionInfo[Directions.Up].SetRoomEdgeCollision(Directions.Up);
			}
			else if (entityBox.Bottom > roomBounds.Bottom) {
				entity.Physics.IsColliding = true;
				entity.Y = roomBounds.Bottom - collisionBox.Bottom;
				if (entity.Physics.ReboundRoomEdge && entity.Physics.ReboundVelocity.Y == 0.0f)
					entity.Physics.ReboundVelocity = new Vector2F(entity.Physics.ReboundVelocity.X, -entity.Physics.VelocityY);
				entity.Physics.VelocityY = 0;
				entity.Physics.CollisionInfo[Directions.Down].SetRoomEdgeCollision(Directions.Down);
			}
		}

		// Check if destroyed outside room.
		private void CheckOutsideRoomBounds(Entity entity) {
			if (entity.Physics.IsDestroyedOutsideRoom &&
				!entity.RoomControl.RoomBounds.Contains(entity.Position))
			{
				entity.Destroy();
				return;
			}
		}
		
		
		//-----------------------------------------------------------------------------
		// Z-Dynamics
		//-----------------------------------------------------------------------------

		public void UpdateEntityZPosition(Entity entity) {

			if (IsSideScrolling) {
				// Convert any nonzero Z-position into Y-position.
				if (entity.ZPosition != 0.0f) {
					entity.Y -= entity.ZPosition;
					entity.ZPosition = 0.0f;
				}
				
				if (entity.Physics.HasGravity && !entity.Physics.OnGroundOverride) {
					// Zero jump speed if the entity is colliding below.
					if (entity.Physics.ZVelocity < 0.0f &&
						entity.Physics.CollisionInfo[Directions.Down].IsColliding)
						entity.Physics.ZVelocity = 0.0f;
					// Zero jump speed if the entity is colliding above.
					// NOTE: this doesn't actually happen in the real game, which is strange.
					//else if (entity.Physics.ZVelocity > 0.0f && entity.Physics.CollisionInfo[Directions.Up].IsColliding)
						//entity.Physics.ZVelocity = 0.0f;
					// Integrate acceleration due to gravity
					entity.Physics.ZVelocity -= entity.Physics.Gravity;
					// Limit to maximum fall speed
					if (entity.Physics.ZVelocity < -entity.Physics.MaxFallSpeed && entity.Physics.MaxFallSpeed >= 0.0f)
						entity.Physics.ZVelocity = -entity.Physics.MaxFallSpeed;
					// Convert the Z-velocity to Y-velocity
					entity.Physics.VelocityY = -entity.Physics.ZVelocity;
				}
				else {
					entity.Physics.ZVelocity = 0.0f;
				}
			}
			else if (entity.ZPosition > 0.0f || entity.Physics.ZVelocity != 0.0f) {
				// Integrate gravity acceleration.
				if (entity.Physics.HasGravity && !entity.Physics.OnGroundOverride) {
					entity.Physics.ZVelocity -= entity.Physics.Gravity;
					if (entity.Physics.ZVelocity < -entity.Physics.MaxFallSpeed && entity.Physics.MaxFallSpeed >= 0.0f)
						entity.Physics.ZVelocity = -entity.Physics.MaxFallSpeed;
				}

				// Integrate z-velocity.
				entity.ZPosition += entity.Physics.ZVelocity;

				// Check if landed on the ground.
				if (entity.ZPosition <= 0.0f)
					LandEntity(entity);
			}
			else
				entity.Physics.ZVelocity = 0.0f;
		}

		public void LandEntity(Entity entity) {
			// Check if landed in a hazard surface.
			CheckHazardSurface(entity);
			if (entity.IsDestroyed)
				return;

			if (entity.Physics.Bounces && entity.Physics.ZVelocity < -1.0f) {
				// Bounce back into the air.
				entity.ZPosition = 0.1f;
				entity.Physics.ZVelocity = -entity.Physics.ZVelocity * 0.5f;

				// Half the lateral velocity upon bouncing.
				if (entity.Physics.Velocity.Length > 0.25f)
					entity.Physics.Velocity *= 0.5f;
				else
					entity.Physics.Velocity = Vector2F.Zero;
				
				entity.OnBounce();
			}
			else {
				// Stay on the ground.
				entity.ZPosition = 0.0f;
				entity.Physics.ZVelocity = 0.0f;

				if (entity.Physics.Bounces) {
					entity.Physics.Velocity = Vector2F.Zero;
					entity.OnBounce();
				}

				entity.OnLand();
			}
		}
		

		//-----------------------------------------------------------------------------
		// Surfaces
		//-----------------------------------------------------------------------------
		
		// Check the surface tile beneath the entity
		private void CheckSurfaceTile(Entity entity) {

			entity.Physics.SurfacePosition = Vector2F.Zero;
			entity.Physics.SurfaceVelocity = Vector2F.Zero;

			// Find the surface tile underneath the entity
			entity.Physics.TopTile = roomControl.TileManager.GetSurfaceTileAtPosition(
				entity.Position + entity.Physics.TopTilePointOffset,
				entity.Physics.MovesWithPlatforms);
			
			// Check for moving platforms in side scrolling mode
			CollisionInfo surfaceCollision = entity.Physics
				.PreviousCollisionInfo[Directions.Down];
			if (roomControl.IsSideScrolling &&
				(surfaceCollision.Tile != null || surfaceCollision.Entity != null))
			{
				// Get the velocity of the surface tile or entity
				if (surfaceCollision.Tile != null) {
					entity.Physics.SurfacePosition = surfaceCollision.Tile.Position;
					entity.Physics.SurfaceVelocity = surfaceCollision.Tile.Velocity;
				}
				else {
					entity.Physics.SurfacePosition = surfaceCollision.Entity.Position;
					entity.Physics.SurfaceVelocity = surfaceCollision.Entity.Physics.Velocity;
				}

				// Move with the surface
				// NOTE: this really needs some checks before execution
				entity.Y += entity.Physics.SurfaceVelocity.Y;
				entity.Physics.VelocityX += entity.Physics.SurfaceVelocity.X;

				// Move with conveyor tiles
				if (surfaceCollision.Tile != null && entity.Physics.MovesWithConveyors)
					entity.Physics.VelocityX += surfaceCollision.Tile.ConveyorVelocity.X;
			}

			// Check if the surface is moving
			if (entity.Physics.TopTile != null) {
				if (entity.Physics.MovesWithPlatforms) {
					entity.Physics.Velocity += entity.Physics.TopTile.Velocity;
					if (!roomControl.IsSideScrolling) {
						entity.Physics.SurfacePosition = entity.Physics.TopTile.Position;
						entity.Physics.SurfaceVelocity += entity.Physics.TopTile.Velocity;
					}
				}
				if (entity.Physics.MovesWithConveyors && entity.Physics.IsOnGround)
					entity.Physics.Velocity += entity.Physics.TopTile.ConveyorVelocity;
			}
			
			// Check if surface tile is a hazardous (water/lava/hole)
			CheckHazardSurface(entity);
		}
		
		// Check if the entity is sitting on a hazardous surface (water/lava/hole).
		private void CheckHazardSurface(Entity entity) {
			if (entity.Physics.IsInHole)
				entity.OnFallInHole();
			else if (entity.Physics.IsInWater)
				entity.OnFallInWater();
			else if (entity.Physics.IsInLava)
				entity.OnFallInLava();
		}


		//-----------------------------------------------------------------------------
		// Ledge Passing
		//-----------------------------------------------------------------------------
		
		// Update ledge passing, handling changes in altitude.
		public void CheckLedges(Entity entity) {
			if (!entity.Physics.PassOverLedges)
				return;

			Point2I prevLocation = entity.RoomControl.GetTileLocation(entity.PreviousPosition + entity.Physics.CollisionBox.Center);
			Point2I location = entity.RoomControl.GetTileLocation(entity.Position + entity.Physics.CollisionBox.Center);

			// When moving over a new tile, check its ledge state.
			if (location != prevLocation) {
				entity.Physics.LedgeTileLocation = new Point2I(-1, -1);

				if (entity.RoomControl.IsTileInBounds(location)) {
					Tile tile = entity.RoomControl.GetTopTile(location);
					
					if (tile != null && tile.IsAnyLedge) {
						entity.Physics.LedgeTileLocation = location;
						// Adjust ledge altitude.
						if (IsMovingUpLedge(entity, tile))
							entity.Physics.LedgeAltitude--;
						else if (IsMovingDownLedge(entity, tile))
							entity.Physics.LedgeAltitude++;
					}
				}
			}
		}

		// Returns true if the entity moving down the ledge.
		public bool IsMovingDownLedge(Entity entity, Tile ledgeTile) {
			return entity.Physics.Velocity.Dot(Directions.ToVector(ledgeTile.LedgeDirection)) > 0.0f;
		}

		// Returns true if the entity moving up the ledge.
		public bool IsMovingUpLedge(Entity entity, Tile ledgeTile) {
			return entity.Physics.Velocity.Dot(Directions.ToVector(ledgeTile.LedgeDirection)) < 0.0f;
		}

		
		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public RoomControl RoomControl {
			get { return roomControl; }
		}

		public bool IsSideScrolling {
			get { return roomControl.IsSideScrolling; }
		}
	}
}
