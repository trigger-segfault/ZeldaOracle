using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeldaOracle.Game.Worlds {
	/// <summary>A structure to keep track of information about a dungeon's floor level.</summary>
	public class DungeonFloor {
		/// <summary>The level assigned to this floor.</summary>
		private Level level;
		/// <summary>The number of the floor. 0 == 1F</summary>
		private int floorNumber;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		/// <summary>Constructs the dungeon floor.</summary>
		public DungeonFloor(Level level, int floorNumber) {
			this.level = level;
			this.floorNumber = floorNumber;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets the level assigned to this floor.</summary>
		public Level Level {
			get { return level; }
		}

		/// <summary>Gets the number of the floor. 0 == 1F</summary>
		public int FloorNumber {
			get { return floorNumber; }
		}

		/// <summary>Gets if this floor has been visited yet.</summary>
		public bool IsDiscovered {
			get {
				if (level != null)
					return level.IsDiscovered;
				return false;
			}
		}
	}
}
