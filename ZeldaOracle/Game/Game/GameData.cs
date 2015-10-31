using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Content.ResourceBuilders;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Common.Scripts;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Entities.Monsters;
using ZeldaOracle.Game.Entities.Projectiles;
using ZeldaOracle.Game.Items.Ammos;
using ZeldaOracle.Game.Items.Drops;
using ZeldaOracle.Game.Items.Weapons;
using ZeldaOracle.Game.Items.Essences;
using ZeldaOracle.Game.Items.KeyItems;
using ZeldaOracle.Game.Items.Equipment;
using ZeldaOracle.Game.Items;
using ZeldaOracle.Game.Items.Rewards;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Tiles.Custom;
using ZeldaOracle.Game.Tiles.EventTiles;
using ZeldaOracle.Game.Worlds;
using System.CodeDom.Compiler;

namespace ScriptingInterface
{
    public interface IScriptType1
    {
        string RunScript(int value);
    }
}

namespace ZeldaOracle.Game {
	
	// A static class for storing links to all game content.
	public class GameData {


		//-----------------------------------------------------------------------------
		// Initialization
		//-----------------------------------------------------------------------------

		// Initializes and loads the game content.
		public static void Initialize() {
			Console.WriteLine("Loading Images");
			LoadImages();

			Console.WriteLine("Loading Sprites");
			LoadSprites();
			
			Console.WriteLine("Loading Animations");
			LoadAnimations();

			Console.WriteLine("Loading Collision Models");
			LoadCollisionModels();

			Console.WriteLine("Loading Property Actions");
			LoadPropertyActions();
			
			Console.WriteLine("Loading Zones");
			LoadZones();

			Console.WriteLine("Loading Tilesets");
			LoadTilesets();

			Console.WriteLine("Loading Fonts");
			LoadFonts();

			Console.WriteLine("Loading Shaders");
			LoadShaders();

			Console.WriteLine("Loading Sound Effects");
			LoadSounds();

			Console.WriteLine("Loading Music");
			LoadMusic();
		}


		//-----------------------------------------------------------------------------
		// Internal
		//-----------------------------------------------------------------------------

		// Assign static fields from their corresponding loaded resources.
		private static void IntegrateResources<T>(string prefix) {
			FieldInfo[] fields = typeof(GameData).GetFields();

			// Assign static fields from their corresponding loaded resources.
			for (int i = 0; i < fields.Length; i++) {
				FieldInfo field = fields[i];
				string name = field.Name.ToLower();
				
				if (field.FieldType == typeof(T) && field.Name.StartsWith(prefix)) {
					name = name.Remove(0, prefix.Length);

					if (Resources.ExistsResource<T>(name))
						field.SetValue(null, Resources.GetResource<T>(name));
					else if (field.GetValue(null) != null) {
						//Console.WriteLine("** WARNING: " + name + " is built programatically.");
					}
					else {
						//Console.WriteLine("** WARNING: " + name + " is never defined.");
					}
				}
			}
			
			// Loop through resource dictionary.
			// Find any resources that don't have corresponding fields in GameData.
			Dictionary<string, T> dictionary = Resources.GetResourceDictionary<T>();
			foreach (KeyValuePair<string, T> entry in dictionary) {
				string name = prefix.ToLower() + entry.Key;
				T resource = entry.Value;

				FieldInfo matchingField = null;
				for (int i = 0; i < fields.Length; i++) {
					FieldInfo field = fields[i];
					if (field.FieldType == typeof(T) && String.Compare(field.Name, name, true) == 0) {
						matchingField = field;
						break;
					}
				}
				if (matchingField == null) {
					//Console.WriteLine("** WARNING: Resource \"" + name + "\" does not have a corresponding field.");
				}
			}
		}


		//-----------------------------------------------------------------------------
		// Image Loading
		//-----------------------------------------------------------------------------

		// Loads the images.
		private static void LoadImages() {
			Resources.LoadImagesFromScript("Images/images.conscript");

			Resources.LoadImage("Images/UI/menu_weapons_a");
			Resources.LoadImage("Images/UI/menu_weapons_b");
			Resources.LoadImage("Images/UI/menu_key_items_a");
			Resources.LoadImage("Images/UI/menu_key_items_b");
			Resources.LoadImage("Images/UI/menu_essences_a");
			Resources.LoadImage("Images/UI/menu_essences_b");


			// Set the variant IDs for images with variants.
			FieldInfo[] fields = typeof(GameData).GetFields();
			Dictionary<string, Image> dictionary = Resources.GetResourceDictionary<Image>();
			string prefix = "VARIANT_";

			foreach (KeyValuePair<string, Image> entry in dictionary) {
				Image image = entry.Value;

				if (image.HasVariants || image.VariantName != "") {
					while (image != null) {
						string name = prefix.ToLower() + image.VariantName;
						
						// Find the matching variant constant for this name.
						FieldInfo matchingField = null;
						for (int i = 0; i < fields.Length; i++) {
							FieldInfo field = fields[i];
							if (field.Name.StartsWith(prefix) && field.FieldType == typeof(int) && String.Compare(field.Name, name, true) == 0) {
								matchingField = field;
								break;
							}
						}

						if (matchingField != null)
							image.VariantID = (int) matchingField.GetValue(null);
						else
							Console.WriteLine("** WARNING: Uknown variant \"" + image.VariantName + "\".");

						image = image.NextVariant;
					}
				}
			}

		}


		//-----------------------------------------------------------------------------
		// Sprite Loading
		//-----------------------------------------------------------------------------

