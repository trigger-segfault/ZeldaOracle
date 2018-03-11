
using ZeldaOracle.Common.Geometry;

namespace ZeldaAPI {
	/// <summary>Returns true if a condition is met.</summary>
	public delegate bool WaitCondition();

	/// <summary>Provides the actions to perform from within a script.</summary>
	public interface ScriptActions {
		ScriptActionsGeneral General { get; }
		ScriptActionsCamera Camera { get; }
		ScriptActionsUnit Unit { get; }
	}
	
	public interface ScriptActionsGeneral {
		/// <summary>Wait for the given condition to become true.</summary>
		void WaitForCondition(WaitCondition condition);
		/// <summary>Wait the given number of ticks.</summary>
		void Wait(int ticks);
		void WaitSeconds(float seconds);
		void Message(string text);
		void BeginCutscene();
		void EndCutscene();
	}
	
	public interface ScriptActionsCamera {
		void LockCameraTargetToEntity(Entity entity);
	}
	
	public interface ScriptActionsUnit {
		void Move(Unit unit, Direction direction, Distance distance, float speed);
		void MakeUnitFaceDirection(Unit unit, Direction direction);
		void Destroy(Unit unit);
		void Kill(Unit unit);
	}
}
