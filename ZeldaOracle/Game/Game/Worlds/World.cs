using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Control.Scripting;

namespace ZeldaOracle.Game.Worlds {
	/// <summary>The world class containing everything about the game.</summary>
	public class World : IEventObjectContainer, IEventObject, IIDObject {

		private Properties properties;
		private EventCollection events;
		private List<Level> levels;
		private List<Dungeon> dungeons;
		private ScriptManager scriptManager;
		private int startLevelIndex;
		private Point2I startRoomLocation;
		private Point2I startTileLocation;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		/// <summary>Constructs an empty world.</summary>
		public World() {
			this.dungeons		= new List<Dungeon>();
			this.levels			= new List<Level>();
			this.scriptManager	= new ScriptManager();

			this.events			= new EventCollection(this);
			this.properties		= new Properties(this);
			this.properties.BaseProperties = new Properties();


			this.properties.BaseProperties.Set("id", "world_name")
				.SetDocumentation("ID", "", "", "General", "The ID used for saves to identify the world.", true, true);

			this.events.AddEvent("start_game", "Start Game", "Initialization",
				"Called when the game first starts.", new ScriptParameter("Game", "game"));
		}


		//-----------------------------------------------------------------------------
		// Accessors
		//-----------------------------------------------------------------------------

		// Levels ---------------------------------------------------------------------

		/// <summary>Gets the collection of levels.</summary>
		public IEnumerable<Level> GetLevels() {
			return levels;
		}

		/// <summary>Gets the level with the specified ID.</summary>
		public Level GetLevel(string levelID) {
			return levels.Find(level => level.ID == levelID);
		}

		/// <summary>Gets the level at the specified index.</summary>
		public Level GetLevelAt(int index) {
			return levels[index];
		}

		/// <summary>Returns true if the level exists in the collection.</summary>
		public bool ContainsLevel(Level level) {
			return levels.Contains(level);
		}

		/// <summary>Returns true if a level with the specified ID exists.</summary>
		public bool ContainsLevel(string levelID) {
			return (GetLevel(levelID) != null);
		}

		/// <summary>Gets the index of the specified level.</summary>
		public int IndexOfLevel(Level level) {
			return levels.IndexOf(level);
		}

		/// <summary>Gets the index of the level with the specified ID.</summary>
		public int IndexOfLevel(string levelID) {
			return levels.FindIndex(level => level.ID == levelID);
		}

		// Dungeons -------------------------------------------------------------------

		/// <summary>Gets the collection of dungeons.</summary>
		public IEnumerable<Dungeon> GetDungeons() {
			return dungeons;
		}

		/// <summary>Gets the dungeon with the specified ID.</summary>
		public Dungeon GetDungeon(string dungeonID) {
			return dungeons.Find(dungeon => { return dungeon.Properties.GetString("id") == dungeonID; });
		}

		/// <summary>Gets the dungeon at the specified index.</summary>
		public Dungeon GetDungeonAt(int index) {
			return dungeons[index];
		}

		/// <summary>Returns true if the dungeon exists in the collection.</summary>
		public bool ContainsDungeon(Dungeon dungeon) {
			return dungeons.Contains(dungeon);
		}

		/// <summary>Returns true if a dungeon with the specified ID exists.</summary>
		public bool ContainsDungeon(string dungeonID) {
			return (GetDungeon(dungeonID) != null);
		}

		/// <summary>Gets the index of the specified dungeon.</summary>
		public int IndexOfDungeon(Dungeon dungeon) {
			return dungeons.IndexOf(dungeon);
		}

		/// <summary>Gets the index of the dungeon with the specified ID.</summary>
		public int IndexOfDungeon(string dungeonID) {
			return dungeons.FindIndex(dungeon => dungeon.ID == dungeonID);
		}

		// Scripts --------------------------------------------------------------------

		/// <summary>Gets the script with the specified ID.</summary>
		public Script GetScript(string scriptID) {
			return ScriptManager.GetScript(scriptID);
		}

		/// <summary>Returns true if the script exists in the collection.</summary>
		public bool ContainsScript(Script script) {
			return ScriptManager.ContainsScript(script);
		}

		/// <summary>Returns true if a script with the specified ID exists.</summary>
		public bool ContainsScript(string scriptID) {
			return scriptManager.ContainsScript(scriptID);
		}

