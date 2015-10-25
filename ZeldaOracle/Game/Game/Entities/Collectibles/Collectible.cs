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
		protected bool showMessage;
		protected int timer;

		//-----------------------------------------------------------------------------
		// Constants
		//-----------------------------------------------------------------------------

		private const int Duration = 513;
		private const int FadeTime = 400;
		private const int PickupableTime = 12;

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

			this.reward			= reward;
			this.showMessage	= false;
			this.timer			= 0;
			this.Graphics.DrawOffset	= new Point2I(-8, -8);
		}

		//-----------------------------------------------------------------------------
		// Collection
		//-----------------------------------------------------------------------------

		public void Collect() {
			Destroy();
			if (showMessage) {
				GameControl.DisplayMessage(new Control.Message(reward.Message));
			}
			reward.OnCollect(GameControl);
		}

		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void Initialize() {
			base.Initialize();
			
			timer								= 0;

			Graphics.Animation					= reward.Animation;
			Graphics.IsGrassEffectVisible		= true;
			Graphics.IsRipplesEffectVisible		= true;
			Graphics.IsShadowVisible			= true;
			Graphics.GrassDrawOffset			= new Point2I(0, 5);
			Graphics.RipplesDrawOffset			= new Point2I(0, 6);
			Graphics.ShadowDrawOffset			= new Point2I(0, 5);

			Physics.CollisionBox				= new Rectangle2I(-4, -4, 8, 8);
			Physics.SoftCollisionBox			= new Rectangle2I(-5, -4, 9, 8);
		}

		public override void Update() {
			base.Update();

			timer++;
			if (reward.HasDuration) {
				if (timer == Duration) {
					Destroy();
				}
				else if (timer == FadeTime)
					graphics.IsFlickering = true;
			}

			// Check for colliding with the player.
			if (physics.IsSoftMeetingEntity(GameControl.Player, 9) && timer >= PickupableTime)
				Collect();
		}

		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public Reward Reward {
			get { return reward; }
		}

		public bool ShowMessage {
			get { return showMessage; }
			set { showMessage = value; }
		}

		public bool IsPickupable {
			get { return (timer >= PickupableTime); }
		}
	}
}
