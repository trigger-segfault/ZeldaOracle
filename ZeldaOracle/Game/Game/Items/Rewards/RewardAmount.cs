using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Graphics.Sprites;

namespace ZeldaOracle.Game.Items.Rewards {
	/// <summary>A reward that can have an amount set and has messages for when full.</summary>
	public abstract class RewardAmount : Reward {
		
		/// <summary>The amount of the reward to give.</summary>
		private int amount;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		/// <summary>Constructs an amount reward.</summary>
		public RewardAmount() {
			amount			= 0;
		}


		//-----------------------------------------------------------------------------
		// Override Methods
		//-----------------------------------------------------------------------------
		
		/// <summary>Called when the reward is being initialized.</summary>
		protected override void OnInitialize() {
			Amount				= RewardData.Amount;
		}


		//-----------------------------------------------------------------------------
		// Static Methods
		//-----------------------------------------------------------------------------

		/// <summary>Initializes the properties for the reward type.</summary>
		public static void InitializeRewardData(RewardData data) {
			data.HoldInChest        = false;
			data.HoldType           = RewardHoldTypes.TwoHands;
			data.HasDuration        = true;
			data.ShowPickupMessage  = false;
			data.WeaponInteract     = true;
		}
		

		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets the amount of the reward given with this reward.</summary>
		public int Amount {
			get { return amount; }
			set { amount = value; }
		}
	}
}
