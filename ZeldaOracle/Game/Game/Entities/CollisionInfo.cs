using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Game.Tiles;

namespace ZeldaOracle.Game.Entities {

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
		
		
		
		//-----------------------------------------------------------------------------
		// Mutators
		//-----------------------------------------------------------------------------
		
		public void Clear() {
			type = CollisionType.None;
			solidObject = null;
		}

		public void SetTileCollision(Tile t) {
			type = CollisionType.Tile;
			solidObject = t;
		}
		
		public void SetEntityCollision(Entity e) {
			type = CollisionType.Entity;
			solidObject = e;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public bool IsColliding {
			get { return (type != CollisionType.None); }
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