		// Loads the sprites and sprite-sheets.
		private static void LoadSprites() {
			Resources.LoadSpriteSheets("SpriteSheets/sprites.conscript");
			IntegrateResources<SpriteSheet>("SHEET_");
			IntegrateResources<Sprite>("SPR_");
			
			// TEMPORARY: Create sprite arrays here.
			SPR_ITEM_SEEDS = new Sprite[] {
				SPR_ITEM_SEED_EMBER,
				SPR_ITEM_SEED_SCENT,
				SPR_ITEM_SEED_PEGASUS,
				SPR_ITEM_SEED_GALE,
				SPR_ITEM_SEED_MYSTERY
			};
			SPR_HUD_HEARTS = new Sprite[] {
				SPR_HUD_HEART_0,
				SPR_HUD_HEART_1,
				SPR_HUD_HEART_2,
				SPR_HUD_HEART_3,
				SPR_HUD_HEART_4,
			};
			SPR_HUD_HEART_PIECES_EMPTY = new Sprite[] {
				SPR_HUD_HEART_PIECES_EMPTY_TOP_LEFT,
				SPR_HUD_HEART_PIECES_EMPTY_BOTTOM_LEFT,
				SPR_HUD_HEART_PIECES_EMPTY_BOTTOM_RIGHT,
				SPR_HUD_HEART_PIECES_EMPTY_TOP_RIGHT
			};
			SPR_HUD_HEART_PIECES_FULL = new Sprite[] {
				SPR_HUD_HEART_PIECES_FULL_TOP_LEFT,
				SPR_HUD_HEART_PIECES_FULL_BOTTOM_LEFT,
				SPR_HUD_HEART_PIECES_FULL_BOTTOM_RIGHT,
				SPR_HUD_HEART_PIECES_FULL_TOP_RIGHT
			};

			SPR_COLOR_CUBE_ORIENTATIONS = new Sprite[6];
			string[] orientations = { "blue_yellow", "blue_red", "yellow_red", "yellow_blue", "red_blue", "red_yellow" };

			for (int i = 0; i < 6; i++) {
				SPR_COLOR_CUBE_ORIENTATIONS[i] = Resources.GetResource<Sprite>("color_cube_" + orientations[i]);
			}
		}


		//-----------------------------------------------------------------------------
		// Animations Loading
		//-----------------------------------------------------------------------------

		private static void LoadAnimations() {
			Resources.LoadAnimations("Animations/animations.conscript");

			// Create gale effect animation.
			SpriteSheet sheet = Resources.GetSpriteSheet("color_effects");
			ANIM_EFFECT_SEED_GALE = new Animation();
			for (int i = 0; i < 12; i++) {
				int y = 1 + (((5 - (i % 4)) % 4) * 4);
				ANIM_EFFECT_SEED_GALE.AddFrame(i, 1, new Sprite(
					GameData.SHEET_COLOR_EFFECTS, ((i % 6) < 3 ? 4 : 5), y, -8, -8));
			}
			Resources.SetResource("effect_seed_gale", ANIM_EFFECT_SEED_GALE);

			ANIM_COLOR_CUBE_ROLLING_ORIENTATIONS = new Animation[6, 4];
			string[] orientations = { "blue_yellow", "blue_red", "yellow_red", "yellow_blue", "red_blue", "red_yellow" };
			string[] directions = { "right", "up", "left", "down" };

			for (int i = 0; i < 6; i++) {
				for (int j = 0; j < 4; j++) {
					ANIM_COLOR_CUBE_ROLLING_ORIENTATIONS[i, j] = Resources.GetResource<Animation>("color_cube_" + orientations[i] + "_" + directions[j]);
				}
			}

			IntegrateResources<Animation>("ANIM_");

			ANIM_EFFECT_SEEDS = new Animation[5];
			ANIM_EFFECT_SEEDS[(int) SeedType.Ember]		= ANIM_EFFECT_SEED_EMBER;
			ANIM_EFFECT_SEEDS[(int) SeedType.Scent]		= ANIM_EFFECT_SEED_SCENT;
			ANIM_EFFECT_SEEDS[(int) SeedType.Gale]		= ANIM_EFFECT_SEED_GALE;
			ANIM_EFFECT_SEEDS[(int) SeedType.Pegasus]	= ANIM_EFFECT_SEED_PEGASUS;
			ANIM_EFFECT_SEEDS[(int) SeedType.Mystery]	= ANIM_EFFECT_SEED_MYSTERY;
		}

		
		//-----------------------------------------------------------------------------
		// Collision Models Loading
		//-----------------------------------------------------------------------------

		private static void LoadCollisionModels() {
			Resources.LoadCollisionModels("Data/collision_models.conscript");
			IntegrateResources<CollisionModel>("MODEL_");
		}

		//-----------------------------------------------------------------------------
		// Property Action Loading
		//-----------------------------------------------------------------------------

		private static bool IsTileDataInstanceOf(IPropertyObject sender, Type type) {
			return (sender is TileDataInstance && (sender as TileDataInstance).TileData.Type == type);
		}
		private static bool IsEventTileDataInstanceOf(IPropertyObject sender, Type type) {
			return (sender is EventTileDataInstance && (sender as EventTileDataInstance).EventTileData.Type == type);
		}
		private static bool IsTileOrTileDataInstanceOf(IPropertyObject sender, Type type) {
			if (sender == null)
				return false;
			return (IsTileDataInstanceOf(sender, type) ||
				IsEventTileDataInstanceOf(sender, type) || sender.GetType() == type);
		}

		private static void LoadPropertyActions() {
			Resources.AddResource<PropertyAction>("zone", delegate(IPropertyObject sender, object value) {
				if (sender is Room) {
					Room room = sender as Room;
					room.Zone = Resources.GetResource<Zone>((string)value);
				}
				else if (sender is Level) {
					Level level = sender as Level;
					level.Zone = Resources.GetResource<Zone>((string)value);
				}
			});

			Resources.AddResource<PropertyAction>("looted", delegate(IPropertyObject sender, object value) {
				if (IsTileOrTileDataInstanceOf(sender, typeof(TileChest))) {
					sender.Properties.Set("sprite_index", (bool)value ? 1 : 0);
				}
			});

			Resources.AddResource<PropertyAction>("lit", delegate(IPropertyObject sender, object value) {
				if (IsTileOrTileDataInstanceOf(sender, typeof(TileLantern))) {
					sender.Properties.Set("sprite_index", (bool)value ? 0 : 1);
				}
			});
			
			Resources.AddResource<PropertyAction>("direction", delegate(IPropertyObject sender, object value) {
				if (IsTileOrTileDataInstanceOf(sender, typeof(NPCEvent))) {
					sender.Properties.Set("substrip_index", (int) value);
				}
			});
			
			Resources.AddResource<PropertyAction>("warp_type", delegate(IPropertyObject sender, object value) {
				if (IsEventTileDataInstanceOf(sender, typeof(WarpEvent))) {
					WarpType warpType = (WarpType) Enum.Parse(typeof(WarpType), (string) value, true);
					EventTileDataInstance eventTile = (EventTileDataInstance) sender;
					if (warpType == WarpType.Entrance)
						eventTile.Sprite = GameData.SPR_EVENT_TILE_WARP_ENTRANCE;
					else if (warpType == WarpType.Tunnel)
						eventTile.Sprite = GameData.SPR_EVENT_TILE_WARP_TUNNEL;
					else if (warpType == WarpType.Stairs)
						eventTile.Sprite = GameData.SPR_EVENT_TILE_WARP_STAIRS;
				}
			});
		}


