using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Properties;
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
using ZeldaOracle.Game.Tiles.Custom;
using ZeldaOracle.Game.GameStates.RoomStates;

namespace ZeldaOracle.Game.Control {

	// Handles the main Zelda gameplay within a room.
	public class RoomControl : GameState {

		private Level			level;
		private Room			room;
		private Point2I			roomLocation;
		private Player			player;
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
			e.Position = position;
			e.Initialize(this);
			entities.Add(e);
		}
		
		// Use this for spawning entites at a position at runtime.
		public void SpawnEntity(Entity e, Vector2F position, float zPosition) {
			e.Position = position;
			e.ZPosition = zPosition;
			e.Initialize(this);
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
			room.Zone = GameData.ZONE_SUMMER;

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
			this.room			= room;
			this.roomLocation	= room.Location;
			this.level			= room.Level;

			// Clear all entities from the old room (except for the player).
			entities.Clear();
			if (player != null) {
				player.Initialize(this);
				entities.Add(player);
			}

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

		public void DestroyRoom() {
			// Set all entities to destroyed (except the player).
			for (int i = 0; i < entities.Count; i++) {
				if (entities[i] != player)
					entities[i].IsDestroyed = true;
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
		
		public void BeginTestWorld(Player player) {

			// Load test level/room.
			level = LoadLevel("Content/Worlds/test_level.zwd");
			//level = LoadLevel("Content/Worlds/interiors.zwd");

			// Create the player.
			this.player = player;
			player.Initialize(this);
			entities.Add(player);
			player.X = 48;
			player.Y = 32;
			
			// Create a movable block tile.
			TileData tdBlock		= new TileData(TileFlags.Solid | TileFlags.Movable);
			tdBlock.Sprite			= GameData.SPR_TILE_MOVABLE_BLOCK;
			tdBlock.SpriteAsObject	= GameData.SPR_TILE_MOVABLE_BLOCK_ASOBJECT;
			tdBlock.CollisionModel	= GameData.MODEL_BLOCK;
			
			// Create a diamond rock tile.
			TileData tdDiamond			= new TileData(TileFlags.Solid | TileFlags.Switchable | TileFlags.SwitchStays);
			tdDiamond.Sprite			= GameData.SPR_TILE_DIAMOND_ROCK;
			tdDiamond.SpriteAsObject	= GameData.SPR_TILE_DIAMOND_ROCK_ASOBJECT;
			tdDiamond.CollisionModel	= GameData.MODEL_BLOCK;
			
			// Create a bush tile.
			TileData tdBush		= new TileData(TileFlags.Solid | TileFlags.Pickupable | TileFlags.Bombable |
									TileFlags.Burnable | TileFlags.Switchable | TileFlags.Cuttable);
			tdBush.Sprite			= GameData.SPR_TILE_BUSH;
			tdBush.SpriteAsObject	= GameData.SPR_TILE_BUSH_ASOBJECT;
			tdBush.BreakAnimation	= GameData.ANIM_EFFECT_LEAVES;
			tdBush.CollisionModel	= GameData.MODEL_BLOCK;
			
			// Create a pot tile.
			TileData tdPot		= new TileData(TileFlags.Solid | TileFlags.Pickupable |
									TileFlags.Cuttable | TileFlags.Switchable | TileFlags.Movable);
			tdPot.Sprite			= GameData.SPR_TILE_POT;
			tdPot.SpriteAsObject	= GameData.SPR_TILE_POT_ASOBJECT;
			tdPot.BreakAnimation	= GameData.ANIM_EFFECT_ROCK_BREAK;
			tdPot.CollisionModel	= GameData.MODEL_BLOCK;
			
			// Create a rock tile.
			TileData tdRock			= new TileData(TileFlags.Solid | TileFlags.Pickupable);
			tdRock.Sprite			= GameData.SPR_TILE_ROCK;
			tdRock.SpriteAsObject	= GameData.SPR_TILE_ROCK_ASOBJECT;
			tdRock.BreakAnimation	= GameData.ANIM_EFFECT_ROCK_BREAK;
			tdRock.CollisionModel	= GameData.MODEL_BLOCK;
			
			// Create a grass tile.
			TileData tdGrass		= new TileData(TileFlags.Grass | TileFlags.Cuttable | TileFlags.Burnable | TileFlags.Bombable);
			tdGrass.Sprite			= GameData.SPR_TILE_GRASS;
			tdGrass.BreakAnimation	= GameData.ANIM_EFFECT_GRASS_LEAVES;
			
			// Create an owl tile.
			TileData tdOwl			= new TileData(typeof(TileOwl), TileFlags.Solid);
			tdOwl.Sprite			= GameData.SPR_TILE_OWL;
			tdOwl.CollisionModel	= GameData.MODEL_BLOCK;
			tdOwl.Properties.Set("text", "You have been spooked by the <blue>Spooky Owl<blue>!");
			
			// Create a lantern tile.
			TileData tdLantern			= new TileData(typeof(TileLantern), TileFlags.Solid);
			tdLantern.Sprite			= GameData.SPR_TILE_LANTERN_UNLIT;
			tdLantern.CollisionModel	= GameData.MODEL_BLOCK;
			tdLantern.Properties.Set("lit", true);

			// Create a Sign tile
			TileData tdSign = new TileData(typeof(TileSign), TileFlags.Solid | TileFlags.Pickupable |
				TileFlags.Burnable | TileFlags.Cuttable | TileFlags.Switchable);
			tdSign.Sprite			= GameData.SPR_TILE_SIGN;
			tdSign.SpriteAsObject	= GameData.SPR_TILE_SIGN_ASOBJECT;
			tdSign.BreakAnimation	= GameData.ANIM_EFFECT_SIGN_BREAK;
			tdSign.CollisionModel	= GameData.MODEL_BLOCK;
			tdSign.Properties.Set("text", "Hello, World!");
			tdSign.Properties.Add(Property.CreateList("list",
					Property.CreateString("1", "one"),
					Property.CreateString("2", "two"),
					Property.CreateString("3", "three")));

			// Create a chest tile.
			TileData tdChest		= new TileData(typeof(TileChest), TileFlags.Solid);
			tdChest.Sprite			= GameData.SPR_TILE_CHEST;
			tdChest.CollisionModel	= GameData.MODEL_BLOCK;
			tdChest.Properties.Set("reward", "rupees_1");

			// Create a reward tile (flippers).
			TileData tdFlippers		= new TileData(typeof(TileReward), TileFlags.Solid);
			tdFlippers.CollisionModel	= GameData.MODEL_CENTER;
			tdFlippers.Properties.Set("reward", "item_flippers_1");
			
			// Create a reward tile (heart piece).
			TileData tdHeartPiece		= new TileData(typeof(TileReward), TileFlags.Solid);
			tdHeartPiece.CollisionModel	= GameData.MODEL_CENTER;
			tdHeartPiece.Properties.Set("reward", "heart_piece");

			/*
			Room r;
			r = level.GetRoom(new Point2I(2, 1));
			r.Zone = GameData.ZONE_INTERIOR;
			r.TileData[1, 2, 1] = tdPot;
			r.TileData[1, 3, 1] = tdPot;
			r.TileData[5, 1, 1] = tdPot;
			r = level.GetRoom(new Point2I(3, 1));
			r.Zone = GameData.ZONE_INTERIOR;
			r.TileData[8, 1, 1] = tdChest;
			r.TileData[8, 2, 1] = tdPot;
			r.TileData[4, 6, 1] = tdPot;
			r.TileData[5, 6, 1] = tdPot;
			r.TileData[6, 6, 1] = tdPot;
			r.TileData[7, 6, 1] = tdPot;
			r.TileData[8, 6, 1] = tdPot;
			*/

			// Setup the rooms.
			Room r;
			r = level.GetRoom(new Point2I(2, 1));
			r.TileData[8, 1, 1] = tdOwl;
			r.TileData[7, 1, 1] = tdChest;
			r.TileData[6, 3, 1] = tdFlippers;
			r.TileData[1, 1, 1] = tdSign;
			r.TileData[2, 5, 1] = tdBlock;
			r.TileData[2, 6, 1] = tdHeartPiece;
			r.TileData[2, 2, 1] = tdGrass;
			r.TileData[2, 3, 1] = tdGrass;
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
			
			r = level.GetRoom(new Point2I(1, 1));
			r.TileData[1, 1, 1] = tdDiamond;
			r.TileData[2, 2, 1] = tdDiamond;
			r.TileData[2, 4, 1] = tdDiamond;
			r.TileData[8, 2, 1] = tdPot;
			
			r = level.GetRoom(new Point2I(3, 0));
			r.TileData[3, 2, 1] = tdLantern;

			r = level.GetRoom(new Point2I(1, 0));
			r.TileData[8, 2, 1] = tdRock;

			r = level.GetRoom(new Point2I(2, 0));
			for (int x = 1; x < 8; x++) {
				for (int y = 2; y < 6; y++) {
					r.TileData[x, y, 1] = tdBush;
				}
			}

			// Set the rooms to random zones.
			/*Random random = new Random();
			for (int x = 0; x < level.Width; x++) {
				for (int y = 0; y < level.Height; y++) {
					int index = random.Next(0, 3);
					Zone zone = GameData.ZONE_SUMMER;
					if (index == 1)
						zone = GameData.ZONE_GRAVEYARD;
					else if (index == 2)
						zone = GameData.ZONE_FOREST;
					level.GetRoom(new Point2I(x, y)).Zone = zone;
				}
			}*/
			
			{
				WorldFile worldFile = new WorldFile();
				World world = new World();
				world.StartLevelIndex = 0;
				world.StartRoomLocation = new Point2I(2, 1);
				world.StartTileLocation = new Point2I(3, 2);
				world.Levels.Add(level);
				level.Name = "Level 1";
				worldFile.Save("Content/Worlds/temp_world.zwd", world);
			}
			{
				WorldFile worldFile = new WorldFile();
				World world = worldFile.Load("Content/Worlds/temp_world.zwd");
				level = world.Levels[0];
			}

			roomLocation = new Point2I(2, 1);
			BeginRoom(level.GetRoom(roomLocation));
		}

		public override void OnBegin() {
			GameControl.RoomControl = this;
		}
		
		public override void OnEnd() {
			
		}

		public override void Update() {
			GameControl.RoomTicks++;

			RoomState roomState = GameControl.CurrentRoomState;
			GameControl.UpdateRoom = roomState.UpdateRoom;
			GameControl.AnimateRoom = roomState.AnimateRoom;

			// Update entities.
			int entityCount = entities.Count;
			for (int i = 0; i < entities.Count; i++) {
				if (entities[i].IsAlive && entities[i].IsInRoom && i < entityCount) {
					if (GameControl.UpdateRoom)
						entities[i].Update();
					if (GameControl.AnimateRoom)
						entities[i].UpdateGraphics();
				}
				else if (entities[i].IsAlive && entities[i].IsInRoom && i >= entityCount) {
					// For entities spawned this frame, only update their graphics component.
					entities[i].Graphics.SyncAnimation();
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
						if (t != null) {
							if (GameControl.UpdateRoom)
								t.Update();
							if (GameControl.AnimateRoom)
								t.UpdateGraphics();
						}
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

			GameControl.HUD.Update();

			GameControl.UpdateRoomState();

			if (GameControl.UpdateRoom) {
				// [Start] Open inventory.
				if (Controls.Start.IsPressed()) {
					GameControl.OpenMenu(GameControl.MenuWeapons);
				}


				// DEBUG KEYS:

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
				if (Keyboard.IsKeyPressed(Keys.Q)) {
					int[] rupees = { 1, 5, 20, 100, 200 };//, 5, 20, 100, 200 };
					int rupee = GRandom.NextInt(rupees.Length);
					Collectible collectible = GameControl.RewardManager.SpawnCollectible("rupees_" + rupees[rupee].ToString());
					collectible.Position = player.Position;
					collectible.ZPosition = 100;
				}
				if (Keyboard.IsKeyPressed(Keys.Y)) {
					GraphicsComponent.DrawCollisionBoxes = !GraphicsComponent.DrawCollisionBoxes;
				}
				if (Keyboard.IsKeyPressed(Keys.K)) {
					Collectible collectible = GameControl.RewardManager.SpawnCollectible("hearts_1");
					collectible.Position = player.Position;
					collectible.ZPosition = 100;
				}
				if (Keyboard.IsKeyPressed(Keys.B)) {
					Collectible collectible = GameControl.RewardManager.SpawnCollectible("ammo_bombs_5");
					collectible.Position = player.Position;
					collectible.ZPosition = 100;
				}
				if (Keyboard.IsKeyPressed(Keys.J)) {
					Collectible collectible = GameControl.RewardManager.SpawnCollectible("ammo_arrows_5");
					collectible.Position = player.Position;
					collectible.ZPosition = 100;
				}
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
			g.End();
			g.Begin(GameSettings.DRAW_MODE_BACK_TO_FRONT);
			for (int i = 0; i < entities.Count; ++i) {
				entities[i].Draw(g);
			}

			// Draw HUD.
			g.End();
			g.Begin(GameSettings.DRAW_MODE_DEFAULT);
			g.ResetTranslation();
			GameControl.HUD.Draw(g, false);

			GameControl.DrawRoomState(g);
		}

		
		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

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
			set { player = value; }
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
