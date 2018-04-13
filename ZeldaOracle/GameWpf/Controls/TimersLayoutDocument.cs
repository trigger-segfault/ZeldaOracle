using Xceed.Wpf.AvalonDock.Layout;
using ZeldaWpf.Util;

namespace ZeldaWpf.Controls {
	/// <summary>A layout document that comes pre-setup with a timer event handlers.</summary>
	public class TimersLayoutDocument : LayoutAnchorable {

		/// <summary>Gets the continuous events for this layout document.</summary>
		public ContinuousEvents ContinuousEvents { get; }

		/// <summary>Gets the scheduled events for this layout document.</summary>
		public ScheduledEvents ScheduledEvents { get; }

		/// <summary>Constructs the timer events layout document.</summary>
		public TimersLayoutDocument() {
			ContinuousEvents = new ContinuousEvents(this);
			ScheduledEvents = new ScheduledEvents(this);
		}
	}
}
