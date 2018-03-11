using System;
using System.Reflection;
using System.Threading;

namespace ZeldaOracle.Game.Control.Scripting {

	/// <summary>An instance of a running script.</summary>
	public class ScriptInstance : ZeldaAPI.ScriptActions {

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
			context = scriptContext;

			isComplete = false;
			isTerminated = false;
			allowAutoResume = true;
			exception = null;

			// Setup the script context
			context.game = roomControl.GameControl;
			context.room = roomControl;
			context.area = roomControl.GameControl.AreaControl;
			context.actions = this;

			// Start the script thread and wait for it to return
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

				// Setup the script context
				context.game = roomControl.GameControl;
				context.room = roomControl;
				context.area = roomControl.GameControl.AreaControl;
				context.actions = this;

				// Resume the script thread and wait for it to return
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

				// Setup the script context
				context.game = roomControl.GameControl;
				context.room = roomControl;
				context.area = roomControl.GameControl.AreaControl;
				context.actions = this;

				// Resume the script thread and wait for it to complete
				signalReturnToCaller.Reset();
				signalResumeScript.Set();
				thread.Join();
			}
		}


		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------

		/// <summary>Interrupt the script, returning control to the main thread. The
		/// script will continue once Resume is called by the main thread.</summary>
		private void ReturnToCaller() {
			// Signal the calling thread that this script has returned
			signalResumeScript.Reset();
			signalReturnToCaller.Set();

			// Wait for the calling thread to signal this to resume
			signalResumeScript.WaitOne();

			// If a terminate was requested, then raise an exception that will
			// terminate the thread function
			if (isTerminated)
				thread.Abort();
				//throw new TargetInvocationException();
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
		// API Methods
		//-----------------------------------------------------------------------------

		public void WaitForCondition(ZeldaAPI.WaitCondition condition) {
			while (!condition.Invoke())
				ReturnToCaller();
		}
		
		public void Wait(int ticks) {
			int startWaitTime = roomControl.GameManager.ElapsedTicks;
			while (roomControl.GameManager.ElapsedTicks - startWaitTime < ticks)
				ReturnToCaller();
		}

		public void Message(string text) {
			roomControl.GameControl.DisplayMessage(text, null, Resume);
			allowAutoResume = false;
			ReturnToCaller();
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
