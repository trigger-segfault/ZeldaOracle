using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Control;
using ZeldaOracle.Game.Entities.Effects;
using ZeldaOracle.Game.Worlds;

namespace ZeldaOracle.Game.Entities {

	// The main class for entity objects in the room.
	public abstract class Entity {

		private RoomControl			roomControl;
		private	bool				isInitialized;
		private bool				isAlive;
		private bool				isInRoom;
		private Entity				transformedEntity; // The entity this entity has transformed into (bomb -> explosion)

		private Vector2F			previousPosition;
		private float				previousZPosition;

		protected Vector2F			position;
		protected float				zPosition;
		protected PhysicsComponent	physics;
		protected GraphicsComponent	graphics;
		protected Point2I			originOffset;
		protected Point2I			centerOffset;
		protected int				actionAlignDistance; // How many pixels off of alignment to interact with the entity (based on center positions).


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public Entity() {
			roomControl			= null;
			isAlive				= false;
			isInRoom			= false;
			isInitialized		= false;
			transformedEntity	= null;
			position			= Vector2F.Zero;
			zPosition			= 0.0f;
			previousPosition	= Vector2F.Zero;
			previousZPosition	= 0.0f;
			physics				= new PhysicsComponent(this);
			graphics			= new GraphicsComponent(this);
			originOffset		= Point2I.Zero;
			centerOffset		= Point2I.Zero;
			actionAlignDistance	= 5;
		}
		

		//-----------------------------------------------------------------------------
		// Interaction Methods
		//-----------------------------------------------------------------------------

		public virtual bool OnPlayerAction(int direction) { return false; }


		//-----------------------------------------------------------------------------
		// Virtual Methods
		//-----------------------------------------------------------------------------

		// Initializes the entity and sets up containment variables.
		public virtual void Initialize() {}

		// Uninitializes the entity and removes all containment variables.
		public virtual void Uninitialize() {}
	
		// Called every step to update the entity.
		public virtual void Update() {
			Vector2F tempPos = position;
			float tempZPos = zPosition;

			// Update the physics component.
			if (physics.IsEnabled)
				physics.Update();

			previousPosition  = tempPos;
			previousZPosition = tempZPos;
		}

		// Called every step to update the entity's gaphics.
		public virtual void UpdateGraphics() {

			// Update the graphics component.
			graphics.Update();
		}

		// Called every step to draw the entity.
		public virtual void Draw(Graphics2D g) {
			graphics.Draw(g);
		}
		
		
		// Called to draw above the entity.
		public virtual void DrawBelow(Graphics2D g, float depthLow, float depthHigh) {
		}
		
		// Called to draw above the entity.
		public virtual void DrawAbove(Graphics2D g, float depthLow, float depthHigh) {
		}

		// Called when the entity enters the room.
		public virtual void OnEnterRoom() {}

		// Called when the entity leaves the room.
		public virtual void OnLeaveRoom() {}

		// Called when the entity lands on the ground.
		public virtual void OnLand() {}

		// Called when the entity falls in a hole.
		public virtual void OnFallInHole() {
			if (physics.IsDestroyedInHoles) {
				RoomControl.SpawnEntity(new EffectFallingObject(), position);
				Destroy();
			}
		}
		
		// Called when the entity falls in water.
		public virtual void OnFallInWater() {
			if (physics.IsDestroyedInHoles) {
				RoomControl.SpawnEntity(new Effect(GameData.ANIM_EFFECT_WATER_SPLASH, DepthLayer.EffectSplash), position);
				Destroy();
			}
		}
		
		// Called when the entity falls in lava.
		public virtual void OnFallInLava() {
			if (physics.IsDestroyedInHoles) {
				RoomControl.SpawnEntity(new Effect(GameData.ANIM_EFFECT_LAVA_SPLASH, DepthLayer.EffectSplash), position);
				Destroy();
			}
		}

		// Special update method for when the entity is being carried.
		public virtual void UpdateCarrying() {}

		// Called immediately after the entity is destroyed.
		public virtual void OnDestroy() {}
		
	
		//-----------------------------------------------------------------------------
		// Management
		//-----------------------------------------------------------------------------

