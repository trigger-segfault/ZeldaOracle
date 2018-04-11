using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace ZeldaWpf.Util {
	/// <summary>An event that fires continuously on the UI thread.</summary>
	public class ContinuousEvent {

		//-----------------------------------------------------------------------------
		// Delegates
		//-----------------------------------------------------------------------------

		/// <summary>A simple event action that has no arguments.</summary>
		public delegate void ContinuousActionHandler();
		/// <summary>A simple event callback that gives the continous event.</summary>
		public delegate void ContinuousEventHandler(ContinuousEvent sender);


		//-----------------------------------------------------------------------------
		// Members
		//-----------------------------------------------------------------------------

		/// <summary>The timer running this event.</summary>
		private DispatcherTimer timer;
		/// <summary>The tag for extra event data.</summary>
		private object tag;
		/// <summary>True if the event has been paused.</summary>
		private bool paused;
		/// <summary>True if the event has been permanently cancelled.</summary>
		private bool cancelled;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Constructs a continous event with no action or callback.</summary>
		public ContinuousEvent(TimeSpan interval, TimerPriority priority) {
			timer = new DispatcherTimer(interval, (DispatcherPriority) priority,
				OnTimerFinished, Application.Current.Dispatcher);
		}

		/// <summary>Constructs a continous event with the specified action.</summary>
		public ContinuousEvent(TimeSpan interval, TimerPriority priority,
			ContinuousActionHandler callback)
		{
			timer = new DispatcherTimer(interval, (DispatcherPriority) priority,
				OnTimerFinished, Application.Current.Dispatcher);
			ActionCallback += callback;
		}

		/// <summary>Constructs a continous event with the specified callback.</summary>
		public ContinuousEvent(TimeSpan interval, TimerPriority priority,
			ContinuousEventHandler callback)
		{
			timer = new DispatcherTimer(interval, (DispatcherPriority) priority,
				OnTimerFinished, Application.Current.Dispatcher);
			SenderCallback += callback;
		}

		/// <summary>Constructs a continous event with the specified callback and tag.</summary>
		public ContinuousEvent(TimeSpan interval, TimerPriority priority, object tag,
			ContinuousEventHandler callback)
		{
			timer = new DispatcherTimer(interval, (DispatcherPriority) priority,
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
		}


		//-----------------------------------------------------------------------------
		// Mutators
		//-----------------------------------------------------------------------------

		/// <summary>Starts the event.</summary>
		public void Start() {
			if (cancelled)
				throw new InvalidOperationException("Cannot start a cancelled " +
					"continuous event!");
			if (!timer.IsEnabled) {
				paused = false;
				timer.Start();
			}
		}

		/// <summary>Stops the event.</summary>
		public void Stop() {
			if (timer.IsEnabled) {
				timer.Stop();
			}
		}

		/// <summary>Resumes the paused event.</summary>
		public void Resume() {
			if (cancelled)
				throw new InvalidOperationException("Cannot resume a cancelled " +
					"continuous event!");
			if (!timer.IsEnabled && paused) {
				cancelled = false;
				timer.Start();
			}
		}

		/// <summary>Pauses the event.</summary>
		public void Pause() {
			if (timer.IsEnabled) {
				paused = true;
				timer.Stop();
			}
		}

		/// <summary>Permanently cancels the event.</summary>
		public void Cancel() {
			cancelled = true;
			paused = false;
			timer.Stop();
			Cancelled?.Invoke(this);
		}


		//-----------------------------------------------------------------------------
		// Events
		//-----------------------------------------------------------------------------

		/// <summary>An event handler that returns no arguments on callback.</summary>
		public event ContinuousActionHandler ActionCallback;

		/// <summary>An event handler that returns the sender on callback.</summary>
		public event ContinuousEventHandler SenderCallback;

		/// <summary>An event handler thats called when canceled.</summary>
		public event ContinuousEventHandler Cancelled;


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets if the timer is currently running.</summary>
		public bool IsRunning {
			get { return timer.IsEnabled; }
		}

		/// <summary>Gets if the timer is paused.</summary>
		public bool IsPaused {
			get { return cancelled; }
		}

		/// <summary>Gets if the timer is permanently cancelled.</summary>
		public bool IsCancelled {
			get { return cancelled; }
		}

		/// <summary>Gets the interval for the event.</summary>
		public TimeSpan Interval {
			get { return timer.Interval; }
		}

		/// <summary>Gets or sets the tag to return in the event arguments.</summary>
		public object Tag {
			get { return tag; }
			set { tag = value; }
		}
	}

	/// <summary>A collection of continuous events that can all be canceled.
	/// Best use is to attach it to the FrameworkElement its associated with.</summary>
	public class ContinuousEvents {

		//-----------------------------------------------------------------------------
		// Constants
		//-----------------------------------------------------------------------------

		/// <summary>The required timer interval to hit at least 60FPS.</summary>
		public static readonly TimeSpan RenderInterval =
			TimeSpan.FromMilliseconds(15);

		/// <summary>The required timer priority to hit at least 60FPS.</summary>
		public const TimerPriority RenderPriority = TimerPriority.High;


		//-----------------------------------------------------------------------------
		// Members
		//-----------------------------------------------------------------------------

		/// <summary>The global collection of cancellable timers.</summary>
		private static HashSet<ContinuousEvent> globalTimers;

		/// <summary>The collection of cancellable timers.</summary>
		private HashSet<ContinuousEvent> timers;
		/// <summary>The element containing this event collection.</summary>
		private FrameworkElement element;
		/// <summary>The window containing this event collection.</summary>
		private Window window;
		/// <summary>The default priority for continuous events.</summary>
		private TimerPriority defaultPriority;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Initializes the global timers collection.</summary>
		static ContinuousEvents() {
			globalTimers = new HashSet<ContinuousEvent>();
			Application.Current.Exit += OnExit;
		}

		/// <summary>Constructs continuous events that only cancel on application exit.</summary>
		public ContinuousEvents() {
			timers = new HashSet<ContinuousEvent>();
			defaultPriority = TimerPriority.Normal;
		}

		/// <summary>Constructs continuous events that cancel on element unloading and
		/// window closed.</summary>
		public ContinuousEvents(FrameworkElement element) : this() {
			if (element is Window) {
				window = (Window) element;
				window.Closed += OnClosed;
			}
			else {
				this.element = element;
				element.Loaded += OnLoaded;
				element.Unloaded += OnUnloaded;
			}
		}

		/// <summary>Constructs continuous events that cancel on window closed.</summary>
		public ContinuousEvents(Window window) : this() {
			this.window = window;
			window.Closed += OnClosed;
		}


		//-----------------------------------------------------------------------------
		// Cancelling
		//-----------------------------------------------------------------------------

		/// <summary>Pauses all global timers.</summary>
		public static void GlobalPauseAll() {
			foreach (ContinuousEvent timer in globalTimers) {
				timer.Pause();
			}
		}

		/// <summary>Resumes all global timers.</summary>
		public static void GlobalResumeAll() {
			foreach (ContinuousEvent timer in globalTimers) {
				timer.Resume();
			}
		}

		/// <summary>Permanently cancels all global timers.</summary>
		public static void GlobalCancelAll() {
			foreach (ContinuousEvent timer in globalTimers.ToArray()) {
				timer.Cancel();
			}
		}

		/// <summary>Pauses all timers.</summary>
		public void PauseAll() {
			foreach (ContinuousEvent timer in timers) {
				timer.Pause();
			}
		}

		/// <summary>Resumes all cancelled timers.</summary>
		public void ResumeAll() {
			foreach (ContinuousEvent timer in timers) {
				timer.Resume();
			}
		}

		/// <summary>Permanently cancels all timers.</summary>
		public void CancelAll() {
			foreach (ContinuousEvent timer in timers.ToArray()) {
				timer.Cancel();
			}
		}


		//-----------------------------------------------------------------------------
		// New
		//-----------------------------------------------------------------------------

		// No Callback ----------------------------------------------------------------

		/// <summary>Creates a new continuous event with no action or callback.</summary>
		public ContinuousEvent New(double seconds) {
			return New(TimeSpan.FromSeconds(seconds), defaultPriority);
		}

		/// <summary>Creates a new continuous event with no action or callback.</summary>
		public ContinuousEvent New(TimeSpan interval) {
			return New(interval, defaultPriority);
		}

		/// <summary>Creates a new continuous event with no action or callback.</summary>
		public ContinuousEvent New(double seconds, TimerPriority priority) {
			return New(TimeSpan.FromSeconds(seconds), priority);
		}

		/// <summary>Creates a new continuous event with no action or callback.</summary>
		public ContinuousEvent New(TimeSpan interval, TimerPriority priority) {
			var timer = new ContinuousEvent(interval, priority);
			timer.Cancelled += OnRemoveContinuousEvent;
			timer.Stop();
			timers.Add(timer);
			globalTimers.Add(timer);
			return timer;
		}

		// Action Callback ------------------------------------------------------------

		/// <summary>Creates a new continuous new event with the specified action.</summary>
		public ContinuousEvent New(double seconds, Action action) {
			return New(TimeSpan.FromSeconds(seconds), defaultPriority, action);
		}

		/// <summary>Creates a new continuous new event with the specified action.</summary>
		public ContinuousEvent New(TimeSpan interval, Action action) {
			return New(interval, defaultPriority, action);
		}

		/// <summary>Creates a new continuous new event with the specified action.</summary>
		public ContinuousEvent New(double seconds, TimerPriority priority,
			Action action)
		{
			return Start(TimeSpan.FromSeconds(seconds), priority, action);
		}

		/// <summary>Creates a new continuous new event with the specified action.</summary>
		public ContinuousEvent New(TimeSpan interval, TimerPriority priority,
			Action callback)
		{
			var timer = new ContinuousEvent(interval, priority);
			timer.Cancelled += OnRemoveContinuousEvent;
			timer.ActionCallback +=
				new ContinuousEvent.ContinuousActionHandler(callback);
			timer.Stop();
			timers.Add(timer);
			globalTimers.Add(timer);
			return timer;
		}

		// Sender Callback ------------------------------------------------------------

		/// <summary>Creates a new continuous new event with the specified callback.</summary>
		public ContinuousEvent New(double seconds,
			Action<ContinuousEvent> callback)
		{
			return New(TimeSpan.FromSeconds(seconds), defaultPriority, callback);
		}

		/// <summary>Creates a new continuous new event with the specified callback.</summary>
		public ContinuousEvent New(TimeSpan interval,
			Action<ContinuousEvent> callback)
		{
			return New(interval, defaultPriority, callback);
		}

		/// <summary>Creates a new continuous new event with the specified callback.</summary>
		public ContinuousEvent New(double seconds, TimerPriority priority,
			Action<ContinuousEvent> callback)
		{
			return New(TimeSpan.FromSeconds(seconds), priority, callback);
		}

		/// <summary>Creates a new continuous new event with the specified callback.</summary>
		public ContinuousEvent New(TimeSpan interval, TimerPriority priority,
			Action<ContinuousEvent> callback)
		{
			var timer = new ContinuousEvent(interval, priority);
			timer.Cancelled += OnRemoveContinuousEvent;
			timer.SenderCallback +=
				new ContinuousEvent.ContinuousEventHandler(callback);
			timer.Stop();
			timers.Add(timer);
			globalTimers.Add(timer);
			return timer;
		}

		// Sender Callback (with tag) -------------------------------------------------

		/// <summary>Creates a new continuous new event with the specified callback and
		/// tag.</summary>
		public ContinuousEvent New(double seconds, object tag,
			Action<ContinuousEvent> callback)
		{
			return New(TimeSpan.FromSeconds(seconds), defaultPriority, tag, callback);
		}

		/// <summary>Creates a new continuous new event with the specified callback and
		/// tag.</summary>
		public ContinuousEvent New(TimeSpan interval, object tag,
			Action<ContinuousEvent> callback)
		{
			return New(interval, defaultPriority, tag, callback);
		}

		/// <summary>Creates a new continuous new event with the specified callback and
		/// tag.</summary>
		public ContinuousEvent New(double seconds, TimerPriority priority,
			object tag, Action<ContinuousEvent> callback)
		{
			return New(TimeSpan.FromSeconds(seconds), priority, tag, callback);
		}

		/// <summary>Creates a new continuous new event with the specified callback and
		/// tag.</summary>
		public ContinuousEvent New(TimeSpan interval, TimerPriority priority,
			object tag, Action<ContinuousEvent> callback)
		{
			var timer = new ContinuousEvent(interval, priority);
			timer.Cancelled += OnRemoveContinuousEvent;
			timer.Tag = tag;
			timer.SenderCallback +=
				new ContinuousEvent.ContinuousEventHandler(callback);
			timer.Stop();
			timers.Add(timer);
			globalTimers.Add(timer);
			return timer;
		}


		//-----------------------------------------------------------------------------
		// New Render
		//-----------------------------------------------------------------------------

		/// <summary>Creates a new continuous new event with the specified action.</summary>
		public ContinuousEvent NewRender(Action callback) {
			return New(RenderInterval, RenderPriority, callback);
		}

		// Sender Callback ------------------------------------------------------------

		/// <summary>Creates a new continuous new event with the specified callback.</summary>
		public ContinuousEvent NewRender(Action<ContinuousEvent> callback) {
			return New(RenderInterval, RenderPriority, callback);
		}

		// Sender Callback (with tag) -------------------------------------------------

		/// <summary>Creates a new continuous new event with the specified callback and
		/// tag.</summary>
		public ContinuousEvent NewRender(object tag,
			Action<ContinuousEvent> callback)
		{
			return New(RenderInterval, RenderPriority, tag, callback);
		}


		//-----------------------------------------------------------------------------
		// Starting
		//-----------------------------------------------------------------------------

		// No Callback ----------------------------------------------------------------

		/// <summary>Starts a new continuous event with no action or callback.</summary>
		public ContinuousEvent Start(double seconds) {
			return Start(TimeSpan.FromSeconds(seconds), defaultPriority);
		}

		/// <summary>Starts a new continuous event with no action or callback.</summary>
		public ContinuousEvent Start(TimeSpan interval) {
			return Start(interval, defaultPriority);
		}

		/// <summary>Starts a new continuous event with no action or callback.</summary>
		public ContinuousEvent Start(double seconds, TimerPriority priority) {
			return Start(TimeSpan.FromSeconds(seconds), priority);
		}

		/// <summary>Starts a new continuous event with no action or callback.</summary>
		public ContinuousEvent Start(TimeSpan interval, TimerPriority priority) {
			var timer = new ContinuousEvent(interval, priority);
			timer.Cancelled += OnRemoveContinuousEvent;
			timers.Add(timer);
			globalTimers.Add(timer);
			return timer;
		}

		// Action Callback ------------------------------------------------------------

		/// <summary>Starts a new continuous new event with the specified action.</summary>
		public ContinuousEvent Start(double seconds, Action action) {
			return Start(TimeSpan.FromSeconds(seconds), defaultPriority, action);
		}

		/// <summary>Starts a new continuous new event with the specified action.</summary>
		public ContinuousEvent Start(TimeSpan interval, Action action) {
			return Start(interval, defaultPriority, action);
		}

		/// <summary>Starts a new continuous new event with the specified action.</summary>
		public ContinuousEvent Start(double seconds, TimerPriority priority,
			Action action)
		{
			return Start(TimeSpan.FromSeconds(seconds), priority, action);
		}

		/// <summary>Starts a new continuous new event with the specified action.</summary>
		public ContinuousEvent Start(TimeSpan interval, TimerPriority priority,
			Action callback)
		{
			var timer = new ContinuousEvent(interval, priority);
			timer.Cancelled += OnRemoveContinuousEvent;
			timer.ActionCallback +=
				new ContinuousEvent.ContinuousActionHandler(callback);
			timers.Add(timer);
			globalTimers.Add(timer);
			return timer;
		}

		// Sender Callback ------------------------------------------------------------

		/// <summary>Starts a new continuous new event with the specified callback.</summary>
		public ContinuousEvent Start(double seconds,
			Action<ContinuousEvent> callback)
		{
			return Start(TimeSpan.FromSeconds(seconds), defaultPriority, callback);
		}

		/// <summary>Starts a new continuous new event with the specified callback.</summary>
		public ContinuousEvent Start(TimeSpan interval,
			Action<ContinuousEvent> callback)
		{
			return Start(interval, defaultPriority, callback);
		}

		/// <summary>Starts a new continuous new event with the specified callback.</summary>
		public ContinuousEvent Start(double seconds, TimerPriority priority,
			Action<ContinuousEvent> callback)
		{
			return Start(TimeSpan.FromSeconds(seconds), priority, callback);
		}

		/// <summary>Starts a new continuous new event with the specified callback.</summary>
		public ContinuousEvent Start(TimeSpan interval, TimerPriority priority,
			Action<ContinuousEvent> callback)
		{
			var timer = new ContinuousEvent(interval, priority);
			timer.Cancelled += OnRemoveContinuousEvent;
			timer.SenderCallback +=
				new ContinuousEvent.ContinuousEventHandler(callback);
			timers.Add(timer);
			globalTimers.Add(timer);
			return timer;
		}

		// Sender Callback (with tag) -------------------------------------------------

		/// <summary>Starts a new continuous new event with the specified callback and
		/// tag.</summary>
		public ContinuousEvent Start(double seconds, object tag,
			Action<ContinuousEvent> callback)
		{
			return Start(TimeSpan.FromSeconds(seconds), defaultPriority, tag,
				callback);
		}

		/// <summary>Starts a new continuous new event with the specified callback and
		/// tag.</summary>
		public ContinuousEvent Start(TimeSpan interval, object tag,
			Action<ContinuousEvent> callback)
		{
			return Start(interval, defaultPriority, tag, callback);
		}

		/// <summary>Starts a new continuous new event with the specified callback and
		/// tag.</summary>
		public ContinuousEvent Start(double seconds, TimerPriority priority,
			object tag, Action<ContinuousEvent> callback)
		{
			return Start(TimeSpan.FromSeconds(seconds), priority, tag, callback);
		}

		/// <summary>Starts a new continuous new event with the specified callback and
		/// tag.</summary>
		public ContinuousEvent Start(TimeSpan interval, TimerPriority priority,
			object tag, Action<ContinuousEvent> callback)
		{
			var timer = new ContinuousEvent(interval, priority);
			timer.Cancelled += OnRemoveContinuousEvent;
			timer.Tag = tag;
			timer.SenderCallback +=
				new ContinuousEvent.ContinuousEventHandler(callback);
			timers.Add(timer);
			globalTimers.Add(timer);
			return timer;
		}


		//-----------------------------------------------------------------------------
		// Starting Render
		//-----------------------------------------------------------------------------
		
		/// <summary>Starts a new continuous new event with the specified action.</summary>
		public ContinuousEvent StartRender(Action callback) {
			return Start(RenderInterval, RenderPriority, callback);
		}

		// Sender Callback ------------------------------------------------------------

		/// <summary>Starts a new continuous new event with the specified callback.</summary>
		public ContinuousEvent StartRender(Action<ContinuousEvent> callback) {
			return Start(RenderInterval, RenderPriority, callback);
		}

		// Sender Callback (with tag) -------------------------------------------------

		/// <summary>Starts a new continuous new event with the specified callback and
		/// tag.</summary>
		public ContinuousEvent StartRender(object tag,
			Action<ContinuousEvent> callback)
		{
			return Start(RenderInterval, RenderPriority, tag, callback);
		}


		//-----------------------------------------------------------------------------
		// Event Handlers
		//-----------------------------------------------------------------------------

		/// <summary>Sets up the window closed event.</summary>
		private void OnLoaded(object sender, RoutedEventArgs e) {
			if (element != null) {
				Window newWindow = Window.GetWindow(element);
				if (newWindow != window) {
					if (window != null)
						window.Closed -= OnClosed;
					window = newWindow;
					window.Closed += OnClosed;
				}
			}
		}

		/// <summary>Cancels all events when the element is unloaded.
		/// And removes the closed event from the window.</summary>
		private void OnUnloaded(object sender, RoutedEventArgs e) {
			if (window != null) {
				window.Closed -= OnClosed;
				window = null;
			}
			CancelAll();
		}

		/// <summary>Cancels all events when the window is closed.</summary>
		private void OnClosed(object sender, EventArgs e) {
			window.Closed -= OnClosed;
			window = null;
			CancelAll();
		}

		/// <summary>Cancels all events when the application is shutting down.</summary>
		private static void OnExit(object sender, ExitEventArgs e) {
			Application.Current.Exit -= OnExit;
			GlobalCancelAll();
		}

		/// <summary>Called to remove the timer from the cancel collection.</summary>
		private void OnRemoveContinuousEvent(ContinuousEvent sender) {
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

		/// <summary>Gets or sets the default priority for new continuous events.</summary>
		public TimerPriority DefaultPriority {
			get { return defaultPriority; }
			set { defaultPriority = value; }
		}
	}
}
