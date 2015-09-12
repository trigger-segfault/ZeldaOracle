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
		private AnimationPlayer animationPlayer;

		public Player() {
			animationPlayer = new AnimationPlayer();
		}
		
		public override void Initialize() {
			// Play the default player animation.
			animationPlayer.Play(GameData.ANIM_PLAYER_DEFAULT);
			animationPlayer.SubStripIndex = 0;
		}

		public override void Update(float timeDelta) {

			Vector2F moveDir = new Vector2F();
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
				position += moveDir * timeDelta * GameSettings.PLAYER_MOVE_SPEED;
				animationPlayer.IsPlaying = true;
			}
			else {
				animationPlayer.IsPlaying = false;
			}
			
			animationPlayer.Update(timeDelta);
		}

		public override void Draw(Common.Graphics.Graphics2D g) {
			g.DrawAnimation(animationPlayer.SubStrip, animationPlayer.PlaybackTime, position.X, position.Y);
		}
	}
}
