using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeldaOracle.Game.Worlds {
	/// <summary>The different modes available for room respawning.</summary>
	public enum RoomRespawnMode {
		/// <summary>Monsters will never respawn.</summary>
		Never,
		/// <summary>Monster respawn counters only decrement when visiting a
		/// room that hasn't been visited before since monsters last respawned.</summary>
		Overworld,
		/// <summary>Monster respawn counters only decrement when visiting a
		/// room that hasn't been visited before since entering this area.</summary>
		Dungeon,
		/// <summary>The room's monsters always respawn when re-entering the room.</summary>
		Always
	}
}
