
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game;

namespace ZeldaAPI {
	/// <summary>Returns true if a condition is met.</summary>
	public delegate bool WaitCondition();
	public delegate bool UpdateCondition();
	public delegate bool TimedUpdateCondition(int elapsedTime);

	public interface Trigger {
		/// <summary>Turn on the trigger, so it runs when any of its events are fired.
		/// </summary>
		void TurnOn();
		/// <summary>Turn on the trigger, so it does not runs when any of its events
		/// are fired.</summary>
		void TurnOff();
	}

	public interface ScriptAction {
		/// <summary>Wait for the action to complete before continuing.
		/// This is a blocking function.</summary>
		void Wait(bool wait = true);
	}

	/// <summary>Provides the actions to perform from within a script.</summary>
	public interface ScriptActions {
		ScriptActionsGeneral General { get; }
		ScriptActionsCamera Camera { get; }
		ScriptActionsUnit Unit { get; }
		ScriptActionsSound Sound { get; }
		ScriptActionsItem Item { get; }
		ScriptActionsTile Tile { get; }
		ScriptActionsTrigger Trigger { get; }
	}
	
	public interface ScriptActionsGeneral {
		/// <summary>Wait for the given condition to become true.</summary>
		void WaitForCondition(WaitCondition condition);
		
		/// <summary>Wait the given number of ticks.</summary>
		void Wait(int ticks);
		
		/// <summary>Wait the given number of seconds.</summary>
		void WaitSeconds(float seconds);

		/// <summary>Display a text message on the screen.</summary>
		/// <param name="text">The text to display.</param>
		/// <returns>The selected option index or -1 if there where no options.</returns>
		int Message(string text);

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
		/// <summary>Set the camera's target to an entity.</summary>
		/// <param name="entity">The target entity, which the camera will follow.</param>
		void SetCameraTargetToEntity(Entity entity);
		
		/// <summary>Set the camera's target to an entity.</summary>
		/// <param name="point">The target point, which the camera will focus on.</param>
		void SetCameraTargetToPoint(Vector2F point);
		
		/// <summary>Reset the camera state so that it follows the player.</summary>
		void ResetCamera();
	}
	
	public interface ScriptActionsUnit {
		/// <summary>Order a unit to move a specified distance in a direction.</summary>
		/// <param name="unit">The unit to move.</param>
		/// <param name="direction">The direction to move in.</param>
		/// <param name="distance">The distance in pixels to move.</param>
		/// <param name="speed">The unit's movement speed in pixels/tick.</param>
		/// <returns>The created action.</returns>
		ScriptAction MoveInDirection(Unit unit, Direction direction, Distance distance, float speed);
		
		/// <summary>Order a unit to move to the specified point.</summary>
		/// <param name="unit">The unit to move.</param>
		/// <param name="point">The point to move to.</param>
		/// <param name="speed">The unit's movement speed in pixels/tick.</param>
		/// <returns>The created action.</returns>
		ScriptAction MoveToPoint(Unit unit, Vector2F point, float speed);
		
		/// <summary>Make a unit face a direction.</summary>
		/// <param name="unit">The unit whose direction will change.</param>
		/// <param name="direction">The direction the unit should be facing.</param>
		void MakeUnitFaceDirection(Unit unit, Direction direction);
		
		//void MakeUnitFacePoint(Unit unit, Vector2F point);
		
		/// <summary>Destroy a unit, instantly removing it from the room.</summary>
		/// <param name="unit">The unit to destroy.</param>
		void Destroy(Unit unit);

		/// <summary>Kill a unit, spawning its death effect.</summary>
		/// <param name="unit">The unit to kill.</param>
		void Kill(Unit unit);

		/// <summary>Order a unit to jump in the air.</summary>
		/// <param name="unit">The unit to jump.</param>
		/// <param name="jumpSpeed">The z-speed in pixels/tick to jump at.</param>
		void Jump(Unit unit, float jumpSpeed);
	}
	
	public interface ScriptActionsItem {
		void GiveReward(Reward reward);
	}
	
	public interface ScriptActionsTile {
		void LightLantern(Lantern lantern);
		void PutOutLantern(Lantern lantern);
		void FlipSwitch(Lever lever);
		void SetColor(ColorTile tile, PuzzleColor color);
		//void Rotate(ColorTile tile, PuzzleColor color);
		void BuildBridge(Bridge bridge);
		void DestroyBridge(Bridge bridge);
	}

	public interface ScriptActionsTrigger {
		void Run(Trigger trigger);
		void TurnOn(Trigger trigger);
		void TurnOff(Trigger trigger);
	}
}
