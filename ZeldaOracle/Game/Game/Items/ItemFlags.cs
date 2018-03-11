using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeldaOracle.Game.Items {
	/// <summary>Flags for how an item can be used.</summary>
	[Flags]
	public enum ItemFlags {
		/// <summary>No item flags are set.</summary>
		None = 0,

		/// <summary>Item takes up both A and B weapon slots.</summary>
		TwoHanded = (1 << 0),

		/// <summary>Item can be used while riding a minecart.</summary>
		UsableInMinecart = (1 << 1),

		/// <summary>Item can be used while jumping.</summary>
		UsableWhileJumping = (1 << 2),

		/// <summary>Item can be used while holding the sword.</summary>
		UsableWithSword = (1 << 3),

		/// <summary>Item can be used while underwater (top-down or side-scrolling).</summary>
		UsableUnderwater = (1 << 4),

		/// <summary>Item can be used while gravitating towards a hole.</summary>
		UsableWhileInHole = (1 << 5),
	}
}
