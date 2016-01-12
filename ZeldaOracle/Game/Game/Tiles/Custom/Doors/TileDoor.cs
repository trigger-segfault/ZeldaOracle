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

namespace ZeldaOracle.Game.Tiles {

	public class TileDoor : Tile, ZeldaAPI.Door {
		protected bool isOpen;
		private bool isPlayerBlockingClose;
		protected Animation animationOpen;
		protected Animation animationClose;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public TileDoor() {
			animationPlayer	= new AnimationPlayer();
			animationOpen	= GameData.ANIM_TILE_DOOR_OPEN;
			animationClose	= GameData.ANIM_TILE_DOOR_CLOSE;
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
				animationPlayer.Play(animationOpen);
				IsSolid = false;
			}
			else if (isOpen && isPlayerBlockingClose)
				isPlayerBlockingClose = false;
			
			if (instantaneous)
				animationPlayer.PlaybackTime = animationPlayer.Animation.Duration;
			if (rememberState)
				Properties.SetBase("opened", true);
		}

		// Close the door.
		public void Close(bool instantaneous = false, bool rememberState = false) {
			if (isOpen) {
				Player player = RoomControl.Player;
				if (CollisionModel.Intersecting(CollisionModel, Position, player.Physics.CollisionBox, player.Position)) {
					isPlayerBlockingClose = true;
				}
				else {
					isOpen = false;
					animationPlayer.Play(animationClose);
					IsSolid = true;
				}
			}
			
			if (instantaneous)
				animationPlayer.PlaybackTime = animationPlayer.Animation.Duration;
			if (rememberState)
				Properties.SetBase("opened", false);
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
			isOpen = Properties.GetBoolean("opened", false);
			isPlayerBlockingClose = false;

			// A closed door will start open if the player is colliding with door.
			if (!isOpen) {
				Player player = RoomControl.Player;
				if (CollisionModel.Intersecting(CollisionModel, Position, player.Physics.CollisionBox, player.Position)) {
					isOpen = true;
					isPlayerBlockingClose = true;
				}
			}

			if (isOpen) {
				animationPlayer.Play(animationOpen);
				IsSolid = false;
			}
			else {
				animationPlayer.Play(animationClose);
				IsSolid = true;
			}

			// Fast-forward the animation to the end.
			animationPlayer.PlaybackTime = animationPlayer.Animation.Duration;
			
			animationPlayer.SubStripIndex = Properties.GetInteger("direction", 0);
		}

		public override void Update() {
			base.Update();

			
			Player player = RoomControl.Player;
			if (isPlayerBlockingClose && !CollisionModel.Intersecting(CollisionModel, Position, player.Physics.CollisionBox, player.Position)) {
				Close();
				isPlayerBlockingClose = false;
				player.MarkRespawn();
			}

			animationPlayer.Update();
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public bool IsOpen {
			get { return (isOpen && !isPlayerBlockingClose); }
		}
	}
}
