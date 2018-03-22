using System;
using System.Collections.Generic;
using System.Reflection;
using ZeldaOracle.Common.Util;
using ZeldaOracle.Game.Worlds;


namespace ZeldaOracle.Game.Control.Scripting {

	//public class ScriptRunInfo {
	//	private string name;
	//	private ZeldaAPI.CustomScriptBase context;
	//	private MethodInfo method;
	//	private Action execute;
	//	private object[] parameters;

	//	public ScriptRunInfo(string name, MethodInfo method, params object[] parameters) {
	//		this.name = name;
	//		this.parameters = parameters;
	//		this.execute = new delegate() {
	//			execute.GetMethodInfo
	//		}
	//	}
	//}

	public class ScriptRunner {

		private GameControl gameControl;
		private Assembly compiledAssembly;
		private ZeldaAPI.CustomScriptBase context;
		private List<ScriptInstance> runningScripts;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ScriptRunner(GameControl gameControl) {
			this.gameControl	= gameControl;
			context				= null;
			compiledAssembly	= null;
			runningScripts		= new List<ScriptInstance>();
		}


		//-----------------------------------------------------------------------------
		// World Initialization
		//-----------------------------------------------------------------------------
		
		public void TerminateAllScripts() {
			for (int i = 0; i < runningScripts.Count; i++) {
				Logs.Scripts.LogNotice("Terminating script: {0}",
					runningScripts[i].Name);
				runningScripts[i].Terminate();
			}
			runningScripts.Clear();
		}

		public void TerminateRoomScripts(RoomControl roomControl) {
			// Terminate all script which are tied to the given room control
			for (int i = 0; i < runningScripts.Count; i++) {
				ScriptInstance script = runningScripts[i];
				if (script.RoomControl == roomControl) {
					Logs.Scripts.LogNotice("Terminating script: {0}", script.Name);
					script.Terminate();
					runningScripts.RemoveAt(i--);
				}
			}
		}

		public bool OnLoadWorld(World world) {
			// Load the assembly
			byte[] rawAssembly = world.ScriptManager.RawAssembly;
			if (rawAssembly == null || rawAssembly.Length == 0)
				return false;
			compiledAssembly = Assembly.Load(rawAssembly);
			if (compiledAssembly == null)
				return false;
			
			// Construct the script context class
			Type contextType = compiledAssembly.GetType(
				ScriptCodeGenerator.ScriptContextFullPath);
			if (contextType == null)
				return false;
			context = ReflectionHelper.Construct
				<ZeldaAPI.CustomScriptBase>(contextType);
			if (context == null)
				return false;

			// Find the MethodInfo for each script
			foreach (Script script in world.Scripts.Values)
				script.MethodInfo = contextType.GetMethod(script.MethodName);

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
			RunScript(context, script.ID, script.MethodInfo, parameters);
		}

		/// <summary>Run a method as a script with the given parameters.</summary>
		public void RunScript(MethodInfo method, object[] parameters) {
			RunScript(context, method.Name, method, parameters);
		}

		public void RunScript(ZeldaAPI.CustomScriptBase context, Action function) {
			MethodInfo method = function.GetMethodInfo();
			RunScript(context, method.Name, method, new object[] {});
		}

		public void RunScript(ZeldaAPI.CustomScriptBase context, string name, Action function) {
			MethodInfo method = function.GetMethodInfo();
			RunScript(context, name, method, new object[] {});
		}

		/// <summary>Run a method as a script with the given parameters.</summary>
		public void RunScript(ZeldaAPI.CustomScriptBase context,
			string name, MethodInfo method, object[] parameters)
		{
			Logs.Scripts.LogNotice("Running script: {0}", name);
			ScriptInstance instance = new ScriptInstance(
				gameControl.RoomControl, name, method, parameters);
			instance.Start(context);
			if (!instance.IsComplete)
				runningScripts.Add(instance);
		}

		/// <summary>Update execution for any running scripts.</summary>
		public void UpdateScriptExecution() {
			for (int i = 0; i < runningScripts.Count; i++) {
				ScriptInstance script = runningScripts[i];

				// Update script actions and attempt to resume execution
				script.Update();
				
				// Check if the script had an exception
				if (script.Exception != null) {
					Logs.Scripts.LogError(
						"Script '{0}' terminated with exception: {1}",
						script.Name, script.Exception.Message);
					//throw script.Exception;
				}

				// Check if the script completed
				if (script.IsComplete) {
					runningScripts.RemoveAt(i--);
					Logs.Scripts.LogNotice("Script completed after {0} ticks: {1}",
						gameControl.GameManager.ElapsedTicks - script.StartTime,
						script.Name);
				}
			}
		}
	}
}
