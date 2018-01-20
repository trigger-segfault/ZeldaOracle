using ZeldaOracle.Common.Graphics;

namespace ZeldaOracle.Game.GameStates.Transitions {
	
	public class RoomTransitionInstant : RoomTransition {

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public RoomTransitionInstant() {
		}

		
		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void Update() {
			// Instantly change to the new room on the first update.
			// Note that this would probably cause bugs if it were done in OnBegin.
			SetupNewRoom(true);
			DestroyOldRoom();
			EndTransition();
		}

		public override void Draw(Graphics2D g) {
			// Draw the old room for a single frame.
			g.ResetTranslation();
			OldRoomControl.Draw(g);
		}
	}
}
