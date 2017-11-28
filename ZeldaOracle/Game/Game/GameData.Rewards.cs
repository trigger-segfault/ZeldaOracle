using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Items.Ammos;
using ZeldaOracle.Game.Items.Drops;
using ZeldaOracle.Game.Items.Weapons;
using ZeldaOracle.Game.Items.Essences;
using ZeldaOracle.Game.Items.KeyItems;
using ZeldaOracle.Game.Items.Equipment;
using ZeldaOracle.Game.Items;
using ZeldaOracle.Game.Items.Rewards;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Entities.Monsters;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Game {

	public partial class GameData {
		
		//-----------------------------------------------------------------------------
		// Inventory Loading
		//-----------------------------------------------------------------------------

		public static void LoadInventory(Inventory inventory, bool obtain = false) {

			// Add ammos.
			inventory.AddAmmos(false,
				new AmmoSatchelSeeds("ammo_ember_seeds", "Ember Seeds", "A burst of fire!",
					new Sprite(GameData.SHEET_ITEMS_SMALL, new Point2I(0, 3)), 0, 20),
				new AmmoSatchelSeeds("ammo_scent_seeds", "Scent Seeds", "An aromatic blast!",
					new Sprite(GameData.SHEET_ITEMS_SMALL, new Point2I(1, 3)), 0, 20),
				new AmmoSatchelSeeds("ammo_pegasus_seeds", "Pegasus Seeds", "Steals speed?",
					new Sprite(GameData.SHEET_ITEMS_SMALL, new Point2I(2, 3)), 0, 20),
				new AmmoSatchelSeeds("ammo_gale_seeds", "Gale Seeds", "A mighty blow!",
					new Sprite(GameData.SHEET_ITEMS_SMALL, new Point2I(3, 3)), 0, 20),
				new AmmoSatchelSeeds("ammo_mystery_seeds", "Mystery Seeds", "A producer of unknown effects.",
					new Sprite(GameData.SHEET_ITEMS_SMALL, new Point2I(4, 3)), 0, 20),
				new Ammo("ammo_bombs", "Bombs", "Very explosive.",
					new Sprite(GameData.SHEET_ITEMS_SMALL, new Point2I(13, 0)), 0, 10),
				new Ammo("ammo_arrows", "Arrows", "A standard arrow.",
					new Sprite(GameData.SHEET_ITEMS_SMALL, new Point2I(15, 1)), 0, 30)
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
		}


		//-----------------------------------------------------------------------------
		// Reward Loading
		//-----------------------------------------------------------------------------

		public static void LoadRewards(RewardManager rewardManager) {

			// Key Items.

			rewardManager.AddReward(new RewardItem("item_flippers_1", "item_flippers", Item.Level1, RewardHoldTypes.TwoHands,
				"You got <red>Zora's Flippers<red>! You can now go for a swim! Press A to swim, B to dive!",
				GameData.SPR_ITEM_ICON_FLIPPERS_1));

			rewardManager.AddReward(new RewardItem("item_flippers_2", "item_flippers", Item.Level2, RewardHoldTypes.TwoHands,
				"You got a <red>Mermaid Suit<red>! Now you can swim in deep waters. Press DPAD to swim, B to dive and A to use items.",
				GameData.SPR_ITEM_ICON_FLIPPERS_2));
			
			// Weapons.

			rewardManager.AddReward(new RewardItem("item_sword_1", "item_sword", Item.Level1, RewardHoldTypes.OneHand,
				"You got a Hero's <red>Wooden Sword<red>! Hold A or B to charge it up, then release it for a spin attack!",
				GameData.SPR_ITEM_ICON_SWORD_1));

			rewardManager.AddReward(new RewardItem("item_sword_2", "item_sword", Item.Level2, RewardHoldTypes.OneHand,
				"You got the sacred <red>Noble Sword<red>!",
				GameData.SPR_ITEM_ICON_SWORD_2));

			rewardManager.AddReward(new RewardItem("item_sword_3", "item_sword", Item.Level3, RewardHoldTypes.OneHand,
				"You got the legendary <red>Master Sword<red>!",
				GameData.SPR_ITEM_ICON_SWORD_3));

			rewardManager.AddReward(new RewardItem("item_shield_1", "item_shield", Item.Level1, RewardHoldTypes.TwoHands,
				"You got a <red>Wooden Shield<red>!",
				GameData.SPR_ITEM_ICON_SHIELD_1));

			rewardManager.AddReward(new RewardItem("item_shield_2", "item_shield", Item.Level2, RewardHoldTypes.TwoHands,
				"You got an <red>Iron Shield<red>!",
				GameData.SPR_ITEM_ICON_SHIELD_2));

			rewardManager.AddReward(new RewardItem("item_shield_3", "item_shield", Item.Level3, RewardHoldTypes.TwoHands,
				"You got the <red>Mirror Shield<red>!",
				GameData.SPR_ITEM_ICON_SHIELD_3));

			rewardManager.AddReward(new RewardItem("item_biggoron_sword", "item_biggoron_sword", Item.Level1, RewardHoldTypes.TwoHands,
				"You got <red>Biggoron's Sword<red>! This two-handed sword is huge!",
				GameData.SPR_ITEM_ICON_BIGGORON_SWORD));

			rewardManager.AddReward(new RewardItem("item_seed_satchel_1", "item_seed_satchel", Item.Level1, RewardHoldTypes.TwoHands,
				"You got a <red>Seed Satchel<red>! And it has <red>20 Ember Seeds<red>!",
				GameData.SPR_ITEM_ICON_SATCHEL));

			rewardManager.AddReward(new RewardItem("item_seed_satchel_2", "item_seed_satchel", Item.Level2, RewardHoldTypes.TwoHands,
				"You can now hold more <red>Mystical Seeds<red> than before!",
				GameData.SPR_ITEM_ICON_SATCHEL));

			rewardManager.AddReward(new RewardItem("item_seed_satchel_3", "item_seed_satchel", Item.Level3, RewardHoldTypes.TwoHands,
				"You can now hold even more <red>Mystical Seeds<red> than before!",
				GameData.SPR_ITEM_ICON_SATCHEL));

			rewardManager.AddReward(new RewardItem("item_bombs_1", "item_bombs", Item.Level1, RewardHoldTypes.TwoHands,
				"You got <red>Bombs<red>! Use them to blow open false walls. Press A or B to set a Bomb. If you also press DPAD, you can throw the Bomb.",
				GameData.SPR_ITEM_ICON_BOMB));

			rewardManager.AddReward(new RewardItem("item_bombs_2", "item_bombs", Item.Level2, RewardHoldTypes.TwoHands,
				"You can now hold more <red>Bombs<red> than before!",
				GameData.SPR_ITEM_ICON_BOMB));

			rewardManager.AddReward(new RewardItem("item_bombs_3", "item_bombs", Item.Level3, RewardHoldTypes.TwoHands,
				"You can now hold even more <red>Bombs<red> than before!",
				GameData.SPR_ITEM_ICON_BOMB));

			rewardManager.AddReward(new RewardItem("item_bracelet_1", "item_bracelet", Item.Level1, RewardHoldTypes.TwoHands,
				"You got the <red>Power Bracelet<red>! Hold the button and press DPAD to lift heavy objects!",
				GameData.SPR_ITEM_ICON_BRACELET));

			rewardManager.AddReward(new RewardItem("item_bracelet_2", "item_bracelet", Item.Level2, RewardHoldTypes.TwoHands,
				"You got the <red>Power Glove<red>! You can now lift heavy objects.",
				GameData.SPR_ITEM_ICON_POWER_GLOVES));

			rewardManager.AddReward(new RewardItem("item_boomerang_1", "item_boomerang", Item.Level1, RewardHoldTypes.TwoHands,
				"You got the <red>Boomerang<red>! Use it to stop enemies in their tracks!",
				GameData.SPR_ITEM_ICON_BOOMERANG_1));

			rewardManager.AddReward(new RewardItem("item_boomerang_2", "item_boomerang", Item.Level2, RewardHoldTypes.TwoHands,
				"It's the <red>Magical Boomerang<red>! Press DPAD while holding the button to control its flight path!",
				GameData.SPR_ITEM_ICON_BOOMERANG_2));

			rewardManager.AddReward(new RewardItem("item_feather_1", "item_feather", Item.Level1, RewardHoldTypes.TwoHands,
				"You got <red>Roc's Feather<red>! You feel as light as a feather!",
				GameData.SPR_ITEM_ICON_FEATHER));

			rewardManager.AddReward(new RewardItem("item_feather_2", "item_feather", Item.Level2, RewardHoldTypes.TwoHands,
				"You got <red>Roc's Cape<red>! Press and hold the button to do a double jump!",
				GameData.SPR_ITEM_ICON_CAPE));

			rewardManager.AddReward(new RewardItem("item_shovel", "item_shovel", Item.Level1, RewardHoldTypes.TwoHands,
				"You got the <red>Shovel<red>! Now start digging!",
				GameData.SPR_ITEM_ICON_SHOVEL));

			rewardManager.AddReward(new RewardItem("item_seed_shooter", "item_seed_shooter", Item.Level1, RewardHoldTypes.TwoHands,
				"You got the <red>Seed Shooter<red>! Pick your seeds, fire, then watch them ricochet.",
				GameData.SPR_ITEM_ICON_SEED_SHOOTER));

			rewardManager.AddReward(new RewardItem("item_slingshot_1", "item_slingshot", Item.Level1, RewardHoldTypes.TwoHands,
				"You got the <red>Slingshot<red>! Choose your seeds and take aim!",
				GameData.SPR_ITEM_ICON_SLINGSHOT_1));

			rewardManager.AddReward(new RewardItem("item_slingshot_2", "item_slingshot", Item.Level2, RewardHoldTypes.TwoHands,
				"You got the <red>Hyper Slingshot<red>! It shoots three seeds at a time!",
				GameData.SPR_ITEM_ICON_SLINGSHOT_2));

			rewardManager.AddReward(new RewardItem("item_bow_1", "item_bow", Item.Level1, RewardHoldTypes.TwoHands,
				"You got <red>Bow<red>! Shoot arrows at your enemies!",
				GameData.SPR_ITEM_ICON_BOW));

			rewardManager.AddReward(new RewardItem("item_bow_2", "item_bow", Item.Level2, RewardHoldTypes.TwoHands,
				"Your <red>Bow<red> has been upgraded! You can now hold more arrows.",
				GameData.SPR_ITEM_ICON_BOW));

			rewardManager.AddReward(new RewardItem("item_bow_3", "item_bow", Item.Level3, RewardHoldTypes.TwoHands,
				"Your <red>Bow<red> has been upgraded! You can now hold even more arrows.",
				GameData.SPR_ITEM_ICON_BOW));

			// Rupees.

			rewardManager.AddReward(new RewardRupee("rupees_1", 1,
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

			rewardManager.AddReward(new RewardAmmo("ammo_ember_seeds_5", "ammo_ember_seeds", 5,
				"You got<n><red>5 Ember Seeds<red>!",
				GameData.SPR_REWARD_SEED_EMBER));

			rewardManager.AddReward(new RewardAmmo("ammo_scent_seeds_5", "ammo_scent_seeds", 5,
				"You got<n><red>5 Scent Seeds<red>!",
				GameData.SPR_REWARD_SEED_SCENT));

			rewardManager.AddReward(new RewardAmmo("ammo_pegasus_seeds_5", "ammo_pegasus_seeds", 5,
				"You got<n><red>5 Pegasus Seeds<red>!",
				GameData.SPR_REWARD_SEED_PEGASUS));

			rewardManager.AddReward(new RewardAmmo("ammo_gale_seeds_5", "ammo_gale_seeds", 5,
				"You got<n><red>5 Gale Seeds<red>!",
				GameData.SPR_REWARD_SEED_GALE));

			rewardManager.AddReward(new RewardAmmo("ammo_mystery_seeds_5", "ammo_mystery_seeds", 5,
				"You got<n><red>5 Mystery Seeds<red>!",
				GameData.SPR_REWARD_SEED_MYSTERY));

			rewardManager.AddReward(new RewardAmmo("ammo_bombs_5", "ammo_bombs", 5,
				"You got<n><red>5 Bombs<red>!",
				GameData.SPR_ITEM_AMMO_BOMB));

			rewardManager.AddReward(new RewardAmmo("ammo_arrows_5", "ammo_arrows", 5,
				"You got<n><red>5 Arrows<red>!",
				GameData.SPR_ITEM_AMMO_ARROW));
						
			// Dungeon.

			rewardManager.AddReward(new RewardDungeonItem("small_key", GameData.SPR_REWARD_SMALL_KEY, RewardHoldTypes.Raise,
				"You found a <red>Small Key<red>!<n>Use it to open a locked door or block in this dungeon."));
			rewardManager.AddReward(new RewardDungeonItem("boss_key", GameData.SPR_REWARD_BOSS_KEY, RewardHoldTypes.Raise,
				"You found the <red>Boss Key<red>!"));
			rewardManager.AddReward(new RewardDungeonItem("map", GameData.SPR_REWARD_MAP, RewardHoldTypes.TwoHands,
				"It<ap>s a <red>Dungeon Map<red>! Press SELECT to see it. The darkened rooms are ones you haven<ap>t been to yet."));
			rewardManager.AddReward(new RewardDungeonItem("compass", GameData.SPR_REWARD_COMPASS, RewardHoldTypes.TwoHands,
				"You found the <red>Compass<red>!<n>Use it to track your position, locate chests, and find keys."));
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
			dropsSeeds.AddDrop(1, rewardManager.GetReward("ammo_ember_seeds_5"));
			dropsSeeds.AddDrop(1, rewardManager.GetReward("ammo_scent_seeds_5"));
			dropsSeeds.AddDrop(1, rewardManager.GetReward("ammo_pegasus_seeds_5"));
			dropsSeeds.AddDrop(1, rewardManager.GetReward("ammo_gale_seeds_5"));
			dropsSeeds.AddDrop(1, rewardManager.GetReward("ammo_mystery_seeds_5"));
			
			DropList dropsAmmo = dropManager.CreateDropList("ammo");
			dropsAmmo.AddDrop(1, rewardManager.GetReward("ammo_bombs_5"));
			dropsAmmo.AddDrop(1, rewardManager.GetReward("ammo_arrows_5"));
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
