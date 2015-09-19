using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Main;

namespace ZeldaOracle.Game.Entities.Players {
	public class PlayerGrabState : PlayerState {
		private int timer;
		private int duration;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public PlayerGrabState() {

		}
		

		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnBegin() {
			base.OnBegin();
			player.Movement.AllowMovementControl = false;

			player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_PULL);
			player.Graphics.AnimationPlayer.Pause();
		}
		
		public override void OnEnd() {
			player.Movement.AllowMovementControl = true;
			base.OnEnd();
		}

		public override void Update() {
			base.Update();
			
			if (!Controls.A.IsDown()) {
				player.BeginNormalState();
			}
			else if (Controls.Right.IsDown()) {
				player.Graphics.AnimationPlayer.PlaybackTime = GameData.ANIM_PLAYER_PULL.Duration;
				//player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_PULL);
			}
			else {
				player.Graphics.AnimationPlayer.PlaybackTime = 0;
				player.Graphics.AnimationPlayer.Stop();
			}
		}
	}
}
