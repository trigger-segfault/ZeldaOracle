using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Graphics.Sprites;

namespace ZeldaOracle.Game.Items.Rewards {
	/// <summary>A reward class used for calculating the reward at runtime.</summary>
	public abstract class LinkedReward : Reward {
		
		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		/// <summary>Constructs the linked reward from the unique ID.</summary>
		public LinkedReward(string id) : base(id) {
		}


		//-----------------------------------------------------------------------------
		// Abstract Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets the reward being linked to.</summary>
		public abstract Reward Reward { get; }
	}
}
