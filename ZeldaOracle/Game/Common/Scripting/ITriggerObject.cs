using System;
using ZeldaOracle.Game.Control;

namespace ZeldaOracle.Common.Scripting {
	public interface IApiObject : IEventObject {
		Type ApiType { get; }
	}

	public interface ITriggerObject : IEventObject {
		TriggerCollection Triggers { get; }
		Type TriggerObjectType { get; }
	}
}
