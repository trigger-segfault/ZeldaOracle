using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Control;
using ZeldaOracle.Game.Tiles;

namespace ZeldaOracle.Game.Entities.Collisions {

	public struct CollisionCheck {

		private CollisionType type;

		private object solidObject;

		private int collisionBoxIndex;

		private CollisionBoxType entityCollisionBoxType;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public CollisionCheck(CollisionType type,
			object solidObject = null, int collisionBoxIndex = 0,
			CollisionBoxType entityCollisionBoxType = CollisionBoxType.Hard)
		{
			this.type				= type;
			this.solidObject		= solidObject;
			this.collisionBoxIndex	= collisionBoxIndex;
			this.entityCollisionBoxType	= entityCollisionBoxType;
		}


		//-----------------------------------------------------------------------------
		// Factory Functions
		//-----------------------------------------------------------------------------

		public static CollisionCheck CreateTileCollision(Tile tile,
			int collisionBoxIndex)
		{
			return new CollisionCheck(CollisionType.Tile , tile, collisionBoxIndex);
		}

		public static CollisionCheck CreateEntityCollision(Entity entity) {
			return new CollisionCheck(CollisionType.Entity, entity);
		}

		public static CollisionCheck CreateRoomEdgeCollision(RoomControl roomControl) {
			return new CollisionCheck(CollisionType.RoomEdge, roomControl, 0,
				CollisionBoxType.Soft);
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public object SolidObject {
			get { return solidObject; }
			set { solidObject = value; }
		}

		public int CollisionBoxIndex {
			get { return collisionBoxIndex; }
			set { collisionBoxIndex = value; }
		}

		public CollisionBoxType EntityCollisionBoxType {
			get { return entityCollisionBoxType; }
			set { entityCollisionBoxType = value; }
		}

		public CollisionType Type {
			get { return type; }
			set { type = value; }
		}

		public bool IsEntity {
			get { return (type == CollisionType.Entity); }
		}
		
		public bool IsTile {
			get { return (type == CollisionType.Tile); }
		}
		
		public bool IsRoomEdge {
			get { return (type == CollisionType.RoomEdge); }
		}

		public Entity Entity {
			get { return SolidObject as Entity; }
		}
		
		public Tile Tile {
			get { return SolidObject as Tile; }
		}

		public Rectangle2F SolidBox {
			get {
				if (type == CollisionType.RoomEdge)
					return (Rectangle2F) ((RoomControl) SolidObject).RoomBounds;
				else if (type == CollisionType.Entity)
					return ((Entity) SolidObject).Physics.PositionedCollisionBox;
				else if (type == CollisionType.Tile)
					return Rectangle2F.Translate(
						((Tile) SolidObject).CollisionModel.Boxes[CollisionBoxIndex],
						((Tile) SolidObject).Position);
				return Rectangle2F.Zero;
			}
		}

		public bool IsInsideCollision {
			get { return type == CollisionType.RoomEdge; }
		}
	}



	

	public class Collision {

		private Entity physicsEntity;
		private Rectangle2F collisionBox;

		private CollisionCheck source;
		private Rectangle2F solidBox;

		private int direction;
		private bool isAutoDodged;
		private bool isMovementCollision;
		private bool isResolved;
		private bool isColliding;
		private bool isResolvable;
		private float penetration;
		private float lateralPenetration;
		private float allowedPenetration;
		private bool[] connections;
		private bool[] allowableDirections;
		private bool isDynamic;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public Collision(Entity physicsEntity, CollisionCheck source) {
			this.source			= source;
			this.physicsEntity	= physicsEntity;
			this.collisionBox	= physicsEntity.Physics.CollisionBox;
			direction			= 0;
			isAutoDodged		= false;
			isMovementCollision	= false;
			isResolved			= false;
			isColliding			= false;
			penetration			= 0.0f;
			lateralPenetration	= 0.0f;
			allowedPenetration	= 0.0f;
			isResolvable		= true;
			connections			= new bool[] { false, false, false, false };
			allowableDirections = new bool[] { false, false, false, false };
			isDynamic			= false;
		}


		//-----------------------------------------------------------------------------
		// Calculations
		//-----------------------------------------------------------------------------

		public float GetEdge(int direction) {
			if (source.IsInsideCollision)
				return SolidBox.GetEdge(direction);
			else
				return SolidBox.GetEdge(Directions.Reverse(direction));
		}

		public bool IsCollidingAt(Vector2F position) {
			// Check for penetration on both axes
			float p = CalcPenetration(position);
			float l = CalcLateralPenetration(position);
			return (CalcPenetration(position) >
					allowedPenetration + GameSettings.EPSILON &&
				CalcLateralPenetration(position) >
					allowedPenetration + GameSettings.EPSILON);
		}
		

