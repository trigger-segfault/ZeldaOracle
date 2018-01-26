using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Game.Entities.Monsters {

	public class MonsterStalfos : BasicMonster {


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------
		
		public MonsterStalfos() {
			// General
			MaxHealth		= 1;
			ContactDamage	= 2;
			color			= MonsterColor.Blue;

			// Movement
			moveSpeed					= 0.5f;
			numMoveAngles				= 16;
			facePlayerOdds				= 0;
			changeDirectionsOnCollide	= true;
			stopTime.Set(0, 0);
			moveTime.Set(30, 80);
								
			// Physics
			Physics.Gravity			= 0.0f;
			Physics.ReboundRoomEdge	= true;
			Physics.ReboundSolid	= true;
			
			// Graphics
			animationMove = GameData.ANIM_MONSTER_STALFOS;
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void Initialize() {
			base.Initialize();

			if (color == MonsterColor.Blue) {
				MaxHealth		= 1;
				ContactDamage	= 2;
			}
			else if (color == MonsterColor.Orange) {
				MaxHealth		= 1;
				ContactDamage	= 2;
			}
			else if (color == MonsterColor.Green) {
				MaxHealth		= 1;
				ContactDamage	= 2;
			}
			else if (color == MonsterColor.Red) {
				MaxHealth		= 1;
				ContactDamage	= 2;
			}

			Health = MaxHealth;
		}

		public override void UpdateAI() {
			base.UpdateAI();

		}
	}
}
