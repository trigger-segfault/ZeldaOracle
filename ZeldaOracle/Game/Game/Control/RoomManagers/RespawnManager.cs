using System.Collections.Generic;
using ZeldaOracle.Game.Worlds;

namespace ZeldaOracle.Game.Control.RoomManagers {

	/// <summary>Manages room clearing and tells a room to respawn when it needs to.
	/// </summary>
	public class RespawnManager : AreaManager {

		//-----------------------------------------------------------------------------
		// Classes
		//-----------------------------------------------------------------------------

		/// <summary>A collection of visited rooms and a counter for a cleared room,
		/// as well as a tracker for cleared monsters.</summary>
		private class RoomRespawning {
			/// <summary>The room that is being managed.</summary>
			private RoomIdentifier room;
			/// <summary>The respawn mode for the area.</summary>
			private RoomRespawnMode respawnMode;
			/// <summary>The collection of visited rooms.</summary>
			private HashSet<RoomIdentifier> visitedRooms;
			/// <summary>The collection of dead monsters.</summary>
			private HashSet<int> deadMonsters;
			/// <summary>True if the room is currently cleared.</summary>
			private bool cleared;


			//-----------------------------------------------------------------------------
			// Constructors
			//-----------------------------------------------------------------------------

			/// <summary>Constructed the visited rooms.</summary>
			public RoomRespawning(RoomIdentifier room, RoomRespawnMode respawnMode) {
				this.room = room;
				this.respawnMode = respawnMode;
				this.visitedRooms = new HashSet<RoomIdentifier>();
				this.deadMonsters = new HashSet<int>();
				this.cleared = false;
			}


			//-----------------------------------------------------------------------------
			// Visiting
			//-----------------------------------------------------------------------------
			
			/// <summary>Tries to add this room to the list of visited rooms. Returns true
			/// if the room was respawned.</summary>
			public bool OnVisitRoom(RoomIdentifier room) {
				if (cleared) {
					if (room != this.room) {
						// Add this room to our list of visited rooms.
						visitedRooms.Add(room);
					}
					else if (CanRespawn) {
						RespawnRoom();
						return true;
					}
				}
				return false;
			}

			private void RespawnRoom() {
				cleared = false;
				if (respawnMode != RoomRespawnMode.Dungeon)
					visitedRooms.Clear();
				deadMonsters.Clear();
			}

			/// <summary>Marks the room as cleared so that visited rooms are counted.
			/// Returns true if the room was not already cleared.</summary>
			public bool Clear() {
				if (!cleared) {
					cleared = true;
					return true;
				}
				return false;
			}

			/// <summary>Marks the monster with the specified ID as cleared.</summary>
			public void ClearMonster(int id) {
				if (id != 0 && id != -1)
					deadMonsters.Add(id);
			}

			/// <summary>Returns true if the monster with the specified ID is dead.
			/// </summary>
			public bool IsMonsterDead(int id) {
				return (cleared || (id != 0 && id != -1 && deadMonsters.Contains(id)));
			}


			//-----------------------------------------------------------------------------
			// Properties
			//-----------------------------------------------------------------------------

			/// <summary>Gets the number of other rooms visited.</summary>
			public int VisitedRoomCount {
				get { return visitedRooms.Count; }
			}

			/// <summary>True if the room is currently cleared.</summary>
			public bool IsCleared {
				get { return cleared; }
			}

			/// <summary>True if the respawn requirements have been fulfilled.</summary>
			public bool CanRespawn {
				get {
					return (cleared && visitedRooms.Count >=
							GameSettings.ROOM_RESPAWN_VISIT_COUNT &&
						respawnMode != RoomRespawnMode.Never) ||
						respawnMode == RoomRespawnMode.Always;
				}
			}
		}


		//-----------------------------------------------------------------------------
		// Members
		//-----------------------------------------------------------------------------
		
		/// <summary>A direct list of the rooms that need respawn management.</summary>
		private Dictionary<RoomIdentifier, RoomRespawning> managedRooms;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		/// <summary>Constructs the room clear manager for the area.</summary>
		public RespawnManager(AreaControl areaControl) : base(areaControl) {
			managedRooms = new Dictionary<RoomIdentifier, RoomRespawning>();
		}


		//-----------------------------------------------------------------------------
		// Room Management
		//-----------------------------------------------------------------------------

		/// <summary>Marks the room as cleared and begins counting visited rooms.
		/// Returns true if the room was not already cleared.</summary>
		public bool ClearRoom(RoomIdentifier room) {
			return GetManagedRoom(room).Clear();
		}

		/// <summary>Marks the room as visited for all clear rooms and respawns this
		/// room if it is ready.</summary>
		public bool VisitRoom(RoomIdentifier room) {
			bool respawned = false;
			foreach (var pair in managedRooms) {
				if (pair.Value.OnVisitRoom(room))
					respawned = true;
			}
			return respawned;
		}

		/// <summary>Gets if the room is cleared.</summary>
		public bool IsRoomCleared(RoomIdentifier room) {
			RoomRespawning visitedRooms;
			if (managedRooms.TryGetValue(room, out visitedRooms))
				return visitedRooms.IsCleared;
			return false;
		}
		
		/// <summary>Checks if a monster with the specified ID is cleared.
		/// Monster IDs are unique to each "root" room.</summary>
		public bool IsMonsterDead(RoomIdentifier room, int monsterID) {
			return GetManagedRoom(room).IsMonsterDead(monsterID);
		}

		/// <summary>Registers a monster with the specified ID as cleared.
		/// Monster IDs are unique to each "root" room.</summary>
		public void ClearMonster(RoomIdentifier room, int monsterID) {
			if (monsterID != 0 && monsterID != -1)
				GetManagedRoom(room).ClearMonster(monsterID);
		}


		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------

		/// <summary>Gets the existing managed room or adds a new one.</summary>
		private RoomRespawning GetManagedRoom(RoomIdentifier room) {
			RoomRespawning managedRoom;
			// Add the managed room if it doesn't exist.
			if (!managedRooms.TryGetValue(room, out managedRoom)) {
				managedRoom = new RoomRespawning(room, RespawnMode);
				managedRooms.Add(room, managedRoom);
			}
			return managedRoom;
		}

		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets the respawn mode of the area.</summary>
		private RoomRespawnMode RespawnMode {
			get { return AreaControl.RespawnMode; }
		}
	}
}