		// Initializes the entity and sets up containment variables.
		public void Initialize(RoomControl control) {
			this.roomControl	= control;
			this.isAlive		= true;
			this.isInRoom		= true;

			if (!isInitialized) {
				isInitialized = true;
				Initialize();
				physics.Initialize();
				previousPosition  = position;
				previousZPosition = zPosition;
			}
		}

		public void RemoveFromRoom() {
			isInRoom = false;
		}

		public void Destroy() {
			if (isAlive) {
				isAlive = false;
				isInRoom = false;
				OnDestroy();
			}
		}

		public void DestroyAndTransform(Entity transformedEntity) {
			Destroy();
			this.transformedEntity = transformedEntity;
		}

		public void EnablePhysics(PhysicsFlags flags = PhysicsFlags.None) {
			physics.IsEnabled = true;
			physics.Flags |= flags;
		}

		public void DisablePhysics() {
			physics.IsEnabled = false;
		}
		
		public void SetPositionByCenter(Vector2F center) {
			position = center - centerOffset;
		}
		
		
		//-----------------------------------------------------------------------------
		// Depth Calculations
		//-----------------------------------------------------------------------------

		public static float CalculateDepth(Entity entity, DepthLayer depthLayer) {
			float depthLayerMin			= 0.1f;
			float depthLayerMax			= 0.9f;
			int depthLayerCount			= (int) DepthLayer.Count;
			float depthLayerRegionSpan	= (depthLayerMax - depthLayerMin) / (depthLayerCount);
			float depthLayerRegionStart	= depthLayerMin + (depthLayerRegionSpan * (int) depthLayer);
			
			// Newer entities draw BELOW older ones.
			int entityIndex = entity.RoomControl.Entities.IndexOf(entity);
			if (entityIndex < 0)
				entityIndex = 0;
			float entityPercent = 1.0f - ((float) entityIndex / entity.RoomControl.Entities.Count);
			float entityDepthRegionSpan = depthLayerRegionSpan / (float) entity.RoomControl.Entities.Count;
			
			float depth = depthLayerRegionStart + (entityPercent * depthLayerRegionSpan);
			depth += entityDepthRegionSpan * 0.5f;

			return depth;

			/*
			float shadowDepth	= 0.05f;
			float ripplesDepth	= depth + (0.01f * entityDepthRegionSpan);
			float grassDepth	= depth + (0.02f * entityDepthRegionSpan);

			float depthPadding = 0.01f * entityDepthRegionSpan;
			
			entity.DrawBelow(g,
				depth - (entityDepthRegionSpan * 0.5f) + depthPadding,
				depth - depthPadding);
			entity.DrawAbove(g,
				grassDepth + depthPadding,
				depth + (entityDepthRegionSpan * 0.5f) - depthPadding);
			*/
		}

		
		//-----------------------------------------------------------------------------
		// Static Methods
		//-----------------------------------------------------------------------------

		public static bool AreEntitiesAligned(Entity a, Entity b, int direction, int threshold) {
			return ((Directions.IsVertical(direction) && Math.Abs(a.Center.X - b.Center.X) <= threshold) ||
				(Directions.IsHorizontal(direction) && Math.Abs(a.Center.Y - b.Center.Y) <= threshold));
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
			set { isAlive = !value; }
		}

		// Returns true if the entity is still alive.
		public bool IsAlive {
			get { return isAlive; }
			set { isAlive = value; }
		}

		// Returns true if the entity is being handled by RoomControl.
		public bool IsInRoom {
			get { return isInRoom; }
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
		
		public Vector2F PreviousPosition {
			get { return previousPosition; }
		}
	
		public float PreviousZPosition {
			get { return previousZPosition; }
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
			get { return (zPosition > 0.0f || (physics.IsEnabled && physics.ZVelocity > 0.0f)); }
		}

		public bool IsOnGround {
			get { return !IsInAir; }
		}

		public Vector2F Origin {
			get { return (position + originOffset); }
			set { position = value - originOffset; }
		}

		public Point2I OriginOffset {
			get { return originOffset; }
			set { originOffset = value; }
		}

		public Vector2F Center {
			get { return position + centerOffset; }
		}
	
		public int ActionAlignDistance {
			get { return actionAlignDistance; }
			set { actionAlignDistance = value; }
		}

		public Entity TransformedEntity {
			get { return transformedEntity; }
		}
	}
}
