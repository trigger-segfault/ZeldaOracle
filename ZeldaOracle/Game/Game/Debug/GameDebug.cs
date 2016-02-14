using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Entities.Monsters;
using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.Entities.Units;
using ZeldaOracle.Game.GameStates;
using ZeldaOracle.Game.GameStates.Transitions;
using ZeldaOracle.Game.Main;
using ZeldaOracle.Game.Control;
using ZeldaOracle.Game.Control.Scripting;
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
using ZeldaOracle.Game.Entities.Collisions;

namespace ZeldaOracle.Game.Debug {
	public class GameDebug {

		public static GameControl GameControl {
			get { return gameControl; }
			set { gameControl = value; }
		}

		public static RoomControl RoomControl {
			get { return gameControl.RoomControl; }
		}

		public static GameManager GameManager {
			get { return gameControl.GameManager; }
		}

		private static GameControl gameControl;

		private static EntityDrawInfo	EntityDebugInfoMode	= EntityDrawInfo.None;
		private static TileDrawInfo		TileDebugInfoMode	= TileDrawInfo.None;

		private enum EntityDrawInfo {
			None = 0,
			CollisionBoxes,
			CollisionTests,
			Count,
		}

		private enum TileDrawInfo {
			None = 0,
			CollisionBoxes,
			GridArea,
			Count,
		}
		
