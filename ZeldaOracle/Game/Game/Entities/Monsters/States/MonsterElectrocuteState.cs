using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;

namespace ZeldaOracle.Game.Entities.Monsters.States {
	
	public class MonsterElectrocuteState : MonsterState {

		private int timer;
		private int animateDuration;
		private int freezePlayerDuration;
		private Animation monsterAnimation;
		private Animation resumeAnimation;

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------
		
		public MonsterElectrocuteState(Animation monsterAnimation) {
			this.animateDuration		= 60;
			this.freezePlayerDuration	= 45;
			this.monsterAnimation		= monsterAnimation;
		}

		//public MonsterElectrocuteState(int stunDuration) {
		//	this.stunDuration	= stunDuration;
		//	this.shakeDuration	= GameSettings.MONSTER_STUN_SHAKE_DURATION;
		//}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void OnBegin(MonsterState previousState) {
			timer = 0;

			monster.Physics.Velocity = Vector2F.Zero;
			resumeAnimation = monster.Graphics.Animation;//monster.Graphics.PauseAnimation();
			monster.Graphics.PlayAnimation(monsterAnimation);
			monster.RoomControl.Player.Freeze();
			//monster.DisablePhysics();

			// TODO: Freeze player.
		}

		public override void OnEnd(MonsterState newState) {
			monster.Graphics.PlayAnimation(resumeAnimation);
			monster.RoomControl.Player.Unfreeze();

			//monster.Graphics.ResumeAnimation();
			//monster.EnablePhysics();
		}

		public override void Update() {
			timer++;

			if (timer > animateDuration) {
				monster.BeginNormalState();
			}

			if (timer == freezePlayerDuration) {
				// TODO: Unfreeze player
			}
		}
	}
}
