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
		private Item[]			equippedItems; // TODO: move this to somewhere else.
		private bool			isBusy;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public Player() {
			moveKeys = new Keys[4];
			moveAxes		= new bool[] { false, false };
			direction		= Directions.Down;
			angle			= Directions.ToAngle(direction);
			pushTimer		= 0;
			isMoving		= false;
			moveSpeed		= GameSettings.PLAYER_MOVE_SPEED;
			moveSpeedScale	= 1.0f;
			equippedItems	= new Item[2] { null, null };
			isBusy			= false;

			// Physics.
			Physics.CollideWithWorld = true;
			Physics.HasGravity = true;

			// Controls.
			moveKeys[Directions.Up]		= Keys.Up;
			moveKeys[Directions.Down]	= Keys.Down;
			moveKeys[Directions.Left]	= Keys.Left;
			moveKeys[Directions.Right]	= Keys.Right;

			// DEBUG: equip a bow item.
			equippedItems[0] = new ItemBow();
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
			if (!isBusy) {
				// Check movement keys.
				isMoving = false;
				if (!CheckMoveKey(Directions.Left) && !CheckMoveKey(Directions.Right))
					moveAxes[0] = false;	// x-axis
				if (!CheckMoveKey(Directions.Down) && !CheckMoveKey(Directions.Up))
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
			}

			// Update equipped items.
			for (int i = 0; i < equippedItems.Length; i++) {
				if (equippedItems[i] != null) {
					equippedItems[i].Player = this;
					equippedItems[i].Update();
				}
			}

			if (isBusy) {

			}
			else {
				// Update animations
				if (isMoving && !Graphics.IsAnimationPlaying)
					Graphics.PlayAnimation();
				if (!isMoving && Graphics.IsAnimationPlaying)
					Graphics.StopAnimation();
			}

			Graphics.SubStripIndex = direction;

			// Update superclass.
			base.Update(ticks);
		}

		public override void Draw(Graphics2D g) {
			base.Draw(g);
		}

		
		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public int Angle {
			get { return angle; }
		}
		
		public int Direction {
			get { return direction; }
		}
		
		public bool IsBusy {
			get { return isBusy; }
			set { isBusy = value; }
		}
	}
}
