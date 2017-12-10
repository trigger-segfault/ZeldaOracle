using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Entities.Projectiles;
using ZeldaOracle.Game.Entities.Projectiles.Seeds;


namespace ZeldaOracle.Game {
	public partial class GameData {
		
		//-----------------------------------------------------------------------------
		// Animations
		//-----------------------------------------------------------------------------

		// UI animations.
		public static Animation ANIM_UI_MAP_CURSOR;

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
		public static Animation ANIM_TILE_SPIKED_FLOOR;
		public static Animation ANIM_TILE_LAVAFALL;
		public static Animation ANIM_TILE_LAVAFALL_BOTTOM;
		public static Animation ANIM_TILE_LAVAFALL_TOP;
		public static Animation ANIM_TILE_WALL_TORCH_RIGHT;
		public static Animation ANIM_TILE_WALL_TORCH_UP;
		public static Animation ANIM_TILE_WALL_TORCH_LEFT;
		public static Animation ANIM_TILE_WALL_TORCH_DOWN;
		public static Animation ANIM_TILE_DOOR_OPEN;
		public static Animation ANIM_TILE_DOOR_CLOSE;
		public static Animation ANIM_TILE_MINECART_DOOR_OPEN;
		public static Animation ANIM_TILE_MINECART_DOOR_CLOSE;
		public static Animation ANIM_TILE_SMALL_KEY_DOOR_OPEN;
		public static Animation ANIM_TILE_SMALL_KEY_DOOR_CLOSE;
		public static Animation ANIM_TILE_BOSS_KEY_DOOR_OPEN;
		public static Animation ANIM_TILE_BOSS_KEY_DOOR_CLOSE;
		public static Animation[,] ANIM_COLOR_CUBE_ROLLING_ORIENTATIONS;
		public static Animation ANIM_TILE_REGROWABLE_PLANT_REGROW;

		// General tiles.
		public static Animation ANIM_MINECART;
		public static Animation ANIM_TURNSTILE_ARROWS_CLOCKWISE;
		public static Animation ANIM_TURNSTILE_ARROWS_COUNTERCLOCKWISE;
		public static Animation ANIM_TURNSTILE_ROTATE_CLOCKWISE;
		public static Animation ANIM_TURNSTILE_ROTATE_COUNTERCLOCKWISE;
		public static Animation ANIM_TILE_CROSSING_GATE_RAISE;
		public static Animation ANIM_TILE_CROSSING_GATE_LOWER;
		public static Animation ANIM_TILE_SEED_BOUNCER;
		public static Animation ANIM_TILE_COLOR_BARRIER_BLUE_RAISE;
		public static Animation ANIM_TILE_COLOR_BARRIER_BLUE_LOWER;
		public static Animation ANIM_TILE_COLOR_BARRIER_RED_RAISE;
		public static Animation ANIM_TILE_COLOR_BARRIER_RED_LOWER;
	
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
		public static Animation ANIM_PLAYER_SWING_NOLUNGE;
		public static Animation ANIM_PLAYER_SWING_BIG;
		public static Animation ANIM_PLAYER_STAB;
		public static Animation ANIM_PLAYER_SPIN;
		public static Animation ANIM_PLAYER_AIM;
		public static Animation ANIM_PLAYER_JUMP;
		public static Animation ANIM_PLAYER_CAPE;
		public static Animation ANIM_PLAYER_SUBMERGED;
		public static Animation ANIM_PLAYER_DIE;
		public static Animation ANIM_PLAYER_RAISE_ONE_HAND;
		public static Animation ANIM_PLAYER_RAISE_TWO_HANDS;
		public static Animation ANIM_PLAYER_DROWN;
		public static Animation ANIM_PLAYER_FALL;
		public static Animation ANIM_PLAYER_CRUSH_HORIZONTAL;
		public static Animation ANIM_PLAYER_CRUSH_VERTICAL;
		public static Animation ANIM_PLAYER_INVISIBLE;
		public static Animation ANIM_PLAYER_MINECART_IDLE;
		public static Animation ANIM_PLAYER_MINECART_CARRY;
		public static Animation ANIM_PLAYER_MINECART_THROW;
		public static Animation ANIM_PLAYER_MINECART_AIM;
		public static Animation ANIM_PLAYER_MINECART_SWING;
		public static Animation ANIM_PLAYER_MINECART_SWING_NOLUNGE;
		public static Animation ANIM_PLAYER_MINECART_SWING_BIG;
		public static Animation ANIM_PLAYER_MERMAID_SWIM;
		public static Animation ANIM_PLAYER_MERMAID_AIM;
		public static Animation ANIM_PLAYER_MERMAID_SWING;
		public static Animation ANIM_PLAYER_MERMAID_SPIN;
		public static Animation ANIM_PLAYER_MERMAID_STAB;
		public static Animation ANIM_PLAYER_MERMAID_THROW;

