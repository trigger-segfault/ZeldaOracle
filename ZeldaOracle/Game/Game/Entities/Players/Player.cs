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

		public Player() {
			
		}

		public override void Initialize() {
			
			testFrame.Image = Resources.GetImage("sheet_player");
			testFrame.StartTime = 0;
			testFrame.Duration = 100;
			testFrame.SourceRect = new Rectangle2I(0, 0, 16, 16);
			testFrame.DrawOffset = Point2I.Zero;
		}

		public override void Update(float timeDelta) {
			
			float speed = GameSettings.PLAYER_MOVE_SPEED;
			if (Keyboard.IsKeyDown(Keys.Right))
				position.X += speed * timeDelta;
			if (Keyboard.IsKeyDown(Keys.Left))
				position.X -= speed * timeDelta;
			if (Keyboard.IsKeyDown(Keys.Down))
				position.Y += speed * timeDelta;
			if (Keyboard.IsKeyDown(Keys.Up))
				position.Y -= speed * timeDelta;
			
		}

		public override void Draw(Common.Graphics.Graphics2D g) {
			g.DrawImage(
				testFrame.Image.Texture,
				position,
				testFrame.SourceRect);
		}
	}
}
