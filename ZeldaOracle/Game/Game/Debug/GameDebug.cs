using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Properties;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.GameStates;
using ZeldaOracle.Game.GameStates.Transitions;
using ZeldaOracle.Game.Main;
using ZeldaOracle.Game.Control;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Tiles.EventTiles;
using ZeldaOracle.Game.Tiles.Custom;
using ZeldaOracle.Game.Worlds;
using ZeldaOracle.Game.Control.Menus;
using ZeldaOracle.Common.Input;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Game.Items.Rewards;
using ZeldaOracle.Game.GameStates.RoomStates;

namespace ZeldaOracle.Game.Debug {
	public class GameDebug {
		
		public static void UpdateRoomDebugKeys(RoomControl roomControl) {
			GameControl gameControl = roomControl.GameControl;

			if (Keyboard.IsKeyPressed(Keys.G)) {
				gameControl.DisplayMessage("I was a <red>hero<red> to broken robots 'cause I was one of them, but how can I sing about being damaged if I'm not?<p> That's like <green>Christina Aguilera<green> singing Spanish. Ooh, wait! That's it! I'll fake it!");
			}
			if (Keyboard.IsKeyPressed(Keys.Insert)) {
				gameControl.Inventory.FillAllAmmo();
			}
			if (Keyboard.IsKeyPressed(Keys.Delete)) {
				gameControl.Inventory.EmptyAllAmmo();
			}
			if (Keyboard.IsKeyPressed(Keys.Home)) {
				gameControl.Player.MaxHealth = 4 * 14;
				gameControl.Player.Health = gameControl.Player.MaxHealth;
			}
			if (Keyboard.IsKeyPressed(Keys.End)) {
				gameControl.Player.Health = 4 * 3;
			}

			if (Keyboard.IsKeyPressed(Keys.T)) {
				switch (gameControl.Player.Tunic) {
				case PlayerTunics.GreenTunic:	gameControl.Player.Tunic = PlayerTunics.RedTunic; break;
				case PlayerTunics.RedTunic:		gameControl.Player.Tunic = PlayerTunics.BlueTunic; break;
				case PlayerTunics.BlueTunic:	gameControl.Player.Tunic = PlayerTunics.GreenTunic; break;
				}
			}
			if (Keyboard.IsKeyPressed(Keys.H)) {
				gameControl.Player.Hurt(0);
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
				Collectible collectible = gameControl.RewardManager.SpawnCollectible("rupees_" + rupees[rupee].ToString());
				collectible.Position = gameControl.Player.Position;
				collectible.ZPosition = 100;
			}
			if (Keyboard.IsKeyPressed(Keys.Y)) {
				GraphicsComponent.DrawCollisionBoxes = !GraphicsComponent.DrawCollisionBoxes;
			}
			if (Keyboard.IsKeyPressed(Keys.K)) {
				Collectible collectible = gameControl.RewardManager.SpawnCollectible("hearts_1");
				collectible.Position = gameControl.Player.Position;
				collectible.ZPosition = 100;
			}
			if (Keyboard.IsKeyPressed(Keys.B)) {
				Collectible collectible = gameControl.RewardManager.SpawnCollectible("ammo_bombs_5");
				collectible.Position = gameControl.Player.Position;
				collectible.ZPosition = 100;
			}
			if (Keyboard.IsKeyPressed(Keys.J)) {
				Collectible collectible = gameControl.RewardManager.SpawnCollectible("ammo_arrows_5");
				collectible.Position = gameControl.Player.Position;
				collectible.ZPosition = 100;
			}
		}

