using System;
using System.Collections.Generic;
using System.Reflection;
using ZeldaOracle.Common.Util;
using ZeldaOracle.Game.Worlds;


namespace ZeldaOracle.Game.Control.Scripting {

	public class ScriptRunner {

		private GameControl gameControl;
		private ZeldaAPI.CustomScriptBase scriptObject;
		private Assembly compiledAssembly;
		private Dictionary<Script, MethodInfo> scriptMethods;
		private List<ScriptInstance> runningScripts;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ScriptRunner(GameControl gameControl) {
			this.gameControl	= gameControl;
			scriptObject		= null;
			compiledAssembly	= null;
			scriptMethods		= new Dictionary<Script, MethodInfo>();
			runningScripts		= new List<ScriptInstance>();
		}


		//-----------------------------------------------------------------------------
		// World Initialization
		//-----------------------------------------------------------------------------
		
		public void TerminateAllScripts() {
			for (int i = 0; i < runningScripts.Count; i++) {
				Logs.Scripts.Log("Terminating script '{0}'",
					runningScripts[i].Script.ID);
				//Console.WriteLine("Terminating script '{0}'",
					//runningScripts[i].Script.ID);
				runningScripts[i].Terminate();
			}
			runningScripts.Clear();
		}

		public void TerminateRoomScripts(RoomControl roomControl) {
			// Terminate all script which are tied to the given room control
			for (int i = 0; i < runningScripts.Count; i++) {
				ScriptInstance script = runningScripts[i];
				if (script.RoomControl == roomControl) {
					Logs.Scripts.Log("Terminating script '{0}'", script.Script.ID);
					script.Terminate();
					runningScripts.RemoveAt(i--);
				}
			}
		}

		public bool OnLoadWorld(World world) {
			scriptMethods.Clear();

			// Load the assembly
			byte[] rawAssembly = world.ScriptManager.RawAssembly;
			if (rawAssembly == null || rawAssembly.Length == 0)
				return false;
			compiledAssembly = Assembly.Load(rawAssembly);
			if (compiledAssembly == null)
				return false;

			// Find the type (class) of the custom script method
			Type type = compiledAssembly.GetType("ZeldaAPI.CustomScripts.CustomScript");
			if (type == null)
				return false;

			// Find the default constructor for the type
			ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);
			if (constructor == null)
				return false;

			// Construct the script object
			scriptObject = (ZeldaAPI.CustomScriptBase) constructor.Invoke(null);
			if (scriptObject == null)
				return false;

			// Create a mapping of scripts to method infos
			foreach (KeyValuePair<string, Script> script in world.Scripts)
				scriptMethods[script.Value] = type.GetMethod(script.Key);

			return true;
		}


		//-----------------------------------------------------------------------------
		// Script Execution
		//-----------------------------------------------------------------------------

		/// <summary>Run a script with the given parameters.</summary>
		public void RunScript(string scriptId, object[] parameters) {
			Script script = gameControl.World.Scripts[scriptId];
			RunScript(script, parameters);
		}

		/// <summary>Run a script with the given parameters.</summary>
		public void RunScript(Script script, object[] parameters) {
			MethodInfo methodInfo = scriptMethods[script];

			if (methodInfo != null) {
				ScriptInstance instance = new ScriptInstance(
					script, gameControl.RoomControl, methodInfo, parameters);
				Logs.Scripts.Log("Running script '{0}'", script.ID);
				instance.Start(scriptObject);
				if (!instance.IsComplete)
					runningScripts.Add(instance);
			}
		}

		/// <summary>Update execution for any running scripts.</summary>
		public void UpdateScriptExecution() {
			for (int i = 0; i < runningScripts.Count; i++) {
				ScriptInstance script = runningScripts[i];
				script.AutoResume();
				if (script.Exception != null) {
					Console.WriteLine("ERROR: script exited with exception");
					//throw script.Exception;
				}
				if (script.IsComplete)
					runningScripts.RemoveAt(i--);
			}
		}
	}
}
