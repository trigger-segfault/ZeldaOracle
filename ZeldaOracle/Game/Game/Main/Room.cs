using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Keys			= Microsoft.Xna.Framework.Input.Keys;

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
 * The class that contains updatable objects in the game.
 * </summary> */
public class Room {
	
	//========== CONSTANTS ===========
	#region Constants

	public const bool IsTileObject = false;

	#endregion
	//=========== MEMBERS ============
	#region Members

	// Containment
	/** <summary> The class managing the game. </summary> */
	private GameManager game;
	/** <summary> The string identifier of the room. </summary> */
	protected string id;

	// Game Objects
	/** <summary> The collection of game objects in the room. </summary> */
	private List<IGameObject> objects;
	/** <summary> The collection of entities in the room. </summary> */
	private List<Entity> entities;
	/** <summary> The grid of tiles in the room. </summary> */
	private Tile[,] tiles;

	#endregion
	//========= CONSTRUCTORS =========
	#region Constructors

	/** <summary> Constructs the default room. </summary> */
	public Room(int gridWidth = 30, int gridHeight = 20, string id = "") {
		// Containment
		this.game			= null;
		this.id				= id;

		// Game Objects
		this.objects		= new List<IGameObject>();
		this.entities		= new List<Entity>();
		this.tiles			= new Tile[gridWidth, gridHeight];
	}
	/** <summary> Constructs the default room. </summary> */
	public Room(Point2I gridSize, string id = "") {
		// Containment
		this.game			= null;
		this.id				= id;

		// Game Objects
		this.objects		= new List<IGameObject>();
		this.entities		= new List<Entity>();
		this.tiles			= new Tile[gridSize.X, gridSize.Y];
	}
	/** <summary> Initializes the game manager. </summary> */
	public virtual void Initialize(GameManager game) {
		// Containment
		this.game			= game;

		// Initialize all the game objects that were added before initialization
		for (int i = 0; i < this.objects.Count; i++) {
			this.objects[i].Initialize(this);
		}

		if (!IsTileObject) {
			for (int x = 0; x < this.GridWidth; x++) {
				for (int y = 0; y < this.GridHeight; y++) {
					if (this.tiles[x, y] != null) {
						this.tiles[x, y].Initialize(this);
					}
				}
			}
		}

		// NOTE: Override this function and call base first
	}
	/** <summary> Uninitializes the game manager. </summary> */
	public virtual void Uninitialize() {
		// Game Objects
		this.ClearObjects();
		if (!IsTileObject)
			this.ClearTiles();
		this.tiles			= null;
		this.entities		= null;
		this.objects		= null;

		// Containment
		this.game			= null;

		// NOTE: Override this function and call base last
	}
	/** <summary> Called to load game manager content. </summary> */
	public virtual void LoadContent(ContentManager content) {

		// NOTE: Override this function and call base first
	}
	/** <summary> Called to unload game manager content. </summary> */
	public virtual void UnloadContent(ContentManager content) {

		// NOTE: Override this function and call base last
	}

	#endregion
	//========== PROPERTIES ==========
	#region Properties
	//--------------------------------
	#region Containment

	/** <summary> Gets the class managing the XNA framework. </summary> */
	public GameBase GameBase {
		get { return game.GameBase; }
	}
	/** <summary> Gets the class managing the game. </summary> */
	public GameManager Game {
		get { return game; }
	}

	#endregion
	//--------------------------------
	#region Information

	/** <summary> Gets or sets the string identifier of the room. </summary> */
	public string ID {
		get { return id; }
		set { id = value; }
	}
	/** <summary> Returns true if the room has been initialized. </summary> */
	public bool IsInitialized {
		get { return game != null; }
	}

	#endregion
	//--------------------------------
	#region Game Objects

	/** <summary> Gets the collection of game objects in the room. </summary> */
	public List<IGameObject> Objects {
		get { return objects; }
	}
	/** <summary> Gets the collection of entities in the room. </summary> */
	public List<Entity> Entities {
		get { return entities; }
	}
	/** <summary> Gets the grid of tiles in the room. </summary> */
	public Tile[,] Tiles {
		get { return tiles; }
	}
	/** <summary> Gets the number of game objects in the room. </summary> */
	public int NumObjects {
		get { return objects.Count; }
	}
	/** <summary> Gets the number of entities in the room. </summary> */
	public int NumEntities {
		get { return entities.Count; }
	}

