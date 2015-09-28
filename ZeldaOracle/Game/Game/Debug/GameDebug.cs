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
			
			//world.StartLevelIndex	= 2;
			//world.StartRoomLocation	= new Point2I(0, 1);

			// Load the levels from java level files.
			world.Levels.Add(LoadJavaLevel("Content/Worlds/test_level.zwd"));
			world.Levels.Add(LoadJavaLevel("Content/Worlds/interiors.zwd"));
			world.Levels.Add(LoadJavaLevel("Content/Worlds/big_interiors.zwd"));
			world.Levels[0].Name = "overworld";
			world.Levels[1].Name = "interiors";
			world.Levels[2].Name = "big_interiors";

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
			TileData tdBush			= new TileData(TileFlags.Solid | TileFlags.Pickupable | TileFlags.Bombable |
										TileFlags.Burnable | TileFlags.Switchable | TileFlags.Cuttable);
			tdBush.Sprite			= GameData.SPR_TILE_BUSH;
			tdBush.SpriteAsObject	= GameData.SPR_TILE_BUSH_ASOBJECT;
			tdBush.BreakAnimation	= GameData.ANIM_EFFECT_LEAVES;
			tdBush.CollisionModel	= GameData.MODEL_BLOCK;
			
			// Create a pot tile.
			TileData tdPot			= new TileData(TileFlags.Solid | TileFlags.Pickupable |
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

			Level level;
			Room r;

			// Setup the overworld rooms.
			level = world.Levels[0];
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

			r = level.GetRoom(new Point2I(2, 2));
			EventTileData ed = new EventTileData();
			ed.Type = typeof(WarpEvent);
			ed.Properties.Add(Property.CreateString("id", "warp_a"));
			ed.Properties.Add(Property.CreateString("warp_type", "tunnel"));
			ed.Properties.Add(Property.CreateString("destination_level", "overworld"));
			ed.Properties.Add(Property.CreateString("destination_warp_point", "warp_b"));
			ed.Position = new Point2I(16, 64);
			r.EventData.Add(ed);

			r = level.GetRoom(new Point2I(1, 1));
			ed = new EventTileData();
			ed.Type = typeof(WarpEvent);
			ed.Properties.Add(Property.CreateString("id", "warp_b"));
			ed.Properties.Add(Property.CreateString("warp_type", "stairs"));
			ed.Properties.Add(Property.CreateString("destination_level", "overworld"));
			ed.Properties.Add(Property.CreateString("destination_warp_point", "warp_a"));
			ed.Position = new Point2I(64, 96);
			r.EventData.Add(ed);

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
			
			// Setup the interior rooms.
			level = world.Levels[1];
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
				worldFile.Save("Content/Worlds/custom_world.zwd", world);
			}
			{
				//WorldFile worldFile = new WorldFile();
				//World world = worldFile.Load("Content/Worlds/temp_world.zwd");
				//level = world.Levels[0];
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
						Tileset tileset = GameData.TILESETS[tilesetIndex - 1];
						byte tilesetSourceX = bin.ReadByte();
						byte tilesetSourceY = bin.ReadByte();
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

	}
}
