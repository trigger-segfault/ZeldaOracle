using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace ZeldaEditor.Util {
	/// <summary>An event that fires once on the UI thread after a delay.</summary>
	public class ScheduledEvent {

		//-----------------------------------------------------------------------------
		// Delegates
		//-----------------------------------------------------------------------------

		/// <summary>A simple event action that has no arguments.</summary>
		public delegate void ScheduledActionHandler();
		/// <summary>A simple event callback that gives the scheduled event.</summary>
		public delegate void ScheduledEventHandler(ScheduledEvent sender);


		//-----------------------------------------------------------------------------
		// Members
		//-----------------------------------------------------------------------------

		/// <summary>The timer running this event.</summary>
		private DispatcherTimer timer;
		/// <summary>The tag for extra event data.</summary>
		private object tag;
		/// <summary>True if the event has been permanently cancelled.</summary>
		private bool cancelled;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Constructs a scheduled event with no action or callback.</summary>
		public ScheduledEvent(TimeSpan delay, TimerPriority priority) {
			timer = new DispatcherTimer(delay, (DispatcherPriority) priority,
				OnTimerFinished, Application.Current.Dispatcher);
		}

		/// <summary>Constructs a scheduled event with the specified action.</summary>
		public ScheduledEvent(TimeSpan delay, TimerPriority priority,
			ScheduledActionHandler callback)
		{
			timer = new DispatcherTimer(delay, (DispatcherPriority) priority,
				OnTimerFinished, Application.Current.Dispatcher);
			ActionCallback += callback;
		}

		/// <summary>Constructs a scheduled event with the specified callback.</summary>
		public ScheduledEvent(TimeSpan delay, TimerPriority priority,
			ScheduledEventHandler callback)
		{
			timer = new DispatcherTimer(delay, (DispatcherPriority) priority,
				OnTimerFinished, Application.Current.Dispatcher);
			SenderCallback += callback;
		}

		/// <summary>Constructs a scheduled event with the specified callback and tag.</summary>
		public ScheduledEvent(TimeSpan delay, TimerPriority priority, object tag,
			ScheduledEventHandler callback)
		{
			timer = new DispatcherTimer(delay, (DispatcherPriority) priority,
				OnTimerFinished, Application.Current.Dispatcher);
			SenderCallback += callback;
			this.tag = tag;
		}


		//-----------------------------------------------------------------------------
		// Event Handlers
		//-----------------------------------------------------------------------------

		/// <summary>Called to execute the callbacks and stop the timer.</summary>
		private void OnTimerFinished(object sender, EventArgs e) {
			ActionCallback?.Invoke();
			SenderCallback?.Invoke(this);
			timer.Stop();
		}


		//-----------------------------------------------------------------------------
		// Mutators
		//-----------------------------------------------------------------------------

		/// <summary>Pauses the event.</summary>
		public void Pause() {
			if (!timer.IsEnabled)
				timer.Start();
		}

		/// <summary>Resumes the event.</summary>
		public void Resume() {
			if (cancelled)
				throw new InvalidOperationException("Cannot resume a cancelled " +
					"scheduled event!");
			if (timer.IsEnabled)
				timer.Stop();
		}

		/// <summary>Permanently cancels the event.</summary>
		public void Cancel() {
			cancelled = true;
			timer.Stop();
			Cancelled?.Invoke(this);
		}


		//-----------------------------------------------------------------------------
		// Events
		//-----------------------------------------------------------------------------

		/// <summary>An event handler that returns no arguments on callback.</summary>
		public event ScheduledActionHandler ActionCallback;

		/// <summary>An event handler that returns the sender on callback.</summary>
		public event ScheduledEventHandler SenderCallback;

		/// <summary>An event handler thats called when canceled.</summary>
		public event ScheduledEventHandler Cancelled;


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets if the timer is still running.</summary>
		public bool IsRunning {
			get { return timer.IsEnabled; }
		}

		/// <summary>Gets if the timer is permanently cancelled.</summary>
		public bool IsCancelled {
			get { return cancelled; }
		}

		/// <summary>Gets the delay for the event.</summary>
		public TimeSpan Delay {
			get { return timer.Interval; }
		}

		/// <summary>Gets or sets the tag to return in the event arguments.</summary>
		public object Tag {
			get { return tag; }
			set { tag = value; }
		}
	}

	/// <summary>A collection of scheduled events that can all be canceled.
	/// Best use is to attach it to the FrameworkElement its associated with.</summary>
	public class ScheduledEvents {

		/// <summary>The global collection of cancellable timers.</summary>
		private static HashSet<ScheduledEvent> globalTimers;

		/// <summary>The collection of cancellable timers.</summary>
		private HashSet<ScheduledEvent> timers;
		/// <summary>The element containing this event collection.</summary>
		private FrameworkElement element;
		/// <summary>The window containing this event collection.</summary>
		private Window window;
		/// <summary>The default priority for scheduled events.</summary>
		private TimerPriority defaultPriority;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Initializes the global timers collection.</summary>
		static ScheduledEvents() {
			globalTimers = new HashSet<ScheduledEvent>();
			Application.Current.Exit += OnExit;
		}

		/// <summary>Constructs scheduled events that only cancel on application exit.</summary>
		public ScheduledEvents() {
			timers = new HashSet<ScheduledEvent>();
			defaultPriority = TimerPriority.Normal;
		}

		/// <summary>Constructs scheduled events that cancel on element unloading and
		/// window closing.</summary>
		public ScheduledEvents(FrameworkElement element) : this() {
			if (element is Window) {
				window = (Window) element;
				window.Closing += OnClosing;
			}
			else {
				this.element = element;
				element.Loaded += OnLoaded;
				element.Unloaded += OnUnloaded;
			}
		}

		/// <summary>Constructs scheduled events that cancel on window closing.</summary>
		public ScheduledEvents(Window window) : this() {
			this.window = window;
			window.Closing += OnClosing;
		}


		//-----------------------------------------------------------------------------
		// Cancelling
		//-----------------------------------------------------------------------------

		/// <summary>Pauses all global timers.</summary>
		public static void GlobalPauseAll() {
			foreach (ScheduledEvent timer in globalTimers) {
				timer.Pause();
			}
		}

		/// <summary>Resumes all global timers.</summary>
		public static void GlobalResumeAll() {
			foreach (ScheduledEvent timer in globalTimers) {
				timer.Resume();
			}
		}

		/// <summary>Permanently cancels all global timers.</summary>
		public static void GlobalCancelAll() {
			foreach (ScheduledEvent timer in globalTimers.ToArray()) {
				timer.Cancel();
			}
		}

		/// <summary>Pauses all timers.</summary>
		public void PauseAll() {
			foreach (ScheduledEvent timer in timers) {
				timer.Pause();
			}
		}

		/// <summary>Resumes all timers.</summary>
		public void ResumeAll() {
			foreach (ScheduledEvent timer in timers) {
				timer.Resume();
			}
		}

		/// <summary>Permanently cancels all timers.</summary>
		public void CancelAll() {
			foreach (ScheduledEvent timer in timers.ToArray()) {
				timer.Cancel();
			}
		}


		//-----------------------------------------------------------------------------
		// Starting
		//-----------------------------------------------------------------------------

		// No Callback ----------------------------------------------------------------

		/// <summary>Schedules a new event with no action or callback.</summary>
		public ScheduledEvent Start(double seconds) {
			return Start(TimeSpan.FromSeconds(seconds), defaultPriority);
		}

		/// <summary>Schedules a new event with no action or callback.</summary>
		public ScheduledEvent Start(TimeSpan delay) {
			return Start(delay, defaultPriority);
		}

		/// <summary>Schedules a new event with no action or callback.</summary>
		public ScheduledEvent Start(double seconds, TimerPriority priority) {
			return Start(TimeSpan.FromSeconds(seconds), priority);
		}

		/// <summary>Schedules a new event with no action or callback.</summary>
		public ScheduledEvent Start(TimeSpan delay, TimerPriority priority) {
			var timer = new ScheduledEvent(delay, priority,
				OnRemoveScheduledEvent);
			timer.Cancelled += OnRemoveScheduledEvent;
			timers.Add(timer);
			globalTimers.Add(timer);
			return timer;
		}

		// Action Callback ------------------------------------------------------------

		/// <summary>Schedules a new event with the specified action.</summary>
		public ScheduledEvent Start(double seconds, Action action) {
			return Start(TimeSpan.FromSeconds(seconds), defaultPriority, action);
		}

		/// <summary>Schedules a new event with the specified action.</summary>
		public ScheduledEvent Start(TimeSpan delay, Action action) {
			return Start(delay, defaultPriority, action);
		}

		/// <summary>Schedules a new event with the specified action.</summary>
		public ScheduledEvent Start(double seconds, TimerPriority priority,
			Action action)
		{
			return Start(TimeSpan.FromSeconds(seconds), priority, action);
		}

		/// <summary>Schedules a new event with the specified action.</summary>
		public ScheduledEvent Start(TimeSpan delay, TimerPriority priority,
			Action callback) {
			var timer = new ScheduledEvent(delay, priority, OnRemoveScheduledEvent);
			timer.Cancelled += OnRemoveScheduledEvent;
			timer.ActionCallback += new ScheduledEvent.ScheduledActionHandler(callback);
			timers.Add(timer);
			globalTimers.Add(timer);
			return timer;
		}

		// Sender Callback ------------------------------------------------------------
		
		/// <summary>Schedules a new event with the specified callback.</summary>
		public ScheduledEvent Start(double seconds, Action<ScheduledEvent> callback) {
			return Start(TimeSpan.FromSeconds(seconds), defaultPriority, callback);
		}

		/// <summary>Schedules a new event with the specified callback.</summary>
		public ScheduledEvent Start(TimeSpan delay, Action<ScheduledEvent> callback) {
			return Start(delay, defaultPriority, callback);
		}

		/// <summary>Schedules a new event with the specified callback.</summary>
		public ScheduledEvent Start(double seconds, TimerPriority priority,
			Action<ScheduledEvent> callback)
		{
			return Start(TimeSpan.FromSeconds(seconds), priority, callback);
		}

		/// <summary>Schedules a new event with the specified callback.</summary>
		public ScheduledEvent Start(TimeSpan delay, TimerPriority priority,
			Action<ScheduledEvent> callback)
		{
			var timer = new ScheduledEvent(delay, priority, OnRemoveScheduledEvent);
			timer.Cancelled += OnRemoveScheduledEvent;
			timer.SenderCallback += new ScheduledEvent.ScheduledEventHandler(callback);
			timers.Add(timer);
			globalTimers.Add(timer);
			return timer;
		}

		// Sender Callback (with tag) -------------------------------------------------
		
		/// <summary>Schedules a new event with the specified callback and tag.</summary>
		public ScheduledEvent Start(double seconds, object tag,
			Action<ScheduledEvent> callback)
		{
			return Start(TimeSpan.FromSeconds(seconds), defaultPriority, tag,
				callback);
		}

		/// <summary>Schedules a new event with the specified callback and tag.</summary>
		public ScheduledEvent Start(TimeSpan delay, object tag,
			Action<ScheduledEvent> callback)
		{
			return Start(delay, defaultPriority, tag, callback);
		}

		/// <summary>Schedules a new event with the specified callback and tag.</summary>
		public ScheduledEvent Start(double seconds, TimerPriority priority,
			object tag, Action<ScheduledEvent> callback)
		{
			return Start(TimeSpan.FromSeconds(seconds), priority, tag, callback);
		}

		/// <summary>Schedules a new event with the specified callback and tag.</summary>
		public ScheduledEvent Start(TimeSpan delay, TimerPriority priority,
			object tag, Action<ScheduledEvent> callback)
		{
			var timer = new ScheduledEvent(delay, priority, OnRemoveScheduledEvent);
			timer.Cancelled += OnRemoveScheduledEvent;
			timer.Tag = tag;
			timer.SenderCallback += new ScheduledEvent.ScheduledEventHandler(callback);
			timers.Add(timer);
			globalTimers.Add(timer);
			return timer;
		}


		//-----------------------------------------------------------------------------
		// Event Handlers
		//-----------------------------------------------------------------------------

		/// <summary>Sets up the window closing event.</summary>
		private void OnLoaded(object sender, RoutedEventArgs e) {
			if (element != null) {
				Window newWindow = Window.GetWindow(element);
				if (newWindow != window) {
					if (window != null)
						window.Closing -= OnClosing;
					window = newWindow;
					window.Closing += OnClosing;
				}
			}
		}

		/// <summary>Cancels all events when the element is unloaded.
		/// And removes the closing event from the window.</summary>
		private void OnUnloaded(object sender, RoutedEventArgs e) {
			if (window != null) {
				window.Closing -= OnClosing;
				window = null;
			}
			CancelAll();
		}

		/// <summary>Cancels all events when the window is closing.</summary>
		private void OnClosing(object sender, CancelEventArgs e) {
			window.Closing -= OnClosing;
			window = null;
			CancelAll();
		}

		/// <summary>Cancels all events when the application is shutting down.</summary>
		private static void OnExit(object sender, ExitEventArgs e) {
			Application.Current.Exit -= OnExit;
			GlobalCancelAll();
		}

		/// <summary>Called to remove the timer from the cancel collection.</summary>
		private void OnRemoveScheduledEvent(ScheduledEvent sender) {
			timers.Remove(sender);
			globalTimers.Remove(sender);
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets the number of active timers in the event collection.</summary>
		public int Count {
			get { return timers.Count; }
		}

		/// <summary>Gets or sets the default priority for new scheduled events.</summary>
		public TimerPriority DefaultPriority {
			get { return defaultPriority; }
			set { defaultPriority = value; }
		}
	}
}
