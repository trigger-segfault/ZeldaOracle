using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.GameStates;
using ZeldaOracle.Game.Main;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Worlds;

namespace ZeldaOracle.Game.Control {

	// Handles the main Zelda gameplay within a room.
	public class RoomControl : GameState {

		private World			world;
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
			world			= null;
			level			= null;
			room			= null;
			player			= null;
			tiles			= null;
			roomLocation	= Point2I.Zero;
			entities		= new List<Entity>();
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
						Tileset tileset = GameData.TILESET_DEFAULT;
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
							Tile t = data.Tileset.CreateTile(data.SheetLocation);
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


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnBegin() {

			// Load test level/room.
			level = LoadLevel("Content/Worlds/test_level.zwd");

			// Create the player.
			player = new Player();
			player.Initialize(this);
			entities.Add(player);
			player.X = 48;
			player.Y = 32;

			// Setup the room.
			roomLocation = new Point2I(2, 1);
			BeginRoom(level.GetRoom(roomLocation));

		}
		
		public override void OnEnd() {
			
		}

		public override void Update(float timeDelta) {
			// TODO: Check for opening pause menu or map screens.

			// Update entities.
			for (int i = 0; i < entities.Count; ++i) {
				if (entities[i].IsAlive) {
					entities[i].Update(timeDelta);
				}
				if (!entities[i].IsAlive) {
					entities.RemoveAt(i--);
				}
			}
			
			// Update tiles.
			for (int x = 0; x < room.Width; x++) {
				for (int y = 0; y < room.Height; y++) {
					for (int i = 0; i < room.LayerCount; i++) {
						Tile t = tiles[x, y, i];
						if (t != null)
							t.Update(timeDelta);
					}
				}
			}

			// Room transitions.
			if (player.X < 0) {
				roomLocation.X -= 1;
				if (roomLocation.X < 0)
					roomLocation.X = level.Width - 1;
				BeginRoom(level.GetRoom(roomLocation));
				player.X += room.Width * GameSettings.TILE_SIZE;
			}
			else if (player.Y < 0) {
				roomLocation.Y -= 1;
				if (roomLocation.Y < 0)
					roomLocation.Y = level.Height - 1;
				BeginRoom(level.GetRoom(roomLocation));
				player.Y += room.Height * GameSettings.TILE_SIZE;
			}
			else if (player.X >= room.Width * GameSettings.TILE_SIZE) {
				roomLocation.X += 1;
				if (roomLocation.X >= level.Width)
					roomLocation.X = 0;
				BeginRoom(level.GetRoom(roomLocation));
				player.X -= room.Width * GameSettings.TILE_SIZE;
			}
			else if (player.Y >= room.Height * GameSettings.TILE_SIZE) {
				roomLocation.Y += 1;
				if (roomLocation.Y >= level.Height)
					roomLocation.Y = 0;
				BeginRoom(level.GetRoom(roomLocation));
				player.Y -= room.Height * GameSettings.TILE_SIZE;
			}

		}

		public override void Draw(Graphics2D g) {
			// Draw tiles.
			for (int x = 0; x < room.Width; x++) {
				for (int y = 0; y < room.Height; y++) {
					for (int i = 0; i < room.LayerCount; i++) {
						Tile t = tiles[x, y, i];
						if (t != null)
							t.Draw(g);
					}
				}
			}
			
			// Draw entities.
			for (int i = 0; i < entities.Count; ++i) {
				entities[i].Draw(g);
			}

			// TODO: Draw HUD.
		}

		
		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		// The world context of the game.
		public World World {
			get { return world; }
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
	}
}
