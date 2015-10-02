using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Worlds;
using ZeldaOracle.Game.GameStates.Transitions;

namespace ZeldaOracle.Game.Tiles.EventTiles {

	public class NPCEvent : EventTile {

		public WarpType warpType;
		public bool warpEnabled;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public NPCEvent() {

		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		protected override void Initialize() {
			base.Initialize();

			// Spawn NPC entity.
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------
	}
}