		// Monster animations.
		public static Animation ANIM_MONSTER_OCTOROK;
		public static Animation ANIM_MONSTER_MOBLIN;
		public static Animation ANIM_MONSTER_DARKNUT;
		public static Animation ANIM_MONSTER_PIG_MOBLIN;
		public static Animation ANIM_MONSTER_SHROUDED_STALFOS;
		public static Animation ANIM_MONSTER_ARM_MIMIC;
		public static Animation ANIM_MONSTER_BEAMOS;
		public static Animation ANIM_MONSTER_PINCER_HEAD;
		public static Animation ANIM_MONSTER_MINI_MOLDORM_HEAD;
		public static Animation ANIM_MONSTER_IRON_MASK;
		public static Animation ANIM_MONSTER_IRON_MASK_NAKED;
		public static Animation ANIM_MONSTER_IRON_MASK_MASK;
		public static Animation ANIM_MONSTER_BUZZ_BLOB;
		public static Animation ANIM_MONSTER_BUZZ_BLOB_ELECTROCUTE;
		public static Animation ANIM_MONSTER_CUKEMAN;
		public static Animation ANIM_MONSTER_LYNEL;
		public static Animation ANIM_MONSTER_ROPE;
		public static Animation ANIM_MONSTER_CROW;
		public static Animation ANIM_MONSTER_FLOOR_MASTER;
		public static Animation ANIM_MONSTER_FLOOR_MASTER_GRAB;
		public static Animation ANIM_MONSTER_WALL_MASTER;
		public static Animation ANIM_MONSTER_WALL_MASTER_GRAB;
		public static Animation ANIM_MONSTER_BIRI;
		public static Animation ANIM_MONSTER_BARI;
		public static Animation ANIM_MONSTER_BARI_ELECTROCUTE;
		public static Animation ANIM_MONSTER_GIBDO;
		public static Animation ANIM_MONSTER_SAND_CRAB;
		public static Animation ANIM_MONSTER_WATER_TEKTIKE;
		public static Animation ANIM_MONSTER_GOPONGA_FLOWER;
		public static Animation ANIM_MONSTER_PEAHAT;
		public static Animation ANIM_MONSTER_GEL;
		public static Animation ANIM_MONSTER_ZOL;
		public static Animation ANIM_MONSTER_ZOL_JUMP;
		public static Animation ANIM_MONSTER_ZOL_BURROW;
		public static Animation ANIM_MONSTER_ZOL_UNBURROW;
		public static Animation ANIM_MONSTER_COLOR_GEL_BODY;
		public static Animation ANIM_MONSTER_COLOR_GEL_HIGHLIGHT;
		public static Animation ANIM_MONSTER_COLOR_GEL_EYES;
		public static Animation ANIM_MONSTER_BUBBLE;
		public static Animation ANIM_MONSTER_STALFOS;
		public static Animation ANIM_MONSTER_STALFOS_JUMP;
		public static Animation ANIM_MONSTER_SPINNING_BLADE_TRAP_SLEEP;
		public static Animation ANIM_MONSTER_SPINNING_BLADE_TRAP;
		public static Animation ANIM_MONSTER_LEEVER;
		public static Animation ANIM_MONSTER_LEEVER_BURROW;
		public static Animation ANIM_MONSTER_LEEVER_UNBURROW;
		public static Animation ANIM_MONSTER_BALL_AND_CHAIN_TROOPER;
		public static Animation ANIM_MONSTER_CANDLE_HEAD;
		public static Animation ANIM_MONSTER_CANDLE_HEAD_SLEEP;
		public static Animation ANIM_MONSTER_BLADE_TRAP;
		public static Animation ANIM_MONSTER_GIANT_BLADE_TRAP;
		public static Animation ANIM_MONSTER_SPIKED_BEETLE;
		public static Animation ANIM_MONSTER_SPIKED_BEETLE_FLIPPED;
		public static Animation ANIM_MONSTER_RIVER_ZORA;
		public static Animation ANIM_MONSTER_RIVER_ZORA_SHOOT;
		public static Animation ANIM_MONSTER_RIVER_ZORA_WATER_SWIRLS;
		public static Animation ANIM_MONSTER_HARDHAT_BEETLE;
		public static Animation ANIM_MONSTER_SPINY_BEETLE;
		public static Animation ANIM_MONSTER_BEETLE;
		public static Animation ANIM_MONSTER_WIZZROBE;
		public static Animation ANIM_MONSTER_WIZZROBE_HAT;
		public static Animation ANIM_MONSTER_LIKE_LIKE;
		public static Animation ANIM_MONSTER_LIKE_LIKE_DEVOUR;
		public static Animation ANIM_MONSTER_ARMOS;
		public static Animation ANIM_MONSTER_TEKTIKE;
		public static Animation ANIM_MONSTER_TEKTIKE_JUMP;
		public static Animation ANIM_MONSTER_POLS_VOICE;
		public static Animation ANIM_MONSTER_POLS_VOICE_JUMP;
		public static Animation ANIM_MONSTER_GHINI;
		public static Animation ANIM_MONSTER_THWOMP;
		public static Animation ANIM_MONSTER_THWOMP_CRUSH;
		public static Animation ANIM_MONSTER_THWOMP_EYE;
		public static Animation ANIM_MONSTER_THWIMP;
		public static Animation ANIM_MONSTER_THWIMP_CRUSH;

