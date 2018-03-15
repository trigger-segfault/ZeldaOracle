using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeldaOracle.Game.Items {
	/// <summary>How the player visually holds the reward when collected.</summary>
	public enum RewardHoldTypes {
		/// <summary>The reward is held offset to the side in one hand.</summary>
		OneHand,
		/// <summary>The reward is held in the center in both hands.</summary>
		TwoHands,
		/// <summary>The player spins the sword then raises it with one hand.</summary>
		Sword,
	}
}
