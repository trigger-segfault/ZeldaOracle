using System;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Util;
using ZeldaOracle.Game.Entities.Monsters.States;

namespace ZeldaOracle.Game.Entities.Monsters {
	
	/// <summary>
	/// Bari
	/// Movement: Gaurds a random tile, circular motions around the tile
	/// Moves for 90, 120, or 150 frames
	/// Electrocutes for 60 frames
	/// Similar to Red Zol
	/// Spawn 2 biri, 5 frames after death effect (not if over-killed with master sword)
	/// Bari won't be "Dead" until at least one biri is dead
	/// </summary>
	public class MonsterBari : Monster {
		
		private enum BariState {
			Moving,
			Electrecuting,
		}

		private GenericStateMachine<BariState> stateMachine;

		private Vector2F guardPoint;
		private int numMoveAngles;
		private float moveSpeed;
		private int angleDuration;

		private int moveAngle;
		private int moveTimer;
		private int stateTimer;
		private int hoverTimer;
		//private WindingOrder rotationDirection;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public MonsterBari() {
			// General
			MaxHealth		= 2;
			ContactDamage	= 2;
			Color			= MonsterColor.Blue;
			
			// Movement
			moveSpeed					= 0.35f;
			numMoveAngles				= 24;
			angleDuration				= 16;

			// Physics
			Physics.Gravity					= 0.0f;
			Physics.CollideWithWorld		= false;
			Physics.ReboundRoomEdge			= true;
			physics.DisableSurfaceContact	= true;

			// Weapon Interactions
			Interactions.SetReaction(InteractionType.Sword,			KillOrElectrocute);
			Interactions.SetReaction(InteractionType.SwordSpin,		KillOrElectrocute);
			Interactions.SetReaction(InteractionType.BiggoronSword,	KillOrElectrocute);

			// Projectile Interactions
			Interactions.SetReaction(InteractionType.EmberSeed,		SenderReactions.Destroy, MonsterReactions.Kill);
			Interactions.SetReaction(InteractionType.ScentSeed,		SenderReactions.Destroy, MonsterReactions.Kill);
			Interactions.SetReaction(InteractionType.PegasusSeed,	SenderReactions.Destroy, MonsterReactions.Kill);
			Interactions.SetReaction(InteractionType.Arrow,			SenderReactions.Destroy, MonsterReactions.Kill);
			Interactions.SetReaction(InteractionType.SwordBeam,		SenderReactions.Intercept, MonsterReactions.Kill);
			Interactions.SetReaction(InteractionType.SwitchHook,		SenderReactions.Intercept, KillOrElectrocute);
			Interactions.SetReaction(InteractionType.BombExplosion,	MonsterReactions.Kill);

			// Behavior
			stateMachine = new GenericStateMachine<BariState>();
			stateMachine.AddState(BariState.Moving)
				.OnBegin(OnBeginMovingBehavior)
				.OnUpdate(OnUpdateMovingBehavior)
				.SetDuration(GameSettings.MONSTER_BARI_ELECTROCUTE_DELAYS[2]);
			stateMachine.AddState(BariState.Electrecuting)
				.OnBegin(OnBeginElectrecuteState)
				.OnEnd(OnEndElectrecuteState)
				.OnUpdate(OnUpdateElectrecuteState)
				.SetDuration(GameSettings.MONSTER_BARI_ELECTROCUTE_DURATION);
		}
		
		
		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------

		public void SteerTowardPoint(Vector2F point) {
			// TODO: this code is duplicated between MonsterBari and MonsterBiri

			moveTimer++;

			if (moveTimer >= angleDuration) {
				moveTimer = 0;
				
				// Determine destination angle and shortest distance to that angle
				int angleTowardPlayer = Orientations.NearestFromVector(
					point - position, numMoveAngles);
				int angleDistance = Orientations.GetNearestAngleDistance(
					moveAngle, angleTowardPlayer, numMoveAngles);

				// If the destination is directly behind us, then choose choose
				// randomly clockwise or counter-clockwise
				if (GMath.Sign(angleDistance) == numMoveAngles / 2 && GRandom.NextBool())
					angleDistance = -angleDistance;

				// Update the movement angle
				if (angleDistance > 0)
					moveAngle = (moveAngle + 1) % numMoveAngles;
				else if (angleDistance < 0)
					moveAngle = (moveAngle + numMoveAngles - 1) % numMoveAngles;
			}
			
			Physics.Velocity = Orientations.ToVector(
				moveAngle, numMoveAngles) * moveSpeed;
		}

