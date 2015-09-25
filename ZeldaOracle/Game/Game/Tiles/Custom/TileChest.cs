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
	public class TileChest : Tile {

		private Reward reward;
		private bool opened;
		private Sprite openedSprite;

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public TileChest() {
			this.Sprite = new Sprite(GameData.SHEET_ZONESET_LARGE, 9, 8);
			this.opened	= false;
			this.openedSprite = new Sprite(GameData.SHEET_ZONESET_LARGE, 10, 8);
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
					// Play chest open sound
				}
				else
					RoomControl.GameControl.DisplayMessage("You won't open from this side!");
				return true;
			}
			return false;
		}

		public override void Initialize() {
			base.Initialize();
			this.reward = RoomControl.GameControl.RewardManager.GetReward("item_sword_1");
		}

	}
}
