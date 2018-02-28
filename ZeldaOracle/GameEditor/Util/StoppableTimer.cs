using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace ZeldaEditor.Util {
	/// <summary>A DispatcherTimer wrapper with the ability to be halted statically.</summary>
	public class StoppableTimer {

		//-----------------------------------------------------------------------------
		// Static Members
		//-----------------------------------------------------------------------------

		/// <summary>The list of existing timers.</summary>
		private static List<WeakReference<DispatcherTimer>> timers;
		/// <summary>The list of stopped timers while all timers are stopped.</summary>
		private static List<WeakReference<DispatcherTimer>> stoppedTimers;
		/// <summary>The number of time the timers have been stopped.</summary>
		private static int stopCount;


		//-----------------------------------------------------------------------------
		// Members
		//-----------------------------------------------------------------------------

		/// <summary>The internal timer.</summary>
		private DispatcherTimer timer;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Initializes the StoppableTimer handler.</summary>
		static StoppableTimer() {
			timers = new List<WeakReference<DispatcherTimer>>();
			stoppedTimers = new List<WeakReference<DispatcherTimer>>();
			stopCount = 0;
		}

		/// <summary>Constructs a StoppableTimer from a DispatcherTimer.</summary>
		private StoppableTimer(DispatcherTimer timer) {
			this.timer = timer;
		}


		//-----------------------------------------------------------------------------
		// Static Constructors
		//-----------------------------------------------------------------------------

		/// <summary>Creates a stopped new timer and adds it to the list for stoppable
		/// timers.</summary>
		public static StoppableTimer Create(TimeSpan interval,
			DispatcherPriority priority, Action callback)
		{
			StoppableTimer timer = StartNew(interval, priority, callback);
			timer.timer.Stop();
			return timer;
		}

		/// <summary>Creates and starts a new timer and adds it to the list for
		/// stoppable timers.</summary>
		public static StoppableTimer StartNew(TimeSpan interval,
			DispatcherPriority priority, Action callback)
		{
			CleanupTimers();
			DispatcherTimer timer = new DispatcherTimer(interval, priority,
				delegate { callback(); }, Application.Current.Dispatcher);
			timers.Add(new WeakReference<DispatcherTimer>(timer));
			return new StoppableTimer(timer);
		}


		//-----------------------------------------------------------------------------
		// Instance Timer Methods
		//-----------------------------------------------------------------------------

		/// <summary>Starts the timer or adds it to the stopped timer list if
		/// timers are currently stopped.</summary>
		public void Start() {
			if (AllStopped) {
				int index = IndexOfTimer(stoppedTimers);
				if (index == -1) {
					index = IndexOfTimer(timers);
					stoppedTimers.Add(timers[index]);
				}
			}
			else {
				timer.Start();
			}
		}

		/// <summary>Stops the timer or removes it to the stopped timer list if
		/// timers are currently stopped.</summary>
		public void Stop() {
			if (AllStopped) {
				int index = IndexOfTimer(stoppedTimers);
				if (index != -1)
					stoppedTimers.RemoveAt(index);
			}
			else {
				timer.Stop();
			}
		}


		//-----------------------------------------------------------------------------
		// Static Timer Methods
		//-----------------------------------------------------------------------------
		
		/// <summary>Stops all timers.</summary>
		public static void StopAll() {
			stopCount++;
			if (stopCount > 1)
				return;
			for (int i = 0; i < timers.Count; i++) {
				DispatcherTimer timer;
				if (timers[i].TryGetTarget(out timer)) {
					stoppedTimers.Add(timers[i]);
					timer.Stop();
				}
				else {
					timers.RemoveAt(i);
					i--;
				}
			}
		}

		/// <summary>Resumes all stopped timers.</summary>
		public static void ResumeAll() {
			if (stopCount > 0)
				stopCount--;
			if (stopCount > 0)
				return;
			while (stoppedTimers.Any()) {
				DispatcherTimer timer;
				if (stoppedTimers[0].TryGetTarget(out timer)) {
					timer.Start();
				}
				stoppedTimers.RemoveAt(0);
			}
		}


		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------

		/// <summary>Returns the index of a timer in a weak reference list.</summary>
		private int IndexOfTimer(List<WeakReference<DispatcherTimer>> timers) {
			for (int i = 0; i < timers.Count; i++) {
				DispatcherTimer timer;
				if (timers[i].TryGetTarget(out timer) && timer == this.timer) {
					return i;
				}
			}
			return -1;
		}

		/// <summary>Cleans up all garbage collected timers.</summary>
		private static void CleanupTimers() {
			for (int i = 0; i < timers.Count; i++) {
				DispatcherTimer timer;
				if (!timers[i].TryGetTarget(out timer)) {
					timers.RemoveAt(i);
					i--;
				}
			}
		}


		//-----------------------------------------------------------------------------
		// Events
		//-----------------------------------------------------------------------------

		/// <summary>Occurs when the timer interval has elapsed.</summary>
		public event EventHandler Tick {
			add { timer.Tick += value; }
			remove { timer.Tick -= value; }
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------
		
		/// <summary>Gets or sets the period of time between timer ticks.</summary>
		/// <exception cref="System.ArgumentOutOfRangeException">interval is less than
		/// 0 or greater than System.Int32.MaxValue milliseconds.</exception>
		public TimeSpan Interval {
			get { return timer.Interval; }
			set { timer.Interval = value; }
		}

		/// <summary>Gets or sets a value that indicates whether the timer is running.</summary>
		public bool IsEnabled {
			get { return timer.IsEnabled; }
			set {
				if (value != timer.IsEnabled) {
					if (value)	Start();
					else		Stop();
				}
			}
		}


		//-----------------------------------------------------------------------------
		// Static Properties
		//-----------------------------------------------------------------------------

		/// <summary>Returns true if all timers are stopped.</summary>
		public bool AllStopped {
			get { return stopCount > 0; }
		}
	}
}