	#endregion
	//--------------------------------
	#region Dimensions

	/** <summary> Gets the width of the tile grid. </summary> */
	public int GridWidth {
		get { return tiles.GetLength(0); }
	}
	/** <summary> Gets the height of the tile grid. </summary> */
	public int GridHeight {
		get { return tiles.GetLength(1); }
	}
	/** <summary> Gets the dimensions of the tile grid. </summary> */
	public Point2I GridSize {
		get { return new Point2I(tiles.GetLength(0), tiles.GetLength(1)); }
	}

	/** <summary> Gets the width of the room. </summary> */
	public int Width {
		get { return tiles.GetLength(0) * Tile.Width; }
	}
	/** <summary> Gets the height of the room. </summary> */
	public int Height {
		get { return tiles.GetLength(1) * Tile.Height; }
	}
	/** <summary> Gets the size of the room. </summary> */
	public Point2I Size {
		get { return new Point2I(tiles.GetLength(0), tiles.GetLength(1)) * Tile.Size; }
	}

	#endregion
	//--------------------------------
	#endregion
	//=========== UPDATING ===========
	#region Updating

	/** <summary> Called every step to update the game. </summary> */
	public virtual void Update() {
		UpdateEntities();
		//UpdateTiles();

		DestroyObjects();

		// NOTE: Override this function and call base
	}
	/** <summary> Called every step to update the entities. </summary> */
	protected virtual void UpdateEntities() {
		for (int i = 0; i < entities.Count; i++) {
			if (!entities[i].IsDestroyed && entities[i].IsUpdatable && entities[i].IsEnabled)
				entities[i].Preupdate();
		}
		for (int i = 0; i < entities.Count; i++) {
			if (!entities[i].IsDestroyed && entities[i].IsUpdatable && entities[i].IsEnabled)
				entities[i].Update();
		}
		for (int i = 0; i < entities.Count; i++) {
			if (!entities[i].IsDestroyed && entities[i].IsUpdatable && entities[i].IsEnabled)
				entities[i].MoveEntity();
		}
		for (int i = 0; i < entities.Count; i++) {
			if (!entities[i].IsDestroyed && entities[i].IsUpdatable && entities[i].IsEnabled)
				entities[i].Postupdate();
		}
	}
	/** <summary> Called every step to update the tiles. </summary> */
	protected virtual void UpdateTiles() {
		for (int x = 0; x < GridWidth; x++) {
			for (int y = 0; y < GridHeight; y++) {
				if (tiles[x, y] != null) {
					if (!tiles[x, y].IsDestroyed && tiles[x, y].IsUpdatable && tiles[x, y].IsEnabled)
						tiles[x, y].Preupdate();
				}
			}
		}
		for (int x = 0; x < GridWidth; x++) {
			for (int y = 0; y < GridHeight; y++) {
				if (tiles[x, y] != null) {
					if (!tiles[x, y].IsDestroyed && tiles[x, y].IsUpdatable && tiles[x, y].IsEnabled)
						tiles[x, y].Update();
				}
			}
		}
		for (int x = 0; x < GridWidth; x++) {
			for (int y = 0; y < GridHeight; y++) {
				if (tiles[x, y] != null) {
					if (!tiles[x, y].IsDestroyed && tiles[x, y].IsUpdatable && tiles[x, y].IsEnabled)
						tiles[x, y].Postupdate();
				}
			}
		}
	}
	/** <summary> Called every step to update the destroyed game objects. </summary> */
	protected virtual void DestroyObjects() {

		for (int i = 0; i < entities.Count; i++) {
			if (entities[i].IsDestroyed) {
				entities[i].Uninitialize();
				objects.Remove(entities[i]);
				entities.RemoveAt(i);
				i--;
			}
		}
		for (int x = 0; x < GridWidth; x++) {
			for (int y = 0; y < GridHeight; y++) {
				if (tiles[x, y] != null) {
					if (tiles[x, y].IsDestroyed) {
						tiles[x, y].Uninitialize();
						if (IsTileObject)
							objects.Remove(tiles[x, y]);
						tiles[x, y] = null;
					}
				}
			}
		}
	}

