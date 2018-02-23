using System;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Audio;

namespace ZeldaOracle.Game.Entities.Monsters {

	public class MonsterThwomp : Monster {

		private enum CrushState {
			Idle,	// Waiting for player to come near
			Crush,	// Accelearting down to the ground
			Hit,	// Hit the ground, screen is shaking. TODO: screen shake
			Raise,	// Moving up back to idle position
		}

		private GenericStateMachine<CrushState> stateMachine;
		private Vector2F hoverPosition;
		private float crushSpeed;
		private int eyeAngle;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public MonsterThwomp() {
			// General.
			ContactDamage	= 4;
			Color			= MonsterColor.DarkBlue;
			IsDamageable	= false;
			isBurnable		= false;
			isStunnable		= false;
			isGaleable		= false;
			IsKnockbackable	= false;

			centerOffset				= new Point2I(0, 0);
			Graphics.DrawOffset			= new Point2I(-16, -16);
			Physics.CollisionBox		= new Rectangle2F(-15, -16, 30, 8);
			Physics.SoftCollisionBox	= new Rectangle2F(-16, -12, 32, 28);//.Inflated(-2, -2);
			Physics.HasGravity			= false;
			Physics.IsSolid				= true;
			Physics.CollideWithWorld	= false;
			Physics.CollideWithRoomEdge	= false;

			// TODO: hard collision box should only apply to top, the Thwomp is not
			// actually supposed to crush the player

			// Weapon Interactions
			SetReaction(InteractionType.Sword,			SenderReactions.Bump, Reactions.ClingEffect);
			SetReaction(InteractionType.SwordSpin,		SenderReactions.Bump, Reactions.ClingEffect);
			SetReaction(InteractionType.BiggoronSword,	Reactions.ClingEffect);
			SetReaction(InteractionType.Shovel,			Reactions.ClingEffect);
			// Projectile Interactions
			SetReaction(InteractionType.Arrow,			SenderReactions.Intercept);
			SetReaction(InteractionType.SwordBeam,		SenderReactions.Intercept);
			SetReaction(InteractionType.Boomerang,		SenderReactions.Intercept, Reactions.ClingEffect);
			SetReaction(InteractionType.SwitchHook,		SenderReactions.Intercept, Reactions.ClingEffect);
			
			// Behavior
			stateMachine = new GenericStateMachine<CrushState>();
			stateMachine.AddState(CrushState.Idle)
				.OnBegin(OnBeginIdleState)
				.OnUpdate(OnUpdateIdleState);
			stateMachine.AddState(CrushState.Crush)
				.OnBegin(OnBeginCrushState)
				.OnUpdate(OnUpdateCrushState);
			stateMachine.AddState(CrushState.Hit)
				.OnBegin(OnBeginHitState)
				.SetDuration(GameSettings.MONSTER_THWOMP_HIT_WAIT_DURATION);
			stateMachine.AddState(CrushState.Raise)
				.OnUpdate(OnUpdateRaiseState);
		}


		//-----------------------------------------------------------------------------
		// States
		//-----------------------------------------------------------------------------
		
		private void OnBeginIdleState() {
			Graphics.PlayAnimation(GameData.ANIM_MONSTER_THWOMP);
			position = hoverPosition;
			physics.Velocity = Vector2F.Zero;
		}

		private void OnUpdateIdleState() {
			// Update eye angle
			int angleToPlayer = Angles.NearestFromVector(
				RoomControl.Player.Center - Center);
			eyeAngle = angleToPlayer;
			Graphics.SubStripIndex = eyeAngle;

			// TODO: after raising, eye will not change back until NOT looking down.
			// TODO: slight delay after raising

			// Check for crushing
			if (Entity.AreEntitiesAligned(this, RoomControl.Player,
				Directions.Down, GameSettings.MONSTER_THWOMP_CRUSH_MIN_ALIGNMENT))
			{
				stateMachine.BeginState(CrushState.Crush);
			}
		}
		
		private void OnBeginCrushState() {
			crushSpeed = GameSettings.MONSTER_THWOMP_CRUSH_INITIAL_SPEED;
			Graphics.PlayAnimation(GameData.ANIM_MONSTER_THWOMP_CRUSH);
		}

		private void OnUpdateCrushState() {
			// Move down
			Physics.VelocityY = crushSpeed;

			// Accelerate crush speed
			if (crushSpeed < GameSettings.MONSTER_THWOMP_CRUSH_MAX_SPEED) {
				crushSpeed = GMath.Min(crushSpeed +
					GameSettings.MONSTER_THWOMP_CRUSH_ACCELERATION,
					GameSettings.MONSTER_THWOMP_CRUSH_MAX_SPEED);
			}

			// Stop crushing upon hitting a solid tile
			// TODO: Properly snap position to be flush with surface.
			// TODO: collide with room edge too
			Rectangle2F crushBox = new Rectangle2F(-16, -16, 32, 32);
			if (Physics.IsPlaceMeetingSolid(position, crushBox)) {
				stateMachine.BeginState(CrushState.Hit);
			}
		}

		private void OnBeginHitState() {
			AudioSystem.PlaySound(GameData.SOUND_BARRIER);
			physics.Velocity = Vector2F.Zero;
		}
		
		private void OnUpdateRaiseState() {
			// Move up until we reach our wait position
			physics.VelocityY = -GameSettings.MONSTER_THWOMP_RAISE_SPEED;
			if (position.Y <= hoverPosition.Y)
				stateMachine.BeginState(CrushState.Idle);
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------
		
		public override void Initialize() {
			base.Initialize();

			hoverPosition = position;
			eyeAngle = Angles.Down;
			Graphics.PlayAnimation(GameData.ANIM_MONSTER_THWOMP);

			stateMachine.BeginState(CrushState.Idle);
		}

		public override void UpdateAI() {
			stateMachine.Update();
		}
	}
}
