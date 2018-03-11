using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Entities.Monsters;

namespace ZeldaOracle.Game.Tiles.Custom.Monsters {
	public class TileMonsterFlyingTile : TileMonster {
		
		private int order;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public TileMonsterFlyingTile() {
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnInitialize() {
			base.OnInitialize();
			order = Properties.GetInteger("order", 0);
		}

		public override void Update() {
			base.Update();

			if (RoomControl.CurrentRoomTicks == StartTime) {
				SpawnMonster();
			}
		}


		//-----------------------------------------------------------------------------
		// Static Methods
		//-----------------------------------------------------------------------------

		/// <summary>Initializes the properties and events for the tile type.</summary>
		public new static void InitializeTileData(TileData data) {
			data.Properties.Set("order", 0)
				.SetDocumentation("Launch Order", "Flying Tile", "The order in which to launch each flying tile in the room.");
			data.EntityType = typeof(MonsterFlyingTile);
		}


		//-----------------------------------------------------------------------------
		// Override Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets the type of monster to spawn.</summary>
		/*public override Type MonsterType {
			get { return typeof(MonsterFlyingTile); }
		}*/

		public override bool IsStatic {
			get { return false; }
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public int StartTime {
			get {
				return GameSettings.MONSTER_FLYING_TILE_START_OFFSET +
					GameSettings.MONSTER_FLYING_TILE_NEXT_OFFSET * order;
			}
		}
	}
}
