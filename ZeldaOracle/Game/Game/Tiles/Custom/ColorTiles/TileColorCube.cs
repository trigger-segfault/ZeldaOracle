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
		
		private const float MOVEMENT_SPEED = 16f / 12f;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public TileColorCube() {
			animationPlayer = new AnimationPlayer();
			soundMove = GameData.SOUND_SWITCH;
		}

		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------
		
		public override bool OnPush(int direction, float movementSpeed) {
			if (base.OnPush(direction, MOVEMENT_SPEED)) {
				offset = Directions.ToPoint(direction);
				
				int spriteIndex = SpriteIndex;
				int oldSpriteIndex = spriteIndex;

				// Find the new sprite index.
				if (Directions.IsVertical(direction))
					SpriteIndex = GMath.Wrap(spriteIndex + 3, 6);
				else if (spriteIndex % 2 == 0)
					SpriteIndex = GMath.Wrap(spriteIndex - 1, 6);
				else
					SpriteIndex = GMath.Wrap(spriteIndex + 1, 6);

				// Play the corresponding animation.
				animationPlayer.Play(GameData.ANIM_COLOR_CUBE_ROLLING_ORIENTATIONS[oldSpriteIndex, direction]);

				return true;
			}
			return false;
		}

		public override void OnInitialize() {
			
		}

		public override void Update() {
			base.Update();

			if (IsMoving) {
				animationPlayer.Update();
			}
		}

		public override void Draw(RoomGraphics g) {

			SpriteAnimation sprite = (!CustomSprite.IsNull ? CustomSprite : CurrentSprite);
			if (IsMoving) {
				g.DrawAnimation(animationPlayer, (Location - offset) * GameSettings.TILE_SIZE, DepthLayer.TileLayer1);
			}
			else if (sprite.IsAnimation) {
				// Draw as an animation.
				g.DrawAnimation(sprite.Animation, Zone.ImageVariantID,
					RoomControl.GameControl.RoomTicks, Position, DepthLayer.TileLayer1);
			}
			else if (sprite.IsSprite) {
				// Draw as a sprite.
				g.DrawSprite(sprite.Sprite, Zone.ImageVariantID, Position, DepthLayer.TileLayer1);
			}
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------
		
		public ColorCubeOrientation ColorOrientation {
			get { return (ColorCubeOrientation) SpriteIndex; }
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
