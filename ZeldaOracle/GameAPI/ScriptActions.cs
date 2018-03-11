
namespace ZeldaAPI {
	/// <summary>Returns true if a condition is met.</summary>
	public delegate bool WaitCondition();

	/// <summary>Provides the actions to perform from within a script.</summary>
	public interface ScriptActions {
		/// <summary>Wait for the given condition to become true.</summary>
		void WaitForCondition(WaitCondition condition);
		/// <summary>Wait the given number of ticks.</summary>
		void Wait(int ticks);
		void Message(string text);
	}
}
