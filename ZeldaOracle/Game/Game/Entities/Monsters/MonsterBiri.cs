using System;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Entities.Monsters.States;

namespace ZeldaOracle.Game.Entities.Monsters {
	
	public class MonsterBiri : Monster {

		private int numMoveAngles;
		private float moveSpeed;
		private int angleDuration;

		private int moveAngle;
		private int moveTimer;
		private int hoverTimer;

		
		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public MonsterBiri() {
			// General
			MaxHealth		= 1;
			ContactDamage	= 2;
			Color			= MonsterColor.Blue;
			
			// Movement
			numMoveAngles				= 24;
			moveSpeed					= 0.25f;
			angleDuration				= 16;

			// Physics
			Physics.Gravity					= 0.0f;
			Physics.CollideWithWorld		= false;
			physics.DisableSurfaceContact	= true;
			Physics.ReboundRoomEdge			= true;

			// Weapon Interactions
			Interactions.SetReaction(InteractionType.Sword,			Reactions.Kill);
			Interactions.SetReaction(InteractionType.SwordSpin,		Reactions.Kill);
			Interactions.SetReaction(InteractionType.BiggoronSword,	Reactions.Kill);

			// Projectile Interactions
			Interactions.SetReaction(InteractionType.EmberSeed,		SenderReactions.Destroy, Reactions.Kill);
			Interactions.SetReaction(InteractionType.ScentSeed,		SenderReactions.Destroy, Reactions.Kill);
			Interactions.SetReaction(InteractionType.PegasusSeed,	SenderReactions.Destroy, Reactions.Kill);
			Interactions.SetReaction(InteractionType.Arrow,			SenderReactions.Destroy, Reactions.Kill);
			Interactions.SetReaction(InteractionType.SwordBeam,		SenderReactions.Destroy, Reactions.Kill);
			Interactions.SetReaction(InteractionType.SwitchHook,		SenderReactions.Intercept, Reactions.Kill);
			Interactions.SetReaction(InteractionType.BombExplosion,	Reactions.Kill);
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

		
		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void Initialize() {
			base.Initialize();

			moveAngle = GRandom.NextInt(numMoveAngles);
			moveTimer = 0;
			hoverTimer = 0;
			graphics.PlayAnimation(GameData.ANIM_MONSTER_BIRI);
		}

		public override void UpdateAI() {
			// Steer toward the player
			SteerTowardPoint(RoomControl.Player.Position);

			// Update hovering
			hoverTimer++;
			int[] hoverZPositions = new int[] { 1, 2, 3, 2 };
			zPosition = hoverZPositions[(hoverTimer / 16) %
				hoverZPositions.Length];
		}
	}
}
