using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Control;

namespace ZeldaOracle.Game.Items.Rewards {
	public class Reward {

		protected string id;
		protected int duration;
		protected int fadeTime;
		protected int pickupableTime;
		protected Animation animation;
		protected Rectangle2I collisionBox;
		protected bool isCollectibleWithItems;

		//-----------------------------------------------------------------------------
		// Static
		//-----------------------------------------------------------------------------

		private static Dictionary<string, Reward> rewards;

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public Reward() {
			this.id				= "";
			this.duration		= -1;
			this.fadeTime		= -1;
			this.pickupableTime	= 0;
			this.animation		= null;
			this.collisionBox	= Rectangle2I.Zero;
			this.isCollectibleWithItems	= false;
		}


		//-----------------------------------------------------------------------------
		// Virtual methods
		//-----------------------------------------------------------------------------

		public virtual void OnCollect(GameControl gameControl) {}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public string ID {
			get { return id; }
		}

		public int Duration {
			get { return duration; }
		}

		public int FadeTime {
			get { return fadeTime; }
		}

		public int PickupableTime {
			get { return pickupableTime; }
		}

		public Animation Animation {
			get { return animation; }
		}

		public Rectangle2I CollisionBox {
			get { return collisionBox; }
		}

		public bool IsCollectibleWithItems {
			get { return isCollectibleWithItems; }
		}
	}
}
