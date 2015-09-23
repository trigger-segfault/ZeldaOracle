using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.GameStates;
using ZeldaOracle.Game.GameStates.Transitions;
using ZeldaOracle.Game.Main;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Worlds;
using ZeldaOracle.Game.Control.Menus;
using ZeldaOracle.Common.Input;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Game.Items.Rewards;

namespace ZeldaOracle.Game.Control {

	// Handles the main Zelda gameplay within a room.
	public class RoomControl : GameState {

		private Level			level;
		private Room			room;
		private Point2I			roomLocation;
		private Player			player;			// The player entity.
		private List<Entity>	entities;
		private Tile[,,]		tiles;
		

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public RoomControl() {
			level			= null;
			room			= null;
			player			= null;
			tiles			= null;
			roomLocation	= Point2I.Zero;
			entities		= new List<Entity>();
		}
		

		//-----------------------------------------------------------------------------
		// Accessors
		//-----------------------------------------------------------------------------
		
		// Return the tile at the given location (can return null).
		public Tile GetTile(Point2I location, int layer) {
			return tiles[location.X, location.Y, layer];
		}

		// Return the tile at the given location (can return null).
		public Tile GetTile(int x, int y, int layer) {
			return tiles[x, y, layer];
		}

		// Return the tile at the given location that's on the highest layer.
		public Tile GetTopTile(int x, int y) {
			for (int i = room.LayerCount - 1; i >= 0; i--) {
				if (tiles[x, y, i] != null)
					return tiles[x, y, i];
			}
			return null;
		}

		// Return the tile at the given location that's on the highest layer.
		public Tile GetTopTile(Point2I location) {
			for (int i = room.LayerCount - 1; i >= 0; i--) {
				if (tiles[location.X, location.Y, i] != null)
					return tiles[location.X, location.Y, i];
			}
			return null;
		}
		
		// Return true if the given tile location is inside the room.
		public bool IsTileInBounds(Point2I location, int layer = 0) {
			return (location.X >= 0 && location.X < room.Width &&
					location.Y >= 0 && location.Y < room.Height &&
					layer >= 0 && layer < room.LayerCount);
		}

		// Return the tile location that the given position in pixels is situated in.
		public Point2I GetTileLocation(Vector2F position) {
			return (Point2I) (position / GameSettings.TILE_SIZE);
		}

		public Rectangle2I GetTileAreaFromRect(Rectangle2F rect, int inflateAmount = 0) {
			Rectangle2I area;
			area.Point	= (Point2I) (rect.TopLeft / (float) GameSettings.TILE_SIZE);
			area.Size	= ((Point2I) (rect.BottomRight / (float) GameSettings.TILE_SIZE)) + Point2I.One - area.Point;
			area.Inflate(inflateAmount, inflateAmount);
			return Rectangle2I.Intersect(area, new Rectangle2I(Point2I.Zero, room.Size));
		}
		

		//-----------------------------------------------------------------------------
		// Mutators
		//-----------------------------------------------------------------------------
		
		
		// Move the given tile to a new location.
		public void MoveTile(Tile tile, Point2I newLocation, int newLayer) {
			tiles[tile.Location.X, tile.Location.Y, tile.Layer] = null;
			tiles[newLocation.X, newLocation.Y, newLayer] = tile;
			tile.Location = newLocation;
			tile.Layer = newLayer;
		}


		//-----------------------------------------------------------------------------
		// Manipulation
		//-----------------------------------------------------------------------------
		
		// Use this for spawning entites at runtime.
		public void SpawnEntity(Entity e) {
			e.Initialize(this);
			entities.Add(e);
		}
		
		// Use this for spawning entites at a position at runtime.
		public void SpawnEntity(Entity e, Vector2F position) {
			e.Initialize(this);
			e.Position = position;
			entities.Add(e);
		}
		
		// Use this for spawning entites at a position at runtime.
		public void SpawnEntity(Entity e, Vector2F position, float zPosition) {
			e.Initialize(this);
			e.Position = position;
			e.ZPosition = zPosition;
			entities.Add(e);
		}
		
