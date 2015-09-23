using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Game.Control;

namespace ZeldaOracle.Game.Items.Rewards {
	public class RewardManager {

		private static GameControl gameControl;
		private static Dictionary<string, Reward> rewards;

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public static void Initialize(GameControl gameControl) {
			RewardManager.gameControl	= gameControl;
			RewardManager.rewards		= new Dictionary<string, Reward>();
		}
	}
}
