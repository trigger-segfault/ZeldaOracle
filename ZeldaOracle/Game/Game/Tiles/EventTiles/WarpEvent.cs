using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Worlds;
using ZeldaOracle.Game.GameStates.Transitions;
using ZeldaOracle.Game.Entities.Players;

namespace ZeldaOracle.Game.Tiles.EventTiles {

	public enum WarpType {
		Tunnel		= 0,
		Entrance	= 1,
		Stairs		= 2,

		Count		= 3
	}

	public class WarpEvent : EventTile {

		public WarpType warpType;
		public bool warpEnabled;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public WarpEvent() {
		}


		//-----------------------------------------------------------------------------
		// Warping
		//-----------------------------------------------------------------------------

		public void Warp() {
			string warpLevelID = Properties.GetString("destination_level", RoomControl.Level.Properties.GetString("id"));
			Level warpLevel = RoomControl.GameControl.World.GetLevel(warpLevelID);
			if (warpLevel == null)
				return;

			// Search for warp tile in all rooms.
			// TODO: include better functions for this task.
			Room destRoom;
			EventTileDataInstance destEvent;
			string warpID = properties.GetString("destination_warp_point", "?");

			if (FindDestinationInLevel(warpID, warpLevel, out destRoom, out destEvent)) {
				int dir = destEvent.Properties.GetInteger("face_direction", Directions.Down);
				RoomControl.TransitionToRoom(destRoom, new RoomTransitionFade(destEvent.Position, dir));
			}
			else {
				Console.WriteLine("Invalid warp destination!");
			}
		}

		public bool FindDestinationInLevel(string id, Level level, out Room destRoom, out EventTileDataInstance destEvent) {
			for (int x = 0; x < level.Width; x++) {
				for (int y = 0; y < level.Height; y++) {
					if (FindDestinationInRoom(id, level.GetRoomAt(x, y), out destRoom, out destEvent))
						return true;
				}
			}
			destRoom = null;
			destEvent = null;
			return false;
		}

		public bool FindDestinationInRoom(string id, Room room, out Room destRoom, out EventTileDataInstance destEvent) {
			for (int i = 0; i < room.EventData.Count; i++) {
				if (room.EventData[i].Properties.GetString("id", "") == id) {
					destRoom = room;
					destEvent = room.EventData[i];
					return true;
				}
			}
			destRoom = null;
			destEvent = null;
			return false;
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		protected override void Initialize() {
			base.Initialize();
			
			string typeName = Properties.GetString("warp_type", "Tunnel");
			warpType		= (WarpType) Enum.Parse(typeof(WarpType), typeName, true);
			collisionBox	= new Rectangle2I(2, 6, 12, 12);
			warpEnabled		= !IsTouchingPlayer();

			RoomControl.PlayerRespawn += delegate(Player player) {
				warpEnabled = !IsTouchingPlayer();
			};
		}

		public override void Update() {
			base.Update();

			if (IsTouchingPlayer()) {
				if (RoomControl.Player.CanUseWarpPoint && warpEnabled && RoomControl.Player.IsOnGround) {
					Warp();
					warpEnabled = false;
				}
			}
			else {
				warpEnabled = true;
			}
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------
	}
}
