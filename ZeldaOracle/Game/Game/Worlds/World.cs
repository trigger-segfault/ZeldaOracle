using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Control.Scripting;

namespace ZeldaOracle.Game.Worlds {
	public class World : IPropertyObject {

		private List<Level> levels;
		private int startLevelIndex;
		private Point2I startRoomLocation;
		private Point2I startTileLocation;
		private Properties properties;
		private ScriptManager scriptManager;
		private Dictionary<string, Dungeon> dungeons;

		
		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public World() {
			this.levels = new List<Level>();
			this.scriptManager = new ScriptManager();

			this.properties = new Properties();
			this.properties.PropertyObject = this;
			this.properties.BaseProperties = new Properties();
			
			this.dungeons = new Dictionary<string, Dungeon>();

			this.properties.BaseProperties.Set("id", "world_name")
				.SetDocumentation("ID", "", "", "", "The id used to refer to this world.", false, true);
		}


		//-----------------------------------------------------------------------------
		// Accessors
		//-----------------------------------------------------------------------------
		
		public bool ExistsLevel(string levelID) {
			return (GetLevel(levelID) != null);
		}

		public Level GetLevelAt(int index) {
			return levels[index];
		}

		public Level GetLevel(string levelID) {
			for (int i = 0; i < levels.Count; i++) {
				if (levels[i].Properties.GetString("id") == levelID)
					return levels[i];
			}
			return null;
		}

		public int GetLevelIndex(string levelID) {
			for (int i = 0; i < levels.Count; i++) {
				if (levels[i].Properties.GetString("id") == levelID)
					return i;
			}
			return -1;
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

		public Dungeon GetDungoen(string dungeonID) {
			if (dungeons.ContainsKey(dungeonID))
				return dungeons[dungeonID];
			return null;
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

		public void AddScript(Script script) {
			scriptManager.AddScript(script);
		}

		public void RemoveScript(Script script) {
			scriptManager.RemoveScript(script);
		}

		public void AddDungeon(Dungeon dungeon) {
			dungeons.Add(dungeon.ID, dungeon);
			dungeon.World = this;
		}

		public void RemoveDungeon(Dungeon dungeon) {
			dungeons.Remove(dungeon.ID);
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
		
		public Dictionary<string, Dungeon> Dungeons {
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

		public Properties Properties {
			get { return properties; }
			set {
				properties = value;
				properties.PropertyObject = this;
			}
		}

		public string Id {
			get { return properties.GetString("id"); }
			set { properties.Set("id", value); }
		}

		public ScriptManager ScriptManager {
			get { return scriptManager; }
		}
	}
}
