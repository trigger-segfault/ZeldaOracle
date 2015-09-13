using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
	class GameData {

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

		}
		
		//-----------------------------------------------------------------------------
		// Sprite Sheet Loading
		//-----------------------------------------------------------------------------

		// Loads the sprite sheets.
		private static void LoadSpriteSheets() {
			
			Image imageZoneset		= Resources.LoadImage("Images/zoneset");
			Image imageSheetPlayer	= Resources.LoadImage("Images/sheet_player");
			Image imageMenuSmall	= Resources.LoadImage("Images/sheet_menu_small");
			Image imageIconsThin	= Resources.LoadImage("Images/sheet_icons_thin");
			Image imageEffects		= Resources.LoadImage("Images/sheet_effects");
			
			SHEET_ZONESET_LARGE	= new SpriteSheet(imageZoneset, 16, 16, 0, 0, 1, 1);
			SHEET_ZONESET_SMALL	= new SpriteSheet(imageZoneset, 8, 8, 187, 0, 1, 1);

			SHEET_PLAYER		= new SpriteSheet(imageSheetPlayer, 16, 16, 0, 0, 1, 1);
			SHEET_MENU_SMALL	= new SpriteSheet(imageZoneset, 8, 8, 0, 0, 1, 1);
			SHEET_ICONS_THIN	= new SpriteSheet(imageZoneset, 8, 16, 0, 0, 1, 1);
			SHEET_EFFECTS		= new SpriteSheet(imageZoneset, 16, 16, 0, 0, 1, 1);

			SheetDebugMenu = Resources.LoadSpriteSheet(Resources.SpriteSheetDirectory + "sheet_debug_menu.conscript");

			//Resources.LoadSpriteSheets(Resources.SpriteSheetDirectory + "sheet_gamepad.conscript");
			//SheetGamePadControls	= Resources.GetSpriteSheet("sheet_gamepad_controls");
			//SheetGamePadArrows		= Resources.GetSpriteSheet("sheet_gamepad_arrows");

			//Resources.LoadSpriteSheets(Resources.SpriteSheetDirectory + "sheet_particles.conscript");
			//Resources.LoadSpriteSheets(Resources.SpriteSheetDirectory + "custom_sheets.conscript");
			//ParticleSR.UsingDegrees = true;
			//Resources.LoadParticles(Resources.ParticleDirectory + "particle_data.conscript");
			//ParticleSR.UsingDegrees = false;
			//Resources.LoadParticles(Resources.ParticleDirectory + "particle_data_before.conscript");
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
			spriteBuilder.SpriteSheet = SHEET_EFFECTS;
			BuildSprite(SPR_SHADOW, 0, 0, -8, -8);
			spriteBuilder.SpriteSheet = SHEET_ICONS_THIN;
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
			spriteBuilder.SpriteSheet = SHEET_MENU_SMALL;
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
			animationBuilder = new AnimationBuilder();
			
			
			// TILE ANIMATIONS:
			animationBuilder.SetSheet(SHEET_ZONESET_LARGE);
			BuildAnim(ANIM_LANTERN)			.AddFrameStrip(16, 1, 8, 4);

			animationBuilder.SetSheet(SHEET_ZONESET_SMALL);
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
			MODEL_BRIDGE_H			= new CollisionModel().AddBox( 0,  0, 16,  4);
			MODEL_BRIDGE_V_LEFT		= new CollisionModel().AddBox( 0,  0,  4, 16);
			MODEL_BRIDGE_V_RIGHT	= new CollisionModel().AddBox(12,  0,  4, 16);
			MODEL_BRIDGE_V			= new CollisionModel().AddBox( 0,  0,  4, 16);
		}
		
		
		//-----------------------------------------------------------------------------
		// Tliesets Loading
		//-----------------------------------------------------------------------------
		
		private static void LoadTilesets() {

			// OVERWORLD TILESET:
			SpriteSheet sheetOverworld = new SpriteSheet(Resources.LoadImage("Images/tileset"), 16, 16, 0, 0, 1, 1);
			TILESET_OVERWORLD = new Tileset(sheetOverworld, 21, 36);
			TILESET_OVERWORLD.DefaultTile = new Point2I(1, 25);
			for (int x = 0; x < TILESET_OVERWORLD.Width; x++) {
				for (int y = 0; y < TILESET_OVERWORLD.Height; y++) {
					TileData data = new TileData();
					data.Tileset		= TILESET_OVERWORLD;
					data.SheetLocation	= new Point2I(x, y);
					data.Sprite			= new Sprite(sheetOverworld, x, y, 0, 0);
					TILESET_OVERWORLD.TileData[x, y] = data;
				}
			}
			TILESET_OVERWORLD.LoadConfig("Content/Tilesets/overworld.txt");
			// Animations
			TILESET_OVERWORLD.TileData[ 1, 15].Animation = ANIM_WATER;
			TILESET_OVERWORLD.TileData[ 2, 15].Animation = ANIM_WATER_DEEP;
			TILESET_OVERWORLD.TileData[ 1, 14].Animation = ANIM_OCEAN;
			TILESET_OVERWORLD.TileData[ 2, 14].Animation = ANIM_OCEAN_SHORE;
			TILESET_OVERWORLD.TileData[ 1, 16].Animation = ANIM_PUDDLE;
			TILESET_OVERWORLD.TileData[ 0, 14].Animation = ANIM_WATERFALL_TOP;
			TILESET_OVERWORLD.TileData[ 0, 15].Animation = ANIM_WATERFALL;
			TILESET_OVERWORLD.TileData[ 0, 16].Animation = ANIM_WATERFALL_BOTTOM;
			TILESET_OVERWORLD.TileData[ 3, 23].Animation = ANIM_FLOWERS;
			// Collision Models
			TILESET_OVERWORLD.TileData[ 1,  4].CollisionModel = MODEL_EDGE_N; // cave entrances
			TILESET_OVERWORLD.TileData[ 0,  5].CollisionModel = MODEL_EDGE_N;
			TILESET_OVERWORLD.TileData[ 1,  5].CollisionModel = MODEL_EDGE_N;
			TILESET_OVERWORLD.TileData[12, 26].CollisionModel = MODEL_EDGE_N; // tree entrances
			TILESET_OVERWORLD.TileData[12, 28].CollisionModel = MODEL_EDGE_N;
			TILESET_OVERWORLD.TileData[17, 22].CollisionModel = MODEL_EDGE_N;
			TILESET_OVERWORLD.TileData[13,  7].CollisionModel = MODEL_DOORWAY; // doorways
			TILESET_OVERWORLD.TileData[13,  8].CollisionModel = MODEL_DOORWAY;
			TILESET_OVERWORLD.TileData[13,  9].CollisionModel = MODEL_DOORWAY;
			TILESET_OVERWORLD.TileData[19,  9].CollisionModel = MODEL_CORNER_NW;
			TILESET_OVERWORLD.TileData[20,  9].CollisionModel = MODEL_CORNER_NE;
			TILESET_OVERWORLD.TileData[15, 17].CollisionModel = MODEL_DOORWAY;
			TILESET_OVERWORLD.TileData[15, 18].CollisionModel = MODEL_DOORWAY;
			TILESET_OVERWORLD.TileData[15, 19].CollisionModel = MODEL_DOORWAY;
			TILESET_OVERWORLD.TileData[12, 18].CollisionModel = MODEL_CORNER_NW;
			TILESET_OVERWORLD.TileData[13, 18].CollisionModel = MODEL_CORNER_NE;
			TILESET_OVERWORLD.TileData[0, 17].CollisionModel = MODEL_CORNER_NW;
			// Ledges
			TILESET_OVERWORLD.TileData[1, 17].CollisionModel = MODEL_EDGE_N;
			TILESET_OVERWORLD.TileData[2, 17].CollisionModel = MODEL_CORNER_NE;
			TILESET_OVERWORLD.TileData[4, 17].CollisionModel = MODEL_CORNER_NW;
			TILESET_OVERWORLD.TileData[5, 17].CollisionModel = MODEL_EDGE_N;
			TILESET_OVERWORLD.TileData[6, 17].CollisionModel = MODEL_CORNER_NE;
			TILESET_OVERWORLD.TileData[0, 18].CollisionModel = MODEL_EDGE_W;
			TILESET_OVERWORLD.TileData[2, 18].CollisionModel = MODEL_EDGE_E;
			TILESET_OVERWORLD.TileData[3, 18].CollisionModel = MODEL_EDGE_E;
			TILESET_OVERWORLD.TileData[4, 18].CollisionModel = MODEL_EDGE_W;
			TILESET_OVERWORLD.TileData[6, 18].CollisionModel = MODEL_EDGE_E;
			TILESET_OVERWORLD.TileData[7, 18].CollisionModel = MODEL_EDGE_E;
			TILESET_OVERWORLD.TileData[0, 19].CollisionModel = MODEL_CORNER_SW;
			TILESET_OVERWORLD.TileData[1, 19].CollisionModel = MODEL_EDGE_S;
			TILESET_OVERWORLD.TileData[2, 19].CollisionModel = MODEL_CORNER_SE;
			TILESET_OVERWORLD.TileData[3, 19].CollisionModel = MODEL_EDGE_W;
			TILESET_OVERWORLD.TileData[4, 19].CollisionModel = MODEL_CORNER_SW;
			TILESET_OVERWORLD.TileData[5, 19].CollisionModel = MODEL_EDGE_S;
			TILESET_OVERWORLD.TileData[6, 19].CollisionModel = MODEL_CORNER_SE;
			TILESET_OVERWORLD.TileData[7, 19].CollisionModel = MODEL_EDGE_W;
			TILESET_OVERWORLD.TileData[0, 20].CollisionModel = MODEL_CORNER_NW;
			TILESET_OVERWORLD.TileData[1, 20].CollisionModel = MODEL_CORNER_NE;
			TILESET_OVERWORLD.TileData[2, 20].CollisionModel = MODEL_INSIDE_CORNER_SE;
			TILESET_OVERWORLD.TileData[3, 20].CollisionModel = MODEL_INSIDE_CORNER_SW;
			TILESET_OVERWORLD.TileData[4, 20].CollisionModel = MODEL_CORNER_NW;
			TILESET_OVERWORLD.TileData[5, 20].CollisionModel = MODEL_CORNER_NE;
			TILESET_OVERWORLD.TileData[6, 20].CollisionModel = MODEL_INSIDE_CORNER_SE;
			TILESET_OVERWORLD.TileData[7, 20].CollisionModel = MODEL_INSIDE_CORNER_SW;
			TILESET_OVERWORLD.TileData[0, 21].CollisionModel = MODEL_CORNER_SW;
			TILESET_OVERWORLD.TileData[1, 21].CollisionModel = MODEL_CORNER_SE;
			TILESET_OVERWORLD.TileData[2, 21].CollisionModel = MODEL_INSIDE_CORNER_NE;
			TILESET_OVERWORLD.TileData[3, 21].CollisionModel = MODEL_INSIDE_CORNER_NW;
			TILESET_OVERWORLD.TileData[4, 21].CollisionModel = MODEL_CORNER_SW;
			TILESET_OVERWORLD.TileData[5, 21].CollisionModel = MODEL_CORNER_SE;
			TILESET_OVERWORLD.TileData[6, 21].CollisionModel = MODEL_INSIDE_CORNER_NE;
			TILESET_OVERWORLD.TileData[7, 21].CollisionModel = MODEL_INSIDE_CORNER_NW;

			TILESETS = new Tileset[] { TILESET_OVERWORLD };
		}

		
		//-----------------------------------------------------------------------------
		// Font Loading
		//-----------------------------------------------------------------------------

		// Loads the fonts.
		private static void LoadFonts() {

			FontDebugMenu = Resources.LoadFont("Fonts/font_debug_menu");
			FontDebugMenuBold = Resources.LoadFont("Fonts/font_debug_menu_bold");
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


			//Resources.LoadSoundGroups(Resources.SoundDirectory + "sounds.conscript");

		}
		

		//-----------------------------------------------------------------------------
		// Music Loading
		//-----------------------------------------------------------------------------

		// Loads the music.
		private static void LoadMusic() {

			//Resources.LoadPlaylists(Resources.MusicDirectory + "music.conscript");
		}



		#pragma warning disable 169, 649 // The field 'example' is never used.
	
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

		public static SpriteAtlas SheetDebugMenu;
		public static SpriteAtlas SheetGamePadControls;
		public static SpriteAtlas SheetGamePadArrows;

		public static SpriteSheet SHEET_MENU_SMALL;
		public static SpriteSheet SHEET_ICONS_THIN;
		public static SpriteSheet SHEET_EFFECTS;

		public static SpriteSheet SHEET_PLAYER;
		public static SpriteSheet SHEET_PLAYER_HURT;
		public static SpriteSheet SHEET_MONSTERS;
		public static SpriteSheet SHEET_MONSTERS_HURT;
		
		public static SpriteSheet SHEET_ZONESET_LARGE;
		public static SpriteSheet SHEET_ZONESET_SMALL;
		public static SpriteSheet SHEET_TILESET_OVERWORLD;
	
	
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

		// Item Icons.
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
