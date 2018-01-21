using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Game.Entities.Monsters.JumpMonsters {
	public class MonsterTektike : JumpMonster {

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public MonsterTektike() {
			color			= MonsterColor.Orange;
			MaxHealth		= 1;
			ContactDamage	= 2;

			Physics.Gravity	= 0.08f;
			moveSpeed		= 1.0f;
			jumpSpeed		= new RangeF(2.0f);
			stopTime		= new RangeI(80, 120);
			stopAnimation	= GameData.ANIM_MONSTER_TEKTIKE;
			jumpAnimation	= GameData.ANIM_MONSTER_TEKTIKE_JUMP;
		}
		
		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void Initialize() {
			base.Initialize();
			
			if (color == MonsterColor.Orange) {
				MaxHealth = 1;
			}
			else {
				MaxHealth = 2;
			}
		}
	}
}
