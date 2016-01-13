using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.GameStates;
using ZeldaOracle.Game.Main;
using ZeldaOracle.Game.Worlds;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Tiles.Custom;

namespace ZeldaOracle.Game.Control.Maps {

	public class ScreenDungeonMap : GameState {

		private Image backgroundImage;
		private Dungeon dungeon;
		private int viewFloorIndex;		// Which floor wer are currently viewing.
		private int playerFloorIndex;	// Which floor the player is on.
		private int floorViewPosition;	// The current view position.
		private int floorViewSpeed;		// How fast to move the view when changing floors.
		private bool isChangingFloors;
		private List<DungeonMapFloor> floors;
		private AnimationPlayer cursorAnimationPlayer;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public ScreenDungeonMap(GameManager gameManager) {
			this.gameManager = gameManager;
			this.backgroundImage = Resources.GetImage("screen_dungeon_map");
			this.dungeon = null;
			this.floors = new List<DungeonMapFloor>();
			this.cursorAnimationPlayer = new AnimationPlayer();
		}


		//-----------------------------------------------------------------------------
		// Map Drawing
		//-----------------------------------------------------------------------------

		private void DrawRoom(Graphics2D g, DungeonMapRoom room, Point2I position) {
			if (room != null) {
				// Determine the sprite to draw for the room.
				Sprite sprite = null;
				if (room.IsDiscovered)
					sprite = room.Sprite;
				else if (dungeon.HasMap)
					sprite = GameData.SPR_UI_MAP_UNDISCOVERED_ROOM;
				if (dungeon.HasCompass) {
					if (room.IsBossRoom)
						sprite = GameData.SPR_UI_MAP_BOSS_ROOM;
					else if (room.HasTreasure)
						sprite = GameData.SPR_UI_MAP_TREASURE_ROOM;
				}

				// Draw the room sprite.
				if (sprite != null)
					g.DrawSprite(sprite, GameData.VARIANT_LIGHT, position);
			}
		}

