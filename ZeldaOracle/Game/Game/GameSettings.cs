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
	class GameSettings {

		public const int		TILE_SIZE		= 16;	// Tile size in texels.
		public const int		SCREEN_WIDTH	= 160;
		public const int		SCREEN_HEIGHT	= 144;
		public const int		VIEW_WIDTH		= 160;
		public const int		VIEW_HEIGHT		= 128;
		public static readonly Point2I	SCREEN_SIZE		= new Point2I(SCREEN_WIDTH, SCREEN_HEIGHT);
		public static readonly Point2I	VIEW_SIZE		= new Point2I(VIEW_WIDTH, VIEW_HEIGHT);
		public static readonly Rectangle2I	SCREEN_BOUNDS	= new Rectangle2I(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT);

		public const int		VIEW_PAN_SPEED	= 1;

		public const float		PLAYER_MOVE_SPEED		= 1;		// Pixels per second.
		public const float		PLAYER_JUMP_SPEED		= 1.8f;
		public const float		DEFAULT_GRAVITY			= 0.125f;	// Default gravity acceleration in pixels per frame^2
		public const float		DEFAULT_MAX_FALL_SPEED	= 4.0f;

		public static readonly Point2I	ROOM_SIZE_SMALL	= new Point2I(10, 8);
		public static readonly Point2I	ROOM_SIZE_LARGE	= new Point2I(15, 11);

		public const int		DEFAULT_TILE_LAYER_COUNT = 3;

		public const string TEXT_UNDEFINED = "<red>undefined<red>";


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
