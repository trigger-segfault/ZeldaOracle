using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Game.Entities.Monsters {

	public class MonsterKeese : BasicMonster {
		
		private enum KeeseState {
			Flying,
			Stopping,
			Stopped,
		}

		private int flyTimer;
		private KeeseState keeseState;
		private RangeI flyStopTime;
		private RangeI flyMoveTime;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------
		
		public MonsterKeese() {
			// General
			MaxHealth		= 1;
			ContactDamage	= 1;
			color			= MonsterColor.Green;
			
			// Keese
			flyStopTime = new RangeI(60, 180);
			flyMoveTime = new RangeI(420, 600);

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
			Physics.Gravity			= 0.0f;
			Physics.ReboundRoomEdge	= true;
			Physics.ReboundSolid	= true;
			
			// Graphics
			animationMove				= GameData.ANIM_MONSTER_KEESE;
			//Graphics.DrawOffset		= new Point2I(-8, -14) + new Point2I(0, 3);
			//centerOffset				= new Point2I(0, -6) + new Point2I(0, 3);

			// Projectile interactions
			SetReaction(InteractionType.SwitchHook, SenderReactions.Intercept, Reactions.Damage);
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void Initialize() {
			base.Initialize();

			speed = 0.0f;
			flyTimer = 40;
			keeseState = KeeseState.Stopped;
			Graphics.PlayAnimation(GameData.ANIM_MONSTER_KEESE_STOP);
		}

		public override void UpdateAI() {
			
			if (keeseState == KeeseState.Flying) {
				// Speed up
				speed = GMath.Min(speed + GameSettings.MONSTER_KEESE_ACCELERATION, 0.75f);
				Graphics.AnimationPlayer.Speed = 
					(speed / moveSpeed) * 0.5f + 0.5f;

				base.UpdateAI();

				flyTimer--;
				if (flyTimer <= 0) {
					keeseState = KeeseState.Stopping;
				}
			}
			else if (keeseState == KeeseState.Stopping) {
				if (speed > 0.0f) {
					// Slow down to a stop
					speed -= GameSettings.MONSTER_KEESE_DECELERATION;
				}
				else {
					flyTimer = GRandom.NextInt(flyStopTime);
					keeseState = KeeseState.Stopped;
					physics.Velocity = Vector2F.Zero;
					Graphics.PlayAnimation(GameData.ANIM_MONSTER_KEESE_FLAP);
					Graphics.AnimationPlayer.Speed = 1.0f;
					return;
				}
				Graphics.AnimationPlayer.Speed =
					(speed / moveSpeed) * 0.5f + 0.5f;
				base.UpdateAI();
			}
			else if (keeseState == KeeseState.Stopped) {
				flyTimer--;
				if (flyTimer <= 0) {
					flyTimer = GRandom.NextInt(flyMoveTime);
					keeseState = KeeseState.Flying;
					Graphics.PlayAnimation(GameData.ANIM_MONSTER_KEESE);
					StartMoving();
					speed = 0.0f;
				}
			}
		}
	}
}