	#endregion
	//=========== DRAWING ============
	#region Drawing

	/** <summary> Called every step to draw the room and its objects. </summary> */
	public virtual void Draw(Graphics2D g) {
		// Draw the game objects
		DrawObjects(g);

		// NOTE: Override this function and call base
	}
	/** <summary> Called every step to draw the objects. </summary> */
	protected virtual void DrawObjects(Graphics2D g) {
		// Sort the objects by depth using insertion sort
		for (int i = 1; i < objects.Count; i++) {
			IGameObject gameObj = objects[i];
			int j;

			for (j = i - 1; j >= 0; j--) {
				if (gameObj.Depth >= objects[j].Depth)
					break;
				objects[j + 1] = objects[j];

			}
			objects[j + 1] = gameObj;
		}

		// Draw the objects
		if (IsTileObject) {
			for (int i = 0; i < objects.Count; i++) {
				if (objects[i].IsVisible && objects[i].IsEnabled) {
					g.Translate(objects[i].ObjectPosition);
					objects[i].Draw(g);
					g.Translate(-objects[i].ObjectPosition);
				}

			}
		}
		else {
			int currentTileRow = 0;
			double currentTileDepth = (double)(Tile.Height) / (double)Height;

			for (int i = 0; i < objects.Count; i++) {
				while (objects[i].Depth > currentTileDepth && currentTileRow < GridHeight) {
					for (int j = 0; j < GridWidth; j++) {
						if (tiles[j, currentTileRow] != null) {
							g.Translate(tiles[j, currentTileRow].ObjectPosition);
							tiles[j, currentTileRow].Draw(g);
							g.Translate(-tiles[j, currentTileRow].ObjectPosition);
						}
					}
					currentTileRow++;
					currentTileDepth = (double)((currentTileRow + 1) * Tile.Height) / (double)Height;
				}
				if (objects[i].IsVisible && objects[i].IsEnabled) {
					g.Translate(objects[i].ObjectPosition);
					objects[i].Draw(g);
					g.Translate(-objects[i].ObjectPosition);
				}
			}
			while (currentTileRow < GridHeight) {
				for (int j = 0; j < GridWidth; j++) {
					if (tiles[j, currentTileRow] != null) {
						g.Translate(tiles[j, currentTileRow].ObjectPosition);
						tiles[j, currentTileRow].Draw(g);
						g.Translate(-tiles[j, currentTileRow].ObjectPosition);
					}
				}
				currentTileRow++;
			}
		}
	}

	#endregion
	//========== MANAGEMENT ==========
	#region Management

	/** <summary> Called when the room becomes active. </summary> */
	public virtual void EnterRoom() {
		for (int i = 0; i < objects.Count; i++) {
			objects[i].EnterRoom();
		}

		// NOTE: Override this function and call base
	}
	/** <summary> Called when the room is no longer active. </summary> */
	public virtual void LeaveRoom() {
		for (int i = 0; i < objects.Count; i++) {
			objects[i].LeaveRoom();
		}

		// NOTE: Override this function and call base
	}

	#endregion
	//========= GAME OBJECTS =========
	#region Game Objects

	/** <summary> Clears all the game objects from the room. </summary> */
	public void ClearObjects() {
		for (int i = 0; i < objects.Count; i++) {
			objects[i].Uninitialize();
		}
		if (!IsTileObject)
			ClearTiles();
		Point2I gridSize = GridSize;

		objects		= new List<IGameObject>();
		entities	= new List<Entity>();
		tiles		= new Tile[gridSize.X, gridSize.Y];
	}

	#endregion
	//=========== ENTITIES ===========
	#region Entities

