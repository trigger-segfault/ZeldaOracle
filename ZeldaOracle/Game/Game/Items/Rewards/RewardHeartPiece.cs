using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Control;

namespace ZeldaOracle.Game.Items.Rewards {
	public class RewardHeartPiece : Reward {

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public RewardHeartPiece() {
			InitSprite(GameData.SPR_REWARD_HEART_PIECE);

			this.id				= "heart_piece";
			this.message		= "You got a <red>Piece of Heart<red>! Collect four in all to get an extra Heart Container! Check them on the Item Screen.";
			this.hasDuration	= false;
			this.holdType		= RewardHoldTypes.TwoHands;
			this.isCollectibleWithItems	= false;
		}
		

		//-----------------------------------------------------------------------------
		// Virtual methods
		//-----------------------------------------------------------------------------

		public override void OnCollect(GameControl gameControl) {
			gameControl.Inventory.PiecesOfHeart++;
			if (gameControl.Inventory.PiecesOfHeart == 4) {
				gameControl.Inventory.PiecesOfHeart = 0;
				gameControl.Player.MaxHealth += 4;
				gameControl.Player.Health += 4;
				gameControl.DisplayMessage(new Message("You got four<n><red>Pieces of Heart<red>!<n>That makes one<n><red>Heart Container<red>!"));
			}
		}
	}
}
