﻿using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Control;
using ZeldaOracle.Game.Entities.Collisions;
using ZeldaOracle.Game.Entities.Effects;

namespace ZeldaOracle.Game.Entities {

	// The main class for entity objects in the room.
	public abstract class Entity {

		private RoomControl			roomControl;
		private	bool				isInitialized;
		private bool				isAlive;
		private bool				isInRoom;
		private int					entityIndex;
		private Entity				transformedEntity; // The entity this entity has transformed into (bomb -> explosion)
		private Vector2F			previousPosition;
		private float				previousZPosition;
		private Properties			properties;

		protected Vector2F			position;
		protected float				zPosition;
		protected Point2I			centerOffset;
		protected int				actionAlignDistance; // How many pixels off of alignment to interact with the entity (based on center positions).
		protected Rectangle2F		buttonActionCollisionBox; // Collision box that for button actions
		protected Sound				soundBounce;
		protected bool              isGrabbable;
		protected bool              isPickupable;
		protected Vector2F          carriedDrawOffset;

		protected PhysicsComponent	physics;
		protected GraphicsComponent	graphics;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public Entity() {
			entityIndex			= -1;
			roomControl			= null;
			isAlive				= false;
			isInRoom			= false;
			isInitialized		= false;
			transformedEntity	= null;
			soundBounce			= null;
			position			= Vector2F.Zero;
			zPosition			= 0.0f;
			previousPosition	= Vector2F.Zero;
			previousZPosition	= 0.0f;
			physics				= new PhysicsComponent(this);
			graphics			= new GraphicsComponent(this);
			centerOffset		= Point2I.Zero;
			actionAlignDistance	= 5;
			buttonActionCollisionBox = Rectangle2F.Zero;
			properties			= null;
		}
		

		//-----------------------------------------------------------------------------
		// Interaction Methods
		//-----------------------------------------------------------------------------

		public virtual bool OnPlayerAction(int direction) { return false; }


		//-----------------------------------------------------------------------------
		// Virtual Methods
		//-----------------------------------------------------------------------------

		/// <summary>Initializes the entity and sets up containment variables.
		/// </summary>
		public virtual void Initialize() {}

		/// <summary>Uninitializes the entity and removes all containment variables.
		/// </summary>
		public virtual void Uninitialize() {}
	
		/// <summary>Called every step to update the entity.</summary>
		public virtual void Update() {
			previousPosition  = position;
			previousZPosition = zPosition;
		}

		/// <summary>Called every step to update the entity's gaphics.</summary>
		public virtual void UpdateGraphics() {
			graphics.Update();
		}

		/// <summary>Called every step to draw the entity.</summary>
		public virtual void Draw(RoomGraphics g) {
			graphics.Draw(g);
		}

		/// <summary>Called every step to draw the entity.</summary>
		public virtual void Draw(RoomGraphics g, DepthLayer depthLayer) {
			graphics.Draw(g, depthLayer);
		}

		/// <summary>Called when the entity enters the room. FIXME: this is never
		/// actually called for entities, but it is called for the player</summary>
		public virtual void OnEnterRoom() {}

		/// <summary>Called when the entity leaves the room.</summary>
		public virtual void OnLeaveRoom() {}

		/// <summary>Called when the entity lands on the ground.</summary>
		public virtual void OnLand() {}
		
		/// <summary>Called when the entity begins falling (used in side-scrolling
		/// mode).</summary>
		public virtual void OnBeginFalling() {}

		/// <summary>Called when the entity bounces off of the ground.</summary>
		public virtual void OnBounce() {
			if (soundBounce != null)
				AudioSystem.PlaySound(soundBounce);
		}

		/// <summary>Occurs when the entity is crushed between two collisions.
		/// The 'rock' collision is what causes the crushing penetration into
		/// the 'hardPlace' collision.</summary>
		public virtual void OnCrush(Collision rock, Collision hardPlace) {}

		/// <summary>Called when the entity falls in a hole.</summary>
		public virtual void OnFallInHole() {
			if (physics.IsDestroyedInHoles) {
				RoomControl.SpawnEntity(new EffectFallingObject(), position);
				AudioSystem.PlaySound(GameData.SOUND_OBJECT_FALL);
				Destroy();
			}
		}
		