		private void SpawnOffspring(Vector2F position) {
			MonsterBiri child = new MonsterBiri();
			RoomControl.SpawnEntity(child, position);
			child.GiveInvincibility(GameSettings.MONSTER_HURT_INVINCIBLE_DURATION);
		}

		private void SpawnOffspring() {
			SpawnOffspring(position + new Vector2F(4, 0));
			SpawnOffspring(position - new Vector2F(4, 0));
		}


		//-----------------------------------------------------------------------------
		// Behavior
		//-----------------------------------------------------------------------------

		private void OnBeginElectrecuteState() {
			stateTimer = 0;
			Physics.Velocity = Vector2F.Zero;
			Graphics.PlayAnimation(GameData.ANIM_MONSTER_BARI_ELECTROCUTE);
			Color = MonsterColor.Blue;
		}
		
		private void OnEndElectrecuteState() {
			Color = MonsterColor.Blue;
		}

		private void OnUpdateElectrecuteState() {
			Physics.Velocity = Vector2F.Zero;

			// Update electrecution sprite
			stateTimer++;
			Graphics.SetAnimation(GameData.ANIM_MONSTER_BARI_ELECTROCUTE);
			if ((stateTimer / 4) % 2 == 0) {
				Color = MonsterColor.Blue;
			}
			else {
				Color = MonsterColor.InverseBlue;
				Graphics.ColorDefinitions.SetAll("inverse_blue");
			}
		}

		private void OnBeginMovingBehavior() {
			Graphics.PlayAnimation(GameData.ANIM_MONSTER_BARI);
			moveAngle = GRandom.NextInt(24);
			stateTimer = 0;
			
			moveAngle = GRandom.NextInt(numMoveAngles);
			moveTimer = 0;
		}

		private void OnUpdateMovingBehavior() {
			//float radians = ((float) moveAngle / (float) numMoveAngles) * GMath.TwoPi;
			//Physics.Velocity = new Vector2F(
			//	(float) Math.Cos(radians) * moveSpeed,
			//	(float) -Math.Sin(radians) * moveSpeed);

			SteerTowardPoint(guardPoint);
			//stateTimer++;
			//if (stateTimer > 16) {
			//	stateTimer = 0;
			//	if (rotationDirection == WindingOrder.Clockwise)
			//		moveAngle = (moveAngle + numMoveAngles - 1) % numMoveAngles;
			//	else if (rotationDirection == WindingOrder.CounterClockwise)
			//		moveAngle = (moveAngle + 1) % numMoveAngles;
			//}
		}


		//-----------------------------------------------------------------------------
		// Interactions
		//-----------------------------------------------------------------------------

		private void KillOrElectrocute(Entity sender, EventArgs args) {
			if (stateMachine.CurrentState == BariState.Electrecuting)
				MonsterReactions.Electrocute(this, sender, args);
			else
				MonsterReactions.Kill(this, sender, args);
		}

		public override void OnTouchPlayer(Entity sender, EventArgs args) {
			if (stateMachine.CurrentState == BariState.Electrecuting)
				MonsterReactions.Electrocute(this, sender, args);
			else
				base.OnTouchPlayer(sender, args);
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void Initialize() {
			base.Initialize();

			stateMachine.BeginState(BariState.Moving);
			hoverTimer = 0;
			zPosition = 1;

			guardPoint = position;
		}

		public override void OnDie() {
			RoomControl.ScheduleEvent(5, SpawnOffspring);
		}

		public override void UpdateAI() {
			stateMachine.Update();

			// Update hovering
			hoverTimer++;
			int[] hoverZPositions = new int[] { 1, 2, 3, 2 };
			zPosition = hoverZPositions[(hoverTimer / 16) %
				hoverZPositions.Length];
		}
	}
}
