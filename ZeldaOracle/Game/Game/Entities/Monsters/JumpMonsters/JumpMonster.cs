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
using ZeldaOracle.Game.Entities.Monsters.Tools;
using ZeldaOracle.Game.Entities.Projectiles;
using ZeldaOracle.Game.Items;
using ZeldaOracle.Game.Items.Weapons;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Graphics.Sprites;

namespace ZeldaOracle.Game.Entities.Monsters {

	public class JumpMonster : Monster {
		
		// Movement.
		protected Animation	stopAnimation;
		protected Animation	jumpAnimation;
		protected float		moveSpeed;
		protected RangeI	stopTime;
		protected RangeF	jumpSpeed;
		protected int		stopTimer;
		protected bool		isJumping;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------
		
		public JumpMonster() {
			color			= MonsterColor.Red;
			MaxHealth		= 1;
			ContactDamage	= 1;

			moveSpeed		= 1.0f;
			stopTime		= new RangeI(30, 60);
			jumpSpeed		= new RangeF(3, 3);
			stopAnimation	= GameData.ANIM_MONSTER_ZOL;
			jumpAnimation	= GameData.ANIM_MONSTER_ZOL_JUMP;

			syncAnimationWithDirection = false;
		}


		//-----------------------------------------------------------------------------
		// Jump Methods
		//-----------------------------------------------------------------------------

		public virtual void OnJump() {
		}
		
		public virtual void OnEndJump() {
		}

		public void Jump() {
			isJumping	= true;
			Graphics.PlayAnimation(jumpAnimation);

			Physics.ZVelocity = GRandom.NextFloat(jumpSpeed.Min, jumpSpeed.Max);
			Physics.Velocity = (RoomControl.Player.Center - Center).Normalized * moveSpeed;

			AudioSystem.PlaySound(GameData.SOUND_MONSTER_JUMP);

			OnJump();
		}

		public void EndJump() {
			Physics.Velocity = Vector2F.Zero;
			isJumping	= false;
			stopTimer	= GRandom.NextInt(stopTime.Min, stopTime.Max);
			Graphics.PlayAnimation(stopAnimation);

			OnEndJump();
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------
		
		public override void Initialize() {
			base.Initialize();

			isJumping	= false;
			stopTimer	= GRandom.NextInt(stopTime.Min, stopTime.Max);
			Graphics.PlayAnimation(stopAnimation);
		}

		public override void UpdateAI() {
			if (IsOnGround) {
				if (isJumping) {
					EndJump();
				}
				else {
					stopTimer--;
					if (stopTimer <= 0) {
						Jump();
					}
				}
			}
		}

	}
}
