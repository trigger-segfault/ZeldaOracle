using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Worlds;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Entities.Collisions;

namespace ZeldaOracle.Game.Entities.Collisions {
	/*
	public class TileCollisionTest {
		private Rectangle2F			entityCollisionBox;
		private Rectangle2F			tileCollisionBox;
		private CollisionBoxType	entityCollisionBoxType;
		private bool				useTileCollisionModel;
		
		
		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public TileCollisionTest(CollisionBoxType entityBoxType) {
			this.entityCollisionBoxType	= entityBoxType;
			this.useTileCollisionModel	= true;
		}
		
		public TileCollisionTest(Rectangle2F entityCollisionBox) {
			this.entityCollisionBoxType	= CollisionBoxType.Custom;
			this.entityCollisionBox		= entityCollisionBox;
			this.useTileCollisionModel	= true;
		}
		
		public TileCollisionTest(CollisionBoxType entityBoxType, Rectangle2F tileCollisionBox) {
			this.entityCollisionBoxType	= entityBoxType;
			this.useTileCollisionModel	= false;
			this.tileCollisionBox		= tileCollisionBox;
		}
		
		public TileCollisionTest(Rectangle2F entityCollisionBox, Rectangle2F tileCollisionBox) {
			this.entityCollisionBoxType	= CollisionBoxType.Custom;
			this.entityCollisionBox		= entityCollisionBox;
			this.useTileCollisionModel	= false;
			this.tileCollisionBox		= tileCollisionBox;
		}

		//-----------------------------------------------------------------------------
		// Collision Test
		//-----------------------------------------------------------------------------

		public CollisionInfo PerformCollisionTest(Entity entity, Tile tile) {
			CollisionInfo collisionInfo = new CollisionInfo();

			// Setup the entity's collision boxe.
			Rectangle2F entityBox = GetEntityCollisionBox(entity);
			entityBox.Point += entity.Position;

			// Iterate over the tiles collision boxes.
			int boxCount = GetTileCollisionBoxCount(tile);
			for (int i = 0; i < boxCount; i++) {
				// Setup the tile's collision boxe.
				Rectangle2F tileBox = GetTileCollisionBox(tile, i);
				tileBox.Point += tile.Position;
				
				// Perform the collision test.
				if (entityBox.Intersects(tileBox)) {
					collisionInfo.SetTileCollision(tile, 0);
				}
			}

			return collisionInfo;
		}


		//-----------------------------------------------------------------------------
		// Accessors
		//-----------------------------------------------------------------------------

		public Rectangle2F GetEntityCollisionBox(Entity entity) {
			if (entityCollisionBoxType == CollisionBoxType.Hard)
				return entity.Physics.CollisionBox;
			else if (entityCollisionBoxType == CollisionBoxType.Soft)
				return entity.Physics.SoftCollisionBox;
			return entityCollisionBox;
		}

		public int GetTileCollisionBoxCount(Tile tile) {
			if (useTileCollisionModel) {
				if (tile.CollisionModel == null)
					return 0;
				return tile.CollisionModel.BoxCount;
			}
			return 1;
		}

		public Rectangle2F GetTileCollisionBox(Tile tile, int boxIndex) {
			if (useTileCollisionModel)
				return tile.CollisionModel.Boxes[boxIndex];
			return tileCollisionBox;
		}
	}
	*/
	
	public class CollisionTestSettings {
		private Rectangle2F			collisionBox1;
		private Rectangle2F			collisionBox2;
		private CollisionBoxType	collisionBoxType1;
		private CollisionBoxType	collisionBoxType2;
		private PhysicsFlags		requiredFlags;
		private Type				requiredType;
		private float				maxZDistance;
		
		
		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public CollisionTestSettings() :
			this(typeof(Entity), CollisionBoxType.Soft)
		{
		}
		
		public CollisionTestSettings(Type entityType, CollisionBoxType collisionBoxType, float maxZDistance = 10) {
			this.requiredType		= entityType;
			this.collisionBoxType1	= collisionBoxType;
			this.collisionBoxType2	= collisionBoxType;
			this.requiredFlags		= PhysicsFlags.None;
			this.maxZDistance		= maxZDistance;
		}
		
		public CollisionTestSettings(Type entityType, CollisionBoxType myBoxType, CollisionBoxType otherBoxType, float maxZDistance = 10) {
			this.requiredType		= entityType;
			this.collisionBoxType1	= myBoxType;
			this.collisionBoxType2	= otherBoxType;
			this.requiredFlags		= PhysicsFlags.None;
			this.maxZDistance		= maxZDistance;
		}
		
