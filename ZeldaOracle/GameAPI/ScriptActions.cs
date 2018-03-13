
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;

namespace ZeldaAPI {
	/// <summary>Returns true if a condition is met.</summary>
	public delegate bool WaitCondition();
	public delegate bool UpdateCondition();
	public delegate bool TimedUpdateCondition(int elapsedTime);

	public interface ScriptAction {
		void Wait(bool wait = true);
	}

	/// <summary>Provides the actions to perform from within a script.</summary>
	public interface ScriptActions {
		ScriptActionsGeneral General { get; }
		ScriptActionsCamera Camera { get; }
		ScriptActionsUnit Unit { get; }
		ScriptActionsSound Sound { get; }
		ScriptActionsItem Item { get; }
	}

	public interface ScriptFunctions {
		ScriptFunctionsUnit Unit { get; }
		ScriptFunctionsEntity Entity { get; }
		ScriptFunctionsTile Tile { get; }
		ScriptFunctionsSound Sound { get; }
		ScriptFunctionsMusic Music { get; }
		//ScriptFunctionsItem Item { get; }
		ScriptFunctionsReward Reward { get; }
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
		
		void FadeScreenOut(Color color, int duration);
		void FadeScreenIn(Color color, int duration);
	}
	
	public interface ScriptActionsSound {
		void PlaySound(Sound sound);
		void PlayMusic(Music music);
		void StopMusic();
	}
	
	public interface ScriptActionsCamera {
		void SetCameraTargetToEntity(Entity entity);
		void SetCameraTargetToPoint(Vector2F point);
		void ResetCamera();
	}
	
	public interface ScriptActionsUnit {
		ScriptAction MoveInDirection(Unit unit, Direction direction, Distance distance, float speed);
		ScriptAction MoveToPoint(Unit unit, Vector2F point, float speed);
		void MakeUnitFaceDirection(Unit unit, Direction direction);
		//void MakeUnitFacePoint(Unit unit, Vector2F point);
		void Destroy(Unit unit);
		void Kill(Unit unit);
		void Jump(Unit unit, float jumpSpeed);
	}
	
	public interface ScriptActionsItem {
		void GiveReward(Reward reward);
	}



	public interface ScriptFunctionsUnit {
		Unit UnitByID(string id);
	}

	public interface ScriptFunctionsEntity {
		Entity EntityByID(string id);
	}

	public interface ScriptFunctionsTile {
	}

	public interface ScriptFunctionsSound {
		Sound SoundByID(string id);
	}

	public interface ScriptFunctionsMusic {
		Music MusicByID(string id);
	}

	public interface ScriptFunctionsReward {
		Reward RewardByID(string id);
	}

}
