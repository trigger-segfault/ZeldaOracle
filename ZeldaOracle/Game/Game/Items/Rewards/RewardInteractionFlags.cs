using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeldaOracle.Game.Items.Rewards {
	// TODO: Switch Reward.InteractWithWeapons to this.
	// This is just extra fluff and is unneccissary if faithfully reproducing the game.
	// It would be helpful if the user wanted to implement Boomerange+Switch hooking
	// of Pieces of Heart like in 3D Zelda titles.
	/// <summary>Flags for how a reward interacts with weapons.</summary>
	[Flags]
	public enum RewardInteractionFlags {
		/// <summary>The reward does not interact with weapons at all.</summary>
		None = 0,

		/// <summary>The reward still draws when being returned by the switch hook
		/// or boomerang.</summary>
		DrawOnReturn = (1 << 0),

		/// <summary>The reward can be collected wen colliding with the sword.</summary>
		Sword = (1 << 1),

		/// <summary>The reward can be picked up with the boomerang.</summary>
		Boomerang = (1 << 2),

		/// <summary>The reward can be picked up with the switch hook.</summary>
		SwitchHook = (1 << 3),
	}
}
