using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Input;
using Color			= ZeldaOracle.Common.Graphics.Color;
using GamePad		= ZeldaOracle.Common.Input.GamePad;
using Keyboard		= ZeldaOracle.Common.Input.Keyboard;
using Mouse			= ZeldaOracle.Common.Input.Mouse;
using Buttons		= ZeldaOracle.Common.Input.Buttons;
using MouseButtons	= ZeldaOracle.Common.Input.MouseButtons;

namespace GameFramework.MyGame.Main {
/** <summary>
 * The main class for tiles in the room.
 * </summary> */
public class Tile : IGameObject {

	//========== CONSTANTS ===========
	#region Constants
	
	/** <summary> The constant size of tiles in pixels. </summary> */
	public static Point2I Size {
		get { return new Point2I(16, 16); }
	}
	/** <summary> The constant width of tiles in pixels. </summary> */
	public static int Width {
		get { return Size.X; }
	}
	/** <summary> The constant height of tiles in pixels. </summary> */
	public static int Height {
		get { return Size.Y; }
	}

	#endregion
	//=========== MEMBERS ============
	#region Members

	// Containment
	/** <summary> The room that contains this tile. </summary> */
	private Room room;
	/** <summary> The string identifier of the tile. </summary> */
	private string id;
	/** <summary> True if the tile has been destroyed. </summary> */
	private bool destroyed;

	// Position
	/** <summary> The position of the tile in the grid. </summary> */
	private Point2I gridPosition;

	// Collision
	/** <summary> True if the tile should be checked for collisions. </summary> */
	private bool collidable;
	/** <summary> True if the tile is a solid block. </summary> */
	private bool solid;

	// Visual
	/** <summary> The draw depth of the tile. </summary> */
	private double depth;
	/** <summary> True if the tile should be drawn. </summary> */
	private bool visible;
	/** <summary> True if the tile should be updated. </summary> */
	private bool updatable;
	/** <summary> True if the tile should be updated or drawn. </summary> */
	private bool enabled;

	#endregion
	//========= CONSTRUCTORS =========
	#region Constructors

	/** <summary> Constructs the default tile. </summary> */
	public Tile(string id = "") {
		// Containment
		this.room			= null;
		this.id				= id;
		this.destroyed		= false;

		// Position
		this.gridPosition	= Point2I.Zero;

		// Collision
		this.collidable		= true;
		this.solid			= true;
		
		// Visual
		this.depth			= 0.0;
		this.visible		= true;
		this.updatable		= true;
		this.enabled		= true;
	}
	/** <summary> Clones the tile. </summary> */
	public virtual Tile Clone() {
		// NOTE: Override this function

		// Containment
		Tile tile		= new Tile(this.id);

		// Collision
		tile.collidable = this.collidable;
		tile.solid		= this.solid;

		// Visual
		tile.depth		= this.depth;
		tile.visible	= this.visible;
		tile.updatable	= this.updatable;
		tile.enabled	= this.enabled;

		return tile;
	}
	/** <summary> Initializes the tile and sets up containment variables. </summary> */
	public virtual void Initialize(Room room) {
		// Containment
		this.room			= room;

		// NOTE: Override this function and call base first
	}
	/** <summary> Initializes the tile's position. </summary> */
	public void InitializeGridPosition(Point2I gridPosition) {
		// Position
		this.gridPosition	= gridPosition;
	}
	/** <summary> Uninitializes the tile and removes all containment variables. </summary> */
	public virtual void Uninitialize() {
		// Containment
		this.room			= null;

		// NOTE: Override this function and call base last
	}

	#endregion
	//========== PROPERTIES ==========
	#region Properties
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
	/** <summary> Gets the room that contains this tile. </summary> */
	public Room Room {
		get { return room; }
	}

	#endregion
	//--------------------------------
	#region Information

	/** <summary> Gets or sets the string identifier for the tile. </summary> */
	public string ID {
		get { return id; }
		set { id = value; }
	}
	/** <summary> Returns true if the tile has been destroyed. </summary> */
	public bool IsDestroyed {
		get { return destroyed; }
	}
	/** <summary> Returns true if the tile has been initialized. </summary> */
	public bool IsInitialized {
		get { return room != null; }
	}