		/// <summary>Called when the entity falls in water in side-scroll mode
		/// .</summary>
		public virtual void OnFallInSideScrollWater() {
			physics.VelocityX = 0.0f;
			RoomControl.SpawnEntity(new Effect(
				GameData.ANIM_EFFECT_WATER_SPLASH,
				DepthLayer.EffectSplash, true), position);
			AudioSystem.PlaySound(GameData.SOUND_PLAYER_WADE);
		}
		
		/// <summary>Called when the entity falls in water.</summary>
		public virtual void OnFallInWater() {
			if (physics.IsDestroyedInHoles) {
				RoomControl.SpawnEntity(new Effect(
					GameData.ANIM_EFFECT_WATER_SPLASH,
					DepthLayer.EffectSplash, true), position);
				AudioSystem.PlaySound(GameData.SOUND_PLAYER_WADE);
				Destroy();
			}
		}
		
		/// <summary>Called when the entity falls in lava.</summary>
		public virtual void OnFallInLava() {
			if (physics.IsDestroyedInHoles) {
				RoomControl.SpawnEntity(new Effect(
					GameData.ANIM_EFFECT_LAVA_SPLASH,
					DepthLayer.EffectSplash, true), position);
				AudioSystem.PlaySound(GameData.SOUND_PLAYER_WADE);
				Destroy();
			}
		}

		/// <summary>Called when the entity is picked up.</summary>
		public virtual void OnPickup() { }

		/// <summary>Called when the entity has been picked up and is now being carried.</summary>
		public virtual void OnCarry() { }

		/// <summary>Called when the carried entity is thrown.</summary>
		public virtual void OnThrow() { }

		/// <summary>Called when the carried entity is dropped.</summary>
		public virtual void OnDrop() { }

		/// <summary>Updates the entity while being carried or picked up.</summary>
		public virtual void UpdateCarrying(bool isPickingUp) { }

		/// <summary>Draws the entity while being carried or picked up.</summary>
		public virtual void DrawCarrying(RoomGraphics g, bool isPickingUp) { }

		/// <summary>Called immediately after the entity is destroyed.</summary>
		public virtual void OnDestroy() {}
		
	
		//-----------------------------------------------------------------------------
		// Management
		//-----------------------------------------------------------------------------

