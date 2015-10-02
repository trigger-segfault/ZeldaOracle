using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Worlds;
using ZeldaOracle.Game.GameStates.Transitions;

namespace ZeldaOracle.Game.Tiles.EventTiles {

	public enum WarpType {
		Tunnel,
		Entrance,
		Stairs
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
			string warpLevelID = Properties.GetString("destination_level", RoomControl.Level.Name);
			Level warpLevel = RoomControl.GameControl.World.GetLevel(warpLevelID);
			if (warpLevel == null)
				return;

			// Search for warp tile in all rooms.
			Room destRoom;
			EventTileDataInstance destEvent;
			string warpID = properties.GetString("destination_warp_point", "?");

			if (FindDestinationInLevel(warpID, warpLevel, out destRoom, out destEvent)) {
				int dir = destEvent.ModifiedProperties.GetInteger("face_direction", Directions.Left); // TODO: modified properties might not cut it here
				RoomControl.TransitionToRoom(destRoom, new RoomTransitionFade(destEvent.Position, dir));
			}
			else {
				Console.WriteLine("Invalid warp destination!");
			}
		}

		public bool FindDestinationInLevel(string id, Level level, out Room destRoom, out EventTileDataInstance destEvent) {
			for (int x = 0; x < level.Width; x++) {
				for (int y = 0; y < level.Height; y++) {
					if (FindDestinationInRoom(id, level.GetRoom(x, y), out destRoom, out destEvent))
						return true;
				}
			}
			destRoom = null;
			destEvent = null;
			return false;
		}

		public bool FindDestinationInRoom(string id, Room room, out Room destRoom, out EventTileDataInstance destEvent) {
			for (int i = 0; i < room.EventData.Count; i++) {
				if (room.EventData[i].ModifiedProperties.GetString("id", "") == id) {
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

			warpType = WarpType.Tunnel;
			if (typeName == "tunnel")
				warpType = WarpType.Tunnel;
			else if (typeName == "entrance")
				warpType = WarpType.Entrance;
			else if (typeName == "stairs")
				warpType = WarpType.Stairs;
			
			collisionBox	= new Rectangle2I(2, 6, 12, 12);
			warpEnabled		= !IsTouchingPlayer();
		}

		public override void Update() {
			base.Update();

			if (IsTouchingPlayer()) {
				if (warpEnabled && RoomControl.Player.IsOnGround) {
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
