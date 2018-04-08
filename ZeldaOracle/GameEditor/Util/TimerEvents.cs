using System.Windows.Threading;

namespace ZeldaEditor.Util {
	/// <summary>Simplified piorities used for UI timers.</summary>
	public enum TimerPriority {
		Low = DispatcherPriority.ApplicationIdle,
		Normal = DispatcherPriority.Normal,
		High = DispatcherPriority.Render,
		VeryHigh = DispatcherPriority.Send,
	}

	/// <summary>A static class for managing continuous and scheduled events.</summary>
	public static class TimerEvents {

		/// <summary>Pauses all continuous and scheduled events.</summary>
		public static void PauseAll() {
			ContinuousEvents.GlobalPauseAll();
			ScheduledEvents.GlobalPauseAll();
		}

		/// <summary>Resumes all continuous and scheduled events.</summary>
		public static void ResumeAll() {
			ContinuousEvents.GlobalResumeAll();
			ScheduledEvents.GlobalResumeAll();
		}

		/// <summary>Permanently cancels all continuous and scheduled events.</summary>
		public static void CancelAll() {
			ContinuousEvents.GlobalCancelAll();
			ScheduledEvents.GlobalCancelAll();
		}
	}
}