		public static void UpdateRoomDebugKeys() {

			if (Keyboard.IsKeyPressed(Keys.P))
				RoomControl.IsSideScrolling = !RoomControl.IsSideScrolling;

			// C: Change color barrier color.
			if (Keyboard.IsKeyPressed(Keys.C)) {
				if (RoomControl.Dungeon != null) {
					PuzzleColor c = (RoomControl.Dungeon.ColorSwitchColor == PuzzleColor.Blue ? PuzzleColor.Red : PuzzleColor.Blue);
					RoomControl.Dungeon.ColorSwitchColor = c;
					if (RoomControl.GetTilesOfType<TileColorBarrier>().Any())
						gameControl.PushRoomState(new RoomStateColorBarrier(c));
				}
			}
			// CTRL+R: Restart the game.
			if (Keyboard.IsKeyPressed(Keys.R) && Keyboard.IsKeyDown(Keys.LControl))
				GameManager.Restart();
			// F5: Pause gameplay.
			if (Keyboard.IsKeyPressed(Keys.F5))
				GameManager.IsGamePaused = !GameManager.IsGamePaused;
			// F6: Step gameplay by one frame.
			if (Keyboard.IsKeyPressed(Keys.F6) && GameManager.IsGamePaused)
				GameManager.NextFrame();
			// OPEN BRACKET: open all open doors.
			if (Keyboard.IsKeyPressed(Keys.OpenBracket))
				RoomControl.OpenAllDoors();
			// CLOSE BRACKET: close all doors.
			else if (Keyboard.IsKeyPressed(Keys.CloseBracket))
				RoomControl.CloseAllDoors();
			// G: Display a test message.
			if (Keyboard.IsKeyPressed(Keys.G)) {
				gameControl.DisplayMessage("I was a <red>hero<red> to broken robots 'cause I was one of them, but how can I sing about being damaged if I'm not?<p> That's like <green>Christina Aguilera<green> singing Spanish. Ooh, wait! That's it! I'll fake it!");
			}
			// INSERT: Fill all ammo.
			if (Keyboard.IsKeyPressed(Keys.Insert)) {
				gameControl.Inventory.FillAllAmmo();
				Dungeon dungeon = gameControl.RoomControl.Dungeon;
				if (dungeon != null) {
					dungeon.NumSmallKeys = Math.Min(dungeon.NumSmallKeys + 3, 9);
					dungeon.HasMap		= true;
					dungeon.HasCompass	= true;
					dungeon.HasBossKey	= true;
				}
			}
			// DELETE: Empty all ammo.
			if (Keyboard.IsKeyPressed(Keys.Delete)) {
				gameControl.Inventory.EmptyAllAmmo();
			}
			// HOME: Set the player's health to max.
			if (Keyboard.IsKeyPressed(Keys.Home)) {
				gameControl.Player.MaxHealth = 4 * 14;
				gameControl.Player.Health = gameControl.Player.MaxHealth;
			}
			// END: Set the player's health to 3 hearts.
			if (Keyboard.IsKeyPressed(Keys.End)) {
				gameControl.Player.Health = 4 * 3;
			}
			
			// T: Cycle which tunic the player is wearing.
			if (Keyboard.IsKeyPressed(Keys.T)) {
				switch (gameControl.Player.Tunic) {
				case PlayerTunics.GreenTunic:	gameControl.Player.Tunic = PlayerTunics.RedTunic; break;
				case PlayerTunics.RedTunic:		gameControl.Player.Tunic = PlayerTunics.BlueTunic; break;
				case PlayerTunics.BlueTunic:	gameControl.Player.Tunic = PlayerTunics.GreenTunic; break;
				}
			}
			// H: Hurt the player in a random direction.
			if (Keyboard.IsKeyPressed(Keys.H)) {
				float angle = GRandom.NextFloat(GMath.FullAngle);
				Vector2F source = gameControl.Player.Center +  new Vector2F(5.0f, angle, true);
				gameControl.Player.Hurt(new DamageInfo(0, source));
			}
			// M: Play music.
			/*if (Keyboard.IsKeyPressed(Keys.M)) {
				AudioSystem.PlaySong("overworld");
			}
			// N: Set the volume to max.
			if (Keyboard.IsKeyPressed(Keys.N)) {
				AudioSystem.MasterVolume = 1.0f;
			}*/
			// N: Noclip mode.
			if (Keyboard.IsKeyPressed(Keys.N)) {
				RoomControl.Player.Physics.CollideWithEntities	= !RoomControl.Player.Physics.CollideWithEntities;
				RoomControl.Player.Physics.CollideWithWorld		= !RoomControl.Player.Physics.CollideWithWorld;
			}
			// Q: Spawn a random rupees collectible.
			if (Keyboard.IsKeyPressed(Keys.Q)) {
				int[] rupees = { 1, 5, 20, 100, 200 };//, 5, 20, 100, 200 };
				int rupee = GRandom.NextInt(rupees.Length);
				Collectible collectible = gameControl.RewardManager.SpawnCollectible("rupees_" + rupees[rupee].ToString());
				collectible.Position = gameControl.Player.Position;
				collectible.ZPosition = 100;
			}
			// Y: Cycle entity debug info.
			if (Keyboard.IsKeyPressed(Keys.Y)) {
				EntityDebugInfoMode = (EntityDrawInfo) (((int) EntityDebugInfoMode + 1) % (int) EntityDrawInfo.Count);
			}
			// U: Cycle tile debug info.
			if (Keyboard.IsKeyPressed(Keys.U)) {
				TileDebugInfoMode = (TileDrawInfo) (((int) TileDebugInfoMode + 1) % (int) TileDrawInfo.Count);
			}
			// J: Spawn a heart collectible.
			if (Keyboard.IsKeyPressed(Keys.K)) {
				Collectible collectible = gameControl.RewardManager.SpawnCollectible("hearts_1");
				collectible.Position = gameControl.Player.Position;
				collectible.ZPosition = 100;
			}
			// B: Spawn bomb collectibles.
			if (Keyboard.IsKeyPressed(Keys.B)) {
				Collectible collectible = gameControl.RewardManager.SpawnCollectible("ammo_bombs_5");
				collectible.Position = gameControl.Player.Position;
				collectible.ZPosition = 100;
			}
			// J: Spawn arrow collectibles.
			if (Keyboard.IsKeyPressed(Keys.J)) {
				Collectible collectible = gameControl.RewardManager.SpawnCollectible("ammo_arrows_5");
				collectible.Position = gameControl.Player.Position;
				collectible.ZPosition = 100;
			}
			// 0: Spawn a monster.
			if (Keyboard.IsKeyPressed(Keys.D0) || Keyboard.IsKeyPressed(Keys.Add)) {
				Monster monster		= new TestMonster();
				//Monster monster		= new MonsterOctorok();
				//Monster monster		= new MonsterMoblin();
				Vector2F position	= new Vector2F(32, 32) + new Vector2F(8, 14);
				RoomControl.SpawnEntity(monster, position);
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
			world.Levels[0].Properties.Set("id", "overworld");
			world.Levels[1].Properties.Set("id", "interiors");
			world.Levels[2].Properties.Set("id", "big_interiors");

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
			r = level.GetRoomAt(2, 1);
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

			r = level.GetRoomAt(2, 2);
			e = r.CreateEventTile(etdWarp, 16, 64);
				e.Properties.Set("id", "warp_a");
				e.Properties.Set("warp_type", "tunnel");
				e.Properties.Set("destination_level", "overworld");
				e.Properties.Set("destination_warp_point", "warp_b");
			
			r = level.GetRoomAt(1, 1);
			e = r.CreateEventTile(etdWarp, 64, 96);
				e.Properties.Set("id", "warp_b");
				e.Properties.Set("warp_type", "stairs");
				e.Properties.Set("destination_level", "overworld");
				e.Properties.Set("destination_warp_point", "warp_a");

			r = level.GetRoomAt(new Point2I(1, 1));
			r.CreateTile(tdDiamond, 1, 1, 1);
			r.CreateTile(tdDiamond, 2, 2, 1);
			r.CreateTile(tdDiamond, 2, 4, 1);
			r.CreateTile(tdPot, 8, 2, 1);

			r = level.GetRoomAt(new Point2I(3, 0));
			r.CreateTile(tdLantern, 3, 2, 1);

			r = level.GetRoomAt(new Point2I(1, 0));
			r.CreateTile(tdRock, 8, 2, 1);

			r = level.GetRoomAt(new Point2I(2, 0));
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
			r = level.GetRoomAt(2, 1);
			r.Zone = GameData.ZONE_INTERIOR;
			r.CreateTile(tdPot, 1, 2, 1);
			r.CreateTile(tdPot, 1, 3, 1);
			r.CreateTile(tdPot, 5, 1, 1);
			r = level.GetRoomAt(3, 1);
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

		private static void DrawTile(Graphics2D g, Tile tile) {
			
			if (TileDebugInfoMode == TileDrawInfo.CollisionBoxes) {
				if (tile.IsSolid && tile.CollisionModel != null) {
					foreach (Rectangle2F box in tile.CollisionModel.Boxes) {
						Rectangle2F r = Rectangle2F.Translate(box, tile.Position);
						g.FillRectangle(r, Color.Red);
						//g.DrawRectangle(r, 1, Color.Maroon);
					}
				}
			}
			else if (TileDebugInfoMode == TileDrawInfo.GridArea) {
				Rectangle2F tileBounds = (Rectangle2F) tile.TileGridArea;
				tileBounds.Point *= GameSettings.TILE_SIZE;
				tileBounds.Size *= GameSettings.TILE_SIZE;
				Color c = Color.Yellow;
				if (tile.Layer == 1)
					c = Color.Blue;
				else if (tile.Layer == 2)
					c = Color.Red;
				g.FillRectangle(tileBounds, c);

				tileBounds = new Rectangle2F(tile.Position, tile.Size * GameSettings.TILE_SIZE);
				c = Color.Olive;
				if (tile.Layer == 1)
					c = Color.Cyan;
				else if (tile.Layer == 2)
					c = Color.Maroon;

				g.DrawLine(new Line2F(tileBounds.TopLeft, tileBounds.BottomRight - new Point2I(1, 1)), 1, c);
				g.DrawLine(new Line2F(tileBounds.TopRight - new Point2I(1, 0), tileBounds.BottomLeft - new Point2I(0, 1)), 1, c);
				g.DrawRectangle(tileBounds, 1, Color.Black);
			}
		}

		private static void DrawEntity(Graphics2D g, Entity entity) {
			
			if (EntityDebugInfoMode == EntityDrawInfo.CollisionBoxes) {
				g.FillRectangle(entity.Physics.SoftCollisionBox + entity.Position, new Color(0, 0, 255, 150));
				g.FillRectangle(entity.Physics.CollisionBox + entity.Position, new Color(255, 0, 0, 150));
				g.FillRectangle(new Rectangle2F(entity.Position, Vector2F.One), new Color(255, 255, 0));

				if (entity is Unit) {
					Unit unit = (Unit) entity;
					foreach (UnitTool tool in unit.EquippedTools) {
						if (tool.IsPhysicsEnabled) {
							g.FillRectangle(tool.PositionedCollisionBox, new Color(255, 0, 255, 150));
						}
					}
				}
			}
			else if (EntityDebugInfoMode == EntityDrawInfo.CollisionTests) {
				if (entity.Physics.IsEnabled && entity.Physics.CollideWithWorld || entity is Player) {
					// Draw the hard collision box.
					Rectangle2F collisionBox = entity.Physics.PositionedCollisionBox;
					Color collisionBoxColor = Color.Yellow;
					if (entity is Player && ((Player) entity).Movement.IsOnSideScrollLadder)
						collisionBoxColor = new Color(255, 160, 0);
					collisionBox.X = GMath.Round(collisionBox.X + 0.001f);
					collisionBox.Y = GMath.Round(collisionBox.Y + 0.001f);
					//collisionBox.Point = GMath.Round(collisionBox.Point);
					g.FillRectangle(collisionBox, collisionBoxColor);

					for (int i = 0; i < 4; i++) {
						CollisionInfoNew collisionInfo = entity.Physics.ClipCollisionInfo[i];
						int axis = Directions.ToAxis(i);

						if (entity.Physics.CollisionInfo[i].IsColliding) {
							Rectangle2F drawBox = collisionBox;
							drawBox.ExtendEdge(i, 1);
							drawBox.ExtendEdge(Directions.Reverse(i), -collisionBox.Size[axis]);
							g.FillRectangle(drawBox, Color.Magenta);
						}

						if (collisionInfo.IsColliding && !collisionInfo.IsResolved) {
							Rectangle2F drawBox = collisionBox;
							float penetration = Math.Max(1.0f, GMath.Round(collisionInfo.PenetrationDistance));
							if (i == Directions.Down || i == Directions.Right)
								drawBox.Point[axis] += drawBox.Size[axis] - penetration;
							drawBox.Size[axis] = penetration;
							
							// Draw the strip of penetration.
							Color penetrationColor = Color.Red;
							if (entity.Physics.AllowEdgeClipping && collisionInfo.IsAllowedClipping)
								penetrationColor = Color.Blue;
							g.FillRectangle(drawBox, penetrationColor);

						}
						if (collisionInfo.IsColliding && collisionInfo.IsResolved) {
							Rectangle2F drawBox2 = collisionBox;
							drawBox2.ExtendEdge(i, 2);
							drawBox2.ExtendEdge(Directions.Reverse(i), -collisionBox.Size[axis] - 1);
							g.FillRectangle(drawBox2, Color.Maroon);
						}
					}
				}
				else if (entity.Physics.IsEnabled && entity.Physics.IsSolid) {
					// Draw the hard collision box.
					Rectangle2F collisionBox = entity.Physics.PositionedCollisionBox;
					g.FillRectangle(collisionBox, Color.Olive);
				}
			}
		}

		public static void DrawRoomTiles(Graphics2D g, RoomControl roomControl) {
			// Draw debug info for tiles.
			foreach (Tile tile in roomControl.GetTiles())
				DrawTile(g, tile);
		}

		public static void DrawRoom(Graphics2D g, RoomControl roomControl) {
			// Draw debug info for entities.
			for (int i = roomControl.Entities.Count - 1; i >= 0; i--)
				DrawEntity(g, roomControl.Entities[i]);
		}
	}
}
