using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Game.Entities.Effects {

	public class EffectGale : Effect {
		
		// FROM SHOOTER TO WALL:
		//   12 before fade.
		//   18 of fade
		// ON MONSTER:
		//   31 before fade. (once fade starts, monster rises up)
		//   18 of fade.
		// ON NOTHING (Dropped from satchel)
		//   0 before fade.
		//   256 of fade.

		private bool droppedFromSatchel;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public EffectGale(bool droppedFromSatchel) :
			base(GameData.ANIM_EFFECT_SEED_GALE, DepthLayer.EffectGale)
		{
			// Interactions
			Interactions.Enable();
			Interactions.InteractionBox = new Rectangle2F(-6, -6, 12, 12);
			if (droppedFromSatchel)
				Interactions.InteractionType = InteractionType.Gale;

			// Gale Effect
			this.droppedFromSatchel = droppedFromSatchel;
			if (droppedFromSatchel)
				CreateDestroyTimer(256, 255, 1);
			else
				CreateDestroyTimer(30, 12, 1);
		}
		

		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void Update() {
			if (droppedFromSatchel) {
				// TODO: Gale player to warp screen
			}

			base.Update();
		}
	}
}
