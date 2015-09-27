using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.GameStates;
using ZeldaOracle.Game.GameStates.RoomStates;
using ZeldaOracle.Game.Items.Rewards;

namespace ZeldaOracle.Game.Tiles.Custom {
	public class TileReward : Tile {

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public TileReward() { }


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		// Called when the player touches any part of the tile area.
		public override void OnTouch() {
			if (RoomControl.Player.IsOnGround) {
				// Give the player the reward.
				string rewardName = Properties.GetString("reward", "rupees_1");
				Reward reward = RoomControl.GameControl.RewardManager.GetReward(rewardName);
				RoomControl.GameControl.PushRoomState(new RoomStateReward(reward));
				BaseProperties.Set("looted", true);
				// Remove the useless tile.
				RoomControl.RemoveTile(this);
			}
		}

		public override void Initialize() {
			base.Initialize();

			if (Properties.GetBoolean("looted", false)) {
				RoomControl.RemoveTile(this);
			}
			else {
				// Change the sprite to the reward.
				string rewardName = Properties.GetString("reward", "rupees_1");
				Reward reward = RoomControl.GameControl.RewardManager.GetReward(rewardName);
				AnimationPlayer.Animation = reward.Animation;
			}
		}

	}
}