		// Monster items.
		public static Animation ANIM_MONSTER_SWORD_HOLD;

		// Colllectible animations.
		public static Animation ANIM_COLLECTIBLE_FAIRY;

		// Player weapon animations.
		public static Animation ANIM_SWORD_HOLD;
		public static Animation ANIM_SWORD_CHARGED;
		public static Animation ANIM_SWORD_SWING;
		public static Animation ANIM_SWORD_MINECART_SWING;
		public static Animation ANIM_SWORD_SPIN;
		public static Animation ANIM_SWORD_STAB;
		public static Animation ANIM_BIG_SWORD_SWING;
		public static Animation ANIM_CANE_SWING;
		public static Animation ANIM_CANE_MINECART_SWING;
		public static Animation ANIM_MAGIC_ROD_SWING;
		public static Animation ANIM_MAGIC_ROD_MINECART_SWING;
		public static Animation ANIM_SEED_SHOOTER;
		public static Animation ANIM_SLINGSHOT_1;
		public static Animation ANIM_SLINGSHOT_2;

		// Projectile animations.
		public static Animation ANIM_ITEM_BOMB;
		public static Animation ANIM_PROJECTILE_PLAYER_ARROW;
		public static Animation ANIM_PROJECTILE_PLAYER_ARROW_CRASH;
		public static Animation ANIM_PROJECTILE_SWORD_BEAM;
		public static Animation ANIM_PROJECTILE_PLAYER_BOOMERANG_1;
		public static Animation ANIM_PROJECTILE_PLAYER_BOOMERANG_2;
		public static Animation ANIM_PROJECTILE_SWITCH_HOOK;
		public static Animation ANIM_PROJECTILE_MAGIC_ROD_FIRE;
		
		// Monster projectile animations.
		public static Animation ANIM_PROJECTILE_MONSTER_ARROW;
		public static Animation ANIM_PROJECTILE_MONSTER_ARROW_CRASH;
		public static Animation ANIM_PROJECTILE_MONSTER_SPEAR;
		public static Animation ANIM_PROJECTILE_MONSTER_BOOMERANG;
		public static Animation ANIM_PROJECTILE_MONSTER_MAGIC;
		public static Animation ANIM_PROJECTILE_MONSTER_FIREBALL;
		public static Animation ANIM_PROJECTILE_MONSTER_BONE;
		public static Animation ANIM_PROJECTILE_MONSTER_ROCK;
	
		// Effect animations.
		public static Animation ANIM_EFFECT_DIRT;
		public static Animation ANIM_EFFECT_WATER_SPLASH;
		public static Animation ANIM_EFFECT_RIPPLES;
		public static Animation ANIM_EFFECT_GRASS;
		public static Animation ANIM_EFFECT_ROCK_BREAK;
		public static Animation ANIM_EFFECT_SIGN_BREAK;
		public static Animation ANIM_EFFECT_LEAVES;
		public static Animation ANIM_EFFECT_GRASS_LEAVES;

		// Color effect animations.
		public static Animation ANIM_EFFECT_LAVA_SPLASH;
		public static Animation ANIM_EFFECT_BOMB_EXPLOSION;
		public static Animation ANIM_EFFECT_MONSTER_EXPLOSION;
		public static Animation ANIM_EFFECT_SEED_EMBER;
		public static Animation ANIM_EFFECT_SEED_SCENT;
		public static Animation ANIM_EFFECT_SEED_PEGASUS;
		public static Animation ANIM_EFFECT_SEED_GALE;
		public static Animation ANIM_EFFECT_SEED_MYSTERY;
		public static Animation ANIM_EFFECT_PEGASUS_DUST;		// The dust the player sprinkles over himself when using a pegasus seed.
		public static Animation ANIM_EFFECT_SPRINT_PUFF;
		public static Animation ANIM_EFFECT_OWL_SPARKLE;
		public static Animation ANIM_ITEM_SCENT_POD;
		public static Animation ANIM_EFFECT_FALLING_OBJECT;
		public static Animation ANIM_EFFECT_CLING;
		public static Animation ANIM_EFFECT_CLING_LIGHT;
		public static Animation ANIM_EFFECT_BURN;
		public static Animation ANIM_EFFECT_SOMARIA_BLOCK_CREATE;
		public static Animation ANIM_EFFECT_SOMARIA_BLOCK_DESTROY;
		public static Animation ANIM_EFFECT_BLOCK_POOF;
		public static Animation ANIM_EFFECT_COLOR_FLAME_RED;
		public static Animation ANIM_EFFECT_COLOR_FLAME_BLUE;
		public static Animation ANIM_EFFECT_COLOR_FLAME_GREEN;
		public static Animation ANIM_EFFECT_COLOR_FLAME_YELLOW;
		public static Animation[] ANIM_EFFECT_SEEDS;

		
		//-----------------------------------------------------------------------------
		// Animations Loading
		//-----------------------------------------------------------------------------

		public static void LoadAnimations() {
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

	}
}