		public void CalcPenetration() {
			penetration = CalcPenetration(physicsEntity.Position);
			lateralPenetration = CalcLateralPenetration(physicsEntity.Position);
		}

		public void CalcLateralPenetration() {
			lateralPenetration = CalcLateralPenetration(physicsEntity.Position);
		}

		public void CalcIsColliding() {
			isColliding = (penetration > allowedPenetration + GameSettings.EPSILON &&
				lateralPenetration > allowedPenetration + GameSettings.EPSILON);
			//isColliding = (penetration > GameSettings.EPSILON &&
			//lateralPenetration > allowedPenetration + GameSettings.EPSILON);
		}

		public float CalcLateralPenetration(Vector2F entityPosition) {
			// Check for separation on the lateral axis
			int lateralAxis = Axes.GetOpposite(Axis);
			Rectangle2F entityBox = Rectangle2F.Translate(
				collisionBox, entityPosition);
			float lateralPenetration = GMath.Min(
				solidBox.BottomRight[lateralAxis] - entityBox.TopLeft[lateralAxis],
				entityBox.BottomRight[lateralAxis] - solidBox.TopLeft[lateralAxis]);
			return lateralPenetration;
		}

		public float CalcPenetration(Vector2F entityPosition) {
			Rectangle2F entityBox = Rectangle2F.Translate(
				collisionBox, entityPosition);
			return (entityBox.GetEdge(direction) - GetEdge(direction)) *
				Directions.ToVector(direction)[Axis];
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------
		
		public bool IsResolvable {
			get { return isResolvable; }
			set { isResolvable = value; }
		}

		public CollisionCheck Source {
			get { return source; }
			set { source = value; }
		}

		public CollisionType Type {
			get { return source.Type; }
		}
		
		public Rectangle2F SolidBox {
			get { return solidBox; }
			set { solidBox = value; }
		}
		
		public Rectangle2F CollisionBox {
			get { return collisionBox; }
			set { collisionBox = value; }
		}
		
		public bool IsDynamic {
			get { return isDynamic; }
			set { isDynamic = value; }
		}

		public bool IsColliding {
			/*get {
				// Check for separation on both axes
				return (penetration > GameSettings.EPSILON &&
					CalcLateralPenetration(physicsEntity.Position) >
						allowedPenetration + GameSettings.EPSILON);
			}*/
			get { return (penetration > allowedPenetration + GameSettings.EPSILON &&
				lateralPenetration > allowedPenetration + GameSettings.EPSILON); }
			//get { return isColliding; }
		}

		public bool IsSafeColliding {
			get { return (penetration > GameSettings.EPSILON &&
				lateralPenetration > allowedPenetration + GameSettings.EPSILON); }
		}

		public bool IsAutoDodged {
			get { return isAutoDodged; }
			set { isAutoDodged = value; }
		}
		
		public int Direction {
			get { return direction; }
			set { direction = value; }
		}
		
		public bool IsResolved {
			get { return isResolved; }
			set { isResolved = value; }
		}
		
		public bool IsMovementCollision {
			get { return isMovementCollision; }
			set { isMovementCollision = value; }
		}
		
		public float Penetration {
			get { return penetration; }
			set { penetration = value; }
		}
		
		public float AllowedPenetration {
			get { return allowedPenetration; }
			set { allowedPenetration = value; }
		}
		
		public bool[] Connections {
			get { return connections; }
		}
		
		public bool[] AllowableDirections {
			get { return allowableDirections; }
		}
		
		public object SolidObject {
			get { return source.SolidObject; }
		}
		
		public Entity Entity {
			get { return source.Entity; }
		}
		
		public Tile Tile {
			get { return source.Tile; }
		}
		
		public bool IsTile {
			get { return source.Type == CollisionType.Tile; }
		}
		
		public bool IsEntity {
			get { return source.Type == CollisionType.Entity; }
		}
		
		public bool IsRoomEdge {
			get { return source.Type == CollisionType.RoomEdge; }
		}
		
		public int Axis {
			get { return Directions.ToAxis(direction); }
		}
		
		public bool IsHorizontal {
			get { return Directions.IsHorizontal(direction); }
		}
		
		public bool IsVertical {
			get { return Directions.IsVertical(direction); }
		}

		public bool IsCollidingAndNotAutoDodged {
			get { return (IsColliding && !isAutoDodged); }
		}
		
		public bool IsCollidingAndNotAllowedClipping {
			get { return (IsColliding && penetration > allowedPenetration); }
		}
		
		public bool IsAllowedClipping {
			get { return (IsColliding && penetration <= allowedPenetration); }
		}
	}










