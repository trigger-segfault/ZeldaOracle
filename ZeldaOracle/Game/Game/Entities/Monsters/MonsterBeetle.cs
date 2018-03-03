using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Game.Entities.Monsters {
	public class MonsterBeetle : BasicMonster {
		public MonsterBeetle() {
			// General.
			MaxHealth		= 1;
			ContactDamage	= 1;
			Color			= MonsterColor.Green;
			
			// Movement.
			moveSpeed					= 0.5f;
			//numMoveAngles				= 8;
			//isMovementDirectionBased	= true;
			changeDirectionsOnCollide	= true;
			movesInAir					= false;
			stopTime.Set(0, 0);
			moveTime.Set(50, 80);
								
			// Physics.
			Physics.Bounces			= true; // This is here for when the monster is digged up or dropped from the ceiling.
			Physics.ReboundRoomEdge	= true;
			Physics.ReboundSolid	= true;
			
			// Graphics.
			animationMove				= GameData.ANIM_MONSTER_BEETLE;
			Graphics.DrawOffset			= new Point2I(-8, -14) + new Point2I(0, 3);
			centerOffset				= new Point2I(0, -6) + new Point2I(0, 3);
			syncAnimationWithDirection	= true;

			// Interactions.
			Interactions.SetReaction(InteractionType.SwitchHook, SenderReactions.Intercept, MonsterReactions.Damage);
		}
	}
}
