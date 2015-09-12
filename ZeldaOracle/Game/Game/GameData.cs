using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Main.ResourceBuilders;
using ZeldaOracle.Common.Scripts;

namespace ZeldaOracle.Game {

// A static class for storing links to all game content.
class GameData {

	private static AnimationBuilder animationBuilder;

	//=========== LOADING ============
	#region Loading
	//--------------------------------
	#region Initialize

	/** <summary> Initializes and loads the game content. </summary> */
	public static void Initialize() {
		Console.WriteLine("Loading Images");
		LoadImages();

		Console.WriteLine("Loading Sprite Sheets");
		LoadSpriteSheets();

		Console.WriteLine("Loading Fonts");
		LoadFonts();

		Console.WriteLine("Loading Shaders");
		LoadShaders();

		Console.WriteLine("Loading Sound Effects");
		LoadSounds();

		Console.WriteLine("Loading Music");
		LoadMusic();

		Console.WriteLine("Loading Languages");
		LoadLanguages();

	}

	#endregion
	//--------------------------------
	#region Images

	private static int PointIndex(int x, int y) {
		return 234 * y + x;
	}
	
	private static AnimationBuilder BuildAnim(Animation animation) {
		return animationBuilder.Begin(animation);
	}

	/** <summary> Loads the images. </summary> */
	private static void LoadImages() {
		animationBuilder = new AnimationBuilder();

		Image sheetPlayer = Resources.LoadImage("Images/sheet_player");
		Image imageZoneGridSmall = Resources.LoadImage("Images/zoneset");
		IMAGE_TILESET = Resources.LoadImage("Images/tileset");

		SHEET_PLAYER = new SpriteSheet(sheetPlayer, 16, 16, 0, 0, 1, 1);
		SpriteSheet sheetZone = new SpriteSheet(imageZoneGridSmall, 16, 16, 0, 0, 1, 1);
		SpriteSheet sheetZoneSmall = new SpriteSheet(imageZoneGridSmall, 8, 8, 187, 0, 1, 1);

		// TEMP: Create some player animations.


		// TILE ANIMATIONS:
		animationBuilder.SetSheet(sheetZone);
		BuildAnim(ANIM_LANTERN)			.AddFrameStrip(16, 1, 8, 4);

		animationBuilder.SetSheet(sheetZoneSmall);
		BuildAnim(ANIM_WATER)			.AddFrameStrip(16, 0, 4, 4).MakeQuad();
		BuildAnim(ANIM_WATER_DEEP)		.AddFrameStrip(16, 0, 5, 4).MakeQuad();
		BuildAnim(ANIM_WATERFALL)		.AddFrameStrip(8, 0, 6, 4).MakeQuad();
		BuildAnim(ANIM_LAVAFALL)		.AddFrameStrip(8, 0, 8, 4).MakeQuad();
		BuildAnim(ANIM_PUDDLE)			.AddFrameStrip(16, 4,10, 3).AddFrame(16, 5,10).MakeQuad();
		BuildAnim(ANIM_WATERFALL_BOTTOM).InsertFrameStrip(0, 8, 0,6, 4, 0,0)
										.InsertFrameStrip(0, 8, 0,6, 4, 8,0)
										.InsertFrameStrip(0, 8, 0,7, 4, 0,8)
										.InsertFrameStrip(0, 8, 0,7, 4, 8,8);
		BuildAnim(ANIM_LAVAFALL_BOTTOM)	.InsertFrameStrip(0, 8, 0,8, 4, 0,0)
										.InsertFrameStrip(0, 8, 0,8, 4, 8,0)
										.InsertFrameStrip(0, 8, 0,9, 4, 0,8)
										.InsertFrameStrip(0, 8, 0,9, 4, 8,8);
		BuildAnim(ANIM_FLOWERS)			.InsertFrameStrip(0, 16,  4,9, 4, 0,0)
										.InsertFrameStrip(0, 16,  4,9, 4, 8,8)
										.InsertFrame(0, 64, 7,10, 8,0)
										.InsertFrame(0, 64, 7,10, 0,8);
		BuildAnim(ANIM_OCEAN)			.InsertFrameStrip(0, 16, 4,4, 4, 0,0, 0,1)
										.InsertFrameStrip(0, 16, 5,4, 4, 8,0, 0,1)
										.InsertFrameStrip(0, 16, 4,4, 4, 0,8, 0,1)
										.InsertFrameStrip(0, 16, 5,4, 4, 8,8, 0,1);
		BuildAnim(ANIM_OCEAN_SHORE)		.InsertFrameStrip(0, 16, 6,4, 4, 0,0, 0,1)
										.InsertFrameStrip(0, 16, 7,4, 4, 8,0, 0,1)
										.InsertFrameStrip(0, 16, 4,4, 4, 0,8, 0,1)
										.InsertFrameStrip(0, 16, 5,4, 4, 8,8, 0,1);
		
		// PLAYER ANIMATIONS:
		animationBuilder.SetSheet(SHEET_PLAYER);
		BuildAnim(ANIM_PLAYER_DEFAULT)			.AddFrameStrip(8, 0,0, 2).Offset(-8, -16).MakeDynamic(4, 2,0);
		BuildAnim(ANIM_PLAYER_SHIELD)			.AddFrameStrip(8, 0,1, 2).Offset(-8, -16).MakeDynamic(4, 2,0);
		BuildAnim(ANIM_PLAYER_SHIELD_BLOCK)		.AddFrameStrip(8, 0,2, 2).Offset(-8, -16).MakeDynamic(4, 2,0);
		BuildAnim(ANIM_PLAYER_HOLD)				.AddFrameStrip(8, 0,5, 2).Offset(-8, -16).MakeDynamic(4, 2,0);
		BuildAnim(ANIM_PLAYER_PUSH)				.AddFrameStrip(8, 0,6, 2).Offset(-8, -16).MakeDynamic(4, 2,0);
		BuildAnim(ANIM_PLAYER_PULL)				.AddFrameStrip(8, 0,7, 2).Offset(-8, -16).SetLooped(false).MakeDynamic(4, 2,0);
		BuildAnim(ANIM_PLAYER_DIG)				.AddFrame(8, 0,9).AddFrame(16, 1,9).Offset(-8, -16).SetLooped(false).MakeDynamic(4, 2,0);
		BuildAnim(ANIM_PLAYER_THROW)			.AddFrame(9, 0,4).Offset(-8, -16).SetLooped(false).MakeDynamic(4, 2,0);
		BuildAnim(ANIM_PLAYER_FALL).AddFrame(16, 1, 20, 0, 0).AddFrame(10, 2, 20, 0, 0).AddFrame(11, 3, 20, 0, 0).Offset(-8, -16).SetLooped(false);
		BuildAnim(ANIM_PLAYER_SHIELD_LARGE)			.AddFrameStrip(8, 0,3, 2).Offset(-8, -16).CreateSubStrip()
													.AddFrameStrip(8, 2,1, 2).Offset(-8, -16).MakeDynamic(3, 2,0);
		BuildAnim(ANIM_PLAYER_SHIELD_LARGE_BLOCK)	.AddFrameStrip(8, 0,2, 2).Offset(-8, -16).MakeDynamic(3, 2,0)
													.CreateSubStrip().AddFrameStrip(8, 2,3, 2).Offset(-8, -16);
		BuildAnim(ANIM_PLAYER_JUMP)
			.AddFrame(9, 0, 11).AddFrame(9, 1, 11).AddFrame(6, 2, 11).AddFrame(6, 1, 0).Offset(-8, -16).SetLooped(false).CreateSubStrip()
			.AddFrame(9, 3, 11).AddFrame(9, 4, 11).AddFrame(6, 5, 11).AddFrame(6, 3, 0).Offset(-8, -16).SetLooped(false).CreateSubStrip()
			.AddFrame(9, 0, 12).AddFrame(9, 1, 12).AddFrame(6, 2, 12).AddFrame(6, 5, 0).Offset(-8, -16).SetLooped(false).CreateSubStrip()
			.AddFrame(9, 3, 12).AddFrame(9, 4, 12).AddFrame(6, 5, 12).AddFrame(6, 7, 0).Offset(-8, -16).SetLooped(false).CreateSubStrip();


		// TEMP: Create a tileset.
		TILESET_DEFAULT = new Tileset(21, 36);
		TILESET_DEFAULT.DefaultTile = new Point2I(1, 25);

		for (int x = 0; x < TILESET_DEFAULT.Width; x++) {
			for (int y = 0; y < TILESET_DEFAULT.Height; y++) {
				TileData data = new TileData();
				data.Tileset		= TILESET_DEFAULT;
				data.SheetLocation	= new Point2I(x, y);
				data.Sprite			= new Sprite(IMAGE_TILESET, new Rectangle2I(x * 17, y * 17, 16, 16), Point2I.Zero);
				TILESET_DEFAULT.TileData[x, y] = data;
			}
		}
		// Animations.
		TILESET_DEFAULT.TileData[ 1, 15].Animation = ANIM_WATER;
		TILESET_DEFAULT.TileData[ 2, 15].Animation = ANIM_WATER_DEEP;
		TILESET_DEFAULT.TileData[ 1, 14].Animation = ANIM_OCEAN;
		TILESET_DEFAULT.TileData[ 2, 14].Animation = ANIM_OCEAN_SHORE;
		TILESET_DEFAULT.TileData[ 1, 16].Animation = ANIM_PUDDLE;
		TILESET_DEFAULT.TileData[ 0, 14].Animation = ANIM_WATERFALL_TOP;
		TILESET_DEFAULT.TileData[ 0, 15].Animation = ANIM_WATERFALL;
		TILESET_DEFAULT.TileData[ 0, 16].Animation = ANIM_WATERFALL_BOTTOM;
		TILESET_DEFAULT.TileData[ 3, 23].Animation = ANIM_FLOWERS;

		TILESETS = new Tileset[] { TILESET_DEFAULT };
		
	}

