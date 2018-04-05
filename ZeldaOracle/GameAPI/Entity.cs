using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;

namespace ZeldaAPI {

	/// <summary>The base interface for all objects in the API.</summary>
	public interface TriggerCollection {
	}

	public interface ApiObject {
		/// <summary>Gets the variables for the API Object.</summary>
		Variables Vars { get; }
		//TriggerCollection Triggers { get; }
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
