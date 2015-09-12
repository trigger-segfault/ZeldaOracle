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
		private Player			player;
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
		// Test
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
					TileData data = new TileData();
					room.TileData[x, y, 0] = data;

					data.Flags = TileFlags.Default;

					tilesetIndex = bin.ReadByte();

					if (tilesetIndex > 0) {
						tilesetSourceX = bin.ReadByte(); 
						tilesetSourceY = bin.ReadByte();
						data.SheetLocation = new Point2I(tilesetSourceX, tilesetSourceY);
					}
					else {
						data.SheetLocation = new Point2I(1, 24);
					}
					
					data.Sprite = new Sprite(GameData.IMAGE_TILESET, 
						data.SheetLocation.X * 17,
						data.SheetLocation.Y * 17,
						16, 16, 0, 0);
				}
			}

			return room;
		}


		//-----------------------------------------------------------------------------
		// Initialization
		//-----------------------------------------------------------------------------

		public void SetupRoom(Room room) {
			this.room = room;

			// Create the tile grid.
			tiles = new Tile[room.Width, room.Height, room.LayerCount];
			for (int x = 0; x < room.Width; x++) {
				for (int y = 0; y < room.Height; y++) {
					for (int i = 0; i < room.LayerCount; i++) {
						TileData data = room.TileData[x, y, i];

						if (data == null) {
							tiles[x, y, i] = null;
						}
						else {
							tiles[x, y, i] = new Tile(data, x, y, i);
							tiles[x, y, i].Initialize(this, data);
						}
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

			// Setup the room.
			SetupRoom(level.Rooms[2, 1]);

			// Create the player.
			player = new Player();
			player.Initialize(this);
			entities.Add(player);

		}
		
		public override void OnEnd() {
			
		}

		public override void Update(float timeDelta) {
			// TODO: Check for opening pause menu or map screens.

			// Update entities.
			for (int i = 0; i < entities.Count; ++i) {
				entities[i].Update(timeDelta);
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
