using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Items.Rewards;

namespace ZeldaOracle.Game.Entities {
	public class Collectible : Entity {

		protected Reward reward;
		protected int timer;

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public Collectible(Reward reward) {
			EnablePhysics(PhysicsFlags.Bounces |
				PhysicsFlags.HasGravity |
				PhysicsFlags.DestroyedOutsideRoom |
				PhysicsFlags.CollideWorld |
				PhysicsFlags.HalfSolidPassable |
				PhysicsFlags.LedgePassable |
				PhysicsFlags.DestroyedInHoles);

			this.reward					= reward;
			this.timer					= 0;
		}

		//-----------------------------------------------------------------------------
		// Collection
		//-----------------------------------------------------------------------------

		public void Collect() {
			Destroy();
			reward.OnCollect(GameControl);
		}

		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void Initialize() {
			base.Initialize();
			
			timer		= 0;
			Graphics.IsGrassEffectVisible		= true;
			Graphics.IsRipplesEffectVisible		= true;
			Graphics.IsShadowVisible			= true;
			Graphics.AnimationPlayer.Animation	= reward.Animation;

			Physics.CollisionBox				= reward.CollisionBox;
		}

		public override void Update() {
			base.Update();

			timer++;
			if (timer == reward.Duration) {
				Destroy();
			}
			else if (timer == reward.FadeDuration)
				graphics.IsFlickering = true;

			// Check for colliding with the player.
			if (physics.IsSoftMeetingEntity(GameControl.Player, 9))
				Collect();
		}

		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public Reward Reward {
			get { return reward; }
		}
	}
}