		// Use this for placing tiles at runtime.
		public void PlaceTile(Tile tile, Point2I location, int layer) {
			PlaceTile(tile, location.X, location.Y, layer);
		}

		// Use this for placing tiles at runtime.
		public void PlaceTile(Tile tile, int x, int y, int layer) {
			tile.Initialize(this);
			tiles[x, y, layer] = tile;
		}

		// Use this for placing tiles at runtime.
		public void RemoveTile(Tile tile) {
			// TODO: OnRemove?
			tiles[tile.Location.X, tile.Location.Y, tile.Layer] = null;
		}


		//-----------------------------------------------------------------------------
		// Temp Loading
		//-----------------------------------------------------------------------------

		public Level LoadLevel(string filename) {
            BinaryReader bin = new BinaryReader(File.OpenRead(filename));
			
			int width = bin.ReadByte();
			int height = bin.ReadByte();
			Level level = new Level(width, height, GameSettings.ROOM_SIZE_SMALL);

			// Load the rooms in the level.
			for (int y = 0; y < height; y++) {
				for (int x = 0; x < width; x++) {
					level.Rooms[x, y] = LoadRoom(bin, level, x, y);
				}
			}

            bin.Close();
            return level;
		}

		public Room LoadRoom(BinaryReader bin, Level level, int locX, int locY) {
			byte width, height, tilesetIndex, tilesetSourceX, tilesetSourceY;
			
			width = bin.ReadByte();
			height = bin.ReadByte();
			level.RoomSize = new Point2I(width, height);
			Room room = new Room(level, locX, locY);

			// Read the tile data.
			for (int y = 0; y < height; y++) {
				for (int x = 0; x < width; x++) {
					tilesetIndex = bin.ReadByte();

					if (tilesetIndex > 0) {
						Tileset tileset = GameData.TILESETS[tilesetIndex - 1];
						tilesetSourceX = bin.ReadByte();
						tilesetSourceY = bin.ReadByte();
						room.TileData[x, y, 0] = tileset.TileData[tilesetSourceX, tilesetSourceY];
					}
					else {
						// Only use default tiles on bottom layer.
						Tileset tileset = GameData.TILESET_OVERWORLD;
						room.TileData[x, y, 0] = tileset.TileData[tileset.DefaultTile.X, tileset.DefaultTile.Y];
					}
					
				}
			}

			return room;
		}


		//-----------------------------------------------------------------------------
		// Initialization
		//-----------------------------------------------------------------------------

		public void BeginRoom(Room room) {
			this.room = room;

			// Clear all entities from the old room (except for the player).
			entities.Clear();
			if (player != null)
				entities.Add(player);

			// Create the new tile grid.
			tiles = new Tile[room.Width, room.Height, room.LayerCount];
			for (int x = 0; x < room.Width; x++) {
				for (int y = 0; y < room.Height; y++) {
					for (int i = 0; i < room.LayerCount; i++) {
						TileData data = room.TileData[x, y, i];

						if (data == null) {
							tiles[x, y, i] = null;
						}
						else {
							Tile t;
							if (data.Tileset != null)
								t = data.Tileset.CreateTile(data.SheetLocation);
							else 
								t = Tile.CreateTile(data);
							t.Location = new Point2I(x, y);
							t.Layer = i;
							t.Initialize(this);
							tiles[x, y, i] = t;
						}
					}
				}
			}
			
			// Initialize the tiles.
			for (int x = 0; x < room.Width; x++) {
				for (int y = 0; y < room.Height; y++) {
					for (int i = 0; i < room.LayerCount; i++) {
						Tile t = tiles[x, y, i];
						if (t != null)
							t.Initialize(this);
					}
				}
			}


		}
		
