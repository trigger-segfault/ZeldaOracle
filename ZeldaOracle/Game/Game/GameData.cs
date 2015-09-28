using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ZeldaOracle.Common.Collision;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Content.ResourceBuilders;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Properties;
using ZeldaOracle.Common.Scripts;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Worlds;

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
					else if (field.GetValue(null) != null)
						Console.WriteLine("** WARNING: " + name + " is built programatically.");
					else 
						Console.WriteLine("** WARNING: " + name + " is never defined.");
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
				if (matchingField == null)
					Console.WriteLine("** WARNING: Resource \"" + name + "\" does not have a corresponding field.");
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
			
			// TEMPORARY: Create seed sprite array here.
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
			
			IntegrateResources<Animation>("ANIM_");
		}

		
		//-----------------------------------------------------------------------------
		// Collision Models Loading
		//-----------------------------------------------------------------------------

		private static void LoadCollisionModels() {
			Resources.LoadCollisionModels("Data/collision_models.conscript");
			IntegrateResources<CollisionModel>("MODEL_");
		}


		//-----------------------------------------------------------------------------
		// Zone Loading
		//-----------------------------------------------------------------------------

		private static void LoadZones() {
			ZONE_SUMMER		= new Zone("summer",	"Summer",		VARIANT_SUMMER);
			ZONE_FOREST		= new Zone("forest",	"Forest",		VARIANT_FOREST);
			ZONE_GRAVEYARD	= new Zone("graveyard",	"Graveyard",	VARIANT_GRAVEYARD);
			ZONE_INTERIOR	= new Zone("interior",	"Interior",		VARIANT_INTERIOR);

			Resources.AddResource("summer",		ZONE_SUMMER);
			Resources.AddResource("forest",		ZONE_FOREST);
			Resources.AddResource("graveyard",	ZONE_GRAVEYARD);
			Resources.AddResource("interior",	ZONE_INTERIOR);
		}

		//-----------------------------------------------------------------------------
		// Tliesets Loading
		//-----------------------------------------------------------------------------

		private static TilesetBuilder tilesetBuilder;

		private static TilesetBuilder BuildTile(int tileX, int tileY) {
			tilesetBuilder.Begin(tileX, tileY);
			return tilesetBuilder;
		}

		private static void LoadTilesets() {
			tilesetBuilder = new TilesetBuilder();

			// OVERWORLD TILESET:
			TILESET_OVERWORLD = new Tileset("overworld", GameData.SHEET_TILESET_OVERWORLD, 21, 36);
			TILESET_OVERWORLD.LoadConfig("Content/Tilesets/overworld.txt");
			TILESET_OVERWORLD.DefaultTile = new Point2I(1, 25);
			tilesetBuilder.Tileset = TILESET_OVERWORLD;

			Resources.AddResource(TILESET_OVERWORLD.ID, TILESET_OVERWORLD);

			// Animations.
			BuildTile( 1, 15).SetAnim(ANIM_TILE_WATER);
			BuildTile( 2, 15).SetAnim(ANIM_TILE_WATER_DEEP);
			BuildTile( 1, 14).SetAnim(ANIM_TILE_OCEAN);
			BuildTile( 2, 14).SetAnim(ANIM_TILE_OCEAN_SHORE);
			BuildTile( 1, 16).SetAnim(ANIM_TILE_PUDDLE);
			BuildTile( 0, 14).SetAnim(ANIM_TILE_WATERFALL_TOP);
			BuildTile( 0, 15).SetAnim(ANIM_TILE_WATERFALL);
			BuildTile( 0, 16).SetAnim(ANIM_TILE_WATERFALL_BOTTOM);
			BuildTile( 3, 23).SetAnim(ANIM_TILE_FLOWERS);
			// Cave entrances
			BuildTile( 1,  4).SetSolidModel(MODEL_EDGE_N); 
			BuildTile( 0,  5).SetSolidModel(MODEL_EDGE_N);
			BuildTile( 1,  5).SetSolidModel(MODEL_EDGE_N);
			BuildTile(12, 26).SetSolidModel(MODEL_EDGE_N);
			// Tree entrances
			BuildTile(12, 28).SetSolidModel(MODEL_EDGE_N);
			BuildTile(17, 22).SetSolidModel(MODEL_EDGE_N);
			BuildTile(13,  7).SetSolidModel(MODEL_DOORWAY);
			// Doorways
			BuildTile(13,  8).SetSolidModel(MODEL_DOORWAY);
			BuildTile(13,  9).SetSolidModel(MODEL_DOORWAY);
			BuildTile(19,  9).SetSolidModel(MODEL_CORNER_NW);
			BuildTile(20,  9).SetSolidModel(MODEL_CORNER_NE);
			BuildTile(15, 17).SetSolidModel(MODEL_DOORWAY);
			BuildTile(15, 18).SetSolidModel(MODEL_DOORWAY);
			BuildTile(15, 19).SetSolidModel(MODEL_DOORWAY);
			BuildTile(12, 18).SetSolidModel(MODEL_CORNER_NW);
			BuildTile(13, 18).SetSolidModel(MODEL_CORNER_NE);
			// Bridges
			BuildTile(15, 12).SetSolidModel(MODEL_BRIDGE_V_LEFT);
			BuildTile(17, 12).SetSolidModel(MODEL_BRIDGE_V_RIGHT);
			BuildTile(18, 12).SetSolidModel(MODEL_BRIDGE_V);
			BuildTile(15, 13).SetSolidModel(MODEL_BRIDGE_V_LEFT);
			BuildTile(17, 13).SetSolidModel(MODEL_BRIDGE_V_RIGHT);
			BuildTile(18, 13).SetSolidModel(MODEL_BRIDGE_V);
			BuildTile(19, 10).SetSolidModel(MODEL_BRIDGE_H);
			BuildTile(19, 11).SetSolidModel(MODEL_BRIDGE_H_TOP);
			BuildTile(19, 13).SetSolidModel(MODEL_BRIDGE_H_BOTTOM);
			BuildTile(20, 10).SetSolidModel(MODEL_BRIDGE_H);
			BuildTile(20, 11).SetSolidModel(MODEL_BRIDGE_H_TOP);
			BuildTile(20, 13).SetSolidModel(MODEL_BRIDGE_H_BOTTOM);
			// Irregular blocks/Ledges
			BuildTile( 0, 17).SetSolidModel(MODEL_CORNER_NW);
			BuildTile( 1, 17).CreateLedge(MODEL_EDGE_N, Directions.North);
			BuildTile( 2, 17).SetSolidModel(MODEL_CORNER_NE);
			BuildTile( 4, 17).SetSolidModel(MODEL_CORNER_NW);
			BuildTile( 5, 17).CreateLedge(MODEL_EDGE_N, Directions.North);
			BuildTile( 6, 17).SetSolidModel(MODEL_CORNER_NE);
			BuildTile( 0, 18).CreateLedge(MODEL_EDGE_W, Directions.West);
			BuildTile( 2, 18).CreateLedge(MODEL_EDGE_E, Directions.East);
			BuildTile( 3, 18).SetSolidModel(MODEL_EDGE_E);
			BuildTile( 4, 18).CreateLedge(MODEL_EDGE_W, Directions.West);
			BuildTile( 6, 18).CreateLedge(MODEL_EDGE_E, Directions.East);
			BuildTile( 7, 18).SetSolidModel(MODEL_EDGE_E);
			BuildTile( 0, 19).SetSolidModel(MODEL_CORNER_SW);
			BuildTile( 1, 19).CreateLedge(MODEL_EDGE_S, Directions.South);
			BuildTile( 2, 19).SetSolidModel(MODEL_CORNER_SE);
			BuildTile( 3, 19).SetSolidModel(MODEL_EDGE_W);
			BuildTile( 4, 19).SetSolidModel(MODEL_CORNER_SW);
			BuildTile( 5, 19).CreateLedge(MODEL_EDGE_S, Directions.South);
			BuildTile( 6, 19).SetSolidModel(MODEL_CORNER_SE);
			BuildTile( 7, 19).SetSolidModel(MODEL_EDGE_W);
			BuildTile( 0, 20).SetSolidModel(MODEL_CORNER_NW);
			BuildTile( 1, 20).SetSolidModel(MODEL_CORNER_NE);
			BuildTile( 2, 20).SetSolidModel(MODEL_INSIDE_CORNER_SE);
			BuildTile( 3, 20).SetSolidModel(MODEL_INSIDE_CORNER_SW);
			BuildTile( 4, 20).SetSolidModel(MODEL_CORNER_NW);
			BuildTile( 5, 20).SetSolidModel(MODEL_CORNER_NE);
			BuildTile( 6, 20).SetSolidModel(MODEL_INSIDE_CORNER_SE);
			BuildTile( 7, 20).SetSolidModel(MODEL_INSIDE_CORNER_SW);
			BuildTile( 0, 21).SetSolidModel(MODEL_CORNER_SW);
			BuildTile( 1, 21).SetSolidModel(MODEL_CORNER_SE);
			BuildTile( 2, 21).SetSolidModel(MODEL_INSIDE_CORNER_NE);
			BuildTile( 3, 21).SetSolidModel(MODEL_INSIDE_CORNER_NW);
			BuildTile( 4, 21).SetSolidModel(MODEL_CORNER_SW);
			BuildTile( 5, 21).SetSolidModel(MODEL_CORNER_SE);
			BuildTile( 6, 21).SetSolidModel(MODEL_INSIDE_CORNER_NE);
			BuildTile( 7, 21).SetSolidModel(MODEL_INSIDE_CORNER_NW);
			BuildTile( 1,  3).CreateLedge(MODEL_BLOCK, Directions.Down);
			BuildTile( 9,  9).CreateLedge(MODEL_EDGE_S, Directions.Down);
			BuildTile( 9, 10).CreateLedge(MODEL_BLOCK, Directions.Down);
			BuildTile( 9, 11).CreateLedge(MODEL_BLOCK, Directions.Down);

			
			// INTERIOR TILESET:
			TILESET_INTERIOR = new Tileset("interior", GameData.SHEET_TILESET_INTERIOR, 10, 15);
			TILESET_INTERIOR.LoadConfig("Content/Tilesets/interior.txt");
			TILESET_INTERIOR.DefaultTile = new Point2I(4, 1);
			tilesetBuilder.Tileset = TILESET_INTERIOR;

			Resources.AddResource(TILESET_INTERIOR.ID, TILESET_INTERIOR);

			// Animations.
			BuildTile( 7,  9).SetAnim(ANIM_TILE_LANTERN);
			BuildTile( 4,  4).SetAnim(ANIM_TILE_WATER);
			BuildTile( 4,  3).SetAnim(ANIM_TILE_PUDDLE);
			BuildTile( 3,  4).SetAnim(ANIM_TILE_WATERFALL);
			BuildTile( 3,  5).SetAnim(ANIM_TILE_WATERFALL_BOTTOM);

			BuildTile(1, 0).CreateLedge(MODEL_BLOCK, Directions.Up);
			BuildTile(0, 1).CreateLedge(MODEL_BLOCK, Directions.Left);
			BuildTile(2, 1).CreateLedge(MODEL_BLOCK, Directions.Right);
			BuildTile(1, 2).CreateLedge(MODEL_BLOCK, Directions.Down);
			//setModelLedge(1, 0, MODEL_BLOCK, Direction.NORTH);
			//setModelLedge(0, 1, MODEL_BLOCK, Direction.WEST);
			//setModelLedge(2, 1, MODEL_BLOCK, Direction.EAST);
			//setModelLedge(1, 2, MODEL_BLOCK, Direction.SOUTH);
			// setModel(6, 1, MODEL_EDGE_N); // Cave
			// setModel(7, 1, MODEL_EDGE_S); // Cave
			// setModel(8, 1, MODEL_EDGE_W); // Cave
			// setModel(9, 1, MODEL_EDGE_E); // Cave

			BuildTile(8, 2).SetSolidModel(MODEL_DOORWAY); // stairs down
			BuildTile(9, 2).SetSolidModel(MODEL_DOORWAY); // stairs up

			BuildTile(9, 3).SetSolidModel(MODEL_EDGE_N); // fireplace
			BuildTile(8, 4).SetSolidModel(MODEL_EDGE_N); // fireplace Post
			BuildTile(8, 5).SetSolidModel(MODEL_EDGE_W); // entrance left
			BuildTile(9, 5).SetSolidModel(MODEL_EDGE_E); // entrance right

			BuildTile(4, 8).SetSolidModel(MODEL_EDGE_N); // bookshelves
			BuildTile(5, 8).SetSolidModel(MODEL_EDGE_N); // drawers
			BuildTile(6, 8).SetSolidModel(MODEL_EDGE_N); // shelves

			for (int i = 0; i < 3; i++) {
				int x = i * 3;
				BuildTile(x + 0, 10).SetSolidModel(MODEL_CORNER_NW);
				BuildTile(x + 1, 10).SetSolidModel(MODEL_EDGE_N);
				BuildTile(x + 2, 10).SetSolidModel(MODEL_CORNER_NE);
				BuildTile(x + 0, 11).SetSolidModel(MODEL_EDGE_W);
				BuildTile(x + 2, 11).SetSolidModel(MODEL_EDGE_E);
				BuildTile(x + 0, 12).SetSolidModel(MODEL_CORNER_SW);
				BuildTile(x + 1, 12).SetSolidModel(MODEL_EDGE_S);
				BuildTile(x + 2, 12).SetSolidModel(MODEL_CORNER_SE);
				BuildTile(x + 0, 13).SetSolidModel(MODEL_INSIDE_CORNER_NW);
				BuildTile(x + 1, 13).SetSolidModel(MODEL_INSIDE_CORNER_NE);
				BuildTile(x + 0, 14).SetSolidModel(MODEL_INSIDE_CORNER_SW);
				BuildTile(x + 1, 14).SetSolidModel(MODEL_INSIDE_CORNER_SE);
			}


			TILESETS = new Tileset[] { TILESET_OVERWORLD, TILESET_INTERIOR };
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
		// Tilesets
		//-----------------------------------------------------------------------------

		public static Tileset TILESET_OVERWORLD;
		public static Tileset TILESET_INTERIOR;
		public static Tileset[] TILESETS;


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

		// Item Icons.
		public static Sprite SPR_ITEM_SEED_EMBER;
		public static Sprite SPR_ITEM_SEED_SCENT;
		public static Sprite SPR_ITEM_SEED_PEGASUS;
		public static Sprite SPR_ITEM_SEED_GALE;
		public static Sprite SPR_ITEM_SEED_MYSTERY;
		public static Sprite[] SPR_ITEM_SEEDS;
		
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
		public static Sprite SPR_ITEM_ICON_SWITCH_HOOK_1;
		public static Sprite SPR_ITEM_ICON_SWITCH_HOOK_2;
		public static Sprite SPR_ITEM_ICON_MAGNET_GLOVES_BLUE;
		public static Sprite SPR_ITEM_ICON_MAGNET_GLOVES_RED;
		public static Sprite SPR_ITEM_ICON_CANE;
		public static Sprite SPR_ITEM_ICON_FIRE_ROD;
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
		public static Animation ANIM_PLAYER_SUBMERGED;
		public static Animation ANIM_PLAYER_DIE;
		public static Animation ANIM_PLAYER_FALL;
		public static Animation ANIM_PLAYER_DROWN;
		public static Animation ANIM_PLAYER_RAISE_ONE_HAND;
		public static Animation ANIM_PLAYER_RAISE_TWO_HANDS;

		// Weapon animations.
		public static Animation ANIM_SWORD_HOLD;
		public static Animation ANIM_SWORD_CHARGED;
		public static Animation ANIM_SWORD_SWING;
		public static Animation ANIM_SWORD_SPIN;
		public static Animation ANIM_SWORD_STAB;

		// Projectile animations.
		public static Animation ANIM_ITEM_BOMB;
		public static Animation ANIM_PROJECTILE_PLAYER_ARROW;
		public static Animation ANIM_PROJECTILE_PLAYER_ARROW_CRASH;
	
		// Effect animations.
		public static Animation ANIM_EFFECT_DIRT;
		public static Animation ANIM_EFFECT_WATER_SPLASH;
		public static Animation ANIM_EFFECT_LAVA_SPLASH;
		public static Animation ANIM_EFFECT_RIPPLES;
		public static Animation ANIM_EFFECT_GRASS;
		public static Animation ANIM_EFFECT_ROCK_BREAK;
		public static Animation ANIM_EFFECT_SIGN_BREAK;
		public static Animation ANIM_EFFECT_LEAVES;
		public static Animation ANIM_EFFECT_GRASS_LEAVES;
		
		// Color effect animations.
		public static Animation ANIM_EFFECT_BOMB_EXPLOSION;
		public static Animation ANIM_EFFECT_MONSTER_EXPLOSION;
		public static Animation ANIM_EFFECT_SEED_EMBER;
		public static Animation ANIM_EFFECT_SEED_SCENT;
		public static Animation ANIM_EFFECT_SEED_PEGASUS;
		public static Animation ANIM_EFFECT_SEED_GALE;
		public static Animation ANIM_EFFECT_SEED_MYSTERY;
		public static Animation ANIM_EFFECT_PEGASUS_DUST;		// The dust the player sprinkles over himself when using a pegasus seed.
		public static Animation ANIM_EFFECT_OWL_SPARKLE;
		public static Animation ANIM_ITEM_SCENT_POD;
		public static Animation ANIM_EFFECT_FALLING_OBJECT;


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

		public static Zone ZONE_SUMMER;
		public static Zone ZONE_FOREST;
		public static Zone ZONE_GRAVEYARD;
		public static Zone ZONE_INTERIOR;


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


#region OLD LOADING

/*
Image imageZoneset			= Resources.LoadImage("Images/Tiles/zoneset");
Image imageGeneralTiles		= Resources.LoadImage("Images/Tiles/general_tiles");
Image imageIconsThin		= Resources.LoadImage("Images/sheet_icons_thin");
Image imageSheetPlayer		= Resources.LoadImage("Images/Units/Player/player");
Image imageSheetPlayerRed	= Resources.LoadImage("Images/Units/Player/player_red");
Image imageSheetPlayerBlue	= Resources.LoadImage("Images/Units/Player/player_blue");
Image imageSheetPlayerHurt	= Resources.LoadImage("Images/Units/Player/player_hurt");
Image imageEffects			= Resources.LoadImage("Images/Effects/basic_effects");
Image imageColorEffects		= Resources.LoadImage("Images/Effects/color_effects");
Image imagePlayerItems		= Resources.LoadImage("Images/player_items");

SHEET_ZONESET_LARGE	= new SpriteSheet(imageZoneset,			16, 16, 0, 0, 1, 1);
SHEET_ZONESET_SMALL	= new SpriteSheet(imageZoneset,			8, 8, 187, 0, 1, 1);
SHEET_GENERAL_TILES	= new SpriteSheet(imageGeneralTiles,	16, 16, 0, 0, 1, 1);

SHEET_PLAYER		= new SpriteSheet(imageSheetPlayer,		16, 16, 0, 0, 1, 1);
SHEET_PLAYER_RED	= new SpriteSheet(imageSheetPlayerRed,	16, 16, 0, 0, 1, 1);
SHEET_PLAYER_BLUE	= new SpriteSheet(imageSheetPlayerBlue,	16, 16, 0, 0, 1, 1);
SHEET_PLAYER_HURT	= new SpriteSheet(imageSheetPlayerHurt,	16, 16, 0, 0, 1, 1);
SHEET_EFFECTS		= new SpriteSheet(imageEffects,			16, 16, 0, 0, 1, 1);
SHEET_COLOR_EFFECTS	= new SpriteSheet(imageColorEffects,	16, 16, 0, 0, 1, 1);
SHEET_PLAYER_ITEMS	= new SpriteSheet(imagePlayerItems,		16, 16, 0, 0, 1, 1);


Resources.LoadSpriteSheets("SpriteSheets/menu_elements.conscript");

SHEET_MENU_SMALL = Resources.GetSpriteSheet("UI/menu_small");
SHEET_MENU_LARGE = Resources.GetSpriteSheet("UI/menu_large");
SHEET_MENU_SMALL_LIGHT = Resources.GetSpriteSheet("UI/menu_small_light");
SHEET_MENU_LARGE_LIGHT = Resources.GetSpriteSheet("UI/menu_large_light");

Resources.LoadSpriteSheets("SpriteSheets/items.conscript");
Resources.LoadSpriteSheets("SpriteSheets/sprite_sheets.conscript");

SHEET_ITEMS_SMALL = Resources.GetSpriteSheet("Items/items_small");
SHEET_ITEMS_LARGE = Resources.GetSpriteSheet("Items/items_large");
SHEET_ITEMS_SMALL_LIGHT = Resources.GetSpriteSheet("Items/items_small_light");
SHEET_ITEMS_LARGE_LIGHT = Resources.GetSpriteSheet("Items/items_large_light");
*/

/*
// SPRITES.
spriteBuilder.SpriteSheet = SHEET_ZONESET_LARGE;
BuildSprite(SPR_TILE_DEFAULT,			0, 2);
BuildSprite(SPR_TILE_DUG,				1, 3);
BuildSprite(SPR_TILE_BUSH,				0, 0);
BuildSprite(SPR_TILE_CRYSTAL,			1, 0);
BuildSprite(SPR_TILE_POT,				2, 0);
BuildSprite(SPR_TILE_ROCK,				3, 0);
BuildSprite(SPR_TILE_DIAMOND_ROCK,		4, 0);
BuildSprite(SPR_TILE_SIGN,				5, 0);
BuildSprite(SPR_TILE_BURNABLE_TREE,		6, 2);
BuildSprite(SPR_TILE_DIRT_PILE,			6, 3);
BuildSprite(SPR_TILE_GRASS,				0, 3);
BuildSprite(SPR_TILE_CACTUS,			5, 3);
BuildSprite(SPR_TILE_ARMOS_STATUE,		8, 1);
BuildSprite(SPR_TILE_LANTERN_UNLIT,		0, 8);
BuildSprite(SPR_TILE_LEVER_LEFT,		5, 8);
BuildSprite(SPR_TILE_LEVER_RIGHT,		6, 8);
BuildSprite(SPR_TILE_BUTTON_UP,			7, 8);
BuildSprite(SPR_TILE_BUTTON_DOWN,		8, 8);
BuildSprite(SPR_TILE_CHEST,				9, 8);
BuildSprite(SPR_TILE_CHEST_OPEN,		10, 8);
BuildSprite(SPR_TILE_BOMBABLE_BLOCK,	0, 9);
BuildSprite(SPR_TILE_MOVABLE_BLOCK,		1, 9);
BuildSprite(SPR_TILE_LOCKED_BLOCK,		3, 9);
BuildSprite(SPR_TILE_EYE_STATUE,		4, 9);
BuildSprite(SPR_TILE_BRIDGE_H,			5, 9);
BuildSprite(SPR_TILE_BRIDGE_V,			6, 9);
BuildSprite(SPR_TILE_COLOR_CUBE_SLOT,	7, 9);
BuildSprite(SPR_TILE_CRACKED_FLOOR,		8, 9);
BuildSprite(SPR_TILE_PIT,				9, 9);

spriteBuilder.SpriteSheet = SHEET_GENERAL_TILES;
BuildSprite(SPR_OWL, 0, 0);
BuildSprite(SPR_OWL_ACTIVATED, 1,0, -8,0).AddPart(2,0, 8,0);

spriteBuilder.SpriteSheet = SHEET_EFFECTS;
BuildSprite(SPR_SHADOW, 0, 0, -8, -8);
		
spriteBuilder.SpriteSheet = SHEET_ITEMS_SMALL;
BuildSprite(SPR_ITEM_ICON_SWORD_1,					0, 0);
BuildSprite(SPR_ITEM_ICON_SWORD_2,					1, 0);
BuildSprite(SPR_ITEM_ICON_SWORD_3,					2, 0);
BuildSprite(SPR_ITEM_ICON_SHIELD_1,					3, 0);
BuildSprite(SPR_ITEM_ICON_SHIELD_2,					4, 0);
BuildSprite(SPR_ITEM_ICON_SHIELD_3,					5, 0);
BuildSprite(SPR_ITEM_ICON_SATCHEL,					6, 0);
BuildSprite(SPR_ITEM_ICON_SATCHEL_EQUIPPED,			7, 0);
BuildSprite(SPR_ITEM_ICON_SEED_SHOOTER,				8, 0);
BuildSprite(SPR_ITEM_ICON_SEED_SHOOTER_EQUIPPED,	9, 0);
BuildSprite(SPR_ITEM_ICON_SLINGSHOT_1,				10, 0);
BuildSprite(SPR_ITEM_ICON_SLINGSHOT_2,				11, 0);
BuildSprite(SPR_ITEM_ICON_SLINGSHOT_2_EQUIPPED,		12, 0);
BuildSprite(SPR_ITEM_ICON_BOMB,						13, 0);
BuildSprite(SPR_ITEM_ICON_BOMBCHEW,					14, 0);
BuildSprite(SPR_ITEM_ICON_SHOVEL,					15, 0);
BuildSprite(SPR_ITEM_ICON_BRACELET,					0, 1);
BuildSprite(SPR_ITEM_ICON_POWER_GLOVES,				1, 1);
BuildSprite(SPR_ITEM_ICON_FEATHER,					2, 1);
BuildSprite(SPR_ITEM_ICON_CAPE,						3, 1);
BuildSprite(SPR_ITEM_ICON_BOOMERANG_1,				4, 1);
BuildSprite(SPR_ITEM_ICON_BOOMERANG_2,				5, 1);
BuildSprite(SPR_ITEM_ICON_SWITCH_HOOK_1,			6, 1);
BuildSprite(SPR_ITEM_ICON_SWITCH_HOOK_2,			7, 1);
BuildSprite(SPR_ITEM_ICON_MAGNET_GLOVES_BLUE,		8, 1);
BuildSprite(SPR_ITEM_ICON_MAGNET_GLOVES_RED,		9, 1);
BuildSprite(SPR_ITEM_ICON_CANE,						10, 1);
BuildSprite(SPR_ITEM_ICON_FIRE_ROD,					11, 1);
BuildSprite(SPR_ITEM_ICON_OCARINA,					12, 1);
BuildSprite(SPR_ITEM_ICON_BOW,						13, 1);
BuildSprite(SPR_ITEM_SEED_EMBER,					0, 3).SetSize(8, 8);
BuildSprite(SPR_ITEM_SEED_SCENT,					1, 3).SetSize(8, 8);
BuildSprite(SPR_ITEM_SEED_PEGASUS,					2, 3).SetSize(8, 8);
BuildSprite(SPR_ITEM_SEED_GALE,						3, 3).SetSize(8, 8);
BuildSprite(SPR_ITEM_SEED_MYSTERY,					4, 3).SetSize(8, 8);
SPR_ITEM_SEEDS = new Sprite[] {
	SPR_ITEM_SEED_EMBER,
	SPR_ITEM_SEED_SCENT,
	SPR_ITEM_SEED_PEGASUS,
	SPR_ITEM_SEED_GALE,
	SPR_ITEM_SEED_MYSTERY };

// SPRITE item_seed_gale, (3, 3); SIZE (8, 8); END;
// SPRITE hud_bracket_left, (3, 3); ADD (8, 8) (0, 8); END;
// 

spriteBuilder.SpriteSheet = SHEET_MENU_SMALL;
BuildSprite(SPR_HUD_BACKGROUND,				2, 4);
BuildSprite(SPR_HUD_BACKGROUND_INVENTORY,	3, 4);
BuildSprite(SPR_HUD_BRACKET_LEFT,		0,2).AddPart(0,3, 0,8);
BuildSprite(SPR_HUD_BRACKET_RIGHT,		1,2).AddPart(1,3, 0,8);
BuildSprite(SPR_HUD_BRACKET_LEFT_RIGHT,	2,2).AddPart(2,3, 0,8);
BuildSprite(SPR_HUD_BRACKET_LEFT_A,		3,2).AddPart(0,3, 0,8);
BuildSprite(SPR_HUD_BRACKET_LEFT_B,		3,3).AddPart(0,3, 0,8);
BuildSprite(SPR_HUD_BRACKET_RIGHT_A,	4,2).AddPart(1,3, 0,8);
BuildSprite(SPR_HUD_BRACKET_RIGHT_B,	4,3).AddPart(1,3, 0,8);
BuildSprite(SPR_HUD_HEART_0,	0, 0);
BuildSprite(SPR_HUD_HEART_1,	1, 0);
BuildSprite(SPR_HUD_HEART_2,	2, 0);
BuildSprite(SPR_HUD_HEART_3,	3, 0);
BuildSprite(SPR_HUD_HEART_4,	4, 0);
BuildSprite(SPR_HUD_RUPEE,		0, 1);
BuildSprite(SPR_HUD_ORE_CHUNK,	1, 1);
BuildSprite(SPR_HUD_KEY,		2, 1);
*/


// TILE ANIMATIONS:
/*
animationBuilder.SetSheet(SHEET_ZONESET_LARGE);
BuildAnim(ANIM_TILE_LANTERN)			.AddFrameStrip(16, 1, 8, 4);

animationBuilder.SetSheet(SHEET_ZONESET_SMALL);
BuildAnim(ANIM_TILE_WATER)			.AddFrameStrip(16, 0, 4, 4).MakeQuad();
BuildAnim(ANIM_TILE_WATER_DEEP)		.AddFrameStrip(16, 0, 5, 4).MakeQuad();
BuildAnim(ANIM_TILE_WATERFALL)		.AddFrameStrip(8, 0, 6, 4).MakeQuad();
BuildAnim(ANIM_TILE_LAVAFALL)		.AddFrameStrip(8, 0, 8, 4).MakeQuad();
BuildAnim(ANIM_TILE_PUDDLE)			.AddFrameStrip(16, 4,10, 3).AddFrame(16, 5,10).MakeQuad();
BuildAnim(ANIM_TILE_WATERFALL_BOTTOM).InsertFrameStrip(0, 8, 0,6, 4, 0,0)
								.InsertFrameStrip(0, 8, 0,6, 4, 8,0)
								.InsertFrameStrip(0, 8, 0,7, 4, 0,8)
								.InsertFrameStrip(0, 8, 0,7, 4, 8,8);
BuildAnim(ANIM_TILE_LAVAFALL_BOTTOM)	.InsertFrameStrip(0, 8, 0,8, 4, 0,0)
								.InsertFrameStrip(0, 8, 0,8, 4, 8,0)
								.InsertFrameStrip(0, 8, 0,9, 4, 0,8)
								.InsertFrameStrip(0, 8, 0,9, 4, 8,8);
BuildAnim(ANIM_TILE_FLOWERS)			.InsertFrameStrip(0, 16,  4,9, 4, 0,0)
								.InsertFrameStrip(0, 16,  4,9, 4, 8,8)
								.InsertFrame(0, 64, 7,10, 8,0)
								.InsertFrame(0, 64, 7,10, 0,8);
BuildAnim(ANIM_TILE_OCEAN)			.InsertFrameStrip(0, 16, 4,4, 4, 0,0, 0,1)
								.InsertFrameStrip(0, 16, 5,4, 4, 8,0, 0,1)
								.InsertFrameStrip(0, 16, 4,4, 4, 0,8, 0,1)
								.InsertFrameStrip(0, 16, 5,4, 4, 8,8, 0,1);
BuildAnim(ANIM_TILE_OCEAN_SHORE)		.InsertFrameStrip(0, 16, 6,4, 4, 0,0, 0,1)
								.InsertFrameStrip(0, 16, 7,4, 4, 8,0, 0,1)
								.InsertFrameStrip(0, 16, 4,4, 4, 0,8, 0,1)
								.InsertFrameStrip(0, 16, 5,4, 4, 8,8, 0,1);
			
// PLAYER ANIMATIONS:
			
animationBuilder.SetSheet(SHEET_PLAYER);
BuildAnim(ANIM_PLAYER_DEFAULT)			.AddFrameStrip(6, 0,0, 2).MakeDynamic(4, 2,0);
BuildAnim(ANIM_PLAYER_SHIELD)			.AddFrameStrip(6, 0,1, 2).MakeDynamic(4, 2,0);
BuildAnim(ANIM_PLAYER_SHIELD_BLOCK)		.AddFrameStrip(6, 0,2, 2).MakeDynamic(4, 2,0);
BuildAnim(ANIM_PLAYER_CARRY)			.AddFrameStrip(6, 0,5, 2).MakeDynamic(4, 2,0);
BuildAnim(ANIM_PLAYER_PUSH)				.AddFrameStrip(6, 0,6, 2).MakeDynamic(4, 2,0);
BuildAnim(ANIM_PLAYER_SWIM)				.AddFrameStrip(6, 0,13, 2).Offset(0, 2).MakeDynamic(4, 2,0);
BuildAnim(ANIM_PLAYER_SUBMERGED)		.AddFrame(16, 0,21, 0,4).AddFrame(16, 1,21, 0,4);
BuildAnim(ANIM_PLAYER_GRAB)				.AddFrame(1, 0,7, 0,0).MakeDynamic(4, 2,0);
BuildAnim(ANIM_PLAYER_DIG)				.AddFrame(8, 0,9).AddFrame(16, 1,9).SetLoopMode(LoopMode.Clamp).MakeDynamic(4, 2,0);
BuildAnim(ANIM_PLAYER_THROW)			.AddFrame(9, 0,4).SetLoopMode(LoopMode.Clamp).MakeDynamic(4, 2,0);
BuildAnim(ANIM_PLAYER_FALL).AddFrame(16, 1, 20, 0, 0).AddFrame(10, 2, 20, 0, 0).AddFrame(11, 3, 20, 0, 0).SetLoopMode(LoopMode.Clamp);
BuildAnim(ANIM_PLAYER_SHIELD_LARGE)			.AddFrameStrip(6, 0,3, 2).CreateSubStrip()
											.AddFrameStrip(6, 2,1, 2).MakeDynamic(3, 2,0);
BuildAnim(ANIM_PLAYER_SHIELD_LARGE_BLOCK)	.AddFrameStrip(6, 0,2, 2).MakeDynamic(3, 2,0);
BuildAnim(ANIM_PLAYER_PULL)
	.AddFrame(1, 1,7, -4,0).CreateSubStrip()
	.AddFrame(1, 3,7, 0,2).CreateSubStrip()
	.AddFrame(1, 5,7, 4,0).CreateSubStrip()
	.AddFrame(1, 7,7, 0,-1);
BuildAnim(ANIM_PLAYER_JUMP)
	.AddFrame(9, 0, 11).AddFrame(9, 1, 11).AddFrame(6, 2, 11).AddFrame(6, 1, 0).SetLoopMode(LoopMode.Clamp).CreateSubStrip()
	.AddFrame(9, 3, 11).AddFrame(9, 4, 11).AddFrame(6, 5, 11).AddFrame(6, 3, 0).SetLoopMode(LoopMode.Clamp).CreateSubStrip()
	.AddFrame(9, 0, 12).AddFrame(9, 1, 12).AddFrame(6, 2, 12).AddFrame(6, 5, 0).SetLoopMode(LoopMode.Clamp).CreateSubStrip()
	.AddFrame(9, 3, 12).AddFrame(9, 4, 12).AddFrame(6, 5, 12).AddFrame(6, 7, 0).SetLoopMode(LoopMode.Clamp);
BuildAnim(ANIM_PLAYER_SWING)
	.AddFrame(3, 4, 8, 0, 0).AddFrame(3, 0, 4, 0, 0).AddFrame(8, 0, 4, 4, 0).AddFrame(3, 0, 4, 0, 0).SetLoopMode(LoopMode.Clamp).CreateSubStrip()
	.AddFrame(3, 3, 8, 0, 0).AddFrame(3, 2, 4, 0, 0).AddFrame(8, 2, 4, 0, -4).AddFrame(3, 2, 4, 0, 0).SetLoopMode(LoopMode.Clamp).CreateSubStrip()
	.AddFrame(3, 2, 8, 0, 0).AddFrame(3, 4, 4, 0, 0).AddFrame(8, 4, 4, -4, 0).AddFrame(3, 4, 4, 0, 0).SetLoopMode(LoopMode.Clamp).CreateSubStrip()
	.AddFrame(3, 1, 8, 0, 0).AddFrame(3, 6, 4, 0, 0).AddFrame(8, 6, 4, 0, 4).AddFrame(3, 6, 4, 0, 0).SetLoopMode(LoopMode.Clamp);
BuildAnim(ANIM_PLAYER_SPIN)
	.AddFrame(5, 0, 4, 3, 0).AddFrame(5, 6, 4, 0, 3).AddFrame(5, 4, 4, -3, 0).AddFrame(5, 2, 4, 0, -3).AddFrame(3, 0, 4, 3, 0).SetLoopMode(LoopMode.Clamp).CreateSubStrip()
	.AddFrame(5, 2, 4, 0, -3).AddFrame(3, 0, 4, 3, 0).AddFrame(5, 6, 4, 0, 3).AddFrame(5, 4, 4, -3, 0).AddFrame(5, 2, 4, 0, -3).SetLoopMode(LoopMode.Clamp).CreateSubStrip()
	.AddFrame(5, 4, 4, -3, 0).AddFrame(5, 2, 4, 0, -3).AddFrame(3, 0, 4, 3, 0).AddFrame(5, 6, 4, 0, 3).AddFrame(5, 4, 4, -3, 0).SetLoopMode(LoopMode.Clamp).CreateSubStrip()
	.AddFrame(5, 6, 4, 0, 3).AddFrame(5, 4, 4, -3, 0).AddFrame(5, 2, 4, 0, -3).AddFrame(3, 0, 4, 3, 0).AddFrame(5, 6, 4, 0, 3).SetLoopMode(LoopMode.Clamp);
			
BuildAnim(ANIM_PLAYER_STAB)
	.AddFrame(6, 0,4, 4,0).AddFrame(7, 0, 4, 0, 0).AddFrame(1, 1, 0, 0, 0).SetLoopMode(LoopMode.Reset).CreateSubStrip()
	.AddFrame(6, 2,4, 0,-4).AddFrame(7, 2, 4, 0, 0).AddFrame(1, 3, 0, 0, 0).SetLoopMode(LoopMode.Reset).CreateSubStrip()
	.AddFrame(6, 4,4, -4,0).AddFrame(7, 4, 4, 0, 0).AddFrame(1, 5, 0, 0, 0).SetLoopMode(LoopMode.Reset).CreateSubStrip()
	.AddFrame(6, 6,4, 0,4).AddFrame(7, 6, 4, 0, 0).AddFrame(1, 7, 0, 0, 0).SetLoopMode(LoopMode.Reset);

// WEAPON ANIMATIONS:
			
animationBuilder.SpriteSheet = SHEET_PLAYER_ITEMS;

BuildAnim(ANIM_SWORD_HOLD)
	.AddFrame(4, 0, 0, 12, 4).Offset(-8, -16).CreateSubStrip()
	.AddFrame(4, 2, 0, -4, -12).Offset(-8, -16).CreateSubStrip()
	.AddFrame(4, 4, 0, -12, 4).Offset(-8, -16).CreateSubStrip()
	.AddFrame(4, 6, 0, 3, 14).Offset(-8, -16);
BuildAnim(ANIM_SWORD_CHARGED)
	.AddFrame(4, 0, 1, 12, 4).AddFrame(4, 0, 0, 12, 4).Offset(-8, -16).SetLoopMode(LoopMode.Repeat).CreateSubStrip()
	.AddFrame(4, 1, 1, -4, -12).AddFrame(4, 2, 0, -4, -12).Offset(-8, -16).SetLoopMode(LoopMode.Repeat).CreateSubStrip()
	.AddFrame(4, 2, 1, -12, 4).AddFrame(4, 4, 0, -12, 4).Offset(-8, -16).SetLoopMode(LoopMode.Repeat).CreateSubStrip()
	.AddFrame(4, 3, 1, 3, 14).AddFrame(4, 6, 0, 3, 14).Offset(-8, -16).SetLoopMode(LoopMode.Repeat);
BuildAnim(ANIM_SWORD_SWING)
	.AddFrame(3, 2, 0, 0, -16).AddFrame(3, 1, 0, 13, -13).AddFrame(8, 0, 0, 20, 4).AddFrame(3, 0, 0, 12, 4).Offset(-8, -16).SetLoopMode(LoopMode.Clamp).CreateSubStrip()
	.AddFrame(3, 0, 0, 16, 0).AddFrame(3, 1, 0, 13, -13).AddFrame(8, 2, 0, -4, -20).AddFrame(3, 2, 0, -4, -12).Offset(-8, -16).SetLoopMode(LoopMode.Clamp).CreateSubStrip()
	.AddFrame(3, 2, 0, 0, -16).AddFrame(3, 3, 0, -13, -13).AddFrame(8, 4, 0, -20, 4).AddFrame(3, 4, 0, -12, 4).Offset(-8, -16).SetLoopMode(LoopMode.Clamp).CreateSubStrip()
	.AddFrame(3, 4, 0, -15, 2).AddFrame(3, 5, 0, -13, 15).AddFrame(8, 6, 0, 3, 20).AddFrame(3, 6, 0, 3, 14).Offset(-8, -16).SetLoopMode(LoopMode.Clamp);
BuildAnim(ANIM_SWORD_SPIN)
	.AddFrame(3, 0, 0, 19, 4).AddFrame(2, 7, 0, 16, 16).AddFrame(3, 6, 0, 3, 19).AddFrame(2, 5, 0, -13, 15).AddFrame(3, 4, 0, -19, 4).AddFrame(2, 3, 0, -13, -13)
	.AddFrame(3, 2, 0, -4, -19).AddFrame(2, 1, 0, 16, -16).AddFrame(3, 0, 0, 19, 4).Offset(-8, -16).SetLoopMode(LoopMode.Clamp).CreateSubStrip()

	.AddFrame(3, 2, 0, -4, -19).AddFrame(2, 1, 0, 16, -16).AddFrame(3, 0, 0, 19, 4).AddFrame(2, 7, 0, 16, 16).AddFrame(3, 6, 0, 3, 19).AddFrame(2, 5, 0, -13, 15)
	.AddFrame(3, 4, 0, -19, 4).AddFrame(2, 3, 0, -13, -13).AddFrame(3, 2, 0, -4, -19).Offset(-8, -16).SetLoopMode(LoopMode.Clamp).CreateSubStrip()

	.AddFrame(3, 4, 0, -19, 4).AddFrame(2, 3, 0, -13, -13).AddFrame(3, 2, 0, -4, -19).AddFrame(2, 1, 0, 16, -16).AddFrame(3, 0, 0, 19, 4).AddFrame(2, 7, 0, 16, 16)
	.AddFrame(3, 6, 0, 3, 19).AddFrame(2, 5, 0, -13, 15).AddFrame(3, 4, 0, -19, 4).Offset(-8, -16).SetLoopMode(LoopMode.Clamp).CreateSubStrip()

	.AddFrame(3, 6, 0, 3, 19).AddFrame(2, 5, 0, -13, 15).AddFrame(3, 4, 0, -19, 4).AddFrame(2, 3, 0, -13, -13).AddFrame(3, 2, 0, -4, -19).AddFrame(2, 1, 0, 16, -16)
	.AddFrame(3, 0, 0, 19, 4).AddFrame(2, 7, 0, 16, 16).AddFrame(3, 6, 0, 3, 19).Offset(-8, -16).SetLoopMode(LoopMode.Clamp);
BuildAnim(ANIM_SWORD_STAB)
	.AddFrame(6, 0,0, 20,4).AddFrame(8, 0,0, 12,4).Offset(-8, -16).SetLoopMode(LoopMode.Clamp).CreateSubStrip()
	.AddFrame(6, 2,0, -4,-20).AddFrame(8, 2,0, -4,-12).Offset(-8, -16).SetLoopMode(LoopMode.Clamp).CreateSubStrip()
	.AddFrame(6, 4,0, -20,4).AddFrame(8, 4,0, -14,4).Offset(-8, -16).SetLoopMode(LoopMode.Clamp).CreateSubStrip()
	.AddFrame(6, 6,0, 3,20).AddFrame(8, 6,0, 3,14).Offset(-8, -16).SetLoopMode(LoopMode.Clamp);
			
// PROJECTILE & ITEM ANIMATIONS:
			
animationBuilder.SpriteSheet = SHEET_PLAYER_ITEMS;
BuildAnim(ANIM_PROJECTILE_PLAYER_ARROW).AddFrame(1, 0,11).Offset(-8, -8).MakeDynamic(8, 1,0);
BuildAnim(ANIM_PROJECTILE_PLAYER_ARROW_CRASH).AddFrameStrip(6, 0,11, 4, -8,-8, 2,0);
BuildAnim(ANIM_ITEM_BOMB).AddFrame(4, 2,8, 0,0).AddFrame(4, 3,8, 0,0);
			
// EFFECT ANIMATIONS:
animationBuilder.SpriteSheet = SHEET_EFFECTS;
			
BuildAnim(ANIM_EFFECT_DIRT)
	.AddFrame(1, 5,0, -14,-12).AddPart(1, 5,0, -10,-6).SetLoopMode(LoopMode.Repeat).CreateSubStrip()
	.AddFrame(1, 4,0, -8,-10).AddPart(1, 5,0, -8,-8).CreateSubStrip()
	.AddFrame(1, 4,0, -2,-12).AddPart(1, 4,0, -6,-6).CreateSubStrip()
	.AddFrame(1, 4,0, -8,-8).AddPart(1, 5,0, -8,-10);
BuildAnim(ANIM_EFFECT_WATER_SPLASH).SetLoopMode(LoopMode.Clamp)
	.AddFrame(4, 2,2, -8, -11).AddPart(4, 3,2, -8, -11)
	.AddFrame(4, 2,2, -10, -13).AddPart(4, 3,2, -6, -13)
	.AddFrame(4, 2,2, -12, -15).AddPart(4, 3,2, -4, -15);
BuildAnim(ANIM_EFFECT_RIPPLES)
	.AddFrame(8, 4,2, -5,-5).AddPart(8, 5,2, -11,-5)
	.AddFrame(8, 4,2, -6,-5).AddPart(8, 5,2, -10,-5)
	.AddFrame(8, 4,2, -7,-4).AddPart(8, 5,2, -9,-4)
	.AddFrame(8, 4,2, -8,-3).AddPart(8, 5,2, -8,-3).Offset(0, -6);
BuildAnim(ANIM_EFFECT_GRASS)
	.AddFrame(6, 6,0, -4,1).AddPart(6, 6,0, 2,1)
	.AddFrame(6, 7,0, -4,1).AddPart(6, 7,0, 2,1).Offset(-8, -14);
BuildAnim(ANIM_EFFECT_ROCK_BREAK).SetLoopMode(LoopMode.Reset)
	.AddFrame(4, 2,0, -4,5).AddPart(4, 2,0, 5,-6).AddPart(4, 2,0, -6,4).AddPart(4, 2,0, 4,3)
	.AddFrame(4, 2,0, -6,-6).AddPart(4, 2,0, 7,-7).AddPart(4, 2,0, -7,5).AddPart(4, 2,0, 6,4)
	.AddFrame(4, 2,0, -7,-7).AddPart(4, 2,0, 9,-8).AddPart(4, 2,0, -9,6).AddPart(4, 2,0, 8,5)
	.AddFrame(4, 2,0, -9,-5).AddPart(4, 2,0, 11,-6).AddPart(4, 2,0, -11,8).AddPart(4, 2,0, 10,7).Offset(-8, -8);
BuildAnim(ANIM_EFFECT_SIGN_BREAK).SetLoopMode(LoopMode.Reset)
	.AddFrame(4, 3,0, -4,5).AddPart(4, 3,0, 5,-6).AddPart(4, 3,0, -6,4).AddPart(4, 3,0, 4,3)
	.AddFrame(4, 3,0, -6,-6).AddPart(4, 3,0, 7,-7).AddPart(4, 3,0, -7,5).AddPart(4, 3,0, 6,4)
	.AddFrame(4, 3,0, -7,-7).AddPart(4, 3,0, 9,-8).AddPart(4, 3,0, -9,6).AddPart(4, 3,0, 8,5)
	.AddFrame(4, 3,0, -9,-5).AddPart(4, 3,0, 11,-6).AddPart(4, 3,0, -11,8).AddPart(4, 3,0, 10,7).Offset(-8, -8);
BuildAnim(ANIM_EFFECT_LEAVES).SetLoopMode(LoopMode.Reset)
	.AddFrame(4, 0,1, 2,-1)	.AddPart(4, 1,1, -8,-4)	.AddPart(4, 0,1, 0,-5)		.AddPart(4, 0,1, 6,-5)
	.AddFrame(4, 2,1, 2,3)	.AddPart(4, 3,1, 3,-3)	.AddPart(4, 1,1, 0,-4)		.AddPart(4, 0,1, -5,-4)
	.AddFrame(4, 3,1, 5,5)	.AddPart(4, 3,1, 2,-1)	.AddPart(4, 1,1, 0,-5)		.AddPart(4, 3,1, -2,-5)
	.AddFrame(4, 3,1, 5,5)	.AddPart(4, 3,1, -1,5)	.AddPart(4, 3,1, 3,-3)		.AddPart(4, 1,1, -1,-9)
	.AddFrame(4, 3,1, 4,7)	.AddPart(4, 3,1, 3,1)	.AddPart(4, 3,1, -5,-10)	.AddPart(4, 1,1, -7,5)
	.AddFrame(4, 2,1, 5,2)	.AddPart(4, 3,1, 6,9)	.AddPart(4, 2,1, -5,-10)	.AddPart(4, 0,1, -10,9)
	.AddFrame(4, 2,1, 8,11)	.AddPart(4, 2,1, 9,2)	.AddPart(4, 2,1, -5,-11)	.AddPart(4, 0,1, -10,5)
	.AddFrame(4, 2,1, 8,9)	.AddPart(4, 2,1, 9,3)	.AddPart(4, 2,1, -7,-12)	.AddPart(4, 0,1, -13,-1)
	.Offset(-8, -8);
BuildAnim(ANIM_ITEM_SCENT_POD).AddFrame(8, 0,2).AddFrame(8, 1,2);
			
// TODO: Flicker function in animation builder.
BuildAnim(ANIM_EFFECT_GRASS_LEAVES).SetLoopMode(LoopMode.Reset).AddDelay(1)
	.AddFrame(1, 0,1, 2,-1)	.AddPart(1, 1,1, -8,-4)	.AddPart(1, 0,1, 0,-5)		.AddPart(1, 0,1, 6,-5)	.AddDelay(1)
		.AddFrame(1, 0,1, 2,-1)	.AddPart(1, 1,1, -8,-4)	.AddPart(1, 0,1, 0,-5)		.AddPart(1, 0,1, 6,-5)	.AddDelay(1)
	.AddFrame(1, 2,1, 2,3)	.AddPart(1, 3,1, 3,-3)	.AddPart(1, 1,1, 0,-4)		.AddPart(1, 0,1, -5,-4)	.AddDelay(1)
		.AddFrame(1, 2,1, 2,3)	.AddPart(1, 3,1, 3,-3)	.AddPart(1, 1,1, 0,-4)		.AddPart(1, 0,1, -5,-4)	.AddDelay(1)
	.AddFrame(1, 3,1, 5,5)	.AddPart(1, 3,1, 2,-1)	.AddPart(1, 1,1, 0,-5)		.AddPart(1, 3,1, -2,-5)	.AddDelay(1)
		.AddFrame(1, 3,1, 5,5)	.AddPart(1, 3,1, 2,-1)	.AddPart(1, 1,1, 0,-5)		.AddPart(1, 3,1, -2,-5)	.AddDelay(1)
	.AddFrame(1, 3,1, 5,5)	.AddPart(1, 3,1, -1,5)	.AddPart(1, 3,1, 3,-3)		.AddPart(1, 1,1, -1,-9)	.AddDelay(1)
		.AddFrame(1, 3,1, 5,5)	.AddPart(1, 3,1, -1,5)	.AddPart(1, 3,1, 3,-3)		.AddPart(1, 1,1, -1,-9)	.AddDelay(1)
	.AddFrame(1, 3,1, 4,7)	.AddPart(1, 3,1, 3,1)	.AddPart(1, 3,1, -5,-10)	.AddPart(1, 1,1, -7,5)	.AddDelay(1)
		.AddFrame(1, 3,1, 4,7)	.AddPart(1, 3,1, 3,1)	.AddPart(1, 3,1, -5,-10)	.AddPart(1, 1,1, -7,5)	.AddDelay(1)
	.AddFrame(1, 2,1, 5,2)	.AddPart(1, 3,1, 6,9)	.AddPart(1, 2,1, -5,-10)	.AddPart(1, 0,1, -10,9)	.AddDelay(1)
		.AddFrame(1, 2,1, 5,2)	.AddPart(1, 3,1, 6,9)	.AddPart(1, 2,1, -5,-10)	.AddPart(1, 0,1, -10,9)	.AddDelay(1)
	.AddFrame(1, 2,1, 8,11)	.AddPart(1, 2,1, 9,2)	.AddPart(1, 2,1, -5,-11)	.AddPart(1, 0,1, -10,5)	.AddDelay(1)
		.AddFrame(1, 2,1, 8,11)	.AddPart(1, 2,1, 9,2)	.AddPart(1, 2,1, -5,-11)	.AddPart(1, 0,1, -10,5)	.AddDelay(1)
	.AddFrame(1, 2,1, 8,9)	.AddPart(1, 2,1, 9,3)	.AddPart(1, 2,1, -7,-12)	.AddPart(1, 0,1, -13,-1).AddDelay(1)
		.AddFrame(1, 2,1, 8,9)	.AddPart(1, 2,1, 9,3)	.AddPart(1, 2,1, -7,-12)	.AddPart(1, 0,1, -13,-1)
	.Offset(-8, -8);
			
// COLOR EFFECT ANIMATIONS:
animationBuilder.SpriteSheet = SHEET_COLOR_EFFECTS;
			
BuildAnim(ANIM_EFFECT_BOMB_EXPLOSION).SetLoopMode(LoopMode.Reset)
	.AddFrame(4, 0,0)
	.AddFrame(4, 0,16)
	.AddFrame(3, 0,0)
	.AddFrame(7, 0,0, -6,-6).AddPart(7, 0,0, 6,-6).AddPart(7, 0,0, -6,2).AddPart(7, 0,0, 6,2)
	.AddFrame(8, 6,1, -8,-8).AddPart(8, 7,1, 8,-8).AddPart(8, 6,2, -8,8).AddPart(8, 7,2, 8,8)
	.AddFrame(9, 1,0, -8,-8).AddPart(9, 1,0, 8,-8).AddPart(9, 1,0, -8,8).AddPart(9, 1,0, 8,8);
BuildAnim(ANIM_EFFECT_SEED_SCENT)
	.AddFrame(3, 8,12).AddFrame(3, 6,12).AddFrame(3, 7,12).Offset(-8, -8);
BuildAnim(ANIM_EFFECT_SEED_PEGASUS).SetLoopMode(LoopMode.Reset)
	.AddFrame(3, 3,4).AddFrame(3, 0,4).AddFrame(3, 1,4).Offset(-8, -8);
BuildAnim(ANIM_EFFECT_SEED_MYSTERY).SetLoopMode(LoopMode.Reset)
	.AddFrame(3, 3,8).AddFrame(3, 0,8).AddFrame(3, 1,8).Offset(-8, -8); // PegasusSeedEffect.ShiftSourcePositions(0, 4);
BuildAnim(ANIM_EFFECT_SEED_EMBER).SetLoopMode(LoopMode.Reset)
	.AddFrame(2, 1,3)
	.AddFrame(2, 1,2).AddFrame(2, 1,17).AddFrame(2, 1,1).RepeatPreviousFrames(3, 9)
	.AddFrame(2, 1,2)
	.Offset(-8, -8);
			
BuildAnim(ANIM_EFFECT_SEED_GALE);
for (int i = 0; i < 12; i++) {
	int y = 1 + (((5 - (i % 4)) % 4) * 4);
	animationBuilder.AddFrame(1, ((i % 6) < 3 ? 4 : 5), y, -8, -8);
}
			
BuildAnim(ANIM_EFFECT_PEGASUS_DUST).SetLoopMode(LoopMode.Reset)
				.AddFrame(1, 5, 12, -12, -10).AddPart(1, 5, 12,  -4, -10)
							
				.AddFrame(1, 8,  9, -14,  -7).AddPart(1, 8, 10, -10, -11).AddPart(1, 8,  9, -1, -7).AddPart(1, 8, 10, -5, -11)
				.AddFrame(1, 8,  6, -14,  -7).AddPart(1, 8,  5, -10, -11).AddPart(1, 8,  6, -1, -7).AddPart(1, 8,  5, -5, -11)
				.AddFrame(1, 8,  1, -13,  -7).AddPart(1, 8,  2, -10, -11).AddPart(1, 8,  1, -1, -7).AddPart(1, 8,  2, -6, -11)

				.AddFrame(1, 8, 14, -17,  -5).AddPart(1, 8, 13, -12,  -9).AddPart(1, 8, 14,  2, -5).AddPart(1, 8, 13, -3,  -9)
				.AddFrame(1, 8,  9, -17,  -5).AddPart(1, 8, 10, -12,  -9).AddPart(1, 8,  9, -4, -5).AddPart(1, 8, 10, -3,  -9);
			
BuildAnim(ANIM_EFFECT_OWL_SPARKLE).SetLoopMode(LoopMode.Reset)
	.AddFrame(9, 9,8).AddFrame(18, 9,9).AddFrame(9, 9,10).AddFrame(9, 9,11).Offset(-8, -8);
BuildAnim(ANIM_EFFECT_FALLING_OBJECT).SetLoopMode(LoopMode.Reset)
	.AddFrame(8, 3,12).AddFrame(12, 4,12).AddFrame(13, 5,12).Offset(-8, -8);
*/
#endregion