	#endregion
	//--------------------------------
	#region Sprite Sheets

	/** <summary> Loads the sprite sheets. </summary> */
	private static void LoadSpriteSheets() {

		SheetDebugMenu			= Resources.LoadSpriteSheet(Resources.SpriteSheetDirectory + "sheet_debug_menu.conscript");

		//Resources.LoadSpriteSheets(Resources.SpriteSheetDirectory + "sheet_gamepad.conscript");
		//SheetGamePadControls	= Resources.GetSpriteSheet("sheet_gamepad_controls");
		//SheetGamePadArrows		= Resources.GetSpriteSheet("sheet_gamepad_arrows");

		Resources.LoadSpriteSheets(Resources.SpriteSheetDirectory + "sheet_particles.conscript");
		//Resources.LoadSpriteSheets(Resources.SpriteSheetDirectory + "custom_sheets.conscript");
		//ParticleSR.UsingDegrees = true;
		//Resources.LoadParticles(Resources.ParticleDirectory + "particle_data.conscript");
		//ParticleSR.UsingDegrees = false;
		//Resources.LoadParticles(Resources.ParticleDirectory + "particle_data_before.conscript");
	}

	#endregion
	//--------------------------------
	#region Fonts

	/** <summary> Loads the fonts. </summary> */
	private static void LoadFonts() {

		FontDebugMenu = Resources.LoadFont("Fonts/font_debug_menu");
		FontDebugMenuBold = Resources.LoadFont("Fonts/font_debug_menu_bold");
	}

