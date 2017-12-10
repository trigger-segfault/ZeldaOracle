using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;

namespace ZeldaOracle.Game {

	public class GameSettings {
		
		// World
		public const int					TILE_SIZE					= 16;	// Tile size in texels.
		public static readonly Point2I		ROOM_SIZE_SMALL				= new Point2I(10, 8);
		public static readonly Point2I		ROOM_SIZE_LARGE				= new Point2I(15, 11);
		public const int					DEFAULT_TILE_LAYER_COUNT	= 3;

		// Display
		public const int					SCREEN_WIDTH			= 160;
		public const int					SCREEN_HEIGHT			= 144;
		public const int					VIEW_WIDTH				= 160;
		public const int					VIEW_HEIGHT				= 128;
		public static readonly Point2I		SCREEN_SIZE				= new Point2I(SCREEN_WIDTH, SCREEN_HEIGHT);
		public static readonly Point2I		VIEW_SIZE				= new Point2I(VIEW_WIDTH, VIEW_HEIGHT);
		public static readonly Rectangle2I	SCREEN_BOUNDS			= new Rectangle2I(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT);
		public const int					VIEW_PAN_SPEED			= 1;

		// Properties
		public const string				TEXT_UNDEFINED				= "<red>undefined<red>";

		// Physics
		public const float				DEFAULT_GRAVITY				= 0.125f;	// Default gravity acceleration in pixels per frame^2
		public const float				DEFAULT_MAX_FALL_SPEED		= 4.0f;
		
		// Drops & Collectibles
		public const float				DROP_ENTITY_SPAWN_ZVELOCITY				= 1.5f;
		public const float				DROP_ENTITY_DIG_VELOCITY				= 0.75f;
		public const int				COLLECTIBLE_ALIVE_DURATION				= 513;
		public const int				COLLECTIBLE_FADE_DELAY					= 400;
		public const int				COLLECTIBLE_PICKUPABLE_DELAY			= 12;
		public const int				COLLECTIBLE_DIG_PICKUPABLE_DELAY		= 16;
		public const int				COLLECTIBLE_FAIRY_ALIVE_DURATION		= 513;
		public const int				COLLECTIBLE_FAIRY_FADE_DELAY			= 400;
		public const int				COLLECTIBLE_FAIRY_HOVER_HEIGHT			= 8;

		// Projectiles and Items
		public static readonly float[]	BRACELET_PUSH_SPEEDS					= { 0.5f, 1.0f };
		public const float				PROJECTILE_ARROW_SPEED					= 3.0f;
		public const int				PROJECTILE_ARROW_DAMAGE					= 1;
		public const float				PROJECTILE_SWORD_BEAM_SPEED				= 3.0f;
		public const float				PROJECTILE_MAGIC_ROD_FIRE_SPEED			= 2.0f;
		public static readonly float[]	PROJECTILE_BOOMERANG_SPEEDS				= { 1.5f, 3.0f };
		public static readonly int[]	PROJECTILE_BOOMERANG_RETURN_DELAYS		= { 41, 100 };
		public static readonly float[]	PROJECTILE_SWITCH_HOOK_SPEEDS			= { 2.0f, 3.0f };
		public static readonly int[]	PROJECTILE_SWITCH_HOOK_LENGTHS			= { 82, 112 };
		public const int				SWITCH_HOOK_LATCH_DURATION				= 20;
		public const float				SWITCH_HOOK_LIFT_SPEED					= 1.0f;
		public const int				SWITCH_HOOK_LIFT_HEIGHT					= 16;
		public const float				SLINGSHOT_SEED_SPEED					= 3.0f;
		public const float				SLINGSHOT_SEED_DEGREE_OFFSET			= 20.0f; // For the 2 extra seed from the hyper slingshot.
		public const float				SEED_SHOOTER_SHOOT_SPEED				= 3.0f;
		public const int				SEED_PROJECTILE_REBOUND_COUNT			= 3;
		public const int				SCENT_POD_DURATION						= 240;
		public const int				SCENT_POD_FADE_DELAY					= 60;
		public const int				BOMB_FUSE_TIME							= 108;
		public const int				BOMB_FLICKER_DELAY						= 72;

		// Units
		public const float				UNIT_KNOCKBACK_SPEED			= 1.0f; // 1.3 ??
		public const int				UNIT_KNOCKBACK_DURATION			= 16;
		public const int				UNIT_HURT_INVINCIBLE_DURATION	= 32;
		public const int				UNIT_HURT_FLICKER_DURATION		= 32;
		public const int				UNIT_KNOCKBACK_ANGLE_SNAP_COUNT	= 16;
		
		public const float				MONSTER_KNOCKBACK_SPEED				= 2.0f; // 1.3 ??
		public const int				MONSTER_HURT_KNOCKBACK_DURATION		= 11;
		public const int				MONSTER_BUMP_KNOCKBACK_DURATION		= 8;

		public const int				MONSTER_HURT_INVINCIBLE_DURATION	= 16;
		public const int				MONSTER_HURT_FLICKER_DURATION		= 16;
		public const int				MONSTER_BURN_DURATION				= 59;
		
