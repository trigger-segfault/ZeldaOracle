using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Game.Entities.Monsters.JumpMonsters {
	public class MonsterPolsVoice : JumpMonster {

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public MonsterPolsVoice() {
			color			= MonsterColor.Orange;
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
			SetReaction(InteractionType.Sword,			Reactions.Bump);
			SetReaction(InteractionType.SwordSpin,		Reactions.Bump);
			SetReaction(InteractionType.BiggoronSword,	Reactions.Bump);
			//SetReaction(InteractionType.Music,			Reactions.Kill);
			// Projectile interactions
			SetReaction(InteractionType.Fire,			SenderReactions.Intercept, Reactions.None);
			SetReaction(InteractionType.RodFire,		SenderReactions.Intercept, Reactions.None);
			SetReaction(InteractionType.EmberSeed,		SenderReactions.Intercept, Reactions.Bump);
			SetReaction(InteractionType.ScentSeed,		SenderReactions.Intercept, Reactions.Bump);
			SetReaction(InteractionType.Arrow,			SenderReactions.Intercept, Reactions.Bump);
			SetReaction(InteractionType.SwordBeam,		SenderReactions.Intercept, Reactions.Bump);
			SetReaction(InteractionType.Boomerang,		SenderReactions.Intercept, Reactions.Bump);
			SetReaction(InteractionType.SwitchHook,		SenderReactions.Intercept, Reactions.Bump);
			// Misc interactions
			SetReaction(InteractionType.Block,	Reactions.Bump);
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
				Physics.Velocity = Angles.ToVector(
					GRandom.NextInt(Angles.AngleCount)) * moveSpeed;
			}
		}
	}
}
