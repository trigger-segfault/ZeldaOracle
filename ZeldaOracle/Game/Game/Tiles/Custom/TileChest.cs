﻿using System;
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

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public TileChest() { }


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------
		
		// Called when the player presses A on this tile, when facing the given direction.
		public override bool OnAction(int direction) {
			if (!Properties.GetBoolean("looted", false)) {
				if (direction == Directions.Up) {
					string rewardName = Properties.GetString("reward", "rupees_1");
					Reward reward = RoomControl.GameControl.RewardManager.GetReward(rewardName);
					RoomControl.GameControl.PushRoomState(new RoomStateReward(reward, (Point2I)Position));
					
					
					// TODO: Play chest open sound
					Properties.SetBase("looted", true);
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

			// TODO: Make open sprite a property
			/*if (Properties.GetBoolean("looted", false))
				Properties.Set("sprite_index", 1);
			else
				Properties.Set("sprite_index", 0);*/
		}

	}
}
