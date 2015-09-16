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
using ZeldaOracle.Game.Tiles;

namespace ZeldaOracle.Game.Entities.Players {
	public class PlayerNormalState : PlayerState {
		

		private Keys[]		moveKeys;
		private bool[]		moveAxes;
		private bool		isMoving;
		private int			pushTimer;
		private float		moveSpeedScale;
		private float		moveSpeed;
		private Vector2F	velocityPrev;
		private Vector2F	motion;		// The vector that's driving the player's velocity.

		private bool		isOnIce;
		private bool		isSwimming;
		private bool		isDiving;


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
			motion			= new Vector2F();
			isSwimming		= false;
			isOnIce			= false;

			// Controls.
			moveKeys[Directions.Up]		= Keys.Up;
			moveKeys[Directions.Down]	= Keys.Down;
			moveKeys[Directions.Left]	= Keys.Left;
			moveKeys[Directions.Right]	= Keys.Right;
		}
		
		
		//-----------------------------------------------------------------------------
		// Internal
		//-----------------------------------------------------------------------------

		public void Jump() {
			if (player.IsOnGround) {
				player.Physics.ZVelocity = GameSettings.PLAYER_JUMP_SPEED;
				player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_JUMP);
			}
		}

		public void CheckTiles() {
			isOnIce = false;
			if (!isSwimming)
				moveSpeedScale = 1.0f;

			if (player.IsOnGround) 
			{
				Point2I origin = (Point2I) player.Position - new Point2I(0, 2);
				Point2I location = origin / new Point2I(GameSettings.TILE_SIZE, GameSettings.TILE_SIZE);
				if (!player.RoomControl.IsTileInBounds(location))
					return;

				for (int i = 0; i < player.RoomControl.Room.LayerCount; i++) {
					Tile tile = player.RoomControl.GetTile(location, i);
					if (tile != null) {
					
						if (tile.Flags.HasFlag(TileFlags.Stairs)) {
							moveSpeedScale = Math.Min(0.5f, moveSpeedScale);
						}
						if (tile.Flags.HasFlag(TileFlags.Ice)) {
							isOnIce = true;
						}
					}
				}

			}
		}

		public void UpdateMoveControls() {
			 // TODO: magic numbers everywhere!

			player.Physics.SetFlags(PhysicsFlags.AutoDodge, true);
			isMoving	= false;
			moveSpeed	= (isSwimming ? 0.5f : GameSettings.PLAYER_MOVE_SPEED);
			float acceleration = 0.08f;
			if (IsJumping)
				acceleration = 0.1f;
			//if (!swimming && player.isSprinting())
			//	moveSpeed *= 1.5;

			// If player is jumping, then allow slippery movement on the descent.

			if (player.IsOnGround || player.Physics.ZVelocity < 0.1f) {
				float speed = moveSpeed * moveSpeedScale;

				// Check movement keys.
				if (!CheckMoveKey(Directions.Left) && !CheckMoveKey(Directions.Right))
					moveAxes[0] = false;	// x-axis
				if (!CheckMoveKey(Directions.Down) && !CheckMoveKey(Directions.Up))
					moveAxes[1] = false;	// y-axis

				// Don't auto-dodge collisions when moving at an angle.
				if (!Angles.IsHorizontal(player.Angle) && !Angles.IsVertical(player.Angle))
					player.Physics.SetFlags(PhysicsFlags.AutoDodge, false);

				// Update motion.
				if (isMoving || IsStroking) {
					if (!isMoving)
						player.Angle = Directions.ToAngle(player.Direction);
					Vector2F keyMotion = Angles.ToVector(Player.Angle) * speed; // The velocity we want to move at.

					// Update slippery motion.
					if (IsJumping || isSwimming || isOnIce) {
						Vector2F velocity = player.Physics.Velocity;

						if (Math.Abs(velocity.X) < Math.Abs(velocityPrev.X) || Math.Sign(velocity.X) != Math.Sign(velocityPrev.X))
							motion.X = player.Physics.Velocity.X;
						if (Math.Abs(velocity.Y) < Math.Abs(velocityPrev.Y) || Math.Sign(velocity.Y) != Math.Sign(velocityPrev.Y))
							motion.Y = player.Physics.Velocity.Y;

						// Apply acceleration.
						motion += keyMotion * acceleration;

						// Limit speed.
						float newLength = motion.Length;
						if (newLength >= speed)
							motion.Length = speed;

						if (Math.Abs(newLength - (motion + (keyMotion * 0.08f)).Length) < acceleration * 2.0f) {
							motion += keyMotion * 0.04f;
						}

						// Snap velocity direction.
						if (isSwimming) {
							// Snap motion to 8 angles on the unit circle.
							int angle = (int)((Math.Atan2(-motion.Y, motion.X) / (GMath.Pi * 0.25f)) + 0.5f);
							player.Physics.Velocity = new Vector2F(
								(float) Math.Cos(angle * GMath.Pi * 0.25f) * motion.Length,
								(float) Math.Sin(angle * GMath.Pi * 0.25f) * motion.Length);
							player.Physics.Velocity = motion;
						}
						else {
							// Snap motion to 16 angles on the unit circle.
							int angle = (int)((Math.Atan2(-motion.Y, motion.X) / (GMath.Pi * 0.25f * 0.5f)) + 0.5f);
							player.Physics.Velocity = new Vector2F(
								(float) Math.Cos(angle * GMath.Pi * 0.25f * 0.5f) * motion.Length,
								(float) Math.Sin(angle * GMath.Pi * 0.25f * 0.5f) * motion.Length);
							player.Physics.Velocity = motion;
						}
					}
					else {
						// For normal ground motion, move at regular speed.
						motion = keyMotion;
						Player.Physics.Velocity = motion;
					}
				}
				else {
					if (isSwimming || isOnIce) {
						// Apply slippery friction.
						float length = motion.Length;
						if (length < 0.05f)
							motion = Vector2F.Zero;
						else
							motion.Length = length - 0.05f;
						player.Physics.Velocity = motion;
					}
					else if (player.IsOnGround) {
						// Stop moving.
						motion = Vector2F.Zero;
						Player.Physics.Velocity = Vector2F.Zero;
						player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_DEFAULT);
					}
				}
			}
			else if (player.IsOnGround) {
				Player.Physics.Velocity = Vector2F.Zero;
			}
		}
		
		private void UpdateSwimming() {
			if (moveSpeedScale > 1.0f)
				moveSpeedScale -= 0.025f;
		
			if (!IsStroking && (Controls.A.IsPressed() || Keyboard.IsKeyPressed(Keys.Space))) {
				//Sounds.PLAYER_SWIM.play();
				moveSpeedScale = 2.0f;
			}

			if (isDiving) {
				//if (player.isAnimationDone() || Keyboard.b.pressed()) {
				//	isDiving = false;
				//	player.resetAnimation();
				//}
			}
			else if (Controls.B.IsPressed()) {
				//isDiving = true;
				//player.setAnimation(false, Animations.PLAYER_DIVE);
				//Sounds.PLAYER_WADE.play();
				//game.addEntity(new Effect(
				//		Resources.SPRITE_EFFECT_SPLASH_WATER,
				//		player.getCenter().plus(0, 4)));
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
			moveSpeed		= GameSettings.PLAYER_MOVE_SPEED;
			moveSpeedScale	= 1.0f;
			pushTimer		= 0;
			isMoving		= false;
			player.Angle	= Directions.ToAngle(player.Direction);
			motion			= Vector2F.Zero;
			isOnIce			= false;
			isSwimming		= false;
			isDiving		= false;
			Player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_DEFAULT);
		}
		
		public override void OnEnd() {
			pushTimer		= 0;
			isMoving		= false;
			player.Angle	= Directions.ToAngle(player.Direction);
			if (Player.IsOnGround) {
				Player.Physics.Velocity = Vector2F.Zero;
				Player.Graphics.StopAnimation();
			}
		}

		public override void Update() {
			CheckTiles();
			UpdateMoveControls();
			if (isSwimming)
				UpdateSwimming();

			// Update animations
			if (!player.IsInAir) {
				if (isMoving && !Player.Graphics.IsAnimationPlaying)
					Player.Graphics.PlayAnimation();
				if (!isMoving && Player.Graphics.IsAnimationPlaying)
					Player.Graphics.StopAnimation();
			}

			// Update pushing.
			CollisionInfo collisionInfo = player.Physics.CollisionInfo[player.Direction];
			if (!player.IsInAir && collisionInfo.Type == CollisionType.Tile && !collisionInfo.Tile.IsMoving) {
				Tile tile = collisionInfo.Tile;
				player.Graphics.AnimationPlayer.Animation = GameData.ANIM_PLAYER_PUSH;
				pushTimer++;
				if (pushTimer > 20 && tile.Flags.HasFlag(TileFlags.Movable)) {
					tile.Push(player.Direction, 1.0f);
					//Message message = new Message("Oof! It's heavy!");
					//player.RoomControl.GameManager.PushGameState(new StateTextReader(message));
					pushTimer = 0;
				}
			}
			else if (!player.IsInAir) {
				player.Graphics.AnimationPlayer.Animation = GameData.ANIM_PLAYER_DEFAULT;
				pushTimer = 0;
			}
			
			Player.UpdateEquippedItems();

			player.Physics.SetFlags(PhysicsFlags.CollideRoomEdge, !isMoving || player.IsInAir);

			velocityPrev = player.Physics.Velocity;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public bool IsJumping {
			get { return player.IsInAir; }
		}

		public bool IsStroking {
			get { return (isSwimming && moveSpeedScale > 1.3f); }
		}
	}
}
