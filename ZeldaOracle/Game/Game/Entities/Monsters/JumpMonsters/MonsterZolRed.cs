using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Game.Entities.Monsters.JumpMonsters {

	public class MonsterZolRed : JumpMonster {

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public MonsterZolRed() {
			Color			= MonsterColor.Red;
			MaxHealth		= 1;
			ContactDamage	= 1;
			softKill		= true; // Offspring will perform the "hard" kill

			// Red Zol observations:
			//   * 26 frames of stopped between movements
			//   * 16 frames of movement at 0.5 speed
			//   * 26 frames after moving before shaking
			//   * 32 frames of shaking
			//   * 26 frames after landing before moving again
			//   * 19 frames after death before Gels spawn
			//   - Once one Gel is dead, the Zol is considered Dead (wont respawn)
			// Conclusion:
			//   * stop time = 26
			//   * crawl time = 16
			//   * prepare jump time = 32

			jumpOdds				= 6; // Needs confirmation
			crawlTime				= new RangeI(16);
			crawlSpeed				= 0.5f;
			stopTime				= new RangeI(26);
			jumpSpeed				= new RangeF(2.0f);
			prepareJumpTime			= new RangeI(32);
			moveSpeed				= 1.0f;
			stopAnimation			= GameData.ANIM_MONSTER_ZOL_RED;
			prepareJumpAnimation	= GameData.ANIM_MONSTER_ZOL_RED_PREPARE_JUMP;
			jumpAnimation			= GameData.ANIM_MONSTER_ZOL_JUMP;
			
			// Weapon interactions
			Interactions.SetReaction(InteractionType.Sword,			MonsterReactions.Kill);
			Interactions.SetReaction(InteractionType.SwordSpin,		MonsterReactions.Kill);
			Interactions.SetReaction(InteractionType.BiggoronSword,	MonsterReactions.Kill);
			// Projectile interactions
			Interactions.SetReaction(InteractionType.Arrow,			SenderReactions.Intercept, MonsterReactions.Kill);
			Interactions.SetReaction(InteractionType.SwordBeam,		SenderReactions.Intercept, MonsterReactions.Kill);
			Interactions.SetReaction(InteractionType.SwitchHook,		SenderReactions.Intercept, MonsterReactions.Kill);
			// Misc interactions
			Interactions.SetReaction(InteractionType.Block,			MonsterReactions.Kill);
			Interactions.SetReaction(InteractionType.ThrownObject,	MonsterReactions.Kill);
		}


		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------
		
		private void SpawnOffspring(Vector2F position) {
			MonsterGel child = new MonsterGel();
			child.Color = MonsterColor.Red;
			child.Properties = Properties;
			RoomControl.SpawnEntity(child, position);
		}

		private void SpawnOffspring() {
			SpawnOffspring(position + new Vector2F(4, 0));
			SpawnOffspring(position - new Vector2F(4, 0));
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void OnDie() {
			RoomControl.ScheduleEvent(19, SpawnOffspring);
		}
	}
}
