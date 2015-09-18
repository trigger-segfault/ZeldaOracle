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
		Dead					= 0x2000,	// The entity is dead and no longer exists.

		// Physics
		DestroyedOutsideRoom	= 0x40,		// The entity is destroyed when it is outside of the room.
		DestroyedInHoles		= 0x20,		// The entity gets destroyed in holes.
		Solid					= 0x1,		// Entity is solid.
		HasGravity				= 0x80,		// The entity is affected by gravity.
		CollideRoomEdge			= 0x4,		// Colide with the edges of rooms.
		ReboundSolid			= 0x8,		// Rebound off of solids.
		ReboundRoomEdge			= 0x10,		// Rebound off of room edges.
		CollideWorld			= 0x2,		// Collide with solids
		Bounces					= 0x100,	// The entity bounces when it falls to the ground.
		AutoDodge				= 0x4000,	// Will move out of the way when colliding the edges of objects.
		HalfSolidPassable		= 0x800,	// The entity can pass over half-solids.
		LedgePassable			= 0x400,	// The entity can pass over ledges.

		// Graphics.
		DynamicDepth			= 0x1000,	// The entity has dynamic depth.
		ShadowVisible			= 0x200,	// A shadows is visible for the entity.
	};


	// The main class for entity objects in the room.
	public abstract class Entity {

		private RoomControl			roomControl;
		private bool				isAlive;
		protected Vector2F			position;
		protected float				zPosition;
		protected PhysicsComponent	physics;
		protected GraphicsComponent	graphics;

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public Entity() {
			roomControl		= null;
			isAlive		= false;
			position	= Vector2F.Zero;
			zPosition	= 0.0f;
			physics		= new PhysicsComponent(this);
			graphics	= new GraphicsComponent(this);
		}


		//-----------------------------------------------------------------------------
		// Virtual methods
		//-----------------------------------------------------------------------------

		// Initializes the entity and sets up containment variables.
		public virtual void Initialize() {}

		// Uninitializes the entity and removes all containment variables.
		public virtual void Uninitialize() {}
	
		// Called every step to update the entity.
		public virtual void Update() {
			// Update the physics component.
			if (physics.IsEnabled)
				physics.Update();

			// Update the graphics component.
			graphics.Update();
		}

		// Called every step to draw the entity.
		public virtual void Draw(Graphics2D g) {
			graphics.Draw(g);
		}

		// Called when the entity enters the room.
		public virtual void OnEnterRoom() {}

		// Called when the entity leaves the room.
		public virtual void OnLeaveRoom() {}
		
	
		//-----------------------------------------------------------------------------
		// Management
		//-----------------------------------------------------------------------------

		// Initializes the entity and sets up containment variables.
		public void Initialize(RoomControl control) {
			this.roomControl = control;
			this.isAlive = true;
			Initialize();
		}

		public void Destroy() {
			if (isAlive) {
				isAlive = false;
				// TODO: OnDestroy()
			}
		}

		public void EnablePhysics(PhysicsFlags flags = PhysicsFlags.None) {
			physics.IsEnabled = true;
			physics.Flags |= flags;
		}

		public void DisablePhysics() {
			physics.IsEnabled = false;
		}
	

		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		// Returns the game control this entity belongs to.
		public GameControl GameControl {
			get { return roomControl.GameControl; }
		}

		// Returns the room control this entity belongs to.
		public RoomControl RoomControl {
			get { return roomControl; }
			set { roomControl = value; }
		}

		// Returns true if the entity has been initialized.
		public bool IsInitialized {
			get { return (roomControl != null); }
		}

		// Returns true if the entity is not alive.
		public bool IsDestroyed {
			get { return !isAlive; }
		}

		// Returns true if the entity is still alive.
		public bool IsAlive {
			get { return isAlive; }
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
	
		// Gets or sets the entity's physics component.
		public PhysicsComponent Physics {
			get { return physics; }
			set { physics = value; physics.Entity = this; }
		}
	
		// Gets or sets the entity's graphics component.
		public GraphicsComponent Graphics {
			get { return graphics; }
			set { graphics = value; graphics.Entity = this; }
		}

		public bool IsInAir {
			get { return (zPosition > 0 || (physics.IsEnabled && physics.ZVelocity > 0)); }
		}

		public bool IsOnGround {
			get { return !IsInAir; }
		}

	}
} // End namespace
