using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Control.Scripting;

namespace ZeldaOracle.Game.Worlds {
	public class World : IPropertyObject, IIDObject {

		private List<Level> levels;
		private int startLevelIndex;
		private Point2I startRoomLocation;
		private Point2I startTileLocation;
		private Properties properties;
		private ScriptManager scriptManager;
		private List<Dungeon> dungeons;

		
		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public World() {
			this.levels = new List<Level>();
			this.scriptManager = new ScriptManager();

			this.properties = new Properties();
			this.properties.PropertyObject = this;
			this.properties.BaseProperties = new Properties();
			
			this.dungeons = new List<Dungeon>();

			this.properties.BaseProperties.Set("id", "world_name")
				.SetDocumentation("ID", "", "General", "The ID used to represent the world.", "The id used to refer to this world.", false, false);
		}


		//-----------------------------------------------------------------------------
		// Accessors
		//-----------------------------------------------------------------------------
		
		public bool ContainsLevel(string levelID) {
			return (GetLevel(levelID) != null);
		}

		public Level GetLevelAt(int index) {
			return levels[index];
		}

		public Level GetLevel(string levelID) {
			return levels.Find(level => { return level.Properties.GetString("id") == levelID; });
		}

		public int GetLevelIndex(string levelID) {
			return levels.FindIndex(level => { return level.Properties.GetString("id") == levelID; });
		}

		public IEnumerable<Level> GetLevels() {
			return levels;
		}

		public void MoveLevel(int oldIndex, int newIndex, bool relative) {
			if (relative)
				newIndex = oldIndex + newIndex;

			if (startLevelIndex == oldIndex)
				startLevelIndex = newIndex;
			else if (startLevelIndex > oldIndex && startLevelIndex <= newIndex && oldIndex < newIndex)
				startLevelIndex--;
			else if (startLevelIndex >= newIndex && startLevelIndex < oldIndex && oldIndex > newIndex)
				startLevelIndex++;
			
			Level level = levels[oldIndex];
			levels.RemoveAt(oldIndex);
			levels.Insert(newIndex, level);
		}

		public Script GetScript(string scriptID) {
			return ScriptManager.GetScript(scriptID);
		}
		
		public bool ContainsScript(string scriptID) {
			return scriptManager.ContainsScript(scriptID);
		}

		public bool ContainsDungeon(string dungeonID) {
			return (GetDungeon(dungeonID) != null);
		}

		public Dungeon GetDungeon(string dungeonID) {
			return dungeons.Find(dungeon => { return dungeon.Properties.GetString("id") == dungeonID; });
		}

		public Dungeon GetDungeonAt(int index) {
			return dungeons[index];
		}

		public IEnumerable<Dungeon> GetDungeons() {
			return dungeons;
		}

		public void MoveDungeon(int oldIndex, int newIndex, bool relative) {
			if (relative)
				newIndex = oldIndex + newIndex;

			Dungeon dungeon = dungeons[oldIndex];
			dungeons.RemoveAt(oldIndex);
			dungeons.Insert(newIndex, dungeon);
		}

		//-----------------------------------------------------------------------------
		// Mutators
		//-----------------------------------------------------------------------------

		public void AddLevel(Level level) {
			levels.Add(level);
			level.World = this;
		}
			
		public void RemoveLevel(Level level) {
			levels.Remove(level);
		}

		public void RemoveLevelAt(int index) {
			levels.RemoveAt(index);
		}

		public void RemoveLevel(string levelID) {
			for (int i = 0; i < levels.Count; i++) {
				if (levels[i].Properties.GetString("id") == levelID) {
					levels.RemoveAt(i);
					break;
				}
			}
		}

		public bool RenameLevel(Level level, string newLevelID) {
			if (level.ID != newLevelID) {
				if (ContainsLevel(newLevelID)) {
					return false;
				}
				level.ID = newLevelID;
			}
			return true;
		}


		public void AddDungeon(Dungeon dungeon) {
			dungeons.Add(dungeon);
			dungeon.World = this;
		}

		public void RemoveDungeon(Dungeon dungeon) {
			dungeons.Remove(dungeon);
		}

		public bool RenameDungeon(Dungeon dungeon, string newDungeonID) {
			if (dungeon.ID != newDungeonID) {
				if (ContainsDungeon(newDungeonID)) {
					return false;
				}
				dungeon.ID = newDungeonID;
			}
			return true;
		}

		public void AddScript(Script script) {
			scriptManager.AddScript(script);
		}

		public void RemoveScript(Script script) {
			scriptManager.RemoveScript(script);
		}

		public bool RenameScript(Script script, string newScriptID) {
			return scriptManager.RenameScript(script, newScriptID);
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------
		
		public List<Level> Levels {
			get { return levels; }
		}
		
		public Dictionary<string, Script> Scripts {
			get { return scriptManager.Scripts; }
		}
		
		public List<Dungeon> Dungeons {
			get { return dungeons; }
		}

		public Room StartRoom {
			get { return levels[startLevelIndex].GetRoomAt(startRoomLocation); }
		}

		public int StartLevelIndex {
			get { return startLevelIndex; }
			set { startLevelIndex = value; }
		}

		public Point2I StartRoomLocation {
			get { return startRoomLocation; }
			set { startRoomLocation = value; }
		}

		public Point2I StartTileLocation {
			get { return startTileLocation; }
			set { startTileLocation = value; }
		}

		public int LevelCount {
			get { return levels.Count; }
		}

		public int DungeonCount {
			get { return dungeons.Count; }
		}

		public Properties Properties {
			get { return properties; }
			set {
				properties = value;
				properties.PropertyObject = this;
			}
		}

		public string ID {
			get { return properties.GetString("id"); }
			set { properties.Set("id", value); }
		}

		public ScriptManager ScriptManager {
			get { return scriptManager; }
		}
	}
}
