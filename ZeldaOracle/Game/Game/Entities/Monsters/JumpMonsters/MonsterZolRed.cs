using System;
using ZeldaOracle.Game.Items;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Entities.Effects;

namespace ZeldaOracle.Game.Entities.Monsters.JumpMonsters {

	public class MonsterZolRed : JumpMonster {

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public MonsterZolRed() {
			Color			= MonsterColor.Red;
			MaxHealth		= 2;
			ContactDamage	= 1;
			//softKill		= true; // Offspring will perform the "hard" kill

			// How to decide when Red Zol splits:
			// 1 Damage? -> Split
			// 2 Damage? -> Kill
			// Burn? -> Split (Seems to be exception to 2 damge rule)
			
			// TODO: Knockback still applies for Block and ScentSeed interactions
			// Most likely magnet ball too.

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

			// Always instantly die/split
			//isKnockbackable         = false;

			jumpOdds                = 6; // Needs confirmation
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
			//Interactions.SetReaction(InteractionType.Sword,			SplitOrKillByLevel);
			//Interactions.SetReaction(InteractionType.SwordSpin,		MonsterReactions.Kill);
			//Interactions.SetReaction(InteractionType.BiggoronSword,	MonsterReactions.Kill);
			// Projectile interactions
			//Interactions.SetReaction(InteractionType.Arrow,			SenderReactions.Intercept, Split);
			//Interactions.SetReaction(InteractionType.SwordBeam,		SenderReactions.Intercept, Split);
			//Interactions.SetReaction(InteractionType.SwitchHook,	SenderReactions.Intercept, Split);
			// Environment Reactions
			Interactions.SetReaction(InteractionType.Fire,			Burn);
			Interactions.SetReaction(InteractionType.Block,			MonsterReactions.Kill);
			//Interactions.SetReaction(InteractionType.ThrownObject,	MonsterReactions.Kill);

		}


		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------
		
		private MonsterGel SpawnOffspring(Vector2F position) {
			MonsterGel child = new MonsterGel();
			child.Color = MonsterColor.Red;
			child.Properties = Properties;
			RoomControl.SpawnEntity(child, position);
			child.MonsterID = MonsterID;
			return child;
		}

		private void SpawnOffspring() {
			// Monster is no longer needed to prevent clearing
			RoomControl.RemoveLingeringMonster(this);

			MonsterGel child1 = SpawnOffspring(position + new Vector2F(4, 0));
			MonsterGel child2 = SpawnOffspring(position - new Vector2F(4, 0));
			// Let the Gel know of their siblings so they can later 
			// mark the other to be ignored for room "respawn clear".
			child1.OffspringSibling = child2;
			child2.OffspringSibling = child1;
		}

		private void Split() {
			// Prevent the room from being cleared in
			// either sense until offspring are spawned.
			RoomControl.AddLingeringMonster(this);

			RoomControl.ScheduleEvent(19, SpawnOffspring);
			CreateDeathEffect();
			Destroy();
		}


		//-----------------------------------------------------------------------------
		// Internal Reactions
		//-----------------------------------------------------------------------------
		
		private void Burn(Entity sender, EventArgs args) {
			Burn(1);
			if (sender.RootEntity is Fire)
				sender.RootEntity.Destroy();
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void OnHurt(DamageInfo damage) {
			base.OnHurt(damage);
			// Split if we're still alive, otherwise instantly die
			if (Health > 0)
				Split();
			else
				Die();
		}
	}
}
