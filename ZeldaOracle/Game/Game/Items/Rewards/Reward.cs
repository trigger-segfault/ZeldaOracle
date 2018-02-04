using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Game.Control;

namespace ZeldaOracle.Game.Items.Rewards {
	public class Reward {

		protected string id;
		protected ISprite sprite;
		protected string message;
		protected bool hasDuration;
		protected bool isCollectibleWithItems;
		protected RewardHoldTypes holdType;
		protected bool onlyShowMessageInChest;
		protected Sound soundBounce;
		

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public Reward() {
			this.id				= "";
			this.sprite			= null;
			this.message		= "";
			this.hasDuration	= false;
			this.isCollectibleWithItems	= false;
			this.onlyShowMessageInChest = false;
			this.soundBounce			= null;
		}


		//-----------------------------------------------------------------------------
		// Virtual Methods
		//-----------------------------------------------------------------------------

		public virtual void OnCollect(GameControl gameControl) { }

		public virtual void OnCollectNoMessage(GameControl gameControl) {
			OnCollect(gameControl);
		}

		public virtual void OnDisplayMessage(GameControl gameControl) {
			gameControl.DisplayMessage(message);
		}

		public virtual bool IsAvailable(GameControl gameControl) {
			return true;
		}

		protected void InitSprite(ISprite sprite) {
			Rectangle2I bounds = sprite.Bounds;
			if (bounds.Width == 8 && bounds.X == 0) {
				this.sprite = new OffsetSprite(sprite, new Point2I(4, 0));
			}
			else {
				this.sprite = sprite;
			}
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public string ID {
			get { return id; }
		}

		public ISprite Sprite {
			get { return sprite; }
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

		public bool OnlyShowMessageInChest {
			get { return onlyShowMessageInChest; }
		}

		public Sound BounceSound {
			get { return soundBounce; }
		}
	}
}
