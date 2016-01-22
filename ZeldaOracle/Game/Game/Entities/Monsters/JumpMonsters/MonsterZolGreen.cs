using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Game.Entities.Monsters.JumpMonsters {
	public class MonsterZolGreen : JumpMonster {

		private int jumpIndex;
		private int numJumps;
		private bool isBurrowing;
		private bool isBurrowed;
		private int burrowTimer;
		private int unburrowDelay;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public MonsterZolGreen() {
			
			color			= MonsterColor.Green;
			MaxHealth		= 1;
			ContactDamage	= 1;

			moveSpeed		= 0.75f;
			jumpSpeed		= new RangeF(2.0f);
			stopTime		= new RangeI(48);
			stopAnimation	= GameData.ANIM_MONSTER_ZOL;
			jumpAnimation	= GameData.ANIM_MONSTER_ZOL_JUMP;
		}


		//-----------------------------------------------------------------------------
		// Burrowing
		//-----------------------------------------------------------------------------

		private void Burrow() {
			isBurrowed	= true;
			isBurrowing	= true;
			Graphics.PlayAnimation(GameData.ANIM_MONSTER_ZOL_BURROW);
			burrowTimer = 0;
		}
		
		private void UnBurrow() {
			isBurrowed	= false;
			jumpIndex	= 0;
			isBurrowing	= true;
			Graphics.PlayAnimation(GameData.ANIM_MONSTER_ZOL_UNBURROW);
			Graphics.IsVisible = true;
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------
		public override void Initialize() {
			base.Initialize();

			jumpIndex		= 0;
			numJumps		= 3;
			isBurrowed		= true;
			unburrowDelay	= 50;
			
			Graphics.IsVisible = false;
		}

		public override void OnJump() {
			base.OnJump();
			jumpIndex++;
		}

		public override void OnEndJump() {
			base.OnEndJump();
			if (jumpIndex > numJumps) {
				Burrow();
			}
		}

		public override void UpdateAI() {
			if (isBurrowing) {
				if (Graphics.IsAnimationDone) {
					isBurrowing = false;

					if (isBurrowed) {
						Graphics.IsVisible = false;
					}
					else {
						Jump();
						Physics.Velocity = Vector2F.Zero;
						Graphics.PlayAnimation(GameData.ANIM_MONSTER_ZOL);
						Graphics.PauseAnimation();
					}
				}
			}
			else if (isBurrowed) {
				burrowTimer++;
				if (burrowTimer > unburrowDelay) {
					float distanceToPlayer = Vector2F.Distance(RoomControl.Player.Center, Center);
					if (distanceToPlayer < 48)
						UnBurrow();
				}
			}
			else {
				base.UpdateAI();
			}
		}
	}
}
