using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Entities.Units;
using ZeldaOracle.Game.Tiles;

namespace ZeldaOracle.Game.Control {
	public class UnitSystem {
		
		private RoomControl roomControl;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public UnitSystem(RoomControl roomControl) {
			this.roomControl = roomControl;
		}


		//-----------------------------------------------------------------------------
		// Physics Update
		//-----------------------------------------------------------------------------

		public void Process() {
			/*
			// Create list of collisions.
			for (int i = 0; i < roomControl.Entities.Count; i++) {
				Unit unit1 = roomControl.Entities[i] as Unit;

				if (unit1 != null) {
					for (int j = 0; j < roomControl.Entities.Count; j++) {
						Unit unit2 = roomControl.Entities[j] as Unit;

						if (unit2 != null) {
							// TODO: 
						}
					}
				}
			}
			*/
		}

		
		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public RoomControl RoomControl {
			get { return roomControl; }
		}
		
		public GameControl GameControl {
			get { return roomControl.GameControl; }
		}
	}
}
