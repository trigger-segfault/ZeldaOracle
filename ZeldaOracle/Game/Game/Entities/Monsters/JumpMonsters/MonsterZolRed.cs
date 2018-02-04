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

			// Red Zol observations:
			//   * 26 frames of stopped between movements
			//   * 16 frames of movement at 0.5 speed
			//   * 26 frames after moving before shaking
			//   * 32 frames of shaking
			//   * 26 frames after landing before moving again
			//   * 19 frames after death before Gels spawn
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
			SetReaction(InteractionType.Sword,			Reactions.Kill);
			SetReaction(InteractionType.SwordSpin,		Reactions.Kill);
			SetReaction(InteractionType.BiggoronSword,	Reactions.Kill);
			// Projectile interactions
			SetReaction(InteractionType.Arrow,			SenderReactions.Intercept, Reactions.Kill);
			SetReaction(InteractionType.SwordBeam,		SenderReactions.Intercept, Reactions.Kill);
			SetReaction(InteractionType.SwitchHook,		SenderReactions.Intercept, Reactions.Kill);
			// Misc interactions
			SetReaction(InteractionType.Block,			Reactions.Kill);
			SetReaction(InteractionType.ThrownObject,	Reactions.Kill);
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void OnDie() {
			// Spawn gels
			// TODO: These need to spawn 19 frames after death (they also need to
			// start with a short invulnerable delay so they aren't immediatly killed)
			RoomControl.SpawnEntity(new MonsterGel() {
				Color = MonsterColor.Red,
				Position = position - new Vector2F(4, 0),
			});
			RoomControl.SpawnEntity(new MonsterGel() {
				Color = MonsterColor.Red,
				Position = position + new Vector2F(4, 0),
			});
		}
	}
}
