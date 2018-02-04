using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Control;
using ZeldaOracle.Game.GameStates;
using ZeldaOracle.Game.GameStates.RoomStates;

namespace ZeldaOracle.Game.Items.Rewards {
	public class RewardHeartPiece : Reward {

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public RewardHeartPiece() {
			InitSprite(GameData.SPR_REWARD_HEART_PIECE);

			this.id				= "heart_piece";
			this.message        = GameSettings.HEART_PIECE_TEXT;
			this.hasDuration	= false;
			this.holdType		= RewardHoldTypes.TwoHands;
			this.isCollectibleWithItems	= false;
		}


		//-----------------------------------------------------------------------------
		// Virtual methods
		//-----------------------------------------------------------------------------

		public override void OnDisplayMessage(GameControl gameControl) {
			if (gameControl.Inventory.PiecesOfHeart == 3) {
				gameControl.DisplayMessage(
					message,
					new RoomStateAction(() => {
						IncrementPiecesOfHeart(gameControl);
					}),
					new RoomStateTextReader(GameSettings.HEART_CONTAINER_TEXT)
				);
			}
			else {
				gameControl.DisplayMessage(
					message,
					new RoomStateAction(() => {
						IncrementPiecesOfHeart(gameControl);
					})
				);
			}
		}

		public override void OnCollectNoMessage(GameControl gameControl) {
			IncrementPiecesOfHeart(gameControl);
		}


		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------

		private void IncrementPiecesOfHeart(GameControl gameControl) {
			gameControl.Inventory.PiecesOfHeart++;
			if (gameControl.Inventory.PiecesOfHeart == 4) {
				AudioSystem.PlaySound(GameData.SOUND_HEART_CONTAINER);
				gameControl.Inventory.PiecesOfHeart = 0;
				gameControl.Player.MaxHealth += 4;
				gameControl.Player.Health = gameControl.Player.MaxHealth;
			}
		}
	}
}
