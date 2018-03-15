using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Game.Control;

namespace ZeldaOracle.Game.Items.Rewards {
	/// <summary>A linked reward that returns the item at its current level.</summary>
	public class LinkedRewardItemCurrent : LinkedReward {
		/// <summary>The canvas sprite for the colored 'C' character.
		/// This only needs to be created once.</summary>
		private static CanvasSprite SPR_C = null;

		/// <summary>The item this reward is linked to.</summary>
		private Item item;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		/// <summary>Constructs a reward for obtaining or reobtaining an item.</summary>
		public LinkedRewardItemCurrent(Item item) : base(item.ID) {
			this.item = item;

			if (SPR_C == null) {
				SPR_C = new CanvasSprite(new Point2I(8, 8));
				Graphics2D g = SPR_C.Begin(GameSettings.DRAW_MODE_DEFAULT);
				foreach (Angle a in Angle.Range) {
					g.DrawString(GameData.FONT_SMALL, "C", a.ToPoint(), Color.Black);
				}
				g.DrawString(GameData.FONT_SMALL, "C", Point2I.Zero, Color.White);
				SPR_C.End(g);
			}

			// We're referencing and setting the sprite so the
			// Reward class does it's centering magic.
			Sprite = item.GetSprite(Item.Level1);
			CompositeSprite composite = new CompositeSprite();
			composite.AddSprite(Sprite);
			//composite.AddSprite(GameData.SPR_HUD_LEVEL, new Point2I(0, 8));
			composite.AddSprite(SPR_C, new Point2I(8, 8));
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
