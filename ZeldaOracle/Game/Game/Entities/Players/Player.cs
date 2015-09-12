using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Input;
using ZeldaOracle.Game.Main;

namespace ZeldaOracle.Game.Entities.Players {
	
	public class Player : Entity {
		private AnimationFrame testFrame;
		private Animation animation;
		private Animation[] walkAnimations;
		private AnimationPlayer animationPlayer;

		private float timer;

		public Player() {
			
		}
		
		public override void Initialize() {
			timer = 0.0f;

			Image image = Resources.GetImage("sheet_player");
			
			walkAnimations = new Animation[4];
			
			for (int dir = 0; dir < 4; dir++) {
				walkAnimations[dir] = new Animation();
				walkAnimations[dir].LoopCount = -1;
				if (dir > 0)
					walkAnimations[dir - 1].NextStrip = walkAnimations[dir];
				for (int i = 0; i < 2; i++)
					walkAnimations[dir].AddFrame(new AnimationFrame(i * 8, 8, image, new Rectangle2I((dir * 17 * 2) + (i * 17), 0, 16, 16), new Point2I(0, 0)));
			}

			animationPlayer = new AnimationPlayer();
			animationPlayer.Play(walkAnimations[0]);
			animationPlayer.SubStripIndex = 0;

			animation = walkAnimations[0];
			
			testFrame.Image = image;
			testFrame.StartTime = 0;
			testFrame.Duration = 100;
			testFrame.SourceRect = new Rectangle2I(0, 0, 16, 16);
			testFrame.DrawOffset = Point2I.Zero;
		}

		public override void Update(float timeDelta) {
			timer += timeDelta * 60.0f;
			while (timer >= animation.Duration && animation.Duration > 0)
				timer -= animation.Duration;

			bool isMoving = false;

			Vector2F moveDir = new Vector2F();
			
			float speed = GameSettings.PLAYER_MOVE_SPEED;
			if (Keyboard.IsKeyDown(Keys.Right)) {
				moveDir.X = 1;
				animationPlayer.SubStripIndex = 0;
			}
			if (Keyboard.IsKeyDown(Keys.Left)) {
				moveDir.X = -1;
				animationPlayer.SubStripIndex = 2;
			}
			if (Keyboard.IsKeyDown(Keys.Down)) {
				moveDir.Y = 1;
				animationPlayer.SubStripIndex = 3;
			}
			if (Keyboard.IsKeyDown(Keys.Up)) {
				moveDir.Y = -1;
				animationPlayer.SubStripIndex = 1;
			}
			
			if (moveDir.Length > 0.001f) {
				moveDir = moveDir.Normalized;
				position += moveDir * speed * timeDelta;
				animationPlayer.IsPlaying = true;
			}
			else {
				animationPlayer.IsPlaying = false;
			}

			animationPlayer.Update(timeDelta);
		}

		public override void Draw(Common.Graphics.Graphics2D g) {
			g.DrawAnimation(animationPlayer.SubStrip, animationPlayer.PlaybackTime, position.X, position.Y);
			//g.DrawSprite(testFrame.Sprite, position);
		}
	}
}
