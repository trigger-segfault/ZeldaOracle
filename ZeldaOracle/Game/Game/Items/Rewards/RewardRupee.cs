using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Game.Control;

namespace ZeldaOracle.Game.Items.Rewards {
	/// <summary>A reward that gives a player a specified amount of rupees.</summary>
	public class RewardRupee : RewardAmmo {

		/// <summary>The amount of rupees to give.</summary>
		private int amount;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public RewardRupee(string id, int amount, string message, ISprite sprite)
			: base(id, "rupees", amount, message, sprite)
		{
			this.amount			= amount;

			Sprite			= sprite;
			Message			= message;
			FullMessage			= message + "<p>But the wallet is full.";
			CantCollectMessage  = message + "<p>But there's no wallet to put it in.";
			HoldType		= RewardHoldTypes.Raise;
			HasDuration		= true;
			ShowMessageOnPickup			= false;
			IsCollectibleWithWeapons	= true;
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		/// <summary>Called when the player collects the reward.</summary>
		public override void OnCollect() {
			if (GameControl.HUD.DynamicRupees >= Ammo.MaxAmount)
				AudioSystem.PlaySound(GameData.SOUND_GET_RUPEE);

			Ammo.Amount += amount;
		}
	}
}
