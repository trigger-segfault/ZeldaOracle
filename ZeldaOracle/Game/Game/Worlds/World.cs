using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeldaOracle.Game.Worlds {
	public class World {

		private List<Level> levels;
		private int currentLevel;

		
		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public World() {
			levels = new List<Level>();
			currentLevel = 0;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------
		
		public Level CurrentLevel {
			get { return levels[currentLevel]; }
		}

		public List<Level> Levels {
			get { return levels; }
		}
	}
}
