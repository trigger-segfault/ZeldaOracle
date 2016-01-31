using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Tiles;

namespace ZeldaOracle.Game.Entities.Collisions {

	public class CollisionInfoNew {

		public int PenetrationDirection { get; set; }
		public float PenetrationDistance { get; set; }
		public Rectangle2F CollisionBox { get; set; }
		public object CollidedObject { get; set; }
		public bool IsColliding { get; set; }
		public bool IsResolved { get; set; }
		
		public bool IsResolvable { get; set; }

		public bool IsValidCollisionInfo { get; set; }
		
		public float MaxAllowedPenetrationDistance { get; set; }


		public bool IsAllowedClipping {
			get { return (IsColliding && PenetrationDistance <= MaxAllowedPenetrationDistance); }
		}
		
		public bool IsCollidingAndNotAllowedClipping {
			get { return (IsColliding && PenetrationDistance > MaxAllowedPenetrationDistance); }
		}

		public CollisionInfoNew() {
			Reset();
		}

		public void Reset() {
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

	public struct CollisionInfo {

		// The type of collision if not None (entity, tile or room-edge)
		private CollisionType type;

		// The object we are colliding with. This is null for room-edge collisions.
		private object solidObject;
		
		// The direction of the collision impact (from the entity's perspective).
		private int direction;
		
		
		//-----------------------------------------------------------------------------
		// Mutators
		//-----------------------------------------------------------------------------
		
		public void Clear() {
			type		= CollisionType.None;
			solidObject	= null;
			direction	= Directions.Right;
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
	}
}
