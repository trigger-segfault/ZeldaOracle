using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeldaOracle.Game.Tiles.Custom.Monsters {
	public abstract class TileMonster : Tile {

		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>True if this monster is not counted towards clearing a room.</summary>
		public bool IgnoreMonster {
			get { return Properties.Get("ignore_monster", true); }
		}

		/// <summary>True if this monster needs to be killed in order to clear the room.</summary>
		public bool NeedsClearing {
			get { return !IgnoreMonster; }
		}
	}
}