	#endregion
	//--------------------------------
	#region Movement

	/** <summary> Gets the x position of the tile. </summary> */
	public double X {
		get { return gridPosition.X * Width; }
	}
	/** <summary> Gets the y position of the tile. </summary> */
	public double Y {
		get { return gridPosition.Y * Height; }
	}
	/** <summary> Gets the position of the tile. </summary> */
	public Vector2F Position {
		get { return gridPosition * Size; }
	}
	/** <summary> Gets the position of the tile. </summary> */
	public Vector2F ObjectPosition {
		get { return gridPosition * Size; }
	}

	/** <summary> Gets the x index of the tile in the grid. </summary> */
	public int GridX {
		get { return gridPosition.X; }
	}
	/** <summary> Gets the y index of the tile in the grid. </summary> */
	public int GridY {
		get { return gridPosition.Y; }
	}
	/** <summary> Gets the index of the tile in the grid. </summary> */
	public Point2I GridPosition {
		get { return gridPosition; }
	}


	#endregion
	//--------------------------------
	#region Collision

	/** <summary> Gets the bounding box of the tile. </summary> */
	public Rectangle2F Bounds {
		get { return new Rectangle2F(Position, Size); }
	}
	/** <summary> Gets the collision box of the tile. </summary> */
	public Rectangle2F Shape {
		get { return new Rectangle2F(Position, Size); }
	}
	/** <summary> Gets the center of the tile's shape. </summary> */
	public Vector2F Center {
		get { return (Position + Size / 2); }
	}
	/** <summary> Gets or sets if the tile should be checked for collisions. </summary> */
	public bool IsCollidable {
		get { return collidable; }
		set { collidable = value; }
	}
	/** <summary> Gets or sets if the tile is a solid block. </summary> */
	public bool IsSolid {
		get { return solid; }
		set { solid = value; }
	}

	#endregion
	//--------------------------------
	#region Visual

	/** <summary> Gets or sets the draw depth of the tile. </summary> */
	public double Depth {
		get { return depth; }
		set { depth = value; }
	}
	/** <summary> Gets or sets if the tile is visible. </summary> */
	public bool IsVisible {
		get { return visible; }
		set { visible = value; }
	}
	/** <summary> Gets or sets if the tile is enabled. </summary> */
	public bool IsEnabled {
		get { return enabled; }
		set { enabled = value; }
	}
	/** <summary> Gets or sets if the tile is updatable. </summary> */
	public bool IsUpdatable {
		get { return updatable; }
		set { updatable = value; }
	}

	#endregion
	//--------------------------------
	#endregion
	//=========== UPDATING ===========
	#region Updating

	/** <summary> Called every step to update the tile. </summary> */
	public virtual void Preupdate() {

		// NOTE: Override this function and call base
	}
	/** <summary> Called every step to update the tile. </summary> */
	public virtual void Update() {

		// NOTE: Override this function and call base
	}
	/** <summary> Called every step to update the tile. </summary> */
	public virtual void Postupdate() {

		// NOTE: Override this function and call base
	}

	#endregion
	//=========== DRAWING ============
	#region Drawing

	/** <summary> Called every step to draw the tile. </summary> */
	public virtual void Draw(Graphics2D g) {

		// NOTE: Override this function and call base
	}

	#endregion
	//========== MANAGEMENT ==========
	#region Management

	/** <summary> Marks the tile for removal from the room. </summary> */
	public virtual void Destroy() {
		destroyed = true;

		// NOTE: Override this function and call base first
	}
	/** <summary> Called when the tile enters the room. </summary> */
	public virtual void EnterRoom() {

		// NOTE: Override this function
	}
	/** <summary> Called when the tile leaves the room. </summary> */
	public virtual void LeaveRoom() {

		// NOTE: Override this function
	}

	#endregion
}
} // End namespace
