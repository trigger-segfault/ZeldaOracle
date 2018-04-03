using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using ZeldaOracle.Game;

namespace ZeldaAPI {
	
	public interface AreaTODO : ApiObject {
	}
	
	public interface RoomTODO : ApiObject {
	}

	public interface World : ApiObject {
	}

	public interface Level : ApiObject {
	}

	/// <summary>Access to a single room within a level.</summary>
	public interface Room : ApiObject {
		/// <summary>Open all doors in the room.</summary>
		/// <param name="instantaneous">If true, the door animation does not play.</param>
		/// <param name="rememberState">True if the doors stays in its new state.</param>
		void OpenAllDoors(bool instantaneous = false, bool rememberState = false);

		/// <summary>Close all doors in the room.</summary>
		/// <param name="instantaneous">If true, the door animation does not play.</param>
		/// <param name="rememberState">True if the doors stays in its new state.</param>
		void CloseAllDoors(bool instantaneous = false, bool rememberState = false);

		/// <summary>Set the new state for all doors.</summary>
		/// <param name="state">The new state of the doors.</param>
		/// <param name="rememberState">True if the doors stays in its new state.</param>
		void SetDoorStates(DoorState state, bool rememberState = false);

		/// <summary>Get an enumerable list of all the room's tiles of the given type.</summary>
		/// <typeparam name="T">The type of the tiles to look for</typeparam>
		IEnumerable<T> GetTilesOfType<T>() where T : class;
		
		/// <summary>Gets the first tile with the given ID in the room.</summary>
		/// <param name="id">The id of the tile to look for.</param>
		Tile GetTileById(string id);

		/// <summary>Spawn a tile by ID if it has not been spawned already.</summary>
		/// <param name="id">The ID of the tile in the room.</param>
		/// <param name="staySpawned">True if the tile should stay spawned even after
		/// re-entering the room.</param>
		void SpawnTile(string id, bool staySpawned = false);

		/// <summary>Despawns a tile by ID if it's already spawned.</summary>
		/// <param name="id">The ID of the tile in the room.</param>
		/// <param name="stayDespawned">True if the tile should stay despawned even after
		/// re-entering the room.</param>
		void DespawnTile(string id, bool stayDespawned = false);

		/// <summary>Get an enumerable list of all the room's entities of the
		/// given type.</summary>
		/// <typeparam name="T">The type of the entities to look for</typeparam>
		IEnumerable<T> GetEntitiesOfType<T>() where T : class;

		/// <summary>Returns true if there are no alive monsters in the room.</summary>
		bool AllMonstersDead();

		//TileData GetTileData(string id);
	}

	/// <summary>Access to a tile's tile data.</summary>
	public interface TileData {
		/// <summary>Spawns the tile.</summary>
		void Spawn();
		/// <summary>Enables the tile.</summary>
		void Enable();
		/// <summary>Disables the tile.</summary>
		void Disable();
		/// <summary>Spawns the tile and enables it.</summary>
		void SpawnAndEnable();
		/// <summary>Returns true if the tile is enabled.</summary>
		bool IsEnabled { get; }
	}
}


