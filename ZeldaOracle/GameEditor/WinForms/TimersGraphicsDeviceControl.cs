using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ZeldaEditor.Util;

namespace ZeldaEditor.WinForms {
	public class TimersGraphicsDeviceControl : GraphicsDeviceControl {

		/// <summary>Gets the continuous events for this control.</summary>
		public ContinuousEvents ContinuousEvents { get; }

		/// <summary>Gets the scheduled events for this control.</summary>
		public ScheduledEvents ScheduledEvents { get; }

		public TimersGraphicsDeviceControl(FrameworkElement element) {
			ContinuousEvents = new ContinuousEvents(element);
			ScheduledEvents = new ScheduledEvents(element);
		}
	}
}
