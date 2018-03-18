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
using ZeldaOracle.Game.API;
using ZeldaOracle.Game.Items;
using ZeldaOracle.Game.Items.Rewards;
using ZeldaOracle.Game.GameStates.RoomStates;
using ZeldaOracle.Game.Entities.Collisions;
using ZeldaOracle.Game.Entities.Monsters.JumpMonsters;
using ZeldaOracle.Game.Entities.Projectiles.PlayerProjectiles;
using ZeldaOracle.Game.Control.RoomManagers;
using ZeldaOracle.Common.Util;

namespace ZeldaOracle.Game.Debugging {
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

		private static DebugScripts debugScripts = new DebugScripts();
		private static GameControl gameControl;
		private static bool showTileCursor = false;
		private static Vector2F mousePosition;
		private static Point2I mouseTileLocation;
		private static string sampledTileName = "";
		private static Action[] printDebugInfoFunctions = new Action[] {
			null,
			PrintDebugInfoEntities,
			PrintDebugInfoInteractions,
			PrintDebugInfoPlayer,
		};
		private static int printDebugInfoFunctionIndex = 0;
		private static PlayerDebugNoClipState playerNoClipState = new PlayerDebugNoClipState();
		public static DevSettings DevSettings { get; set; } = new DevSettings();
		private static EntityDrawInfo	EntityDebugInfoMode	= EntityDrawInfo.None;
		private static TileDrawInfo		TileDebugInfoMode	= TileDrawInfo.None;

		private enum EntityDrawInfo {
			None = 0,
			CollisionTests,
			CollisionBoxes,
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

		private static void EquipStartWeapon(string id, int slot) {
			Inventory inventory = GameControl.RoomControl.Player.Inventory;
			ItemWeapon weapon = inventory.GetItem(id) as ItemWeapon;
			if (weapon != null)
				gameControl.MenuWeapons.EquipWeapon(weapon, slot);
		}

		public static void OnGameStart() {
			Player player = GameControl.RoomControl.Player;

			// Allow the player to swim in water and ocean
			player.SwimmingSkills =
				PlayerSwimmingSkills.CanSwimInWater;
				//PlayerSwimmingSkills.CanSwimInWater |
				//PlayerSwimmingSkills.CanSwimInOcean;

			// Obtain all items and ammo
			foreach (Item item in GameControl.Inventory.GetItems()) {
				GameControl.Inventory.ObtainItem(item);
			}

			foreach (Ammo ammo in GameControl.Inventory.GetAmmos()) {
				GameControl.Inventory.ObtainAmmo(ammo);
			}

			// Equip starting weapons
			EquipStartWeapon(DevSettings.Inventory.A, Inventory.SLOT_A);
			EquipStartWeapon(DevSettings.Inventory.B, Inventory.SLOT_B);
		}

		public static void PrintDebugInfoEntities() {
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine("Entity Heirarchy:");
			Console.ForegroundColor = ConsoleColor.White;

			List<Entity> entities = RoomControl.EntityManager.EntityUpdateOrder;
			for (int i = 0; i < entities.Count; i++) {
				Entity entity = entities[i];
				if (entity.Parent == null)
					PrintEntityDebugInfo(entity, " ");
			}
		}

		public static void PrintEntityDebugInfo(Entity entity, string tabs) {
			Console.WriteLine("{0, 4}.{1}{2}",
				entity.EntityIndex, tabs, entity.GetType().Name);
			foreach (Entity child in entity.Children) {
				PrintEntityDebugInfo(child, tabs + "  ");
			}
		}

