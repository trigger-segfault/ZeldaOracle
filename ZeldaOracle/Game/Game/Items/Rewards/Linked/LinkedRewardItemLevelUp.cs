using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Graphics.Sprites;

namespace ZeldaOracle.Game.Items.Rewards.Linked {
	/// <summary>A linked reward that's useful for obtaining a leveled up item or
	/// the first level of the item if it is unobtained.</summary>
	public class LinkedRewardItemLevelUp : LinkedReward {

		/// <summary>The item this reward is linked to.</summary>
		private Item item;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		/// <summary>Constructs a reward for obtaining a leveled up item or the first
		/// level of the item if it is unobtained.</summary>
		public LinkedRewardItemLevelUp(Item item)
			: base("item_" + item.ID + "_lvlup")
		{
			this.item = item;

			CanvasSprite spriteUp = Resources.Get<ISprite>("linked_reward_item_up")
				as CanvasSprite;
			if (spriteUp == null) {
				spriteUp = new CanvasSprite(new Point2I(8, 8));
				Graphics2D g = spriteUp.Begin(GameSettings.DRAW_MODE_DEFAULT);
				foreach (Angle a in Angle.Range) {
					g.DrawString(GameData.FONT_SMALL, "<up>", a.ToPoint(), Color.Black);
				}
				g.DrawString(GameData.FONT_SMALL, "<up>", Point2I.Zero, Color.White);
				spriteUp.End(g);
				Resources.Add<ISprite>("linked_reward_item_up", spriteUp);
			}

			// We're referencing and setting the sprite so the
			// Reward class does it's centering magic.
			Sprite = item.GetSprite(Item.Level2);
			CompositeSprite composite = new CompositeSprite();
			composite.AddSprite(Sprite);
			composite.AddSprite(spriteUp, new Point2I(8, 8));
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
				int level = Item.Level1;
				if (item.IsObtained)
					level = GMath.Min(item.Level + 1, item.MaxLevel);
				return RewardManager.GetReward(RewardItem.ItemName(item, level));
			}
		}
	}
}
