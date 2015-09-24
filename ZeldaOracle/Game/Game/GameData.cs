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
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Scripts;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Main.ResourceBuilders;

namespace ZeldaOracle.Game {

	// A static class for storing links to all game content.
	public class GameData {

		private static AnimationBuilder animationBuilder;


		//-----------------------------------------------------------------------------
		// Initialization
		//-----------------------------------------------------------------------------

		// Initializes and loads the game content.
		public static void Initialize() {

			Console.WriteLine("Loading Images");
			LoadImages();

			Console.WriteLine("Loading Sprite Sheets");
			LoadSpriteSheets();

			Console.WriteLine("Loading Sprites");
			LoadSprites();
			
			Console.WriteLine("Loading Animations");
			LoadAnimations();

			Console.WriteLine("Loading Collision Models");
			LoadCollisionModels();
			
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
		// Image Loading
		//-----------------------------------------------------------------------------
		
		private static int PointIndex(int x, int y) {
			return ((234 * y) + x);
		}
	
		// Loads the images.
		private static void LoadImages() {

			Resources.LoadImage("Images/UI/menu_weapons_a");
			Resources.LoadImage("Images/UI/menu_weapons_b");
			Resources.LoadImage("Images/UI/menu_key_items_a");
			Resources.LoadImage("Images/UI/menu_key_items_b");
			Resources.LoadImage("Images/UI/menu_essences_a");
			Resources.LoadImage("Images/UI/menu_essences_b");
		}

		//-----------------------------------------------------------------------------
		// Sprite Sheet Loading
		//-----------------------------------------------------------------------------

		// Loads the sprite sheets.
		private static void LoadSpriteSheets() {

			Image imageZoneset			= Resources.LoadImage("Images/zoneset");
			Image imageSheetPlayer		= Resources.LoadImage("Images/sheet_player");
			Image imageSheetPlayerRed	= Resources.LoadImage("Images/sheet_player_red");
			Image imageSheetPlayerBlue	= Resources.LoadImage("Images/sheet_player_blue");
			Image imageSheetPlayerHurt	= Resources.LoadImage("Images/sheet_player_hurt");
			Image imageIconsThin		= Resources.LoadImage("Images/sheet_icons_thin");
			Image imageEffects			= Resources.LoadImage("Images/Effects/basic_effects");
			Image imageColorEffects		= Resources.LoadImage("Images/Effects/color_effects");
			Image imagePlayerItems		= Resources.LoadImage("Images/sheet_player_items");
			Image imageGeneralTiles		= Resources.LoadImage("Images/general_tiles");

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
		}

		
		//-----------------------------------------------------------------------------
		// Sprite Loading
		//-----------------------------------------------------------------------------

		private static SpriteBuilder spriteBuilder;

		private static SpriteBuilder BuildSprite(Sprite sprite, int sheetX, int sheetY) {
			return BuildSprite(sprite, sheetX, sheetY, 0, 0);
		}

		private static SpriteBuilder BuildSprite(Sprite sprite, int sheetX, int sheetY, int offsetX, int offsetY) {
			sprite.Set(new Sprite(spriteBuilder.SpriteSheet, sheetX, sheetY, offsetX, offsetY));
			spriteBuilder.Begin(sprite);
			return spriteBuilder;
		}

		private static void LoadSprites() {
			spriteBuilder = new SpriteBuilder();

			// SPRITES.
			spriteBuilder.SpriteSheet = SHEET_ZONESET_LARGE;
			BuildSprite(SPR_TILE_DEFAULT,	0, 2);
			BuildSprite(SPR_TILE_DUG,		1, 3);
			BuildSprite(SPR_BUSH,			0, 0);
			BuildSprite(SPR_CRYSTAL,		1, 0);
			BuildSprite(SPR_POT,			2, 0);
			BuildSprite(SPR_ROCK,			3, 0);
			BuildSprite(SPR_DIAMOND_ROCK,	4, 0);
			BuildSprite(SPR_SIGN,			5, 0);
			BuildSprite(SPR_BURNABLE_TREE,	6, 2);
			BuildSprite(SPR_DIRT_PILE,		6, 3);
			BuildSprite(SPR_GRASS,			0, 3);
			BuildSprite(SPR_CACTUS,			5, 3);
			BuildSprite(SPR_ARMOS_STATUE,	8, 1);
			BuildSprite(SPR_LANTERN_UNLIT,	0, 8);
			BuildSprite(SPR_LEVER_LEFT,		5, 8);
			BuildSprite(SPR_LEVER_RIGHT,	6, 8);
			BuildSprite(SPR_BUTTON_UP,		7, 8);
			BuildSprite(SPR_BUTTON_DOWN,	8, 8);
			BuildSprite(SPR_CHEST,			9, 8);
			BuildSprite(SPR_CHEST_OPEN,		10, 8);
			BuildSprite(SPR_BOMBABLE_BLOCK,	0, 9);
			BuildSprite(SPR_MOVABLE_BLOCK,	1, 9);
			BuildSprite(SPR_LOCKED_BLOCK,	3, 9);
			BuildSprite(SPR_EYE_STATUE,		4, 9);
			BuildSprite(SPR_BRIDGE_H,		5, 9);
			BuildSprite(SPR_BRIDGE_V,		6, 9);
			BuildSprite(SPR_COLOR_CUBE_SLOT,7, 9);
			BuildSprite(SPR_CRACKED_FLOOR,	8, 9);
			BuildSprite(SPR_PIT,			9, 9);

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
			SPR_ITEM_SEEDS = new Sprite[] { SPR_ITEM_SEED_EMBER, SPR_ITEM_SEED_SCENT, SPR_ITEM_SEED_PEGASUS, SPR_ITEM_SEED_GALE, SPR_ITEM_SEED_MYSTERY };

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
		}
		
		
		//-----------------------------------------------------------------------------
		// Animations Loading
		//-----------------------------------------------------------------------------
	
		private static AnimationBuilder BuildAnim(Animation animation) {
			animationBuilder.Begin(animation);
			return animationBuilder;
		}

		private static void LoadAnimations() {
			string assetName = "Animations/animations.conscript";
			
			Console.WriteLine("READING SCRIPT " + assetName);
			AnimationSR sr = new AnimationSR();

			try {
				Stream stream = TitleContainer.OpenStream("Content/" + assetName);
				StreamReader reader = new StreamReader(stream, Encoding.Default);
				sr.ReadScript(reader);
				stream.Close();
			}
			catch (FileNotFoundException) {
				Console.WriteLine("Error loading file \"" + assetName + "\"");
			}


			animationBuilder = new AnimationBuilder();
		
			
			// TILE ANIMATIONS:
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
			/*
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
			*/

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
			BuildAnim(ANIM_PROJECTILE_PLAYER_ARROW_CRASH).AddFrameStrip(6, 0, 11,4, -8,-8, 2,0);
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
			for (int i = 0; i < 29; i++) {
				int y = 1 + (((5 - (i % 4)) % 4) * 4);
				animationBuilder.AddFrame(1, ((i % 6) < 3 ? 4 : 5), y);
			}
			animationBuilder.Offset(-8, -8);

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

			
			
			

			
			Console.WriteLine("===================================");
			Console.WriteLine("CREATING LIST OF MEMBERS:");
			Console.WriteLine("===================================");

			FieldInfo[] fields = typeof(GameData).GetFields();
			for (int i = 0; i < fields.Length; i++) {
				FieldInfo field = fields[i];
				string name = field.Name.ToLower();

				// Sprites.
				//if (field.FieldType == typeof(Sprite)) {
				//	name = name.Remove(0, "SPR_".Length);
				//	if (Resources.SpriteExists(name)) {
				//		field.SetValue(null, Resources.GetSprite(name));
				//		Console.WriteLine(" - " + name);
				//	}
				//}

				// Animations.
				if (field.FieldType == typeof(Animation)) {
					name = name.Remove(0, "ANIM_".Length);
					if (Resources.AnimationExists(name)) {
						field.SetValue(null, Resources.GetAnimation(name));
						Console.WriteLine(" - " + name);
					}
				}
			}
		}


		//-----------------------------------------------------------------------------
		// Collision Models Loading
		//-----------------------------------------------------------------------------

		private static void LoadCollisionModels() {
			MODEL_BLOCK				= new CollisionModel().AddBox( 0,  0, 16, 16);
			MODEL_EDGE_E			= new CollisionModel().AddBox( 8,  0,  8, 16);
			MODEL_EDGE_N			= new CollisionModel().AddBox( 0,  0, 16,  7);
			MODEL_EDGE_W			= new CollisionModel().AddBox( 0,  0,  8, 16);
			MODEL_EDGE_S			= new CollisionModel().AddBox( 0,  8, 16,  8);
			MODEL_DOORWAY			= new CollisionModel().AddBox( 0,  0, 16,  6);
			MODEL_CORNER_NE			= new CollisionModel().AddBox( 8,  0,  8, 16).AddBox(0, 0, 8, 7);
			MODEL_CORNER_NW			= new CollisionModel().AddBox( 0,  0,  8,  8).AddBox(8, 0, 8, 7);
			MODEL_CORNER_SW			= new CollisionModel().AddBox( 0,  8, 16,  8).AddBox(0, 0, 8, 8);
			MODEL_CORNER_SE			= new CollisionModel().AddBox( 0,  8, 16,  0).AddBox(8, 0, 8, 8);
			MODEL_INSIDE_CORNER_NE	= new CollisionModel().AddBox( 8,  0,  8,  7);
			MODEL_INSIDE_CORNER_NW	= new CollisionModel().AddBox( 0,  0,  8,  7);
			MODEL_INSIDE_CORNER_SW	= new CollisionModel().AddBox( 0,  8,  8,  8);
			MODEL_INSIDE_CORNER_SE	= new CollisionModel().AddBox( 8,  8,  8,  8);
			MODEL_BRIDGE_H_TOP		= new CollisionModel().AddBox( 0,  0, 16,  4);
			MODEL_BRIDGE_H_BOTTOM	= new CollisionModel().AddBox( 0, 13, 16,  3);
			MODEL_BRIDGE_H			= new CollisionModel().AddBox( 0,  0, 16,  4).AddBox( 0, 13, 16,  3);
			MODEL_BRIDGE_V_LEFT		= new CollisionModel().AddBox( 0,  0,  4, 16);
			MODEL_BRIDGE_V_RIGHT	= new CollisionModel().AddBox(12,  0,  4, 16);
			MODEL_BRIDGE_V			= new CollisionModel().AddBox( 0,  0,  4, 16).AddBox(12,  0,  4, 16);
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
			SpriteSheet sheetOverworld = new SpriteSheet(Resources.LoadImage("Images/tileset"), 16, 16, 0, 0, 1, 1);
			TILESET_OVERWORLD = new Tileset(sheetOverworld, 21, 36);
			TILESET_OVERWORLD.LoadConfig("Content/Tilesets/overworld.txt");
			TILESET_OVERWORLD.DefaultTile = new Point2I(1, 25);
			tilesetBuilder.Tileset = TILESET_OVERWORLD;

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
			// Irregular Ledges
			BuildTile( 0, 17).SetSolidModel(MODEL_CORNER_NW);
			BuildTile( 1, 17).SetSolidModel(MODEL_EDGE_N).AddFlags(TileFlags.Ledge);
			BuildTile( 2, 17).SetSolidModel(MODEL_CORNER_NE);
			BuildTile( 4, 17).SetSolidModel(MODEL_CORNER_NW);
			BuildTile( 5, 17).SetSolidModel(MODEL_EDGE_N).AddFlags(TileFlags.Ledge);
			BuildTile( 6, 17).SetSolidModel(MODEL_CORNER_NE);
			BuildTile( 0, 18).SetSolidModel(MODEL_EDGE_W).AddFlags(TileFlags.Ledge);
			BuildTile( 2, 18).SetSolidModel(MODEL_EDGE_E).AddFlags(TileFlags.Ledge);
			BuildTile( 3, 18).SetSolidModel(MODEL_EDGE_E).AddFlags(TileFlags.Ledge);
			BuildTile( 4, 18).SetSolidModel(MODEL_EDGE_W).AddFlags(TileFlags.Ledge);
			BuildTile( 6, 18).SetSolidModel(MODEL_EDGE_E).AddFlags(TileFlags.Ledge);
			BuildTile( 7, 18).SetSolidModel(MODEL_EDGE_E).AddFlags(TileFlags.Ledge);
			BuildTile( 0, 19).SetSolidModel(MODEL_CORNER_SW);
			BuildTile( 1, 19).SetSolidModel(MODEL_EDGE_S).AddFlags(TileFlags.Ledge);
			BuildTile( 2, 19).SetSolidModel(MODEL_CORNER_SE);
			BuildTile( 3, 19).SetSolidModel(MODEL_EDGE_W).AddFlags(TileFlags.Ledge);
			BuildTile( 4, 19).SetSolidModel(MODEL_CORNER_SW);
			BuildTile( 5, 19).SetSolidModel(MODEL_EDGE_S).AddFlags(TileFlags.Ledge);
			BuildTile( 6, 19).SetSolidModel(MODEL_CORNER_SE);
			BuildTile( 7, 19).SetSolidModel(MODEL_EDGE_W).AddFlags(TileFlags.Ledge);
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
			
			
			// Ledges.
			BuildTile( 1,  3).AddFlags(TileFlags.Ledge);

			TILESETS = new Tileset[] { TILESET_OVERWORLD };
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
		public static Tileset[] TILESETS;


		//-----------------------------------------------------------------------------
		// Images
		//-----------------------------------------------------------------------------


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

		public static SpriteSheet SHEET_EFFECTS;
		public static SpriteSheet SHEET_COLOR_EFFECTS;

		public static SpriteSheet SHEET_PLAYER;
		public static SpriteSheet SHEET_PLAYER_RED;
		public static SpriteSheet SHEET_PLAYER_BLUE;
		public static SpriteSheet SHEET_PLAYER_HURT;
		public static SpriteSheet SHEET_MONSTERS;
		public static SpriteSheet SHEET_MONSTERS_HURT;
		public static SpriteSheet SHEET_PLAYER_ITEMS;
	
		public static SpriteSheet SHEET_ZONESET_LARGE;
		public static SpriteSheet SHEET_ZONESET_SMALL;
		public static SpriteSheet SHEET_TILESET_OVERWORLD;
		public static SpriteSheet SHEET_GENERAL_TILES;
	
	
	//-----------------------------------------------------------------------------
	// Sprites
	//-----------------------------------------------------------------------------
	
	// Effects.
		public static Sprite SPR_SHADOW	= new Sprite();

	// Special Background tiles.
		public static Sprite SPR_TILE_DEFAULT	= new Sprite();	// The default ground background tile.
		public static Sprite SPR_TILE_DUG		= new Sprite();		// A hole in the ground created by a shovel.
	
	// Object tiles.
		public static Sprite SPR_BUSH				= new Sprite();
		public static Sprite SPR_CRYSTAL			= new Sprite();
		public static Sprite SPR_POT				= new Sprite();
		public static Sprite SPR_ROCK				= new Sprite();
		public static Sprite SPR_DIAMOND_ROCK		= new Sprite();
		public static Sprite SPR_SIGN				= new Sprite();
		public static Sprite SPR_GRASS				= new Sprite();
		public static Sprite SPR_MOVABLE_BLOCK		= new Sprite();
		public static Sprite SPR_BOMBABLE_BLOCK		= new Sprite();
		public static Sprite SPR_LOCKED_BLOCK		= new Sprite();
		public static Sprite SPR_CHEST				= new Sprite();
		public static Sprite SPR_CHEST_OPEN			= new Sprite();
		public static Sprite SPR_DIRT_PILE			= new Sprite();
		public static Sprite SPR_BURNABLE_TREE		= new Sprite();
		public static Sprite SPR_CACTUS				= new Sprite();
		public static Sprite SPR_BUTTON_UP			= new Sprite();
		public static Sprite SPR_BUTTON_DOWN		= new Sprite();
		public static Sprite SPR_LEVER_LEFT			= new Sprite();
		public static Sprite SPR_LEVER_RIGHT		= new Sprite();
		public static Sprite SPR_LANTERN_UNLIT		= new Sprite();
		public static Sprite SPR_EYE_STATUE			= new Sprite();
		public static Sprite SPR_BRIDGE_H			= new Sprite();
		public static Sprite SPR_BRIDGE_V			= new Sprite();
		public static Sprite SPR_COLOR_CUBE_SLOT	= new Sprite();
		public static Sprite SPR_CRACKED_FLOOR		= new Sprite();
		public static Sprite SPR_PIT				= new Sprite();
		public static Sprite SPR_PLANT				= new Sprite();
		public static Sprite SPR_ARMOS_STATUE		= new Sprite();
		public static Sprite SPR_OWL				= new Sprite();
		public static Sprite SPR_OWL_ACTIVATED		= new Sprite();

	// Item Icons.
		public static Sprite SPR_ITEM_SEED_EMBER				= new Sprite();
		public static Sprite SPR_ITEM_SEED_SCENT				= new Sprite();
		public static Sprite SPR_ITEM_SEED_PEGASUS				= new Sprite();
		public static Sprite SPR_ITEM_SEED_GALE					= new Sprite();
		public static Sprite SPR_ITEM_SEED_MYSTERY				= new Sprite();
		public static Sprite[] SPR_ITEM_SEEDS;

		public static Sprite SPR_ITEM_ICON_SWORD_1				= new Sprite();
		public static Sprite SPR_ITEM_ICON_SWORD_2				= new Sprite();
		public static Sprite SPR_ITEM_ICON_SWORD_3				= new Sprite();
		public static Sprite SPR_ITEM_ICON_SHIELD_1				= new Sprite();
		public static Sprite SPR_ITEM_ICON_SHIELD_2				= new Sprite();
		public static Sprite SPR_ITEM_ICON_SHIELD_3				= new Sprite();
		public static Sprite SPR_ITEM_ICON_SATCHEL				= new Sprite();
		public static Sprite SPR_ITEM_ICON_SATCHEL_EQUIPPED		= new Sprite();
		public static Sprite SPR_ITEM_ICON_SEED_SHOOTER			= new Sprite();
		public static Sprite SPR_ITEM_ICON_SEED_SHOOTER_EQUIPPED= new Sprite();
		public static Sprite SPR_ITEM_ICON_SLINGSHOT_1			= new Sprite();
		public static Sprite SPR_ITEM_ICON_SLINGSHOT_2			= new Sprite();
		public static Sprite SPR_ITEM_ICON_SLINGSHOT_2_EQUIPPED	= new Sprite();
		public static Sprite SPR_ITEM_ICON_BOMB					= new Sprite();
		public static Sprite SPR_ITEM_ICON_BOMBCHEW				= new Sprite();
		public static Sprite SPR_ITEM_ICON_SHOVEL				= new Sprite();
		public static Sprite SPR_ITEM_ICON_BRACELET				= new Sprite();
		public static Sprite SPR_ITEM_ICON_POWER_GLOVES			= new Sprite();
		public static Sprite SPR_ITEM_ICON_FEATHER				= new Sprite();
		public static Sprite SPR_ITEM_ICON_CAPE					= new Sprite();
		public static Sprite SPR_ITEM_ICON_BOOMERANG_1			= new Sprite();
		public static Sprite SPR_ITEM_ICON_BOOMERANG_2			= new Sprite();
		public static Sprite SPR_ITEM_ICON_SWITCH_HOOK_1		= new Sprite();
		public static Sprite SPR_ITEM_ICON_SWITCH_HOOK_2		= new Sprite();
		public static Sprite SPR_ITEM_ICON_MAGNET_GLOVES_BLUE	= new Sprite();
		public static Sprite SPR_ITEM_ICON_MAGNET_GLOVES_RED	= new Sprite();
		public static Sprite SPR_ITEM_ICON_CANE					= new Sprite();
		public static Sprite SPR_ITEM_ICON_FIRE_ROD				= new Sprite();
		public static Sprite SPR_ITEM_ICON_OCARINA				= new Sprite();
		public static Sprite SPR_ITEM_ICON_BOW					= new Sprite();
	
	// HUD Sprites.
		public static Sprite SPR_HUD_BRACKET_LEFT		= new Sprite();
		public static Sprite SPR_HUD_BRACKET_LEFT_A		= new Sprite();
		public static Sprite SPR_HUD_BRACKET_LEFT_B		= new Sprite();
		public static Sprite SPR_HUD_BRACKET_RIGHT		= new Sprite();
		public static Sprite SPR_HUD_BRACKET_RIGHT_A	= new Sprite();
		public static Sprite SPR_HUD_BRACKET_RIGHT_B	= new Sprite();
		public static Sprite SPR_HUD_BRACKET_LEFT_RIGHT	= new Sprite();
		public static Sprite SPR_HUD_HEART_0			= new Sprite();
		public static Sprite SPR_HUD_HEART_1			= new Sprite();
		public static Sprite SPR_HUD_HEART_2			= new Sprite();
		public static Sprite SPR_HUD_HEART_3			= new Sprite();
		public static Sprite SPR_HUD_HEART_4			= new Sprite();
		public static Sprite SPR_HUD_RUPEE				= new Sprite();
		public static Sprite SPR_HUD_ORE_CHUNK			= new Sprite();
		public static Sprite SPR_HUD_KEY				= new Sprite();
		public static Sprite SPR_HUD_BACKGROUND				= new Sprite();	// The lighter version when the inventory is closed.
		public static Sprite SPR_HUD_BACKGROUND_INVENTORY	= new Sprite();	// The darker version that when the inventory is opened.

	
	//-----------------------------------------------------------------------------
	// Animations
	//-----------------------------------------------------------------------------

		// Tile animations.
		public static Animation ANIM_TILE_WATER					= new Animation();
		public static Animation ANIM_TILE_OCEAN					= new Animation();
		public static Animation ANIM_TILE_OCEAN_SHORE			= new Animation();
		public static Animation ANIM_TILE_FLOWERS				= new Animation();
		public static Animation ANIM_TILE_WATERFALL				= new Animation();
		public static Animation ANIM_TILE_WATERFALL_BOTTOM		= new Animation();
		public static Animation ANIM_TILE_WATERFALL_TOP			= new Animation();
		public static Animation ANIM_TILE_WATER_DEEP			= new Animation();
		public static Animation ANIM_TILE_PUDDLE				= new Animation();
		public static Animation ANIM_TILE_LANTERN				= new Animation();
		public static Animation ANIM_TILE_LAVAFALL				= new Animation();
		public static Animation ANIM_TILE_LAVAFALL_BOTTOM		= new Animation();
		public static Animation ANIM_TILE_LAVAFALL_TOP			= new Animation();
	
		// Player animations.
		public static Animation ANIM_PLAYER_DEFAULT				= new Animation();
		public static Animation ANIM_PLAYER_CARRY				= new Animation();
		public static Animation ANIM_PLAYER_SHIELD				= new Animation();
		public static Animation ANIM_PLAYER_SHIELD_BLOCK		= new Animation();
		public static Animation ANIM_PLAYER_SHIELD_LARGE		= new Animation();
		public static Animation ANIM_PLAYER_SHIELD_LARGE_BLOCK	= new Animation();
		public static Animation ANIM_PLAYER_SWIM				= new Animation();
		public static Animation ANIM_PLAYER_PUSH				= new Animation();
		public static Animation ANIM_PLAYER_GRAB				= new Animation();
		public static Animation ANIM_PLAYER_PULL				= new Animation();
		public static Animation ANIM_PLAYER_DIG					= new Animation();
		public static Animation ANIM_PLAYER_THROW				= new Animation();
		public static Animation ANIM_PLAYER_SWING				= new Animation();
		public static Animation ANIM_PLAYER_SWING_BIG			= new Animation();
		public static Animation ANIM_PLAYER_STAB				= new Animation();
		public static Animation ANIM_PLAYER_SPIN				= new Animation();
		public static Animation ANIM_PLAYER_AIM					= new Animation();
		public static Animation ANIM_PLAYER_JUMP				= new Animation();
		public static Animation ANIM_PLAYER_SUBMERGED			= new Animation();
		public static Animation ANIM_PLAYER_DIE					= new Animation();
		public static Animation ANIM_PLAYER_FALL				= new Animation();
		public static Animation ANIM_PLAYER_DROWN				= new Animation();

		// Weapon animations.
		public static Animation ANIM_SWORD_HOLD					= new Animation();
		public static Animation ANIM_SWORD_CHARGED				= new Animation();
		public static Animation ANIM_SWORD_SWING				= new Animation();
		public static Animation ANIM_SWORD_SPIN					= new Animation();
		public static Animation ANIM_SWORD_STAB					= new Animation();

		// Projectile animations.
		public static Animation ANIM_ITEM_BOMB						= new Animation();
		public static Animation ANIM_PROJECTILE_PLAYER_ARROW		= new Animation();
		public static Animation ANIM_PROJECTILE_PLAYER_ARROW_CRASH	= new Animation();
	
		// Effect animations.
		public static Animation ANIM_EFFECT_DIRT				= new Animation();
		public static Animation ANIM_EFFECT_WATER_SPLASH		= new Animation();
		public static Animation ANIM_EFFECT_LAVA_SPLASH			= new Animation();
		public static Animation ANIM_EFFECT_RIPPLES				= new Animation();
		public static Animation ANIM_EFFECT_GRASS				= new Animation();
		public static Animation ANIM_EFFECT_ROCK_BREAK			= new Animation();
		public static Animation ANIM_EFFECT_SIGN_BREAK			= new Animation();
		public static Animation ANIM_EFFECT_LEAVES				= new Animation();
		public static Animation ANIM_EFFECT_GRASS_LEAVES		= new Animation();
		
		// Color effect animations.
		public static Animation ANIM_EFFECT_BOMB_EXPLOSION		= new Animation();
		public static Animation ANIM_EFFECT_MONSTER_EXPLOSION	= new Animation();
		public static Animation ANIM_EFFECT_SEED_EMBER			= new Animation();
		public static Animation ANIM_EFFECT_SEED_SCENT			= new Animation();
		public static Animation ANIM_EFFECT_SEED_PEGASUS		= new Animation();
		public static Animation ANIM_EFFECT_SEED_GALE			= new Animation();
		public static Animation ANIM_EFFECT_SEED_MYSTERY		= new Animation();
		public static Animation ANIM_EFFECT_PEGASUS_DUST		= new Animation(); // The dust the player sprinkles over himself when using a pegasus seed.
		public static Animation ANIM_EFFECT_OWL_SPARKLE			= new Animation();
		public static Animation ANIM_ITEM_SCENT_POD				= new Animation();
		public static Animation ANIM_EFFECT_FALLING_OBJECT		= new Animation();


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
	
	
	//-----------------------------------------------------------------------------
	// Fonts
	//-----------------------------------------------------------------------------

	public static RealFont FontDebugMenu;
	public static RealFont FontDebugMenuBold;
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
	public static RenderTarget2D RenderTargetDebug;

}
} // end namespace
