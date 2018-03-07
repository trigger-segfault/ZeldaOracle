using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Game.Entities.Effects {

	public class BombExplosion : Effect {

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public BombExplosion() :
			base(GameData.ANIM_EFFECT_BOMB_EXPLOSION)
		{
			// Graphics
			Graphics.DepthLayer = DepthLayer.EffectBombExplosion;

			// Physics
			Physics.Enable(PhysicsFlags.None);
			Physics.CollisionBox = new Rectangle2F(-12, -12, 24, 24);
			
			// Interactions
			Interactions.Enable();
			Interactions.InteractionBox	= new Rectangle2F(-12, -12, 24, 24);
			Interactions.InteractionZRange = new RangeF(-32, 32);
		}
		

		//-----------------------------------------------------------------------------
		// Collision Handlers
		//-----------------------------------------------------------------------------

		public override void Update() {
			// Enable the Bomb explosion interaction type after a short delay
			float playbackTime = Graphics.AnimationPlayer.PlaybackTime;
			if (playbackTime > GameSettings.BOMB_EXPLOSION_DAMAGE_DELAY)
				Interactions.InteractionType = InteractionType.BombExplosion;

			base.Update();
		}
	}
}
