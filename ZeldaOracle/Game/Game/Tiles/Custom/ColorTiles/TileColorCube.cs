using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaAPI;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Entities.Projectiles;

namespace ZeldaOracle.Game.Tiles {

	/// <summary>The color cube's sprite index has the value of one of these orientations.</summary>
	public enum ColorCubeOrientation {
		/// <summary>Blue on top with yellow on the side.</summary>
		BlueYellow = 0,
		/// <summary>Blue on top with red on the side.</summary>
		BlueRed,
		/// <summary>Yellow on top with red on the side.</summary>
		YellowRed,
		/// <summary>Yellow on top with blue on the side.</summary>
		YellowBlue,
		/// <summary>Red on top with blue on the side.</summary>
		RedBlue,
		/// <summary>Red on top with yellow on the side.</summary>
		RedYellow
	}

	public class TileColorCube : Tile {

		private Point2I offset;
		private ColorCubeOrientation orientation;

		private const float MOVEMENT_SPEED = 16f / 12f;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public TileColorCube() {
			soundMove = GameData.SOUND_SWITCH;
			
			Graphics.SyncPlaybackWithRoomTicks = false;
		}

		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------
		
		public override bool OnPush(int direction, float movementSpeed) {
			if (base.OnPush(direction, MOVEMENT_SPEED)) {
				offset = Directions.ToPoint(direction);
				
				ColorCubeOrientation oldOrientation = orientation;

				// Find the new sprite index.
				if (Directions.IsVertical(direction))
					orientation = (ColorCubeOrientation) GMath.Wrap((int) orientation + 3, 6);
				else if ((int) orientation % 2 == 0)
					orientation = (ColorCubeOrientation) GMath.Wrap((int) orientation - 1, 6);
				else
					orientation = (ColorCubeOrientation) GMath.Wrap((int) orientation + 1, 6);

				// Play the corresponding animation.
				Graphics.PlayAnimation(GameData.ANIM_COLOR_CUBE_ROLLING_ORIENTATIONS[(int) oldOrientation, direction]);

				// Set an absolute draw position because the animation should not move with the tile.
				Graphics.SetAbsoluteDrawPosition(Position);

				return true;
			}
			return false;
		}

		public override void OnInitialize() {
			base.OnInitialize();

			int orientationIndex = Properties.GetInteger("orientation", 0);
			orientation = (ColorCubeOrientation) orientationIndex;
			Graphics.PlaySpriteAnimation(SpriteList[orientationIndex]);
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------
		
		public ColorCubeOrientation ColorOrientation {
			get { return orientation; }
		}

		public PuzzleColor TopColor {
			get {
				ColorCubeOrientation orientation = ColorOrientation;
				if (orientation == ColorCubeOrientation.BlueRed || orientation == ColorCubeOrientation.BlueYellow)
					return PuzzleColor.Blue;
				if (orientation == ColorCubeOrientation.RedBlue || orientation == ColorCubeOrientation.RedYellow)
					return PuzzleColor.Red;
				return PuzzleColor.Yellow;
			}
		}
	}
}
