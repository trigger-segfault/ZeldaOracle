using System;
using System.Collections.Generic;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Entities.Collisions;
using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.Tiles;

namespace ZeldaOracle.Game.Control {

	public class RoomPhysics {

		private RoomControl roomControl;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public RoomPhysics(RoomControl roomControl) {
			this.roomControl = roomControl;
		}


		//-----------------------------------------------------------------------------
		// Physics Processing
		//-----------------------------------------------------------------------------

		/// <summary>Process all entity physics for the room.</summary>
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
		private void ProcessEntityPhysics(Entity entity) {
			// Update Z dynamics
			UpdateEntityZPosition(entity);

			// Initialize the collision state for this frame
			InitPhysicsState(entity);

			// Check the surface tile beneath the entity
			CheckSurfaceTile(entity);

			// Resolve collisions with solid objects while integrating velocity
			CheckSolidCollisions(entity);

			// Check for landing and falling in side-scrolling mode
			if (IsSideScrolling && entity.Physics.HasGravity) {
				if (!entity.Physics.IsOnGroundPrevious && entity.IsOnGround)
					LandEntity(entity);
				if (entity.Physics.IsOnGroundPrevious && !entity.IsOnGround)
					entity.OnBeginFalling();
			}

			// Update ledge passing
			UpdateLedgePassing(entity);

			// Check if destroyed outside room
			CheckOutsideRoomBounds(entity);

			entity.Physics.IsOnGroundPrevious = entity.Physics.IsOnGround;
		}

		/// <summary>Initialize an entity's physics state for the frame.</summary>
		private void InitPhysicsState(Entity entity) {
			entity.Physics.PreviousVelocity		= entity.Physics.Velocity;
			entity.Physics.PreviousZVelocity	= entity.Physics.ZVelocity;
			entity.Physics.IsColliding			= false;
			
			// Swap the current and previous collision lists
			List<Collision> temp = entity.Physics.PreviousPotentialCollisions;
			entity.Physics.PreviousPotentialCollisions =
				entity.Physics.PotentialCollisions;
			entity.Physics.PotentialCollisions = temp;
			entity.Physics.PotentialCollisions.Clear();
		}

		
		//-----------------------------------------------------------------------------
		// Solid Collisions
		//-----------------------------------------------------------------------------

		/// <summary>Detect and resolve collisions with solid objects (entities,
		/// tiles, and room edges).</summary>
		private void CheckSolidCollisions(Entity entity) {

			// Handle circular tile collisions
			if (entity.Physics.CollideWithWorld) {
				if (entity.Physics.CollideWithWorld &&
					entity.Physics.CheckRadialCollisions)
				{
					Rectangle2F checkArea = Rectangle2F.Union(
						Rectangle2F.Translate(entity.Physics.CollisionBox,
							entity.Position),
						Rectangle2F.Translate(entity.Physics.CollisionBox,
							entity.Position + entity.Physics.Velocity));

					foreach (Tile tile in
						roomControl.TileManager.GetTilesTouching(checkArea))
					{
						if (CanCollideWithTile(entity, tile) &&
							tile.CollisionStyle == CollisionStyle.Circular)
						{
							ResolveCircularCollision(entity, tile,
								tile.Position, tile.CollisionModel);
						}
					}
				}
			}

			// Detect all potential collisions with nearby solid objects
			DetectPotentialCollisions(entity);

			// Resolve all penetrating collisions
			ResolveCollisions(entity);
			
			// Update the standing collision
			entity.Physics.PreviousStandingCollision =
				entity.Physics.StandingCollision;
			if (IsSideScrolling)
				entity.Physics.StandingCollision =
					entity.Physics.GetCollisionInDirection(Directions.Down);
			else
				entity.Physics.StandingCollision = null;

			// Check if the entity is being crushed between two collisions
			CheckCrush(entity);

			// Check the player's side-scrolling ladder collisions
			if (entity.Physics.CollideWithWorld && (entity is Player))
				CheckPlayerLadderClimbing((Player) entity);
		}

		
		//-----------------------------------------------------------------------------
		// Circular Collisions
		//-----------------------------------------------------------------------------
		
