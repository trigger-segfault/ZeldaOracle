using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Input;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Entities.Projectiles;
using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.Worlds;
using ZeldaOracle.Common.Audio;

namespace ZeldaOracle.Game.Tiles {

	public class TileDoor : Tile, ZeldaAPI.Door {
		protected bool isOpen;
		private bool isPlayerBlockingClose;
		protected Animation animationOpen;
		protected Animation animationClose;
		protected Sound openCloseSound;
		private bool hasUpdated;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public TileDoor() {
			animationOpen	= GameData.ANIM_TILE_DOOR_OPEN;
			animationClose	= GameData.ANIM_TILE_DOOR_CLOSE;
			openCloseSound	= GameData.SOUND_DUNGEON_DOOR;
			Graphics.SyncPlaybackWithRoomTicks = false;
		}


		//-----------------------------------------------------------------------------
		// Door methods
		//-----------------------------------------------------------------------------
		/*
		public void Open(bool instantaneous, bool rememberState) {
			Open(false, false);
		}
		public void Close(bool instantaneous, bool rememberState) {
			Close(false, false);
		}*/

		// Open the door.
		public void Open(bool instantaneous = false, bool rememberState = false) {
			if (!isOpen) {
				isOpen = true;
				Graphics.PlayAnimation(animationOpen);
				IsSolid = false;
				if (!instantaneous && openCloseSound != null)
					AudioSystem.PlaySound(GameData.SOUND_DUNGEON_DOOR);
			}
			else if (isOpen && isPlayerBlockingClose)
				isPlayerBlockingClose = false;

			Properties.Set("open", true);
			
			if (instantaneous)
				Graphics.AnimationPlayer.SkipToEnd();
		}

		// Close the door.
		public void Close(bool instantaneous = false, bool rememberState = false) {
			if (isOpen) {
				Player player = RoomControl.Player;
				bool isTouchingPlayer = CollisionModel.Intersecting(CollisionModel, Position, player.Physics.PositionedCollisionBox);


				if (isTouchingPlayer && !hasUpdated) {
						isPlayerBlockingClose = true;
				}
				else {
					if (isTouchingPlayer) {
						// Damage & respawn the player
						player.RespawnDeathInstantaneous();
					}

					isOpen = false;
					Graphics.PlayAnimation(animationClose);
					IsSolid = true;
					if (!instantaneous && openCloseSound != null)
						AudioSystem.PlaySound(GameData.SOUND_DUNGEON_DOOR);
				}
			}

			Properties.Set("open", false);
			
			if (instantaneous)
				Graphics.AnimationPlayer.SkipToEnd();
		}

		// Find a door that's connected to this one in an adjacent room.
		public TileDataInstance GetConnectedDoor() {
			// Find the location of the adjacent room.
			int dir = Directions.Reverse(Properties.GetInteger("direction", 0));
			Point2I roomLocation = RoomControl.RoomLocation + Directions.ToPoint(dir);

			if (RoomControl.Level.ContainsRoom(roomLocation)) {
				Room adjacentRoom = RoomControl.Level.GetRoomAt(roomLocation);

				// Find the location of the adjacent door tile.
				Point2I doorLocation = Location;
				if (dir == Directions.Down)
					doorLocation.Y = 0;
				else if (dir == Directions.Up)
					doorLocation.Y = adjacentRoom.Height - 1;
				else if (dir == Directions.Right)
					doorLocation.X = 0;
				else if (dir == Directions.Left)
					doorLocation.X = adjacentRoom.Width - 1;

				// Look for a tile of the same type among the tile layers.
				for (int i = 0; i < adjacentRoom.LayerCount; i++) {
					TileDataInstance t = adjacentRoom.GetTile(doorLocation, i);
					if (t != null && t.Type == GetType())
						return t;
				}
			}
			return null;
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------
		
		public override void OnInitialize() {
			isOpen = Properties.GetBoolean("open", false);
			isPlayerBlockingClose = false;

			hasUpdated = false;

			animationOpen = TileData.TileData.SpriteList[1].Animation;
			animationClose = TileData.TileData.SpriteList[2].Animation;

			// A closed door will start open if the player is colliding with door.
			if (!isOpen) {
				Player player = RoomControl.Player;
				if (CollisionModel.Intersecting(CollisionModel, Position, player.Physics.CollisionBox, player.Position)) {
					isOpen = true;
					isPlayerBlockingClose = true;
				}
			}

			if (isOpen) {
				Graphics.PlayAnimation(animationOpen);
				IsSolid = false;
			}
			else {
				Graphics.PlayAnimation(animationClose);
				IsSolid = true;
			}

			// Fast-forward the animation to the end.
			Graphics.AnimationPlayer.SkipToEnd();
			Graphics.SubStripIndex = Properties.GetInteger("direction", 0);
		}

		public override void Update() {
			base.Update();

			hasUpdated = true;
			
			Player player = RoomControl.Player;
			if (isPlayerBlockingClose && !CollisionModel.Intersecting(CollisionModel, Position, player.Physics.CollisionBox, player.Position)) {
				Close();
				isPlayerBlockingClose = false;
				player.MarkRespawn();
			}
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public bool IsOpen {
			get { return (isOpen && !isPlayerBlockingClose); }
		}

		public int Direction {
			 get { return Properties.GetInteger("direction", 0); }
		}
	}
}
