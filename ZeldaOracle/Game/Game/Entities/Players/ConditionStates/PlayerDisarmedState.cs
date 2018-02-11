
namespace ZeldaOracle.Game.Entities.Players.States {

	public class PlayerDisarmedState : PlayerState {

		private int duration;
		private int timer;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public PlayerDisarmedState(int duration) {
			this.duration = duration;

			StateParameters.ProhibitWeaponUse = true;
		}
		

		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnBegin(PlayerState previousState) {
			timer = 0;
			player.InterruptWeapons();
		}

		public override void Update() {
			timer++;
			if (timer >= duration)
				End();
		}
	}
}
