using ZeldaOracle.Game.Entities.Players;

namespace ZeldaOracle.Game.Control.RoomManagers {
	/// <summary>Base class for room management, contained by a RoomControl.</summary>
	public abstract class RoomManager {
		/// <summary>The room control containing this manager.</summary>
		private RoomControl roomControl;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Constructs the base room manager.</summary>
		public RoomManager(RoomControl roomControl) {
			this.roomControl = roomControl;
		}

		
		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------
		
		/// <summary>Get the reference to the room controller.</summary>
		public RoomControl RoomControl {
			get { return roomControl; }
		}

		/// <summary>Get the reference to the area controller.</summary>
		public AreaControl AreaControl {
			get { return roomControl.AreaControl; }
		}

		/// <summary>Get the reference to the game controller.</summary>
		public GameControl GameControl {
			get { return roomControl.GameControl; }
		}

		/// <summary>Get the reference to the player entity.</summary>
		public Player Player {
			get { return roomControl.Player; }
		}
	}
}
