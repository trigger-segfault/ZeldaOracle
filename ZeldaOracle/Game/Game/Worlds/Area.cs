using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.API;
using ZeldaOracle.Game.Entities;

namespace ZeldaOracle.Game.Worlds {
	public enum MapType {
		None,
		Overworld,
		Dungeon,
	}

	public class Area : IEventObject, IIDObject, IVariableObject {
		private World world;
		private Properties properties;
		private Variables variables;
		private EventCollection events;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public Area() {
			this.properties = new Properties(this);
			this.properties.BaseProperties = new Properties();
			this.variables	= new Variables(this);
			this.events		= new EventCollection(this);

			// General
			properties.BaseProperties.Set("id", "")
				.SetDocumentation("ID", "", "", "General", "The id used to refer to this dungeon.", false, false);
			properties.BaseProperties.Set("name", "")
				.SetDocumentation("Name", "", "", "General", "The readable name of this area in-game.");
			properties.BaseProperties.Set("music", "")
				.SetDocumentation("Music", "song", "", "General", "The default music to play in this area.", false, true);

			// Settings
			//properties.BaseProperties.Set("dungeon", false)
			//	.SetDocumentation("Is Dungeon", "", "", "Settings", "True if this area is treated as a dungeon.");
			properties.BaseProperties.Set("map_type", MapType.None.ToString())
				.SetDocumentation("Map Type", "enum", "MapType", "Settings", "The type of map to display for this area.");
			//properties.BaseProperties.Set("map_id",  "")
			//	.SetDocumentation("Map ID", "", "", "Settings", "The type of map to display for this area.");
			properties.BaseProperties.SetEnumStr("respawn_mode", RoomRespawnMode.Overworld)
				.SetDocumentation("Room Respawn Mode", "enum", typeof(RoomRespawnMode), "Settings", "The type of map to display for this area.");

			// Yes, the enum type and editor enum type are supposed to be different.
			properties.BaseProperties.SetEnumStr("spawn_mode", MonsterSpawnMode.Random)
				.SetDocumentation("Monster Spawn Mode", "enum", typeof(AreaSpawnMode), "Settings", "The method for spawning monsters in a room.");
			// Note area name message is only displayed on certain
			// warp points so that setting goes in WarpAction.

			// Puzzle
			properties.BaseProperties.SetEnumInt("color_switch_color", PuzzleColor.Blue)
				.SetDocumentation("Color Switch Color", "enum", typeof(PuzzleColor), "Puzzle", "");

			// Dungeon
			properties.BaseProperties.Set("dungeon_level", 0)
				.SetDocumentation("Dungeon Level", "", "", "Dungeon", "The level of the dungeon to display on the map. <1 means no level is displayed.");
			properties.BaseProperties.Set("small_keys", 0)
				.SetDocumentation("Small Keys Held", "", "", "Dungeon", "The number of held small keys for this dungeon.", false, false);
			properties.BaseProperties.Set("boss_key_obtained", false)
				.SetDocumentation("Boss Key Obtained", "", "", "Dungeon", "True if the boss key for this dungeon has been obtained.", false, false);
			properties.BaseProperties.Set("map_obtained", false)
				.SetDocumentation("Map Obtained", "", "", "Dungeon", "True if the map for this dungeon has been obtained.", false, false);
			properties.BaseProperties.Set("compass_obtained", false)
				.SetDocumentation("Compass Obtained", "", "", "Dungeon", "True if the compass for this dungeon has been obtained.", false, false);
			properties.BaseProperties.Set("completed", false)
				.SetDocumentation("Is Completed", "", "", "Dungeon", "True if the dungeon has been completed.", false, false);

			// Events
			events.AddEvent("area_start", "Area Start", "Transition", "Called when the area begins.");
			events.AddEvent("completed", "Completed", "Progress", "Called when the area has been completed.");
		}

		public Area(string id, string name) :
			this()
		{
			properties.Set("id", id);
			properties.Set("name", name);
		}

		public Area(Area copy) {
			properties	= new Properties(copy.properties, this);
			variables	= new Variables(copy.variables, this);
			events		= new EventCollection(copy.events, this);
		}


		//-----------------------------------------------------------------------------
		// Accessors
		//-----------------------------------------------------------------------------

		// Return a sorted list of all the floors in the dungeon.
		public DungeonFloor[] GetDungeonFloors() {
			int lowestFloorNumber = int.MaxValue;
			int highestFloorNumber = int.MinValue;
			bool foundAny = false;

			foreach (Level level in world.GetLevels()) {
				if (level.AreaID == ID) {
					foundAny = true;
					if (level.FloorNumber < lowestFloorNumber)
						lowestFloorNumber = level.FloorNumber;
					if (level.FloorNumber > highestFloorNumber)
						highestFloorNumber = level.FloorNumber;
				}
			}

			if (!foundAny)
				return new DungeonFloor[0];

			int floorNumberCount = (highestFloorNumber - lowestFloorNumber) + 1;
			DungeonFloor[] floors = new DungeonFloor[floorNumberCount];

			for (int i = 0; i < floorNumberCount; i++)
				floors[i] = new DungeonFloor(null, lowestFloorNumber + i);
			foreach (Level level in world.GetLevels()) {
				if (level.AreaID == ID)
					floors[level.FloorNumber - lowestFloorNumber] = new DungeonFloor(level, level.FloorNumber);
			}

			return floors;
		}

