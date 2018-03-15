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
	public class RewardRecoveryHeart : RewardAmount {
		
		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------
		
		/// <summary>Constructs the recovery heart reward</summary>
		public RewardRecoveryHeart(string id)
			: base(id)
		{
			HoldInChest		= false;
			HoldType		= RewardHoldTypes.TwoHands;
			HasDuration		= true;
			ShowMessageOnPickup			= false;
			InteractWithWeapons	= true;
		}

		/// <summary>Constructs the recovery heart reward</summary>
		public RewardRecoveryHeart(string id, int amount, string message, ISprite sprite)
			: this(id)
		{

			Sprite			= sprite;
			Message			= message;
			Amount			= amount;
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		/// <summary>Called when the player collects the reward.</summary>
		public override void OnCollect() {
			if (GameControl.HUD.DynamicHealth >= Player.MaxHealth)
				AudioSystem.PlaySound(GameData.SOUND_GET_HEART);

			Player.Health += Amount * 4;
		}


		//-----------------------------------------------------------------------------
		// Overridden Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets if the reward is already at capacity.</summary>
		public override bool IsFull {
			get { return Player.Health >= Player.MaxHealth; }
		}
	}
}
