using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Tiles.EventTiles;

namespace ZeldaOracle.Game.Worlds {

	public class DungeonFloor {

		private Level level;
		private int floorNumber;

		public DungeonFloor(Level level, int floorNumber) {
			this.level = level;
			this.floorNumber = floorNumber;
		}
		
		public Level Level {
			get { return level; }
		}

		public int FloorNumber {
			get { return floorNumber; }
		}
		
		public bool IsBossFloor {
			get { return false; }
		}
		
		public bool IsDiscovered {
			get {
				if (level != null)
					return level.IsDiscovered;
				return false;
			}
		}
	}

	public class Dungeon : IEventObject, IIDObject {
		private World world;
		private Properties properties;
		private EventCollection events;

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public Dungeon() {
			this.events		= new EventCollection(this);
			this.properties	= new Properties(this);
			this.properties.BaseProperties	= new Properties();

			properties.BaseProperties.Set("id",			"")
				.SetDocumentation("ID", "", "", "General", "The id used to refer to this dungeon.", false, false);
			properties.BaseProperties.Set("name",		"")
				.SetDocumentation("Name", "", "", "General", "The name of this dungeon in-game.");
			properties.BaseProperties.Set("small_keys",	0)
				.SetDocumentation("Small Keys Held", "", "", "Progress", "The number of held small keys for this dungeon.");
			properties.BaseProperties.Set("boss_key",	false)
				.SetDocumentation("Boss Key Obtained", "", "", "Progress", "True if the boss key for this dungeon has been obtained.");
			properties.BaseProperties.Set("map",		false)
				.SetDocumentation("Map Obtained", "", "", "Progress", "True if the map for this dungeon has been obtained.");
			properties.BaseProperties.Set("compass",	false)
				.SetDocumentation("Compass Obtained", "", "", "Progress", "True if the compass for this dungeon has been obtained.");
			properties.BaseProperties.Set("color_switch_color", (int) PuzzleColor.Blue)
				.SetDocumentation("Color Switch Color", "enum", "PuzzleColor", "Puzzle", "");
		}

		public Dungeon(string id, string name) : 
			this()
		{
			properties.Set("id", id);
			properties.Set("name", name);
		}

		public Dungeon(Dungeon copy) : 
			this()
		{
			properties.SetAll(copy.properties);
			events.SetAll(copy.events);
		}


		//-----------------------------------------------------------------------------
		// Accessors
		//-----------------------------------------------------------------------------

		// Return a sorted list of all the floors in the dungeon.
		public DungeonFloor[] GetFloors() {
			int lowestFloorNumber = int.MaxValue;
			int highestFloorNumber = int.MinValue;
			bool foundAny = false;

			foreach (Level level in world.Levels) {
				if (level.DungeonID == ID) {
					foundAny = true;
					if (level.DungeonFloor < lowestFloorNumber)
						lowestFloorNumber = level.DungeonFloor;
					if (level.DungeonFloor > highestFloorNumber)
						highestFloorNumber = level.DungeonFloor;
				}
			}

			if (!foundAny)
				return new DungeonFloor[0];

			int floorNumberCount = (highestFloorNumber - lowestFloorNumber) + 1;
			DungeonFloor[] floors = new DungeonFloor[floorNumberCount];

			for (int i = 0; i < floorNumberCount; i++)
				floors[i] = new DungeonFloor(null, lowestFloorNumber + i);
			foreach (Level level in world.Levels) {
				if (level.DungeonID == ID)
					floors[level.DungeonFloor - lowestFloorNumber] = new DungeonFloor(level, level.DungeonFloor);
			}

			return floors;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public World World {
			get { return world; }
			set { world = value; }
		}

		public int NumSmallKeys {
			get { return properties.GetInteger("small_keys", 0); }
			set { properties.Set("small_keys", value); }
		}
		
		public bool HasBossKey {
			get { return properties.GetBoolean("boss_key", false); }
			set { properties.Set("boss_key", value); }
		}
		
		public bool HasMap {
			get { return properties.GetBoolean("map", false); }
			set { properties.Set("map", value); }
		}
		
		public bool HasCompass {
			get { return properties.GetBoolean("compass", false); }
			set { properties.Set("compass", value); }
		}
		
		public string ID {
			get { return properties.GetString("id", ""); }
			 set { properties.Set("id", value); }
		}
		
		public string Name {
			get { return properties.GetString("name", ""); }
			set { properties.Set("name", value); }
		}

		public Properties Properties {
			get { return properties; }
			set {
				properties = value;
				properties.PropertyObject = this;
			}
		}

		public int LowestFloorNumber {
			get {
				int lowest = Int32.MaxValue;
				foreach (Level level in world.Levels) {
					if (level.DungeonID == ID && level.DungeonFloor < lowest)
						lowest = level.DungeonFloor;
				}
				return lowest;
			}
		}
		
		public int HighestFloorNumber {
			get {
				int highest = Int32.MinValue;
				foreach (Level level in world.Levels) {
					if (level.DungeonID == ID && level.DungeonFloor > highest)
						highest = level.DungeonFloor;
				}
				return highest;
			}
		}

		public PuzzleColor ColorSwitchColor {
			get { return (PuzzleColor) properties.GetInteger("color_switch_color", (int) PuzzleColor.Blue); }
			set { properties.Set("color_switch_color", (int) value); }
		}

		public EventCollection Events {
			get { return events; }
		}
	}
}
