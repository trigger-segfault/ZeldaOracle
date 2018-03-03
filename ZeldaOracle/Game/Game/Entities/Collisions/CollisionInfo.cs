using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Control;
using ZeldaOracle.Game.Tiles;

namespace ZeldaOracle.Game.Entities.Collisions {

	/// <summary>Types of solid objects that can cause collisions.</summary>
	public enum CollisionType {
		None = -1,
		Tile,
		Entity,
		RoomEdge,
	}


	/// <summary>Stores the colliding features.</summary>
	public struct CollisionCheck {

		private CollisionType type;
		private Entity physicsEntity;
		private object solidObject;
		private int collisionBoxIndex;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public CollisionCheck(Entity physicsEntity, CollisionType type,
			object solidObject, int collisionBoxIndex)
		{
			this.type				= type;
			this.physicsEntity		= physicsEntity;
			this.solidObject		= solidObject;
			this.collisionBoxIndex	= collisionBoxIndex;
		}


		//-----------------------------------------------------------------------------
		// Factory Functions
		//-----------------------------------------------------------------------------

		public static CollisionCheck CreateTileCollision(
			Entity physicsEntity, Tile tile, int collisionBoxIndex)
		{
			return new CollisionCheck(physicsEntity, CollisionType.Tile,
				tile, collisionBoxIndex);
		}

		public static CollisionCheck CreateEntityCollision(
			Entity physicsEntity, Entity entity)
		{
			return new CollisionCheck(physicsEntity, CollisionType.Entity,
				entity, 0);
		}

