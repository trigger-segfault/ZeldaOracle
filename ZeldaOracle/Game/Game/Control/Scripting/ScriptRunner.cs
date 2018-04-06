using System;
using System.Collections.Generic;
using System.Reflection;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Common.Util;
using ZeldaOracle.Game.Worlds;

namespace ZeldaOracle.Game.Control.Scripting {

	/// <summary>Used to execute scripts during runtime. Scripts are run in their own
	/// threads so that they can perform blocking actions such as sleeping or
	/// displaying a text message. There is no need for thread safety, however, because
	/// script threads are run in lock-step with the main thread.</summary>
	public class ScriptRunner {

		private GameControl gameControl;
		/// <summary>The loaded script assembly.</summary>
		private Assembly compiledAssembly;
		/// <summary>The script context class from the script assembly, which contains
		/// methods for all user scripts and trigger scripts.</summary>
		private ZeldaAPI.CustomScriptBase context;
		/// <summary>The list of scripts which are currently running.</summary>
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
		
		/// <summary>Terminate all scripts, aborting their threads.</summary>
		public void TerminateAllScripts() {
			for (int i = 0; i < runningScripts.Count; i++) {
				Logs.Scripts.LogNotice("Terminating script: {0}",
					runningScripts[i].Name);
				runningScripts[i].Terminate();
			}
			runningScripts.Clear();
		}

		/// <summary>Terminate all scripts which are tied to the given room control.</summary>
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

		/// <summary>Called upon loading a world, in order to load the script assembly
		/// and construct the script context class.</summary>
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

			// Find the MethodInfo for each user script
			foreach (Script script in world.Scripts.Values)
				script.MethodInfo = contextType.GetMethod(script.MethodName);

			return true;
		}


		//-----------------------------------------------------------------------------
		// Script Execution
		//-----------------------------------------------------------------------------

		/// <summary>Get the script's MethodInfo from the scripting assembly.</summary>
		private MethodInfo GetMethodInfo(Script script) {
			if (script.MethodInfo == null)
				script.MethodInfo = context.GetType().GetMethod(script.MethodName);
			return script.MethodInfo;
		}

		/// <summary>Run a script with the given parameters.</summary>
		public void RunScript(Script script, object[] parameters) {
			if (script.MethodInfo != null)
				RunScript(context, script.ID, GetMethodInfo(script), null, parameters);
			else
				Logs.Scripts.LogError("No MethodInfo found for trigger script '" +
					script.MethodName + "'");
		}

		/// <summary>Run a method as a script with the given parameters.</summary>
		public void RunScript(MethodInfo method, object[] parameters) {
			RunScript(context, method.Name, method, null, parameters);
		}

		public void RunScript(ZeldaAPI.CustomScriptBase context, Action function) {
			MethodInfo method = function.GetMethodInfo();
			RunScript(context, method.Name, method, null, new object[] {});
		}

		public void RunScript(ZeldaAPI.CustomScriptBase context,
			string name, Action function)
		{
			MethodInfo method = function.GetMethodInfo();
			RunScript(context, name, method, null, new object[] {});
		}

		public void RunTrigger(Trigger trigger, object caller) {
			RunScript(context, trigger.Script.MethodName,
				GetMethodInfo(trigger.Script), trigger, new object[] { caller });
		}

		/// <summary>Run a method as a script with the given parameters.</summary>
		public void RunScript(ZeldaAPI.CustomScriptBase context,
			string name, MethodInfo method, Trigger trigger, object[] parameters)
		{
			Logs.Scripts.LogNotice("Running script: {0}", name);
			ScriptInstance instance = new ScriptInstance(
				gameControl.RoomControl, name, method, trigger, parameters);
			instance.Start(context);
			if (!instance.IsComplete)
				runningScripts.Add(instance);
		}

		/// <summary>Update execution for any running scripts. This is intended to be
		/// called by GameControl.</summary>
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