	#endregion
	//--------------------------------
	#region Shaders

	/** <summary> Loads the shaders. </summary> */
	private static void LoadShaders() {

	}

	#endregion
	//--------------------------------
	#region Sound Effects

	/** <summary> Loads the sound effects. </summary> */
	private static void LoadSounds() {


		//Resources.LoadSoundGroups(Resources.SoundDirectory + "sounds.conscript");

	}

	#endregion
	//--------------------------------
	#region Music

	/** <summary> Loads the music. </summary> */
	private static void LoadMusic() {

		//Resources.LoadPlaylists(Resources.MusicDirectory + "music.conscript");
	}

	#endregion
	//--------------------------------
	#region Languages

	/** <summary> Loads the languages. </summary> */
	private static void LoadLanguages() {

	}

	#endregion
	//--------------------------------
	#endregion

	//========== GAME DATA ===========

	#pragma warning disable 169, 649 // The field 'example' is never used.
	
	//-----------------------------------------------------------------------------
	// Tilesets
	//-----------------------------------------------------------------------------

	public static Tileset TILESET_DEFAULT;
	public static Tileset[] TILESETS;

	//-----------------------------------------------------------------------------
	// Images
	//-----------------------------------------------------------------------------

	public static Image IMAGE_TILESET;

	//-----------------------------------------------------------------------------
	// Sprite Sheets
	//-----------------------------------------------------------------------------

