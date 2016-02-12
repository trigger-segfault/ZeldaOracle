using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Entities.Projectiles;

namespace ZeldaOracle.Game.Tiles {

	// The color cube's sprite index has the value of one of these orientations.
	public enum ColorCubeOrientation {
		BlueYellow = 0,
		BlueRed,
		YellowRed,
		YellowBlue,
		RedBlue,
		RedYellow
	}

	public enum PuzzleColor {
		None = -1,
		Red,
		Yellow,
		Blue,
		Count,
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
			Graphics.PlaySprite(SpriteList[orientationIndex]);
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