		//-----------------------------------------------------------------------------
		// Zone Loading
		//-----------------------------------------------------------------------------

		private static void LoadZones() {
			ZONE_DEFAULT	= new Zone("",			"(none)",		VARIANT_NONE);
			ZONE_SUMMER		= new Zone("summer",	"Summer",		VARIANT_SUMMER);
			ZONE_FOREST		= new Zone("forest",	"Forest",		VARIANT_FOREST);
			ZONE_GRAVEYARD	= new Zone("graveyard",	"Graveyard",	VARIANT_GRAVEYARD);
			ZONE_INTERIOR	= new Zone("interior",	"Interior",		VARIANT_INTERIOR);
			ZONE_PRESENT	= new Zone("present", "Present", VARIANT_PRESENT);
			ZONE_INTERIOR_PRESENT	= new Zone("interior_present", "Interior Present", VARIANT_INTERIOR_PRESENT);
			ZONE_AGES_DUNGEON_1	= new Zone("ages_dungeon_1", "Ages Dungeon 1", VARIANT_AGES_DUNGEON_1);

			Resources.AddResource("default",	ZONE_DEFAULT);
			Resources.AddResource("summer",		ZONE_SUMMER);
			Resources.AddResource("forest",		ZONE_FOREST);
			Resources.AddResource("graveyard",	ZONE_GRAVEYARD);
			Resources.AddResource("interior",	ZONE_INTERIOR);
			Resources.AddResource("present",	ZONE_PRESENT);
			Resources.AddResource("interior_present", ZONE_INTERIOR_PRESENT);
			Resources.AddResource("ages_dungeon_1", ZONE_AGES_DUNGEON_1);
		}

		//-----------------------------------------------------------------------------
		// Tliesets Loading
		//-----------------------------------------------------------------------------

		private static void LoadTilesets() {
			// Load tilesets and tile data.
			Resources.LoadTilesets("Tilesets/cliffs.conscript");
			Resources.LoadTilesets("Tilesets/land.conscript");
			Resources.LoadTilesets("Tilesets/forest.conscript");
			Resources.LoadTilesets("Tilesets/water.conscript");
			Resources.LoadTilesets("Tilesets/town.conscript");
			Resources.LoadTilesets("Tilesets/interior.conscript");
			Resources.LoadTilesets("Tilesets/dungeon.conscript");
			Resources.LoadTilesets("Tilesets/objects.conscript");
			Resources.LoadTilesets("Tilesets/objects_nv.conscript");
			Resources.LoadTilesets("Tilesets/tile_data.conscript");
			Resources.LoadTilesets("Tilesets/overworld.conscript");
			Resources.LoadTilesets("Tilesets/interior2.conscript");

			EventTileset eventTileset = new EventTileset("events", null, 12, 16);
			Resources.AddResource(eventTileset.ID, eventTileset);

			// Create an warp event.
			EventTileData etd = new EventTileData(typeof(WarpEvent));
			etd.Sprite = SPR_EVENT_TILE_WARP_STAIRS;
			etd.Properties.Set("warp_type", "tunnel")
				.SetDocumentation("Warp Type", "enum", "WarpType", "", "The type of warp point.");
			etd.Properties.Set("destination_level", "")
				.SetDocumentation("Destination Level", "", "", "", "The level where the destination point is in.");
			etd.Properties.Set("destination_warp_point", "")
				.SetDocumentation("Destination Warp Point", "", "", "", "The id of the warp point destination.");
			etd.Properties.Set("face_direction", Directions.Down)
				.SetDocumentation("Face Direction", "direction", "", "", "The direction the player should face when entering a room through this Warp Point.");
			etd.Sprite = GameData.SPR_EVENT_TILE_WARP_TUNNEL;
			Resources.AddResource("warp", etd);
			eventTileset.TileData[0, 0] = etd;
			
			// Create an NPC event.
			etd = new EventTileData(typeof(NPCEvent));
			etd.Sprite = Resources.GetAnimation("npc_shopkeeper");
			etd.Properties.Set("substrip_index", Directions.Down);
			etd.Properties.Set("npc_flags", (int) NPCFlags.Default)
				.SetDocumentation("NPC Options", "enum_flags", "", "", "The options for the NPC.");
			etd.Properties.Set("direction", Directions.Down)
				.SetDocumentation("Direction", "direction", "", "", "The default direction the NPC faces.");
			etd.Properties.Set("text", "<red>undefined<red>")
				.SetDocumentation("Text", "text_message", "", "", "The text to display when the NPC is talked to.");
			etd.Properties.Set("animation", "npc_shopkeeper")
				.SetDocumentation("Animation", "animation", "", "", "The animation of the NPC.");
			etd.Properties.Set("animation_talk", "")
				.SetDocumentation("Talk Animation", "animation", "", "", "The animation of the NPC when being talked to.");
			Resources.AddResource("npc", etd);
			eventTileset.TileData[1, 0] = etd;
			
			// Create a monster event.
			etd = new EventTileData(typeof(MonsterEvent));
			etd.Sprite = Resources.GetAnimation("monster_octorok");
			
			etd.Properties.Set("color", "Red")
				.SetDocumentation("Color", "enum", "MonsterColor", "", "The color of the monster.");
			etd.Properties.Set("respawn_type", "Normal")
				.SetDocumentation("Respawn Type", "enum", "MonsterRespawnType", "", "When a monster respawns.");

			etd.Properties.Set("substrip_index", Directions.Down);
			//etd.Properties.Set("image_variant", GameData.VARIANT_RED);

			Resources.AddResource("monster_octorok_red", etd);
			eventTileset.TileData[2, 0] = etd;



			IntegrateResources<Tileset>("TILESET_");
		}


