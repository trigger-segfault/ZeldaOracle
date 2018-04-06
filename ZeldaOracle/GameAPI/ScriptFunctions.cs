
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game;

namespace ZeldaAPI {
	public interface ScriptFunctions {
		ScriptFunctionsUnit Unit { get; }
		ScriptFunctionsEntity Entity { get; }
		ScriptFunctionsTile Tile { get; }
		ScriptFunctionsSound Sound { get; }
		ScriptFunctionsMusic Music { get; }
		//ScriptFunctionsItem Item { get; }
		ScriptFunctionsReward Reward { get; }
		ScriptFunctionsTrigger Trigger { get; }
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

	public interface ScriptFunctionsTrigger {
		Trigger TriggerByName(ApiObject obj, string name);
		/// <summary>The trigger that is currently executing.</summary>
		Trigger ThisTrigger { get; }
	}

	// Some brainstorming below...

	//public enum UnitEvent {
	//	Dies,
	//	Spawns,
	//	IsDestroyed,
	//	IsDamaged,
	//	IsKnockedBack,
	//}

	//public enum EntityEvent {
	//	IsSpawned,
	//	IsDestroyed,
	//}
	
	//public interface ScriptEvents {
		//void RoomTransition(Room room, Direction direction);
		//void UnitEvent(Unit unit, UnitEvent unitEvent);
		//void EntityEvent(Entity entity, EntityEvent entityEvent)
	//}
}
