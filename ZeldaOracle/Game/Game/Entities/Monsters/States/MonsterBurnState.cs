using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;

namespace ZeldaOracle.Game.Entities.Monsters.States {
	
	public class MonsterBurnState : MonsterState {
		
		private int timer;
		private int burnDuration;
		private int burnDamage;
		private AnimationPlayer flameAnimationPlayer;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public MonsterBurnState(int burnDamage) {
			this.burnDamage				= burnDamage;
			this.burnDuration			= GameSettings.MONSTER_BURN_DURATION;
			this.flameAnimationPlayer	= new AnimationPlayer();
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void OnBegin(MonsterState previousState) {
			timer = 0;
			flameAnimationPlayer.Play(GameData.ANIM_EFFECT_BURN);
			
			monster.Physics.Velocity = Vector2F.Zero;
			monster.Graphics.PauseAnimation();
			monster.IsPassable = true;

			// Apply damage (make monster invincible for burn duration).
			DamageInfo damageInfo = new DamageInfo(burnDamage);
			damageInfo.ApplyKnockBack		= false;
			damageInfo.Flicker				= false;
			damageInfo.InvincibleDuration	= burnDuration;
			monster.Hurt(damageInfo);
		}

		public override void OnEnd(MonsterState newState) {
			monster.Graphics.ResumeAnimation();
			monster.IsPassable = false;
			monster.OnBurnComplete();
		}

		public override void Update() {
			flameAnimationPlayer.Update();

			// Return to normal after the burn duration.
			timer++;
			if (timer > burnDuration)
				monster.BeginNormalState();
		}

		public override void DrawOver(RoomGraphics g) {
			// Draw the flame effect.
			g.DrawAnimationPlayer(flameAnimationPlayer, monster.Center -
				new Vector2F(0, monster.ZPosition), DepthLayer.EffectMonsterBurnFlame);
		}
	}
}
