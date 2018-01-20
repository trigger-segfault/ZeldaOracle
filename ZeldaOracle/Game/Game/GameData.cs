using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Worlds;
using ZeldaOracle.Common.Scripts.CustomReaders;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Game.Items.Rewards;

namespace ZeldaOracle.Game {
	
	// A static class for storing links to all game content.
	public partial class GameData {
		
		//-----------------------------------------------------------------------------
		// Initialization
		//-----------------------------------------------------------------------------

		// Initializes and loads the game content. NOTE: The order here is important.
		public static void Initialize(RewardManager rewardManager = null) {
			/*
			CommandReferenceParam param = CommandParamParser.ParseReferenceParams(
				"(any objs...)...");
				//"any name, (int gridLocationX, int gridLocationY), (int drawOffsetX, any drawOffsetY) = (0, fuck)");
				//"float x, string y, bool width, (int a, int b) = (1, 2), float height, (int hoop) = (0), bool asd");
				//"float x, string y, bool width, (int a, int b, (string c)), float height");

			Console.WriteLine(CommandParamParser.ToString(param));
			throw new LoadContentException("END");
			*/

			Console.WriteLine("Loading Palette Dictionaries");
			LoadPaletteDictionaries();

			Console.WriteLine("Loading Palettes");
			LoadPalettes();

			Console.WriteLine("Loading Shaders");
			LoadShaders();

			Console.WriteLine("Pre-Loading Zones");
			LoadZonesPreTileData();

			Console.WriteLine("Loading Images");
			LoadImages();

			Console.WriteLine("Loading Sprites");
			LoadSprites();

			Console.WriteLine("Loading Animations");
			LoadAnimations();

			Console.WriteLine("Loading Collision Models");
			LoadCollisionModels();
			
			Console.WriteLine("Loading Fonts");
			LoadFonts();

			Console.WriteLine("Loading Sound Effects");
			LoadSounds();

			Console.WriteLine("Loading Music");
			LoadMusic();

			// CONSCRIPT DESIGNER ONLY
			if (rewardManager != null) {
				Console.WriteLine("Loading Rewards");
				LoadRewards(rewardManager);
			}

			Console.WriteLine("Loading Tiles");
			LoadTiles();

			Console.WriteLine("Loading Tilesets");
			LoadTilesets();

			Console.WriteLine("Loading Zones");
			LoadZonesPostTileData();

			//Resources.LoadScript("Palettes/dungeon_ages_1_converter.conscript", new PaletteConverterSR());
		}


		//-----------------------------------------------------------------------------
		// Internal
		//-----------------------------------------------------------------------------

