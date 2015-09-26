using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Properties;
using ZeldaOracle.Game.GameStates;
using ZeldaOracle.Game.GameStates.RoomStates;
using ZeldaOracle.Game.Items.Rewards;

namespace ZeldaOracle.Game.Tiles.Custom {
	public class TileChest : Tile {

		private Reward reward;
		private bool opened;
		private Sprite openedSprite;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public TileChest() {
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------
		
		// Called when the player presses A on this tile, when facing the given direction.
		public override bool OnAction(int direction) {
			if (!opened) {
				if (direction == Directions.Up) {
					RoomControl.GameControl.PushRoomState(new RoomStateReward(reward, (Point2I)Position));
					opened = true;
					Sprite = openedSprite;
					// TODO: Play chest open sound
					BaseProperties.Set("opened", true);
				}
				else {
					RoomControl.GameControl.DisplayMessage("It won't open from this side!");
				}
				return true;
			}
			return false;
		}

		public override void Initialize() {
			base.Initialize();

			string rewardName = properties.GetString("reward", "rupees_1");
			reward = RoomControl.GameControl.RewardManager.GetReward(rewardName);
			
			opened = Properties.GetBoolean("opened", false);
			openedSprite = GameData.SPR_TILE_CHEST_OPEN;

			if (opened)
				Sprite = openedSprite;
			else
				Sprite = GameData.SPR_TILE_CHEST;
		}

	}
}
