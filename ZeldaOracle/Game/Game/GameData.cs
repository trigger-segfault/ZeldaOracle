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

namespace ZeldaOracle.Game {
	
	// A static class for storing links to all game content.
	public partial class GameData {


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

			Console.WriteLine("Loading Zones");
			LoadZones();

			Console.WriteLine("Loading Tilesets");
			LoadTilesets();

			Console.WriteLine("Loading Fonts");
			LoadFonts();

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

			//Resources.GetImage("screen_dungeon_map");

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

		/*
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
		*/

		/*
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
		*/
		
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
		/*
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
		*/
		// TODO: Get rid of property actions and migrate their purpose to the Editor
		// TODO: Lantern lit states in editor
		// TODO: Chest looted states in editor
		// TODO: Level/room change zones
		private static void LoadPropertyActions() {
			/*Resources.AddResource<PropertyAction>("zone", delegate(IPropertyObject sender, object value) {
				if (sender is Room) {
					Room room = sender as Room;
					room.Zone = Resources.GetResource<Zone>((string)value);
				}
				else if (sender is Level) {
					Level level = sender as Level;
					level.Zone = Resources.GetResource<Zone>((string)value);
				}
			});*/
			/*
			Resources.AddResource<PropertyAction>("looted", delegate(IPropertyObject sender, object value) {
				if (IsTileOrTileDataInstanceOf(sender, typeof(TileChest))) {
					sender.Properties.Set("sprite_index", (bool)value ? 1 : 0);
				}
			});*/
			/* 
			Resources.AddResource<PropertyAction>("lit", delegate(IPropertyObject sender, object value) {
				if (IsTileOrTileDataInstanceOf(sender, typeof(TileLantern))) {
					sender.Properties.Set("sprite_index", (bool)value ? 0 : 1);
				}
			});*/
			/*Resources.AddResource<PropertyAction>("direction", delegate(IPropertyObject sender, object value) {
				if (IsTileOrTileDataInstanceOf(sender, typeof(NPCEvent))) {
					sender.Properties.Set("substrip_index", (int) value);
				}
			});*/
			/*Resources.AddResource<PropertyAction>("warp_type", delegate(IPropertyObject sender, object value) {
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
			});*/
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

			// Create a warp event.
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
			
			// Create monster events.
			CreateMonsterEvent(2, 0, "monster_octorok_red",		"monster_octorok",	typeof(MonsterOctorok),	MonsterColor.Red);
			CreateMonsterEvent(3, 0, "monster_octorok_blue",	"monster_octorok",	typeof(MonsterOctorok),	MonsterColor.Blue);
			CreateMonsterEvent(4, 0, "monster_moblin_red",		"monster_moblin",	typeof(MonsterMoblin),	MonsterColor.Red);
			CreateMonsterEvent(5, 0, "monster_moblin_blue",		"monster_moblin",	typeof(MonsterMoblin),	MonsterColor.Blue);
			CreateMonsterEvent(6, 0, "monster_gibdo",			"monster_gibdo",	typeof(MonsterGibdo),	MonsterColor.Red);
			CreateMonsterEvent(7, 0, "monster_rope",			"monster_rope",		typeof(MonsterRope),	MonsterColor.Green);
			CreateMonsterEvent(9, 0, "monster_beetle",			"monster_beetle",	typeof(MonsterBeetle),	MonsterColor.Green);
			CreateMonsterEvent(8, 0, "monster_lynel",			"monster_lynel",	typeof(MonsterLynel),	MonsterColor.Red);

			// Create a pit tile data.
			TileData pitTile = new TileData();
			pitTile.EnvironmentType = TileEnvironmentType.Hole;
			pitTile.Sprite = GameData.SPR_TILE_PIT;
			Resources.AddResource("pit", pitTile);

			IntegrateResources<Tileset>("TILESET_");
		}

		private static EventTileData CreateMonsterEvent(int sx, int sy, string id, string animation, Type monsterType, MonsterColor color) {
			EventTileData etd = new EventTileData(typeof(MonsterEvent));
			etd.Sprite = Resources.GetAnimation(animation);

			etd.Properties.Set("color", (int) color)
				.SetDocumentation("Color", "enum", "MonsterColor", "", "The color of the monster.");
			etd.Properties.Set("respawn_type", "Normal")
				.SetDocumentation("Respawn Type", "enum", "MonsterRespawnType", "", "When a monster respawns.");
			etd.Properties.Set("monster_type", monsterType.Name)
				.SetDocumentation("Monster Type", "string", "", "", "When a monster respawns.");

			etd.Events.AddEvent("event_die", "Die", "Occurs when the monster dies.", new ScriptParameter("Monster", "monster"));
			etd.Properties.Set("event_die", "")
				.SetDocumentation("Die", "script", "", "", "Occurs when the monster dies.");

			etd.Properties.Set("substrip_index", Directions.Down);
			if (color == MonsterColor.Red)
				etd.Properties.Set("image_variant", GameData.VARIANT_RED);
			else if (color == MonsterColor.Blue)
				etd.Properties.Set("image_variant", GameData.VARIANT_BLUE);
			else if (color == MonsterColor.Green)
				etd.Properties.Set("image_variant", GameData.VARIANT_GREEN);
			else if (color == MonsterColor.Orange)
				etd.Properties.Set("image_variant", GameData.VARIANT_ORANGE);

			Resources.AddResource(id, etd);
			EventTileset eventTileset = Resources.GetResource<EventTileset>("events");
			eventTileset.TileData[sx, sy] = etd;
			return etd;
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
		// Sound Loading
		//-----------------------------------------------------------------------------

		// Loads the sound effects.
		private static void LoadSounds() {
			Resources.LoadSounds(Resources.SoundDirectory + "sounds.conscript");

		}

		// Loads the music.
		private static void LoadMusic() {
			Resources.LoadMusic(Resources.MusicDirectory + "music.conscript");
		}


		//-----------------------------------------------------------------------------
		// Tilesets
		//-----------------------------------------------------------------------------

		public static Tileset TILESET_OVERWORLD;
		public static Tileset TILESET_INTERIOR;
		public static Tileset TILESET_CLIFFS;


		//-----------------------------------------------------------------------------
		// Image Variants
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
		public static Zone ZONE_AGES_DUNGEON_1; // Spirit's Grave
		public static Zone ZONE_AGES_DUNGEON_4; // Skull Dungeon


		//-----------------------------------------------------------------------------
		// Fonts
		//-----------------------------------------------------------------------------

		public static GameFont FONT_LARGE;
		public static GameFont FONT_SMALL;


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
