using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;


/*
 *	GAME TODO LIST:
 *		- Work out depth for things (Player submerge should be behind all things)
 *		- Work out position/origin/center confusions.
 *		- Player:
 *			- 
 *		- Graphics:
 *			- Drawing sprites/animations with different sprite sheets (player hurt, different zones, menu light/dark)
 *		- Resources:
 *			- Define animations in a file
 *		- Properties
 *		
 *	DEBUG KEYS:
 *		- 1: Speed up the game.
 *		- G: Read text.
 *		- V: (When reading text) skip to next line.
 *		- Z: Fire an arrow.
*/

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
		
		// Units
		public const float				UNIT_KNOCKBACK_SPEED			= 1.0f; // 1.3 ??
		public const int				UNIT_KNOCKBACK_DURATION			= 16;
		public const int				UNIT_HURT_INVINCIBLE_DURATION	= 16;
		public const int				UNIT_HURT_FLICKER_DURATION		= 32;
		public const int				UNIT_KNOCKBACK_ANGLE_SNAP_COUNT	= 16;
		
		//public const int				InvincibleDuration					= 25;
		//public const int				InvincibleControlRestoreDuration	= 8;
		//public const int				KnockbackSnapCount					= 16;
		//public const float			KnockbackSpeed						= 1.3f;


		// Player
		public const float				PLAYER_MOVE_SPEED			= 1.0f;		// Pixels per second.
		public const float				PLAYER_JUMP_SPEED			= 1.8f;
		public const int				PLAYER_SPRINT_DURATION		= 480;
		public const float				PLAYER_SPRINT_SPEED_SCALE	= 1.5f;

		// Monsters
		public const int				MONSTER_STUN_DURATION		= 400;	// How long a monster gets stunned for (by boomerang/pegasus seeds).
		public const int				MONSTER_STUN_SHAKE_DURATION	= 60;	// How long the monster shakes while stunned.



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
			SortMode		= SpriteSortMode.BackToFront,
			SamplerState	= SamplerState.PointClamp
		};
	}
}
