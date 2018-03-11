using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeldaAPI {

	/// <summary>Access to an entity.</summary>
	public interface Entity {
		// Nothing yet...
	}

	public interface Unit : Entity {
	}

	/// <summary>The player entity.</summary>
	public interface Player : Unit {
	}

}