	/** <summary> Clears all the entities from the room. </summary> */
	public void ClearEntities() {
		for (int i = 0; i < entities.Count; i++) {
			entities[i].Uninitialize();
			objects.Remove(entities[i]);
		}
		entities = new List<Entity>();
	}
	/** <summary> Adds the entity to the room. </summary> */
	public void AddEntity(Entity entity) {
		objects.Add(entity);
		entities.Add(entity);
		if (IsInitialized)
			entity.Initialize(this);
	}
	/** <summary> Removes the entity from the room. </summary> */
	public void RemoveEntity(Entity entity) {
		for (int i = 0; i < entities.Count; i++) {
			if (entities[i] == entity) {
				entities[i].Uninitialize();
				objects.Remove(entities[i]);
				entities.RemoveAt(i);
				return;
			}
		}
	}
	/** <summary> Removes the entities with the given ID from the room. </summary> */
	public void RemoveEntity(string id) {
		for (int i = 0; i < entities.Count; i++) {
			if (entities[i].ID == id) {
				entities[i].Uninitialize();
				objects.Remove(entities[i]);
				entities.RemoveAt(i);
				i--;
			}
		}
	}
	/** <summary> Gets the entity with the given ID. </summary> */
	public Entity GetEntity(string id) {
		for (int i = 0; i < entities.Count; i++) {
			if (entities[i].ID == id)
				return entities[i];
		}
		return null;
	}
	/** <summary> Gets the list of entities with the given ID. </summary> */
	public Entity[] GetEntities(string id) {
		List<Entity> entityList = new List<Entity>();
		for (int i = 0; i < entities.Count; i++) {
			if (entities[i].ID == id)
				entityList.Add(entities[i]);
		}
		return entityList.ToArray();
	}
	/** <summary> Returns true if the specified entity exists in the room. </summary> */
	public bool EntityExists(Entity entity) {
		for (int i = 0; i < entities.Count; i++) {
			if (entities[i] == entity)
				return true;
		}
		return false;
	}
	/** <summary> Returns true if an entity with the given ID exists. </summary> */
	public bool EntityExists(string id) {
		for (int i = 0; i < entities.Count; i++) {
			if (entities[i].ID == id)
				return true;
		}
		return false;
	}

	#endregion
	//============ TILES =============
	#region Tiles

