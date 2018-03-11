using System;
using System.Reflection;
using System.Threading;
using ZeldaOracle.Common.Util;
using ZeldaOracle.Game.Control.Scripting.Actions;

namespace ZeldaOracle.Game.Control.Scripting {

	/// <summary>An instance of a running script.</summary>
	public class ScriptInstance {

		private Script script;
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


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public ScriptInstance(Script script, RoomControl roomControl,
			MethodInfo method, object[] parameters)
		{
			this.script = script;
			this.roomControl = roomControl;
			this.method = method;
			this.parameters = parameters;
			thread = new Thread(ThreadFunction);
			isComplete = false;
			exception = null;

			signalResumeScript = new ManualResetEvent(true);
			signalReturnToCaller = new ManualResetEvent(false);
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
		}


		//-----------------------------------------------------------------------------
		// Script Thread Methods
		//-----------------------------------------------------------------------------

		public void LogMessage(string format, params object[] args) {
			string message = String.Format(format, args);
			Logs.Scripts.Log("{0}: {1}", script.ID, message);
		}

		public void PerformUpdate(ZeldaAPI.WaitCondition update) {
			ReturnToCaller();
			while (!update.Invoke())
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
		public Script Script {
			get { return script; }
		}

		public Exception Exception {
			get { return exception; }
		}

		public RoomControl RoomControl {
			get { return roomControl; }
		}
	}
}
