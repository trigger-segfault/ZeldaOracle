using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeldaOracle.Game.Control.RoomManagers {
	/// <summary>Base class for area management, contained by a AreaControl.</summary>
	public abstract class AreaManager {
		/// <summary>The area control containing this manager.</summary>
		private AreaControl areaControl;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Constructs the base area manager.</summary>
		public AreaManager(AreaControl areaControl) {
			this.areaControl = areaControl;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Get the reference to the area controller.</summary>
		public AreaControl AreaControl {
			get { return areaControl; }
		}

		/// <summary>Get the reference to the game controller.</summary>
		public GameControl GameControl {
			get { return areaControl.GameControl; }
		}

		/// <summary>Get the reference to the room controller.</summary>
		public RoomControl RoomControl {
			get { return areaControl.RoomControl; }
		}
	}
}
