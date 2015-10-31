using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Control;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Entities.Monsters;
using ZeldaOracle.Game.Worlds;

namespace ZeldaOracle.Game.Tiles.EventTiles {

	public class MonsterEvent : EventTile {

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public MonsterEvent() {
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		protected override void Initialize() {
			base.Initialize();

			// Spawn the monster entity.
			Monster monster = new Monster();
			Vector2F center = position + new Vector2F(8, 8);
			monster.SetPositionByCenter(center);
			RoomControl.SpawnEntity(monster);
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------
	}
}
