using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;

namespace GameFramework.MyGame.Main {
/** <summary>
 * The main class for entity objects in the room.
 * </summary> */
public class Entity : IGameObject {

	//========== CONSTANTS ===========
	#region Constants

	#endregion
	//=========== MEMBERS ============
	#region Members

	// Containment
	/** <summary> The room that contains this entity. </summary> */
	private Room room;
	/** <summary> The container that contains this entity. </summary> */
	//private Container container;
	/** <summary> The string identifier of the entity. </summary> */
	private string id;
	/** <summary> True if the entity has been destroyed. </summary> */
	private bool destroyed;

	// Movement
	/** <summary> The position of the entity in the room. </summary> */
	public Vector2F Position;
	/** <summary> The velocity the entity travels each update. </summary> */
	public Vector2F Velocity;
	/** <summary> The last direction the entity was facing. </summary> */
	private double lastDirection;

	// Collision
	/** <summary> The collision shape of the entity. </summary> */
	public IShape2F CollisionShape;
	/** <summary> True if this entity should be checked for collisions. </summary> */
	private bool collidable;

	// Visual
	/** <summary> The draw depth of the entity. </summary> */
	private double depth;
	/** <summary> True if the entity should be drawn. </summary> */
	private bool visible;
	/** <summary> True if the entity should be updated. </summary> */
	private bool updatable;
	/** <summary> True if the entity should be updated or drawn. </summary> */
	private bool enabled;
	/** <summary> True if the entity can update while in the menu. </summary> */
	private bool updateInMenu;

	#endregion
	//========= CONSTRUCTORS =========
	#region Constructors

	/** <summary> Constructs the default entity. </summary> */
	public Entity(string id = "") {
		// Containment
		this.room			= null;
		//this.container	= null;
		this.id				= id;
		this.destroyed		= false;

		// Movement
		this.Position		= Vector2F.Zero;
		this.Velocity		= Vector2F.Zero;
		this.lastDirection	= 0.0;

		// Collision
		this.CollisionShape	= Rectangle2F.Zero;
		this.collidable		= true;
		
		// Visual
		this.depth			= 0.0;
		this.visible		= true;
		this.updatable		= true;
		this.enabled		= true;
		this.updateInMenu	= false;
	}
	/** <summary> Clones the entity. </summary> */
	public virtual Entity Clone() {
		// NOTE: Override this function

		// Containment
		Entity entity			= new Entity(this.id);

		// Movement
		entity.Position			= this.Position;
		entity.Velocity			= this.Velocity;
		entity.lastDirection	= this.lastDirection;
		
		// Collision
		entity.CollisionShape	= this.CollisionShape;
		entity.collidable		= this.collidable;
		
		// Visual
		entity.depth			= this.depth;
		entity.visible			= this.visible;
		entity.updatable		= this.updatable;
		entity.enabled			= this.enabled;

		return entity;
	}
	/** <summary> Initializes the entity and sets up containment variables. </summary> */
	public virtual void Initialize(Room room) {
		this.room			= room;
		//this.container	= container;

		// NOTE: Override this function and call base first
	}
	/** <summary> Uninitializes the entity and removes all containment variables. </summary> */
	public virtual void Uninitialize() {
		//this.container	= null;
		this.room			= null;

		// NOTE: Override this function and call base last
	}

	//========== PROPERTIES ==========
	#region Properties
	#endregion
	//--------------------------------
	#region Containment

	/** <summary> Gets the class managing the XNA framework. </summary> */
	public GameBase GameBase {
		get { return room.GameBase; }
	}
	/** <summary> Gets the class managing the game. </summary> */
	public GameManager Game {
		get { return room.Game; }
	}
	/** <summary> Gets the room that contains this entity. </summary> */
	public Room Room {
		get { return room; }
	}
	/** <summary> The container that contains this entity. </summary> */
	/*public Container Container {
		get { return container; }
	}*/

	#endregion
	//--------------------------------
	#region Information

	/** <summary> Gets or sets the string identifier for the entity. </summary> */
	public string ID {
		get { return id; }
		set { id = value; }
	}
	/** <summary> Returns true if the entity has been destroyed. </summary> */
	public bool IsDestroyed {
		get { return destroyed; }
	}
	/** <summary> Returns true if the entity has been initialized. </summary> */
	public bool IsInitialized {
		get { return room != null; }
	}

	#endregion
	//--------------------------------
	#region Movement

