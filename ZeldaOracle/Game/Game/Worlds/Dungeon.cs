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

	public class Dungeon : IPropertyObject {

		//private string id;
		private World world;
		private Properties properties;
		

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public Dungeon() {
			properties = new Properties();
			properties.BaseProperties = new Properties();
			properties.PropertyObject = this;

			properties.BaseProperties.Set("id",			"");
			properties.BaseProperties.Set("name",		"");
			properties.BaseProperties.Set("small_keys",	0);
			properties.BaseProperties.Set("boss_key",	false);
			properties.BaseProperties.Set("map",		false);
			properties.BaseProperties.Set("compass",	false);
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
		}


		//-----------------------------------------------------------------------------
		// Accessors
		//-----------------------------------------------------------------------------

		// Return a sorted list of all the floors in the dungeon.
		public DungeonFloor[] GetFloors() {
			int lowestFloorNumber = Int32.MaxValue;
			int highestFloorNumber = Int32.MinValue;
			bool foundAny = false;

			foreach (Level level in world.Levels) {
				if (level.Dungeon == this) {
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
				if (level.Dungeon == this)
					floors[level.DungeonFloor - lowestFloorNumber] = new DungeonFloor(level, level.DungeonFloor);
			}

			return floors;
			/*

			List<Level> floors = new List<Level>();
			foreach (Level level in world.Levels) {
				if (level.Dungeon == this)
					floors.Add(level);
			}
			floors.Sort(delegate(Level a, Level b) {
				int floorA = a.DungeonFloor;
				int floorB = b.DungeonFloor;

				if (floorA > floorB)
					return 1;
				else if (floorA < floorB)
					return -1;
				else 
					return 0;
			});

			if (floors.Count == 0)
				return new Level[0];

			int minFloorNumber = floors[0].DungeonFloor;
			int maxFloorNumber = floors[floors.Count - 1].DungeonFloor;
			int floorNumberCount = maxFloorNumber - minFloorNumber;
			Level[] floors2 = new Level[floorNumberCount];

			foreach (Level floor in floors) {
				int index = floor.DungeonFloor - minFloorNumber;
				floors2[index] = floor;
			}

			return floors.ToArray();
			*/
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
					if (level.DungeonFloor < lowest)
						lowest = level.DungeonFloor;
				}
				return lowest;
			}
		}
		
		public int HighestFloorNumber {
			get {
				int highest = Int32.MinValue;
				foreach (Level level in world.Levels) {
					if (level.DungeonFloor > highest)
						highest = level.DungeonFloor;
				}
				return highest;
			}
		}
	}
}