		public CollisionTestSettings(Type entityType, Rectangle2F myBox, CollisionBoxType otherBoxType, float maxZDistance = 10) {
			this.requiredType		= entityType;
			this.collisionBox1		= myBox;
			this.collisionBoxType1	= CollisionBoxType.Custom;
			this.collisionBoxType2	= otherBoxType;
			this.requiredFlags		= PhysicsFlags.None;
			this.maxZDistance		= maxZDistance;
		}
		
		public CollisionTestSettings(Type entityType, Rectangle2F myBox, Rectangle2F otherBox, float maxZDistance = 10) {
			this.requiredType		= entityType;
			this.collisionBox1		= myBox;
			this.collisionBox2		= otherBox;
			this.collisionBoxType1	= CollisionBoxType.Custom;
			this.collisionBoxType2	= CollisionBoxType.Custom;
			this.requiredFlags		= PhysicsFlags.None;
			this.maxZDistance		= maxZDistance;
		}

		public CollisionTestSettings(Type entityType, Rectangle2F myBox, CollisionBoxType otherBoxType) {
			this.requiredType       = entityType;
			this.collisionBox1      = myBox;
			this.collisionBoxType1  = CollisionBoxType.Custom;
			this.collisionBoxType2  = otherBoxType;
			this.requiredFlags      = PhysicsFlags.None;
			this.maxZDistance       = 0f;
		}


		//-----------------------------------------------------------------------------
		// Accessors
		//-----------------------------------------------------------------------------

		public Rectangle2F GetCollisionBox1(Entity entity1) {
			if (collisionBoxType1 == CollisionBoxType.Hard)
				return entity1.Physics.CollisionBox;
			else if (collisionBoxType1 == CollisionBoxType.Soft)
				return entity1.Physics.SoftCollisionBox;
			return collisionBox1;
		}

		public Rectangle2F GetCollisionBox2(Entity entity2) {
			if (collisionBoxType2 == CollisionBoxType.Hard)
				return entity2.Physics.CollisionBox;
			else if (collisionBoxType2 == CollisionBoxType.Soft)
				return entity2.Physics.SoftCollisionBox;
			return collisionBox1;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public Type RequiredType {
			get { return requiredType; }
			set { requiredType = value; }
		}

		public PhysicsFlags RequiredFlags {
			get { return requiredFlags; }
			set { requiredFlags = value; }
		}

		public float MaxZDistance {
			get { return maxZDistance; }
			set { maxZDistance = value; }
		}

		public Rectangle2F CollisionBox1 {
			get { return collisionBox1; }
			set { collisionBox1 = value; }
		}
		
		public Rectangle2F CollisionBox2 {
			get { return collisionBox2; }
			set { collisionBox2 = value; }
		}

		public CollisionBoxType CollisionBoxType1 {
			get { return collisionBoxType1; }
			set { collisionBoxType1 = value; }
		}
		
		public CollisionBoxType CollisionBoxType2 {
			get { return collisionBoxType2; }
			set { collisionBoxType2 = value; }
		}
	}
	
	public class CollisionTest {

		//-----------------------------------------------------------------------------
		// Static Collision Test Methods
		//-----------------------------------------------------------------------------

		// Perform a collision test between two entities.
		public static CollisionInfo PerformCollisionTest(Entity entity, Entity other, CollisionTestSettings settings) {
			CollisionInfo collisionInfo = new CollisionInfo();

			// Check that the other entity meets the collision requirements.
			if ((entity != other) &&
				(settings.RequiredType == null	|| settings.RequiredType.IsAssignableFrom(other.GetType())) &&
				(settings.RequiredFlags == 0	|| other.Physics.Flags.HasFlag(settings.RequiredFlags)) &&
				(settings.MaxZDistance < 0.0f	|| GMath.Abs(entity.ZPosition - other.ZPosition) <= settings.MaxZDistance))
			{
				// Setup the collision boxes.
				Rectangle2F cbox1 = settings.GetCollisionBox1(entity);
				cbox1.Point += entity.Position;
				Rectangle2F cbox2 = settings.GetCollisionBox2(other);
				cbox2.Point += other.Position;
				
				// Perform the collision test.
				if (cbox1.Intersects(cbox2)) {
					collisionInfo.SetEntityCollision(other, 0);
				}
			}

			return collisionInfo;
		}

