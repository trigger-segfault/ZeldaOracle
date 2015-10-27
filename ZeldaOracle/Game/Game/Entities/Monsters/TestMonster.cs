using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Input;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.Entities.Effects;
using ZeldaOracle.Game.Entities.Projectiles;
using ZeldaOracle.Game.Items;
using ZeldaOracle.Game.Items.Weapons;

namespace ZeldaOracle.Game.Entities.Monsters {

	public class TestMonster : Monster {

		private Keys[] movementKeys;
		private float moveSpeed;
		private bool isMoving;
		private bool[] moveAxes;
		private int moveAngle;
		private int moveDirection;

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------
		
		public TestMonster() {

			moveSpeed		= 1.0f;
			isMoving		= false;
			moveAxes		= new bool[] { false, false };
			moveDirection	= Directions.Right;
			moveAngle		= Angles.Right;

			movementKeys = new Keys[] {
				Keys.NumPad6, // Right
				Keys.NumPad8, // Up
				Keys.NumPad4, // Left
				Keys.NumPad5, // Down
			};

			syncAnimationWithDirection = true;
			MaxHealth = 10;
		}
		
		
		// Poll the movement key for the given direction, returning true if
		// it is down. This also manages the strafing behavior of movement.
		private bool CheckMoveKey(int dir) {
			if (Keyboard.IsKeyDown(movementKeys[dir])) {
				isMoving = true;

				if (!moveAxes[(dir + 1) % 2])
					moveAxes[dir % 2] = true;

				if (moveAxes[dir % 2]) {
					moveDirection = dir;
					moveAngle = dir * 2;

					if (Keyboard.IsKeyDown(movementKeys[(dir + 1) % 4]))
						moveAngle = (moveAngle + 1) % 8;
					if (Keyboard.IsKeyDown(movementKeys[(dir + 3) % 4]))
						moveAngle = (moveAngle + 7) % 8;
				}
				return true;
			}
			return false;
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void Initialize() {
			base.Initialize();
		}

		public override void Update() {
			isMoving = false;

			// Check movement keys.
			if (!CheckMoveKey(Directions.Left) && !CheckMoveKey(Directions.Right))
				moveAxes[Axes.X] = false;
			if (!CheckMoveKey(Directions.Up) && !CheckMoveKey(Directions.Down))
				moveAxes[Axes.Y] = false;

			direction = moveDirection;

			if (isMoving) {
				Vector2F moveVector = Angles.ToVector(moveAngle, true) * moveSpeed;
				physics.Velocity = moveVector;
				if (!Graphics.IsAnimationPlaying)
					Graphics.PlayAnimation();
			}
			else {
				physics.Velocity = Vector2F.Zero;
				if (Graphics.IsAnimationPlaying)
					Graphics.StopAnimation();
			}

			base.Update();
		}
	}
}