	public class CollisionInfoNew {

		public Entity Entity { get; set; }

		public int PenetrationDirection { get; set; }
		public float PenetrationDistance { get; set; }
		public Rectangle2F CollisionBox { get; set; }
		public object CollidedObject { get; set; }
		public bool IsColliding { get; set; }
		public bool IsResolved { get; set; }
		public bool IsResolvable { get; set; }
		public bool IsValidCollisionInfo { get; set; }
		public float MaxAllowedPenetrationDistance { get; set; }
		public CollisionCheck Source { get; set; }

		public bool IsAllowedClipping {
			get { return (IsColliding && PenetrationDistance <= MaxAllowedPenetrationDistance); }
		}
		
		public bool IsCollidingAndNotAllowedClipping {
			get { return (IsColliding && PenetrationDistance > MaxAllowedPenetrationDistance); }
		}

		public CollisionInfoNew() {
			Reset();
		}

		public CollisionInfoNew(CollisionInfoNew copy) {
			this.CollisionBox = copy.CollisionBox;
			this.Entity = copy.Entity;
			this.CollidedObject = copy.CollidedObject;
		}

		public void Reset() {
			Entity					= null;
			IsColliding				= false;
			IsResolved				= false;
			PenetrationDistance		= 0.0f;
			CollidedObject			= null;
			CollisionBox			= Rectangle2F.Zero;
			PenetrationDirection	= -1;
			MaxAllowedPenetrationDistance	= 0.0f;
			IsValidCollisionInfo	= false;
		}
	}

	public enum CollisionType {
		None,
		Tile,
		Entity,
		RoomEdge,
	}

	public class CollisionInfo {

		// The type of collision if not None (entity, tile or room-edge)
		private CollisionType type;

		// The object we are colliding with. This is null for room-edge collisions.
		private object solidObject;
		
		// The direction of the collision impact (from the entity's perspective).
		private int direction;
		
		private bool isAutoDodged;

		private bool isMovementCollision;
		private bool isResolved;

		private float penetration;
		private Rectangle2F solidBox;
		private CollisionCheck source;

		
		//-----------------------------------------------------------------------------
		// Mutators
		//-----------------------------------------------------------------------------
		
		public void Clear() {
			type			= CollisionType.None;
			solidObject		= null;
			direction		= Directions.Right;
			isAutoDodged	= false;
			isMovementCollision	= false;
			isResolved			= false;
			penetration			= 0.0f;
		}

		public void SetTileCollision(Tile tile, int direction) {
			this.type			= CollisionType.Tile;
			this.solidObject	= tile;
			this.direction		= direction;
		}
		
		public void SetEntityCollision(Entity entity, int direction) {
			this.type			= CollisionType.Entity;
			this.solidObject	= entity;
			this.direction		= direction;
		}
		
		public void SetRoomEdgeCollision(int direction) {
			this.type			= CollisionType.RoomEdge;
			this.solidObject	= null;
			this.direction		= direction;
		}
		
		public void SetCollision(object obj, int direction) {
			this.type			= CollisionType.None;
			this.solidObject	= obj;
			this.direction		= direction;
			if (obj is Tile)
				this.type = CollisionType.Tile;
			else if (obj is Entity)
				this.type = CollisionType.Entity;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public bool IsColliding {
			get { return (type != CollisionType.None); }
		}

		public bool IsAutoDodged {
			get { return isAutoDodged; }
			set { isAutoDodged = value; }
		}

		public bool IsCollidingAndNotAutoDodged {
			get { return (IsColliding && !isAutoDodged); }
		}
		
		public int Axis {
			get { return Directions.ToAxis(direction); }
		}
		
		public int Direction {
			get { return direction; }
			set { direction = value; }
		}

		public CollisionType Type {
			get { return type; }
			set { type = value; }
		}
		
		public object SolidObject {
			get { return solidObject; }
			set { solidObject = value; }
		}
		
		public Entity Entity {
			get { return (type == CollisionType.Entity ? (Entity) solidObject : null); }
		}
		
		public Tile Tile {
			get { return (type == CollisionType.Tile ? (Tile) solidObject : null); }
		}
		
		public bool IsResolved {
			get { return isResolved; }
			set { isResolved = value; }
		}
		
		public bool IsMovementCollision {
			get { return isMovementCollision; }
			set { isMovementCollision = value; }
		}
		
		public float Penetration {
			get { return penetration; }
			set { penetration = value; }
		}
		
		public Rectangle2F SolidBox {
			get { return solidBox; }
			set { solidBox = value; }
		}
		
		public CollisionCheck Source {
			get { return source; }
			set { source = value; }
		}
	}
}
