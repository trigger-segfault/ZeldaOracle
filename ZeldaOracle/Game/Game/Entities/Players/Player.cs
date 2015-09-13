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

		private AnimationPlayer	animationPlayer;		
		private Keys[]			moveKeys;
		private bool[]			moveAxes;
		private bool			isMoving;
		private int				direction;
		private int				angle;
		private int				pushTimer;
		private float			moveSpeedScale;
		private float			moveSpeed;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public Player() {
			moveKeys		= new Keys[4];
			moveAxes		= new bool[] { false, false };
			animationPlayer = new AnimationPlayer();
			direction		= Direction.Down;
			pushTimer		= 0;
			isMoving		= false;
			moveSpeed		= GameSettings.PLAYER_MOVE_SPEED;
			moveSpeedScale	= 1.0f;
			flags			|= EntityFlags.CollideWorld;

			moveKeys[Direction.Up]		= Keys.Up;
			moveKeys[Direction.Down]	= Keys.Down;
			moveKeys[Direction.Left]	= Keys.Left;
			moveKeys[Direction.Right]	= Keys.Right;
		}


		//-----------------------------------------------------------------------------
		// Movement
		//-----------------------------------------------------------------------------
		
		private bool CheckMoveKey(int dir) {
			
			if (Keyboard.IsKeyDown(moveKeys[dir])) {
				isMoving = true;
			
				if (!moveAxes[(dir + 1) % 2])
					moveAxes[dir % 2] = true;
				if (moveAxes[dir % 2]) {
					angle = dir * 2;
					direction   = dir;
			
					if (Keyboard.IsKeyDown(moveKeys[(dir + 1) % 4])) 
						angle = (angle + 1) % 8;
					if (Keyboard.IsKeyDown(moveKeys[(dir + 3) % 4]))
						angle = (angle + 7) % 8;
				}
				return true;
			}
			return false;
		}

		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------
		
		public override void Initialize() {
			// Play the default player animation.
			animationPlayer.Play(GameData.ANIM_PLAYER_DEFAULT);
			animationPlayer.SubStripIndex = 0;
		}

		public override void Update(float ticks) {
			
			// Check movement keys.
			isMoving = false;
			if (!CheckMoveKey(Direction.Left) && !CheckMoveKey(Direction.Right))
				moveAxes[0] = false;	// x-axis
			if (!CheckMoveKey(Direction.Down) && !CheckMoveKey(Direction.Up))
				moveAxes[1] = false;	// y-axis

			// Update motion.
			if (isMoving) {
				float a = (angle / 8.0f) * (float) GMath.Pi * 2.0f;
				Vector2F motion = new Vector2F((float) Math.Cos(a), -(float) Math.Sin(a));
				physics.Velocity = motion * moveSpeed;
				animationPlayer.IsPlaying = true;
			}
			else {
				physics.Velocity = Vector2F.Zero;
			}

			// Update animations
			if (isMoving && !animationPlayer.IsPlaying)
				animationPlayer.Play();
			if (!isMoving && animationPlayer.IsPlaying)
				animationPlayer.Stop();

			animationPlayer.SubStripIndex = direction;
			animationPlayer.Update(ticks);

			// Update superclass.
			base.Update(ticks);
		}

		public override void Draw(Common.Graphics.Graphics2D g) {
			g.DrawAnimation(animationPlayer.SubStrip, animationPlayer.PlaybackTime, position.X, position.Y);
		}
	}
}