	/** <summary> Gets or sets the x position of the entity. </summary> */
	public double X {
		get { return Position.X; }
		set { Position.X = value; }
	}
	/** <summary> Gets or sets the y position of the entity. </summary> */
	public double Y {
		get { return Position.Y; }
		set { Position.Y = value; }
	}
	/** <summary> Gets or sets the horizontal speed of the entity. </summary> */
	public double HSpeed {
		get { return Velocity.X; }
		set { Velocity.X = value; }
	}
	/** <summary> Gets or sets the vertical speed of the entity. </summary> */
	public double VSpeed {
		get { return Velocity.Y; }
		set { Velocity.Y = value; }
	}
	/** <summary> Gets or sets the direction the entity is moving in. </summary> */
	public double Direction {
		get { return (Velocity.IsZero ? lastDirection : Velocity.Direction); }
		set { Velocity.Direction = value; }
	}
	/** <summary> Gets or sets the speed of the entity. </summary> */
	public double Speed {
		get { return Velocity.Length; }
		set { Velocity.Length = value; }
	}
	/** <summary> Gets the position of the entity. </summary> */
	public Vector2F ObjectPosition {
		get { return Position; }
	}

	#endregion
	//--------------------------------
	#region Collision

	/** <summary> Gets the collision box of the entity. </summary> */
	public Rectangle2F Bounds {
		get { return CollisionShape.Bounds + Position; }
	}
	/** <summary> Gets the collision shape of the entity. </summary> */
	public IShape2F Shape {
		get { return CollisionShape.Translate(Position); }
	}
	/** <summary> Gets the center of the entity's shape. </summary> */
	public Vector2F Center {
		get { return CollisionShape.Center + Position; }
	}
	/** <summary> Gets or sets if the entity can be collided with. </summary> */
	public bool IsCollidable {
		get { return collidable; }
		set { collidable = value; }
	}

	#endregion
	//--------------------------------
	#region Visual

	/** <summary> Gets or sets the draw depth of the entity. </summary> */
	public double Depth {
		get { return depth; }
		set { depth = value; }
	}
	/** <summary> Gets or sets if the entity is visible. </summary> */
	public bool IsVisible {
		get { return visible; }
		set { visible = value; }
	}
	/** <summary> Gets or sets if the entity is enabled. </summary> */
	public bool IsEnabled {
		get { return enabled; }
		set { enabled = value; }
	}
	/** <summary> Gets or sets if the entity is updatable. </summary> */
	public bool IsUpdatable {
		get { return updatable; }
		set { updatable = value; }
	}
	/** <summary> Gets or sets if the entity can update while in the menu. </summary> */
	public bool IsUpdatableInMenu {
		get { return updateInMenu; }
		set { updateInMenu = value; }
	}

	#endregion
	//--------------------------------
	#endregion
	//========= INFORMATION ==========
	#region Information
	
	/** <summary> Returns the collision shape of the entity. </summary> */
	public virtual IShape2F GetShape() {
		return CollisionShape.Translate(Position);

		// NOTE: Override this function
	}
	/** <summary> Returns the collision bounds of the entity. </summary> */
	public virtual Rectangle2F GetBounds() {
		return CollisionShape.Bounds + Position;

		// NOTE: Override this function
	}

	#endregion
	//=========== UPDATING ===========
	#region Updating

	/** <summary> Called every step to update the entity. </summary> */
	public virtual void Preupdate() {

		// NOTE: Override this function and call base
	}
	/** <summary> Called every step to update the entity. </summary> */
	public virtual void Update() {

		// NOTE: Override this function and call base
	}
	/** <summary> Called every step to update the entity. </summary> */
	public virtual void Postupdate() {

		// NOTE: Override this function and call base
	}
	/** <summary> Called every step after Update and before Postupdate to move the entity. </summary> */
	public virtual void MoveEntity() {
		Position		+= Velocity;
		lastDirection	= Velocity.Direction;
	}

	#endregion
	//=========== DRAWING ============
	#region Drawing

	/** <summary> Called every step to draw the entity. </summary> */
	public virtual void Draw(Graphics2D g) {

		// NOTE: Override this function and call base
	}

	#endregion
	//========== MANAGEMENT ==========
	#region Management

	/** <summary> Marks the entity for removal from the room. </summary> */
	public virtual void Destroy() {
		destroyed = true;

		// NOTE: Override this function and call base first
	}
	/** <summary> Called when the entity enters the room. </summary> */
	public virtual void EnterRoom() {

		// NOTE: Override this function
	}
	/** <summary> Called when the entity leaves the room. </summary> */
	public virtual void LeaveRoom() {

		// NOTE: Override this function
	}

	#endregion
}
} // End namespace
