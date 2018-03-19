using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Control;

namespace ZeldaOracle.Game.Items.Rewards {
	/// <summary>A reward that gives a player an item at a specified level.</summary>
	public class RewardItem : Reward {

		/// <summary>The item to reward.</summary>
		private Item item;
		/// <summary>The level of the item to reward.</summary>
		private int level;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		/// <summary>Constructs a reward item based on the item and level.</summary>
		public RewardItem(Item item, int level) : base(ItemName(item, level)) {
			this.item = item;
			this.level = level;
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		/// <summary>Called when the reward is being initialized.</summary>
		protected override void OnInitialize() {
			Sprite				= item.GetSprite(level);
			Message				= item.GetMessage(level);
			HoldType			= item.HoldType;
			HasDuration			= false;
			ShowPickupMessage	= true;
			InteractWithWeapons	= false;
		}

		/// <summary>Called when the player collects the reward.</summary>
		public override void OnCollect() {
			Inventory.ObtainItem(item);
			// Don't level-down the item
			item.Level = GMath.Max(item.Level, level);
		}


		//-----------------------------------------------------------------------------
		// Static Methods
		//-----------------------------------------------------------------------------

		/// <summary>Constructs an the reward name for an item at the specified level.</summary>
		public static string ItemName(Item item, int level) {
			string name = "item_" + item.ID;
			if (item.IsLeveled)
				name += "_" + (level + 1);
			return name;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets the item this reward represents.</summary>
		public Item Item {
			get { return item; }
		}

		/// <summary>Gets the item level this reward represents.</summary>
		public int Level {
			get { return level; }
		}
	}
}
