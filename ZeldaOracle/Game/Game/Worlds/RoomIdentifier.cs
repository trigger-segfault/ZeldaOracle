using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Game.Worlds {
	/// <summary>Identifies a room with its root level and location.</summary>
	public struct RoomIdentifier {
		/// <summary>The root level containing this room.</summary>
		public Level Level { get; set; }
		/// <summary>The location of the room in the level.</summary>
		public Point2I Location { get; set; }


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------
		
		/// <summery>Constructs the root room identifier with a room.</summery>
		public RoomIdentifier(Room room) {
			Room rootRoom = room.RootRoom;
			Level = rootRoom.Level;
			Location = rootRoom.Location;
		}


		//-----------------------------------------------------------------------------
		// Accessors
		//-----------------------------------------------------------------------------

		/// <summary>Gets the room of the root level.</summary>
		public Room GetRoom() {
			return Level.GetRoomAt(Location);
		}

		/// <summary>Gets the room of the specified level.
		/// Returns null if the level does not have the same root level.</summary>
		/*public Room GetRoom(Level level) {
			if (level.RootLevel == Level)
				return level.GetRoomAt(Location);
			return null;
		}*/


		//-----------------------------------------------------------------------------
		// General
		//-----------------------------------------------------------------------------

		/// <summary>Returns a string representation of the room identifier.</summary>
		public override string ToString() {
			return Level.ID + "[" + Location.X + ", " + Location.Y + "]";
		}

		/// <summary>Gets the hash code for the room identifier.</summary>
		public override int GetHashCode() {
			return Level.GetHashCode() ^ Location.GetHashCode();
		}

		/// <summary>Returns true if the room identifier is equal to the room.</summary>
		public override bool Equals(object obj) {
			if (obj is RoomIdentifier)
				return this == ((RoomIdentifier) obj);
			else if (obj is Room)
				return this == ((Room) obj);
			return false;
		}


		//-----------------------------------------------------------------------------
		// Operators
		//-----------------------------------------------------------------------------

		/// <summary>Returns true if the room identifiers are equal.</summary>
		public static bool operator ==(RoomIdentifier a, RoomIdentifier b) {
			return (a.Level == b.Level && a.Location == b.Location);
		}

		/// <summary>Returns true if the room identifiers are not equal.</summary>
		public static bool operator !=(RoomIdentifier a, RoomIdentifier b) {
			return (a.Level != b.Level || a.Location != b.Location);
		}


		//-----------------------------------------------------------------------------
		// Casting
		//-----------------------------------------------------------------------------

		/// <summary>Implicitly casts a room to a room identifier.</summary>
		public static implicit operator RoomIdentifier(Room room) {
			return new RoomIdentifier(room);
		}
	}
}
