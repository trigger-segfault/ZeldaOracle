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
		protected Animation animation;
		protected string message;
		protected bool hasDuration;
		protected bool isCollectibleWithItems;
		protected RewardHoldTypes holdType;

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public Reward() {
			this.id				= "";
			this.animation		= null;
			this.message		= "";
			this.hasDuration	= false;
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

		public Animation Animation {
			get { return animation; }
		}

		public string Message {
			get { return message; }
		}

		public bool HasDuration {
			get { return hasDuration; }
		}

		public bool IsCollectibleWithItems {
			get { return isCollectibleWithItems; }
		}

		public RewardHoldTypes HoldType {
			get { return holdType; }
		}
	}
}
