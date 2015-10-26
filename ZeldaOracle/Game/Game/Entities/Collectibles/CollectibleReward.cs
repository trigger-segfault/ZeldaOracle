using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Items.Rewards;

namespace ZeldaOracle.Game.Entities {
	public class CollectibleReward : Collectible {

		private Reward reward;
		private bool showMessage;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public CollectibleReward(Reward reward) {
			this.reward			= reward;
			this.showMessage	= false;

			// Physics.
			Physics.CollisionBox		= new Rectangle2I(-4, -9, 8, 8);
			Physics.SoftCollisionBox	= new Rectangle2I(-5, -9, 9, 8);
			EnablePhysics(
				PhysicsFlags.Bounces |
				PhysicsFlags.HasGravity |
				PhysicsFlags.CollideRoomEdge |
				PhysicsFlags.CollideWorld |
				PhysicsFlags.HalfSolidPassable |
				PhysicsFlags.DestroyedInHoles);

			// Graphics.
			centerOffset					= new Point2I(0, -5);
			Graphics.DrawOffset				= new Point2I(-8, -13);
			Graphics.RipplesDrawOffset		= new Point2I(0, 1);
			Graphics.IsGrassEffectVisible	= true;
			Graphics.IsRipplesEffectVisible	= true;
		}


		//-----------------------------------------------------------------------------
		// Collection
		//-----------------------------------------------------------------------------

		public override void Collect() {
			if (showMessage)
				GameControl.DisplayMessage(reward.Message);
			reward.OnCollect(GameControl);
			Destroy();
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void Initialize() {
			base.Initialize();
			
			hasDuration = reward.HasDuration;

			Graphics.PlayAnimation(reward.Animation);
		}

		public override void OnLand() {
			// Disable collisions after landing.
			Physics.CollideWithWorld = false;
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
	}
}
