using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Game.Entities.Monsters.States {
	
	public class MonsterStunState : MonsterState {

		private int timer;
		private int stunDuration;
		private int shakeDuration;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------
		
		public MonsterStunState() {
			this.stunDuration	= GameSettings.MONSTER_STUN_DURATION;
			this.shakeDuration	= GameSettings.MONSTER_STUN_SHAKE_DURATION;
		}

		public MonsterStunState(int stunDuration) {
			this.stunDuration	= stunDuration;
			this.shakeDuration	= GameSettings.MONSTER_STUN_SHAKE_DURATION;
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void OnBegin(MonsterState previousState) {
			timer			= 0;

			monster.Physics.Velocity = Vector2F.Zero;
			monster.Graphics.PauseAnimation();
			//monster.DisablePhysics();
		}

		public override void OnEnd(MonsterState newState) {
			
			monster.Graphics.ResumeAnimation();
			//monster.EnablePhysics();
		}

		public override void Update() {
			timer++;

			if (timer >= stunDuration - shakeDuration) {
				if (timer % 8 == 0)
					monster.Position += new Vector2F(1, 0);
				else if (timer % 8 == 4)
					monster.Position -= new Vector2F(1, 0);
			}
			if (timer > stunDuration) {
				monster.BeginNormalState();
			}
		}
	}
}
