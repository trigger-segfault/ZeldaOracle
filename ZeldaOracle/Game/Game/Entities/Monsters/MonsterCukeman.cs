using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Game.Entities.Monsters {

	public class MonsterCukeman : MonsterBuzzBlob {

		private string[] catchPhrases;

		
		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public MonsterCukeman() {
			// List of phrases that are said upon talking to this monster
			catchPhrases = new string[] {
				"Feel my cold, steely gaze!!!",
				"Hu? Did I say that?",
				"I wish I could go to a tropical southern island.",
				"Really? I mean, I knew that!",
				"I want to ride a plane. Anywhere<ap>s fine.",
				"3 Large,<n>2 Regular.", "I<ap>m so sleepy.",
				"I want a nice tropical vacation."
			};
			
			// Graphics
			animationMove = GameData.ANIM_MONSTER_CUKEMAN;

			// Disable the transform-into-cukeman reaction which was set by the
			// BuzzBlob base class
			Interactions.SetReaction(InteractionType.MysterySeed, Reactions.MysterySeed);
		}
		

		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override bool OnPlayerAction(int direction) {
			int index = GRandom.NextInt(catchPhrases.Length);
			RoomControl.GameControl.DisplayMessage(catchPhrases[index]);
			return true;
		}
	}
}