		public static void PrintDebugInfoInteractions() {
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine("Entity Interactions:");
			Console.ForegroundColor = ConsoleColor.White;
			foreach (InteractionInstance interaction in
				RoomControl.InteractionManager.Interactions)
			{
				Console.Write("{0, 5}: ", interaction.Duration);
				Console.ForegroundColor = ConsoleColor.Green;
				Console.Write(interaction.ActionEntity.GetType().Name);
				Console.ForegroundColor = ConsoleColor.White;
				Console.Write(" --");
				Console.ForegroundColor = ConsoleColor.Cyan;
				Console.Write(interaction.Type.ToString());
				Console.ForegroundColor = ConsoleColor.White;
				Console.Write("--> ");
				Console.ForegroundColor = ConsoleColor.Green;
				Console.Write(interaction.ReactionEntity.GetType().Name);
				Console.ForegroundColor = ConsoleColor.White;
				Console.WriteLine("");
			}
			
			Console.WriteLine("");
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine("Protected Interactions:");
			Console.ForegroundColor = ConsoleColor.White;
			foreach (ProtectedInteraction protection in
				RoomControl.InteractionManager.ProtectedInteractions)
			{
				Console.ForegroundColor = ConsoleColor.White;
				Console.Write(" ");
				Console.ForegroundColor = ConsoleColor.Green;
				Console.Write(protection.ProtectingEntity.GetType().Name);
				Console.ForegroundColor = ConsoleColor.White;
				Console.Write(" protects ");
				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.Write(protection.ProtectedEntity.GetType().Name);
				Console.ForegroundColor = ConsoleColor.White;
				Console.Write(" <--");
				Console.ForegroundColor = ConsoleColor.Cyan;
				Console.Write(protection.ProtectedInteractionType.ToString());
				Console.ForegroundColor = ConsoleColor.White;
				Console.Write("-- ");
				Console.ForegroundColor = ConsoleColor.Red;
				Console.Write(protection.InteractionEntity.GetType().Name);
				Console.ForegroundColor = ConsoleColor.White;
				Console.WriteLine("");
			}
		}

		public static void PrintDebugInfoPlayer() {
			Player player = RoomControl.Player;
			
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine("Player Information:");
			Console.ForegroundColor = ConsoleColor.White;
			
			string weaponStateName = "";
			if (player.WeaponState != null)
				weaponStateName = player.WeaponState.GetType().Name;
			string controlStateName = "";
			if (player.ControlState != null)
				controlStateName = player.ControlState.GetType().Name;
			string environmentStateName = "";
			if (player.EnvironmentState != null)
				environmentStateName = player.EnvironmentState.GetType().Name;

			Console.WriteLine("FPS: " + GameControl.GameManager.FPS.ToString("00.#"));

			Console.WriteLine("Player.Velocity:      {0}", player.Physics.Velocity);
			if (player.Physics.OnGroundOverride)
				Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("Player.IsOnGround:    {0}", player.Physics.IsOnGround);
			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine("Player.Motion:        {0}", player.Movement.Motion);
			Console.WriteLine("Player.Direction:     {0}", Directions.ToString(player.Direction));
			Console.WriteLine("Player.UseDirection:  {0}", player.UseDirection.ToString());
			Console.WriteLine("Player.UseAngle:      {0}", player.UseAngle.ToString());
			Console.WriteLine("Player.MoveDirection: {0}", player.MoveDirection.ToString());
			Console.WriteLine("Player.MoveAngle:     {0}", player.MoveAngle.ToPoint());
			Console.WriteLine("Player.IsMoving:      {0}", player.Movement.IsMoving);
			Console.ForegroundColor = ConsoleColor.Cyan;
			Console.WriteLine("Ctl: {0}", controlStateName);
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("Wpn: {0}", weaponStateName);
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine("Env: {0}", environmentStateName);
			Console.ForegroundColor = ConsoleColor.Magenta;
			Console.Write("Cnd: ");
			int conditionStateCount = player.ConditionStates.Count();
			for (int i = 0; i < 4; i++) {
				if (i > 0)
					Console.Write("     ");
				if (i < conditionStateCount) {
					PlayerState state = player.ConditionStates.ElementAt(i);
					string stateName = state.GetType().Name;
					Console.WriteLine("{0}", stateName);
				}
				else
					Console.WriteLine("{0}", "");
			}
			Console.WriteLine("");
			Console.ForegroundColor = ConsoleColor.White;
			Console.BackgroundColor = ConsoleColor.Black;
		}
		
