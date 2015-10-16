using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Entities.Projectiles;

namespace ZeldaOracle.Game.Tiles {

	public enum ColorCubeOrientations {
		BlueYellow,
		BlueRed,
		YellowRed,
		YellowBlue,
		RedBlue,
		RedYellow
	}

	public class TileColorCube : Tile {

		private AnimationPlayer animationPlayer;

		private Point2I offset;

		private const float MOVEMENT_SPEED = 16f / 12f;
		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public TileColorCube() {
			animationPlayer = new AnimationPlayer();
		}

		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------
		
		public override bool OnPush(int direction, float movementSpeed) {
			if (base.OnPush(direction, MOVEMENT_SPEED)) {
				offset = Directions.ToPoint(direction);
				bool vertical = (direction % 2 == 1);
				int spriteIndex = Properties.GetInteger("sprite_index");
				int oldSpriteIndex = SpriteIndex;
				if (direction % 2 == 1)
					spriteIndex = GMath.Wrap(spriteIndex + 3, 6);
				else if (spriteIndex % 2 == 0)
					spriteIndex = GMath.Wrap(spriteIndex - 1, 6);
				else
					spriteIndex = GMath.Wrap(spriteIndex + 1, 6);
				Properties.Set("sprite_index", spriteIndex);

				animationPlayer.Play(GameData.ANIM_COLOR_CUBE_ROLLING_ORIENTATIONS[oldSpriteIndex, direction]);

				return true;
			}
			return false;
		}

		public override void Update() {
			base.Update();
			if (IsMoving) {
				animationPlayer.Update();
			}
		}

		public override void Initialize() {
			
		}

		public override void Draw(Graphics2D g) {

			SpriteAnimation sprite = (!CustomSprite.IsNull ? CustomSprite : CurrentSprite);
			if (IsMoving) {
				g.DrawAnimation(animationPlayer, (Location - offset) * GameSettings.TILE_SIZE);
			}
			else if (sprite.IsAnimation) {
				// Draw as an animation.
				g.DrawAnimation(sprite.Animation, Zone.ImageVariantID,
					RoomControl.GameControl.RoomTicks, Position);
			}
			else if (sprite.IsSprite) {
				// Draw as a sprite.
				g.DrawSprite(sprite.Sprite, Zone.ImageVariantID, Position);
			}
		}
	}
}
