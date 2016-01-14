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

		private Image	backgroundImage;
		private Dungeon	dungeon;
		private int		viewFloorIndex;		// Which floor wer are currently viewing.
		private DungeonMapFloor viewFloor;
		private int		playerFloorNumber;	// Which floor the player is on.
		private Point2I	playerRoomLocation;
		private int		floorViewPosition;	// The current view position.
		private int		floorViewSpeed;		// How fast to move the view when changing floors.
		private bool	isChangingFloors;
		private int		cursorTimer;
		private int		lowestFloorNumber;
		private int		highestFloorNumber;
		private List<DungeonMapFloor> floors;
		private List<DungeonMapFloor> discoveredFloors;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public ScreenDungeonMap(GameManager gameManager) {
			this.gameManager		= gameManager;
			this.backgroundImage	= Resources.GetImage("screen_dungeon_map");
			this.dungeon			= null;
			this.floors				= new List<DungeonMapFloor>();
			this.discoveredFloors	= new List<DungeonMapFloor>();
		}


		//-----------------------------------------------------------------------------
		// Map Drawing
		//-----------------------------------------------------------------------------

		private void DrawRoom(Graphics2D g, DungeonMapRoom room, Point2I position) {
			if (room == null)
				return;

			// Determine the sprite to draw for the room.
			Sprite sprite = null;
			if (room.IsDiscovered)
				sprite = room.Sprite;
			else if (dungeon.HasMap)
				sprite = GameData.SPR_UI_MAP_UNDISCOVERED_ROOM;

			// Determine extra sprite to draw for the room (treasure, boss, or player).
			Sprite extraSprite = null;
			if (playerRoomLocation == room.Location && playerFloorNumber == room.Floor.FloorNumber &&
				(cursorTimer >= 32 || isChangingFloors || room.Floor != viewFloor))
			{
				extraSprite = GameData.SPR_UI_MAP_PLAYER;
			}
			else if (dungeon.HasCompass) {
				if (room.IsBossRoom)
					extraSprite = GameData.SPR_UI_MAP_BOSS_ROOM;
				else if (room.HasTreasure)
					extraSprite = GameData.SPR_UI_MAP_TREASURE_ROOM;
			}

			// Draw the two sprites.
			if (sprite != null)
				g.DrawSprite(sprite, GameData.VARIANT_LIGHT, position);
			if (extraSprite != null)
				g.DrawSprite(extraSprite, GameData.VARIANT_LIGHT, position);
		}

		private void DrawFloor(Graphics2D g, DungeonMapFloor floor, Point2I position) {
			// Draw the floor background rectangle.
			g.DrawSprite(GameData.SPR_UI_MAP_FLOOR_BACKGROUND,
				GameData.VARIANT_LIGHT, new Rectangle2I(position, new Point2I(64, 64)));
			
			// Draw the rooms.
			for (int x = 0; x < floor.Width; x++) {
				for (int y = 0; y < floor.Height; y++) {
					Point2I drawPos = position + (new Point2I(x, y) * 8);
					DrawRoom(g, floor.Rooms[x, y], drawPos);
				}
			}
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public void OnOpen() {
			Room playerRoom = GameControl.LastRoomOnMap;

			dungeon				= playerRoom.Dungeon;
			playerRoomLocation	= playerRoom.Location;
			playerFloorNumber	= 0;
			viewFloorIndex		= 0;
			floorViewSpeed		= 8;
			isChangingFloors	= false;
			cursorTimer			= 0;
			viewFloor			= null;
			
			// Add the dungeon floors.
			DungeonFloor[] levelFloors = dungeon.GetFloors();
			lowestFloorNumber	= levelFloors[0].FloorNumber;
			highestFloorNumber	= levelFloors[levelFloors.Length - 1].FloorNumber;
			floors.Clear();
			discoveredFloors.Clear();

			for (int i = 0; i < levelFloors.Length; i++) {
				DungeonMapFloor floor = new DungeonMapFloor(levelFloors[i]);
				floors.Add(floor);

				if (floor.DungeonFloor.Level == playerRoom.Level) {
					playerFloorNumber	= floor.FloorNumber;
					viewFloor			= floor;
					viewFloorIndex		= discoveredFloors.Count;
				}
				if (floor.IsDiscovered || dungeon.HasMap)
					discoveredFloors.Add(floor);
			}
			
			floorViewPosition = viewFloorIndex * 80;
		}

		public void OnClose() {

		}

		public override void OnBegin() {
		}
		
		public override void Update() {
			cursorTimer++;
			if (cursorTimer >= 64)
				cursorTimer = 0;

			// [SELECT] to  close the map screen.
			if (Controls.Select.IsPressed()) {
				GameControl.CloseMapScreen();
				return;
			}

			if (!isChangingFloors) {
				// [UP] and [DOWN] to change floors.
				if (Controls.Up.IsPressed() && viewFloorIndex < discoveredFloors.Count - 1) {
					viewFloorIndex++;
					isChangingFloors = true;
					viewFloor = discoveredFloors[viewFloorIndex];
					AudioSystem.PlaySound("UI/menu_cursor_move");
				}
				else if (Controls.Down.IsPressed() && viewFloorIndex > 0) {
					viewFloorIndex--;
					isChangingFloors = true;
					viewFloor = discoveredFloors[viewFloorIndex];
					AudioSystem.PlaySound("UI/menu_cursor_move");
				}
			}
			else {
				// Update changing of floor view position.
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

				if (discoveredFloors.Contains(floor)) {
					// Draw the floor's label box on the left side of the screen.
					Point2I floorPos = floorBasePos - new Point2I(0, i * 8);
					string floorName = floor.FloorNumberText;
					g.DrawString(GameData.FONT_SMALL, floorName, floorPos, new Color(248, 248, 216)); // drop shadow
					g.DrawString(GameData.FONT_SMALL, floorName, floorPos + new Point2I(0, -1), new Color(56, 32, 16));
					g.DrawSprite(GameData.SPR_UI_MAP_FLOOR_BOX_LEFT, GameData.VARIANT_LIGHT, floorPos + new Point2I(32, 0));
					g.DrawSprite(GameData.SPR_UI_MAP_FLOOR_BOX_RIGHT, GameData.VARIANT_LIGHT, floorPos + new Point2I(40, 0));

					// Draw the icons around the name box.
					if (viewFloor == floor)
						g.DrawSprite(GameData.SPR_UI_MAP_FLOOR_INDICATOR, GameData.VARIANT_LIGHT, floorPos + new Point2I(24, 0));
					if (playerFloorNumber == floor.FloorNumber)
						g.DrawSprite(GameData.SPR_UI_MAP_PLAYER, GameData.VARIANT_LIGHT, floorPos + new Point2I(36, 0));
					if (floor.IsBossFloor && dungeon.HasCompass)
						g.DrawSprite(GameData.SPR_UI_MAP_BOSS_FLOOR, GameData.VARIANT_LIGHT, floorPos + new Point2I(48, 0));
					
					// Draw the floor's room display on the right side of the screen.
					int discoveredFloorIndex = discoveredFloors.IndexOf(floor);
					Point2I floorRoomDisplayPos = new Point2I(80, 40 - (80 * discoveredFloorIndex) + floorViewPosition);
					if (floorRoomDisplayPos.Y < GameSettings.SCREEN_HEIGHT && floorRoomDisplayPos.Y > -80)
						DrawFloor(g, floor, floorRoomDisplayPos);
				
					// Draw room display cursor.
					if (!isChangingFloors && viewFloor == floor && cursorTimer < 32) {
						Point2I drawPos = floorRoomDisplayPos + (playerRoomLocation * 8);
						g.DrawSprite(GameData.SPR_UI_MAP_CURSOR, GameData.VARIANT_LIGHT, drawPos);
					}
				}
			}
			
			// Draw floor view traversal arrows.
			if (!isChangingFloors) {
    			if (viewFloorIndex > 0)
    				g.DrawSprite(GameData.SPR_UI_MAP_ARROW_DOWN, GameData.VARIANT_LIGHT, 108, 108);
    			if (viewFloorIndex < discoveredFloors.Count - 1)
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
