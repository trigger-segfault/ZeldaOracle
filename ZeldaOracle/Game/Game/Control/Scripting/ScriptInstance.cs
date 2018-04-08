using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Common.Util;
using ZeldaOracle.Game.Control.Scripting.Interface.Actions;
using ZeldaOracle.Game.Control.Scripting.Interface.Functions;

namespace ZeldaOracle.Game.Control.Scripting {

	/// <summary>An instance of a running script action.</summary>
	public class ScriptAction : ZeldaAPI.ScriptAction {

		/// <summary>The script instance this action is running for.</summary>
		public ScriptInstance ScriptInstance { get; set; }
		
		/// <summary>The update function called after every game tick. This function
		/// should return true if the action has completed.</summary>
		public ZeldaAPI.UpdateCondition UpdateFunction { get; set; }

		/// <summary>True if this action has completed.</summary>
		public bool IsComplete { get; set; }

		/// <summary>True if the script instance is waiting for this action to
		/// complete.</summary>
		public bool WaitForCompletion { get; set; }

		/// <summary>Update the action, calling its update function and
		/// checking if it has completed.</summary>
		public void Update() {
			if (!IsComplete && UpdateFunction != null) {
				if (UpdateFunction.Invoke())
					IsComplete = true;
			}
		}

		/// <summary>Wait for the action to complete.</summary>
		public void Wait(bool wait = true) {
			Logs.Scripts.LogInfo("Waiting for action to complete");
			ScriptInstance.WaitForCondition(delegate() {
				return IsComplete;
			});
		}
	}

	/// <summary>An instance of a running script.</summary>
	public class ScriptInstance {

		//private Script script;
		private string name;
		private MethodInfo method;
		private object[] parameters;
		private Thread thread;
		private ManualResetEvent signalResumeScript;
		private ManualResetEvent signalReturnToCaller;
		private bool isComplete;
		private RoomControl roomControl;
		private ZeldaAPI.CustomScriptBase context;
		private bool allowAutoResume;
		private bool isTerminated;
		private Exception exception;
		private int startTime;
		private List<ScriptAction> actions;
		private Trigger trigger;

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public ScriptInstance(RoomControl roomControl, string name,
			MethodInfo method, Trigger trigger, object[] parameters)
		{
			this.name				= name;
			this.roomControl		= roomControl;
			this.method				= method;
			this.parameters			= parameters;
			this.trigger			= trigger;
			actions					= new List<ScriptAction>();
			thread					= new Thread(ThreadFunction);
			isComplete				= false;
			exception				= null;
			signalResumeScript		= new ManualResetEvent(true);
			signalReturnToCaller	= new ManualResetEvent(false);
		}


		//-----------------------------------------------------------------------------
		// Thread Operations
		//-----------------------------------------------------------------------------
		
		/// <summary>Start the script and wait for it to return.</summary>
		public void Start(ZeldaAPI.CustomScriptBase scriptContext) {
			context			= scriptContext;
			isComplete		= false;
			isTerminated	= false;
			allowAutoResume	= true;
			exception		= null;
			startTime		= RoomControl.GameControl.GameManager.ElapsedTicks;
			actions.Clear();

			// Start the script thread and wait for it to return
			SetupContext();
			signalReturnToCaller.Reset();
			signalResumeScript.Set();
			thread.Start();
			signalReturnToCaller.WaitOne();
			if (isComplete)
				thread.Join();
		}

		/// <summary>Resume the script and wait for it to return.</summary>
		public void Resume() {
			if (!isComplete) {
				allowAutoResume = true;
				
				// Resume the script thread and wait for it to return
				SetupContext();
				signalReturnToCaller.Reset();
				signalResumeScript.Set();
				signalReturnToCaller.WaitOne();
				if (isComplete)
					thread.Join();
			}
		}

		/// <summary>Called at the end of each game tick to update any scripts which
		/// have not yet completed.</summary>
		public void AutoResume() {
			if (allowAutoResume && !isComplete)
				Resume();
		}

