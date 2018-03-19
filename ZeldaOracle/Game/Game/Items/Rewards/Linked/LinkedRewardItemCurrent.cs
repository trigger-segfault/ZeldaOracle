using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Game.Control;

namespace ZeldaOracle.Game.Items.Rewards.Linked {
	/// <summary>A linked reward that returns the item at its current level.</summary>
	public class LinkedRewardItemCurrent : LinkedReward {
		
		/// <summary>The item this reward is linked to.</summary>
		private Item item;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------
		
		/// <summary>Constructs a reward for obtaining or reobtaining an item.</summary>
		public LinkedRewardItemCurrent(Item item) : base("item_" + item.ID) {
			this.item = item;

			CanvasSprite spriteC = Resources.Get<ISprite>("linked_reward_item_c")
				as CanvasSprite;
			if (spriteC == null) {
				spriteC = new CanvasSprite(new Point2I(8, 8));
				Graphics2D g = spriteC.Begin(GameSettings.DRAW_MODE_DEFAULT);
				foreach (Angle a in Angle.Range) {
					g.DrawString(GameData.FONT_SMALL, "C", a.ToPoint(), Color.Black);
				}
				g.DrawString(GameData.FONT_SMALL, "C", Point2I.Zero, Color.White);
				spriteC.End(g);
				Resources.Add<ISprite>("linked_reward_item_c", spriteC);
			}

			// We're referencing and setting the sprite so the
			// Reward class does it's centering magic.
			Sprite = item.GetSprite(Item.Level1);
			CompositeSprite composite = new CompositeSprite();
			composite.AddSprite(Sprite);
			composite.AddSprite(spriteC, new Point2I(8, 8));
			Sprite = composite;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets the item this reward represents.</summary>
		public Item Item {
			get { return item; }
		}

		/// <summary>Gets the reward being linked to.</summary>
		public override Reward Reward {
			get {
				return RewardManager.GetReward(RewardItem.ItemName(item, item.Level));
			}
		}
	}
}
