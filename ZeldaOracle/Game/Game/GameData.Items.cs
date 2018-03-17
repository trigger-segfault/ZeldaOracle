using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Items.Ammos;
using ZeldaOracle.Game.Items.Drops;
using ZeldaOracle.Game.Items;
using ZeldaOracle.Game.Items.Rewards;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Entities.Monsters;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Common.Content;

namespace ZeldaOracle.Game {

	public partial class GameData {

		//-----------------------------------------------------------------------------
		// Items
		//-----------------------------------------------------------------------------

		public static Item ITEM_SWORD;
		public static Item ITEM_SHIELD;
		public static Item ITEM_BRACELET;
		public static Item ITEM_FEATHER;
		public static Item ITEM_BOMBS;
		public static Item ITEM_SEED_SATCHEL;
		public static Item ITEM_SEED_SHOOTER;
		public static Item ITEM_SLINGSHOT;
		public static Item ITEM_BOW;
		public static Item ITEM_SWITCH_HOOK;
		public static Item ITEM_CANE;
		public static Item ITEM_MAGNET_GLOVES;
		public static Item ITEM_BOOMERANG;
		public static Item ITEM_SHOVEL;
		public static Item ITEM_BIGGORON_SWORD;
		public static Item ITEM_MAGIC_ROD;
		public static Item ITEM_OCARINA;


		//-----------------------------------------------------------------------------
		// Inventory Loading
		//-----------------------------------------------------------------------------

		public static void LoadInventory(Inventory inventory, bool obtain = false) {
			// Clear the resources if they're being reloaded
			Resources.ClearDictionary<Item>();
			Resources.ClearDictionary<Ammo>();

			Resources.LoadItems(Resources.ItemDirectory + "items.conscript");
			IntegrateResources<Item>("ITEM_");
			IntegrateResources<Ammo>("AMMO_");
			inventory.LoadResources();
			inventory.SetMaxLevel("wallet");

			// Add ammos.
			/*inventory.AddAmmos(false,
				new Ammo("rupees", "Rupees", "A currency.",
					GameData.SPR_REWARD_RUPEE_SMALL_GREEN, 0, 99),
				new Ammo("ore_chunks", "Ore Chunks", "Subrosian currency.",
					GameData.SPR_REWARD_ORE_CHUNK_BLUE, 0, 99),
				new AmmoSatchelSeeds("ember_seeds", "Ember Seeds", "A burst of fire!",
					GameData.SPR_ITEM_SEED_EMBER, 0, 20),
				new AmmoSatchelSeeds("scent_seeds", "Scent Seeds", "An aromatic blast!",
					GameData.SPR_ITEM_SEED_SCENT, 0, 20),
				new AmmoSatchelSeeds("pegasus_seeds", "Pegasus Seeds", "Steals speed?",
					GameData.SPR_ITEM_SEED_PEGASUS, 0, 20),
				new AmmoSatchelSeeds("gale_seeds", "Gale Seeds", "A mighty blow!",
					GameData.SPR_ITEM_SEED_GALE, 0, 20),
				new AmmoSatchelSeeds("mystery_seeds", "Mystery Seeds", "A producer of unknown effects.",
					GameData.SPR_ITEM_SEED_MYSTERY, 0, 20),
				new Ammo("bombs", "Bombs", "Very explosive.",
					GameData.SPR_ITEM_AMMO_BOMB, 0, 10),
				new Ammo("arrows", "Arrows", "A standard arrow.",
					GameData.SPR_ITEM_AMMO_ARROW, 0, 30)
			);

			// Add weapons.
			inventory.AddItems(obtain,
				// Currently equipped items:
				new ItemSword(),
				new ItemFeather(),

				// Items in inventory menu:
				new ItemCane(),
				new ItemShield(),
				new ItemShovel(),
				new ItemMagicRod(),
				new ItemSwitchHook(),
				new ItemBracelet(),
				new ItemBow(),
				new ItemBombs(),
				new ItemOcarina(),
				new ItemBigSword(),
				new ItemBoomerang(),
				new ItemSeedSatchel(),
				new ItemSeedShooter(),
				new ItemSlingshot(),
				new ItemMagnetGloves(),
				
				// Key items:
				new ItemWallet(),
				new ItemMembersCard(),
				new ItemMagicPotion(),
				new ItemEssenceSeed(),

				// Essesnces:
				new ItemEssence1(),
				new ItemEssence2(),
				new ItemEssence3(),
				new ItemEssence4(),
				new ItemEssence5(),
				new ItemEssence6(),
				new ItemEssence7(),
				new ItemEssence8()
			);

			// Add key items.
			inventory.AddItems(false,
				new ItemFlippers());

			// Keep wallet as max level unless the game has wallet limitations
			inventory.SetMaxLevel("wallet");*/
		}


