using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Graphics.Sprites;

namespace ZeldaOracle.Game.Entities.Players.States {

	public class PlayerBusyState : PlayerState {

		private int timer;
		private int duration;
		private Dictionary<int, Action<PlayerState>> actions;
		private Action<PlayerState> endAction;
		private Animation animation;
		private Animation animationInMinecart;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public PlayerBusyState() {
			this.timer		= 0;
			this.duration	= 0;
			this.actions	= new Dictionary<int,Action<PlayerState>>();
			this.endAction	= null;
			this.animation	= null;
			this.animationInMinecart = null;
		}
		
		public PlayerBusyState(int duration, Animation animation) {
			this.timer		= 0;
			this.duration	= duration;
			this.actions	= new Dictionary<int,Action<PlayerState>>();
			this.endAction	= null;
			this.animation	= animation;
			this.animationInMinecart = animation;
		}

		//-----------------------------------------------------------------------------
		// Customization
		//-----------------------------------------------------------------------------

		public void AddDelayedAction(int time, Action<PlayerState> action) {
			actions[time] = action;
		}

		public void SetEndAction(Action<PlayerState> action) {
			endAction = action;
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnBegin(PlayerState previousState) {
			//player.Movement.MoveCondition = PlayerMoveCondition.OnlyInAir;
			StateParameters.ProhibitMovementControlOnGround = true;
			timer = 0;
			
			if (actions.ContainsKey(0))
				actions[0].Invoke(this);
		}
		
		public override void OnEnd(PlayerState newState) {
			//player.Movement.MoveCondition = PlayerMoveCondition.FreeMovement;
			timer = 0;
			duration = 0;
			actions.Clear();
			if (endAction != null)
				endAction(this);
		}

		public override void OnExitMinecart() {
			base.OnExitMinecart();
			if (player.Graphics.Animation == animation && animationInMinecart != null)
				player.Graphics.SetAnimation(animationInMinecart);
		}

		public override void Update() {
			base.Update();

			timer++;

			if (actions.ContainsKey(timer))
				actions[timer].Invoke(this);

			if (timer >= duration) {
				End();
				//player.BeginNormalState();
			}
		}

		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public int Duration {
			get { return duration; }
			set { duration = value; }
		}
		
		public Animation Animation {
			get { return animation; }
			set { animation = value; }
		}
		
		public Animation AnimationInMinecart {
			get { return animationInMinecart; }
			set { animationInMinecart = value; }
		}
	}
}