		private void ResolveCircularCollision(Entity entity, Tile tile,
			Vector2F modelPos, CollisionModel model)
		{
			for (int i = 0; i < model.BoxCount; i++) {
				Rectangle2F box = Rectangle2F.Translate(model.Boxes[i], modelPos);
				ResolveCircularCollision(entity, tile, box);
			}
		}
		
		private void ResolveCircularCollision(Entity entity, object solidObject,
			Rectangle2F solidBox)
		{
			Rectangle2F collisionBox = entity.Physics.CollisionBox;
			Rectangle2F entityBoxPrev = entity.Physics.CollisionBox;
			entityBoxPrev.Point += entity.Position;

			// If already colliding with the object, then push away from its center
			if (entityBoxPrev.Intersects(solidBox)) {
				Vector2F correction = (entity.Position - solidBox.Center).Normalized;
				correction = Vector2F.SnapDirectionByCount(correction, 16);
				entity.Position += 1.0f * correction;
				return;
			}

			// Predict collisions for each axis
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
							//if (!IsCollidingAt(entity, newPos, false)) // TODO
								entity.Position = newPos;
						}
					}
					newVelocity[axis] = 0.0f;
					entity.Physics.Velocity = newVelocity;
				}
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
			if (checkTile.IsAnyLedge && entity.Physics.PassOverLedges &&
				((entity.Physics.LedgePassState != LedgePassState.None &&
					entity.Physics.LedgePassTile == checkTile) ||
				IsMovingDownLedge(entity, checkTile) ||
				IsMovingUpLedge(entity, checkTile) && entity.Physics.LedgeAltitude > 0))
			{
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
		
		/// <summary>Returns true if the entity should rebound off of a collision.
		/// </summary>
		private bool CanReboundColliison(Entity entity, Collision collision) {
			if (collision.IsRoomEdge)
				return entity.Physics.ReboundRoomEdge;
			return entity.Physics.ReboundSolid;
		}
		
		/// <summary>Returns true if the collision is able to crush the entity.
		/// </summary>
		private bool CanColliisonCrush(Entity entity, Collision collision) {
			if (entity.Physics.IsCrushable) {
				if (collision.IsEntity)
					return false;
				if (collision.IsTile)
					return (collision.Tile.IsMoving &&
						collision.Tile.MoveDirection ==
							Directions.Reverse(collision.Direction));
			}
			return false;
		}

		/// <summary>Get the allowable penetration distance for a collision.</summary>
		private float GetAllowedPenetration(Entity entity, CollisionCheck collision) {
			if (!entity.Physics.AllowEdgeClipping)
				return 0.0f;
			if (collision.IsTile && (collision.Tile.IsInMotion ||
				collision.Tile.IsLadder))
				return 0.0f;
			if (collision.IsRoomEdge)
				return 0.0f;
			return entity.Physics.EdgeClipAmount;
		}
				

		//-----------------------------------------------------------------------------
		// Collision Detection
		//-----------------------------------------------------------------------------
		
		/// <summary>Find an entity's nearby collisions.</summary>
		private void DetectPotentialCollisions(Entity entity) {
			// Create a list of all potential collisions
			Rectangle2F checkArea = Rectangle2F.Union(
				Rectangle2F.Translate(entity.Physics.CollisionBox, entity.Position),
				Rectangle2F.Translate(entity.Physics.CollisionBox,
					entity.Position + entity.Physics.Velocity));
			checkArea.Inflate(10, 10);
			foreach (CollisionCheck check in
				GetPotentialCollisions(entity, checkArea))
			{
				Collision collision = InitPotentialCollision(entity, check);
				entity.Physics.PotentialCollisions.Add(collision);
			}

			// Find collision edges which are shared among two static solid objects
			ConnectStaticCollisions(entity);

			// Calculate the penetration direction and distance for each collision
			for (int i = 0; i < entity.Physics.PotentialCollisions.Count; i++) {
				Collision collision = entity.Physics.PotentialCollisions[i];
				collision.Direction = CalcCollisionDirection(entity, collision);

				if (collision.Direction >= 0) {
					collision.CalcPenetration();
					// Do not allow any additional penetration if not already
					// penetrating
					collision.AllowedPenetration = GMath.Clamp(
						collision.AllowedPenetration,
						0.0f, collision.Penetration);
					collision.CalcIsColliding();
				}
				else {
					entity.Physics.PotentialCollisions.RemoveAt(i--);
				}
			}
		}
		
		/// <summary>Returns an enumerable list of all solid objects that an entity can
		/// potentially collide with.</summary> 
		private IEnumerable<CollisionCheck> GetPotentialCollisions(Entity entity,
			Rectangle2F area)
		{
			if (entity.Physics.CollideWithWorld) {
				// Find nearby solid entities
				foreach (Entity other in RoomControl.Entities) {
					if (other != entity && CanCollideWithEntity(entity, other)) {
						yield return CollisionCheck.CreateEntityCollision(other);
					}
				}

				// Find nearby solid tiles
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
					// Collide with ladder tops
					else if (IsSideScrolling &&
						tile.IsLadder && (entity is Player) &&
						entity.Physics.HasGravity &&
						entity.Physics.VelocityY >= 0.0f &&
						entity.Physics.PositionedCollisionBox.Bottom <= tile.Bounds.Top &&
						!((Player) entity).Movement.IsMovingInDirection(Directions.Down))
					{
						// Check if this tile is a top-most ladder
						Tile checkAboveTile = RoomControl.TileManager
							.GetSurfaceTile(tile.Location - new Point2I(0, 1));
						if (checkAboveTile != null && checkAboveTile.IsLadder)
							continue;
						yield return CollisionCheck.CreateTileCollision(tile, 0);
					}
				}
			}
			// Find room edges
			if (entity.Physics.CollideWithRoomEdge) {
				yield return CollisionCheck.CreateRoomEdgeCollision(
					entity.RoomControl);
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
				collision.CollisionBox = entity.Physics.GetCollisionBox(
					entity.Physics.RoomEdgeCollisionBoxType);
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

			// Determine the allowed penetration distance
			collision.AllowedPenetration = GetAllowedPenetration(entity, source);
			collision.AllowedLateralPenetration = collision.AllowedPenetration;

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
				if (a.IsTile && a.Tile.IsLadder) {
					a.Connections[Directions.Right] = true;
					a.Connections[Directions.Left] = true;
					a.Connections[Directions.Down] = true;
				}
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
		private int CalcCollisionDirection(Entity entity, Collision collision) {
			Rectangle2F solidBox = collision.SolidBox;
			Rectangle2F entityBox = Rectangle2F.Translate(
				collision.CollisionBox, entity.Position);
			
			// Calculate the penetration on each axis
			Vector2F penetrationBox;
			penetrationBox.X = GMath.Min(solidBox.Right - entityBox.Left,
				entityBox.Right - solidBox.Left);
			penetrationBox.Y = GMath.Min(solidBox.Bottom - entityBox.Top,
				entityBox.Bottom - solidBox.Top);
			
			// Get the collision direction for each axis
			int[] axisDirections = new int[2] { -1, -1 };
			for (int axis = 0; axis < 2; axis++) {
				axisDirections[axis] = Axes.GetDirection(axis,
					solidBox.Center[axis] - entityBox.Center[axis]);
				if (collision.Connections[(axisDirections[axis] + 2) % 4]) {
						axisDirections[axis] = -1;
				}
				else if (collision.Source.IsInsideCollision)
					axisDirections[axis] = Directions.Reverse(axisDirections[axis]);
			}
			if (axisDirections[0] == -1 && axisDirections[1] == -1)
				return -1;
			
			// Set the collision direction on the axis of least penetration,
			int collisionAxis = -1;
			if (axisDirections[Axes.X] >= 0 && axisDirections[Axes.Y] >= 0)
				collisionAxis = (penetrationBox.X < penetrationBox.Y ? Axes.X : Axes.Y);
			else if (axisDirections[Axes.X] >= 0)
				collisionAxis = Axes.X;
			else if (axisDirections[Axes.Y] >= 0)
				collisionAxis = Axes.Y;
			collision.Direction = axisDirections[collisionAxis];
			return collision.Direction;
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
				if (collision != null)
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
				entity.Physics.PotentialCollisions)
			{
				if (oppositeCollision.Direction == oppositeDirection &&
					oppositeCollision.Penetration <= 0.0f &&
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
			
			// Apply impulse to update velocity in the collision direction
			if (entity.Physics.Velocity.Dot(
				Directions.ToVector(collision.Direction)) > 0.0f)
			{
				Vector2F newVelocity = entity.Physics.Velocity;

				if (CanReboundColliison(entity, collision)) {
					collision.IsRebound = true;
					newVelocity[collision.Axis] *= -1.0f;
				}
				else {
					newVelocity[collision.Axis] = 0.0f;
				}

				entity.Physics.Velocity = newVelocity;
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
				if (IsSideScrolling && entity.Physics.HasGravity &&
					collision.Axis == Axes.X)
				{
					// Step up onto blocks if it is a small enough distance
					if (entity.Physics.PreviousStandingCollision != null)
						PerformSideScrollCollisionSnap(entity, collision);
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
					if (!IsCollidingAt(entity, nextPosition, dodgeDirection) &&
						!IsCollidingAt(entity, goalPosition, collision.Direction))
					{
						entity.Position += dodgeVector * moveAmount;
						collision.IsDodged = true;

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

		// Attempt to snap a collision.
		private bool PerformSideScrollCollisionSnap(Entity entity, Collision collision) {
			// Only dodge if moving perpendicular to the edge
			int axis = collision.Axis;
			int lateralAxis = Axes.GetOpposite(axis);
			Rectangle2F entityBox = Rectangle2F.Translate(
				collision.CollisionBox, entity.Position);

			// Check dodging for both edges of the solid object
			int dodgeDirection = Directions.Up;
			Vector2F dodgeVector = Directions.ToVector(dodgeDirection);
			float distanceToEdge = Math.Abs(entityBox.GetEdge(
				Directions.Reverse(dodgeDirection)) -
				collision.SolidBox.GetEdge(dodgeDirection));
				
			// Check if the distance to the edge is within dodge range
			if (distanceToEdge <= entity.Physics.AutoDodgeDistance) {
				Vector2F nextPosition = GMath.Round(entity.Position) +
					(dodgeVector * 1);
				Vector2F goalPosition = entity.Position +
					Directions.ToVector(collision.Direction) * 2 +
					(dodgeVector * distanceToEdge);

				// Make sure the entity is not colliding when placed at the solid
				// object's edge
				if (!IsCollidingAt(entity, nextPosition, dodgeDirection) &&
					!IsCollidingAt(entity, goalPosition, collision.Direction))
				{
					entity.Position = goalPosition;
					collision.IsDodged = true;

					// Change this collision into a downward collision
					collision.Direction = Directions.Down;
					collision.CalcPenetration();

					// Adjust and recalculate penetration for all collisions
					AdjustCollisionPenetrations(entity,
						distanceToEdge, dodgeDirection);
					AdjustCollisionPenetrations(entity, 2,
						Directions.Reverse(collision.Direction));
					return true; // Dodged successfully
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
				bool prevIsColliding = collision.IsColliding;

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
				//collision.CalcIsColliding();
				if (!collision.IsColliding && prevIsColliding)
					collision.IsResolved = true;
			}
		}

		private bool IsCollidingAt(Entity entity, Vector2F position, int direction) {
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
			foreach (Collision collision in entity.Physics.PotentialCollisions) {
				if (collision.Axis == axis && collision.IsSafeColliding) {
					if (best == null ||
						GetPriorityCollision(best, collision) == collision)
					{
						best = collision;
					}
				}
			}
			return best;
		}

		/// <summary>Return which of two collisions takes resolution priority. This
		/// does not check if the collisions are actually penetrating.</summary>
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

		/// <summary>Check if the entity is being crushed between two collisions.
		/// </summary>
		private void CheckCrush(Entity entity) {
			if (!entity.Physics.IsCrushable)
				return;
			
			// Iterate collisions which are able to crush the entity
			foreach (Collision rock in entity.Physics.Collisions) {
				if (CanColliisonCrush(entity, rock)) {
					int crushDirection = rock.Tile.MoveDirection;
					int crushAxis = Directions.ToAxis(crushDirection);

					// Iterate collisions which are caused by the crushing collision
					foreach (Collision hardPlace in entity.Physics.Collisions) {
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
		// Side Scrolling Ladder Collisions
		//-----------------------------------------------------------------------------

		// Check the player's side-scrolling ladder collisions.
		private void CheckPlayerLadderClimbing(Player player) {
			if (!IsSideScrolling)
				return;
			
			// Get the highest ladder the player is touching
			Rectangle2F climbBoxPrev = Rectangle2F.Translate(
				player.Movement.ClimbCollisionBox, player.Position);
			Tile ladder = GetHighestLadder(climbBoxPrev, player.PreviousPosition);
			
			// Check for beginning climbing
			if (ladder != null && !player.IsOnSideScrollLadder) {
				Rectangle2F ladderBox = ladder.Bounds;
				Rectangle2F collisionBox = player.Physics.PositionedCollisionBox;
				Rectangle2F climbBoxNext = Rectangle2F.Translate(
					player.Movement.ClimbCollisionBox,
					player.Position + player.Physics.Velocity);

				// Check if this ladder is a ladder-top
				Tile checkAboveTile = player.RoomControl.TileManager
					.GetSurfaceTile(ladder.Location - new Point2I(0, 1));
				bool isTopLadder = (checkAboveTile == null ||
					!checkAboveTile.IsLadder);

				// Check for beginning climbing by moving up while falling
				if ((player.Physics.VelocityY >= 0.0f || !player.Physics.HasGravity) &&
					!player.Physics.IsInWater &&
					player.Movement.IsMovingInDirection(Directions.Up) ||
					(player.Movement.IsMovingInDirection(Directions.Down) &&
						isTopLadder && player.Center.Y < ladderBox.Top))
				{
					player.BeginEnvironmentState(player.SideScrollLadderState);
					player.IntegrateStateParameters();
					player.Physics.VelocityY = 0.0f;
				}

				// Check for beginning climbing by stepping off of a solid object and
				// onto the ladder. Make sure the player is not on a flat surface
				// aligned with the ladder-top.
				else if (climbBoxNext.Intersects(ladderBox) &&
					player.Physics.PreviousStandingCollision != null &&
					player.Physics.StandingCollision == null &&
					player.Physics.VelocityY >= 0.0f &&
					(!isTopLadder || collisionBox.Bottom > ladderBox.Top ||
						player.Physics.PreviousStandingCollision
							.GetEdge(Directions.Down) != ladderBox.Top))
				{
					player.BeginEnvironmentState(player.SideScrollLadderState);
					player.IntegrateStateParameters();
					return;
				}
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
		
		
		//-----------------------------------------------------------------------------
		// Room Boundaries
		//-----------------------------------------------------------------------------

		/// <summary>Check if destroyed outside room.</summary>
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

		private void UpdateEntityZPosition(Entity entity) {
			if (IsSideScrolling) {
				// Convert any nonzero Z-position into Y-position
				if (entity.ZPosition != 0.0f) {
					entity.Y -= entity.ZPosition;
					entity.ZPosition = 0.0f;
				}
				
				if (entity.Physics.HasGravity && !entity.Physics.OnGroundOverride) {
					// Zero jump speed if the entity is colliding below
					if (entity.Physics.ZVelocity < 0.0f &&
						entity.Physics.IsCollidingInDirection(Directions.Down))
						entity.Physics.ZVelocity = 0.0f;
					// Integrate acceleration due to gravity
					entity.Physics.ZVelocity -= entity.Physics.Gravity;
					// Limit to maximum fall speed
					if (entity.Physics.MaxFallSpeed >= 0.0f &&
						entity.Physics.ZVelocity < -entity.Physics.MaxFallSpeed)
						entity.Physics.ZVelocity = -entity.Physics.MaxFallSpeed;
					// Convert the Z-velocity to Y-velocity
					entity.Physics.VelocityY = -entity.Physics.ZVelocity;
				}
				else {
					entity.Physics.ZVelocity = 0.0f;
				}
			}
			else if (entity.ZPosition > 0.0f || entity.Physics.ZVelocity != 0.0f) {
				// Integrate gravity acceleration
				if (entity.Physics.HasGravity && !entity.Physics.OnGroundOverride) {
					entity.Physics.ZVelocity -= entity.Physics.Gravity;
					if (entity.Physics.MaxFallSpeed >= 0.0f &&
						entity.Physics.ZVelocity < -entity.Physics.MaxFallSpeed)
						entity.Physics.ZVelocity = -entity.Physics.MaxFallSpeed;
				}

				// Integrate z-velocity
				entity.ZPosition += entity.Physics.ZVelocity;

				// Check if landed on the ground
				if (entity.ZPosition <= 0.0f)
					LandEntity(entity);
			}
			else
				entity.Physics.ZVelocity = 0.0f;
		}

		private void LandEntity(Entity entity) {
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
			if (IsSideScrolling &&
				entity.Physics.StandingCollision != null &&
				entity.Physics.StandingCollision.IsTileOrEntity)
			{
				// Get the velocity of the surface tile or entity
				if (entity.Physics.StandingCollision.IsTile) {
					entity.Physics.SurfacePosition =
						entity.Physics.StandingCollision.Tile.Position;
					entity.Physics.SurfaceVelocity =
						entity.Physics.StandingCollision.Tile.Velocity;
				}
				else {
					entity.Physics.SurfacePosition =
						entity.Physics.StandingCollision.Entity.Position;
					entity.Physics.SurfaceVelocity =
						entity.Physics.StandingCollision.Entity.Physics.Velocity;
				}

				// Move with the surface
				// TODO: this really needs some checks before execution
				entity.Y += entity.Physics.SurfaceVelocity.Y;
				entity.Physics.VelocityX += entity.Physics.SurfaceVelocity.X;

				// Move with conveyor tiles
				if (entity.Physics.StandingCollision.IsTile &&
					entity.Physics.MovesWithConveyors)
				{
					entity.Physics.VelocityX +=
						entity.Physics.StandingCollision.Tile.ConveyorVelocity.X;
				}
			}

			// Check if the surface is moving
			if (entity.Physics.TopTile != null) {
				if (entity.Physics.MovesWithPlatforms) {
					entity.Physics.Velocity += entity.Physics.TopTile.Velocity;
					if (!IsSideScrolling) {
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
		
		/// <summary>Check if the entity is sitting on a hazardous surface
		/// (water/lava/hole)</summary>
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
		
		/// <summary>Update ledge passing, handling changes in altitude.</summary>
		private void UpdateLedgePassing(Entity entity) {
			if (!entity.Physics.PassOverLedges)
				return;

			Point2I prevLocation = entity.Physics.LedgeTileLocation;
			entity.Physics.LedgeTileLocation = entity.RoomControl.GetTileLocation(
				entity.Position + entity.Physics.CollisionBox.Center);

			// When moving over a new tile, check its ledge state
			if (entity.Physics.LedgeTileLocation != prevLocation) {
				entity.Physics.LedgePassState = LedgePassState.None;
				entity.Physics.LedgePassTile = null;

				if (entity.RoomControl.IsTileInBounds(
					entity.Physics.LedgeTileLocation))
				{
					Tile tile = entity.RoomControl.GetTopTile(
						entity.Physics.LedgeTileLocation);
					
					if (tile != null && tile.IsAnyLedge) {
						// Adjust ledge altitude
						if (IsMovingUpLedge(entity, tile) &&
							entity.Physics.LedgeAltitude > 0)
						{
							entity.Physics.LedgeAltitude--;
							entity.Physics.LedgePassTile = tile;
							entity.Physics.LedgePassState = LedgePassState.PassingUp;
						}
						else if (IsMovingDownLedge(entity, tile)) {
							entity.Physics.LedgeAltitude++;
							entity.Physics.LedgePassTile = tile;
							entity.Physics.LedgePassState = LedgePassState.PassingDown;
						}
					}
				}
			}
		}

		/// <summary>Returns true if the entity moving down the ledge.</summary>
		private bool IsMovingDownLedge(Entity entity, Tile ledgeTile) {
			return entity.Physics.Velocity.Dot(
				Directions.ToVector(ledgeTile.LedgeDirection)) > 0.0f;
		}

		/// <summary>Returns true if the entity moving up the ledge.</summary>
		private bool IsMovingUpLedge(Entity entity, Tile ledgeTile) {
			return entity.Physics.Velocity.Dot(
				Directions.ToVector(ledgeTile.LedgeDirection)) < 0.0f;
		}

		
		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Get the reference to the room controller.</summary>
		public RoomControl RoomControl {
			get { return roomControl; }
		}

		/// <summary>Returns true if the current room is a side-scrolling room.
		/// </summary>
		public bool IsSideScrolling {
			get { return roomControl.IsSideScrolling; }
		}
	}
}
