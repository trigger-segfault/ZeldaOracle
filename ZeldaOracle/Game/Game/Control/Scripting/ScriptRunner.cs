using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using ZeldaOracle.Game.Worlds;


namespace ZeldaOracle.Game.Control.Scripting {

	public class ScriptRunner {

		private GameControl gameControl;
		private ZeldaAPI.CustomScriptBase scriptObject;
		private Assembly compiledAssembly;
		private Dictionary<string, MethodInfo> scriptMethods;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ScriptRunner(GameControl gameControl) {
			this.gameControl		= gameControl;
			this.scriptObject		= null;
			this.compiledAssembly	= null;
			this.scriptMethods		= new Dictionary<string, MethodInfo>();
		}


		//-----------------------------------------------------------------------------
		// World Initialization
		//-----------------------------------------------------------------------------
		
		public bool OnLoadWorld(World world) {
			scriptMethods.Clear();

			// Load the assembly.
			byte[] rawAssembly = world.ScriptManager.RawAssembly;
			if (rawAssembly == null || rawAssembly.Length == 0)
				return false;
			compiledAssembly = Assembly.Load(rawAssembly);
			if (compiledAssembly == null)
				return false;

			// Find the type (class) of the custom script method.
			Type type = compiledAssembly.GetType("ZeldaAPI.CustomScripts.CustomScript");
			if (type == null)
				return false;

			// Find the default constructor for the type.
			ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);
			if (constructor == null)
				return false;

			// Construct the script object.
			scriptObject = (ZeldaAPI.CustomScriptBase) constructor.Invoke(null);
			if (scriptObject == null)
				return false;

			// Find the script method infos.
			foreach (string scriptName in world.Scripts.Keys) {
				scriptMethods[scriptName] = type.GetMethod(scriptName);
			}

			return true;
		}


		//-----------------------------------------------------------------------------
		// Script Execution
		//-----------------------------------------------------------------------------
		
		public void RunScript(string scriptID, object[] parameters) {
			// Find the script's method.
			MethodInfo methodInfo = scriptMethods[scriptID];

			if (methodInfo != null) {
				// Setup script object member variables.
				scriptObject.game = gameControl;
				scriptObject.room = gameControl.RoomControl;

				// Invoke the method.
				methodInfo.Invoke(scriptObject, parameters);
			}
		}
		/*
		public static void RunScript(GameControl gameControl, Script script, params object[] parameters) {
			Assembly assembly = script.CompiledAssembly;
			if (assembly == null)
				return;

			// Find the type (class) of the custom script method.
			Type type = assembly.GetType("ZeldaAPI.CustomScripts.CustomScript");
			if (type == null)
				return;

			// Find the default constructor for the type.
			ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);
			if (constructor == null)
				return;

			// Find the RunScript method for the type.
			MethodInfo method = type.GetMethod("RunScript");
			if (type == null)
				return;

			// Construct the script object.
			ZeldaAPI.CustomScriptBase scriptObject = (ZeldaAPI.CustomScriptBase) constructor.Invoke(null);
			if (scriptObject == null)
				return;

			// Setup the script's public class members.
			scriptObject.game = null;
			scriptObject.room = gameControl.RoomControl;
			
			// Invoke the RunScript method.
			method.Invoke(scriptObject, parameters);
		}
		*/
	}
}
