using System;
using System.Collections.Generic;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Game.Entities.Players;

namespace ZeldaOracle.Game.Entities.Monsters {
	
	public class MonsterMiniMoldorm : Monster {
		
		private const int NUM_BODY_PARTS = 2;
		private const int MAX_HISTORY = 20;
		private Vector2F[] bodyPositions;
		private ISprite[] bodySprites;
		private List<Vector2F> positionHistory;
		private int timer;
		private float rotationSpeed;
		private float moveSpeed;
		private Vector2F moveVector;

		
		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public MonsterMiniMoldorm() {
			positionHistory = new List<Vector2F>();
			bodyPositions = new Vector2F[NUM_BODY_PARTS];
			bodySprites = new ISprite[NUM_BODY_PARTS];
			bodySprites[0] = GameData.SPR_MONSTER_MINI_MOLDORM_BODY_SEGMENT_LARGE;
			bodySprites[1] = GameData.SPR_MONSTER_MINI_MOLDORM_BODY_SEGMENT_SMALL;

			// General
			MaxHealth		= 4;
			ContactDamage	= 2;
			Color			= MonsterColor.Green;
			
			// Movement
			moveSpeed		= 1.0f;
			isFlying		= true;
			Physics.Flags |=
				PhysicsFlags.ReboundSolid |
				PhysicsFlags.ReboundRoomEdge;
			
			// Graphics
			Graphics.DrawOffset			= new Point2I(-8, -8);
			centerOffset				= new Point2I(0, 0);
			syncAnimationWithDirection	= true;

			// Physics
			Physics.HasGravity			= false;
			Physics.IsDestroyedInHoles	= false;
			Physics.CollisionBox		= new Rectangle2F(-6, -6, 12, 12);
			Physics.SoftCollisionBox	= Physics.CollisionBox.Inflated(-2, -2);

			// Projectile Interactions
			SetReaction(InteractionType.Gale,			SenderReactions.Intercept, Reactions.None);
			SetReaction(InteractionType.GaleSeed,		SenderReactions.Intercept, Reactions.None);
			SetReaction(InteractionType.PegasusSeed,	SenderReactions.Intercept, Reactions.None);
			SetReaction(InteractionType.ScentSeed,		SenderReactions.Intercept, Reactions.None);
			SetReaction(InteractionType.Fire,			SenderReactions.Intercept, Reactions.None);
			SetReaction(InteractionType.RodFire,		SenderReactions.Intercept, Reactions.None);
			SetReaction(InteractionType.Boomerang,		SenderReactions.Intercept, Reactions.ClingEffect);
			SetReaction(InteractionType.SwitchHook,		SenderReactions.Intercept, Reactions.Damage);
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void Initialize() {
			base.Initialize();
			
			Direction = Directions.Up;

			// Start moving in a random angle
			float randomAngle = GMath.TwoPi * GRandom.NextFloat();
			moveVector.X = (float) Math.Cos(randomAngle);
			moveVector.Y = (float) Math.Sin(randomAngle);
			rotationSpeed = 0.05f;
			if (GRandom.NextBool())
				rotationSpeed *= -1.0f;

			timer = 0;
			positionHistory.Clear();
			for (int i = 0; i < NUM_BODY_PARTS; i++)
				bodyPositions[i] = position;
			for (int i = 0; i < MAX_HISTORY; i++)
				positionHistory.Add(position);
			Graphics.PlayAnimation(GameData.ANIM_MONSTER_MINI_MOLDORM_HEAD);
		}

		public override void UpdateAI() {
			// Check for rebound collisions to update movement vector.
			// We cannot rely on using Physics.Velocity because facing direction does 
			// not always sync with physics velocity.
			for (int i = 0; i < 4; i++) {
				int axis = Directions.ToAxis(i);
				if (physics.IsCollidingInDirection(i))
					moveVector[axis] = -moveVector[axis];
			}

			// Rotate velocity
			float angle = rotationSpeed;
			float cosAngle = (float) Math.Cos(angle);
			float sinAngle = (float) Math.Sin(angle);
			float x = (moveVector.X * cosAngle) - (moveVector.Y * sinAngle);
			float y = (moveVector.Y * cosAngle) + (moveVector.X * sinAngle);
			moveVector.X = x;
			moveVector.Y = y;
			moveVector = moveVector.Normalized * moveSpeed;

			physics.Velocity = moveVector;
			Angle = Angles.NearestFromVector(moveVector);

			// Reverse rotation direction regularly
			timer++;
			if (timer > 60 && GRandom.NextInt(60) == 0) {
				rotationSpeed *= -1.0f;
				timer = 0;
			}
		}

		public override void Update() {
			base.Update();
			
			for (int i = 0; i < NUM_BODY_PARTS; i++) {
				bodyPositions[i] = positionHistory[MAX_HISTORY - ((i + 1) * 7)];
			}

			positionHistory.Add(position);
			positionHistory.RemoveAt(0);
		}

		public override void Draw(RoomGraphics g) {
			// Draw body segments
			SpriteDrawSettings drawSettings = new SpriteDrawSettings() {
				Colors = Graphics.ModifiedColorDefinitions
			};
			for (int i = NUM_BODY_PARTS - 1; i >= 0; i--) {
				g.DrawSprite(bodySprites[i], drawSettings,
					bodyPositions[i], Graphics.DepthLayer);
			}

			// Draw head/eyes
			base.Draw(g);
		}
	}
}
