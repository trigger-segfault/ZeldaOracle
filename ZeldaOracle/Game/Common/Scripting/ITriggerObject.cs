
namespace ZeldaOracle.Common.Scripting {
	public interface ITriggerObject : IEventObject {
		TriggerCollection Triggers { get; }
	}
}
