using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace ZeldaAPI {
	
	// Custom attributes to give information to the script editor.
	namespace Attributes {
		/**<summary>A description attribute class for use with the legacy WinForms editor.</summary>*/
		public class Description : Attribute {
			/**<summary>Constructs the description attribute.</summary>*/
			public Description(string description) {
				this.Text = description;
			}
			/**<summary>Gets the text of the description attribute.</summary>*/
			public string Text { get; set; }
		}
	}

	/**<summary>The base class that scripts are defined in.</summary>*/
	public class CustomScriptBase {
		/**<summary>Access to the current room.</summary>*/
		public Room room;
		/**<summary>Access to the game.</summary>*/
		public Game game;
	}

	/**<summary>Access to the game.</summary>*/
	public interface Game {
		// Nothing yet...
    }

	/**<summary>Access to a single room within a level.</summary>*/
	public interface Room {
		/**<summary>Open all doors in the room.</summary>
		 * <param name="instantaneous">If true, the door animation does not play.</param>
		 * <param name="rememberState">True if the doors stays in its new state.</param>*/
		[Attributes.Description("Open all doors in the room.")]
		void OpenAllDoors(bool instantaneous = false, bool rememberState = false);

		/**<summary>Close all doors in the room.</summary>
		 * <param name="instantaneous">If true, the door animation does not play.</param>
		 * <param name="rememberState">True if the doors stays in its new state.</param>*/
		[Attributes.Description("Close all doors in the room.")]
		void CloseAllDoors(bool instantaneous = false, bool rememberState = false);

		/**<summary>Set the new state for all doors.</summary>
		 * <param name="state">The new state of the doors.</param>
		 * <param name="rememberState">True if the doors stays in its new state.</param>*/
		[Attributes.Description("Set the opened state for all doors.")]
		void SetDoorStates(DoorState state, bool rememberState = false);

		/**<summary>Get an enumerable list of all the room's tiles of the given type.</summary>
		 * <typeparam name="T">The type of the tiles to look for</typeparam>*/
		[Attributes.Description("Get an enumerable list of all the room's tiles of the given type.")]
		IEnumerable<T> GetTilesOfType<T>() where T : class;
		
		/**<summary>Gets the first tile with the given ID in the room.</summary>
		 * <param name="id">The id of the tile to look for.</param>*/
		Tile GetTileById(string id);
		
		/**<summary>Spawn a tile by ID if it has not been spawned already.</summary>
		 * <param name="id">The ID of the tile in the room.</param>
		 * <param name="staySpawned">True if the tile should stay spawned even after re-entering the room.</param>*/
		[Attributes.Description("Spawn a tile by ID if it has not been spawned already.")]
		void SpawnTile(string id, bool staySpawned = false);

		/**<summary>Get an enumerable list of all the room's entities of the given type.</summary>
		 * <typeparam name="T">The type of the entities to look for</typeparam>*/
		[Attributes.Description("Get an enumerable list of all the room's entities of the given type.")]
		IEnumerable<T> GetEntitiesOfType<T>() where T : class;

		/**<summary>Returns true if there are no alive monsters in the room.</summary>*/
		[Attributes.Description("Returns true if there are no alive monsters in the room.")]
		bool AllMonstersDead();

		//TileData GetTileData(string id);
		
    }

	/**<summary>Access to a tile's tile data.</summary>*/
	public interface TileData {
		/**<summary>Spawns the tile.</summary>*/
		void Spawn();
		/**<summary>Enables the tile.</summary>*/
		void Enable();
		/**<summary>Disables the tile.</summary>*/
		void Disable();
		/**<summary>Spawns the tile and enables it.</summary>*/
		void SpawnAndEnable();
		/**<summary>Returns true if the tile is enabled.</summary>*/
		bool IsEnabled { get; }
	}
}


