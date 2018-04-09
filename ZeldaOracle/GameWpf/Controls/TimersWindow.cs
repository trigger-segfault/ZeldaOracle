using System.Windows;
using ZeldaWpf.Util;

namespace ZeldaWpf.Controls {
	/// <summary>A window that comes pre-setup with a timer event handlers.</summary>
	public class TimersWindow : Window {

		/// <summary>Gets the continuous events for this window.</summary>
		public ContinuousEvents ContinuousEvents { get; }

		/// <summary>Gets the scheduled events for this window.</summary>
		public ScheduledEvents ScheduledEvents { get; }

		/// <summary>Constructs the timer events window.</summary>
		public TimersWindow() {
			ContinuousEvents = new ContinuousEvents(this);
			ScheduledEvents = new ScheduledEvents(this);
		}
	}
}
