using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Control;
using ZeldaOracle.Game.Entities;

namespace ZeldaOracle.Game.Items.Rewards {
	public class RewardManager {

		private GameControl gameControl;
		private Dictionary<string, Reward> rewards;

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public RewardManager(GameControl gameControl) {
			this.gameControl	= gameControl;
			this.rewards		= new Dictionary<string, Reward>();
		}


		//-----------------------------------------------------------------------------
		// Rewards
		//-----------------------------------------------------------------------------

		public Collectible SpawnCollectibleFromBreakableTile(Reward reward, Point2I position) {
			Collectible collectible = new Collectible(reward);
			gameControl.RoomControl.SpawnEntity(collectible);
			collectible.Position = position;
			collectible.Physics.ZVelocity = 1.5f;
			return collectible;
		}
		public Collectible SpawnCollectibleFromBreakableTile(string id, Point2I position) {
			return SpawnCollectibleFromBreakableTile(rewards[id], position);
		}

		public Collectible SpawnCollectible(Reward reward) {
			Collectible collectible = new Collectible(reward);
			gameControl.RoomControl.SpawnEntity(collectible);
			return collectible;
		}

		public Collectible SpawnCollectible(string id) {
			return SpawnCollectible(rewards[id]);
		}

		public Reward AddReward(Reward reward) {
			if (!rewards.ContainsKey(reward.ID))
				rewards.Add(reward.ID, reward);
			return rewards[reward.ID];
		}

		public Reward GetReward(string id) {
			return rewards[id];
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

	}
}