		public void EnterAdjacentRoom(int direction) {
			// Find the adjacent room.
			Point2I relative = Directions.ToPoint(direction);
			Point2I nextLocation = roomLocation + relative;
			if (!level.ContainsRoom(nextLocation))
				return;
			Room nextRoom = level.GetRoom(nextLocation);

			// Setup the new room control.
			RoomControl newControl = new RoomControl();
			newControl.gameManager	= gameManager;
			newControl.level		= level;
			newControl.room			= nextRoom;
			newControl.roomLocation	= nextLocation;
			newControl.player		= player;
			newControl.BeginRoom(nextRoom);
			entities.Remove(player);
			
			// Move the player to the new room.
			player.RoomControl = newControl;
			player.Position -= relative * nextRoom.Size * GameSettings.TILE_SIZE;

			// Play the transition.
			RoomTransitionPush transition = new RoomTransitionPush(this, newControl, direction);
			gameManager.PopGameState();
			gameManager.PushGameState(transition);
			GameControl.RoomControl = newControl;
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------
		
		public void BeginTestWorld() {

			// Load test level/room.
			level = LoadLevel("Content/Worlds/test_level.zwd");
			//level = LoadLevel("Content/Worlds/ledge_jump_world.zwd");

			// Create the player.
			player = new Player();
			player.Initialize(this);
			entities.Add(player);
			player.X = 48;
			player.Y = 32;

			// Setup the room.
			roomLocation = new Point2I(2, 1);
			Room r = level.GetRoom(roomLocation);
			
			// Create an owl tile.
			TileData tdOwl			= new TileData(typeof(TileOwl), TileFlags.Solid);
			tdOwl.Sprite			= GameData.SPR_OWL;
			tdOwl.CollisionModel	= GameData.MODEL_BLOCK;
			
			// Create a lantern tile.
			TileData tdLantern			= new TileData(typeof(TileLantern), TileFlags.Solid);
			tdLantern.Sprite			= GameData.SPR_LANTERN_UNLIT;
			tdLantern.CollisionModel	= GameData.MODEL_BLOCK;
			
			// Create a Sign tile
			TileData tdSign = new TileData(typeof(TileSign), TileFlags.Solid | TileFlags.Pickupable |
				TileFlags.Burnable | TileFlags.Cuttable);
			tdSign.Sprite			= new Sprite(GameData.SHEET_ZONESET_LARGE, 5, 0);
			tdSign.SpriteAsObject	= new Sprite(GameData.SHEET_ZONESET_LARGE, 5, 1);
			tdSign.BreakAnimation	= GameData.ANIM_EFFECT_SIGN_BREAK;
			tdSign.CollisionModel	= GameData.MODEL_BLOCK;
			
			// Create a movable block tile.
			TileData tdBlock		= new TileData(TileFlags.Solid | TileFlags.Movable);
			tdBlock.Sprite			= new Sprite(GameData.SHEET_ZONESET_LARGE, 1, 9);
			tdBlock.SpriteAsObject	= new Sprite(GameData.SHEET_ZONESET_LARGE, 2, 9);
			tdBlock.CollisionModel	= GameData.MODEL_BLOCK;
			
			// Create a bush tile.
			TileData tdBush		= new TileData(TileFlags.Solid | TileFlags.Pickupable | TileFlags.Bombable |
									TileFlags.Burnable | TileFlags.Switchable | TileFlags.Cuttable);
			tdBush.Sprite			= new Sprite(GameData.SHEET_ZONESET_LARGE, 0, 0);
			tdBush.SpriteAsObject	= new Sprite(GameData.SHEET_ZONESET_LARGE, 0, 1);
			tdBush.BreakAnimation	= GameData.ANIM_EFFECT_LEAVES;
			tdBush.CollisionModel	= GameData.MODEL_BLOCK;
			
			// Create a pot tile.
			TileData tdPot		= new TileData(TileFlags.Solid | TileFlags.Pickupable |
									TileFlags.Cuttable | TileFlags.Switchable | TileFlags.Movable);
			tdPot.Sprite			= new Sprite(GameData.SHEET_ZONESET_LARGE, 2, 0);
			tdPot.SpriteAsObject	= new Sprite(GameData.SHEET_ZONESET_LARGE, 2, 1);
			tdPot.BreakAnimation	= GameData.ANIM_EFFECT_ROCK_BREAK;
			tdPot.CollisionModel	= GameData.MODEL_BLOCK;
			
			// Create a rock tile.
			TileData tdRock			= new TileData(TileFlags.Solid | TileFlags.Pickupable);
			tdRock.Sprite			= new Sprite(GameData.SHEET_ZONESET_LARGE, 3, 0);
			tdRock.SpriteAsObject	= new Sprite(GameData.SHEET_ZONESET_LARGE, 3, 1);
			tdRock.BreakAnimation	= GameData.ANIM_EFFECT_ROCK_BREAK;
			tdRock.CollisionModel	= GameData.MODEL_BLOCK;
			
			// Create a grass tile.
			TileData tdGrass		= new TileData(TileFlags.Grass | TileFlags.Cuttable | TileFlags.Burnable | TileFlags.Bombable);
			tdGrass.Sprite			= new Sprite(GameData.SHEET_ZONESET_LARGE, 0, 3);
			tdGrass.BreakAnimation	= GameData.ANIM_EFFECT_GRASS_LEAVES;

			//r.TileData[2, 1, 1] = tdLantern;
			r.TileData[2, 1, 1] = tdOwl;
			r.TileData[2, 3, 1] = tdSign;
			r.TileData[2, 5, 1] = tdBlock;
			r.TileData[1, 1, 1] = tdBush;
			r.TileData[1, 2, 1] = tdPot;
			//r.TileData[2, 1, 1] = tdRock;

			r.TileData[2, 2, 1] = tdGrass;
			//r.TileData[2, 3, 1] = td;
			r.TileData[2, 4, 1] = tdGrass;
			r.TileData[3, 4, 1] = tdGrass;
			r.TileData[4, 5, 1] = tdGrass;
			r.TileData[3, 6, 1] = tdGrass;
			r.TileData[4, 6, 1] = tdGrass;
			r.TileData[5, 6, 1] = tdGrass;
			r.TileData[4, 7, 1] = tdGrass;
			r.TileData[5, 7, 1] = tdGrass;
			r.TileData[6, 7, 1] = tdGrass;
			r.TileData[7, 7, 1] = tdGrass;
			r.TileData[7, 2, 1] = tdGrass;
			r.TileData[8, 2, 1] = tdGrass;
			r.TileData[8, 3, 1] = tdGrass;

			BeginRoom(r);
		}

		public override void OnBegin() {
			GameControl.RoomControl = this;
		}
		
		public override void OnEnd() {
			
		}

		public override void Update() {
			GameControl.RoomTicks++;

			// Update entities.
			int entityCount = entities.Count;
			for (int i = 0; i < entities.Count; i++) {
				if (entities[i].IsAlive && entities[i].IsInRoom && i < entityCount) {
					entities[i].Update();
				}
				else if (entities[i].IsAlive && entities[i].IsInRoom && i >= entityCount) {
					// For entities spawned this frame, only update their graphics component.
					entities[i].Graphics.AnimationPlayer.SubStripIndex = entities[i].Graphics.SubStripIndex;
					//entities[i].Graphics.Update();
				}
			}

			// Remove destroyed entities.
			for (int i = 0; i < entities.Count; i++) {
				
				if (!entities[i].IsAlive || !entities[i].IsInRoom) {
					entities.RemoveAt(i--);
				}
			}
			
			// Update tiles.
			for (int i = 0; i < room.LayerCount; i++) {
				for (int x = 0; x < room.Width; x++) {
					for (int y = 0; y < room.Height; y++) {
						Tile t = tiles[x, y, i];
						if (t != null)
							t.Update();
					}
				}
			}

			// Detect room transitions.
			if (player.AllowRoomTransition) {
				for (int direction = 0; direction < Directions.Count; direction++) {
					CollisionInfo info = player.Physics.CollisionInfo[direction];

					if (info.Type == CollisionType.RoomEdge &&
						(Controls.GetArrowControl(direction).IsDown() || Controls.GetAnalogDirection(direction) || player.AutoRoomTransition))
					{
						EnterAdjacentRoom(direction);
						break;
					}
				}
			}

			// [Start] Open inventory.
			if (Controls.Start.IsPressed()) {
				GameControl.OpenMenu(GameControl.MenuWeapons);
			}
			if (Keyboard.IsKeyPressed(Keys.G)) {
				GameControl.DisplayMessage("I was a <red>hero<red> to broken robots 'cause I was one of them, but how can I sing about being damaged if I'm not?<p> That's like <green>Christina Aguilera<green> singing Spanish. Ooh, wait! That's it! I'll fake it!");
			}
			if (Keyboard.IsKeyPressed(Keys.Insert)) {
				GameControl.Inventory.FillAllAmmo();
			}
			if (Keyboard.IsKeyPressed(Keys.Delete)) {
				GameControl.Inventory.EmptyAllAmmo();
			}
			if (Keyboard.IsKeyPressed(Keys.Home)) {
				GameControl.Player.MaxHealth = 4 * 14;
				GameControl.Player.Health = GameControl.Player.MaxHealth;
			}
			if (Keyboard.IsKeyPressed(Keys.End)) {
				GameControl.Player.Health = 4 * 3;
			}
			GameControl.HUD.Update();

			if (Keyboard.IsKeyPressed(Keys.T)) {
				switch (player.Tunic) {
				case PlayerTunics.GreenTunic: player.Tunic = PlayerTunics.RedTunic; break;
				case PlayerTunics.RedTunic: player.Tunic = PlayerTunics.BlueTunic; break;
				case PlayerTunics.BlueTunic: player.Tunic = PlayerTunics.GreenTunic; break;
				}
			}
			if (Keyboard.IsKeyPressed(Keys.H)) {
				player.Hurt(0);
			}
			if (Keyboard.IsKeyPressed(Keys.M)) {
				AudioSystem.PlaySong("overworld");
			}
			if (Keyboard.IsKeyPressed(Keys.N)) {
				AudioSystem.MasterVolume = 1.0f;
			}
			if (Keyboard.IsKeyPressed(Keys.R) || Keyboard.IsKeyPressed(Keys.E)) {
				int sprite = Keyboard.IsKeyPressed(Keys.E) ? 6 : 5;
				int amount = Keyboard.IsKeyPressed(Keys.E) ? 5 : 1;
				Reward reward = new RewardRupee(
					"rupee_" + amount.ToString(), amount,
					new Sprite(GameData.SHEET_ITEMS_SMALL, sprite, 3, -5, -15),
					new Rectangle2I(-6, -10, 9, 9)
				);
				Collectible collectible = new Collectible(reward);
				collectible.ZPosition = 100;
				collectible.Position = player.Position;
				SpawnEntity(collectible);
			}
		}

		public override void Draw(Graphics2D g) {

			// Draw the room.
			g.Translate(0, 16);

			// Draw tiles.
			for (int i = 0; i < room.LayerCount; i++) {
				for (int x = 0; x < room.Width; x++) {
					for (int y = 0; y < room.Height; y++) {
						Tile t = tiles[x, y, i];
						if (t != null)
							t.Draw(g);
					}
				}
			}
			
			// Draw entities.
			DrawMode drawMode = new DrawMode();
			drawMode.BlendState = BlendState.AlphaBlend;
			drawMode.SortMode	= SpriteSortMode.BackToFront;
			drawMode.SamplerState = SamplerState.PointClamp;
			
			g.End();
			g.Begin(drawMode);
			for (int i = 0; i < entities.Count; ++i) {
				entities[i].Draw(g);
			}
			
			// Draw HUD.
			drawMode.SortMode = SpriteSortMode.Deferred;
			g.End();
			g.Begin(drawMode);
			g.ResetTranslation();
			GameControl.HUD.Draw(g, false);
		}

		
		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public GameControl GameControl {
			get { return gameManager.GameControl; }
		}

		// The current level that contains the room the player is in.
		public Level Level {
			get { return level; }
		}

		// The current room the player is in.
		public Room Room {
			get { return room; }
		}

		// The player entity (NOTE: this can be null)
		public Player Player {
			get { return player; }
		}

		// Get the list of entities.
		public List<Entity> Entities {
			get { return entities; }
		}

		// Get the size of the room in pixels.
		public Rectangle2I RoomBounds {
			get { return new Rectangle2I(Point2I.Zero, room.Size * GameSettings.TILE_SIZE); }
		}

		// Get the room's location within the level.
		public Point2I RoomLocation {
			get { return roomLocation; }
		}
	}
}
