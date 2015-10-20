using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Worlds;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Entities.Collisions;

namespace ZeldaOracle.Game.Entities.Collisions {
	
	
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

		// Perform a collision test.
		public static CollisionInfo PerformCollisionTest(Entity entity, Entity other, CollisionTestSettings settings) {
			CollisionInfo collisionInfo = new CollisionInfo();

			// Check that the other entity meets the collision requirements.
			if ((entity != other) &&
				(settings.RequiredType == null	|| settings.RequiredType.IsAssignableFrom(other.GetType())) &&
				(settings.RequiredFlags == 0	|| other.Physics.Flags.HasFlag(settings.RequiredFlags)) &&
				(settings.MaxZDistance < 0.0f	|| Math.Abs(entity.ZPosition - other.ZPosition) <= settings.MaxZDistance))
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
	}
	
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
			for (int i = index + 1; i < entity.RoomControl.Entities.Count; i++) {
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

}
