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

namespace ZeldaOracle.Game.Items.Rewards.Health {
	public class RewardHeartPiece : Reward {

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public RewardHeartPiece() {
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
		// Static Methods
		//-----------------------------------------------------------------------------

		/// <summary>Initializes the properties for the reward type.</summary>
		public static void InitializeRewardData(RewardData data) {
			data.HoldInChest		= true;
			data.HoldType			= RewardHoldTypes.TwoHands;
			data.HasDuration		= false;
			data.ShowPickupMessage	= true;
			data.WeaponInteract		= false;
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
