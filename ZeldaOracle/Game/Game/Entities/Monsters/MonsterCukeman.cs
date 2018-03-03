using System;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Game.Entities.Monsters {

	public class MonsterCukeman : MonsterBuzzBlob {

		private string[] catchPhrases;

		
		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public MonsterCukeman() {			
			// Graphics
			animationMove = GameData.ANIM_MONSTER_CUKEMAN;

			// Interactions
			Interactions.SetReaction(InteractionType.ButtonAction,
				EntityReactions.TriggerButtonReaction, SayCatchPhrase);
			// Disable the transform-into-cukeman reaction which was set by the
			// BuzzBlob base class
			Interactions.SetReaction(InteractionType.MysterySeed,
				Reactions.MysterySeed);
			
			// Create the list of phrases that are said upon talking to this monster
			catchPhrases = new string[] {
				"Feel my cold, steely gaze!!!",
				"Hu? Did I say that?",
				"I wish I could go to a tropical southern island.",
				"Really? I mean, I knew that!",
				"I want to ride a plane. Anywhere<ap>s fine.",
				"3 Large,<n>2 Regular.", "I<ap>m so sleepy.",
				"I want a nice tropical vacation."
			};
		}
		

		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public void SayCatchPhrase(Entity sender, EventArgs args) {
			int index = GRandom.NextInt(catchPhrases.Length);
			RoomControl.GameControl.DisplayMessage(catchPhrases[index]);
		}
	}
}
