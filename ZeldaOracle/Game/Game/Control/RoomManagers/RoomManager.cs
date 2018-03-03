
namespace ZeldaOracle.Game.Control {

	/// <summary>Base class for room management, contained by a RoomControl.</summary>
	public abstract class RoomManager {
		
		private RoomControl roomControl;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

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
		
		/// <summary>Get the reference to the game controller.</summary>
		public GameControl GameControl {
			get { return roomControl.GameControl; }
		}
	}
}
