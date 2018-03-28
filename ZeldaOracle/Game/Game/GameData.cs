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
using ZeldaOracle.Game.Entities.Monsters;
using ZeldaOracle.Game.Tiles.ActionTiles;
using System.Diagnostics;
using ZeldaOracle.Common.Scripts;
using ZeldaOracle.Common.Util;
using ZeldaOracle.Common.Graphics.Shaders;

namespace ZeldaOracle.Game {
	
	// A static class for storing links to all game content.
	public partial class GameData {
		
		//-----------------------------------------------------------------------------
		// Initialization
		//-----------------------------------------------------------------------------

		// Initializes and loads the game content. NOTE: The order here is important.
		public static void Initialize(bool preloadSprites = true) {
			/*
			CommandReferenceParam param = CommandParamParser.ParseReferenceParams(
				"(any objs...)...");
				//"any name, (int gridLocationX, int gridLocationY), (int drawOffsetX, any drawOffsetY) = (0, fuck)");
				//"float x, string y, bool width, (int a, int b) = (1, 2), float height, (int hoop) = (0), bool asd");
				//"float x, string y, bool width, (int a, int b, (string c)), float height");

			Console.WriteLine(CommandParamParser.ToString(param));
			throw new LoadContentException("END");
			*/

			Logs.InitializeLogs();

			Stopwatch watch = Stopwatch.StartNew();
			Stopwatch audioWatch = new Stopwatch();
			Stopwatch spriteWatch = new Stopwatch();
			ScriptReader.Watch.Restart();

			if (preloadSprites &&
				Resources.PalettedSpriteDatabase.DatabaseFileExists())
				Resources.PalettedSpriteDatabase.Load();

			Logs.Initialization.LogNotice("Loading Palette Dictionaries");
			LoadPaletteDictionaries();

			Logs.Initialization.LogNotice("Loading Palettes");
			LoadPalettes();

			Logs.Initialization.LogNotice("Loading Shaders");
			LoadShaders();

			Logs.Initialization.LogNotice("Pre-Loading Zones");
			LoadZonesPreTileData();

			Logs.Initialization.LogNotice("Loading Images");
			LoadImages();

			spriteWatch.Start();

			Logs.Initialization.LogNotice("Loading Sprites");
			LoadSprites();

			spriteWatch.Stop();

			Logs.Initialization.LogNotice("Loading Animations");
			LoadAnimations();

			Logs.Initialization.LogNotice("Loading Collision Models");
			LoadCollisionModels();
			
			Logs.Initialization.LogNotice("Loading Fonts");
			LoadFonts();

			audioWatch.Start();

			Logs.Initialization.LogNotice("Loading Sound Effects");
			LoadSounds();

			Logs.Initialization.LogNotice("Loading Music");
			LoadMusic();

			audioWatch.Stop();
			
			Logs.Initialization.LogNotice("Loading Inventory");
			LoadInventory();

			Logs.Initialization.LogNotice("Loading Rewards");
			LoadRewards();

			Logs.Initialization.LogNotice("Loading Tiles");
			LoadTiles();

			Logs.Initialization.LogNotice("Loading Tilesets");
			LoadTilesets();

			Logs.Initialization.LogNotice("Loading Zones");
			LoadZonesPostTileData();

			//Logs.Initialization.LogNotice("Took {0} ms to load sprites.",
			//	spriteWatch.ElapsedMilliseconds);
			//Logs.Initialization.LogNotice("Took {0} ms to load audio.",
			//	audioWatch.ElapsedMilliseconds);
			Logs.Initialization.LogInfo("Took {0} ms to parse conscripts.",
				ScriptReader.Watch.ElapsedMilliseconds);
			Logs.Initialization.LogInfo("Took {0} ms to load game data.",
				watch.ElapsedMilliseconds);

			if (!Resources.PalettedSpriteDatabase.IsPreloaded) {
				watch.Restart();
				Resources.PalettedSpriteDatabase.Save();
				
				Logs.Initialization.LogInfo(
					"Took {0} ms to save sprite database.",
					watch.ElapsedMilliseconds);
			}
		}

