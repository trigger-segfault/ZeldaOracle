using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeldaOracle.Game.Worlds {
	/// <summary>Spawn modes for monsters to override the area spawn mode.</summary>
	public enum MonsterSpawnMode {
		/// <summary>This monster will spawn using the area's spawn mode.</summary>
		Normal = 0,
		/// <summary>This monster will spawn in a random position.</summary>
		Random = 1,
		/// <summary>This monster will spawn in its current position.</summary>
		Fixed = 2
	}

	/// <summary>The method for spawning monsters for the area.
	/// This is only used for the editor.</summary>
	public enum AreaSpawnMode {
		/// <summary>All monsters will spawn in a random position.</summary>
		Random = 1,
		/// <summary>All monsters will spawn in their current position.</summary>
		Fixed = 2
	}
}
