using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;

using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;

//using ZeldaOracle.Common.Graphics.Particles;
using ZeldaOracle.Common.Scripts;
using System.IO;
//using ParticleGame.Project.Particles;

namespace ZeldaOracle.Game.Main {
/** <summary>
 * A static class for storing links to all game content.
 * </summary> */
class GameData {

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

	/** <summary> Loads the images. </summary> */
	private static void LoadImages() {

		Resources.LoadImage("Images/sheet_player");

		// TEMP: Create some player animations.

		//ANIM_PLAYER_DEFAULT.
		
		/*
		Image palette = Resources.LoadImage("Images/Palette");

		List<Color> colors = new List<Color>();


		int index = 0;
		Color[] colorData = new Color[palette.Width * palette.Height];
		palette.Texture.GetData<Color>(colorData);

		StreamWriter writer = new StreamWriter("Palette.txt");

		int[,] colorIndexes = new int[32, 12];
		int[,] colorRows = new int[32, 12];

		for (int i = 0; i < 18; i++) {
			for (int j = 0; j < 12; j++) {
				Color color = colorData[PointIndex(i * 13, (11 - j) * 10)];
				writer.Write("Color.FromAarg(" + color.A + ", " + color.R + ", " + color.G +"),");
				index++;
				if ((j + 1) % 4 != 0)
					writer.Write(" ");
				else
					writer.Write(writer.NewLine);
			}
			writer.Write(writer.NewLine);
		}

		//Image p = Resources.LoadImage("Images/ColorPal");



		writer.Close();
		*/
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

	//-----------------------------------------------------------------------------
	// Images
	//-----------------------------------------------------------------------------


	//-----------------------------------------------------------------------------
	// Sprite Sheets
	//-----------------------------------------------------------------------------

	public static SpriteSheet SheetDebugMenu;
	public static SpriteSheet SheetGamePadControls;
	public static SpriteSheet SheetGamePadArrows;

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
	public static Animation ANIM_WATER;
	public static Animation ANIM_OCEAN;
	public static Animation ANIM_OCEAN_SHORE;
	public static Animation ANIM_FLOWERS;
	public static Animation ANIM_WATERFALL;
	public static Animation ANIM_WATERFALL_BOTTOM;
	public static Animation ANIM_WATERFALL_TOP;
	public static Animation ANIM_WATER_DEEP;
	public static Animation ANIM_PUDDLE;
	public static Animation ANIM_LANTERN;
	public static Animation ANIM_LAVAFALL;
	public static Animation ANIM_LAVAFALL_BOTTOM;
	public static Animation ANIM_LAVAFALL_TOP;
	
	// Player animations.
	public static Animation ANIM_PLAYER_DEFAULT;
	public static Animation ANIM_PLAYER_HOLD;
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
	public static Animation ANIM_PLAYER_SPIN_SWORD;
	public static Animation ANIM_PLAYER_AIM;
	public static Animation ANIM_PLAYER_JUMP;
	public static Animation ANIM_PLAYER_DIVE;
	public static Animation ANIM_PLAYER_DIE;
	public static Animation ANIM_PLAYER_FALL;
	public static Animation ANIM_PLAYER_DROWN;
	
	// Projectile animations.
	public static Animation ANIM_PROJECTILE_PLAYER_ARROW;
	public static Animation ANIM_PROJECTILE_PLAYER_ARROW_CRASH;
	
	// Effect animations.
	public static Animation ANIM_EFFECT_DIRT;
	

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

}
} // end namespace