		// Objects --------------------------------------------------------------------

		/// <summary>Gets the collection of property objects in the world.</summary>
		public IEnumerable<IPropertyObject> GetPropertyObjects() {
			yield return this;
			foreach (Level level in levels) {
				foreach (IPropertyObject propertyObject in level.GetPropertyObjects()) {
					yield return propertyObject;
				}
			}
			foreach (Dungeon dungeon in dungeons) {
				yield return dungeon;
			}
		}

		/// <summary>Gets the collection of event objects in the world.</summary>
		public IEnumerable<IEventObject> GetEventObjects() {
			yield return this;
			foreach (Level level in levels) {
				foreach (IEventObject eventObject in level.GetEventObjects()) {
					yield return eventObject;
				}
			}
			foreach (Dungeon dungeon in dungeons) {
				yield return dungeon;
			}
		}

		/// <summary>Gets the collection of defined eventsin the world.</summary>
		public IEnumerable<Event> GetDefinedEvents() {
			foreach (IEventObject eventObject in GetEventObjects()) {
				foreach (Event evnt in eventObject.Events.GetEvents()) {
					if (evnt.IsDefined) {
						yield return evnt;
					}
				}
			}
		}


		//-----------------------------------------------------------------------------
		// Mutators
		//-----------------------------------------------------------------------------

		// Levels ---------------------------------------------------------------------

		/// <summary>Adds the level to the end of the list.</summary>
		public void AddLevel(Level level) {
			levels.Add(level);
			level.World = this;
		}

		/// <summary>Inserts the level at the specified index.</summary>
		public void InsertLevel(int index, Level level) {
			levels.Insert(index, level);
			level.World = this;
		}

		/// <summary>Removes the specified dungeon.</summary>
		public void RemoveLevel(Level level) {
			levels.Remove(level);
		}

		/// <summary>Removes the level with the specified ID.</summary>
		public void RemoveLevel(string levelID) {
			int index = levels.FindIndex(level => level.ID == levelID);
			if (index != -1)
				levels.RemoveAt(index);
		}

		/// <summary>Removes the level at the specified index.</summary>
		public void RemoveLevelAt(int index) {
			levels.RemoveAt(index);
		}

		/// <summary>Renames the specified dungeon.</summary>
		public bool RenameLevel(Level level, string newLevelID) {
			if (level.ID != newLevelID) {
				if (ContainsLevel(newLevelID)) {
					return false;
				}
				level.ID = newLevelID;
			}
			return true;
		}

		/// <summary>Renames the dungeon with the specified ID.</summary>
		public bool RenameLevel(string oldLevelID, string newLevelID) {
			return RenameLevel(GetLevel(oldLevelID), newLevelID);
		}

		/// <summary>Renames the dungeon at the specified index.</summary>
		public bool RenameLevelAt(int index, string newLevelID) {
			return RenameLevel(levels[index], newLevelID);
		}

		/// <summary>Moves the level at the specified index to the new index.</summary>
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

		// Dungeons -------------------------------------------------------------------

		/// <summary>Adds the dungeon to the end of the list.</summary>
		public void AddDungeon(Dungeon dungeon) {
			dungeons.Add(dungeon);
			dungeon.World = this;
		}

		/// <summary>Inserts the dungeon at the specified index.</summary>
		public void InsertDungeon(int index, Dungeon dungeon) {
			dungeons.Insert(index, dungeon);
			dungeon.World = this;
		}

		/// <summary>Removes the specified dungeon.</summary>
		public void RemoveDungeon(Dungeon dungeon) {
			dungeons.Remove(dungeon);
		}

		/// <summary>Removes the dungeon with the specified ID.</summary>
		public void RemoveDungeon(string dungeonID) {
			int index = dungeons.FindIndex(dungeon => dungeon.ID == dungeonID);
			if (index != -1)
				dungeons.RemoveAt(index);
		}

		/// <summary>Renames the dungeon at the specified index.</summary>
		public void RemoveDungeonAt(int index) {
			dungeons.RemoveAt(index);
		}

