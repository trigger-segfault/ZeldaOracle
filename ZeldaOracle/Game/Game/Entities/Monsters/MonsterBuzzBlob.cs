using System;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Game.Entities.Monsters {

	public class MonsterBuzzBlob : BasicMonster {
		
		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public MonsterBuzzBlob() {
			// General
			MaxHealth		= 1;
			ContactDamage	= 2;
			Color			= MonsterColor.Green;
			
			// Movement
			moveSpeed					= 0.25f;
			numMoveAngles				= 8;
			changeDirectionsOnCollide	= true;
			movesInAir					= false;
			facePlayerOdds				= 0;
			stopTime.Set(0, 0);
			moveTime.Set(50, 80);
			
			// Graphics
			animationMove				= GameData.ANIM_MONSTER_BUZZ_BLOB;
			syncAnimationWithDirection	= false;
			
			// Physics
			Physics.Bounces				= true; // This is here for when the monster is digged up or dropped from the ceiling
			Physics.ReboundRoomEdge		= true;
			Physics.ReboundSolid		= true;
			
			// Interactions
			Interactions.InteractionBox = new Rectangle2I(-4, -10, 8, 9);
			// Interactions (Tools)
			Interactions.SetReaction(InteractionType.Sword,			MonsterReactions.Electrocute);
			Interactions.SetReaction(InteractionType.SwordSpin,		MonsterReactions.Electrocute);
			Interactions.SetReaction(InteractionType.BiggoronSword,	MonsterReactions.Electrocute);
			// Interactions (Projectiles)
			Interactions.SetReaction(InteractionType.Arrow,			SenderReactions.Intercept, MonsterReactions.Damage);
			Interactions.SetReaction(InteractionType.SwordBeam,		SenderReactions.Intercept, MonsterReactions.Kill);
			Interactions.SetReaction(InteractionType.SwitchHook,		SenderReactions.Intercept, MonsterReactions.Electrocute);
			Interactions.SetReaction(InteractionType.MysterySeed,	SenderReactions.Intercept, TransformIntoCukeman);
			Interactions.SetReaction(InteractionType.BombExplosion,	MonsterReactions.Kill);
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		private void TransformIntoCukeman(Entity sender, EventArgs args) {
			MonsterCukeman cukeman = new MonsterCukeman() {
				Position = position,
				ZPosition = ZPosition,
			};
			DestroyAndTransform(cukeman);
			RoomControl.SpawnEntity(cukeman);
		}

		public override void OnElectrocute() {
			Graphics.PlayAnimation(GameData.ANIM_MONSTER_BUZZ_BLOB_ELECTROCUTE);
		}

		public override void OnElectrocuteComplete() {
			StartMoving();
		}
	}
}
