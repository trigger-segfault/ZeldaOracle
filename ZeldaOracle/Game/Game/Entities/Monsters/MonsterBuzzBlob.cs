using System;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Entities.Monsters.States;

namespace ZeldaOracle.Game.Entities.Monsters {

	public class MonsterBuzzBlob : BasicMonster {

		private int electrocuteDelayTimer;

		public MonsterBuzzBlob() {
			// General.
			MaxHealth		= 1;
			ContactDamage	= 2;
			color			= MonsterColor.Green;
			
			// Movement.
			moveSpeed					= 0.25f;
			numMoveAngles				= 8;
			//isMovementDirectionBased = true;
			orientationStyle			= OrientationStyle.Angle;
			changeDirectionsOnCollide	= true;
			movesInAir					= false;
			facePlayerOdds				= 0;
			stopTime.Set(0, 0);
			moveTime.Set(50, 80);
			
			// Physics.
			Physics.Bounces			= true; // This is here for when the monster is digged up or dropped from the ceiling.
			Physics.ReboundRoomEdge	= true;
			Physics.ReboundSolid	= true;
			
			// Graphics.
			animationMove				= GameData.ANIM_MONSTER_BUZZ_BLOB;
			syncAnimationWithDirection	= false;
			//animationMove				= GameData.ANIM_MONSTER_BEAMOS;
			//syncAnimationWithDirection	= true;
			
			// Interactions (Tools)
			SetReaction(InteractionType.Sword,			SenderReactions.Damage, Reactions.Electrocute);
			SetReaction(InteractionType.SwordSpin,		SenderReactions.Damage, Reactions.Electrocute);
			SetReaction(InteractionType.BiggoronSword,	SenderReactions.Damage, Reactions.Electrocute);
			// Interactions (Projectiles)
			SetReaction(InteractionType.Arrow,			SenderReactions.Intercept, Reactions.Damage);
			SetReaction(InteractionType.SwordBeam,		SenderReactions.Intercept, Reactions.Kill);
			SetReaction(InteractionType.SwitchHook,		SenderReactions.Intercept, SenderReactions.Damage, Reactions.Electrocute);
			SetReaction(InteractionType.MysterySeed,	SenderReactions.Intercept, TransformIntoCukeman);
		}

		public override void Initialize() {
			base.Initialize();
			electrocuteDelayTimer = 0;
		}

		private void TransformIntoCukeman(Monster monster, Entity sender, EventArgs args) {
			MonsterCukeman cukeman = new MonsterCukeman() {
				Position = position,
				ZPosition = ZPosition,
			};
			DestroyAndTransform(cukeman);
			RoomControl.SpawnEntity(cukeman);
		}

		public void OnElectrocute() {
			if (electrocuteDelayTimer == 0) {
				BeginState(new MonsterElectrocuteState(
					GameData.ANIM_MONSTER_BUZZ_BLOB_ELECTROCUTE));
				electrocuteDelayTimer = 40;
			}
			//electrocuteTimer = 0;
			//isElectrocuting = true;
			//Graphics.PlayAnimation(GameData.ANIM_MONSTER_BUZZ_BLOB_ELECTROCUTE);
			//isPassable = true;
			//StopMoving();
		}

		public override void UpdateAI() {
			base.UpdateAI();

			if (!(CurrentState is MonsterElectrocuteState) &&
				electrocuteDelayTimer > 0)
			{
				electrocuteDelayTimer--;
			}

			//electrocuteTimer++;
			//if (electrocuteTimer > 60) {
			//	isElectrocuting = false;
			//	Graphics.PlayAnimation(GameData.ANIM_MONSTER_BUZZ_BLOB);
			//	isPassable = false;
			//}
		}
	}
}
