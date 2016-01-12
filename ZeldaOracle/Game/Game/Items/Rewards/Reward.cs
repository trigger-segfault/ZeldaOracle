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
		protected bool onlyShowMessageInChest;
		

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public Reward() {
			this.id				= "";
			this.animation		= null;
			this.message		= "";
			this.hasDuration	= false;
			this.isCollectibleWithItems	= false;
			this.onlyShowMessageInChest = false;
		}


		//-----------------------------------------------------------------------------
		// Virtual Methods
		//-----------------------------------------------------------------------------

		public virtual void OnCollect(GameControl gameControl) {}

		public virtual bool IsAvailable(GameControl gameControl) {
			return true;
		}

		protected void InitAnimation(SpriteAnimation animation) {
			if (animation.IsSprite)
				InitSprite(animation.Sprite);
			else if (animation.IsAnimation)
				InitAnimation(animation.Animation);
			else
				this.animation = null;
		}

		protected void InitSprite(Sprite sprite) {
			if (sprite.SourceRect.Width == 8 && sprite.DrawOffset.X == 0) {
				sprite = new Sprite(sprite);
				sprite.DrawOffset = new Point2I(4, sprite.DrawOffset.Y);
			}
			InitAnimation(new Animation(sprite));
		}
		
		protected void InitAnimation(Animation animation) {
			this.animation = animation;
		}


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

		public bool OnlyShowMessageInChest {
			get { return onlyShowMessageInChest; }
		}
	}
}