		private void DrawFloor(Graphics2D g, DungeonMapFloor floor, Point2I position) {
			Point2I cursorPos = new Point2I(-1, -1);

			// Draw the level background rectangle.
			g.DrawSprite(GameData.SPR_UI_MAP_FLOOR_BACKGROUND,
				GameData.VARIANT_LIGHT, new Rectangle2I(position, new Point2I(64, 64)));
			
			// Draw the rooms.
			for (int x = 0; x < 8; x++) {
				for (int y = 0; y < 8; y++) {
					Point2I drawPos = position + (new Point2I(x, y) * 8);
					DungeonMapRoom room = floor.Rooms[x, y];
					DrawRoom(g, room, drawPos);
					if (room != null && room.Room == GameControl.RoomControl.Room)
						cursorPos = drawPos;
				}
			}

			// Draw room cursor.
			if (cursorPos >= Vector2F.Zero)
				g.DrawAnimation(cursorAnimationPlayer, GameData.VARIANT_LIGHT, cursorPos);
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public void OnOpen() {
			dungeon	= GameControl.RoomControl.Dungeon;

			//floorNumbers		= new int[] { -2, -1, 0, 1, 2 };
			playerFloorIndex	= 0;
			viewFloorIndex		= playerFloorIndex;
			floorViewPosition	= viewFloorIndex * 80;
			floorViewSpeed		= 8;
			isChangingFloors	= false;

			floors.Clear();
			floors.Add(new DungeonMapFloor(GameControl.RoomControl.Level, 0));

			cursorAnimationPlayer.Play(GameData.ANIM_UI_MAP_CURSOR);
		}

		public void OnClose() {

		}

		public override void OnBegin() {
		}
		
		public override void Update() {
			cursorAnimationPlayer.Update();

			// [SELECT] Close the map screen.
			if (Controls.Select.IsPressed()) {
				GameControl.CloseMapScreen();
				return;
			}

			// [UP] and [DOWN] to change floors.
			if (!isChangingFloors) {
				if (Controls.Up.IsPressed() && viewFloorIndex < floors.Count - 1) {
					viewFloorIndex++;
					isChangingFloors = true;
					AudioSystem.PlaySound("UI/menu_cursor_move");
				}
				else if (Controls.Down.IsPressed() && viewFloorIndex > 0) {
					viewFloorIndex--;
					isChangingFloors = true;
					AudioSystem.PlaySound("UI/menu_cursor_move");
				}
			}
			else {
				int nextFloorViewPosition = viewFloorIndex * 80;
				if (floorViewPosition < nextFloorViewPosition)
					floorViewPosition = Math.Min(floorViewPosition + floorViewSpeed, nextFloorViewPosition);
				else if (floorViewPosition > nextFloorViewPosition)
					floorViewPosition = Math.Max(floorViewPosition - floorViewSpeed, nextFloorViewPosition);
				if (floorViewPosition == nextFloorViewPosition)
					isChangingFloors = false;
			}
		}
		
		public override void Draw(Graphics2D g) {
			if (dungeon == null)
				return;
			
			Color black = Color.Black;//(lightDark == GameData.VARIANT_LIGHT ? new Color(16, 16, 16) : Color.Black);

			// Draw the background.
			g.DrawImage(backgroundImage, Point2I.Zero);

			// TODO: Draw the dungeon name panel.

			// Draw the floors.
			Point2I floorBasePos = new Point2I();
			if (floors.Count < 6)
				floorBasePos.Y = 72 + (8 * (floors.Count / 2));
			else
				floorBasePos.Y = 88 + (4 * (floors.Count - 6));

			for (int i = 0; i < floors.Count; i++) {
				DungeonMapFloor floor = floors[i];

				// Draw the floor's label box on the left side of the screen.
				Point2I floorPos = floorBasePos - new Point2I(0, i * 8);
				string floorName = floor.FloorNumberText;
				g.DrawString(GameData.FONT_SMALL, floorName, floorPos, new Color(248, 248, 216)); // drop shadow
				g.DrawString(GameData.FONT_SMALL, floorName, floorPos + new Point2I(0, -1), new Color(56, 32, 16));
				g.DrawSprite(GameData.SPR_UI_MAP_FLOOR_BOX_LEFT, GameData.VARIANT_LIGHT, floorPos + new Point2I(32, 0));
				g.DrawSprite(GameData.SPR_UI_MAP_FLOOR_BOX_RIGHT, GameData.VARIANT_LIGHT, floorPos + new Point2I(40, 0));
				if (viewFloorIndex == i)
					g.DrawSprite(GameData.SPR_UI_MAP_FLOOR_INDICATOR, GameData.VARIANT_LIGHT, floorPos + new Point2I(24, 0));
				if (playerFloorIndex == i)
					g.DrawSprite(GameData.SPR_UI_MAP_PLAYER, GameData.VARIANT_LIGHT, floorPos + new Point2I(36, 0));
				if (floor.IsBossFloor)
					g.DrawSprite(GameData.SPR_UI_MAP_BOSS_FLOOR, GameData.VARIANT_LIGHT, floorPos + new Point2I(48, 0));
							
				// Draw the floor's room display on the right side of the screen.
				Point2I floorRoomDisplayPos = new Point2I(80, 40 - (80 * i) + floorViewPosition);
				if (floorRoomDisplayPos.Y < GameSettings.SCREEN_HEIGHT && floorRoomDisplayPos.Y > -80)
					DrawFloor(g, floor, floorRoomDisplayPos);
			}
			
			// Draw floor view traversal arrows.
			if (!isChangingFloors) {
    			if (viewFloorIndex > 0)
    				g.DrawSprite(GameData.SPR_UI_MAP_ARROW_DOWN, GameData.VARIANT_LIGHT, 108, 108);
    			if (viewFloorIndex < floors.Count - 1)
    				g.DrawSprite(GameData.SPR_UI_MAP_ARROW_UP, GameData.VARIANT_LIGHT, 108, 28);
			}

			// Draw the items panel.
			if (dungeon.HasMap)
				g.DrawSprite(GameData.SPR_REWARD_MAP, GameData.VARIANT_LIGHT, 8, 110);
			if (dungeon.HasCompass)
				g.DrawSprite(GameData.SPR_REWARD_COMPASS, GameData.VARIANT_LIGHT, 32, 110);
			if (dungeon.HasBossKey)
				g.DrawSprite(GameData.SPR_REWARD_BOSS_KEY, GameData.VARIANT_LIGHT, 8, 128);
			if (dungeon.NumSmallKeys > 0) {
				g.DrawSprite(GameData.SPR_REWARD_SMALL_KEY, GameData.VARIANT_LIGHT, 32, 128);
				g.DrawString(GameData.FONT_SMALL, "X" + dungeon.NumSmallKeys.ToString(), new Point2I(40, 136), new Color(144, 136, 16)); // drop shadow
				g.DrawString(GameData.FONT_SMALL, "X" + dungeon.NumSmallKeys.ToString(), new Point2I(40, 136 - 1), new Color(32, 24, 16));
			}
		}
	}
}
