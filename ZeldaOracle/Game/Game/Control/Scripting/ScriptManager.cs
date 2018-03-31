using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ZeldaOracle.Game.Control.Scripting {

	/// <summary>Manages all user-defined scripts (not trigger scripts) for a world.
	/// </summary>
	public class ScriptManager {

		/// <summary>The collection of scripts.</summary>
		private Dictionary<string, Script> scripts;
		/// <summary>The raw data for the compiled script assembly.</summary>
		private byte[] rawAssembly;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Constructs the script manager.</summary>
		public ScriptManager() {
			scripts		= new Dictionary<string, Script>();
			rawAssembly = null;
		}


		//-----------------------------------------------------------------------------
		// Accessors
		//-----------------------------------------------------------------------------

		/// <summary>Gets the script with the specified ID.</summary>
		public Script GetScript(string scriptID) {
			Script script;
			scripts.TryGetValue(scriptID, out script);
			return script;
		}

		/// <summary>Returns true if the script exists in the collection.</summary>
		public bool ContainsScript(Script script) {
			return scripts.ContainsKey(script.ID);
		}

		/// <summary>Returns true if a script with the specified ID exists.</summary>
		public bool ContainsScript(string scriptID) {
			return scripts.ContainsKey(scriptID);
		}


		//-----------------------------------------------------------------------------
		// Mutators
		//-----------------------------------------------------------------------------

		/// <summary>Adds the script from the collection.</summary>
		public void AddScript(Script script) {
			scripts.Add(script.ID, script);
		}

		/// <summary>Removes the script from the collection.</summary>
		public void RemoveScript(Script script) {
			scripts.Remove(script.ID);
		}

		/// <summary>Removes the script with the specified id from the collection.
		/// </summary>
		public void RemoveScript(string scriptID) {
			scripts.Remove(scriptID);
		}

		/// <summary>Renames the specified script.</summary>
		public bool RenameScript(Script script, string newScriptID) {
			if (newScriptID != script.ID) {
				if (scripts.ContainsKey(newScriptID)) {
					return false;
				}
				scripts.Remove(script.ID);
				script.ID = newScriptID;
				scripts.Add(script.ID, script);
			}
			return true;
		}

		/// <summary>Renames the script with the specified ID.</summary>
		public bool RenameScript(string oldScriptID, string newScriptID) {
			return RenameScript(scripts[oldScriptID], newScriptID);
		}

		/// <summary>Returns true if the script has a valid function name.</summary>
		public static bool IsValidScriptName(string name) {
			if (name.Length == 0)
				return false;
			if (!char.IsLetter(name[0]) && name[0] != '_')
				return false;

			for (int i = 1; i < name.Length; i++) {
				char c = name[i];
				if (!char.IsLetterOrDigit(c) && c != '_') {
					return false;
				}
			}
			return true;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets the collection of scripts.</summary>
		public ReadOnlyDictionary<string, Script> Scripts {
			get { return new ReadOnlyDictionary<string, Script>(scripts); }
		}

		/// <summary>Gets the raw data for the compiled script assembly.</summary>
		public byte[] RawAssembly {
			get { return rawAssembly; }
			set { rawAssembly = value; }
		}

		/// <summary>Gets the number of scripts stored in the script manager.</summary>
		public int ScriptCount {
			get { return scripts.Count; }
		}
	}
}