		/// <summary>Renames the specified dungeon.</summary>
		public bool RenameDungeon(Dungeon dungeon, string newDungeonID) {
			if (dungeon.ID != newDungeonID) {
				if (ContainsDungeon(newDungeonID)) {
					return false;
				}
				dungeon.ID = newDungeonID;
			}
			return true;
		}

		/// <summary>Renames the dungeon with the specified ID.</summary>
		public bool RenameDungeon(string oldDungeonID, string newDungeonID) {
			return RenameDungeon(GetDungeon(oldDungeonID), newDungeonID);
		}

		/// <summary>Renames the dungeon at the specified index.</summary>
		public bool RenameDungeonAt(int index, string newDungeonID) {
			return RenameDungeon(dungeons[index], newDungeonID);
		}

		/// <summary>Moves the dungeon at the specified index to the new index.</summary>
		public void MoveDungeon(int oldIndex, int newIndex, bool relative) {
			if (relative)
				newIndex = oldIndex + newIndex;

			Dungeon dungeon = dungeons[oldIndex];
			dungeons.RemoveAt(oldIndex);
			dungeons.Insert(newIndex, dungeon);
		}

		// Scripts --------------------------------------------------------------------

		/// <summary>Adds the script from the collection.</summary>
		public void AddScript(Script script) {
			scriptManager.AddScript(script);
		}

		/// <summary>Removes the script from the collection.</summary>
		public void RemoveScript(Script script) {
			scriptManager.RemoveScript(script);
		}

		/// <summary>Removes the script with the specified id from the collection.</summary>
		public void RemoveScript(string scriptID) {
			scriptManager.RemoveScript(scriptID);
		}

		/// <summary>Renames the specified script.</summary>
		public bool RenameScript(Script script, string newScriptID) {
			return scriptManager.RenameScript(script, newScriptID);
		}

		/// <summary>Renames the script with the specified ID.</summary>
		public bool RenameScript(string oldScriptID, string newScriptID) {
			return scriptManager.RenameScript(oldScriptID, newScriptID);
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets the save identifier for the world.</summary>
		public string ID {
			get { return properties.GetString("id"); }
			set { properties.Set("id", value); }
		}

		/// <summary>Gets the properties for the world.</summary>
		public Properties Properties {
			get { return properties; }
			set {
				properties = value;
				properties.PropertyObject = this;
			}
		}

		/// <summary>Gets the events for the world.</summary>
		public EventCollection Events {
			get { return events; }
		}

		/// <summary>Gets the script manager for the world.</summary>
		public ScriptManager ScriptManager {
			get { return scriptManager; }
		}

		/// <summary>Gets the collection of levels for the world.</summary>
		public List<Level> Levels {
			get { return levels; }
		}

		/// <summary>Gets the collection of dungeons for the world.</summary>
		public List<Dungeon> Dungeons {
			get { return dungeons; }
		}

		/// <summary>Gets the collection of scripts for the world.</summary>
		public ReadOnlyDictionary<string, Script> Scripts {
			get { return scriptManager.Scripts; }
		}

		/// <summary>Gets the number of levels stored in the world.</summary>
		public int LevelCount {
			get { return levels.Count; }
		}

		/// <summary>Gets the number of dungeons stored in the world.</summary>
		public int DungeonCount {
			get { return dungeons.Count; }
		}

		/// <summary>Gets the number of scripts stored in the script manager.</summary>
		public int ScriptCount {
			get { return scriptManager.ScriptCount; }
		}

		// Startup --------------------------------------------------------------------

		/// <summary>Gets the start room for the world.</summary>
		public Room StartRoom {
			get { return levels[startLevelIndex].GetRoomAt(startRoomLocation); }
		}

		/// <summary>Gets the index of the starting level for the world.</summary>
		public int StartLevelIndex {
			get { return startLevelIndex; }
			set { startLevelIndex = value; }
		}

		/// <summary>Gets the starting level for the world.</summary>
		public Level StartLevel {
			get { return levels[startLevelIndex]; }
		}

		/// <summary>Gets the starting location of the start room for the world.</summary>
		public Point2I StartRoomLocation {
			get { return startRoomLocation; }
			set { startRoomLocation = value; }
		}

		/// <summary>Gets the starting location in the start room for the world.</summary>
		public Point2I StartTileLocation {
			get { return startTileLocation; }
			set { startTileLocation = value; }
		}
	}
}
