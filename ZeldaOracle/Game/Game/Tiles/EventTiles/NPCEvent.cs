using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Worlds;
using ZeldaOracle.Game.Entities;

namespace ZeldaOracle.Game.Tiles.EventTiles {

	public class NPCEvent : EventTile {

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
			NPC npc = new NPC();
			RoomControl.SpawnEntity(npc, position - npc.Graphics.DrawOffset);
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------
	}
}
