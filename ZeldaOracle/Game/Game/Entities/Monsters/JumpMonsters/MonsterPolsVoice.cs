using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Game.Entities.Monsters.JumpMonsters {

	public class MonsterPolsVoice : JumpMonster {

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public MonsterPolsVoice() {
			Color			= MonsterColor.Orange;
			MaxHealth		= 1;
			ContactDamage	= 2;

			Physics.Gravity	= 0.06f;
			moveSpeed		= 0.5f;
			jumpSpeed		= new RangeF(1.3f);
			stopTime		= new RangeI(45);
			stopAnimation	= GameData.ANIM_MONSTER_POLS_VOICE;
			jumpAnimation	= GameData.ANIM_MONSTER_POLS_VOICE_JUMP;
			jumpSound		= null;

			// NOTE: Pols Voice can still be affected by:
			//  - Bombs
			//  - Thrown objects
			//  - Gale seeds
			//  - Pegasus seeds
			//  - TODO: Killed by Music

			// Weapon interactions
			Interactions.SetReaction(InteractionType.Sword,			MonsterReactions.Bump);
			Interactions.SetReaction(InteractionType.SwordSpin,		MonsterReactions.Bump);
			Interactions.SetReaction(InteractionType.BiggoronSword,	MonsterReactions.Bump);
			//Interactions.SetReaction(InteractionType.Music,			Reactions.Kill);
			// Projectile interactions
			Interactions.SetReaction(InteractionType.Fire,			SenderReactions.Intercept);
			Interactions.SetReaction(InteractionType.RodFire,		SenderReactions.Intercept);
			Interactions.SetReaction(InteractionType.EmberSeed,		SenderReactions.Intercept, MonsterReactions.Bump);
			Interactions.SetReaction(InteractionType.ScentSeed,		SenderReactions.Intercept, MonsterReactions.Bump);
			Interactions.SetReaction(InteractionType.Arrow,			SenderReactions.Intercept, MonsterReactions.Bump);
			Interactions.SetReaction(InteractionType.SwordBeam,		SenderReactions.Intercept, MonsterReactions.Bump);
			Interactions.SetReaction(InteractionType.Boomerang,		SenderReactions.Intercept, MonsterReactions.Bump);
			Interactions.SetReaction(InteractionType.SwitchHook,		SenderReactions.Intercept, MonsterReactions.Bump);
			// Misc interactions
			Interactions.SetReaction(InteractionType.Block,	MonsterReactions.Bump);
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void OnJump() {
			if (GRandom.NextInt(GameSettings.MONSTER_POLS_VOICE_SEEK_PLAYER_ODDS) == 0) {
				// Occasionally jump quickly toward the player
				Physics.Velocity = (RoomControl.Player.Center -
					Center).Normalized * moveSpeed;
				Physics.Velocity *=
					GameSettings.MONSTER_POLS_VOICE_SEEK_MOVE_SPEED_MULTIPLIER;
				Physics.ZVelocity *=
					GameSettings.MONSTER_POLS_VOICE_SEEK_JUMP_SPEED_MULTIPLIER;
			}
			else {
				// Otherwise, jump in a random angle
				Physics.Velocity = GRandom.NextAngle().ToVector(moveSpeed);
			}
		}
	}
}