	public static SpriteAtlas SheetDebugMenu;
	public static SpriteAtlas SheetGamePadControls;
	public static SpriteAtlas SheetGamePadArrows;
	
	public static SpriteSheet SHEET_PLAYER;
	public static SpriteSheet SHEET_PLAYER_HURT;
	public static SpriteSheet SHEET_MONSTERS;
	public static SpriteSheet SHEET_MONSTERS_HURT;
	
	
	//-----------------------------------------------------------------------------
	// Sprites
	//-----------------------------------------------------------------------------
	
	// Effects.
	public static SpriteEx SPR_SHADOW;

	// Special Background tiles.
	public static SpriteEx SPR_TILE_DEFAULT;	// The default ground background tile.
	public static SpriteEx SPR_TILE_DUG;		// A hole in the ground created by a shovel.
	
	// Object tiles.
	public static SpriteEx SPR_BUSH;
	public static SpriteEx SPR_CRYSTAL;
	public static SpriteEx SPR_POT;
	public static SpriteEx SPR_ROCK;
	public static SpriteEx SPR_DIAMOND_ROCK;
	public static SpriteEx SPR_SIGN;
	public static SpriteEx SPR_GRASS;
	public static SpriteEx SPR_MOVABLE_BLOCK;
	public static SpriteEx SPR_BOMBABLE_BLOCK;
	public static SpriteEx SPR_LOCKED_BLOCK;
	public static SpriteEx SPR_CHEST;
	public static SpriteEx SPR_CHEST_OPEN;
	public static SpriteEx SPR_DIRT_PILE;
	public static SpriteEx SPR_BURNABLE_TREE;
	public static SpriteEx SPR_CACTUS;
	public static SpriteEx SPR_BUTTON_UP;
	public static SpriteEx SPR_BUTTON_DOWN;
	public static SpriteEx SPR_LEVER_LEFT;
	public static SpriteEx SPR_LEVER_RIGHT;
	public static SpriteEx SPR_LANTERN_UNLIT;
	public static SpriteEx SPR_EYE_STATUE;
	public static SpriteEx SPR_BRIDGE_H;
	public static SpriteEx SPR_BRIDGE_V;
	public static SpriteEx SPR_COLOR_CUBE_SLOT;
	public static SpriteEx SPR_CRACKED_FLOOR;
	public static SpriteEx SPR_PIT;
	public static SpriteEx SPR_PLANT;
	public static SpriteEx SPR_ARMOS_STATUE;

	// Item Icons.
	public static SpriteEx SPR_ITEM_ICON_SWORD_1;
	public static SpriteEx SPR_ITEM_ICON_SWORD_2;
	public static SpriteEx SPR_ITEM_ICON_SWORD_3;
	public static SpriteEx SPR_ITEM_ICON_SHIELD_1;
	public static SpriteEx SPR_ITEM_ICON_SHIELD_2;
	public static SpriteEx SPR_ITEM_ICON_SHIELD_3;
	public static SpriteEx SPR_ITEM_ICON_SATCHEL;
	public static SpriteEx SPR_ITEM_ICON_SATCHEL_EQUIPPED;
	public static SpriteEx SPR_ITEM_ICON_SEED_SHOOTER;
	public static SpriteEx SPR_ITEM_ICON_SEED_SHOOTER_EQUIPPED;
	public static SpriteEx SPR_ITEM_ICON_SLINGSHOT_1;
	public static SpriteEx SPR_ITEM_ICON_SLINGSHOT_2;
	public static SpriteEx SPR_ITEM_ICON_SLINGSHOT_2_EQUIPPED;
	public static SpriteEx SPR_ITEM_ICON_BOMB;
	public static SpriteEx SPR_ITEM_ICON_BOMBCHEW;
	public static SpriteEx SPR_ITEM_ICON_SHOVEL;
	public static SpriteEx SPR_ITEM_ICON_BRACELET;
	public static SpriteEx SPR_ITEM_ICON_POWER_GLOVES;
	public static SpriteEx SPR_ITEM_ICON_FEATHER;
	public static SpriteEx SPR_ITEM_ICON_CAPE;
	public static SpriteEx SPR_ITEM_ICON_BOOMERANG_1;
	public static SpriteEx SPR_ITEM_ICON_BOOMERANG_2;
	public static SpriteEx SPR_ITEM_ICON_SWITCH_HOOK_1;
	public static SpriteEx SPR_ITEM_ICON_SWITCH_HOOK_2;
	public static SpriteEx SPR_ITEM_ICON_MAGNET_GLOVES_BLUE;
	public static SpriteEx SPR_ITEM_ICON_MAGNET_GLOVES_RED;
	public static SpriteEx SPR_ITEM_ICON_CANE;
	public static SpriteEx SPR_ITEM_ICON_FIRE_ROD;
	public static SpriteEx SPR_ITEM_ICON_OCARINA;
	public static SpriteEx SPR_ITEM_ICON_BOW;
	
