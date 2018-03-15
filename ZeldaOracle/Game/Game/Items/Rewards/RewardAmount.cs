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
		public RewardAmount(string id) : base(id) {
			amount			= 0;
			fullMessage		= "";
		}
		
		/// <summary>Clones the data for the specified reward.</summary>
		public override void Clone(Reward reward) {
			base.Clone(reward);

			if (reward is RewardAmount) {
				var rewardAmount = (RewardAmount) reward;

				amount		= rewardAmount.amount;
				fullMessage	= rewardAmount.fullMessage;
			}
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
