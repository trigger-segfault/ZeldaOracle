using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeldaOracle.Game.Tiles {
	public abstract class TileEntitySpawnOnce : Tile {


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public void SpawnEntity() {
			if (CanSpawn) {
				CanSpawn = false;
				RoomControl.RemoveTile(this);
				OnSpawnEntity();
			}
		}

		protected virtual void OnSpawnEntity() { }


		//-----------------------------------------------------------------------------
		// Virtual Properties
		//-----------------------------------------------------------------------------

		public virtual bool CanSpawn {
			get { return RoomControl.RoomNumber != Properties.GetInteger("last_room_number", -1); }
			private set { Properties.Set("last_room_number", RoomControl.RoomNumber); }
		}


		//-----------------------------------------------------------------------------
		// Override Properties
		//-----------------------------------------------------------------------------

		public override bool IsStatic {
			get { return false; }
		}
	}
}
