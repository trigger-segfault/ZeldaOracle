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
using ZeldaOracle.Game.Items;
using ZeldaOracle.Game.Items.Weapons;
using ZeldaOracle.Game.Control;

namespace ZeldaOracle.Game.Entities.Players {
	public class PlayerNormalState : PlayerState {
		
		private Keys[]	moveKeys;
		private bool[]	moveAxes;
		private bool	isMoving;
		//private int		direction;
		//private int		angle;
		private int		pushTimer;
		private float	moveSpeedScale;
		private float	moveSpeed;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public PlayerNormalState() : base() {
			moveKeys		= new Keys[4];
			moveAxes		= new bool[] { false, false };
			pushTimer		= 0;
			isMoving		= false;
			moveSpeed		= GameSettings.PLAYER_MOVE_SPEED;
			moveSpeedScale	= 1.0f;

			// Controls.
			moveKeys[Directions.Up]		= Keys.Up;
			moveKeys[Directions.Down]	= Keys.Down;
			moveKeys[Directions.Left]	= Keys.Left;
			moveKeys[Directions.Right]	= Keys.Right;
		}
		
		
		//-----------------------------------------------------------------------------
		// Internal
		//-----------------------------------------------------------------------------

		public void UpdateMoveControls() {
			// Check movement keys.
			isMoving = false;
			if (!CheckMoveKey(Directions.Left) && !CheckMoveKey(Directions.Right))
				moveAxes[0] = false;	// x-axis
			if (!CheckMoveKey(Directions.Down) && !CheckMoveKey(Directions.Up))
				moveAxes[1] = false;	// y-axis
			
			// Update motion.
			if (isMoving) {
				float a = (Player.Angle / 8.0f) * (float) GMath.Pi * 2.0f;
				Vector2F motion = new Vector2F((float) Math.Cos(a), -(float) Math.Sin(a));
				Player.Physics.Velocity = motion * moveSpeed;
			}
			else {
				Player.Physics.Velocity = Vector2F.Zero;
			}
		}
		
		private bool CheckMoveKey(int dir) {
			if (Keyboard.IsKeyDown(moveKeys[dir])) {
				isMoving = true;
			
				if (!moveAxes[(dir + 1) % 2])
					moveAxes[dir % 2] = true;
				if (moveAxes[dir % 2]) {
					Player.Angle = dir * 2;
					Player.Direction = dir;
			
					if (Keyboard.IsKeyDown(moveKeys[(dir + 1) % 4])) 
						Player.Angle = (Player.Angle + 1) % 8;
					if (Keyboard.IsKeyDown(moveKeys[(dir + 3) % 4]))
						Player.Angle = (Player.Angle + 7) % 8;
				}
				return true;
			}
			return false;
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnBegin() {
			pushTimer		= 0;
			isMoving		= false;
			player.Angle	= Directions.ToAngle(player.Direction);
			Player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_DEFAULT);
		}
		
		public override void OnEnd() {
			pushTimer		= 0;
			isMoving		= false;
			player.Angle	= Directions.ToAngle(player.Direction);
			Player.Physics.Velocity = Vector2F.Zero;
			Player.Graphics.StopAnimation();
		}

		public override void Update() {
			if (Keyboard.IsKeyPressed(Keys.Space))
				player.Physics.ZVelocity = GameSettings.PLAYER_JUMP_SPEED;

			UpdateMoveControls();

			// Update animations
			if (isMoving && !Player.Graphics.IsAnimationPlaying)
				Player.Graphics.PlayAnimation();
			if (!isMoving && Player.Graphics.IsAnimationPlaying)
				Player.Graphics.StopAnimation();

			// Update pushing.
			CollisionInfo collisionInfo = player.Physics.CollisionInfo[player.Direction];
			if (collisionInfo.Type == CollisionType.Tile) {
				player.Graphics.AnimationPlayer.Animation = GameData.ANIM_PLAYER_PUSH;
				pushTimer++;
				if (pushTimer > 30) {
					
					Message message = new Message("Oof! It's heavy!");
					player.RoomControl.GameManager.QueueGameStates(
						new StateTextReader(message)
					);
					pushTimer = 0;
				}
			}
			else {
				player.Graphics.AnimationPlayer.Animation = GameData.ANIM_PLAYER_DEFAULT;
				pushTimer = 0;
			}
			
			Player.UpdateEquippedItems();

			player.Physics.SetFlags(PhysicsFlags.CollideRoomEdge, !isMoving || player.IsInAir);
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public bool IsJumping {
			get { return player.IsInAir; }
		}
	}
}
