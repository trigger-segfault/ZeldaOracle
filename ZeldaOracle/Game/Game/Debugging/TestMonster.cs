using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Input;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Entities.Monsters;
using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.Entities.Effects;
using ZeldaOracle.Game.Entities.Monsters.Tools;
using ZeldaOracle.Game.Entities.Projectiles;
using ZeldaOracle.Game.Items;
using ZeldaOracle.Game.Items.Weapons;
using ZeldaOracle.Game.Entities.Projectiles.MonsterProjectiles;

namespace ZeldaOracle.Game.Debugging {

	public class TestMonster : Monster {

		/*
		 * -8, -8
			SUBSTRIP repeat; ADD frame, 4, (0, 0), ( 12,   4);
			SUBSTRIP repeat; ADD frame, 4, (2, 0), ( -4, -12);
			SUBSTRIP repeat; ADD frame, 4, (4, 0), (-12,   4);
			SUBSTRIP repeat; ADD frame, 4, (6, 0), (  3,  14); END;
		*/

		private readonly Rectangle2I[] SWORD_BOXES = new Rectangle2I[] {
			new Rectangle2I( 12 - 8,   4 - 8 + 7, 11, 2),
			new Rectangle2I(  4 - 8 + 7, -12 - 8 + 3 + 2, 2, 11),
			new Rectangle2I(-12 - 8 + 3 + 2,   4 - 8 + 7, 11, 2),
			new Rectangle2I( -4 - 8 + 7,  14 - 8, 2, 8),
		};


		private Keys[] movementKeys;
		private float moveSpeed;
		private bool isMoving;
		private bool[] moveAxes;
		private int moveAngle;
		private int moveDirection;
		private MonsterToolSword toolSword;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------
		
		public TestMonster() {

			moveSpeed		= 0.75f;
			isMoving		= false;
			moveAxes		= new bool[] { false, false };
			moveDirection	= Direction.Right;
			moveAngle		= Angles.Right;

			toolSword = new MonsterToolSword();

			movementKeys = new Keys[] {
				Keys.NumPad6, // Right
				Keys.NumPad8, // Up
				Keys.NumPad4, // Left
				Keys.NumPad5, // Down
			};

			syncAnimationWithDirection = true;
			MaxHealth = 10;
			
			Color = MonsterColor.Orange;
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

			//Graphics.PlayAnimation(GameData.ANIM_MONSTER_MOBLIN);
			Graphics.PlayAnimation(GameData.ANIM_MONSTER_DARKNUT);
		}

		public override void UpdateAI() {
			isMoving = false;

			// Check movement keys.
			if (!CheckMoveKey(Direction.Left) && !CheckMoveKey(Direction.Right))
				moveAxes[Axes.X] = false;
			if (!CheckMoveKey(Direction.Up) && !CheckMoveKey(Direction.Down))
				moveAxes[Axes.Y] = false;

			direction = moveDirection;

			if (isMoving) {
				Vector2F moveVector = Angles.ToVector(moveAngle, true) * moveSpeed;
				physics.Velocity = moveVector;
				if (!Graphics.IsAnimationPlaying) {
					Graphics.PlayAnimation();
					if (toolSword.IsEquipped)
						toolSword.AnimationPlayer.Play();
				}
			}
			else {
				physics.Velocity = Vector2F.Zero;
				if (Graphics.IsAnimationPlaying) {
					Graphics.StopAnimation();
					if (toolSword.IsEquipped)
						toolSword.AnimationPlayer.Stop();
				}
			}

			// NumPad7: Shoot a projectile
			if (Keyboard.IsKeyPressed(Keys.NumPad7)) {
				//ShootFromDirection(new MagicProjectile(), direction, 2.0f);
				//ShootFromDirection(new RockProjectile(), direction, 2.0f);
				ShootFromDirection(new MonsterArrow(), direction, 2.0f);
			}

			// NumPad /: Equip/Unequip a sword
			if (Keyboard.IsKeyPressed(Keys.Divide)) {
				if (!toolSword.IsEquipped)
					EquipTool(toolSword);
				else
					UnequipTool(toolSword);
			}
		}
	}
}
