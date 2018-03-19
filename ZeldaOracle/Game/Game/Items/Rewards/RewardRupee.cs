using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Game.Control;

namespace ZeldaOracle.Game.Items.Rewards {
	/// <summary>A reward that gives a player a specified amount of rupees.</summary>
	public class RewardRupee : RewardAmmo {
		
		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------
		
		public RewardRupee() {
		}


		//-----------------------------------------------------------------------------
		// Static Methods
		//-----------------------------------------------------------------------------

		/// <summary>Initializes the properties for the reward type.</summary>
		public new static void InitializeRewardData(RewardData data) {
			data.AmmoData = Resources.Get<AmmoData>("rupees");
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		/// <summary>Called when the player collects the reward.</summary>
		public override void OnCollect() {
			if (GameControl.HUD.DynamicRupees >= Ammo.MaxAmount)
				AudioSystem.PlaySound(GameData.SOUND_GET_RUPEE);

			Ammo.Amount += Amount;
		}
	}
}
