using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;

namespace ZeldaAPI {

	public interface TriggerCollection {
	}

	//public interface VariableCollection {
	//}

	public interface ApiObject {
		//TriggerCollection Triggers { get; }
		//VariableCollection Variables { get; }
	}


	/// <summary>Access to an entity.</summary>
	public interface Entity : ApiObject {

		string ID { get; }
		Vector2F Position { get; set; }
		float ZPosition { get; set; }
	}

	public interface Unit : Entity {
	}

	public interface NPC : Unit {
		string Text { get; set; }
	}

	/// <summary>The player entity.</summary>
	public interface Player : Unit {
	}

	public interface Sound {
	}

	public interface Music {
	}

	public interface Reward {
		string ID { get; }
	}
}