		// Perform a collision test between a collision box and an entity.
		public static CollisionInfo PerformCollisionTest(Vector2F position, Entity other, CollisionTestSettings settings) {
			CollisionInfo collisionInfo = new CollisionInfo();

			// Check that the other entity meets the collision requirements.
			if ((settings.RequiredType == null  || settings.RequiredType.IsAssignableFrom(other.GetType())) &&
				(settings.RequiredFlags == 0    || other.Physics.Flags.HasFlag(settings.RequiredFlags))) {
				// Setup the collision boxes.
				Rectangle2F cbox1 = settings.CollisionBox1;
				cbox1.Point += position;
				Rectangle2F cbox2 = settings.GetCollisionBox2(other);
				cbox2.Point += other.Position;

				// Perform the collision test.
				if (cbox1.Intersects(cbox2)) {
					collisionInfo.SetEntityCollision(other, 0);
				}
			}

			return collisionInfo;
		}
	}
	/*
	public class CollisionIterator {
		private Entity					entity;
		private CollisionTestSettings	settings;
		private int						index;
		private CollisionInfo			collisionInfo;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public CollisionIterator(Entity entity, CollisionTestSettings settings) {
			this.entity		= entity;
			this.settings	= settings;
			this.index		= -1;
		}
		
		public CollisionIterator(Entity entity, Type entityType, CollisionBoxType collisionBoxType, float maxZDistance = 10) :
			this(entity, new CollisionTestSettings(entityType, collisionBoxType, maxZDistance))
		{
		}
		
		public CollisionIterator(Entity entity, Type entityType, CollisionBoxType myBoxType, CollisionBoxType otherBoxType, float maxZDistance = 10) :
			this(entity, new CollisionTestSettings(entityType, myBoxType, otherBoxType, maxZDistance))
		{
		}
		
		public CollisionIterator(Entity entity, Type entityType, Rectangle2F myBox, CollisionBoxType otherBoxType, float maxZDistance = 10) :
			this(entity, new CollisionTestSettings(entityType, myBox, otherBoxType, maxZDistance))
		{
		}
		
		public CollisionIterator(Entity entity, Type entityType, Rectangle2F myBox, Rectangle2F otherBox, float maxZDistance = 10) :
			this(entity, new CollisionTestSettings(entityType, myBox, otherBox, maxZDistance))
		{
		}


		//-----------------------------------------------------------------------------
		// Iteration
		//-----------------------------------------------------------------------------
		
		public void Begin(Entity entity) {
			this.entity = entity;
			Next();
		}

		public void Begin() {
			index = -1;
			Next();
		}

		public void Next() {
			// Find the index of the next collision.
			CollisionInfo info;
			for (int i = index + 1; i < entity.RoomControl.EntityCount; i++) {
				info = CollisionTest.PerformCollisionTest(entity, entity.RoomControl.Entities[i], settings);
				if (info.IsColliding) {
					index = i;
					collisionInfo = info;
					return;
				}
			}
			index = -1;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------
		
		public bool IsGood() {
			return (index >= 0);
		}
		
		public CollisionInfo CollisionInfo {
			get { return collisionInfo; }
		}
	}

	
	
	public class TileCollisionIterator {
		private Entity				entity;
		private TileCollisionTest	collisionTest;
		private Point2I				location;
		private int					layer;
		private CollisionInfo		collisionInfo;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public TileCollisionIterator(Entity entity, TileCollisionTest collisionTest) {
			this.entity			= entity;
			this.collisionTest	= collisionTest;
			this.location		= new Point2I();
			this.layer			= 0;
		}


		//-----------------------------------------------------------------------------
		// Iteration
		//-----------------------------------------------------------------------------
		
		public void Begin(Entity entity) {
			this.entity = entity;
			location = new Point2I(-1, 0);
			layer = 0;
			Next();
		}

		public void Begin() {
			location = new Point2I(-1, 0);
			layer = 0;
			Next();
		}

		public void Next() {
			Tile tile = null;
			while (tile == null) {
				if (!NextLocation())
					break;
				tile = entity.RoomControl.GetTile(location, layer);
				if (tile != null) {
					collisionInfo = collisionTest.PerformCollisionTest(entity, tile);
					if (!collisionInfo.IsColliding)
						tile = null;
				}
			}
		}

		private bool NextLocation() {
			location.X++;
			if (location.X >= entity.RoomControl.Room.Width) {
				location.X = 0;
				location.Y++;
				if (location.Y >= entity.RoomControl.Room.Height) {
					location.Y = 0;
					layer++;
					if (layer >= entity.RoomControl.Room.LayerCount) {
						// Out of bounds!
						return false;
					}
				}
			}
			return true;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------
		
		public bool IsGood() {
			return (entity.RoomControl.IsTileInBounds(location, layer));
		}
		
		public CollisionInfo CollisionInfo {
			get { return collisionInfo; }
		}
	}
	*/
}
