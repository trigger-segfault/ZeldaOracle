using System;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Audio;

namespace ZeldaOracle.Game.Entities.Monsters {

	public class MonsterThwomp : Monster {

		private enum CrushState {
			Idle,	// Waiting for player to come near
			Crush,	// Accelearting down to the ground
			Hit,	// Hit the ground, screen is shaking
			Raise,	// Moving up back to idle positio.
		}

		private CrushState crushState;
		private Vector2F hoverPosition;
		float crushSpeed;
		int hitWaitTimer;
		int eyeAngle;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public MonsterThwomp() {
			// General.
			ContactDamage	= 4;
			Color			= MonsterColor.DarkBlue;
			isDamageable	= false;
			isBurnable		= false;
			isStunnable		= false;
			isGaleable		= false;
			IsKnockbackable	= false;

			Physics.HasGravity = false;

			centerOffset				= Point2I.Zero;
			Graphics.DrawOffset			= new Point2I(-8, -8);
			Physics.CollisionBox		= new Rectangle2F(-8, -8, 32, 32);
			Physics.SoftCollisionBox	= new Rectangle2F(-8, -8, 32, 32).Inflated(-2, -2);
			Physics.IsSolid				= true;

			// Interactions (Tools)
			SetReaction(InteractionType.Sword,			SenderReactions.Bump, Reactions.ClingEffect);
			SetReaction(InteractionType.SwordSpin,		SenderReactions.Bump, Reactions.ClingEffect);
			SetReaction(InteractionType.BiggoronSword,	Reactions.ClingEffect);
			SetReaction(InteractionType.Shovel,			Reactions.ClingEffect);
			// Interactions (Projectiles)
			SetReaction(InteractionType.Arrow,			SenderReactions.Intercept);
			SetReaction(InteractionType.SwordBeam,		SenderReactions.Intercept);
			SetReaction(InteractionType.Boomerang,		SenderReactions.Intercept, Reactions.ClingEffect);
			SetReaction(InteractionType.SwitchHook,		SenderReactions.Intercept, Reactions.ClingEffect);
						
			SetReaction(InteractionType.Sword,			Reactions.ParryWithClingEffect);
			SetReaction(InteractionType.SwordSpin,		Reactions.ParryWithClingEffect);
			SetReaction(InteractionType.BiggoronSword,	Reactions.ClingEffect);
			SetReaction(InteractionType.Shovel,			Reactions.ClingEffect);
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------
		
		public override void Initialize() {
			base.Initialize();
			Graphics.PlayAnimation(GameData.ANIM_MONSTER_THWOMP);
			crushState = CrushState.Idle;
			hoverPosition = position;
			eyeAngle = Angles.Down;
			Graphics.PlayAnimation(GameData.ANIM_MONSTER_THWOMP);
		}

		public override void UpdateAI() {
			
			if (crushState == CrushState.Raise) {
				// Move up until we reach our wait position
				position.Y -= GameSettings.MONSTER_THWOMP_RAISE_SPEED;
				if (position.Y <= hoverPosition.Y) {
					crushState = CrushState.Idle;
					position = hoverPosition;
					Graphics.PlayAnimation(GameData.ANIM_MONSTER_THWOMP);
				}
			}
			else  if (crushState == CrushState.Crush) {
				// Move down
				position.Y += crushSpeed;

				// Accelerate crush speed
				if (crushSpeed < GameSettings.MONSTER_THWOMP_CRUSH_MAX_SPEED) {
					crushSpeed = Math.Min(crushSpeed +
						GameSettings.MONSTER_THWOMP_CRUSH_ACCELERATION,
						GameSettings.MONSTER_THWOMP_CRUSH_MAX_SPEED);
				}

				// TODO: Stop crushing upon hitting a solid tile
				if (position.Y >= hoverPosition.Y + GameSettings.TILE_SIZE * 4) {
					crushState = CrushState.Hit;
					hitWaitTimer = 0;
					AudioSystem.PlaySound(GameData.SOUND_BARRIER);
				}
			}
			else  if (crushState == CrushState.Hit) {
				// Wait a bit before we raise back up
				hitWaitTimer++;
				if (hitWaitTimer > GameSettings.MONSTER_THWOMP_HIT_WAIT_DURATION) {
					crushState = CrushState.Raise;
				}
			}
			else {
				// Update eye angle.
				int angleToPlayer = Angles.NearestFromVector(
					RoomControl.Player.Center - Center);
				eyeAngle = angleToPlayer;
				Graphics.SubStripIndex = eyeAngle;
				// TODO: after raising, eye will not change back until not looking down.
				// TODO: slight delay after raising

				// Check for crushing
				if (Entity.AreEntitiesAligned(this, RoomControl.Player,
					Directions.Down, GameSettings.MONSTER_THWOMP_CRUSH_MIN_ALIGNMENT))
				{
					crushState = CrushState.Crush;
					crushSpeed = GameSettings.MONSTER_THWOMP_CRUSH_INITIAL_SPEED;
					Graphics.PlayAnimation(GameData.ANIM_MONSTER_THWOMP_CRUSH);
				}
			}
		}
	}
}
