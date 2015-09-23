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
		protected int fadeDuration;
		protected Animation animation;
		protected Rectangle2I collisionBox;
		protected bool isCollectibleWithItems;

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public Reward() {
			this.id				= "";
			this.duration		= -1;
			this.fadeDuration	= -1;
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

		public int FadeDuration {
			get { return fadeDuration; }
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
