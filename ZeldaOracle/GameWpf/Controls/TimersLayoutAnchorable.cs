using Xceed.Wpf.AvalonDock.Layout;
using ZeldaWpf.Util;

namespace ZeldaWpf.Controls {
	/// <summary>A layout anchorable that comes pre-setup with a timer event handlers.</summary>
	public class TimersLayoutAnchorable : LayoutAnchorable {

		/// <summary>Gets the continuous events for this layout anchorable.</summary>
		public ContinuousEvents ContinuousEvents { get; }

		/// <summary>Gets the scheduled events for this layout anchorable.</summary>
		public ScheduledEvents ScheduledEvents { get; }

		/// <summary>Constructs the timer events layout anchorable.</summary>
		public TimersLayoutAnchorable() {
			ContinuousEvents = new ContinuousEvents(this);
			ScheduledEvents = new ScheduledEvents(this);
		}
	}
}
