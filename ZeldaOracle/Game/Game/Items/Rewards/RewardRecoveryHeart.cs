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
	public class RewardRecoveryHeart : Reward {

		/// <summary>The amount of full hearts to restore.</summary>
		private int amount;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public RewardRecoveryHeart(string id, int amount, string message, ISprite sprite)
			: base(id)
		{
			this.amount		= amount;

			Sprite			= sprite;
			Message			= message;
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
			if (GameControl.HUD.DynamicHealth >= Player.MaxHealth)
				AudioSystem.PlaySound(GameData.SOUND_GET_HEART);

			Player.Health += amount * 4;
		}
	}
}
