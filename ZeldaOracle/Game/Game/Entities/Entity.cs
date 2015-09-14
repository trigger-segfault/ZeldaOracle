using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Control;
using ZeldaOracle.Game.Worlds;

namespace ZeldaOracle.Game.Entities {

	// Flags for certain entity properties.
	// Some of these are physics related.
	[Flags]
	public enum EntityFlags {
		None					= 0,
		Solid					= 0x1,		// Entity is solid.
		CollideWorld			= 0x2,		// Collide with solids
		CollideRoomEdge			= 0x4,		// Colide with the edges of rooms.
		ReboundSolid			= 0x8,		// Rebound off of solids.
		ReboundRoomEdge			= 0x10,		// Rebound off of room edges.
		DestroyedInHoles		= 0x20,		// The entity gets destroyed in holes.
		DestroyedOutsideRoom	= 0x40,		// The entity is destroyed when it is outside of the room.
		HasGravity				= 0x80,		// The entity is affected by gravity.
		Bounces					= 0x100,	// The entity bounces when it falls to the ground.
		ShadowVisible			= 0x200,	// A shadows is visible for the entity.
		LedgePassable			= 0x400,	// The entity can pass over ledges.
		HalfSolidPassable		= 0x800,	// The entity can pass over half-solids.
		DynamicDepth			= 0x1000,	// The entity has dynamic depth.
		Dead					= 0x2000,	// The entity is dead and no longer exists.
		AutoDodge				= 0x4000,	// Will move out of the way when colliding the edges of objects.
	};


	// The main class for entity objects in the room.
	public abstract class Entity {

		private RoomControl			control;
		private bool				isAlive;
		protected EntityFlags		flags;
		protected Vector2F			position;
		protected float				zPosition;
		protected PhysicsComponent	physics;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public Entity() {
			control		= null;
			flags		= EntityFlags.None;
			isAlive		= false;
			position	= Vector2F.Zero;
			zPosition	= 0.0f;
			physics		= new PhysicsComponent(this);
		}

		// Initializes the entity and sets up containment variables.
		public void Initialize(RoomControl control) {
			this.control = control;
			this.isAlive = true;
			Initialize();
		}


		//-----------------------------------------------------------------------------
		// Virtual methods
		//-----------------------------------------------------------------------------

		// Initializes the entity and sets up containment variables.
		public virtual void Initialize() {}

		// Uninitializes the entity and removes all containment variables.
		public virtual void Uninitialize() {}
	
		// Called every step to update the entity.
		public virtual void Update(float ticks) {
			physics.Update(ticks);
		}

		// Called every step to draw the entity.
		public virtual void Draw(Graphics2D g) {}

		// Called when the entity enters the room.
		public virtual void OnEnterRoom() {}

		// Called when the entity leaves the room.
		public virtual void OnLeaveRoom() {}
		

	
		//-----------------------------------------------------------------------------
		// Management
		//-----------------------------------------------------------------------------

		public void Destroy() {
			if (isAlive) {
				isAlive = false;
				// TODO: OnDestroy()
			}
		}
	

		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		// Returns the room control this entity belongs to.
		public RoomControl RoomControl {
			get { return control; }
			set { control = value; }
		}

		// Returns true if the entity has been initialized.
		public bool IsInitialized {
			get { return (control != null); }
		}

		// Returns true if the entity is not alive.
		public bool IsDestroyed {
			get { return !isAlive; }
		}

		// Returns true if the entity is still alive.
		public bool IsAlive {
			get { return isAlive; }
		}
	
		// Gets or sets the flags.
		public EntityFlags Flags {
			get { return flags; }
			set { flags = value; }
		}

		// Gets or sets the position of the entity.
		public Vector2F Position {
			get { return position; }
			set { position = value; }
		}

		// Gets or sets the x-position of the entity.
		public float X {
			get { return position.X; }
			set { position.X = value; }
		}

		// Gets or sets the y-position of the entity.
		public float Y {
			get { return position.Y; }
			set { position.Y = value; }
		}
	
		// Gets or sets the entity's z-position.
		public float ZPosition {
			get { return zPosition; }
			set { zPosition = value; }
		}
	}
} // End namespace
