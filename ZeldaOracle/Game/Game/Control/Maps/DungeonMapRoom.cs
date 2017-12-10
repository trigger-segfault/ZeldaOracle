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
using ZeldaOracle.Common.Graphics.Sprites;

namespace ZeldaOracle.Game.Control.Maps {
	
	public class DungeonMapRoom {

		private DungeonMapFloor floor;
		private Room	room;
		private Point2I	location;
		private bool	hasTreasure;
		private bool	isBossRoom;
		private bool	isDiscovered;
		private ISprite	sprite;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public DungeonMapRoom() {
			this.floor			= null;
			this.room			= null;
			this.hasTreasure	= false;
			this.isBossRoom		= false;
			this.sprite			= null;
			this.isDiscovered	= false;
			this.location		= Point2I.Zero;
		}


		//-----------------------------------------------------------------------------
		// Static Methods
		//-----------------------------------------------------------------------------

		public static DungeonMapRoom Create(Room room, DungeonMapFloor floor) {
			// Don't show empty rooms.
			if (IsRoomEmpty(room))
				return null;

			// Create the map room object.
			DungeonMapRoom mapRoom = new DungeonMapRoom() {
				room			= room,
				hasTreasure		= room.HasUnopenedTreasure(),
				isDiscovered	= room.IsDiscovered,
				isBossRoom		= room.IsBossRoom,
				location		= room.Location,
				sprite			= null,
				floor			= floor,
			};
			
			// Determine the sprite to draw for this room based on its connections.
			ISprite[] connectedSprites = new ISprite[16] {
				GameData.SPR_UI_MAP_ROOM_NONE,
				GameData.SPR_UI_MAP_ROOM_RIGHT,
				GameData.SPR_UI_MAP_ROOM_UP,
				GameData.SPR_UI_MAP_ROOM_UP_RIGHT,
				GameData.SPR_UI_MAP_ROOM_LEFT,
				GameData.SPR_UI_MAP_ROOM_LEFT_RIGHT,
				GameData.SPR_UI_MAP_ROOM_LEFT_UP,
				GameData.SPR_UI_MAP_ROOM_LEFT_UP_RIGHT,
				GameData.SPR_UI_MAP_ROOM_DOWN,
				GameData.SPR_UI_MAP_ROOM_DOWN_RIGHT,
				GameData.SPR_UI_MAP_ROOM_DOWN_UP,
				GameData.SPR_UI_MAP_ROOM_DOWN_UP_RIGHT,
				GameData.SPR_UI_MAP_ROOM_DOWN_LEFT,
				GameData.SPR_UI_MAP_ROOM_DOWN_LEFT_RIGHT,
				GameData.SPR_UI_MAP_ROOM_DOWN_LEFT_UP,
				GameData.SPR_UI_MAP_ROOM_DOWN_LEFT_UP_RIGHT,
			};

			// Check for room connections.
			int[] connected = new int[] { 0, 0, 0, 0 };
			for (int y = 0; y < room.Height; y++) {
				bool freeOnRight = true;
				bool freeOnLeft = true;
				for (int i = 0; i < room.LayerCount; i++) {
					TileDataInstance left = room.GetTile(0, y, i);
					if (left != null && left.SolidType != TileSolidType.NotSolid && !typeof(TileDoor).IsAssignableFrom(left.Type))
						freeOnLeft = false;
					TileDataInstance right = room.GetTile(room.Width - 1, y, i);
					if (right != null && right.SolidType != TileSolidType.NotSolid && !typeof(TileDoor).IsAssignableFrom(right.Type))
						freeOnRight = false;;
				}
				if (freeOnRight)
					connected[Directions.Right] = 1;
				if (freeOnLeft)
					connected[Directions.Left] = 1;
			}
			for (int x = 0; x < room.Width; x++) {
				bool freeOnUp = true;
				bool freeOnDown = true;
				for (int i = 0; i < room.LayerCount; i++) {
					TileDataInstance up = room.GetTile(x, 0, i);
					TileDataInstance down = room.GetTile(x, room.Height - 1, i);
					if (up != null && up.SolidType != TileSolidType.NotSolid && !typeof(TileDoor).IsAssignableFrom(up.Type))
						freeOnUp = false;
					if (down != null && down.SolidType != TileSolidType.NotSolid && !typeof(TileDoor).IsAssignableFrom(down.Type))
						freeOnDown = false;
				}
				if (freeOnUp)
					connected[Directions.Up] = 1;
				if (freeOnDown)
					connected[Directions.Down] = 1;
			}

			int spiteIndex = (connected[0]) + (connected[1] << 1) + (connected[2] << 2) + (connected[3] << 3);
			mapRoom.sprite = connectedSprites[spiteIndex];

			return mapRoom;
		}

		public static bool IsRoomEmpty(Room room) {
			TileData sameTileType = null;
			for (int layer = 0; layer < room.LayerCount; layer++) {
				for (int x = 0; x < room.Width; x++) {
					for (int y = 0; y < room.Height; y++) {
						TileDataInstance tile = room.GetTile(x, y, layer);
						if (tile != null && layer > 0)
							return false;
						if (layer == 0) {
							if (sameTileType != null && tile == null)
								return false;
							else if (sameTileType != null && tile != null && tile.TileData != sameTileType)
								return false;
							else if (sameTileType == null && tile != null)
								sameTileType = tile.TileData;
						}
					}
				}
			}
			return true;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public bool IsBossRoom {
			get { return isBossRoom; }
		}

		public bool HasTreasure {
			get { return hasTreasure; }
		}

		public bool IsDiscovered {
			get { return isDiscovered; }
		}

		public ISprite Sprite {
			get { return sprite; }
		}

		public Room Room {
			get { return room; }
		}

		public Point2I Location {
			get { return location; }
		}

		public DungeonMapFloor Floor {
			get { return floor; }
			set { floor = value; }
		}
	}

}