		/// <summary>Unloads everything from GameData.</summary>
		public static void Uninitialize() {
			// Unset all game data
			IEnumerable<FieldInfo> fields = typeof(GameData).GetFields()
				.Where(field => field.IsStatic);
			foreach (FieldInfo field in fields) {
				field.SetValue(null, null);
			}
		}


		//-----------------------------------------------------------------------------
		// Internal
		//-----------------------------------------------------------------------------

		/// <summary>Assign static fields from their corresponding loaded resources.</summary>
		private static void IntegrateResources<T>(string prefix,
			bool subtypes = false)
		{
			IEnumerable<FieldInfo> fields = typeof(GameData).GetFields()
				.Where(field =>
					field.Name.StartsWith(prefix) &&
					(subtypes || field.FieldType == typeof(T)) &&
					(!subtypes || typeof(T).IsAssignableFrom(field.FieldType)) &&
					field.IsStatic);

			// Set the values of the static fields to their corresponding loaded resources.
			foreach (FieldInfo field in fields) {
				string name = field.Name.ToLower().Remove(0, prefix.Length);
				
				if (Resources.Contains<T>(name)) {
					T resource = Resources.Get<T>(name);
					if (subtypes && !field.FieldType.IsAssignableFrom(resource.GetType()))
						throw new LoadContentException("Field of type '" +
							field.FieldType.Name + " cannot be assigned to resource '" +
							name + "' of type '" + resource.GetType().Name + "'!");
					field.SetValue(null, resource);
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
			Dictionary<string, T> dictionary = Resources.GetDictionary<T>();
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
		}


		//-----------------------------------------------------------------------------
		// Collision Model Loading
		//-----------------------------------------------------------------------------

		private static void LoadCollisionModels() {
			Resources.LoadCollisionModels("Data/collision_models.conscript");
			IntegrateResources<CollisionModel>("MODEL_");

			// Leap ledges' models must be constant to prevent any bugs
			MODEL_LEAP_LEDGE_RIGHT	= new CollisionModel(new Rectangle2I(12, 0, 4, 16));
			MODEL_LEAP_LEDGE_UP		= new CollisionModel(new Rectangle2I(0, 0, 16, 4));
			MODEL_LEAP_LEDGE_LEFT	= new CollisionModel(new Rectangle2I(0, 0, 4, 16));
			MODEL_LEAP_LEDGE_DOWN	= new CollisionModel(new Rectangle2I(0, 12, 16, 4));
			MODEL_LEAP_LEDGES = new CollisionModel[Direction.Count] {
				MODEL_LEAP_LEDGE_RIGHT,
				MODEL_LEAP_LEDGE_UP,
				MODEL_LEAP_LEDGE_LEFT,
				MODEL_LEAP_LEDGE_DOWN
			};
		}


		//-----------------------------------------------------------------------------
		// Zone Loading
		//-----------------------------------------------------------------------------

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

			IntegrateResources<Tileset>("TILESET_");
		}

		private static void LoadTiles() {
			Resources.LoadTiles("Tiles/tiles.conscript");
		}


		//-----------------------------------------------------------------------------
		// Font Loading
		//-----------------------------------------------------------------------------

		// Loads the fonts.
		private static void LoadFonts() {
			Resources.LoadGameFonts("Fonts/fonts.conscript");

			IntegrateResources<GameFont>("FONT_");
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

			// Menu palette is made programatically as it's just a 16 unit offset (Maxes at 248)
			Palette entitiesMenu = new Palette(Resources.Get<Palette>("entities_default"));
			foreach (var pair in entitiesMenu.GetDefinedConsts()) {
				for (int i = 0; i < PaletteDictionary.ColorGroupSize; i++) {
					if (pair.Value[i].IsUndefined)
						continue;
					Color color = pair.Value[i].Color;
					color.R = (byte) GMath.Min(248, color.R + 16);
					color.G = (byte) GMath.Min(248, color.G + 16);
					color.B = (byte) GMath.Min(248, color.B + 16);
					pair.Value[i].Color = color;
				}
			}
			foreach (var pair in entitiesMenu.GetDefinedColors()) {
				for (int i = 0; i < PaletteDictionary.ColorGroupSize; i++) {
					if (pair.Value[i].IsUndefined)
						continue;
					Color color = pair.Value[i].Color;
					color.R = (byte) GMath.Min(248, color.R + 16);
					color.G = (byte) GMath.Min(248, color.G + 16);
					color.B = (byte) GMath.Min(248, color.B + 16);
					pair.Value[i].Color = color;
				}
			}
			entitiesMenu.UpdatePalette();
			Resources.Add<Palette>("entities_menu", entitiesMenu);

			IntegrateResources<Palette>("PAL_");
			
			// Map monster colors to color definitions
			MONSTER_COLOR_DEFINITION_MAP = new string[(int) MonsterColor.Count];
			MONSTER_COLOR_DEFINITION_MAP[(int) MonsterColor.Red]			= "red";
			MONSTER_COLOR_DEFINITION_MAP[(int) MonsterColor.Blue]			= "blue";
			MONSTER_COLOR_DEFINITION_MAP[(int) MonsterColor.Orange]			= "orange";
			MONSTER_COLOR_DEFINITION_MAP[(int) MonsterColor.Green]			= "green";
			MONSTER_COLOR_DEFINITION_MAP[(int) MonsterColor.DarkBlue]		= "shaded_blue";
			MONSTER_COLOR_DEFINITION_MAP[(int) MonsterColor.DarkRed]		= "shaded_red";
			MONSTER_COLOR_DEFINITION_MAP[(int) MonsterColor.Gold]			= "gold";
			MONSTER_COLOR_DEFINITION_MAP[(int) MonsterColor.InverseBlue]	= "inverse_blue";
		}


		//-----------------------------------------------------------------------------
		// Shader Loading
		//-----------------------------------------------------------------------------

		// Loads the shaders.
		private static void LoadShaders() {
			Resources.LoadShader<PaletteShader>("Shaders/palette");
			Resources.LoadShader<SineShiftShader>("Shaders/sine_shift");
			IntegrateResources<Shader>("SHADER_", true);
			/*SHADER_PALETTE =
				Resources.LoadShader<PaletteShader>("Shaders/palette");
			SHADER_SINE_SHIFT = Resources.LoadShader("Shaders/sine_shift");*/

			//SHADER_PALETTE = new PaletteShader(SHADER_PALETTE_LERP);
			if (SHADER_PALETTE != null) {
				SHADER_PALETTE.TilePalette = PAL_TILES_DEFAULT;
				SHADER_PALETTE.EntityPalette = PAL_ENTITIES_DEFAULT;
			}

			GameSettings.DRAW_MODE_PALLETE.Effect = SHADER_PALETTE;
		}
		
		//public static Effect SHADER_PALETTE_LERP;

		public static PaletteShader SHADER_PALETTE;

		public static SineShiftShader SHADER_SINE_SHIFT;


		//-----------------------------------------------------------------------------
		// Collision Models
		//-----------------------------------------------------------------------------

		public static CollisionModel MODEL_EMPTY;
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

		public static CollisionModel MODEL_LEAP_LEDGE_RIGHT;
		public static CollisionModel MODEL_LEAP_LEDGE_UP;
		public static CollisionModel MODEL_LEAP_LEDGE_LEFT;
		public static CollisionModel MODEL_LEAP_LEDGE_DOWN;
		public static CollisionModel[] MODEL_LEAP_LEDGES;


		//-----------------------------------------------------------------------------
		// Zones
		//-----------------------------------------------------------------------------

		public static Zone ZONE_DEFAULT;


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
		public static Palette PAL_TILES_ELECTROCUTED;

		public static Palette PAL_DUNGEON_MAP_DEFAULT;
		public static Palette PAL_MENU_DEFAULT;

		public static string[] MONSTER_COLOR_DEFINITION_MAP;


		//-----------------------------------------------------------------------------
		// Render Targets
		//-----------------------------------------------------------------------------

		public static RenderTarget RenderTargetGame;
		public static RenderTarget RenderTargetGameTemp;
		public static RenderTarget RenderTargetDebug;
	}
}
