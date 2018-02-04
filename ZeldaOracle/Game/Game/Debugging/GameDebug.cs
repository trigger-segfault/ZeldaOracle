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
using ZeldaOracle.Game.Tiles.ActionTiles;
using ZeldaOracle.Game.Tiles.Custom;
using ZeldaOracle.Game.Worlds;
using ZeldaOracle.Game.Control.Menus;
using ZeldaOracle.Common.Input;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Game.Items;
using ZeldaOracle.Game.Items.Rewards;
using ZeldaOracle.Game.GameStates.RoomStates;
using ZeldaOracle.Game.Entities.Collisions;
using ZeldaOracle.Game.Entities.Monsters.JumpMonsters;

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
		private static bool showTileCursor = false;
		private static Vector2F mousePosition;
		private static Point2I mouseLocation;

		public static DevSettings DevSettings { get; set; } = new DevSettings();
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

		public static void LoadDevSettings() {
			// Load the development settings.
			DevSettings.Load();
		}

		public static void OnGameStart() {
			Player player = GameControl.RoomControl.Player;

			// Allow the player to swim in water and ocean.
			player.SwimmingSkills =
				PlayerSwimmingSkills.CanSwimInWater |
				PlayerSwimmingSkills.CanSwimInOcean;

			// Equip the sword and feather.
			// TODO
		}
		
		public static void UpdateRoomDebugKeys() {
			bool ctrl = (Keyboard.IsKeyDown(Keys.LControl) ||
				Keyboard.IsKeyDown(Keys.RControl));
			bool shift = (Keyboard.IsKeyDown(Keys.LShift) ||
				Keyboard.IsKeyDown(Keys.RShift));

			// CTRL+Q: Quit the game
			if (ctrl && Keyboard.IsKeyPressed(Keys.Q))
				GameManager.Exit();
			// CTRL+R: Restart the game.
			if (ctrl && Keyboard.IsKeyPressed(Keys.R))
				GameManager.Restart();
			// CTRL+R: Toggle console window.
			if (ctrl && Keyboard.IsKeyPressed(Keys.T))
				GameManager.IsConsoleOpen = !GameManager.IsConsoleOpen;
			// F5: Pause gameplay.
			if (!ctrl && Keyboard.IsKeyPressed(Keys.F5))
				GameManager.IsGamePaused = !GameManager.IsGamePaused;
			// F6: Step gameplay by one frame.
			if (!ctrl && Keyboard.IsKeyPressed(Keys.F6) && GameManager.IsGamePaused)
				GameManager.NextFrame();

			// Ctrl+U: Toggle side scrolling
			if (ctrl && Keyboard.IsKeyPressed(Keys.I))
				RoomControl.IsSideScrolling = !RoomControl.IsSideScrolling;
			// Ctrl+U: Toggle underwater
			if (ctrl && Keyboard.IsKeyPressed(Keys.U))
				RoomControl.IsUnderwater = !RoomControl.IsUnderwater;

			// Ctrl+Shift+Arrows: Instantly change rooms.
			if (ctrl && shift && Keyboard.IsKeyPressed(Keys.Up))
				ChangeRooms(Directions.Up);
			if (ctrl && shift && Keyboard.IsKeyPressed(Keys.Down))
				ChangeRooms(Directions.Down);
			if (ctrl && shift && Keyboard.IsKeyPressed(Keys.Right))
				ChangeRooms(Directions.Right);
			if (ctrl && shift && Keyboard.IsKeyPressed(Keys.Left))
				ChangeRooms(Directions.Left);
			// Ctrl+Y: Cycle current room's zone
			if (ctrl && Keyboard.IsKeyPressed(Keys.Y)) {
				List<string> zoneNames = Resources.GetResourceKeyList<Zone>();
				int index = zoneNames.IndexOf(RoomControl.Room.Zone.ID);
				if (shift)
					index = (index + zoneNames.Count - 1) % zoneNames.Count;
				else
					index = (index + 1) % zoneNames.Count;
				RoomControl.Room.Zone = Resources.GetResource<Zone>(zoneNames[index]);
				//GameData.PaletteShader.TilePalette = RoomControl.Zone.Palette;
				Console.WriteLine("Changed to zone '" + RoomControl.Zone.ID + "'");
			}

			// L: Level-up item in menu
			if (!ctrl && Keyboard.IsKeyPressed(Keys.L)) {
				if (GameManager.CurrentGameState is InventoryMenu) {
					InventoryMenu menu = ((InventoryMenu) GameManager.CurrentGameState);
					ISlotItem slotItem = menu.CurrentSlotGroup.CurrentSlot.SlotItem;
					if (slotItem is Item) {
						Item item = (Item) slotItem;
						int oldLevel = item.Level;
						item.Level = (item.Level + 1) % (item.MaxLevel + 1);
						if (item.Level != oldLevel)
							menu.ResetDescription();
					}
				}
			}
			// Ctrl+L: Level-up all obtained items to max level
			if (ctrl && Keyboard.IsKeyPressed(Keys.L)) {
				foreach (Item item in GameControl.Inventory.GetItems()) {
					if (item.IsObtained)
						item.Level = item.MaxLevel;
				}
				if (GameManager.CurrentGameState is InventoryMenu) {
					InventoryMenu menu = ((InventoryMenu) GameManager.CurrentGameState);
					menu.ResetDescription();
				}
			}

			// C: Change color barrier color.
			if (!ctrl && Keyboard.IsKeyPressed(Keys.C)) {
				if (RoomControl.Dungeon != null) {
					PuzzleColor c = (RoomControl.Dungeon.ColorSwitchColor == PuzzleColor.Blue ? PuzzleColor.Red : PuzzleColor.Blue);
					RoomControl.Dungeon.ColorSwitchColor = c;
					if (RoomControl.GetTilesOfType<TileColorBarrier>().Any())
						gameControl.PushRoomState(new RoomStateColorBarrier(c));
				}
			}
			// OPEN BRACKET: open all open doors.
			if (!ctrl && Keyboard.IsKeyPressed(Keys.OpenBracket))
				RoomControl.OpenAllDoors();
			// CLOSE BRACKET: close all doors.
			else if (!ctrl && Keyboard.IsKeyPressed(Keys.CloseBracket))
				RoomControl.CloseAllDoors();

			// G: Display a test message.
			if (!ctrl && Keyboard.IsKeyPressed(Keys.G)) {
				gameControl.DisplayMessage("I was a <red>hero<red> to broken robots 'cause I was one of them, but how can I sing about being damaged if I'm not?<p> That's like <green>Christina Aguilera<green> singing Spanish. Ooh, wait! That's it! I'll fake it!");
			}
			// INSERT: Fill all ammo.
			if (!ctrl && Keyboard.IsKeyPressed(Keys.Insert)) {
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
			if (!ctrl && Keyboard.IsKeyPressed(Keys.Delete)) {
				gameControl.Inventory.EmptyAllAmmo();
			}
			// HOME: Set the player's health to max.
			if (!ctrl && Keyboard.IsKeyPressed(Keys.Home)) {
				gameControl.Player.MaxHealth = 4 * 14;
				gameControl.Player.Health = gameControl.Player.MaxHealth;
			}
			// END: Set the player's health to 3 hearts.
			if (!ctrl && Keyboard.IsKeyPressed(Keys.End)) {
				gameControl.Player.Health = 4 * 3;
			}
			
			// T: Cycle which tunic the player is wearing.
			if (!ctrl && Keyboard.IsKeyPressed(Keys.T)) {
				switch (gameControl.Player.Tunic) {
				case PlayerTunics.GreenTunic:	gameControl.Player.Tunic = PlayerTunics.RedTunic; break;
				case PlayerTunics.RedTunic:		gameControl.Player.Tunic = PlayerTunics.BlueTunic; break;
				case PlayerTunics.BlueTunic:	gameControl.Player.Tunic = PlayerTunics.GreenTunic; break;
				}
			}
			// H: Hurt the player in a random direction.
			if (!ctrl && Keyboard.IsKeyPressed(Keys.H)) {
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
			if (!ctrl && Keyboard.IsKeyPressed(Keys.N)) {
				RoomControl.Player.Physics.CollideWithEntities	= !RoomControl.Player.Physics.CollideWithEntities;
				RoomControl.Player.Physics.CollideWithWorld		= !RoomControl.Player.Physics.CollideWithWorld;
			}
			// Q: Spawn a random rupees collectible.
			if (!ctrl && Keyboard.IsKeyPressed(Keys.Q)) {
				int[] rupees = { 1, 5, 20, 100, 200 };//, 5, 20, 100, 200 };
				int rupee = GRandom.NextInt(rupees.Length);
				Collectible collectible = gameControl.RewardManager.SpawnCollectible("rupees_" + rupees[rupee].ToString());
				collectible.Position = gameControl.Player.Position;
				collectible.ZPosition = 100;
			}
			// Y: Cycle entity debug info.
			if (!ctrl && Keyboard.IsKeyPressed(Keys.Y)) {
				EntityDebugInfoMode = (EntityDrawInfo) (((int) EntityDebugInfoMode + 1) % (int) EntityDrawInfo.Count);
			}
			// U: Cycle tile debug info.
			if (!ctrl && Keyboard.IsKeyPressed(Keys.U)) {
				TileDebugInfoMode = (TileDrawInfo) (((int) TileDebugInfoMode + 1) % (int) TileDrawInfo.Count);
			}
			// J: Spawn a heart collectible.
			if (!ctrl && Keyboard.IsKeyPressed(Keys.K)) {
				Collectible collectible = gameControl.RewardManager.SpawnCollectible("hearts_1");
				collectible.Position = gameControl.Player.Position;
				collectible.ZPosition = 100;
			}
			// B: Spawn bomb collectibles.
			if (!ctrl && Keyboard.IsKeyPressed(Keys.B)) {
				Collectible collectible = gameControl.RewardManager.SpawnCollectible("ammo_bombs_5");
				collectible.Position = gameControl.Player.Position;
				collectible.ZPosition = 100;
			}
			// J: Spawn arrow collectibles.
			if (!ctrl && Keyboard.IsKeyPressed(Keys.J)) {
				Collectible collectible = gameControl.RewardManager.SpawnCollectible("ammo_arrows_5");
				collectible.Position = gameControl.Player.Position;
				collectible.ZPosition = 100;
			}
			// 0: Spawn a monster.
			if (!ctrl && Keyboard.IsKeyPressed(Keys.D0) || Keyboard.IsKeyPressed(Keys.Add)) {
				Monster monster		= new TestMonster();
				//Monster monster		= new MonsterOctorok();
				//Monster monster		= new MonsterMoblin();
				Vector2F position	= new Vector2F(32, 32) + new Vector2F(8, 14);
				RoomControl.SpawnEntity(monster, position);
			}

			mousePosition = Mouse.GetPosition();
			mousePosition.X /= (float) GameControl.GameManager.GameBase.Window.ClientBounds.Width;
			mousePosition.X *= GameSettings.VIEW_WIDTH;
			mousePosition.Y /= (float) GameControl.GameManager.GameBase.Window.ClientBounds.Height;
			mousePosition.Y *= GameSettings.SCREEN_HEIGHT;
			mousePosition.Y -= 16.0f; // HUD
			mousePosition += RoomControl.ViewControl.Position;
			mouseLocation = RoomControl.GetTileLocation(mousePosition);

			if (ctrl)
				showTileCursor = true;
			else
				showTileCursor = false;
			
			if (ctrl && Mouse.IsButtonPressed(MouseButtons.Left)) {
				Vector2F spawnPosition = mouseLocation * GameSettings.TILE_SIZE;
				spawnPosition += new Vector2F(8, 8);
				Monster monster = new MonsterBari() {
					//Color = MonsterColor.Red
				};
				monster.SetPositionByCenter(spawnPosition);
				RoomControl.SpawnEntity(monster);

				//TileData tileData = new TileData(typeof(TileMonsterBeamos),
				//	TileFlags.None);
				//tileData.CollisionModel = GameData.MODEL_BLOCK;
				//tileData.SolidType = TileSolidType.Solid;
				//RoomControl.SpawnTile(new TileDataInstance(tileData, 
				//	mouseLocation.X, mouseLocation.Y, 2), false);
			}
		}
		
		private static void ChangeRooms(int direction) {

			Point2I roomLocation = RoomControl.RoomLocation;
			Point2I adjacentRoomLocation = roomLocation + Directions.ToPoint(direction);
			if (RoomControl.Level.ContainsRoom(adjacentRoomLocation))
			{
				Room adjacentRoom = RoomControl.Level.GetRoomAt(adjacentRoomLocation);
				RoomControl.TransitionToRoom(adjacentRoom, new RoomTransitionInstant());
			}
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

			if (showTileCursor)
			{
				Rectangle2I mouseTileRec = new Rectangle2I(
					mouseLocation * GameSettings.TILE_SIZE,
					new Point2I(GameSettings.TILE_SIZE, GameSettings.TILE_SIZE));
				g.DrawRectangle(mouseTileRec, 1, Color.Red);
			}
		}
	}
}
