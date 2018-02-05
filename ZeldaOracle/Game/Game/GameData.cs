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
			MODEL_LEAP_LEDGES = new CollisionModel[Directions.Count] {
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
		/*
		private static ActionTileData CreateMonsterAction(int sx, int sy, string id, string animation, Type monsterType, MonsterColor color) {
			ActionTileData etd = new ActionTileData(typeof(MonsterAction));
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

			Resources.AddResource(id, etd);
			return etd;
		}*/


		//-----------------------------------------------------------------------------
		// Font Loading
		//-----------------------------------------------------------------------------

		// Loads the fonts.
		private static void LoadFonts() {
			Resources.LoadGameFonts("Fonts/fonts.conscript");

			IntegrateResources<GameFont>("FONT_");

			//FONT_LARGE = Resources.GetGameFont("Fonts/font_large");
			//FONT_SMALL = Resources.GetGameFont("Fonts/font_small");
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
			Palette entitiesMenu = new Palette(Resources.GetResource<Palette>("entities_default"));
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
			Resources.AddResource<Palette>("entities_menu", entitiesMenu);

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
		// Collision Models
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

		public static CollisionModel MODEL_LEAP_LEDGE_RIGHT;
		public static CollisionModel MODEL_LEAP_LEDGE_UP;
		public static CollisionModel MODEL_LEAP_LEDGE_LEFT;
		public static CollisionModel MODEL_LEAP_LEDGE_DOWN;
		public static CollisionModel[] MODEL_LEAP_LEDGES;


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
		public static Palette PAL_TILES_ELECTROCUTED;

		public static Palette PAL_DUNGEON_MAP_DEFAULT;
		public static Palette PAL_MENU_DEFAULT;

		public static string[] MONSTER_COLOR_DEFINITION_MAP;


		//-----------------------------------------------------------------------------
		// Render Targets
		//-----------------------------------------------------------------------------

		public static PaletteShader PaletteShader;

		public static RenderTarget2D RenderTargetGame;
		public static RenderTarget2D RenderTargetGameTemp;
		public static RenderTarget2D RenderTargetDebug;
	}
}
