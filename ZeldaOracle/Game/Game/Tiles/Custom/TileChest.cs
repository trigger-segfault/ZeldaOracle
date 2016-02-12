using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Scripting;
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
					
					AudioSystem.PlaySound(GameData.SOUND_CHEST_OPEN);
					Properties.Set("looted", true);
					Properties.Set("enabled", true); // Opened chest are always spawned.

					//Properties.SetBase("looted", true);
					//Properties.SetBase("enabled", true); // Opened chest are always spawned.
					CustomSprite = SpriteList[1];
				}
				else {
					RoomControl.GameControl.DisplayMessage("It won't open from this side!");
				}
				return true;
			}
			return false;
		}

		public override void OnInitialize() {
			base.OnInitialize();
			
			CustomSprite = SpriteList[IsLooted ? 1 : 0];
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public bool IsLooted {
			get { return Properties.GetBoolean("looted", false); }
		}
	}
}
