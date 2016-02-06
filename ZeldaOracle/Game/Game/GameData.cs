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

		// Initializes and loads the game content. NOTE: The order here is important.
		public static void Initialize() {
			/*
			CommandReferenceParam param = CommandParamParser.ParseReferenceParams(
				"(any objs...)...");
				//"any name, (int gridLocationX, int gridLocationY), (int drawOffsetX, any drawOffsetY) = (0, fuck)");
				//"float x, string y, bool width, (int a, int b) = (1, 2), float height, (int hoop) = (0), bool asd");
				//"float x, string y, bool width, (int a, int b, (string c)), float height");

			Console.WriteLine(CommandParamParser.ToString(param));
			throw new LoadContentException("END");
			*/

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

			Console.WriteLine("Loading Tilesets");
			LoadTilesets();

			Console.WriteLine("Loading Zones");
			LoadZones();
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
				
				if (Resources.ExistsResource<T>(name)) {
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
					field => String.Compare(field.Name, name, true) == 0);
				
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
						field => String.Compare(field.Name, name, true) == 0);
						
					// Set the image's variant ID to the field's value.
					if (matchingField != null)
						subimg.VariantID = (int) matchingField.GetValue(null);
					else
						Console.WriteLine("** WARNING: Uknown variant \"" + subimg.VariantName + "\".");
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

		private static void LoadZones() {
			TileData ground = Resources.GetResource<TileData>("default_ground");
			TileData floor  = Resources.GetResource<TileData>("default_floor");

			ZONE_DEFAULT			= new Zone("",					"(none)",			VARIANT_NONE,				ground);
			ZONE_SUMMER				= new Zone("summer",			"Summer",			VARIANT_SUMMER,				ground);
			ZONE_FOREST				= new Zone("forest",			"Forest",			VARIANT_FOREST,				ground);
			ZONE_GRAVEYARD			= new Zone("graveyard",			"Graveyard",		VARIANT_GRAVEYARD,			ground);
			ZONE_INTERIOR			= new Zone("interior",			"Interior",			VARIANT_INTERIOR,			floor);
			ZONE_PRESENT			= new Zone("present",			"Present",			VARIANT_PRESENT,			floor);
			ZONE_INTERIOR_PRESENT	= new Zone("interior_present",	"Interior Present",	VARIANT_INTERIOR_PRESENT,	floor);
			ZONE_AGES_DUNGEON_1		= new Zone("ages_dungeon_1",	"Ages Dungeon 1",	VARIANT_AGES_DUNGEON_1,		floor);
			ZONE_AGES_DUNGEON_4		= new Zone("ages_dungeon_4",	"Ages Dungeon 4",	VARIANT_AGES_DUNGEON_4,		floor);

			Resources.AddResource("default",	ZONE_DEFAULT);
			Resources.AddResource("summer",		ZONE_SUMMER);
			Resources.AddResource("forest",		ZONE_FOREST);
			Resources.AddResource("graveyard",	ZONE_GRAVEYARD);
			Resources.AddResource("interior",	ZONE_INTERIOR);
			Resources.AddResource("present",	ZONE_PRESENT);
			Resources.AddResource("interior_present", ZONE_INTERIOR_PRESENT);
			Resources.AddResource("ages_dungeon_1", ZONE_AGES_DUNGEON_1);
			Resources.AddResource("ages_dungeon_4", ZONE_AGES_DUNGEON_4);
		}

		//-----------------------------------------------------------------------------
		// Tliesets Loading
		//-----------------------------------------------------------------------------

		private static void LoadTilesets() {
			// Load tilesets and tile data.
			Resources.LoadTilesets("Tilesets/cliffs.conscript");

			Resources.LoadTilesets("Tilesets/objects_nv.conscript");
			Resources.LoadTilesets("Tilesets/objects.conscript");

			Resources.LoadTilesets("Tilesets/land.conscript");
			Resources.LoadTilesets("Tilesets/forest.conscript");
			Resources.LoadTilesets("Tilesets/water.conscript");
			Resources.LoadTilesets("Tilesets/town.conscript");
			Resources.LoadTilesets("Tilesets/interior.conscript");
			Resources.LoadTilesets("Tilesets/dungeon.conscript");
			
			Resources.LoadTilesets("Tilesets/event_tile_data.conscript");
			// OLD Tilesets:
			//Resources.LoadTilesets("Tilesets/tile_data.conscript");
			//Resources.LoadTilesets("Tilesets/overworld.conscript");
			//Resources.LoadTilesets("Tilesets/interior2.conscript");

			IntegrateResources<Tileset>("TILESET_");
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


		//-----------------------------------------------------------------------------
		// Fonts
		//-----------------------------------------------------------------------------

		public static GameFont FONT_LARGE;
		public static GameFont FONT_SMALL;


		//-----------------------------------------------------------------------------
		// Render Targets
		//-----------------------------------------------------------------------------

		public static RenderTarget2D RenderTargetGame;
		public static RenderTarget2D RenderTargetGameTemp;
		public static RenderTarget2D RenderTargetDebug;
	}
}
