using System;
using System.ComponentModel;
using ZeldaOracle.Common.Scripting;

namespace ZeldaOracle.Game {
	
	/// <summary>Magnetic polarity (north or south).</summary>
	public enum Polarity {
		None = -1,
		North = 0,
		South = 1,
	}
	
	/// <summary>The types of seeds.</summary>
	public enum SeedType {
		Ember	= 0,
		Scent	= 1,
		Pegasus	= 2,
		Gale	= 3,
		Mystery	= 4,
	}

	/// <summary>Buttons used to perform player actions (A or B).</summary>
	public enum ActionButtons {
		A = 0,
		B = 1,

		[Browsable(false)]
		Count = 2,
	}
	
	/// <summary>The different colored tunics the player can wear.</summary>
	public enum PlayerTunics {
		GreenTunic	= 0,
		RedTunic	= 1,
		BlueTunic	= 2,
	}
	
	/// <summary>The types of liquids that the player can swim in.</summary>
	[Flags]
	public enum PlayerSwimmingSkills {
		/// <summary>The player cannot swim in anything.</summary>
		CannotSwim		= 0,

		/// <summary>The player can swim in normal and deep water.</summary>
		CanSwimInWater	= (1 << 0),

		/// <summary>The player can swim in ocean water.
		/// CanSwimInWater should always be set with this.</summary>
		CanSwimInOcean	= (1 << 1),

		/// <summary>The player can swim in lava without burning to a crisp.</summary>
		CanSwimInLava	= (1 << 2),
	}

	/// <summary>Spawn modes for monsters to override the area spawn mode.</summary>
	public enum MonsterSpawnMode {
		/// <summary>This monster will spawn using the area's spawn mode.</summary>
		Normal = 0,

		/// <summary>This monster will spawn in a random position.</summary>
		Random = 1,

		/// <summary>This monster will spawn in its current position.</summary>
		Fixed = 2,
	}

	/// <summary>The method for spawning monsters for the area.
	/// This is only used for the editor.</summary>
	public enum AreaSpawnMode {
		/// <summary>All monsters will spawn in a random position.</summary>
		Random = 1,

		/// <summary>All monsters will spawn in their current position.</summary>
		Fixed = 2,
	}

	/// <summary>The different modes available for a specific monster's respawning.</summary>
	public enum MonsterRespawnType {
		/// <summary>The monster will never respawn.</summary>
		Never = 0,

		/// <summary>The monster will always respawn when entering the room.</summary>
		Always = 1,

		/// <summary>The monster will use the area's existing respawn mode.</summary>
		Normal = 2,
	}

	/// <summary>The different modes available for room respawning.</summary>
	public enum RoomRespawnMode {
		/// <summary>Monsters will never respawn.</summary>
		Never = 0,

		/// <summary>Monster respawn counters only decrement when visiting a
		/// room that hasn't been visited before since monsters last respawned.</summary>
		Overworld = 1,

		/// <summary>Monster respawn counters only decrement when visiting a
		/// room that hasn't been visited before since entering this area.</summary>
		Dungeon = 2,

		/// <summary>The room's monsters always respawn when re-entering the room.</summary>
		Always = 3,
	}

	/// <summary>An interface for objects that can be intercepted during
	/// entity interactions.</summary>
	public interface IInterceptable {
		/// <summary>Intercept the object. Returns true if the object was successfully
		/// intercpted.</summary>
		bool Intercept();
	}

}