		//-----------------------------------------------------------------------------
		// Font Loading
		//-----------------------------------------------------------------------------

		// Loads the fonts.
		private static void LoadFonts() {
			Resources.LoadGameFonts("Fonts/fonts.conscript");

			FONT_LARGE = Resources.GetGameFont("Fonts/font_large");
			FONT_SMALL = Resources.GetGameFont("Fonts/font_small");
		}


		//-----------------------------------------------------------------------------
		// Shader Loading
		//-----------------------------------------------------------------------------

		// Loads the shaders.
		private static void LoadShaders() {
			// None yet...
		}


		//-----------------------------------------------------------------------------
		// Sound Effects Loading
		//-----------------------------------------------------------------------------

		// Loads the sound effects.
		private static void LoadSounds() {
			Resources.LoadSounds(Resources.SoundDirectory + "sounds.conscript");

		}


		//-----------------------------------------------------------------------------
		// Music Loading
		//-----------------------------------------------------------------------------

		// Loads the music.
		private static void LoadMusic() {
			Resources.LoadMusic(Resources.MusicDirectory + "music.conscript");
		}


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
				new ItemWallet(),
				new ItemSword(),
				new ItemShovel(),
				new ItemCane(),
				new ItemMagicRod(),
				new ItemSwitchHook(),
				new ItemBracelet(),
				new ItemFeather(),
				new ItemBow(),
				new ItemEssence1(),
				new ItemEssence2(),
				new ItemEssence3(),
				new ItemEssence4(),
				new ItemEssence5(),
				new ItemEssence6(),
				new ItemEssence7(),
				new ItemEssence8(),
				new ItemMagicPotion(),
				new ItemEssenceSeed(),
				new ItemBombs(),
				new ItemOcarina(),
				new ItemBigSword(),
				new ItemMembersCard(),
				new ItemSword(),
				new ItemShield(),
				new ItemBoomerang(),
				new ItemSeedSatchel(),
				new ItemSeedShooter(),
				new ItemSlingshot());

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
				"You got the <red>Power Bracelet<red>! Hold the button and press DPAD to life heavy objects!",
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
				"You got <red>1 Rupee<red>!<n>That's terrible.",
				GameData.SPR_REWARD_RUPEE_GREEN));

			rewardManager.AddReward(new RewardRupee("rupees_5", 5,
				"You got<n><red>5 Rupees<red>!",
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

			// Monsters?

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
			dropsDigMonsters.AddDrop(5, typeof(Monster)); // TODO: Beetle as a dig Drop
			dropsDigMonsters.AddDrop(2, typeof(Monster)); // TODO: Rope as a dig Drop
			DropList dropsDig = dropManager.CreateDropList("dig", 1, 4);
			dropsDig.AddDrop(46, dropsDigRupees);
			dropsDig.AddDrop(25, dropsHearts);
			dropsDig.AddDrop(7, dropsDigMonsters);
		}


		//-----------------------------------------------------------------------------
		// Tilesets
		//-----------------------------------------------------------------------------

		public static Tileset TILESET_OVERWORLD;
		public static Tileset TILESET_INTERIOR;
		public static Tileset TILESET_CLIFFS;


		//-----------------------------------------------------------------------------
		// Images & Image Variants
		//-----------------------------------------------------------------------------

		public static int VARIANT_NONE		= 0;
		public static int VARIANT_DARK		= 1;
		public static int VARIANT_LIGHT		= 2;
		public static int VARIANT_RED		= 3;
		public static int VARIANT_BLUE		= 4;
		public static int VARIANT_GREEN		= 5;
		public static int VARIANT_YELLOW	= 6;
		public static int VARIANT_HURT		= 7;
		public static int VARIANT_SUMMER	= 8;
		public static int VARIANT_FOREST	= 9;
		public static int VARIANT_GRAVEYARD	= 10;
		public static int VARIANT_INTERIOR	= 11;
		public static int VARIANT_PRESENT	= 12;
		public static int VARIANT_INTERIOR_PRESENT	= 13;
		public static int VARIANT_AGES_DUNGEON_1	= 14;

		public static int VARIANT_ORANGE	= 15;

		//-----------------------------------------------------------------------------
		// Sprite Sheets
		//-----------------------------------------------------------------------------

		public static SpriteSheet SHEET_MENU_SMALL;
		public static SpriteSheet SHEET_MENU_LARGE;
		public static SpriteSheet SHEET_MENU_SMALL_LIGHT;
		public static SpriteSheet SHEET_MENU_LARGE_LIGHT;
		public static SpriteSheet SHEET_ITEMS_SMALL;
		public static SpriteSheet SHEET_ITEMS_LARGE;
		public static SpriteSheet SHEET_ITEMS_SMALL_LIGHT;
		public static SpriteSheet SHEET_ITEMS_LARGE_LIGHT;

		public static SpriteSheet SHEET_BASIC_EFFECTS;
		public static SpriteSheet SHEET_COLOR_EFFECTS;

		public static SpriteSheet SHEET_PLAYER;
		public static SpriteSheet SHEET_PLAYER_RED;
		public static SpriteSheet SHEET_PLAYER_BLUE;
		public static SpriteSheet SHEET_PLAYER_HURT;
		//public static SpriteSheet SHEET_MONSTERS;
		//public static SpriteSheet SHEET_MONSTERS_HURT;
		public static SpriteSheet SHEET_PLAYER_ITEMS;
	
		public static SpriteSheet SHEET_ZONESET_LARGE;
		public static SpriteSheet SHEET_ZONESET_SMALL;
		public static SpriteSheet SHEET_TILESET_OVERWORLD;
		public static SpriteSheet SHEET_TILESET_INTERIOR;
		public static SpriteSheet SHEET_GENERAL_TILES;


		//-----------------------------------------------------------------------------
		// Sprites
		//-----------------------------------------------------------------------------

		// Effects.
		public static Sprite SPR_SHADOW;

		// Special Background tiles.
		public static Sprite SPR_TILE_DEFAULT;	// The default ground background tile.
		public static Sprite SPR_TILE_DUG;		// A hole in the ground created by a shovel.
	
		// Player sprites.
		public static Sprite SPR_PLAYER_FORWARD;

		// Entities.
		public static Sprite SPR_SWITCH_HOOK_LINK;

		// Object tiles.
		public static Sprite SPR_TILE_BUSH;
		public static Sprite SPR_TILE_BUSH_ASOBJECT;
		public static Sprite SPR_TILE_CRYSTAL;
		public static Sprite SPR_TILE_CRYSTAL_ASOBJECT;
		public static Sprite SPR_TILE_POT;
		public static Sprite SPR_TILE_POT_ASOBJECT;
		public static Sprite SPR_TILE_ROCK;
		public static Sprite SPR_TILE_ROCK_ASOBJECT;
		public static Sprite SPR_TILE_DIAMOND_ROCK;
		public static Sprite SPR_TILE_DIAMOND_ROCK_ASOBJECT;
		public static Sprite SPR_TILE_SIGN;
		public static Sprite SPR_TILE_SIGN_ASOBJECT;
		public static Sprite SPR_TILE_GRASS;
		public static Sprite SPR_TILE_MOVABLE_BLOCK;
		public static Sprite SPR_TILE_MOVABLE_BLOCK_ASOBJECT;
		public static Sprite SPR_TILE_BOMBABLE_BLOCK;
		public static Sprite SPR_TILE_LOCKED_BLOCK;
		public static Sprite SPR_TILE_CHEST;
		public static Sprite SPR_TILE_CHEST_OPEN;
		public static Sprite SPR_TILE_DIRT_PILE;
		public static Sprite SPR_TILE_BURNABLE_TREE;
		public static Sprite SPR_TILE_CACTUS;
		public static Sprite SPR_TILE_BUTTON_UP;
		public static Sprite SPR_TILE_BUTTON_DOWN;
		public static Sprite SPR_TILE_LEVER_LEFT;
		public static Sprite SPR_TILE_LEVER_RIGHT;
		public static Sprite SPR_TILE_LANTERN_UNLIT;
		public static Sprite SPR_TILE_EYE_STATUE;
		public static Sprite SPR_TILE_BRIDGE_H;
		public static Sprite SPR_TILE_BRIDGE_V;
		public static Sprite SPR_TILE_COLOR_CUBE_SLOT;
		public static Sprite SPR_TILE_CRACKED_FLOOR;
		public static Sprite SPR_TILE_PIT;
		public static Sprite SPR_TILE_ARMOS_STATUE;
		public static Sprite SPR_TILE_OWL;
		public static Sprite SPR_TILE_OWL_ACTIVATED;
		public static Sprite SPR_TILE_SOMARIA_BLOCK;

		public static Sprite[] SPR_COLOR_CUBE_ORIENTATIONS;

		// Item Icons.
		public static Sprite SPR_ITEM_SEED_EMBER;
		public static Sprite SPR_ITEM_SEED_SCENT;
		public static Sprite SPR_ITEM_SEED_PEGASUS;
		public static Sprite SPR_ITEM_SEED_GALE;
		public static Sprite SPR_ITEM_SEED_MYSTERY;
		public static Sprite[] SPR_ITEM_SEEDS;

		public static Sprite SPR_ITEM_AMMO_ARROW;
		public static Sprite SPR_ITEM_AMMO_BOMB;
		
		public static Sprite SPR_ITEM_ICON_BIGGORON_SWORD;
		public static Sprite SPR_ITEM_ICON_BIGGORON_SWORD_EQUIPPED;

		public static Sprite SPR_ITEM_ICON_SWORD_1;
		public static Sprite SPR_ITEM_ICON_SWORD_2;
		public static Sprite SPR_ITEM_ICON_SWORD_3;
		public static Sprite SPR_ITEM_ICON_SHIELD_1;
		public static Sprite SPR_ITEM_ICON_SHIELD_2;
		public static Sprite SPR_ITEM_ICON_SHIELD_3;
		public static Sprite SPR_ITEM_ICON_SATCHEL;
		public static Sprite SPR_ITEM_ICON_SATCHEL_EQUIPPED;
		public static Sprite SPR_ITEM_ICON_SEED_SHOOTER;
		public static Sprite SPR_ITEM_ICON_SEED_SHOOTER_EQUIPPED;
		public static Sprite SPR_ITEM_ICON_SLINGSHOT_1;
		public static Sprite SPR_ITEM_ICON_SLINGSHOT_2;
		public static Sprite SPR_ITEM_ICON_SLINGSHOT_2_EQUIPPED;
		public static Sprite SPR_ITEM_ICON_BOMB;
		public static Sprite SPR_ITEM_ICON_BOMBCHEW;
		public static Sprite SPR_ITEM_ICON_SHOVEL;
		public static Sprite SPR_ITEM_ICON_BRACELET;
		public static Sprite SPR_ITEM_ICON_POWER_GLOVES;
		public static Sprite SPR_ITEM_ICON_FEATHER;
		public static Sprite SPR_ITEM_ICON_CAPE;
		public static Sprite SPR_ITEM_ICON_BOOMERANG_1;
		public static Sprite SPR_ITEM_ICON_BOOMERANG_2;
		public static Sprite SPR_ITEM_ICON_SWITCH_HOOK;
		public static Sprite SPR_ITEM_ICON_HOOK_SHOT;
		public static Sprite SPR_ITEM_ICON_MAGNET_GLOVES_BLUE;
		public static Sprite SPR_ITEM_ICON_MAGNET_GLOVES_RED;
		public static Sprite SPR_ITEM_ICON_CANE;
		public static Sprite SPR_ITEM_ICON_ROD_OF_ESSENCES;
		public static Sprite SPR_ITEM_ICON_MAGIC_ROD;
		public static Sprite SPR_ITEM_ICON_OCARINA;
		public static Sprite SPR_ITEM_ICON_BOW;
		public static Sprite SPR_ITEM_ICON_FLIPPERS_1;
		public static Sprite SPR_ITEM_ICON_FLIPPERS_2;
		public static Sprite SPR_ITEM_ICON_MAGIC_POTION;
		public static Sprite SPR_ITEM_ICON_MEMBERS_CARD;
		public static Sprite SPR_ITEM_ICON_ESSENCE_SEED;
		public static Sprite SPR_ITEM_ICON_ESSENCE_1;
		public static Sprite SPR_ITEM_ICON_ESSENCE_2;
		public static Sprite SPR_ITEM_ICON_ESSENCE_3;
		public static Sprite SPR_ITEM_ICON_ESSENCE_4;
		public static Sprite SPR_ITEM_ICON_ESSENCE_5;
		public static Sprite SPR_ITEM_ICON_ESSENCE_6;
		public static Sprite SPR_ITEM_ICON_ESSENCE_7;
		public static Sprite SPR_ITEM_ICON_ESSENCE_8;

		// Reward Icons.
		public static Sprite SPR_REWARD_RUPEE_GREEN;
		public static Sprite SPR_REWARD_RUPEE_RED;
		public static Sprite SPR_REWARD_RUPEE_BLUE;
		public static Sprite SPR_REWARD_RUPEE_BIG_BLUE;
		public static Sprite SPR_REWARD_RUPEE_BIG_RED;
		public static Sprite SPR_REWARD_HEART;
		public static Sprite SPR_REWARD_HEARTS_3;
		public static Sprite SPR_REWARD_SEED_EMBER;
		public static Sprite SPR_REWARD_SEED_SCENT;
		public static Sprite SPR_REWARD_SEED_PEGASUS;
		public static Sprite SPR_REWARD_SEED_GALE;
		public static Sprite SPR_REWARD_SEED_MYSTERY;
		public static Sprite SPR_REWARD_HEART_PIECE;
		public static Sprite SPR_REWARD_HEART_CONTAINER;
		public static Sprite SPR_REWARD_HARP;
	
		// HUD Sprites.
		public static Sprite SPR_HUD_BACKGROUND;
		public static Sprite SPR_HUD_BRACKET_LEFT;
		public static Sprite SPR_HUD_BRACKET_LEFT_A;
		public static Sprite SPR_HUD_BRACKET_LEFT_B;
		public static Sprite SPR_HUD_BRACKET_RIGHT;
		public static Sprite SPR_HUD_BRACKET_RIGHT_A;
		public static Sprite SPR_HUD_BRACKET_RIGHT_B;
		public static Sprite SPR_HUD_BRACKET_LEFT_RIGHT;
		public static Sprite SPR_HUD_HEART_0;
		public static Sprite SPR_HUD_HEART_1;
		public static Sprite SPR_HUD_HEART_2;
		public static Sprite SPR_HUD_HEART_3;
		public static Sprite SPR_HUD_HEART_4;
		public static Sprite SPR_HUD_RUPEE;
		public static Sprite SPR_HUD_ORE_CHUNK;
		public static Sprite SPR_HUD_KEY;
		public static Sprite SPR_HUD_X;
		public static Sprite SPR_HUD_LEVEL;
		public static Sprite SPR_HUD_TEXT_NEXT_ARROW;
		public static Sprite SPR_HUD_HEART_PIECES_EMPTY_TOP_LEFT;
		public static Sprite SPR_HUD_HEART_PIECES_EMPTY_TOP_RIGHT;
		public static Sprite SPR_HUD_HEART_PIECES_EMPTY_BOTTOM_LEFT;
		public static Sprite SPR_HUD_HEART_PIECES_EMPTY_BOTTOM_RIGHT;
		public static Sprite SPR_HUD_HEART_PIECES_FULL_TOP_LEFT;
		public static Sprite SPR_HUD_HEART_PIECES_FULL_TOP_RIGHT;
		public static Sprite SPR_HUD_HEART_PIECES_FULL_BOTTOM_LEFT;
		public static Sprite SPR_HUD_HEART_PIECES_FULL_BOTTOM_RIGHT;
		public static Sprite SPR_HUD_SAVE_BUTTON;
		public static Sprite[] SPR_HUD_HEARTS;
		public static Sprite[] SPR_HUD_HEART_PIECES_EMPTY;
		public static Sprite[] SPR_HUD_HEART_PIECES_FULL;
		
		// Event tiles.
		public static Sprite SPR_EVENT_TILE_WARP_STAIRS;
		public static Sprite SPR_EVENT_TILE_WARP_TUNNEL;
		public static Sprite SPR_EVENT_TILE_WARP_ENTRANCE;
		public static Sprite SPR_EVENT_TILE_TRACK_INTERSECTION;


		//-----------------------------------------------------------------------------
		// Animations
		//-----------------------------------------------------------------------------

		// Tile animations.
		public static Animation ANIM_TILE_WATER;
		public static Animation ANIM_TILE_OCEAN;
		public static Animation ANIM_TILE_OCEAN_SHORE;
		public static Animation ANIM_TILE_FLOWERS;
		public static Animation ANIM_TILE_WATERFALL;
		public static Animation ANIM_TILE_WATERFALL_BOTTOM;
		public static Animation ANIM_TILE_WATERFALL_TOP;
		public static Animation ANIM_TILE_WATER_DEEP;
		public static Animation ANIM_TILE_PUDDLE;
		public static Animation ANIM_TILE_LANTERN;
		public static Animation ANIM_TILE_LAVAFALL;
		public static Animation ANIM_TILE_LAVAFALL_BOTTOM;
		public static Animation ANIM_TILE_LAVAFALL_TOP;

		public static Animation[,] ANIM_COLOR_CUBE_ROLLING_ORIENTATIONS;
	
		// Player animations.
		public static Animation ANIM_PLAYER_DEFAULT;
		public static Animation ANIM_PLAYER_CARRY;
		public static Animation ANIM_PLAYER_SHIELD;
		public static Animation ANIM_PLAYER_SHIELD_BLOCK;
		public static Animation ANIM_PLAYER_SHIELD_LARGE;
		public static Animation ANIM_PLAYER_SHIELD_LARGE_BLOCK;
		public static Animation ANIM_PLAYER_SWIM;
		public static Animation ANIM_PLAYER_PUSH;
		public static Animation ANIM_PLAYER_GRAB;
		public static Animation ANIM_PLAYER_PULL;
		public static Animation ANIM_PLAYER_DIG;
		public static Animation ANIM_PLAYER_THROW;
		public static Animation ANIM_PLAYER_SWING;
		public static Animation ANIM_PLAYER_SWING_BIG;
		public static Animation ANIM_PLAYER_STAB;
		public static Animation ANIM_PLAYER_SPIN;
		public static Animation ANIM_PLAYER_AIM;
		public static Animation ANIM_PLAYER_JUMP;
		public static Animation ANIM_PLAYER_CAPE;
		public static Animation ANIM_PLAYER_SUBMERGED;
		public static Animation ANIM_PLAYER_DIE;
		public static Animation ANIM_PLAYER_RAISE_ONE_HAND;
		public static Animation ANIM_PLAYER_RAISE_TWO_HANDS;
		public static Animation ANIM_PLAYER_DROWN;
		public static Animation ANIM_PLAYER_FALL_HOLE;
		public static Animation ANIM_PLAYER_CRUSH_HORIZONTAL;
		public static Animation ANIM_PLAYER_INVISIBLE;

		// Monster animations.
		public static Animation ANIM_MONSTER_OCTOROK;
		public static Animation ANIM_MONSTER_MOBLIN;
		public static Animation ANIM_MONSTER_DARKNUT;
		public static Animation ANIM_MONSTER_PIG_MOBLIN;
		public static Animation ANIM_MONSTER_SHROUDED_STALFOS;
		public static Animation ANIM_MONSTER_ARM_MIMIC;
		public static Animation ANIM_MONSTER_BEAMOS;
		public static Animation ANIM_MONSTER_PINCER_HEAD;
		public static Animation ANIM_MONSTER_MINI_MOLDORM_HEAD;
		public static Animation ANIM_MONSTER_IRON_MASK;
		public static Animation ANIM_MONSTER_IRON_MASK_NAKED;
		public static Animation ANIM_MONSTER_IRON_MASK_MASK;
		public static Animation ANIM_MONSTER_BUZZ_BLOB;
		public static Animation ANIM_MONSTER_CUKEMAN;
		public static Animation ANIM_MONSTER_BUZZ_BLOB_ELECTROCUTE;
		public static Animation ANIM_MONSTER_LYNEL;
		public static Animation ANIM_MONSTER_ROPE;
		public static Animation ANIM_MONSTER_CROW;
		public static Animation ANIM_MONSTER_FLOOR_MASTER;
		public static Animation ANIM_MONSTER_FLOOR_MASTER_GRAB;
		public static Animation ANIM_MONSTER_WALL_MASTER;
		public static Animation ANIM_MONSTER_WALL_MASTER_GRAB;
		public static Animation ANIM_MONSTER_BIRI;
		public static Animation ANIM_MONSTER_BARI;
		public static Animation ANIM_MONSTER_BARI_ELECTROCUTE;
		public static Animation ANIM_MONSTER_GIBDO;
		public static Animation ANIM_MONSTER_SAND_CRAB;
		public static Animation ANIM_MONSTER_WATER_TEKTIKE;
		public static Animation ANIM_MONSTER_GOPONGA_FLOWER;
		public static Animation ANIM_MONSTER_PEAHAT;
		public static Animation ANIM_MONSTER_GEL;
		public static Animation ANIM_MONSTER_ZOL;
		public static Animation ANIM_MONSTER_ZOL_JUMP;
		public static Animation ANIM_MONSTER_ZOL_BURROW;
		public static Animation ANIM_MONSTER_ZOL_UNBURROW;
		public static Animation ANIM_MONSTER_COLOR_GEL_BODY;
		public static Animation ANIM_MONSTER_COLOR_GEL_HIGHLIGHT;
		public static Animation ANIM_MONSTER_COLOR_GEL_EYES;
		public static Animation ANIM_MONSTER_BUBBLE;
		public static Animation ANIM_MONSTER_STALFOS;
		public static Animation ANIM_MONSTER_STALFOS_JUMP;
		public static Animation ANIM_MONSTER_SPINNING_BLADE_TRAP_SLEEP;
		public static Animation ANIM_MONSTER_SPINNING_BLADE_TRAP;
		public static Animation ANIM_MONSTER_LEEVER;
		public static Animation ANIM_MONSTER_LEEVER_BURROW;
		public static Animation ANIM_MONSTER_LEEVER_UNBURROW;
		public static Animation ANIM_MONSTER_BALL_AND_CHAIN_TROOPER;
		public static Animation ANIM_MONSTER_CANDLE_HEAD;
		public static Animation ANIM_MONSTER_CANDLE_HEAD_SLEEP;
		public static Animation ANIM_MONSTER_BLADE_TRAP;
		public static Animation ANIM_MONSTER_GIANT_BLADE_TRAP;
		public static Animation ANIM_MONSTER_SPIKED_BEETLE;
		public static Animation ANIM_MONSTER_SPIKED_BEETLE_FLIPPED;
		public static Animation ANIM_MONSTER_RIVER_ZORA;
		public static Animation ANIM_MONSTER_RIVER_ZORA_SHOOT;
		public static Animation ANIM_MONSTER_RIVER_ZORA_WATER_SWIRLS;
		public static Animation ANIM_MONSTER_HARDHAT_BEETLE;
		public static Animation ANIM_MONSTER_SPINY_BEETLE;
		public static Animation ANIM_MONSTER_BEETLE;
		public static Animation ANIM_MONSTER_WIZZROBE;
		public static Animation ANIM_MONSTER_WIZZROBE_HAT;
		public static Animation ANIM_MONSTER_LIKE_LIKE;
		public static Animation ANIM_MONSTER_LIKE_LIKE_DEVOUR;
		public static Animation ANIM_MONSTER_ARMOS;
		public static Animation ANIM_MONSTER_TEKTIKE;
		public static Animation ANIM_MONSTER_TEKTIKE_JUMP;
		public static Animation ANIM_MONSTER_POLS_VOICE;
		public static Animation ANIM_MONSTER_POLS_VOICE_JUMP;

		// Monster items.
		public static Animation ANIM_MONSTER_SWORD_HOLD;

		// Colllectible animations.
		public static Animation ANIM_COLLECTIBLE_FAIRY;

		// Weapon animations.
		public static Animation ANIM_SWORD_HOLD;
		public static Animation ANIM_SWORD_CHARGED;
		public static Animation ANIM_SWORD_SWING;
		public static Animation ANIM_SWORD_SPIN;
		public static Animation ANIM_SWORD_STAB;
		public static Animation ANIM_BIG_SWORD_SWING;
		public static Animation ANIM_CANE_SWING;
		public static Animation ANIM_MAGIC_ROD_SWING;
		public static Animation ANIM_SEED_SHOOTER;
		public static Animation ANIM_SLINGSHOT_1;
		public static Animation ANIM_SLINGSHOT_2;

		// Projectile animations.
		public static Animation ANIM_ITEM_BOMB;
		public static Animation ANIM_PROJECTILE_PLAYER_ARROW;
		public static Animation ANIM_PROJECTILE_PLAYER_ARROW_CRASH;
		public static Animation ANIM_PROJECTILE_SWORD_BEAM;
		public static Animation ANIM_PROJECTILE_PLAYER_BOOMERANG_1;
		public static Animation ANIM_PROJECTILE_PLAYER_BOOMERANG_2;
		public static Animation ANIM_PROJECTILE_SWITCH_HOOK;
		public static Animation ANIM_PROJECTILE_MAGIC_ROD_FIRE;
		
		// Monster projectile animations.
		public static Animation ANIM_PROJECTILE_MONSTER_ARROW;
		public static Animation ANIM_PROJECTILE_MONSTER_ARROW_CRASH;
		public static Animation ANIM_PROJECTILE_MONSTER_SPEAR;
		public static Animation ANIM_PROJECTILE_MONSTER_BOOMERANG;
		public static Animation ANIM_PROJECTILE_MONSTER_MAGIC;
		public static Animation ANIM_PROJECTILE_MONSTER_FIREBALL;
		public static Animation ANIM_PROJECTILE_MONSTER_BONE;
		public static Animation ANIM_PROJECTILE_MONSTER_ROCK;
	
		// Effect animations.
		public static Animation ANIM_EFFECT_DIRT;
		public static Animation ANIM_EFFECT_WATER_SPLASH;
		public static Animation ANIM_EFFECT_RIPPLES;
		public static Animation ANIM_EFFECT_GRASS;
		public static Animation ANIM_EFFECT_ROCK_BREAK;
		public static Animation ANIM_EFFECT_SIGN_BREAK;
		public static Animation ANIM_EFFECT_LEAVES;
		public static Animation ANIM_EFFECT_GRASS_LEAVES;

		// Color effect animations.
		public static Animation ANIM_EFFECT_LAVA_SPLASH;
		public static Animation ANIM_EFFECT_BOMB_EXPLOSION;
		public static Animation ANIM_EFFECT_MONSTER_EXPLOSION;
		public static Animation ANIM_EFFECT_SEED_EMBER;
		public static Animation ANIM_EFFECT_SEED_SCENT;
		public static Animation ANIM_EFFECT_SEED_PEGASUS;
		public static Animation ANIM_EFFECT_SEED_GALE;
		public static Animation ANIM_EFFECT_SEED_MYSTERY;
		public static Animation ANIM_EFFECT_PEGASUS_DUST;		// The dust the player sprinkles over himself when using a pegasus seed.
		public static Animation ANIM_EFFECT_SPRINT_PUFF;
		public static Animation ANIM_EFFECT_OWL_SPARKLE;
		public static Animation ANIM_ITEM_SCENT_POD;
		public static Animation ANIM_EFFECT_FALLING_OBJECT;
		public static Animation ANIM_EFFECT_CLING;
		public static Animation ANIM_EFFECT_CLING_LIGHT;
		public static Animation ANIM_EFFECT_BURN;
		public static Animation ANIM_EFFECT_SOMARIA_BLOCK_CREATE;
		public static Animation ANIM_EFFECT_SOMARIA_BLOCK_DESTROY;

		public static Animation[] ANIM_EFFECT_SEEDS;


		//-----------------------------------------------------------------------------
		// Collision Models.
		//-----------------------------------------------------------------------------

		public static CollisionModel MODEL_BLOCK;
		public static CollisionModel MODEL_EDGE_E;
		public static CollisionModel MODEL_EDGE_N;
		public static CollisionModel MODEL_EDGE_W;
		public static CollisionModel MODEL_EDGE_S;
		public static CollisionModel MODEL_DOORWAY;
		public static CollisionModel MODEL_CORNER_NE;
		public static CollisionModel MODEL_CORNER_NW;
		public static CollisionModel MODEL_CORNER_SW;
		public static CollisionModel MODEL_CORNER_SE;
		public static CollisionModel MODEL_INSIDE_CORNER_NE;
		public static CollisionModel MODEL_INSIDE_CORNER_NW;
		public static CollisionModel MODEL_INSIDE_CORNER_SW;
		public static CollisionModel MODEL_INSIDE_CORNER_SE;
		public static CollisionModel MODEL_BRIDGE_H_TOP;
		public static CollisionModel MODEL_BRIDGE_H_BOTTOM;
		public static CollisionModel MODEL_BRIDGE_H;
		public static CollisionModel MODEL_BRIDGE_V_LEFT;
		public static CollisionModel MODEL_BRIDGE_V_RIGHT;
		public static CollisionModel MODEL_BRIDGE_V;
		public static CollisionModel MODEL_CENTER;


		//-----------------------------------------------------------------------------
		// Zones
		//-----------------------------------------------------------------------------

		public static Zone ZONE_DEFAULT;
		public static Zone ZONE_SUMMER;
		public static Zone ZONE_FOREST;
		public static Zone ZONE_GRAVEYARD;
		public static Zone ZONE_INTERIOR;
		public static Zone ZONE_PRESENT;
		public static Zone ZONE_INTERIOR_PRESENT;
		public static Zone ZONE_AGES_DUNGEON_1;


		//-----------------------------------------------------------------------------
		// Fonts
		//-----------------------------------------------------------------------------

		public static GameFont FONT_LARGE;
		public static GameFont FONT_SMALL;


		//-----------------------------------------------------------------------------
		// Shaders
		//-----------------------------------------------------------------------------
		


		//-----------------------------------------------------------------------------
		// Sound Effects
		//-----------------------------------------------------------------------------
		


		//-----------------------------------------------------------------------------
		// Music
		//-----------------------------------------------------------------------------
		


		//-----------------------------------------------------------------------------
		// Render Targets
		//-----------------------------------------------------------------------------

		public static RenderTarget2D RenderTargetGame;
		public static RenderTarget2D RenderTargetGameTemp;
		public static RenderTarget2D RenderTargetDebug;
	}
}
