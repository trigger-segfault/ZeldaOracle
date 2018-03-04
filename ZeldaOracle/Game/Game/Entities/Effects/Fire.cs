using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Tiles;

namespace ZeldaOracle.Game.Entities.Effects {

	public class Fire : Effect, IInterceptable {

		private int timer;
		private bool isAbsorbed;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public Fire() :
			base(GameData.ANIM_EFFECT_SEED_EMBER)
		{
			// Graphics
			Graphics.DrawOffset	= new Point2I(0, -2);
			Graphics.DepthLayer	= DepthLayer.EffectFire;

			// Physics
			Physics.Enable(PhysicsFlags.HasGravity);

			// Interactions
			Interactions.Enable();
			Interactions.InteractionBox = new Rectangle2F(-6, -6, 12, 12);

			// Fire
			isAbsorbed = false;
		}

		
		//-----------------------------------------------------------------------------
		// Implementations
		//-----------------------------------------------------------------------------
		
		public bool Intercept() {
			isAbsorbed = true;
			return true;
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void Initialize() {
			base.Initialize();
			timer = 0;
		}

		public override void OnDestroyTimerDone() {
			// Burn tiles
			Point2I location = RoomControl.GetTileLocation(position);
			if (RoomControl.IsTileInBounds(location)) {
				Tile tile = RoomControl.GetTopTile(location);
				if (tile != null)
					tile.OnBurn();
			}
			Destroy();
		}

		public override void Update() {
			timer++;

			Interactions.InteractionType = InteractionType.None;

			if (timer > 3) {
				Interactions.InteractionType = InteractionType.Fire;

				if (isAbsorbed) {
					Destroy();
					return;
				}
			}

			base.Update();
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public bool IsAbsorbed {
			get { return isAbsorbed; }
			set { isAbsorbed = value; }
		}
	}
}