		// Assign static fields from their corresponding loaded resources.
		private static void IntegrateResources<T>(string prefix) {
			IEnumerable<FieldInfo> fields = typeof(GameData).GetFields()
				.Where(field =>
					field.Name.StartsWith(prefix) &&
					field.FieldType == typeof(T) &&
					field.IsStatic);

			// Set the values of the static fields to their corresponding loaded resources.
			foreach (FieldInfo field in fields) {
				string name = field.Name.ToLower().Remove(0, prefix.Length);
				
				if (Resources.ContainsResource<T>(name)) {
					field.SetValue(null, Resources.GetResource<T>(name));
				}
				else if (field.GetValue(null) != null) {
					//Console.WriteLine("** WARNING: " + name + " is built programatically.");
				}
				else {
					//Console.WriteLine("** WARNING: " + name + " is never defined.");
				}
			}
			
			// Loop through resource dictionary.
			// Find any resources that don't have corresponding fields in GameData.
			Dictionary<string, T> dictionary = Resources.GetResourceDictionary<T>();
			foreach (KeyValuePair<string, T> entry in dictionary) {
				string name = prefix.ToLower() + entry.Key;
				FieldInfo matchingField = fields.FirstOrDefault(
					field => string.Compare(field.Name, name, true) == 0);
				
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
			string prefix = "VARIANT_";
			IEnumerable<FieldInfo> fields = typeof(GameData).GetFields()
				.Where(field =>
					field.Name.StartsWith(prefix) &&
					field.FieldType == typeof(int) &&
					field.IsStatic);
			
			// Generate image variant IDs.
			int variantIndex = 0;
			foreach (FieldInfo field in fields)
				field.SetValue(null, variantIndex++);
			
			// Link image variants with their variant IDs
			Dictionary<string, Image> dictionary = Resources.GetResourceDictionary<Image>();
			foreach (Image image in dictionary.Values.Where(img => img.HasVariants)) {
				// Loop through the image's variants.
				for (Image subimg = image; subimg != null; subimg = subimg.NextVariant) {
					// Find the field with this name.
					string name = prefix.ToLower() + subimg.VariantName;
					FieldInfo matchingField = fields.FirstOrDefault(
						field => string.Compare(field.Name, name, true) == 0);
						
					// Set the image's variant ID to the field's value.
					if (matchingField != null)
						subimg.VariantID = (int) matchingField.GetValue(null);
					else
						Console.WriteLine("** WARNING: Unknown variant \"" + subimg.VariantName + "\".");
				}
			}
		}


		//-----------------------------------------------------------------------------
		// Collision Model Loading
		//-----------------------------------------------------------------------------

		private static void LoadCollisionModels() {
			Resources.LoadCollisionModels("Data/collision_models.conscript");
			IntegrateResources<CollisionModel>("MODEL_");
		}


		//-----------------------------------------------------------------------------
		// Zone Loading
		//-----------------------------------------------------------------------------

		private static void LoadZonesOld() {
			TileData ground = Resources.GetResource<TileData>("ground");
			TileData floor  = Resources.GetResource<TileData>("floor");
			//TileData background  = Resources.GetResource<TileData>("default_background");

			ZONE_DEFAULT			= new Zone("",					"(none)",			VARIANT_NONE,				ground);
			ZONE_DEFAULT.PaletteID = "present";
			ZONE_SUMMER				= new Zone("summer",			"Summer",			VARIANT_SUMMER,				ground);
			ZONE_SUMMER.PaletteID = "summer";
			ZONE_FOREST				= new Zone("forest",			"Forest",			VARIANT_FOREST,				ground);
			ZONE_FOREST.PaletteID = "present";
			ZONE_GRAVEYARD			= new Zone("graveyard",			"Graveyard",		VARIANT_GRAVEYARD,			ground);
			ZONE_GRAVEYARD.PaletteID = "present";
			ZONE_INTERIOR			= new Zone("interior",			"Interior",			VARIANT_INTERIOR,			floor);
			ZONE_INTERIOR.PaletteID = "dungeon_ages_1";
			ZONE_PRESENT			= new Zone("present",			"Present",			VARIANT_PRESENT,			floor);
			ZONE_PRESENT.StyleDefinitions.Set(StyleGroups.Cliffs, "past");
			ZONE_PRESENT.StyleDefinitions.Set(StyleGroups.Railings, "past");
			ZONE_PRESENT.PaletteID	= "present";
			ZONE_INTERIOR_PRESENT	= new Zone("interior_present",	"Interior Present",	VARIANT_INTERIOR_PRESENT,	floor);
			ZONE_INTERIOR_PRESENT.PaletteID = "dungeon_ages_1";
			ZONE_AGES_DUNGEON_1		= new Zone("ages_dungeon_1",	"Ages Dungeon 1",	VARIANT_AGES_DUNGEON_1,		floor);
			ZONE_AGES_DUNGEON_1.PaletteID = "dungeon_ages_1";
			ZONE_AGES_DUNGEON_4		= new Zone("ages_dungeon_4",	"Ages Dungeon 4",	VARIANT_AGES_DUNGEON_4,		floor);
			ZONE_AGES_DUNGEON_4.PaletteID = "dungeon_ages_1";

			ZONE_SIDESCROLL_AGES_DUNGEON_1	= new Zone("sidescroll_ages_dungeon_1",	"Ages Dungeon 1", VARIANT_AGES_DUNGEON_1, floor);
			ZONE_SIDESCROLL_AGES_DUNGEON_1.IsSideScrolling = true;
			ZONE_SIDESCROLL_AGES_DUNGEON_1.PaletteID = "dungeon_ages_1";

			ZONE_UNDERWATER_PRESENT	= new Zone("underwater_present", "Underwater Present", VARIANT_UNDERWATER_PRESENT, ground);
			ZONE_UNDERWATER_PRESENT.IsUnderwater = true;
			ZONE_UNDERWATER_PRESENT.PaletteID = "present";

			Resources.AddResource("default",	ZONE_DEFAULT);
			Resources.AddResource("summer",		ZONE_SUMMER);
			Resources.AddResource("forest",		ZONE_FOREST);
			Resources.AddResource("graveyard",	ZONE_GRAVEYARD);
			Resources.AddResource("interior",	ZONE_INTERIOR);
			Resources.AddResource("present",	ZONE_PRESENT);
			Resources.AddResource("interior_present", ZONE_INTERIOR_PRESENT);
			Resources.AddResource("ages_dungeon_1", ZONE_AGES_DUNGEON_1);
			Resources.AddResource("ages_dungeon_4", ZONE_AGES_DUNGEON_4);
			Resources.AddResource("sidescroll_ages_dungeon_1", ZONE_SIDESCROLL_AGES_DUNGEON_1);
			Resources.AddResource("underwater_present", ZONE_UNDERWATER_PRESENT);
		}

		private static void LoadZonesPreTileData() {
			Resources.LoadZones("Zones/zones.conscript", false);
			IntegrateResources<Zone>("ZONE_");
		}

		private static void LoadZonesPostTileData() {
			Resources.LoadZones("Zones/zones.conscript", true);
			IntegrateResources<Zone>("ZONE_");
		}

		//-----------------------------------------------------------------------------
		// Tliesets Loading
		//-----------------------------------------------------------------------------

		private static void LoadTilesets() {
			// Load tilesets and tile data.
			Resources.LoadTilesets("Tilesets/tilesets.conscript");
			/*Resources.LoadTilesets("Tilesets/cliffs.conscript");

			Resources.LoadTilesets("Tilesets/objects_nv.conscript");
			Resources.LoadTilesets("Tilesets/objects.conscript");

			Resources.LoadTilesets("Tilesets/land.conscript");
			Resources.LoadTilesets("Tilesets/forest.conscript");
			Resources.LoadTilesets("Tilesets/water.conscript");
			Resources.LoadTilesets("Tilesets/town.conscript");
			Resources.LoadTilesets("Tilesets/interior.conscript");
			Resources.LoadTilesets("Tilesets/dungeon.conscript");
			Resources.LoadTilesets("Tilesets/sidescroll.conscript");
			
			Resources.LoadTilesets("Tilesets/event_tile_data.conscript");*/
			// OLD Tilesets:
			//Resources.LoadTilesets("Tilesets/tile_data.conscript");
			//Resources.LoadTilesets("Tilesets/overworld.conscript");
			//Resources.LoadTilesets("Tilesets/interior2.conscript");

			IntegrateResources<Tileset>("TILESET_");
		}

		private static void LoadTiles() {
			Resources.LoadTiles("Tiles/tiles.conscript");
		}
		/*
		private static EventTileData CreateMonsterEvent(int sx, int sy, string id, string animation, Type monsterType, MonsterColor color) {
			EventTileData etd = new EventTileData(typeof(MonsterEvent));
			etd.Sprite = Resources.GetAnimation(animation);

			etd.Properties.Set("color", (int) color)
				.SetDocumentation("Color", "enum", "MonsterColor", "", "The color of the monster.");
			etd.Properties.Set("respawn_type", "Normal")
				.SetDocumentation("Respawn Type", "enum", "MonsterRespawnType", "", "When a monster respawns.");
			etd.Properties.Set("monster_type", monsterType.Name)
				.SetDocumentation("Monster Type", "string", "", "", "When a monster respawns.");
			etd.Properties.Set("dead", false);

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
		}*/


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
		// Palette Loading
		//-----------------------------------------------------------------------------

		// Loads the palette dictionaries.
		private static void LoadPaletteDictionaries() {
			Resources.LoadPaletteDictionaries("Palettes/Dictionaries/palette_dictionaries.conscript");

			IntegrateResources<PaletteDictionary>("PAL_");
		}

		// Loads the palettes.
		private static void LoadPalettes() {
			Resources.LoadPalettes("Palettes/palettes.conscript");

			IntegrateResources<Palette>("PAL_");
		}

		//-----------------------------------------------------------------------------
		// Shader Loading
		//-----------------------------------------------------------------------------

		// Loads the shaders.
		private static void LoadShaders() {
			PALETTE_SHADER		= Resources.LoadShader("Shaders/palette_shader");
			PALETTE_LERP_SHADER	= Resources.LoadShader("Shaders/palette_lerp_shader");
			
			GameSettings.DRAW_MODE_DEFAULT.Effect = PALETTE_LERP_SHADER;

			PaletteShader = new PaletteShader(PALETTE_LERP_SHADER);
			PaletteShader.TilePalette = PAL_TILES_DEFAULT;
			PaletteShader.EntityPalette = PAL_ENTITIES_DEFAULT;
			//PaletteShader.EntityPalette = PAL_ENTITIES_ELECTROCUTED;
		}

		public static Effect PALETTE_SHADER;
		public static Effect PALETTE_LERP_SHADER;

		//-----------------------------------------------------------------------------
		// Tilesets
		//-----------------------------------------------------------------------------

		public static Tileset TILESET_OVERWORLD;
		public static Tileset TILESET_INTERIOR;
		public static Tileset TILESET_CLIFFS;


		//-----------------------------------------------------------------------------
		// Image Variants
		//-----------------------------------------------------------------------------

		public static int VARIANT_NONE;
		public static int VARIANT_NORMAL;
		public static int VARIANT_DARK;
		public static int VARIANT_LIGHT;
		public static int VARIANT_RED;
		public static int VARIANT_BLUE;
		public static int VARIANT_GREEN;
		public static int VARIANT_YELLOW;
		public static int VARIANT_ORANGE;
		public static int VARIANT_HURT;
		public static int VARIANT_SUMMER;
		public static int VARIANT_FOREST;
		public static int VARIANT_GRAVEYARD;
		public static int VARIANT_INTERIOR;
		public static int VARIANT_PRESENT;
		public static int VARIANT_INTERIOR_PRESENT;
		public static int VARIANT_UNDERWATER_PRESENT;
		public static int VARIANT_AGES_DUNGEON_1;
		public static int VARIANT_AGES_DUNGEON_2;
		public static int VARIANT_AGES_DUNGEON_3;
		public static int VARIANT_AGES_DUNGEON_4;
		

		//-----------------------------------------------------------------------------
		// Collision Models.
		//-----------------------------------------------------------------------------

		public static CollisionModel MODEL_BLOCK;
		public static CollisionModel MODEL_EDGE_E;
		public static CollisionModel MODEL_EDGE_N;
		public static CollisionModel MODEL_EDGE_W;
		public static CollisionModel MODEL_EDGE_S;
		public static CollisionModel MODEL_DOORWAY;
		public static CollisionModel MODEL_DOORWAY_HALF_RIGHT;
		public static CollisionModel MODEL_DOORWAY_HALF_LEFT;
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
		public static Zone ZONE_SIDESCROLL_AGES_DUNGEON_1;
		public static Zone ZONE_UNDERWATER_PRESENT;


		//-----------------------------------------------------------------------------
		// Fonts
		//-----------------------------------------------------------------------------

		public static GameFont FONT_LARGE;
		public static GameFont FONT_SMALL;


		//-----------------------------------------------------------------------------
		// Palettes
		//-----------------------------------------------------------------------------
		
		public static PaletteDictionary PAL_TILE_DICTIONARY;
		public static PaletteDictionary PAL_ENTITY_DICTIONARY;
		
		public static Palette PAL_ENTITIES_DEFAULT;
		public static Palette PAL_ENTITIES_MENU;
		public static Palette PAL_ENTITIES_ELECTROCUTED;

		public static Palette PAL_TILES_DEFAULT;
		public static Palette PAL_PRESENT;
		public static Palette PAL_SUMMER;
		public static Palette PAL_DUNGEON_AGES_1;
		public static Palette PAL_TILES_ELECTROCUTED;


		//-----------------------------------------------------------------------------
		// Render Targets
		//-----------------------------------------------------------------------------

		public static PaletteShader PaletteShader;

		public static RenderTarget2D RenderTargetGame;
		public static RenderTarget2D RenderTargetGameTemp;
		public static RenderTarget2D RenderTargetDebug;
	}
}
