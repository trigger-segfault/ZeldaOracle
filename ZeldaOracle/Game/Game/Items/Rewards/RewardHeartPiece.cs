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

		public RewardHeartPiece() : base("heart_piece") {
			Sprite			= GameData.SPR_REWARD_HEART_PIECE;
			Message			= GameSettings.HEART_PIECE_TEXT;
			HoldType		= RewardHoldTypes.TwoHands;
			HasDuration		= false;
			ShowMessageOnPickup	= true;
			InteractWithWeapons	= false;
		}

		public RewardHeartPiece(string id) : base(id) {
			Sprite			= GameData.SPR_REWARD_HEART_PIECE;
			Message			= GameSettings.HEART_PIECE_TEXT;
			HoldType		= RewardHoldTypes.TwoHands;
			HasDuration		 = false;
			ShowMessageOnPickup	= true;
			InteractWithWeapons	= false;
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void OnDisplayMessage() {
			if (GameControl.Inventory.PiecesOfHeart == 3) {
				GameControl.DisplayMessage(
					Message,
					null,
					new RoomStateAction(() => {
						IncrementPiecesOfHeart();
					}),
					new RoomStateTextReader(GameSettings.HEART_CONTAINER_TEXT, null)
				);
			}
			else {
				GameControl.DisplayMessage(
					Message,
					null,
					new RoomStateAction(() => {
						IncrementPiecesOfHeart();
					})
				);
			}
		}

		public override void OnCollectNoMessage() {
			IncrementPiecesOfHeart();
		}


		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------

		private void IncrementPiecesOfHeart() {
			Inventory.PiecesOfHeart++;
			if (Inventory.PiecesOfHeart == 4) {
				AudioSystem.PlaySound(GameData.SOUND_HEART_CONTAINER);
				Inventory.PiecesOfHeart = 0;
				Player.MaxHealth += 4;
				Player.Health = Player.MaxHealth;
			}
		}
	}
}
