using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Control;
using ZeldaOracle.Game.Entities;

namespace ZeldaOracle.Game.Items.Rewards {
	/// <summary>The manager and container for all available rewards in the game.</summary>
	public class RewardManager {

		/// <summary>The game control running the game. This may be null in editors.</summary>
		private GameControl gameControl;
		/// <summary>The inventory for the game. This will not be null in editors.</summary>
		private Inventory inventory;
		/// <summary>The collection of rewards.</summary>
		private Dictionary<string, Reward> rewards;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		/// <summary>Constructs the in-game reward manager.</summary>
		public RewardManager(GameControl gameControl) {
			this.gameControl	= gameControl;
			this.inventory		= gameControl.Inventory;
			this.rewards		= new Dictionary<string, Reward>();
		}

		/// <summary>Constructs the in-editor reward manager.</summary>
		public RewardManager(Inventory inventory) {
			this.gameControl	= null;
			this.inventory		= inventory;
			this.rewards		= new Dictionary<string, Reward>();
		}


		//-----------------------------------------------------------------------------
		// Rewards
		//-----------------------------------------------------------------------------

		public Collectible SpawnCollectibleFromBreakableTile(Reward reward, Point2I position) {
			Collectible collectible = new CollectibleReward(reward);
			gameControl.RoomControl.SpawnEntity(collectible);
			collectible.Position = position;
			collectible.Physics.ZVelocity = 1.5f;
			return collectible;
		}
		public Collectible SpawnCollectibleFromBreakableTile(string id, Point2I position) {
			return SpawnCollectibleFromBreakableTile(GetReward(id), position);
		}

		public Collectible SpawnCollectible(Reward reward) {
			Collectible collectible = new CollectibleReward(reward);
			gameControl.RoomControl.SpawnEntity(collectible);
			return collectible;
		}

		public Collectible SpawnCollectible(string id) {
			return SpawnCollectible(rewards[id]);
		}

		public Reward AddReward(Reward reward) {
			if (!rewards.ContainsKey(reward.ID)) {
				rewards.Add(reward.ID, reward);
				reward.RewardManager = this;
			}
			return rewards[reward.ID];
		}

		public void AddRewardItem(string itemID) {
			AddRewardItem(inventory.GetItem(itemID));
		}

		public void AddRewardItem(Item item) {
			if (item.IsLeveled) {
				AddReward(new LinkedRewardItemCurrent(item));
				AddReward(new LinkedRewardItemLevelUp(item));
			}
			foreach (int level in item.GetLevels()) {
				AddReward(new RewardItem(item, level));
			}
		}

		public Reward GetReward(string id) {
			Reward reward;
			rewards.TryGetValue(id, out reward);
			// Only return LinkedRewards when in-game
			if (reward is LinkedReward && gameControl != null) {
				return ((LinkedReward) reward).Reward;
			}
			return reward;
		}

		public bool HasReward(string id) {
			return rewards.ContainsKey(id);
		}

		public IEnumerable<Reward> GetRewards() {
			foreach (var pair in rewards) {
				yield return pair.Value;
			}
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------
		
		public Dictionary<string, Reward> RewardDictionary {
			get { return rewards; }
		}

		/// <summary>Reference to the current game control.</summary>
		public GameControl GameControl {
			get { return gameControl; }
		}

		/// <summary>Reference to the game's inventory system.</summary>
		public Inventory Inventory {
			get { return inventory; }
		}
	}
}