		/// <summary>Gets all levels with this area assigned to them.</summary>
		public IEnumerable<Level> GetLevels() {
			foreach (Level level in world.GetLevels()) {
				if (level.AreaID == ID)
					yield return level;
			}
		}

		/// <summary>Gets all rooms with this area assigned to them.</summary>
		public IEnumerable<Room> GetRooms() {
			foreach (Level level in GetLevels()) {
				foreach (Room room in level.GetRooms()) {
					//string areaID = room.AreaID;
					//if (string.IsNullOrWhiteSpace(areaID) || areaID == ID)
						yield return room;
				}
			}
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets or sets the world containing this area.</summary>
		public World World {
			get { return world; }
			set { world = value; }
		}

		/// <summary>Gets or sets the ID for the area.</summary>
		public string ID {
			get { return properties.GetString("id", ""); }
			set { properties.Set("id", value); }
		}

		/// <summary>Gets or sets the readable name for the area.</summary>
		public string Name {
			get { return properties.GetString("name", ""); }
			set { properties.Set("name", value); }
		}

		/// <summary>Gets or sets the properties for this area.</summary>
		public Properties Properties {
			get { return properties; }
			set {
				properties = value;
				properties.PropertyObject = this;
			}
		}

		/// <summary>Gets the events for this area.</summary>
		public EventCollection Events {
			get { return events; }
		}

		/// <summary>Gets the variables for this area.</summary>
		public Variables Vars {
			get { return variables; }
		}

		// Settings -------------------------------------------------------------------
		
		/// <summary>Gets or sets the method for monster respawning in this area.</summary>
		public RoomRespawnMode RespawnMode {
			get { return properties.GetEnum("respawn_mode", RoomRespawnMode.Overworld); }
			set { properties.SetEnum("respawn_mode", value); }
		}

		/// <summary>Gets or sets the type of map to use for this area.</summary>
		public MapType MapType {
			get { return properties.GetEnum("map_type", MapType.None); }
			set { properties.SetEnum("map_type", value); }
		}

		/// <summary>Gets or sets the spawn methods for monsters in the rooms in the
		/// area.</summary>
		public MonsterSpawnMode SpawnMode {
			get { return properties.GetEnum("spawn_mode", MonsterSpawnMode.Random); }
			set { properties.SetEnum("spawn_mode", value); }
		}
		
		/// <summary>Gets or sets the music for this area.</summary>
		public Song Music {
			get { return properties.GetResource<Song>("music", null); }
		}

		// Puzzle ---------------------------------------------------------------------

		/// <summary>Gets or sets the synced color switch color for this area.</summary>
		public PuzzleColor ColorSwitchColor {
			get { return properties.TryGetEnum("color_switch_color", PuzzleColor.Blue); }
			set { properties.SetEnum("color_switch_color", value); }
		}

		// Dungeon --------------------------------------------------------------------
		
		/// <summary>Gets or sets the level number of this dungeon. (Not zero-indexed).</summary>
		public int DungeonLevel {
			get { return properties.Get("dungeon_level", 0); }
			set { properties.Set("dungeon_level", value); }
		}

		/// <summary>Gets or sets the number of small keys currently held.</summary>
		public int SmallKeyCount {
			get { return properties.GetInteger("small_keys", 0); }
			set { properties.Set("small_keys", value); }
		}

		/// <summary>Gets or sets if the boss key has been obtained.</summary>
		public bool HasBossKey {
			get { return properties.GetBoolean("boss_key_obtained", false); }
			set { properties.Set("boss_key_obtained", value); }
		}

		/// <summary>Gets or sets if the dungeon map has been obtained.</summary>
		public bool HasMap {
			get { return properties.GetBoolean("map_obtained", false); }
			set { properties.Set("map_obtained", value); }
		}

		/// <summary>Gets or sets if the compass has been obtained.</summary>
		public bool HasCompass {
			get { return properties.GetBoolean("compass_obtained", false); }
			set { properties.Set("compass_obtained", value); }
		}

		/// <summary>Gets or sets if the area has been completed.</summary>
		public bool IsCompleted {
			get { return properties.GetBoolean("completed", false); }
			set { properties.Set("completed", value); }
		}

		/// <summary>Gets the lowest dungeon floor number.</summary>
		public int LowestFloorNumber {
			get {
				int lowest = Int32.MaxValue;
				foreach (Level level in world.GetLevels()) {
					if (level.AreaID == ID && level.FloorNumber < lowest)
						lowest = level.FloorNumber;
				}
				return lowest;
			}
		}

		/// <summary>Gets the highest dungeon floor number.</summary>
		public int HighestFloorNumber {
			get {
				int highest = Int32.MinValue;
				foreach (Level level in world.GetLevels()) {
					if (level.AreaID == ID && level.FloorNumber > highest)
						highest = level.FloorNumber;
				}
				return highest;
			}
		}
	}
}
