using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.API;
using ZeldaOracle.Game.Control.Scripting;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Tiles.ActionTiles;

namespace ZeldaOracle.Game.Worlds {
	/// <summary>The world class containing everything about the game.</summary>
	public class World : IEventObjectContainer, IEventObject, IIDObject,
		IVariableObjectContainer, IVariableObject
	{

		private Properties properties;
		private EventCollection events;
		private Variables variables;
		private List<Level> levels;
		private List<Area> areas;
		private ScriptManager scriptManager;
		private int startLevelIndex;
		private Point2I startRoomLocation;
		private Point2I startTileLocation;

		/// <summary>Keep an instance of a default area to use when the level does not
		/// have one assigned.</summary>
		private Area defaultArea;

		/// <summary>The next unqiue monster ID used for linking.</summary>
		private int nextMonsterID;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		/// <summary>Constructs an empty world.</summary>
		public World() {
			this.areas		= new List<Area>();
			this.levels			= new List<Level>();
			this.scriptManager	= new ScriptManager();

			this.events			= new EventCollection(this);
			this.properties		= new Properties(this);
			this.properties.BaseProperties = new Properties();
			this.variables		= new Variables(this);

			this.defaultArea	= new Area();
			this.defaultArea.World = this;

			this.nextMonsterID  = int.MaxValue;

			this.properties.BaseProperties.Set("id", "world_name")
				.SetDocumentation("ID", "", "", "General", "The ID used for saves to identify the world.", true, true);

			this.events.AddEvent("start_game", "Start Game", "Initialization",
				"Called when the game first starts.", new ScriptParameter(typeof(ZeldaAPI.Game), "game"));

			// This will be debug-assigned as "Link" in GameControl.StartGame until we
			// have an enter name screen.
			this.variables.AddBuiltIn("player", "");
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
		
		// Sub-Levels -----------------------------------------------------------------

		/// <summary>Gets the collection of all rooms in the levels.</summary>
		public IEnumerable<Room> GetRooms() {
			foreach (Level level in levels) {
				foreach (Room room in level.GetRooms()) {
					yield return room;
				}
			}
		}

		/// <summary>Gets the collection of all tiles in the levels.</summary>
		public IEnumerable<TileDataInstance> GetTiles() {
			foreach (Level level in levels) {
				foreach (Room room in level.GetRooms()) {
					foreach (var tile in room.GetTiles()) {
						yield return tile;
					}
				}
			}
		}

		/// <summary>Gets the collection of all tiles in the levels.</summary>
		public IEnumerable<ActionTileDataInstance> GetActionTiles() {
			foreach (Level level in levels) {
				foreach (Room room in level.GetRooms()) {
					foreach (var tile in room.GetActionTiles(false)) {
						yield return tile;
					}
				}
			}
		}

		// Areas ----------------------------------------------------------------------

		/// <summary>Gets the collection of areas.</summary>
		public IEnumerable<Area> GetAreas() {
			return areas;
		}

		/// <summary>Gets the area with the specified ID.</summary>
		public Area GetArea(string areaID) {
			return areas.Find(area => { return area.Properties.GetString("id") == areaID; });
		}

		/// <summary>Gets the area at the specified index.</summary>
		public Area GetAreaAt(int index) {
			return areas[index];
		}

		/// <summary>Returns true if the area exists in the collection.</summary>
		public bool ContainsArea(Area area) {
			return areas.Contains(area);
		}

		/// <summary>Returns true if a area with the specified ID exists.</summary>
		public bool ContainsArea(string areaID) {
			return (GetArea(areaID) != null);
		}

		/// <summary>Gets the index of the specified area.</summary>
		public int IndexOfArea(Area area) {
			return areas.IndexOf(area);
		}

		/// <summary>Gets the index of the area with the specified ID.</summary>
		public int IndexOfArea(string areaID) {
			return areas.FindIndex(area => area.ID == areaID);
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
			foreach (Area area in areas) {
				yield return area;
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
			foreach (Area area in areas) {
				yield return area;
			}
		}

		/// <summary>Gets the collection of defined events in the world.</summary>
		public IEnumerable<Event> GetDefinedEvents() {
			foreach (IEventObject eventObject in GetEventObjects()) {
				foreach (Event evnt in eventObject.Events.GetEvents()) {
					if (evnt.IsDefined) {
						yield return evnt;
					}
				}
			}
			foreach (Area area in areas) {
				foreach (Event evnt in area.Events.GetEvents()) {
					if (evnt.IsDefined) {
						yield return evnt;
					}
				}
			}
		}

		/// <summary>Gets the collection of variables objects in the world.</summary>
		public IEnumerable<IVariableObject> GetVariableObjects() {
			yield return this;
			foreach (Level level in levels) {
				foreach (IVariableObject variableObject in level.GetVariableObjects()) {
					yield return variableObject;
				}
			}
			foreach (Area area in areas) {
				yield return area;
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

		/// <summary>Removes the specified level.</summary>
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

		/// <summary>Renames the specified level.</summary>
		public bool RenameLevel(Level level, string newLevelID) {
			if (level.ID != newLevelID) {
				if (ContainsLevel(newLevelID)) {
					return false;
				}
				level.ID = newLevelID;
			}
			return true;
		}

		/// <summary>Renames the level with the specified ID.</summary>
		public bool RenameLevel(string oldLevelID, string newLevelID) {
			return RenameLevel(GetLevel(oldLevelID), newLevelID);
		}

		/// <summary>Renames the level at the specified index.</summary>
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
		
		// Areas ----------------------------------------------------------------------

		/// <summary>Adds the area to the end of the list.</summary>
		public void AddArea(Area area) {
			areas.Add(area);
			area.World = this;
		}

		/// <summary>Inserts the area at the specified index.</summary>
		public void InsertArea(int index, Area area) {
			areas.Insert(index, area);
			area.World = this;
		}

		/// <summary>Removes the specified area.</summary>
		public void RemoveArea(Area area) {
			areas.Remove(area);
		}

		/// <summary>Removes the area with the specified ID.</summary>
		public void RemoveArea(string areaID) {
			int index = areas.FindIndex(area => area.ID == areaID);
			if (index != -1)
				areas.RemoveAt(index);
		}

		/// <summary>Renames the area at the specified index.</summary>
		public void RemoveAreaAt(int index) {
			areas.RemoveAt(index);
		}

		/// <summary>Renames the specified area.</summary>
		public bool RenameArea(Area area, string newAreaID) {
			if (area.ID != newAreaID) {
				if (ContainsArea(newAreaID)) {
					return false;
				}
				area.ID = newAreaID;
			}
			return true;
		}

		/// <summary>Renames the area with the specified ID.</summary>
		public bool RenameArea(string oldAreaID, string newAreaID) {
			return RenameArea(GetArea(oldAreaID), newAreaID);
		}

		/// <summary>Renames the area at the specified index.</summary>
		public bool RenameAreaAt(int index, string newAreaID) {
			return RenameArea(areas[index], newAreaID);
		}

		/// <summary>Moves the area at the specified index to the new index.</summary>
		public void MoveArea(int oldIndex, int newIndex, bool relative) {
			if (relative)
				newIndex = oldIndex + newIndex;

			Area area = areas[oldIndex];
			areas.RemoveAt(oldIndex);
			areas.Insert(newIndex, area);
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
		// In-Game Methods
		//-----------------------------------------------------------------------------

		/// <summary>Assigns unique IDs to every monster in the game for respawn
		/// purposes.</summary>
		public void AssignMonsterIDs() {
			foreach (Room room in GetRooms()) {
				room.AssignMonsterIDs();
				/*if (room.CanLinkMonsters && !room.AreMonstersLinked) {
					Room rootRoom = room.RootRoom;
					if (rootRoom == room) {
						room.LinkInitialMonsters();
					}
					else if (rootRoom.CanLinkMonsters) {
						if (!room.AreMonstersLinked)
							rootRoom.LinkInitialMonsters();
						room.LinkMonsters(rootRoom);
					}
				}*/
			}
		}

		/// <summary>Gets the next unique monster ID for linking.</summary>
		public int NextMonsterID() {
			return nextMonsterID--;
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

		/// <summary>Gets the variables for the world.</summary>
		public Variables Vars {
			get { return variables; }
		}

		/// <summary>Gets the script manager for the world.</summary>
		public ScriptManager ScriptManager {
			get { return scriptManager; }
		}

		/// <summary>Gets the collection of levels for the world.</summary>
		/*public List<Level> Levels {
			get { return levels; }
		}*/

		/// <summary>Gets the collection of areas for the world.</summary>
		/*public List<Area> Areas {
			get { return areas; }
		}*/

		/// <summary>Gets the collection of scripts for the world.</summary>
		public ReadOnlyDictionary<string, Script> Scripts {
			get { return scriptManager.Scripts; }
		}

		/// <summary>Gets the number of levels stored in the world.</summary>
		public int LevelCount {
			get { return levels.Count; }
		}

		/// <summary>Gets the number of areas stored in the world.</summary>
		public int AreaCount {
			get { return areas.Count; }
		}

		/// <summary>Gets the number of scripts stored in the script manager.</summary>
		public int ScriptCount {
			get { return scriptManager.ScriptCount; }
		}

		/// <summary>Gets the default area for this world when none is supplied.</summary>
		public Area DefaultArea {
			get { return defaultArea; }
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
