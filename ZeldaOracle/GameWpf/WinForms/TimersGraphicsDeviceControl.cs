using System.Windows;
using ZeldaWpf.Util;

namespace ZeldaWpf.WinForms {
	/// <summary>A graphics device control that employs the containment of timer
	/// events.</summary>
	public class TimersGraphicsDeviceControl : GraphicsDeviceControl {

		/// <summary>Gets the continuous events for this control.</summary>
		public ContinuousEvents ContinuousEvents { get; }

		/// <summary>Gets the scheduled events for this control.</summary>
		public ScheduledEvents ScheduledEvents { get; }

		/// <summary>Constructs the timers graphics control.</summary>
		public TimersGraphicsDeviceControl(FrameworkElement element) {
			ContinuousEvents = new ContinuousEvents(element);
			ScheduledEvents = new ScheduledEvents(element);
		}
	}
}