		public static CollisionCheck CreateRoomEdgeCollision(
			Entity physicsEntity)
			{
			return new CollisionCheck(physicsEntity, CollisionType.RoomEdge,
				physicsEntity.RoomControl, 0);
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

		public bool IsInsideCollision {
			get { return (type == CollisionType.RoomEdge); }
		}

		public Entity Entity {
			get { return (SolidObject as Entity); }
		}
		
		public Tile Tile {
			get { return (SolidObject as Tile); }
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
	}

	
	/// <summary>Information about a collision between an entity and a solid object.
	/// </summary>
	public class Collision {

		private Entity physicsEntity;
		private Rectangle2F collisionBox;

		private CollisionCheck source;
		private Rectangle2F solidBox;
		private bool isDynamic;
		private bool[] connections;

		//private int direction;
		private Direction direction;
		private float initialPenetration;
		private float penetration;
		private float allowedPenetration;
		private float lateralPenetration;
		private float allowedLateralPenetration;

		private bool isResolved;
		private bool isDodged;
		private bool isRebound;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public Collision(Entity physicsEntity, CollisionCheck source) {
			this.source			= source;
			this.physicsEntity	= physicsEntity;
			this.collisionBox	= physicsEntity.Physics.CollisionBox;
			direction			= Direction.Right;
			isDodged			= false;
			isRebound			= false;
			isResolved			= false;
			penetration			= 0.0f;
			lateralPenetration	= 0.0f;
			allowedLateralPenetration = 0.0f;
			allowedPenetration	= 0.0f;
			connections			= new bool[] { false, false, false, false };
			isDynamic			= false;
		}


		//-----------------------------------------------------------------------------
		// Calculations
		//-----------------------------------------------------------------------------

		/// <summary>Return the position along the axis of the collision-face for a
		/// collision with this solid object in the given direction.</summary>
		public float GetEdge(int direction) {
			if (source.IsInsideCollision)
				return SolidBox.GetEdge(direction);
			else
				return SolidBox.GetEdge(Directions.Reverse(direction));
		}
		
		/// <summary>Calculate the perpendicular and latera penetration distances.
		/// </summary>
		public void CalcPenetration() {
			bool prevIsColliding = IsColliding;
			penetration = CalcPenetration(physicsEntity.Position);
			lateralPenetration = CalcLateralPenetration(physicsEntity.Position);
			if (prevIsColliding && !IsColliding)
				isResolved = true;
		}
		
		/// <summary>Calculate the lateral penetration distance.</summary>
		public void CalcLateralPenetration() {
			bool prevIsColliding = IsColliding;
			lateralPenetration = CalcLateralPenetration(physicsEntity.Position);
			if (prevIsColliding && !IsColliding)
				isResolved = true;
		}

		public void CalcIsColliding() {
			//bool newIsColliding = (penetration >
			//	allowedPenetration + GameSettings.EPSILON &&
			//	lateralPenetration > allowedLateralPenetration + GameSettings.EPSILON);
			//if (isColliding && !newIsColliding)
			//	isResolved = true;
			//isColliding = newIsColliding;
		}
		
		/// <summary>Returns true if this collision would be penetrating if the physics
		/// entity were placed at the given position.</summary>
		public bool IsCollidingAt(Vector2F position) {
			// Check for penetration on both axes
			float p = CalcPenetration(position);
			float l = CalcLateralPenetration(position);
			return (CalcPenetration(position) >
					allowedPenetration + GameSettings.EPSILON &&
				CalcLateralPenetration(position) >
					allowedLateralPenetration + GameSettings.EPSILON);
		}
		
		/// <summary>Calculate the lateral penetration distance if the physics entity
		/// were placed at the given position.</summary>
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

		/// <summary>Calculate the penetration distance if the physics entity were
		/// placed at the given position.</summary>
		public float CalcPenetration(Vector2F entityPosition) {
			Rectangle2F entityBox = Rectangle2F.Translate(
				collisionBox, entityPosition);
			return (entityBox.GetEdge(direction) - GetEdge(direction)) *
				Directions.ToVector(direction)[Axis];
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------
		
		/// <summary>The colliding features.</summary>
		public CollisionCheck Source {
			get { return source; }
			set { source = value; }
		}

		/// <summary>The type of collision.</summary>
		public CollisionType Type {
			get { return source.Type; }
		}
		
		/// <summary>True if this collision is with a solid tile.</summary>
		public bool IsTile {
			get { return (source.Type == CollisionType.Tile); }
		}
		
		/// <summary>True if this collision is with a solid entity.</summary>
		public bool IsEntity {
			get { return (source.Type == CollisionType.Entity); }
		}
		
		/// <summary>True if this collision is with a solid tile or entity.</summary>
		public bool IsTileOrEntity {
			get {
				return (source.Type == CollisionType.Tile ||
					source.Type == CollisionType.Entity);
			}
		}
		
		/// <summary>True if this collision is with the edge of the room.</summary>
		public bool IsRoomEdge {
			get { return (source.Type == CollisionType.RoomEdge); }
		}
		
		/// <summary>Get the solid object that the physics entity is colliding with.
		/// </summary>
		public object SolidObject {
			get { return source.SolidObject; }
		}
		
		/// <summary>The solid tile that the entity is colliding with. If this is not a
		/// tile collision, than this will return null.</summary>
		public Tile Tile {
			get { return source.Tile; }
		}
		
		/// <summary>The solid entity that the entity is colliding with. If this is not
		/// a entity collision, than this will return null.</summary>
		public Entity Entity {
			get { return source.Entity; }
		}
		
		/// <summary>True if the entity is colliding with a dynamic object, such as a
		/// moving tile.</summary>
		public bool IsDynamic {
			get { return isDynamic; }
			set { isDynamic = value; }
		}
		
		/// <summary>The solid objects positioned collision box.<summary>
		public Rectangle2F SolidBox {
			get { return solidBox; }
			set { solidBox = value; }
		}
		
		/// <summary>The physics entity's collision box that is used to check
		/// penetration.</summary>
		public Rectangle2F CollisionBox {
			get { return collisionBox; }
			set { collisionBox = value; }
		}

		/// <summary>The direction of penetration, which is typically the nearest
		/// direction from the player to the solid object.</summary>
		public Direction Direction {
			get { return direction; }
			set { direction = value; }
		}
		
		/// <summary>The axis of collision.</summary>
		public int Axis {
			get { return direction.Axis; }
		}
		
		/// <summary>True if the collision is happening on the X-axis.</summary>
		public bool IsHorizontal {
			get { return direction.IsHorizontal; }
		}
		
		/// <summary>True if the collision is happening on the Y-axis.</summary>
		public bool IsVertical {
			get { return direction.IsVertical; }
		}

		/// <summary>True if this collision is penetrating beyond the allowed
		/// penetration distance.</summary>
		public bool IsColliding {
			get {
				return (penetration > allowedPenetration + GameSettings.EPSILON &&
					IsLaterallyColliding);
			}
		}

		/// <summary>True if this collision is penetrating at all.</summary>
		public bool IsSafeColliding {
			get {
				return (penetration > GameSettings.EPSILON &&
					IsLaterallyColliding);
			}
		}

		/// <summary>True if the physics entity's collision box is overlapping the
		/// solid objects collision box on the axes parallel to the collision.
		/// </summary>
		public bool IsLaterallyColliding {
			get {
				return (lateralPenetration >
					allowedLateralPenetration + GameSettings.EPSILON);
			}
		}

		/// <summary>True if this collision was dodged.</summary>
		public bool IsDodged {
			get { return isDodged; }
			set { isDodged = value; }
		}
		
		/// <summary>True if this collision was resolved during physics processing.
		/// </summary>
		public bool IsResolved {
			get { return isResolved; }
			set { isResolved = value; }
		}
		
		/// <summary>Get whether this collision was rebounded off of.</summary>
		public bool IsRebound {
			get { return isRebound; }
			set { isRebound = value; }
		}
		
		/// <summary>The penetration distance perpendicular to the collision.</summary>
		public float Penetration {
			get { return penetration; }
			set { penetration = value; }
		}
		
		/// <summary>The penetration distance perpendicular to the collision, which was
		/// detected before resolving any collisions.</summary>
		public float InitialPenetration {
			get { return initialPenetration; }
			set { initialPenetration = value; }
		}
		
		/// <summary>The maximum allowable perpendicular penetration distance.
		/// </summary>
		public float AllowedPenetration {
			get { return allowedPenetration; }
			set { allowedPenetration = value; }
		}
				
		/// <summary>The maximum allowable parallel penetration distance.</summary>
		public float AllowedLateralPenetration {
			get { return allowedLateralPenetration; }
			set { allowedLateralPenetration = value; }
		}
		
		/// <summary>Array of connections in each direction, where true represents that
		/// this solid object shares an edge with an adjacent solid objects.</summary>
		public bool[] Connections {
			get { return connections; }
		}
	}
}