		//-----------------------------------------------------------------------------
		// Reward Loading
		//-----------------------------------------------------------------------------

		public static void LoadRewards(RewardManager rewardManager) {
			// Clear the resources if they're being reloaded
			Resources.ClearDictionary<Reward>();

			rewardManager.LoadItems();
			Resources.LoadRewards(Resources.ItemDirectory + "rewards.conscript");
			rewardManager.LoadResources();
			IntegrateResources<Reward>("REWARD_");
			//return;

			/*foreach (Item item in rewardManager.Inventory.GetItems()) {
				rewardManager.AddRewardItem(item);
			}*/

			// Key Items

			/*rewardManager.AddReward(new RewardItem("flippers_1", "flippers", Item.Level1, RewardHoldTypes.TwoHands,
				"You got <red>Zora's Flippers<red>! You can now go for a swim! Press <a> to swim, <b> to dive!",
				GameData.SPR_ITEM_ICON_FLIPPERS_1));

			rewardManager.AddReward(new RewardItem("flippers_2", "flippers", Item.Level2, RewardHoldTypes.TwoHands,
				"You got a <red>Mermaid Suit<red>! Now you can swim in deep waters. Press <dpad> to swim, <b> to dive and <a> to use items.",
				GameData.SPR_ITEM_ICON_FLIPPERS_2));
			
			// Weapons

			rewardManager.AddReward(new RewardItem("sword_1", "sword", Item.Level1, RewardHoldTypes.OneHand,
				"You got a Hero's <red>Wooden Sword<red>! Hold <a> or <b> to charge it up, then release it for a spin attack!",
				GameData.SPR_ITEM_ICON_SWORD_1));

			rewardManager.AddReward(new RewardItem("sword_2", "sword", Item.Level2, RewardHoldTypes.OneHand,
				"You got the sacred <red>Noble Sword<red>!",
				GameData.SPR_ITEM_ICON_SWORD_2));

			rewardManager.AddReward(new RewardItem("sword_3", "sword", Item.Level3, RewardHoldTypes.OneHand,
				"You got the legendary <red>Master Sword<red>!",
				GameData.SPR_ITEM_ICON_SWORD_3));

			rewardManager.AddReward(new RewardItem("shield_1", "shield", Item.Level1, RewardHoldTypes.TwoHands,
				"You got a <red>Wooden Shield<red>!",
				GameData.SPR_ITEM_ICON_SHIELD_1));

			rewardManager.AddReward(new RewardItem("shield_2", "shield", Item.Level2, RewardHoldTypes.TwoHands,
				"You got an <red>Iron Shield<red>!",
				GameData.SPR_ITEM_ICON_SHIELD_2));

			rewardManager.AddReward(new RewardItem("shield_3", "shield", Item.Level3, RewardHoldTypes.TwoHands,
				"You got the <red>Mirror Shield<red>!",
				GameData.SPR_ITEM_ICON_SHIELD_3));

			rewardManager.AddReward(new RewardItem("biggoron_sword", "biggoron_sword", Item.Level1, RewardHoldTypes.TwoHands,
				"You got <red>Biggoron's Sword<red>! This two-handed sword is huge!",
				GameData.SPR_ITEM_ICON_BIGGORON_SWORD));

			rewardManager.AddReward(new RewardItem("seed_satchel_1", "seed_satchel", Item.Level1, RewardHoldTypes.TwoHands,
				"You got a <red>Seed Satchel<red>! And it has <red>20 Ember Seeds<red>!",
				GameData.SPR_ITEM_ICON_SATCHEL));

			rewardManager.AddReward(new RewardItem("seed_satchel_2", "seed_satchel", Item.Level2, RewardHoldTypes.TwoHands,
				"You can now hold more <red>Mystical Seeds<red> than before!",
				GameData.SPR_ITEM_ICON_SATCHEL));

			rewardManager.AddReward(new RewardItem("seed_satchel_3", "seed_satchel", Item.Level3, RewardHoldTypes.TwoHands,
				"You can now hold even more <red>Mystical Seeds<red> than before!",
				GameData.SPR_ITEM_ICON_SATCHEL));

			rewardManager.AddReward(new RewardItem("bombs_1", "bombs", Item.Level1, RewardHoldTypes.TwoHands,
				"You got <red>Bombs<red>! Use them to blow open false walls. Press <a> or <b> to set a Bomb. If you also press <dpad>, you can throw the Bomb.",
				GameData.SPR_ITEM_ICON_BOMB));

			rewardManager.AddReward(new RewardItem("bombs_2", "bombs", Item.Level2, RewardHoldTypes.TwoHands,
				"You can now hold more <red>Bombs<red> than before!",
				GameData.SPR_ITEM_ICON_BOMB));

			rewardManager.AddReward(new RewardItem("bombs_3", "bombs", Item.Level3, RewardHoldTypes.TwoHands,
				"You can now hold even more <red>Bombs<red> than before!",
				GameData.SPR_ITEM_ICON_BOMB));

			rewardManager.AddReward(new RewardItem("bracelet_1", "bracelet", Item.Level1, RewardHoldTypes.TwoHands,
				"You got the <red>Power Bracelet<red>! Hold the button and press <dpad> to lift heavy objects!",
				GameData.SPR_ITEM_ICON_BRACELET));

			rewardManager.AddReward(new RewardItem("bracelet_2", "bracelet", Item.Level2, RewardHoldTypes.TwoHands,
				"You got the <red>Power Glove<red>! You can now lift heavy objects.",
				GameData.SPR_ITEM_ICON_POWER_GLOVES));

			rewardManager.AddReward(new RewardItem("boomerang_1", "boomerang", Item.Level1, RewardHoldTypes.TwoHands,
				"You got the <red>Boomerang<red>! Use it to stop enemies in their tracks!",
				GameData.SPR_ITEM_ICON_BOOMERANG_1));

			rewardManager.AddReward(new RewardItem("boomerang_2", "boomerang", Item.Level2, RewardHoldTypes.TwoHands,
				"It's the <red>Magical Boomerang<red>! Press <dpad> while holding the button to control its flight path!",
				GameData.SPR_ITEM_ICON_BOOMERANG_2));

			rewardManager.AddReward(new RewardItem("feather_1", "feather", Item.Level1, RewardHoldTypes.TwoHands,
				"You got <red>Roc's Feather<red>! You feel as light as a feather!",
				GameData.SPR_ITEM_ICON_FEATHER));

			rewardManager.AddReward(new RewardItem("feather_2", "feather", Item.Level2, RewardHoldTypes.TwoHands,
				"You got <red>Roc's Cape<red>! Press and hold the button to do a double jump!",
				GameData.SPR_ITEM_ICON_CAPE));

			rewardManager.AddReward(new RewardItem("shovel", "shovel", Item.Level1, RewardHoldTypes.TwoHands,
				"You got the <red>Shovel<red>! Now start digging!",
				GameData.SPR_ITEM_ICON_SHOVEL));

			rewardManager.AddReward(new RewardItem("seed_shooter", "seed_shooter", Item.Level1, RewardHoldTypes.TwoHands,
				"You got the <red>Seed Shooter<red>! Pick your seeds, fire, then watch them ricochet.",
				GameData.SPR_ITEM_ICON_SEED_SHOOTER));

			rewardManager.AddReward(new RewardItem("slingshot_1", "slingshot", Item.Level1, RewardHoldTypes.TwoHands,
				"You got the <red>Slingshot<red>! Choose your seeds and take aim!",
				GameData.SPR_ITEM_ICON_SLINGSHOT_1));

			rewardManager.AddReward(new RewardItem("slingshot_2", "slingshot", Item.Level2, RewardHoldTypes.TwoHands,
				"You got the <red>Hyper Slingshot<red>! It shoots three seeds at a time!",
				GameData.SPR_ITEM_ICON_SLINGSHOT_2));

			rewardManager.AddReward(new RewardItem("bow_1", "bow", Item.Level1, RewardHoldTypes.TwoHands,
				"You got <red>Bow<red>! Shoot arrows at your enemies!",
				GameData.SPR_ITEM_ICON_BOW));

			rewardManager.AddReward(new RewardItem("bow_2", "bow", Item.Level2, RewardHoldTypes.TwoHands,
				"Your <red>Bow Quiver<red> has been upgraded! You can now hold more arrows.",
				GameData.SPR_ITEM_ICON_BOW));

			rewardManager.AddReward(new RewardItem("bow_3", "bow", Item.Level3, RewardHoldTypes.TwoHands,
				"Your <red>Bow Quiver<red> has been upgraded! You can now hold even more arrows.",
				GameData.SPR_ITEM_ICON_BOW));

			rewardManager.AddReward(new RewardItem("magnet_gloves", "magnet_gloves", Item.Level1, RewardHoldTypes.OneHand,
				"You got the Magnetic Gloves! Their magnetic might attracts and repels.",
				GameData.SPR_ITEM_ICON_MAGNET_GLOVES_SOUTH));*/

			// Rupees

			/*rewardManager.AddReward(new RewardRupee("rupees_1", 1,
				"You got <red>1 Rupee<red>!<n>...",
				GameData.SPR_REWARD_RUPEE_SMALL_GREEN));

			rewardManager.AddReward(new RewardRupee("rupees_5", 5,
				"You got<n><red>5 Rupees<red>!",
				GameData.SPR_REWARD_RUPEE_RED));

			rewardManager.AddReward(new RewardRupee("rupees_10", 10,
				"You got<n><red>10 Rupees<red>!",
				GameData.SPR_REWARD_RUPEE_RED));

			rewardManager.AddReward(new RewardRupee("rupees_20", 20,
				"You got<n><red>20 Rupees<red>!<n>That's not bad.",
				GameData.SPR_REWARD_RUPEE_BLUE));

			rewardManager.AddReward(new RewardRupee("rupees_30", 30,
				"You got<n><red>30 Rupees<red>!<n>That's nice.",
				GameData.SPR_REWARD_RUPEE_BLUE));

			rewardManager.AddReward(new RewardRupee("rupees_50", 50,
				"You got<n><red>50 Rupees<red>!<n>How lucky!",
				GameData.SPR_REWARD_RUPEE_BLUE));

			rewardManager.AddReward(new RewardRupee("rupees_100", 100,
				"You got <red>100<n>Rupees<red>! I bet<n>you're thrilled!",
				GameData.SPR_REWARD_RUPEE_BIG_BLUE));

			rewardManager.AddReward(new RewardRupee("rupees_150", 150,
				"You got <red>150<n>Rupees<red>!<n>Way to go!!!",
				GameData.SPR_REWARD_RUPEE_BIG_RED));

			rewardManager.AddReward(new RewardRupee("rupees_200", 200,
				"You got <red>200<n>Rupees<red>! That's<n>pure bliss!",
				GameData.SPR_REWARD_RUPEE_BIG_RED));

			// Hearts.

			rewardManager.AddReward(new RewardHeartPiece());

			rewardManager.AddReward(new RewardHeartContainer());

			rewardManager.AddReward(new RewardRecoveryHeart("hearts_1", 1,
				"You recovered<n>only one <red>heart<red>!",
				GameData.SPR_REWARD_HEART));

			rewardManager.AddReward(new RewardRecoveryHeart("hearts_3", 3,
				"You got three<n><red>hearts<red>!",
				GameData.SPR_REWARD_HEARTS_3));

			// Ammo.

			rewardManager.AddReward(new RewardAmmo("ember_seeds_5", "ember_seeds", 5,
				"You got<n><red>5 Ember Seeds<red>!",
				GameData.SPR_REWARD_SEED_EMBER));

			rewardManager.AddReward(new RewardAmmo("scent_seeds_5", "scent_seeds", 5,
				"You got<n><red>5 Scent Seeds<red>!",
				GameData.SPR_REWARD_SEED_SCENT));

			rewardManager.AddReward(new RewardAmmo("pegasus_seeds_5", "pegasus_seeds", 5,
				"You got<n><red>5 Pegasus Seeds<red>!",
				GameData.SPR_REWARD_SEED_PEGASUS));

			rewardManager.AddReward(new RewardAmmo("gale_seeds_5", "gale_seeds", 5,
				"You got<n><red>5 Gale Seeds<red>!",
				GameData.SPR_REWARD_SEED_GALE));

			rewardManager.AddReward(new RewardAmmo("mystery_seeds_5", "mystery_seeds", 5,
				"You got<n><red>5 Mystery Seeds<red>!",
				GameData.SPR_REWARD_SEED_MYSTERY));

			rewardManager.AddReward(new RewardAmmo("bombs_5", "bombs", 5,
				"You got<n><red>5 Bombs<red>!",
				GameData.SPR_ITEM_AMMO_BOMB));

			rewardManager.AddReward(new RewardAmmo("arrows_5", "arrows", 5,
				"You got<n><red>5 Arrows<red>!",
				GameData.SPR_ITEM_AMMO_ARROW));
						
			// Dungeon.

			rewardManager.AddReward(new RewardDungeonItem("small_key", GameData.SPR_REWARD_SMALL_KEY, RewardHoldTypes.OneHand,
				"You found a <red>Small Key<red>!<n>Use it to open a locked door or block in this dungeon."));
			rewardManager.AddReward(new RewardDungeonItem("boss_key", GameData.SPR_REWARD_BOSS_KEY, RewardHoldTypes.OneHand,
				"You found the <red>Boss Key<red>!"));
			rewardManager.AddReward(new RewardDungeonItem("map", GameData.SPR_REWARD_MAP, RewardHoldTypes.TwoHands,
				"It<ap>s a <red>Dungeon Map<red>! Press SELECT to see it. The darkened rooms are ones you haven<ap>t been to yet."));
			rewardManager.AddReward(new RewardDungeonItem("compass", GameData.SPR_REWARD_COMPASS, RewardHoldTypes.TwoHands,
				"You found the <red>Compass<red>!<n>Use it to track your position, locate chests, and find keys."));*/
		}

		
		//-----------------------------------------------------------------------------
		// Drop Lists Loading
		//-----------------------------------------------------------------------------

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
