using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Util;

namespace ZeldaOracle.Game.Entities.Players.States {
	public class PlayerRespawnDeathState : PlayerState {

		private enum RespawnState {
			DeathAnimation,
			ViewPanning,
			Delay,
		}

		private bool waitForAnimation;
		private GenericStateMachine<RespawnState> subStateMachine;

		// Crush: 44 frames of squished. 40 frames of flicker
		// (blank, normal, blank, squish, blank, squish, blank, normal, (repeat))


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public PlayerRespawnDeathState() {
			waitForAnimation = true;
			
			StateParameters.EnableAutomaticRoomTransitions	= true;
			StateParameters.EnableStrafing					= true;
			StateParameters.DisableSolidCollisions			= true;
			StateParameters.DisableInteractionCollisions	= true;
			StateParameters.DisableGravity					= true;
			StateParameters.DisablePlayerControl			= true;
			StateParameters.DisableSurfaceContact			= true;

			// Configure the sub-state-machine
			subStateMachine = new GenericStateMachine<RespawnState>();
			subStateMachine.AddState(RespawnState.DeathAnimation)
				.OnUpdate(OnUpdateDeathAnimationState)
				.OnEnd(OnEndDeathAnimationState);
			subStateMachine.AddState(RespawnState.ViewPanning)
				.OnUpdate(OnUpdateViewPanningState)
				.OnEnd(OnEndViewPanningState);
			subStateMachine.AddState(RespawnState.Delay)
				.OnBegin(OnBeginDelayState)
				.AddEvent(16, delegate() {
					End();
				});
		}


		//-----------------------------------------------------------------------------
		// State Callbacks
		//-----------------------------------------------------------------------------

		private void OnUpdateDeathAnimationState() {
			// Wait for the death animation to complete
			if (!waitForAnimation || player.Graphics.IsAnimationDone)
				subStateMachine.NextState();
		}

		private void OnEndDeathAnimationState() {
			player.Graphics.IsVisible = false;
			player.Respawn();
		}

		private void OnUpdateViewPanningState() {
			// Wait for the view to pan to the player
			if (player.RoomControl.ViewControl.IsCenteredOnTarget())
				subStateMachine.NextState();
		}

		private void OnEndViewPanningState() {
			player.Graphics.IsVisible = true;
			player.Hurt(new DamageInfo(2));
		}
		
		private void OnBeginDelayState() {
			player.Graphics.PlayAnimation(player.Animations.Default);
			player.Graphics.PauseAnimation();
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnBegin(PlayerState previousState) {
			player.InterruptWeapons();
			player.Movement.StopMotion();
			player.Physics.ZVelocity = 0.0f;
			player.KnockbackVelocity = Vector2F.Zero;

			subStateMachine.InitializeOnState(RespawnState.DeathAnimation);
		}

		public override void Update() {
			subStateMachine.Update();
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public bool WaitForAnimation {
			get { return waitForAnimation; }
			set { waitForAnimation = value; }
		}
	}
}
