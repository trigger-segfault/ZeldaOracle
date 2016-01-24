using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace ZeldaAPI {
	/*
	public interface ICustomScript {
		void RunScript(ZeldaAPI.Room Room);
	}*/

	namespace Attributes {

		public class Description : Attribute {
			public Description(string description) {
				this.Text = description;
			}
			public string Text { get; set; }
		}
	}

	public class CustomScriptBase {
		// Common variables, accessible to all scripts:
		// These will be set before calling RunScript.
		public Room room;
		public Game game;
	}

    public interface Room {
		[Attributes.Description("Open all doors in the room.")]
		void OpenAllDoors(bool instantaneous = false, bool rememberState = false);
		
		[Attributes.Description("Close all doors in the room.")]
		void CloseAllDoors(bool instantaneous = false, bool rememberState = false);
		
		[Attributes.Description("Set the opened state for all doors.")]
		void SetDoorStates(DoorState state, bool rememberState = false);
		
		[Attributes.Description("Get an enumerable list of all the room's tiles of the given type.")]
		IEnumerable<T> GetTilesOfType<T>() where T : class;


		Tile GetTileById(string id);


		[Attributes.Description("Spawn a tile by ID if it has not been spawned already.")]
		void SpawnTile(string id, bool staySpawned = false);
		
		[Attributes.Description("Get an enumerable list of all the room's entities of the given type.")]
		IEnumerable<T> GetEntitiesOfType<T>() where T : class;
		
		[Attributes.Description("Returns true if there are no alive monsters in the room.")]
		bool AllMonstersDead();

		//TileData GetTileData(string id);
		
    }

	public interface TileData {
		void Spawn();
		void Enable();
		void Disable();
		void SpawnAndEnable();
		bool IsEnabled { get; }
	}
	
    public interface Game {
    }

	public class TestClass {
		public TestClass() {
		}

		public void DoSomething() {
		}
	}
}

//namespace ZeldaAPI.CustomScripts{public class CustomScript : ICustomScript{public void RunScript(Room Room){Room.OpenAllDoors();}}}
