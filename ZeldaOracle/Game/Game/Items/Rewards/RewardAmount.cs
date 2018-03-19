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
		/// <summary>The message displayed when the cannot be carried because the
		/// container is full.</summary>
		private string fullMessage;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		/// <summary>Constructs an amount reward.</summary>
		public RewardAmount() {
			amount			= 0;
			fullMessage		= "";
		}


		//-----------------------------------------------------------------------------
		// Virtual Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets if the reward is already at capacity.</summary>
		public virtual bool IsFull {
			get { return false; }
		}

		/// <summary>Gets the appropriate message to display when collecting the
		/// reward.</summary>
		public override string AppropriateMessage {
			get {
				string text = Message;
				if (!CanCollect && !string.IsNullOrWhiteSpace(CantCollectMessage))
					text = CantCollectMessage;
				else if (IsFull && !string.IsNullOrWhiteSpace(FullMessage))
					text = FullMessage;
				return text;
			}
		}


		//-----------------------------------------------------------------------------
		// Override Methods
		//-----------------------------------------------------------------------------
		
		/// <summary>Called when the reward is being initialized.</summary>
		protected override void OnInitialize() {
			FullMessage			= RewardData.FullMessage;
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

		/// <summary>Gets the message displayed when the reward is already at capacity.</summary>
		public string FullMessage {
			get { return fullMessage; }
			set { fullMessage = value; }
		}
	}
}
