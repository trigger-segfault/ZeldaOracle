using System.Windows.Controls;
using ZeldaEditor.Util;

namespace ZeldaEditor.Controls {
	/// <summary>A user control that comes pre-setup with a timer event handlers.</summary>
	public class TimersUserControl : UserControl {

		/// <summary>Gets the continuous events for this user control.</summary>
		public ContinuousEvents ContinuousEvents { get; }

		/// <summary>Gets the scheduled events for this user control.</summary>
		public ScheduledEvents ScheduledEvents { get; }

		/// <summary>Constructs the timer events user control.</summary>
		public TimersUserControl() {
			ContinuousEvents = new ContinuousEvents(this);
			ScheduledEvents = new ScheduledEvents(this);
		}
	}
}