		public static void UpdateRoomDebugKeys() {
			bool ctrl = (Keyboard.IsKeyDown(Keys.LControl) ||
				Keyboard.IsKeyDown(Keys.RControl));
			bool shift = (Keyboard.IsKeyDown(Keys.LShift) ||
				Keyboard.IsKeyDown(Keys.RShift));
			
			if (ctrl && Keyboard.IsKeyPressed(Keys.S)) {
				GameControl.ScriptRunner.RunScript(
					debugScripts, debugScripts.DebugScript1);
			}

			// Print debug info to the console window
			if (GameManager.IsConsoleOpen &&
				printDebugInfoFunctions[printDebugInfoFunctionIndex] != null)
			{
				Console.SetCursorPosition(0, 0);
				Console.ForegroundColor = ConsoleColor.White;
				Console.BackgroundColor = ConsoleColor.Black;
				Console.Clear();
				printDebugInfoFunctions[printDebugInfoFunctionIndex]();
				Console.WriteLine("");
				Console.ForegroundColor = ConsoleColor.White;
				Console.BackgroundColor = ConsoleColor.Black;
			}

			// CTRL+Q: Quit the game
			if (ctrl && Keyboard.IsKeyPressed(Keys.Q))
				GameManager.Exit();
			// CTRL+R: Restart the game.
			if (ctrl && Keyboard.IsKeyPressed(Keys.R))
				GameManager.Restart();
			// CTRL+T: Toggle console window.
			if (ctrl && Keyboard.IsKeyPressed(Keys.T))
				GameManager.IsConsoleOpen = !GameManager.IsConsoleOpen;
			// F1: Cycle debug console printouts
			if (!ctrl && Keyboard.IsKeyPressed(Keys.F1)) {
				if (!GameManager.IsConsoleOpen) {
					GameManager.IsConsoleOpen = true;
				}
				else {
					printDebugInfoFunctionIndex = (printDebugInfoFunctionIndex + 1) %
						printDebugInfoFunctions.Length;
				}

				Console.Clear();
				if (printDebugInfoFunctions[printDebugInfoFunctionIndex] == null) {
					Console.ForegroundColor = ConsoleColor.Yellow;
					Console.WriteLine("Log Messages:");
					Console.ForegroundColor = ConsoleColor.Gray;
					Logs.LoggingSystem.PrintAllLogMessages();
				}
			}
			// F5: Pause gameplay.
			if (!ctrl && Keyboard.IsKeyPressed(Keys.F5))
				GameManager.IsGamePaused = !GameManager.IsGamePaused;
			// F6: Step gameplay by one frame.
			if (!ctrl && Keyboard.IsKeyPressed(Keys.F6) && GameManager.IsGamePaused)
				GameManager.NextFrame();
			// Ctrl+M: Mute/unmute
			if (ctrl && Keyboard.IsKeyPressed(Keys.M))
				AudioSystem.IsMasterMuted = !AudioSystem.IsMasterMuted;
			// Ctrl+U: Toggle side scrolling
			if (ctrl && Keyboard.IsKeyPressed(Keys.I))
				RoomControl.IsSideScrolling = !RoomControl.IsSideScrolling;
			// Ctrl+U: Toggle underwater
			if (ctrl && Keyboard.IsKeyPressed(Keys.U))
				RoomControl.IsUnderwater = !RoomControl.IsUnderwater;

			// Ctrl+Shift+Arrows: Instantly change rooms.
			if (ctrl && shift && Keyboard.IsKeyPressed(Keys.Up))
				ChangeRooms(Direction.Up);
			if (ctrl && shift && Keyboard.IsKeyPressed(Keys.Down))
				ChangeRooms(Direction.Down);
			if (ctrl && shift && Keyboard.IsKeyPressed(Keys.Right))
				ChangeRooms(Direction.Right);
			if (ctrl && shift && Keyboard.IsKeyPressed(Keys.Left))
				ChangeRooms(Direction.Left);
			// Ctrl+Y: Cycle current room's zone
			if (ctrl && Keyboard.IsKeyPressed(Keys.Y)) {
				List<string> zoneNames = new List<string>();
				zoneNames.AddRange(Resources.GetDictionaryKeys<Zone>());
				int index = zoneNames.IndexOf(RoomControl.Room.Zone.ID);
				if (shift)
					index = (index + zoneNames.Count - 1) % zoneNames.Count;
				else
					index = (index + 1) % zoneNames.Count;
				RoomControl.Room.Zone = Resources.Get<Zone>(zoneNames[index]);
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
				if (RoomControl.Area != null) {
					PuzzleColor c = (RoomControl.Area.ColorSwitchColor == PuzzleColor.Blue ? PuzzleColor.Red : PuzzleColor.Blue);
					RoomControl.Area.ColorSwitchColor = c;
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
				Area dungeon = gameControl.RoomControl.Area;
				if (dungeon != null) {
					dungeon.SmallKeyCount = GMath.Min(dungeon.SmallKeyCount + 3, 9);
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
				Vector2F source = gameControl.Player.Center + Vector2F.FromPolar(5.0f, angle);
				gameControl.Player.Hurt(new DamageInfo(0, source));
			}
			// N: Noclip mode.
			if (!ctrl && Keyboard.IsKeyPressed(Keys.N)) {
				if (playerNoClipState.IsActive)
					playerNoClipState.End();
				else
					RoomControl.Player.BeginConditionState(playerNoClipState);
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
				Collectible collectible = gameControl.RewardManager.SpawnCollectible("bombs_5");
				collectible.Position = gameControl.Player.Position;
				collectible.ZPosition = 100;
			}
			// J: Spawn arrow collectibles.
			if (!ctrl && Keyboard.IsKeyPressed(Keys.J)) {
				Collectible collectible = gameControl.RewardManager.SpawnCollectible("arrows_5");
				collectible.Position = gameControl.Player.Position;
				collectible.ZPosition = 100;
			}
			// 0: Spawn a test monster
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
			mousePosition += RoomControl.ViewControl.Camera.TopLeft;
			Point2I mouseTileLocationPrev = mouseTileLocation;
			mouseTileLocation = RoomControl.GetTileLocation(mousePosition);

			if (ctrl)
				showTileCursor = true;
			else
				showTileCursor = false;
			
			// Middle mouse: sample the tile under the mouse
			if (Mouse.IsButtonPressed(MouseButtons.Middle) ||
				(Mouse.IsButtonDown(MouseButtons.Middle) &&
					mouseTileLocation != mouseTileLocationPrev))
			{
				Tile newSampledTile = RoomControl.GetTopTile(mouseTileLocation);
				if (newSampledTile != null && newSampledTile.TileData.BaseData != null) {
					sampledTileName = newSampledTile.TileData.BaseData.ResourceName;
				}
			}

			// Ctrl + Shift + LeftMouse: place the sampled tile at the mouse location
			if (ctrl && shift &&
				(Mouse.IsButtonPressed(MouseButtons.Left) ||
					(Mouse.IsButtonDown(MouseButtons.Left) &&
					mouseTileLocation != mouseTileLocationPrev)))
			{
				// Replace the top-most tile with the sampled tile
				int layer = 0;
				Tile topTile = RoomControl.GetTopTile(mouseTileLocation);
				if (topTile != null)
					layer = topTile.Layer;
				TileData tileData = Resources.Get<TileData>(sampledTileName);
				if (tileData != null) {
					Tile tile = Tile.CreateTile(tileData);
					if (tile != null)
						RoomControl.PlaceTile(tile, mouseTileLocation, layer);
				}
			}

			// Ctrl + LeftMouse: place an entity at the mouse position
			else if (ctrl && Mouse.IsButtonPressed(MouseButtons.Left)) {
				Vector2F spawnPosition = mouseTileLocation * GameSettings.TILE_SIZE;
				spawnPosition += new Vector2F(8, 8);
				Monster monster = new MonsterIronMask();
				//monster.Color = MonsterColor.Red;

				//monster.Properties = new Properties();
				//monster.Properties.Set("cover_tile", "bush");

				//monster.Color = MonsterColor.Red;
				monster.SetPositionByCenter(spawnPosition);
				RoomControl.SpawnEntity(monster);
				//RoomControl.SpawnEntity(new MagnetBall(), spawnPosition);

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
			if (RoomControl.Level.ContainsRoom(adjacentRoomLocation)) {
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
						g.DrawRectangle(r, 1, Color.Maroon);
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

		private static void FillCollisionModel(Graphics2D g, CollisionModel model,
			Vector2F position, Color color)
		{
			foreach (Rectangle2F box in model.Boxes) {
				Rectangle2F r = Rectangle2F.Translate(box, position);
				r.Point = GameUtil.Bias(r.Point);
				g.FillRectangle(r, color);
			}
		}

		private static void DrawCollisionModel(Graphics2D g, CollisionModel model,
			Vector2F position, Color color)
		{
			foreach (Rectangle2F box in model.Boxes) {
				Rectangle2F r = Rectangle2F.Translate(box, position);
				r.Point = GameUtil.Bias(r.Point);
				g.DrawRectangle(r, 1, color);
			}
		}

		private static void DrawCollisionModelEdge(Graphics2D g, CollisionModel model,
			Vector2F position, int edgeDirection, float edgeWidth, Color color)
		{
			int axis = Directions.ToAxis(edgeDirection);
			int lateralAxis = Axes.GetOpposite(axis);
			foreach (Rectangle2F box in model.Boxes) {
				Rectangle2F r = Rectangle2F.Translate(box, position);
				r.Point = GameUtil.Bias(r.Point);
				r.ExtendEdge(Directions.Reverse(edgeDirection),
					-(r.Size[axis] - edgeWidth));
				g.FillRectangle(r, color);
			}
		}

		private static void DrawEntity(Graphics2D g, Entity entity) {
			
			if (EntityDebugInfoMode != EntityDrawInfo.None) {
			}

			if (EntityDebugInfoMode == EntityDrawInfo.CollisionBoxes) {
				// Blue interaction box
				if (entity.Interactions.IsEnabled) {
					Color color = Color.Blue;
					if (entity.Interactions.CurrentActions.Count > 0)
						color = Color.Red;
					if (entity.Interactions.CurrentReactions.Count > 0) {
						if (entity.Interactions.CurrentActions.Count > 0)
							color = Color.Yellow;
						else
							color = Color.Green;
					}
					g.FillRectangle(
						entity.Interactions.PositionedInteractionBox, color * 0.5f);
					g.DrawRectangle(
						entity.Interactions.PositionedInteractionBox, 1, color);
				}
				// Yellow origin point
				g.FillRectangle(new Rectangle2F(entity.Position, Vector2F.One),
					Color.Yellow);
			}
			else if (EntityDebugInfoMode == EntityDrawInfo.CollisionTests) {
				Rectangle2F collisionBox = entity.Physics.PositionedCollisionBox;
				
				// Draw all collisions
				foreach (Collision collision in entity.Physics.PotentialCollisions) {
					Color color = Color.Red;
					float penetrationEdgeWidth = 1.0f;

					// Colorize collision
					if (collision.IsSafeColliding) {
						if (collision.IsColliding)
							color = Color.Red;
						else
							color = Color.Blue;
						penetrationEdgeWidth = GMath.Max(1.0f,
							GMath.Abs(collision.Penetration));
					}
					else if (collision.IsDodged)
						color = Color.Green;
					else if (collision.IsResolved)
						color = Color.Cyan;
					else
						color = Color.White;
					
					// Color standing and centered collisions
					if (entity.Physics.StandingCollision == collision)
					{
						if (collision.IsTile) {
							FillCollisionModel(g, collision.Tile.CollisionModel,
								collision.Tile.Position, Color.Magenta * 0.3f);
							DrawCollisionModel(g, collision.Tile.CollisionModel,
								collision.Tile.Position, Color.Magenta);
						}
					}
					else if (entity.Physics.GetCenteredCollisionInDirection(
							collision.Direction) == collision)
					{
						if (collision.IsTile) {
							FillCollisionModel(g, collision.Tile.CollisionModel,
								collision.Tile.Position, Color.Yellow * 0.3f);
							DrawCollisionModel(g, collision.Tile.CollisionModel,
								collision.Tile.Position, Color.Yellow);
						}
					}

					// Draw the collision edge
					int edgeDirection = Directions.Reverse(collision.Direction);
					int axis = Directions.ToAxis(edgeDirection);
					Rectangle2F r = collision.SolidBox;
					r.Point = GameUtil.Bias(r.Point);
					if (collision.Source.IsInsideCollision)
						r.ExtendEdge(edgeDirection, -r.Size[axis] + 1);
					else
						r.ExtendEdge(Directions.Reverse(edgeDirection), -r.Size[axis]);
					r.ExtendEdge(Directions.Reverse(edgeDirection), penetrationEdgeWidth);
					g.FillRectangle(r, color);
				}

				if (entity.Physics.IsEnabled &&
					entity.Physics.CollideWithWorld || entity is Player)
				{
					collisionBox.X = GMath.Round(collisionBox.X + 0.001f);
					collisionBox.Y = GMath.Round(collisionBox.Y + 0.001f);
					
					// Draw the hard collision box
					Color collisionBoxColor = Color.Yellow;
					if (entity is Player && ((Player) entity).IsOnSideScrollLadder)
						collisionBoxColor = new Color(255, 160, 0);
					collisionBox.Point = GameUtil.Bias(collisionBox.Point);
					g.FillRectangle(collisionBox, collisionBoxColor);
				}
				else if (entity.Physics.IsEnabled && entity.Physics.IsSolid) {
					// Draw the hard collision box.
					g.FillRectangle(collisionBox, Color.Olive);
				}

				// Draw ledge altitude number
				if (entity.Physics.PassOverLedges) {
					string altitudeString = String.Format("{0}",
						entity.Physics.LedgeAltitude);
					g.DrawString(GameData.FONT_SMALL, altitudeString, 
						collisionBox.TopRight + new Vector2F(1, (collisionBox.Height * 0.5f) - 4),
						Color.White);
				}
			}
		}

		public static void DrawRoomTiles(Graphics2D g, RoomControl roomControl) {
			// Draw debug info for tiles
			foreach (Tile tile in roomControl.GetTiles())
				DrawTile(g, tile);
		}

		public static void DrawRoom(Graphics2D g, RoomControl roomControl) {
			// Draw debug info for entities
			for (int i = roomControl.Entities.Count - 1; i >= 0; i--) {
				if (roomControl.Entities[i] != roomControl.Player)
					DrawEntity(g, roomControl.Entities[i]);
			}
			DrawEntity(g, roomControl.Player);

			if (showTileCursor)
			{
				Rectangle2I mouseTileRec = new Rectangle2I(
					mouseTileLocation * GameSettings.TILE_SIZE,
					new Point2I(GameSettings.TILE_SIZE, GameSettings.TILE_SIZE));
				g.DrawRectangle(mouseTileRec, 1, Color.Red);
			}
		}

		public static void DrawDebugUI(Graphics2D g) {
			Rectangle2F bounds = new Rectangle2F(0, 0,
				GameSettings.VIEW_WIDTH * GameManager.GameScale,
				GameSettings.VIEW_HEIGHT * GameManager.GameScale);
		}
	}
}
