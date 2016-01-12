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

namespace ZeldaOracle.Game.Control.Menus {
	public class ScreenDungeonMap : GameState {

		private Image backgroundImage;
		private Dungeon dungeon;
		private int viewFloorIndex; // Which floor wer are currently viewing.
		private int playerFloorIndex; // Which floor the player is on.
		private int[] floorNumbers; // 2 = "3F", 1 = "2F", 0 = "1F", -1 = "B1F", -2 = "B2F", etc.
		private int floorViewPosition;		// The current view position.
		private int floorViewSpeed;			// How fast to move the view when changing floors.
		private bool isChangingFloors;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public ScreenDungeonMap(GameManager gameManager) {
			this.gameManager = gameManager;
			this.backgroundImage = Resources.GetImage("screen_dungeon_map");
			this.dungeon = null;
		}


		//-----------------------------------------------------------------------------
		// Internal methods
		//-----------------------------------------------------------------------------

		private void DrawFloor(Graphics2D g, int floorIndex, Point2I position) {
			g.DrawSprite(GameData.SPR_UI_MAP_FLOOR_BACKGROUND, GameData.VARIANT_LIGHT, new Rectangle2I(position, new Point2I(64, 64)));

			// Draw the rooms.
			for (int x = 0; x < 8; x++) {
				for (int y = 0; y < 8; y++) {
					// Draw room.
				}
			}
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public void OnOpen() {
			dungeon	= GameControl.RoomControl.Dungeon;

			floorNumbers		= new int[] { -2, -1, 0, 1, 2 };
			playerFloorIndex	= 0;
			viewFloorIndex		= playerFloorIndex;
			floorViewPosition	= viewFloorIndex * 80;
			floorViewSpeed		= 8;
			isChangingFloors	= false;
		}

		public void OnClose() {

		}

		public override void OnBegin() {
		}
		
		public override void Update() {
			// [SELECT] Close the map screen.
			if (Controls.Select.IsPressed()) {
				GameControl.CloseMapScreen();
				return;
			}

			// [UP] and [DOWN] to change floors.
			if (!isChangingFloors) {
				if (Controls.Up.IsPressed() && viewFloorIndex < floorNumbers.Length - 1) {
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
			if (floorNumbers.Length < 6)
				floorBasePos.Y = 72 + (8 * (floorNumbers.Length / 2));
			else
				floorBasePos.Y = 88 + (4 * (floorNumbers.Length - 6));

			for (int i = 0; i < floorNumbers.Length; i++) {
				// Draw the floor's label box on the left side of the screen.
				Point2I floorPos = floorBasePos - new Point2I(0, i * 8);
				string floorName;
				if (floorNumbers[i] < 0)
					floorName = "B" + (-floorNumbers[i]) + "F";
				else
					floorName = " " + (floorNumbers[i] + 1) + "F";
				g.DrawString(GameData.FONT_SMALL, floorName, floorPos, new Color(248, 248, 216)); // drop shadow
				g.DrawString(GameData.FONT_SMALL, floorName, floorPos + new Point2I(0, -1), new Color(56, 32, 16));
				g.DrawSprite(GameData.SPR_UI_MAP_FLOOR_BOX_LEFT, GameData.VARIANT_LIGHT, floorPos + new Point2I(32, 0));
				g.DrawSprite(GameData.SPR_UI_MAP_FLOOR_BOX_RIGHT, GameData.VARIANT_LIGHT, floorPos + new Point2I(40, 0));
				if (viewFloorIndex == i)
					g.DrawSprite(GameData.SPR_UI_MAP_FLOOR_INDICATOR, GameData.VARIANT_LIGHT, floorPos + new Point2I(24, 0));
				if (playerFloorIndex == i)
					g.DrawSprite(GameData.SPR_UI_MAP_PLAYER, GameData.VARIANT_LIGHT, floorPos + new Point2I(36, 0));
				//if (isBossFloor)
					//g.DrawSprite(GameData.SPR_UI_MAP_BOSS_FLOOR, GameData.VARIANT_LIGHT, floorPos + new Point2I(48, 0));
							
				// Draw the floor's room display on the right side of the screen.
				Point2I floorRoomDisplayPos = new Point2I(80, 40 - (80 * i) + floorViewPosition);
				if (floorRoomDisplayPos.Y < GameSettings.SCREEN_HEIGHT && floorRoomDisplayPos.Y > -80)
					DrawFloor(g, i, floorRoomDisplayPos);
			}
			
			// Draw floor view traversal arrows.
			if (!isChangingFloors) {
    			if (viewFloorIndex > 0)
    				g.DrawSprite(GameData.SPR_UI_MAP_ARROW_DOWN, GameData.VARIANT_LIGHT, 108, 108);
    			if (viewFloorIndex < floorNumbers.Length - 1)
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

			// TODO: Draw the room map panel.
		}
	}
}
