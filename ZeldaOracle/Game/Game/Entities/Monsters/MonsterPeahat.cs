using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Game.Entities.Monsters {

	public class MonsterPeahat : BasicMonster {
		
		private enum FlyState {
			Stopped,
			Flying,
		}

		private GenericStateMachine<FlyState> stateMachine;
		private int flyTimer;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------
		
		public MonsterPeahat() {
			// General
			MaxHealth		= 2;
			ContactDamage	= 2;
			Color			= MonsterColor.Red;
			IsKnockbackable	= false;
			
			// Movement
			isFlying					= true;
			moveSpeed					= 0.75f;
			numMoveAngles				= 24;
			facePlayerOdds				= 0;
			changeDirectionsOnCollide	= true;
			movesInAir					= true;
			stopTime.Set(0, 0);
			moveTime.Set(15, 120);
								
			// Physics
			Physics.Gravity				= 0.0f;
			Physics.CollideWithWorld	= false;
			Physics.ReboundRoomEdge		= true;
			
			// Graphics
			animationMove = GameData.ANIM_MONSTER_PEAHAT;

			// Projectile interactions
			SetReaction(InteractionType.SwitchHook,	SenderReactions.Intercept, Reactions.Damage);
			SetReaction(InteractionType.Boomerang,	SenderReactions.Intercept);
			
			// Behavior
			stateMachine = new GenericStateMachine<FlyState>();
			stateMachine.AddState(FlyState.Stopped)
				.OnBegin(BeginStoppedState)
				.SetDuration(GameSettings.MONSTER_PEAHAT_STOP_DURATION);
			stateMachine.AddState(FlyState.Flying)
				.OnBegin(BeginFlyingState)
				.OnUpdate(UpdateFlyingState);
		}


		//-----------------------------------------------------------------------------
		// States
		//-----------------------------------------------------------------------------
		
		private void BeginStoppedState() {
			StopMoving();
			graphics.AnimationPlayer.Speed = 0.0f;
			physics.Velocity = Vector2F.Zero;
			Physics.Gravity = GameSettings.DEFAULT_GRAVITY;
			isDamageable = true;
			isStunnable = true;
		}
		
		private void BeginFlyingState() {
			Graphics.AnimationPlayer.Speed = 0.0f;
			flyTimer = GRandom.NextInt(GameSettings.MONSTER_PEAHAT_FLY_DURATION);
			speed = 0.0f;
			moveSpeed = 0.0f;
			isDamageable = true;
			isStunnable = true;
		}

		private void UpdateFlyingState() {
			flyTimer--;

			bool isVulnerable = (zPosition == 0);
			isDamageable = isVulnerable;
			isStunnable = isVulnerable;

			// Update deceleration
			if (flyTimer <= GameSettings.MONSTER_PEAHAT_DECELERATE_DURATION) {
				// Slow down animation
				Graphics.AnimationPlayer.Speed = GMath.Max(0.0f,
					Graphics.AnimationPlayer.Speed - 0.01f);

				if (moveSpeed > 0.0f || zPosition > 0.0f) {
					// Slow down movement and lower to the ground
					moveSpeed = GMath.Max(0.0f,
						moveSpeed - GameSettings.MONSTER_PEAHAT_DECELERATION);
					zPosition = GMath.Max(0.0f, zPosition -
						GameSettings.MONSTER_PEAHAT_LOWER_SPEED);
				}
				else if (Graphics.AnimationPlayer.Speed <= 0.0f) {
					// Begin the stopped state once the animation has stopped
					moveSpeed = 0.0f;
					stateMachine.BeginState(FlyState.Stopped);
					return;
				}
			}
			// Update acceleration
			else {
				// Speed up animation
				Graphics.AnimationPlayer.Speed = GMath.Min(1.0f,
					Graphics.AnimationPlayer.Speed + 0.01f);

				if (Graphics.AnimationPlayer.Speed > 0.5f) {
					// Speed up movement and raise into the air
					moveSpeed = GMath.Min(GameSettings.MONSTER_PEAHAT_FLY_SPEED,
						moveSpeed + GameSettings.MONSTER_PEAHAT_ACCELERATION);
					zPosition = GMath.Min(GameSettings.MONSTER_PEAHAT_FLY_ALTITUDE,
						zPosition + GameSettings.MONSTER_PEAHAT_RAISE_SPEED);
				}
			}
			
			Physics.Gravity = 0.0f;
			speed = moveSpeed;

			base.UpdateAI();
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void Initialize() {
			base.Initialize();
			stateMachine.BeginState(FlyState.Flying);
		}

		public override void OnBurn() {
			stateMachine.BeginState(FlyState.Flying);
			Physics.Gravity = GameSettings.DEFAULT_GRAVITY;
		}

		public override void OnStun() {
			stateMachine.BeginState(FlyState.Flying);
		}

		public override void UpdateAI() {
			stateMachine.Update();
		}
	}
}
