using ZeldaOracle.Game.Items.Drops;
using ZeldaOracle.Game.Items.Rewards;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Entities.Monsters;
using ZeldaOracle.Common.Content;

namespace ZeldaOracle.Game {
	/// <summary>A static class for storing links to all game content.</summary>
	public static partial class GameData {

		//-----------------------------------------------------------------------------
		// Inventory Loading
		//-----------------------------------------------------------------------------

		/// <summary>Loads "Items/items.conscript"</summary>
		private static void LoadInventory() {
			Resources.LoadItems(Resources.ItemDirectory + "items.conscript");
			// No use for integrating these resources
		}


		//-----------------------------------------------------------------------------
		// Reward Loading
		//-----------------------------------------------------------------------------

		/// <summary>Loads "Items/rewards.conscript"</summary>
		private static void LoadRewards() {
			Resources.LoadRewards(Resources.ItemDirectory + "rewards.conscript");
			// No use for integrating these resources
		}


		//-----------------------------------------------------------------------------
		// Drop Lists Loading
		//-----------------------------------------------------------------------------

		/// <summary>Manually loads drops.</summary>
		public static void LoadDrops(DropManager dropManager, RewardManager rewardManager) {

			DropList dropsRupees = dropManager.CreateDropList("rupees");
			dropsRupees.AddDrop(3, rewardManager.GetReward("rupees_1"));
			dropsRupees.AddDrop(1, rewardManager.GetReward("rupees_5"));

			DropList dropsHearts = dropManager.CreateDropList("hearts");
			dropsHearts.AddDrop(4, rewardManager.GetReward("hearts_1"));
			dropsHearts.AddDrop(1, typeof(CollectibleFairy));
			
			DropList dropsSeeds = dropManager.CreateDropList("seeds");
			dropsSeeds.AddDrop(1, rewardManager.GetReward("ember_seeds_5"));
			dropsSeeds.AddDrop(1, rewardManager.GetReward("scent_seeds_5"));
			dropsSeeds.AddDrop(1, rewardManager.GetReward("pegasus_seeds_5"));
			dropsSeeds.AddDrop(1, rewardManager.GetReward("gale_seeds_5"));
			dropsSeeds.AddDrop(1, rewardManager.GetReward("mystery_seeds_5"));
			
			DropList dropsAmmo = dropManager.CreateDropList("ammo");
			dropsAmmo.AddDrop(1, rewardManager.GetReward("bombs_5"));
			dropsAmmo.AddDrop(1, rewardManager.GetReward("arrows_5"));
			dropsAmmo.AddDrop(5, dropsSeeds);
			
			
			// Drops that are created by default for tiles.
			DropList dropsDefault = dropManager.CreateDropList("default", 1, 3);
			dropsDefault.AddDrop(50, dropsAmmo);
			dropsDefault.AddDrop(46, dropsRupees);
			dropsDefault.AddDrop(25, dropsHearts);

			// Drops that are created when a ground tile is dug up.
			DropList dropsDigRupees = new DropList();
			dropsDigRupees.AddDrop(25, dropsRupees);
			dropsDigRupees.AddDrop(1, rewardManager.GetReward("rupees_100"));
			DropList dropsDigMonsters = new DropList();
			dropsDigMonsters.AddDrop(5, typeof(MonsterBeetle));
			dropsDigMonsters.AddDrop(2, typeof(MonsterRope));
			DropList dropsDig = dropManager.CreateDropList("dig", 1, 4);
			dropsDig.AddDrop(46, dropsDigRupees);
			dropsDig.AddDrop(25, dropsHearts);
			dropsDig.AddDrop(7, dropsDigMonsters);
			
			//DropList dropsDig = dropManager.CreateDropList("dig", 1, 1);
			//dropsDig.AddDrop(1, typeof(MonsterRope));
			//dropsDig.AddDrop(1, typeof(MonsterBeetle));
			//dropsDig.AddDrop(1, typeof(MonsterLynel));
		}
	}
}