	// HUD Sprites.
	public static SpriteEx SPR_HUD_BRACKET_LEFT;
	public static SpriteEx SPR_HUD_BRACKET_LEFT_A;
	public static SpriteEx SPR_HUD_BRACKET_LEFT_B;
	public static SpriteEx SPR_HUD_BRACKET_RIGHT;
	public static SpriteEx SPR_HUD_BRACKET_RIGHT_A;
	public static SpriteEx SPR_HUD_BRACKET_RIGHT_B;
	public static SpriteEx SPR_HUD_BRACKET_LEFT_RIGHT;
	public static SpriteEx SPR_HUD_HEART_0;
	public static SpriteEx SPR_HUD_HEART_1;
	public static SpriteEx SPR_HUD_HEART_2;
	public static SpriteEx SPR_HUD_HEART_3;
	public static SpriteEx SPR_HUD_HEART_4;
	public static SpriteEx SPR_HUD_RUPEE;
	public static SpriteEx SPR_HUD_ORE_CHUNK;
	public static SpriteEx SPR_HUD_KEY;

	
	//-----------------------------------------------------------------------------
	// Animations
	//-----------------------------------------------------------------------------

	// Tile animations.
	public static Animation ANIM_WATER						= new Animation();
	public static Animation ANIM_OCEAN						= new Animation();
	public static Animation ANIM_OCEAN_SHORE				= new Animation();
	public static Animation ANIM_FLOWERS					= new Animation();
	public static Animation ANIM_WATERFALL					= new Animation();
	public static Animation ANIM_WATERFALL_BOTTOM			= new Animation();
	public static Animation ANIM_WATERFALL_TOP				= new Animation();
	public static Animation ANIM_WATER_DEEP					= new Animation();
	public static Animation ANIM_PUDDLE						= new Animation();
	public static Animation ANIM_LANTERN					= new Animation();
	public static Animation ANIM_LAVAFALL					= new Animation();
	public static Animation ANIM_LAVAFALL_BOTTOM			= new Animation();
	public static Animation ANIM_LAVAFALL_TOP				= new Animation();
	
	// Player animations.
	public static Animation ANIM_PLAYER_DEFAULT				= new Animation();
	public static Animation ANIM_PLAYER_HOLD				= new Animation();
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
	public static Animation ANIM_PLAYER_SPIN_SWORD			= new Animation();
	public static Animation ANIM_PLAYER_AIM					= new Animation();
	public static Animation ANIM_PLAYER_JUMP				= new Animation();
	public static Animation ANIM_PLAYER_DIVE				= new Animation();
	public static Animation ANIM_PLAYER_DIE					= new Animation();
	public static Animation ANIM_PLAYER_FALL				= new Animation();
	public static Animation ANIM_PLAYER_DROWN				= new Animation();
	
	// Projectile animations.
	public static Animation ANIM_PROJECTILE_PLAYER_ARROW		= new Animation();
	public static Animation ANIM_PROJECTILE_PLAYER_ARROW_CRASH	= new Animation();
	
	// Effect animations.
	public static Animation ANIM_EFFECT_DIRT	= new Animation();
	

	//-----------------------------------------------------------------------------
	// Collision Models.
	//-----------------------------------------------------------------------------
	/*
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
	*/
	
	//-----------------------------------------------------------------------------
	// Fonts
	//-----------------------------------------------------------------------------

	public static Font FontDebugMenu;
	public static Font FontDebugMenuBold;

	
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

	
#pragma warning restore 169, 649 // The field 'example' is never used.

}
} // end namespace