		public static World CreateTestWorld() {
			// Create the world.
			World world = new World();
			world.StartLevelIndex	= 0;
			world.StartRoomLocation	= new Point2I(2, 1);
			world.StartTileLocation	= new Point2I(3, 2);
			
			// Load the levels from java level files.
			world.Levels.Add(LoadJavaLevel("Content/Worlds/test_level.zwd"));
			world.Levels.Add(LoadJavaLevel("Content/Worlds/interiors.zwd"));
			world.Levels.Add(LoadJavaLevel("Content/Worlds/big_interiors.zwd"));
			world.Levels[0].Name = "overworld";
			world.Levels[1].Name = "interiors";
			world.Levels[2].Name = "big_interiors";

			TileData tdBlock		= Resources.GetResource<TileData>("movable_block");
			TileData tdDiamond		= Resources.GetResource<TileData>("diamond_rock");
			TileData tdBush			= Resources.GetResource<TileData>("bush");
			TileData tdPot			= Resources.GetResource<TileData>("pot");
			TileData tdRock			= Resources.GetResource<TileData>("rock");
			TileData tdGrass		= Resources.GetResource<TileData>("grass");
			TileData tdOwl			= Resources.GetResource<TileData>("owl");
			TileData tdLantern		= Resources.GetResource<TileData>("lantern");
			TileData tdSign			= Resources.GetResource<TileData>("sign");
			TileData tdChest		= Resources.GetResource<TileData>("chest");
			TileData tdReward		= Resources.GetResource<TileData>("reward");
			EventTileData etdWarp	= Resources.GetResource<EventTileData>("warp");

			Level level;
			Room r;
			TileDataInstance t;
			EventTileDataInstance e;

			// Setup the overworld rooms.
			level = world.Levels[0];
			r = level.GetRoom(2, 1);
			t = r.CreateTile(tdOwl, 8, 1, 1);
				t.Properties.Set("text", "Hello, World!");
			t = r.CreateTile(tdChest, 7, 1, 1);
				t.Properties.Set("reward", "heart_piece");
			t = r.CreateTile(tdReward, 6, 3, 1);
				t.Properties.Set("reward", "item_flippers_1");
			t = r.CreateTile(tdSign, 1, 1, 1);
				t.Properties.Set("text", "This will<n> prime your load catchers and boost your desktop wallpaper.");
			t = r.CreateTile(tdReward, 2, 6, 1);
				t.Properties.Set("reward", "heart_piece");
			r.CreateTile(tdBlock, 2, 5, 1);
			r.CreateTile(tdGrass, 2, 2, 1);
			r.CreateTile(tdGrass, 2, 3, 1);
			r.CreateTile(tdGrass, 2, 4, 1);
			r.CreateTile(tdGrass, 3, 4, 1);
			r.CreateTile(tdGrass, 4, 5, 1);
			r.CreateTile(tdGrass, 3, 6, 1);
			r.CreateTile(tdGrass, 4, 6, 1);
			r.CreateTile(tdGrass, 5, 6, 1);
			r.CreateTile(tdGrass, 4, 7, 1);
			r.CreateTile(tdGrass, 5, 7, 1);
			r.CreateTile(tdGrass, 6, 7, 1);
			r.CreateTile(tdGrass, 7, 7, 1);
			r.CreateTile(tdGrass, 7, 2, 1);
			r.CreateTile(tdGrass, 8, 2, 1);
			r.CreateTile(tdGrass, 8, 3, 1);

			r = level.GetRoom(2, 2);
			e = r.CreateEventTile(etdWarp, 16, 64);
				e.Properties.Add(Property.CreateString("id", "warp_a"));
				e.Properties.Add(Property.CreateString("warp_type", "tunnel"));
				e.Properties.Add(Property.CreateString("destination_level", "overworld"));
				e.Properties.Add(Property.CreateString("destination_warp_point", "warp_b"));
			
			r = level.GetRoom(1, 1);
			e = r.CreateEventTile(etdWarp, 64, 96);
				e.Properties.Add(Property.CreateString("id", "warp_b"));
				e.Properties.Add(Property.CreateString("warp_type", "stairs"));
				e.Properties.Add(Property.CreateString("destination_level", "overworld"));
				e.Properties.Add(Property.CreateString("destination_warp_point", "warp_a"));

			r = level.GetRoom(new Point2I(1, 1));
			r.CreateTile(tdDiamond, 1, 1, 1);
			r.CreateTile(tdDiamond, 2, 2, 1);
			r.CreateTile(tdDiamond, 2, 4, 1);
			r.CreateTile(tdPot, 8, 2, 1);

			r = level.GetRoom(new Point2I(3, 0));
			r.CreateTile(tdLantern, 3, 2, 1);

			r = level.GetRoom(new Point2I(1, 0));
			r.CreateTile(tdRock, 8, 2, 1);

			r = level.GetRoom(new Point2I(2, 0));
			for (int x = 1; x < 8; x++) {
				for (int y = 2; y < 6; y++) {
					r.CreateTile(tdBush, x, y, 1);
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
			
			// Setup the interior rooms.
			level = world.Levels[1];
			r = level.GetRoom(2, 1);
			r.Zone = GameData.ZONE_INTERIOR;
			r.CreateTile(tdPot, 1, 2, 1);
			r.CreateTile(tdPot, 1, 3, 1);
			r.CreateTile(tdPot, 5, 1, 1);
			r = level.GetRoom(3, 1);
			r.Zone = GameData.ZONE_INTERIOR;
			r.CreateTile(tdChest, 8, 1, 1);
			r.CreateTile(tdPot, 8, 2, 1);
			r.CreateTile(tdPot, 4, 6, 1);
			r.CreateTile(tdPot, 5, 6, 1);
			r.CreateTile(tdPot, 6, 6, 1);
			r.CreateTile(tdPot, 7, 6, 1);
			r.CreateTile(tdPot, 8, 6, 1);

			// Save and load the world.
			{
				WorldFile worldFile = new WorldFile();
				worldFile.Save("Content/Worlds/custom_world.zwd", world);
			}
			{
				WorldFile worldFile = new WorldFile();
				world = worldFile.Load("Content/Worlds/custom_world.zwd");
			}
			
			return world;
		}

		
		// Load a level from an java level file.
		public static Level LoadJavaLevel(string filename) {
            BinaryReader bin = new BinaryReader(File.OpenRead(filename));
			int width	= bin.ReadByte();
			int height	= bin.ReadByte();
			Level level	= new Level(width, height, GameSettings.ROOM_SIZE_SMALL);

			// Load the rooms in the level.
			for (int y = 0; y < height; y++) {
				for (int x = 0; x < width; x++) {
					level.Rooms[x, y] = LoadJavaRoom(bin, level, x, y);
				}
			}

			level.RoomSize = level.Rooms[0, 0].Size;
			level.RoomLayerCount = level.Rooms[0, 0].LayerCount;

            bin.Close();
			return level;
		}
		
		// Load a single room from an java level file.
		public static Room LoadJavaRoom(BinaryReader bin, Level level, int locX, int locY) {
			byte width		= bin.ReadByte();
			byte height		= bin.ReadByte();
			level.RoomSize	= new Point2I(width, height);
			Room room		= new Room(level, locX, locY);
			room.Zone		= GameData.ZONE_SUMMER;

			// Read the tile data.
			for (int y = 0; y < height; y++) {
				for (int x = 0; x < width; x++) {
					byte tilesetIndex = bin.ReadByte();

					if (tilesetIndex > 0) {
						Tileset tileset = GameData.TILESET_OVERWORLD;
						if (tilesetIndex == 2)
							tileset = GameData.TILESET_INTERIOR;
						byte tilesetSourceX = bin.ReadByte();
						byte tilesetSourceY = bin.ReadByte();
						room.CreateTile(tileset.TileData[tilesetSourceX, tilesetSourceY], x, y, 0);
					}
					else {
						// Only use default tiles on bottom layer.
						Tileset tileset = GameData.TILESET_OVERWORLD;
						room.CreateTile(tileset.TileData[tileset.DefaultTile.X, tileset.DefaultTile.Y], x, y, 0);
					}
					
				}
			}

			return room;
		}

	}
}