	/** <summary> Clears all the tiles from the grid. </summary> */
	public void ClearTiles() {
		for (int x = 0; x < GridWidth; x++) {
			for (int y = 0; y < GridHeight; y++) {
				if (tiles[x, y] != null) {
					tiles[x, y].Uninitialize();
					if (IsTileObject)
						objects.Remove(tiles[x, y]);
					tiles[x, y] = null;
				}
			}
		}
	}
	/** <summary> Adds the tile to the grid. </summary> */
	public void AddTile(Tile tile, int gridX, int gridY) {
		AddTile(tile, new Point2I(gridX, gridY));
	}
	/** <summary> Adds the tile to the grid. </summary> */
	public void AddTile(Tile tile, Point2I gridPosition) {
		if (gridPosition >= 0 && gridPosition < GridSize) {
			if (tiles[gridPosition.X, gridPosition.Y] != null) {
				tiles[gridPosition.X, gridPosition.Y].Uninitialize();
				if (IsTileObject)
					objects[objects.IndexOf(tiles[gridPosition.X, gridPosition.Y])] = tile;
				tiles[gridPosition.X, gridPosition.Y] = tile;
				tiles[gridPosition.X, gridPosition.Y].InitializeGridPosition(gridPosition);
				if (IsInitialized)
					tiles[gridPosition.X, gridPosition.Y].Initialize(this);
			}
			else {
				if (IsTileObject)
					objects.Add(tile);
				tiles[gridPosition.X, gridPosition.Y] = tile;
				tiles[gridPosition.X, gridPosition.Y].InitializeGridPosition(gridPosition);
				if (IsInitialized)
					tiles[gridPosition.X, gridPosition.Y].Initialize(this);
			}
		}
	}
	/** <summary> Removes the tile from the grid. </summary> */
	public void RemoveTile(Tile tile) {
		for (int x = 0; x < GridWidth; x++) {
			for (int y = 0; y < GridHeight; y++) {
				if (tiles[x, y] == tile) {
					tiles[x, y].Uninitialize();
					if (IsTileObject)
						objects.Remove(tiles[x, y]);
					tiles[x, y] = null;
					return;
				}
			}
		}
	}
	/** <summary> Removes the tile at the specified position from the grid. </summary> */
	public void RemoveTile(int gridX, int gridY) {
		RemoveTile(new Point2I(gridX, gridY));
	}
	/** <summary> Removes the tile at the specified position from the grid. </summary> */
	public void RemoveTile(Point2I gridPosition) {
		if (gridPosition >= 0 && gridPosition < GridSize) {
			if (tiles[gridPosition.X, gridPosition.Y] != null) {
				tiles[gridPosition.X, gridPosition.Y].Uninitialize();
				if (IsTileObject)
					objects.Remove(tiles[gridPosition.X, gridPosition.Y]);
				tiles[gridPosition.X, gridPosition.Y] = null;
			}
		}
	}
	/** <summary> Removes the tiles with the given ID from the grid. </summary> */
	public void RemoveTile(string id) {
		for (int x = 0; x < GridWidth; x++) {
			for (int y = 0; y < GridHeight; y++) {
				if (tiles[x, y] != null) {
					if (tiles[x, y].ID == id) {
						tiles[x, y].Uninitialize();
						if (IsTileObject)
							objects.Remove(tiles[x, y]);
						tiles[x, y] = null;
					}
				}
			}
		}
	}
	/** <summary> Gets the tile at the specified position in the grid. </summary> */
	public Tile GetTile(int gridX, int gridY) {
		return GetTile(new Point2I(gridX, gridY));
	}
	/** <summary> Gets the tile at the specified position in the grid. </summary> */
	public Tile GetTile(Point2I gridPosition) {
		if (gridPosition >= 0 && gridPosition < GridSize) {
			return tiles[gridPosition.X, gridPosition.Y];
		}
		return null;
	}
	/** <summary> Gets the tile with the given ID in the grid. </summary> */
	public Tile GetTile(string id) {
		for (int x = 0; x < GridWidth; x++) {
			for (int y = 0; y < GridHeight; y++) {
				if (tiles[x, y] != null) {
					if (tiles[x, y].ID == id) {
						return tiles[x, y];
					}
				}
			}
		}
		return null;
	}
	/** <summary> Gets the list of tiles with the given ID in the grid. </summary> */
	public Tile[] GetTiles(string id) {
		List<Tile> tileList = new List<Tile>();
		for (int x = 0; x < GridWidth; x++) {
			for (int y = 0; y < GridHeight; y++) {
				if (tiles[x, y] != null) {
					if (tiles[x, y].ID == id) {
						tileList.Add(tiles[x, y]);
					}
				}
			}
		}
		return tileList.ToArray();
	}
	/** <summary> Returns true if a tile exists at the specified position in the grid. </summary> */
	public bool TileExists(int gridX, int gridY) {
		return TileExists(new Point2I(gridX, gridY));
	}
	/** <summary> Returns true if a tile exists at the specified position in the grid. </summary> */
	public bool TileExists(Point2I gridPosition) {
		if (gridPosition >= 0 && gridPosition < GridSize) {
			return tiles[gridPosition.X, gridPosition.Y] != null;
		}
		return false;
	}
	/** <summary> Returns true if the specified tile exists in the grid. </summary> */
	public bool TileExists(Tile tile) {
		for (int x = 0; x < GridWidth; x++) {
			for (int y = 0; y < GridHeight; y++) {
				if (tiles[x, y] == tile) {
					return true;
				}
			}
		}
		return false;
	}
	/** <summary> Returns true if a tile with the given ID exists in the grid. </summary> */
	public bool TileExists(string id) {
		for (int x = 0; x < GridWidth; x++) {
			for (int y = 0; y < GridHeight; y++) {
				if (tiles[x, y] != null) {
					if (tiles[x, y].ID == id) {
						return true;
					}
				}
			}
		}
		return false;
	}

	#endregion
}
} // End namespace
