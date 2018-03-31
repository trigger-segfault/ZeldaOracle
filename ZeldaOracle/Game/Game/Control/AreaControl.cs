using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Translation;
using ZeldaOracle.Game.API;
using ZeldaOracle.Game.Control.RoomManagers;
using ZeldaOracle.Game.Worlds;

namespace ZeldaOracle.Game.Control {
	public class AreaControl : ZeldaAPI.Area, IVariableObject {
		/// <summary>The game control running the game.</summary>
		private GameControl gameControl;
		/// <summary>The current area.</summary>
		private Area area;
		/// <summary>The manager for clearing and respawning rooms.</summary>
		private RespawnManager respawnManager;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		/// <summary>Constructs the area control.</summary>
		public AreaControl(GameControl gameControl, Area area) {
			this.gameControl = gameControl;
			this.area = area;
			this.respawnManager = new RespawnManager(this);
		}


		//-----------------------------------------------------------------------------
		// Management
		//-----------------------------------------------------------------------------

		/// <summary>Called when the area is entered during room begin.</summary>
		public void BeginArea() {
			GameControl.FireEvent(area, "area_start");
		}

		/// <summary>Called when the area is left during room end.</summary>
		public void EndArea() {
			foreach (Room room in area.GetRooms()) {
				room.OnLeaveArea();
			}
		}

		/// <summary>Completes the area.</summary>
		public void Complete() {
			if (IsCompleted)
				return;

			area.IsCompleted = true;
			GameControl.FireEvent(area, "completed");
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets the game control running the game.</summary>
		public GameControl GameControl {
			get { return gameControl; }
		}

		/// <summary>Gets the room control for the current room.</summary>
		public RoomControl RoomControl {
			get { return gameControl.RoomControl; }
		}

		/// <summary>Gets the manager for clearing and respawning rooms.</summary>
		public RespawnManager RespawnManager {
			get { return respawnManager; }
		}

		/// <summary>Gets the current room.</summary>
		public Room Room {
			get { return gameControl.RoomControl.Room; }
		}

		/// <summary>Gets the current area.</summary>
		public Area Area {
			get { return area; }
		}

		/// <summary>Gets the readable name of the area.</summary>
		public string Name {
			get { return area.Name; }
		}

		/// <summary>Gets the message displayed when entering the area.</summary>
		public string EnterMessage {
			get {
				string str = "";
				if (area.DungeonLevel != -1)
					str += "Level " + area.DungeonLevel + "\n";
				// Center-align area names
				str += FormatCodes.AlignCenterCharacter;
				str += area.Name;
				return str;
			}
		}

		/// <summary>Gets if the area has been completed.</summary>
		public bool IsCompleted {
			get { return area.IsCompleted; }
		}

		/// <summary>Gets the respawn mode for the current area.</summary>
		public RoomRespawnMode RespawnMode {
			get { return area.RespawnMode; }
		}

		/// <summary>Gets the variables for the area.</summary>
		public Variables Vars {
			get { return area.Vars; }
		}
	}
}