		public void Update() {
			if (!isComplete) {
				// Update active actions
				for (int i = 0; i < actions.Count; i++) {
					actions[i].Update();
					if (actions[i].IsComplete)
						actions.RemoveAt(i--);
				}
			}
			
			if (allowAutoResume && !isComplete)
				Resume();
		}

		public void Terminate() {
			if (!isComplete) {
				isTerminated = true;

				// Resume the script thread and wait for it to complete
				SetupContext();
				signalReturnToCaller.Reset();
				signalResumeScript.Set();
				thread.Join();
			}
		}
		
		private void SetupContext() {
			context.game = roomControl.GameControl;
			context.room = roomControl;
			context.area = roomControl.GameControl.AreaControl;
			context.player = roomControl.Player;
			context.Actions = new ScriptActions(this);
			context.Functions = new ScriptFunctions(this);
		}


		//-----------------------------------------------------------------------------
		// Script Thread Methods
		//-----------------------------------------------------------------------------

		public void LogMessage(string format, params object[] args) {
			string message = String.Format(format, args);
			Logs.Scripts.LogInfo("{0}: {1}", name, message);
		}

		public ScriptAction BeginAction(ZeldaAPI.UpdateCondition update) {
			ScriptAction action = new ScriptAction() {
				ScriptInstance = this,
				IsComplete = false,
				UpdateFunction = update,
			};
			actions.Add(action);
			return action;
		}

		public void PerformUpdate(ZeldaAPI.UpdateCondition update) {
			ReturnToCaller();
			while (!update.Invoke())
				ReturnToCaller();
		}

		public void PerformUpdate(ZeldaAPI.TimedUpdateCondition update) {
			ReturnToCaller();
			int startTime = RoomControl.GameControl.GameManager.ElapsedTicks;
			while (!update.Invoke(
				RoomControl.GameControl.GameManager.ElapsedTicks - startTime))
				ReturnToCaller();
		}

		public void WaitForCondition(ZeldaAPI.WaitCondition condition) {
			if (condition != null) {
				while (!condition.Invoke())
					ReturnToCaller();
			}
			else {
				ReturnToCaller();
			}
		}

		public void WaitForResume() {
			allowAutoResume = false;
			ReturnToCaller();
		}

		/// <summary>Interrupt the script, returning control to the main thread. The
		/// script will continue once Resume is called by the main thread.</summary>
		public void ReturnToCaller() {
			// Signal the calling thread that this script has returned
			signalResumeScript.Reset();
			signalReturnToCaller.Set();

			// Wait for the calling thread to signal this to resume
			signalResumeScript.WaitOne();

			// If a terminate was requested, then raise an exception that will
			// terminate the thread function
			if (isTerminated)
				thread.Abort();
		}

		/// <summary>The function which is invoked by starting script thread.</summary>
		private void ThreadFunction() {
			try {
				// Invoke the script function
				var methodParams = method.GetParameters();
				var type = parameters[0].GetType();
				method.Invoke(context, parameters);
			}
			catch (ThreadAbortException) {
				Thread.ResetAbort();
				isTerminated = true;
			}
			catch (TargetInvocationException e) {
				exception = e.InnerException;
				isTerminated = true;
			}
			catch (Exception e) {
				exception = e;
				isTerminated = true;
			}
			
			// Signal the calling thread that this script has completed
			isComplete = true;
			signalReturnToCaller.Set();
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>True if the script has completed execution.</summary>
		public bool IsComplete {
			get { return isComplete; }
		}

		/// <summary>True if the script was terminated before completion.</summary>
		public bool IsTerminated {
			get { return isTerminated; }
		}

		/// <summary>True if the script was terminated with an exception.</summary>
		public bool HasException {
			get { return isTerminated; }
		}

		/// <summary>The script which is running.</summary>
		//public Script Script {
		//	get { return script; }
		//}

		public string Name {
			get { return name; }
		}

		public Exception Exception {
			get { return exception; }
		}

		public RoomControl RoomControl {
			get { return roomControl; }
		}

		public Trigger Trigger {
			get { return trigger; }
		}

		public int StartTime {
			get { return startTime; }
			set { startTime = value; }
		}
	}
}
