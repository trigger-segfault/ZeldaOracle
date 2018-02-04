using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Entities.Monsters.States;
using ZeldaOracle.Game.Entities.Units;

namespace ZeldaOracle.Game.Entities.Monsters {

	public class MonsterCukeman : BasicMonster {

		private string[] catchPhrases;

		
		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public MonsterCukeman() {
			// General
			MaxHealth		= 1;
			ContactDamage	= 2;
			Color			= MonsterColor.Green;
			
			catchPhrases = new string[] {
				"Feel my cold, steely gaze!!!",
				"Hu? Did I say that?",
				"I wish I could go to a tropical southern island.",
				"Really? I mean, I knew that!",
				"I want to ride a plane. Anywhere<ap>s fine.",
				"3 Large,<n>2 Regular.", "I<ap>m so sleepy.",
				"I want a nice tropical vacation."
			};

			// Movement
			moveSpeed					= 0.25f;
			numMoveAngles				= 8;
			orientationStyle			= OrientationStyle.Angle;
			changeDirectionsOnCollide	= true;
			movesInAir					= false;
			facePlayerOdds				= 0;
			stopTime.Set(0, 0);
			moveTime.Set(50, 80);
			
			// Physics
			Physics.ReboundRoomEdge	= true;
			Physics.ReboundSolid	= true;
			
			// Graphics.
			animationMove				= GameData.ANIM_MONSTER_CUKEMAN;
			syncAnimationWithDirection	= false;

			// Weapon interactions
			SetReaction(InteractionType.Sword,			Reactions.Electrocute);
			SetReaction(InteractionType.SwordSpin,		Reactions.Electrocute);
			SetReaction(InteractionType.BiggoronSword,	Reactions.Electrocute);
			// Projectile interactions
			SetReaction(InteractionType.Arrow,			SenderReactions.Intercept, Reactions.Damage);
			SetReaction(InteractionType.SwordBeam,		SenderReactions.Intercept, Reactions.Kill);
			SetReaction(InteractionType.SwitchHook,		SenderReactions.Intercept, Reactions.Electrocute);
			SetReaction(InteractionType.MysterySeed,	SenderReactions.Intercept);
			SetReaction(InteractionType.BombExplosion,	Reactions.Kill);
			// Player
			SetReaction(InteractionType.ButtonAction,	SayCatchPhrase);
		}
		

		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void Initialize() {
			base.Initialize();
		}

		public void SayCatchPhrase(Monster monster, Entity sender, EventArgs args) {
			int index = GRandom.NextInt(catchPhrases.Length);
			RoomControl.GameControl.DisplayMessage(catchPhrases[index]);
		}
	}
}