		/// <summary>Initializes the entity and sets up containment variables.
		/// </summary>
		public void Initialize(RoomControl control) {
			this.roomControl	= control;
			this.isAlive		= true;
			this.isInRoom		= true;

			if (!isInitialized) {
				isInitialized = true;
				Initialize();
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
			transformedEntity.Properties = properties;
			this.transformedEntity = transformedEntity;
		}

		/// <summary>Enable physics with the given physics flags.</summary>
		public void EnablePhysics(PhysicsFlags flags = PhysicsFlags.None) {
			physics.IsEnabled = true;
			physics.Flags |= flags;
		}

		/// <summary>Disable physics for this entity.</summary>
		public void DisablePhysics() {
			physics.IsEnabled = false;
		}
		
		public void SetPositionByCenter(Vector2F center) {
			position = center - centerOffset;
		}

		
		//-----------------------------------------------------------------------------
		// Static Methods
		//-----------------------------------------------------------------------------

		public static bool AreEntitiesAligned(Entity a, Entity b, int direction,
			float threshold)
		{
			return (GMath.Abs(a.Center - b.Center)[!Directions.IsVertical(direction)] <=
				threshold);
		}

		public static bool AreEntitiesCollisionAligned(Entity a, Entity b, int direction,
			CollisionBoxType collisionType)
		{
			Rectangle2F aBox = a.Physics.GetCollisionBox(collisionType) + a.Position;
			Rectangle2F bBox = b.Physics.GetCollisionBox(collisionType) + b.Position;
			if (Directions.IsVertical(direction))
				return aBox.LeftRight.Intersects(bBox.LeftRight);
			else
				return aBox.TopBottom.Intersects(bBox.TopBottom);
		}


		//-----------------------------------------------------------------------------
		// Virtual Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets or sets if the entity can currently be grabbed.</summary>
		public bool IsGrabbable {
			get { return isGrabbable; }
			set { isGrabbable = value; }
		}

		/// <summary>Gets or sets if the entity can currently be picked up.</summary>
		public bool IsPickupable {
			get { return isPickupable; }
			set { isPickupable = value; }
		}

		/// <summary>Gets or sets the extra draw offset applied while carring the entity.</summary>
		public Vector2F CarriedDrawOffset {
			get { return carriedDrawOffset; }
			set { carriedDrawOffset = value; }
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Returns the game control this entity belongs to.</summary>
		public GameControl GameControl {
			get { return roomControl.GameControl; }
		}

		/// <summary>Returns the room control this entity belongs to.</summary>
		public RoomControl RoomControl {
			get { return roomControl; }
			set { roomControl = value; }
		}

		/// <summary>Returns true if the entity has been initialized.</summary>
		public bool IsInitialized {
			get { return (roomControl != null); }
		}

		/// <summary>Returns true if the entity is not alive.</summary>
		public bool IsDestroyed {
			get { return !isAlive; }
			set { isAlive = !value; }
		}

		/// <summary>Returns true if the entity is still alive.</summary>
		public bool IsAlive {
			get { return isAlive; }
			set { isAlive = value; }
		}

		/// <summary>Returns true if the entity is being handled by RoomControl.
		/// </summary>
		public bool IsInRoom {
			get { return isInRoom; }
		}

		public int EntityIndex {
			get { return entityIndex; }
			set { entityIndex = value; }
		}

		/// <summary>Gets or sets the position of the entity.</summary>
		public Vector2F Position {
			get { return position; }
			set { position = value; }
		}

		public Vector2F DrawPosition {
			get {
				bool horizontal = GMath.Abs(Physics.SurfaceVelocity.X) > GameSettings.EPSILON &&
										(!Physics.IsCollidingInDirection(Directions.Left) &&
										!Physics.IsCollidingInDirection(Directions.Right));
				bool vertical = GMath.Abs(Physics.SurfaceVelocity.Y) > GameSettings.EPSILON &&
										(!Physics.IsCollidingInDirection(Directions.Up) &&
										!Physics.IsCollidingInDirection(Directions.Down));
				Vector2F surfacePosition = Vector2F.Zero;
				if (horizontal)	surfacePosition.X = Physics.SurfacePosition.X;
				if (vertical)	surfacePosition.Y = Physics.SurfacePosition.Y;
				return GameUtil.Bias(surfacePosition) -
					GameUtil.ReverseBias(surfacePosition - position);
			}
		}

		/// <summary>Gets or sets the x-position of the entity.</summary>
		public float X {
			get { return position.X; }
			set { position.X = value; }
		}

		/// <summary>Gets or sets the y-position of the entity.</summary>
		public float Y {
			get { return position.Y; }
			set { position.Y = value; }
		}
	
		/// <summary>Gets or sets the entity's z-position.</summary>
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
	
		/// <summary>Gets or sets the entity's physics component.</summary>
		public PhysicsComponent Physics {
			get { return physics; }
			set { physics = value; physics.Entity = this; }
		}
	
		/// <summary>Gets or sets the entity's graphics component.</summary>
		public GraphicsComponent Graphics {
			get { return graphics; }
			set { graphics = value; graphics.Entity = this; }
		}

		public bool IsInAir {
			get {
				if (Physics.IsEnabled)
					return Physics.IsInAir;
				return (zPosition > 0.0f);
			}
		}

		public bool IsOnGround {
			get { return !IsInAir; }
		}

		public Vector2F Center {
			get { return position + centerOffset; }
		}

		public Vector2F DrawCenter {
			get { return DrawPosition + centerOffset; }
		}

		public Vector2F CenterOffset {
			get { return centerOffset; }
		}
	
		public int ActionAlignDistance {
			get { return actionAlignDistance; }
			set { actionAlignDistance = value; }
		}

		public Rectangle2F ButtonActionCollisionBox {
			get { return buttonActionCollisionBox; }
		}

		public Entity TransformedEntity {
			get { return transformedEntity; }
		}

		public Sound BounceSound {
			get { return soundBounce; }
			set { soundBounce = value; }
		}

		public Properties Properties {
			get { return properties; }
			set { properties = value; }
		}

		/// <summary>Gets the position of the entity relative to the view.</summary>
		public Vector2F ViewPosition {
			get { return DrawPosition - RoomControl.ViewControl.Position; }
		}

		/// <summary>Gets the center of the entity relative to the view.</summary>
		public Vector2F ViewCenter {
			get { return DrawCenter - RoomControl.ViewControl.Position; }
		}
	}
}