		public const float				PLAYER_KNOCKBACK_SPEED				= 1.25f; // 1.3 ??
		public const int				PLAYER_HURT_KNOCKBACK_DURATION		= 15;
		public const int				PLAYER_BUMP_KNOCKBACK_DURATION		= 11;
		//public const int				MONSTER_HURT_INVINCIBLE_DURATION	= 16;
		//public const int				MONSTER_HURT_FLICKER_DURATION		= 16;
		//public const int				MONSTER_BURN_DURATION				= 59;
		
		//public const int				InvincibleDuration					= 25;
		//public const int				InvincibleControlRestoreDuration	= 8;
		//public const int				KnockbackSnapCount					= 16;
		//public const float			KnockbackSpeed						= 1.3f;


		// Player
		public const float				PLAYER_MOVE_SPEED					= 1.0f;		// Pixels per second.
		public const float				PLAYER_JUMP_SPEED					= 2.0f;
		public const float				PLAYER_CAPE_JUMP_SPEED				= 0.5f;
		public const float				PLAYER_SIDESCROLL_JUMP_SPEED		= 2.25f;
		public const float				PLAYER_SIDESCROLL_CAPE_JUMP_SPEED	= 0.5f;
		public const float				PLAYER_CAPE_REQUIRED_FALLSPEED		= 1.0f;		// Player must be falling this fast to be able to deploy cape.
		public const float				PLAYER_CAPE_GRAVITY					= 0.04f;	// 0.04 = 1/25
		public const int				PLAYER_SPRINT_DURATION				= 480;
		public const float				PLAYER_SPRINT_SPEED_SCALE			= 1.5f;
		public const int				PLAYER_SPRINT_EFFECT_INTERVAL		= 10;
		public const float				PLAYER_DEFAULT_PUSH_SPEED			= 0.5f;

		// Monsters
		public const int				MONSTER_STUN_DURATION					= 400;	// How long a monster gets stunned for (by boomerang/pegasus seeds).
		public const int				MONSTER_STUN_SHAKE_DURATION				= 60;	// How long the monster shakes at the end of being stunned.
		public const int				MONSTER_BOOMERANG_DAMAGE				= 1;
		public const int				MONSTER_THWOMP_CRUSH_MIN_ALIGNMENT		= 5; // TODO
		public const float				MONSTER_THWOMP_CRUSH_ACCELERATION		= 0.2f;
		public const float				MONSTER_THWOMP_CRUSH_INITIAL_SPEED		= 0.5f;
		public const float				MONSTER_THWOMP_CRUSH_MAX_SPEED			= 2.5f;
		public const float				MONSTER_THWOMP_RAISE_SPEED				= 0.5f;
		public const int				MONSTER_THWOMP_HIT_SHAKE_DURATION		= 46;
		public const int				MONSTER_THWOMP_HIT_WAIT_DURATION		= 46 + 16;

		// Tiles
		public const int				TILE_BUTTON_TILE_RAISE_AMOUNT			= 2;	// Pixels to raise certain tiles when pushed over a button.
		public const int				TILE_BUTTON_UNCOVER_RELEASE_DELAY		= 27;	// Delay between being uncovered and becoming released.
		public static readonly Rectangle2F	TILE_BUTTON_PLAYER_PRESS_AREA		= new Rectangle2F(0, 5, 16, 16);
		public const int				TILE_CRACKED_FLOOR_CRUMBLE_DELAY		= 30;
		public const float				TILE_ROLLER_MOVE_SPEED					= 0.5f;
		public const float				TILE_PULL_HANDLE_EXTEND_SPEED			= 0.25f;
		public const float				TILE_PULL_HANDLE_RETRACT_SPEED			= 0.25f;
		public const int				TILE_PULL_HANDLE_PLAYER_PULL_DURATION	= 40;
		public const int				TILE_PULL_HANDLE_PLAYER_PAUSE_DURATION	= 20;
		public const int				TILE_PULL_HANDLE_EXTEND_LENGTH			= 64;
		public const int				TILE_PULL_HANDLE_WALL_OFFSET			= 8;



		//-----------------------------------------------------------------------------
		// Draw modes
		//-----------------------------------------------------------------------------

		public static DrawMode DRAW_MODE_DEFAULT = new DrawMode() {
			BlendState		= BlendState.AlphaBlend,
			SortMode		= SpriteSortMode.Deferred,
			SamplerState	= SamplerState.PointClamp
		};

		public static DrawMode DRAW_MODE_BACK_TO_FRONT = new DrawMode() {
			BlendState		= BlendState.AlphaBlend,
			SortMode		= SpriteSortMode.FrontToBack, // Use FrontToBack so our depth values mean 0 is below and 1 is above.
			SamplerState	= SamplerState.PointClamp
		};

		public static DrawMode DRAW_MODE_ROOM_GRAPHICS = new DrawMode() {
			BlendState		= BlendState.AlphaBlend,
			SortMode		= SpriteSortMode.Deferred,
			SamplerState	= SamplerState.PointClamp
		};
	}
}
