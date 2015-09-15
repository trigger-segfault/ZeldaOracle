using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Input;
using ZeldaOracle.Game.Main;
using ZeldaOracle.Game.Entities.Effects;
using ZeldaOracle.Game.Entities.Projectiles;

namespace ZeldaOracle.Game.Entities.Players {
	
	public class Player : Unit {
	
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
			moveKeys = new Keys[4];
			moveAxes		= new bool[] { false, false };
			direction		= Direction.Down;
			angle			= Direction.ToAngle(direction);
			pushTimer		= 0;
			isMoving		= false;
			moveSpeed		= GameSettings.PLAYER_MOVE_SPEED;
			moveSpeedScale	= 1.0f;

			// Physics.
			Physics.CollideWithWorld = true;
			Physics.HasGravity = true;

			// Controls.
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
					direction = dir;
			
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
			base.Initialize();

			// Play the default player animation.
			Graphics.PlayAnimation(GameData.ANIM_PLAYER_DEFAULT);
		}

		public override void Update(float ticks) {
			
			// DEBUG: Press space to jump.
			if (Keyboard.IsKeyPressed(Keys.Space))
				physics.ZVelocity = GameSettings.PLAYER_JUMP_SPEED;

			// DEBUG: Press Z to shoot an arrow.
			if (Keyboard.IsKeyPressed(Keys.Z)) {
				Projectile projectile = new Projectile();
				
				// General
				projectile.Position			= new Vector2F(position.X, position.Y - 8) + (Angle.ToVector(angle) * 8.0f);
				projectile.Angle			= angle;
				projectile.Physics.Velocity	= Angle.ToVector(angle) * 3.0f;
				projectile.Owner			= this;

				// Graphics.
				projectile.Graphics.SubStripIndex = angle;
				projectile.Graphics.PlayAnimation(GameData.ANIM_PROJECTILE_PLAYER_ARROW);

				// Physics.
				projectile.Physics.CollisionBox	= new Rectangle2F(-2, -2, 4, 4);
				projectile.EnablePhysics(PhysicsFlags.CollideWorld | PhysicsFlags.LedgePassable |
									PhysicsFlags.HalfSolidPassable | PhysicsFlags.DestroyedOutsideRoom);

				// Crash event.
				Vector2F v = projectile.Physics.Velocity;
				projectile.EventCollision += delegate() {
					// Create crash effect.
					Effect effect = new Effect();
					effect.Position = projectile.Position;
					effect.CreateDestroyTimer(32);
					
					effect.Physics.Velocity		= -(v.Normalized) * 0.25f;
					effect.Physics.ZVelocity	= 1;
					effect.Physics.Gravity		= 0.07f;
					effect.EnablePhysics(PhysicsFlags.HasGravity);
					
					effect.Graphics.IsShadowVisible = false;
					effect.Graphics.PlayAnimation(GameData.ANIM_PROJECTILE_PLAYER_ARROW_CRASH);

					RoomControl.SpawnEntity(effect);
					projectile.Destroy();
				};

				RoomControl.SpawnEntity(projectile);
			}

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
			}
			else {
				physics.Velocity = Vector2F.Zero;
			}

			// Update animations
			if (isMoving && !Graphics.IsAnimationPlaying)
				Graphics.PlayAnimation();
			if (!isMoving && Graphics.IsAnimationPlaying)
				Graphics.StopAnimation();

			Graphics.SubStripIndex = direction;

			// Update superclass.
			base.Update(ticks);
		}

		public override void Draw(Common.Graphics.Graphics2D g) {
			base.Draw(g);
		}
	}
}
